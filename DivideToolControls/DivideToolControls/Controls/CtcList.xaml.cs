using DivideToolControls.Helper;
using DivideToolControls.Model;
using DivideToolControls.WinCtls;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DivideToolControls.Controls
{
    /// <summary>
    /// CtcList.xaml 的交互逻辑
    /// </summary>
    public partial class CtcList : UserControl
    {
        private SqlOperator op;

        public List<CtcVo> listvo;

        private int ACount;

        private int BCount = 1;

        private int CCount = 2;

        public List<CtcVo> CheckList = new List<CtcVo>();

        public IMAGE_INFO_STRUCT InfoStruct;

        public int SlideZoom;

        public event RoutedEventHandler AddRefresh;

        public event RoutedEventHandler DeleteRefresh;

        public CtcList(string path)
        {
            InitializeComponent();
            op = new SqlOperator(SqlModeType.sqllite, path);
            BinData();
            txt_Name.Text = "王某";
            txt_Sex.Text = "男";
            txt_Age.Text = "46";
            txt_Hos.Text = "088";
            txt_Bq.Text = "101";
            txt_Bed.Text = "808";
        }

        public int AddData(int x, int y, int width, int height, double size)
        {
            string cmdText = "insert into RegUnitInfo(size,GlobalPosX,GlobalPosY,Width,Height) Values('" + size + "','" + x + "','" + y + "','" + width + "','" + height + "')";
            op.ExecuteNonQuery(cmdText, CommandType.Text, null);
            DataTable dataTable = op.ExecuteDataTable("select id,size,GlobalPosX,GlobalPosY,Width,Height from RegUnitInfo", CommandType.Text, null);
            CtcVo ctcVo = new CtcVo();
            ctcVo.index = dataTable.Rows[dataTable.Rows.Count - 1]["id"].ToString();
            ctcVo.GlobalPosX = x;
            ctcVo.GlobalPosY = y;
            ctcVo.Width = width;
            ctcVo.Height = height;
            ctcVo.Size = Math.Round(size, 2).ToString();
            listvo.Add(ctcVo);
            DgList.ItemsSource = null;
            DgList.ItemsSource = listvo;
            return int.Parse(ctcVo.index);
        }

        private void DgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        public void BinData()
        {
            DataTable dataTable = op.ExecuteDataTable("select id,size,GlobalPosX,GlobalPosY,Width,Height from RegUnitInfo", CommandType.Text, null);
            listvo = new List<CtcVo>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                CtcVo ctcVo = new CtcVo();
                ctcVo.index = dataTable.Rows[i]["id"].ToString();
                ctcVo.ImageSource = null;
                ctcVo.GlobalPosX = int.Parse(dataTable.Rows[i]["GlobalPosX"].ToString());
                ctcVo.GlobalPosY = int.Parse(dataTable.Rows[i]["GlobalPosY"].ToString());
                ctcVo.Width = int.Parse(dataTable.Rows[i]["Width"].ToString());
                ctcVo.Height = int.Parse(dataTable.Rows[i]["Height"].ToString());
                ctcVo.Size = dataTable.Rows[i]["Size"].ToString();
                ctcVo.Diagnosis = "无诊断";
                listvo.Add(ctcVo);
            }
            DgList.ItemsSource = listvo;
        }

        public void GetChPdf(string templatePath, string newFilePath, Dictionary<string, string> parameters)
        {
            PdfReader pdfReader = new PdfReader(templatePath);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFilePath, FileMode.Create));
            AcroFields acroFields = pdfStamper.AcroFields;
            BaseFont value = BaseFont.CreateFont("C:\\Windows\\Fonts\\simsun.ttc,0", "Identity-H", true);
            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                acroFields.SetFieldProperty(parameter.Key, "textfont", value, null);
                acroFields.SetField(parameter.Key, parameter.Value);
            }
            for (int i = 0; i < CheckList.Count; i++)
            {
                int nDataLength = 0;
                DllImageFuc.GetImageDataRoiFunc(InfoStruct, SlideZoom, CheckList[i].GlobalPosX, CheckList[i].GlobalPosY, CheckList[i].Width, CheckList[i].Height, out IntPtr datas, ref nDataLength, true);
                byte[] array = new byte[nDataLength];
                if (datas != IntPtr.Zero)
                {
                    Marshal.Copy(datas, array, 0, nDataLength);
                }
                DllImageFuc.DeleteImageDataFunc(datas);
                iTextSharp.text.Image instance = iTextSharp.text.Image.GetInstance(array);
                instance.ScaleAbsoluteWidth(40f);
                instance.ScaleAbsoluteHeight(40f);
                instance.SetAbsolutePosition(40 + i * 50 + 10, 400f);
                PdfContentByte overContent = pdfStamper.GetOverContent(1);
                overContent.AddImage(instance);
            }
            pdfStamper.FormFlattening = true;
            pdfStamper.Close();
            pdfReader.Close();
        }

        public MemoryStream ReizeImage(Stream ss, double Bl)
        {
            System.Drawing.Image image = new Bitmap(ss);
            double num = 0.0;
            int num2 = 0;
            int num3 = 0;
            if (image.Width > image.Height)
            {
                num2 = (int)Bl;
                num = Bl / (double)image.Width;
                num3 = (int)((double)image.Height * num);
            }
            else
            {
                num3 = (int)Bl;
                num = Bl / (double)image.Height;
                num2 = (int)((double)image.Width * num);
            }
            Bitmap bitmap = new Bitmap(image, new System.Drawing.Size(num2, num3));
            MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Bmp);
            return memoryStream;
        }

        private void Generatepdf_Click(object sender, RoutedEventArgs e)
        {
            string templatePath = AppDomain.CurrentDomain.BaseDirectory + "ctc\\报告模板.pdf";
            string text = AppDomain.CurrentDomain.BaseDirectory + "ctc\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".pdf";
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("txtNo", DateTime.Now.ToString("yyyyMMddhh"));
            dictionary.Add("txtName", txt_Name.Text);
            dictionary.Add("txtSex", txt_Sex.Text);
            dictionary.Add("txtAge", txt_Age.Text);
            dictionary.Add("txtHospital", txt_Hos.Text);
            dictionary.Add("txtDate", DateTime.Now.ToString());
            dictionary.Add("txtdia", txt_diagnosis.Text);
            dictionary.Add("txtBed", txt_Bq.Text);
            dictionary.Add("txtzj", "测试专家");
            GetChPdf(templatePath, text, dictionary);
            Process.Start(text);
        }

        private void dlook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string str = ((Button)sender).Tag.ToString();
                DataTable dataTable = op.ExecuteDataTable("select * from RegUnitInfo where id='" + str + "'", CommandType.Text, null);
                CtcViewer ctcViewer = new CtcViewer();
                ctcViewer.MaxCount = -1;
                for (int i = 0; i < 10; i++)
                {
                    if (dataTable.Rows[0]["DImageData" + i].ToString() != "")
                    {
                        byte[] buffer = (byte[])dataTable.Rows[0]["DImageData" + i];
                        MemoryStream streamSource = new MemoryStream(buffer);
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                        bitmapImage.CacheOption = BitmapCacheOption.None;
                        bitmapImage.StreamSource = streamSource;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();
                        ctcViewer._listbit.Add(bitmapImage);
                        ctcViewer.MaxCount++;
                    }
                }
                ctcViewer.MaxScale = 40;
                ctcViewer.lbl_Scale.Content = 40 + "X";
                ctcViewer.ctc_img.Source = ctcViewer._listbit[0];
                ctcViewer.x3dSlider.Visibility = Visibility.Visible;
                ctcViewer.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ctcViewer.ShowDialog();
            }
            catch (Exception)
            {
                MessageBox.Show("数据不存在或者转换错误！");
            }
        }

        private void glook_Click_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string str = ((Button)sender).Tag.ToString();
                DataTable dataTable = op.ExecuteDataTable("select * from RegUnitInfo where id='" + str + "'", CommandType.Text, null);
                byte[] buffer = (byte[])dataTable.Rows[0]["HImageData"];
                MemoryStream streamSource = new MemoryStream(buffer);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bitmapImage.CacheOption = BitmapCacheOption.None;
                bitmapImage.StreamSource = streamSource;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                CtcViewer ctcViewer = new CtcViewer();
                ctcViewer.x3dSlider.Visibility = Visibility.Collapsed;
                ctcViewer.ctc_img.Source = bitmapImage;
                ctcViewer.MaxScale = 80;
                ctcViewer.lbl_Scale.Content = 80 + "X";
                ctcViewer.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ctcViewer.ShowDialog();
            }
            catch (Exception)
            {
                MessageBox.Show("数据不存在或者转换错误！");
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (DgList.SelectedItem == null)
            {
                return;
            }
            RadioButton radioButton = (RadioButton)sender;
            CtcVo ctcVo = (CtcVo)DgList.SelectedItem;
            if (CheckList.Contains(ctcVo))
            {
                int index = CheckList.IndexOf(ctcVo);
                CheckList[index].CheckValue = radioButton.Tag.ToString();
            }
            else
            {
                ctcVo.CheckValue = radioButton.Tag.ToString();
                CheckList.Add(ctcVo);
            }
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < CheckList.Count; i++)
            {
                string checkValue = CheckList[i].CheckValue;
                if (checkValue == "1")
                {
                    num++;
                }
                else if (checkValue == "2")
                {
                    num2++;
                }
                else
                {
                    CheckList.Remove(ctcVo);
                }
            }
            txt_qzsl.Text = num2.ToString();
            txt_xssl.Text = num.ToString();
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            string text = ((Button)sender).Tag.ToString();
            op.ExecuteNonQuery("delete from RegUnitInfo where id='" + text + "'", CommandType.Text, null);
            listvo.Remove((CtcVo)DgList.SelectedItem);
            DgList.Items.Refresh();
            if (this.DeleteRefresh != null)
            {
                this.DeleteRefresh(text, e);
            }
        }
    }
}

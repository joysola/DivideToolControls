using DivideToolControls.AnnotationControls;
using DivideToolControls.Controls;
using DivideToolControls.DeepZoom;
using DivideToolControls.DeepZoomControls;
using DivideToolControls.DynamicGeometry.Enum;
using DivideToolControls.Model;
using DivideToolControls.WinCtls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Xceed.Wpf.AvalonDock.Layout;

namespace DivideToolControls.Helper
{
    public class AnnoWindHelper
    {
        public static AnnoWindHelper Instance { get; } = new AnnoWindHelper();
        public Dictionary<int, AnnoDIYCtcRect> AnnoDiyCtcRectDic { get; } = new Dictionary<int, AnnoDIYCtcRect>();

        public Visibility isAnnoManageWind = Visibility.Collapsed;

        private MultiScaleImage msi = ZoomModel.MulScaImg;

        private List<AnnoTmaRect> tmaObjList = new List<AnnoTmaRect>();


        public void InitAnnoWinRegisterEvents()
        {
            ZoomModel.AnnoListWind = new AnnoListWind();
            ZoomModel.AnnoListWind.Owner = Application.Current.MainWindow;
            ZoomModel.AnnoListWind.txt_xbz.TextChanged += mc_TextChanged;
            ZoomModel.AnnoListWind.cbo_mc.SelectionChanged += cbo_mcSelectionChanged;
            ZoomModel.AnnoListWind.txt_qsr.TextChanged += txt_qsrTextChanged;
            ZoomModel.AnnoListWind.btnyc.MouseLeftButtonDown += btnyc_change;
            ZoomModel.AnnoListWind.btnqyc.MouseLeftButtonDown += allhidden_change;
            ZoomModel.AnnoListWind.ckb_clinfo.Click += ckb_clinfo;
            ZoomModel.AnnoListWind.btnsc.MouseLeftButtonDown += DeleteItem;
            ZoomModel.AnnoListWind.btnallsc.MouseLeftButtonDown += btnallsc_MouseLeftButtonDown;
            ZoomModel.AnnoListWind.cbo_mc.DropDownOpened += DropDownOpened;
            ZoomModel.AnnoListWind.cbo_mc.DropDownClosed += DropDownClosed;
            ZoomModel.AnnoListWind.LineWidthComboBox.SelectionChanged += LineWidthComboBox_SelectionChanged;
            ZoomModel.AnnoListWind._colorPicker.SelectedColorChanged += ColorPicker_SelectedColorChanged;
            ZoomModel.AnnoListWind.Rad_1.Checked += Rad_Checked;
            ZoomModel.AnnoListWind.Rad_2.Checked += Rad_Checked;
            ZoomModel.AnnoListWind.Rad_3.Checked += Rad_Checked;
            ZoomModel.AnnoListWind.Rad_4.Checked += Rad_Checked;
            ZoomModel.AnnoListWind.ShowMs.Click += ShowMs_Click;
            ZoomModel.AnnoListWind.All_ClShow.Click += All_ClShow_Click;
            ZoomModel.AnnoListWind.All_ClHidden.Click += All_ClHidden_Click;
            ZoomModel.AnnoListWind.All_MsShow.Click += All_MsShow_Click;
            ZoomModel.AnnoListWind.All_MsHidden.Click += All_MsHidden_Click;
            ZoomModel.AnnoListWind.CloseHandler += AnnoListWind_CloseHandler;
            ZoomModel.AnnoWind = new AnnoWind();
            ZoomModel.AnnoWind.Owner = Application.Current.MainWindow;
            //ZoomModel.AnnoWind._SaveAnno.Click += SaveAnno_Click;
            //ZoomModel.AnnoWind._CancelAnno.Click += CancelAnno_Click;
        }

        private void mc_TextChanged(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ZoomModel.AnnoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                ZoomModel.ObjList[selectedIndex].AnnotationName = ZoomModel.AnnoListWind.txt_xbz.Text;
                ZoomModel.AnnoListWind.cbo_mc.ItemsSource = null;
                ZoomModel.AnnoListWind.cbo_mc.ItemsSource = ZoomModel.ObjList;
                ZoomModel.AnnoListWind.cbo_mc.SelectedIndex = selectedIndex;
            }
        }
        private void cbo_mcSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = ZoomModel.AnnoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex == -1)
            {
                return;
            }
            foreach (AnnoBase item in ZoomModel.ObjList)
            {
                item.IsActive(Visibility.Collapsed);
            }
            ZoomModel.AnnoListWind.txt_qsr.Text = ZoomModel.ObjList[selectedIndex].AnnotationDescription;
            ZoomModel.AnnoListWind.txt_xbz.Text = ZoomModel.ObjList[selectedIndex].AnnotationName;
            ZoomModel.AnnoListWind.tbk_info.Text = ZoomModel.ObjList[selectedIndex].CalcMeasureInfo1();
            ZoomModel.AnnoListWind.ckb_clinfo.IsChecked = ZoomModel.ObjList[selectedIndex].isVisble == Visibility.Visible ? true : false;
            ZoomModel.AnnoListWind.ShowMs.IsChecked = ZoomModel.ObjList[selectedIndex].isMsVisble;
            ZoomModel.AnnoListWind._colorPicker.SelectedColor = ((SolidColorBrush)ZoomModel.ObjList[selectedIndex].BorderBrush).Color;
            ZoomModel.AnnoListWind.LineWidthComboBox.SelectedValue = ZoomModel.ObjList[selectedIndex].Size;
            foreach (ComboBoxItem item2 in ZoomModel.AnnoListWind.LineWidthComboBox.Items)
            {
                if (item2.Content.Equals(ZoomModel.ObjList[selectedIndex].Size.ToString()))
                {
                    ZoomModel.AnnoListWind.LineWidthComboBox.SelectedItem = item2;
                    break;
                }
            }
            if (ZoomModel.ObjList[selectedIndex].isHidden == Visibility.Visible)
            {
                ZoomModel.ObjList[selectedIndex].IsActive(Visibility.Visible);
            }
            if (ZoomModel.ObjList[selectedIndex].isHidden == Visibility.Visible)
            {
                ZoomModel.AnnoListWind.btnyc.Opacity = 1.0;
                ZoomModel.AnnoListWind.btnyc.SetValue(ToolTipService.ToolTipProperty, "隐藏");
                ZoomModel.ObjList[selectedIndex].isHidden = Visibility.Visible;
            }
            else
            {
                ZoomModel.AnnoListWind.btnyc.Opacity = 0.5;
                ZoomModel.AnnoListWind.btnyc.SetValue(ToolTipService.ToolTipProperty, "显示");
                ZoomModel.ObjList[selectedIndex].isHidden = Visibility.Collapsed;
            }
            if (ZoomModel.ObjList[selectedIndex].isFinish && ZoomModel.IsDrop)
            {
                dw(sender, new RoutedEventArgs());
            }
            if (ZoomModel.ObjList[selectedIndex].GetType() == typeof(AnnoPin))
            {
                SetAnnoListRadioButton(true);
                string a = ZoomModel.ObjList[selectedIndex].PinType.Substring(12, 1);
                if (a == "1")
                {
                    ZoomModel.AnnoListWind.Rad_1.IsChecked = true;
                }
                else if (a == "2")
                {
                    ZoomModel.AnnoListWind.Rad_2.IsChecked = true;
                }
                else if (a == "3")
                {
                    ZoomModel.AnnoListWind.Rad_3.IsChecked = true;
                }
                else if (a == "4")
                {
                    ZoomModel.AnnoListWind.Rad_4.IsChecked = true;
                }
            }
            else
            {
                SetAnnoListRadioButton(false);
            }
            ReDraw();
        }
        private void txt_qsrTextChanged(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ZoomModel.AnnoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                ZoomModel.ObjList[selectedIndex].AnnotationDescription = ZoomModel.AnnoListWind.txt_qsr.Text;
                ZoomModel.ObjList[selectedIndex].UpadteTextBlock();
            }
        }
        private void btnyc_change(object sender, RoutedEventArgs e)
        {
            int num = 0;
            int selectedIndex = ZoomModel.AnnoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex == -1)
            {
                return;
            }
            if (ZoomModel.AnnoListWind.btnyc.Opacity == 0.5)
            {
                ZoomModel.ObjList[selectedIndex].IsActive(Visibility.Visible);
                if (ZoomModel.AnnoListWind.ckb_clinfo.IsChecked == true)
                {
                    ZoomModel.ObjList[selectedIndex].isVisble = Visibility.Visible;
                }
                ZoomModel.ObjList[selectedIndex].isHidden = Visibility.Visible;
                foreach (AnnoBase item in ZoomModel.ObjList)
                {
                    if (item.isHidden == Visibility.Visible)
                    {
                        num++;
                        if (num == ZoomModel.ObjList.Count)
                        {
                            ZoomModel.AnnoListWind.btnqyc.SetValue(ToolTipService.ToolTipProperty, "全部隐藏");
                            ZoomModel.AnnoListWind.btnqyc.Opacity = 1.0;
                        }
                    }
                }
                ZoomModel.AnnoListWind.btnyc.Opacity = 1.0;
                ZoomModel.AnnoListWind.btnyc.SetValue(ToolTipService.ToolTipProperty, "隐藏");
            }
            else
            {
                num = 0;
                ZoomModel.AnnoListWind.btnyc.SetValue(ToolTipService.ToolTipProperty, "显示");
                ZoomModel.ObjList[selectedIndex].IsActive(Visibility.Collapsed);
                ZoomModel.ObjList[selectedIndex].isVisble = Visibility.Collapsed;
                ZoomModel.ObjList[selectedIndex].isHidden = Visibility.Collapsed;
                ZoomModel.AnnoListWind.btnyc.Opacity = 0.5;
                foreach (AnnoBase item2 in ZoomModel.ObjList)
                {
                    if (item2.isHidden == Visibility.Collapsed)
                    {
                        num++;
                        if (num == ZoomModel.ObjList.Count)
                        {
                            ZoomModel.AnnoListWind.btnqyc.SetValue(ToolTipService.ToolTipProperty, "全部显示");
                            ZoomModel.AnnoListWind.btnqyc.Opacity = 0.5;
                        }
                    }
                }
            }
            ReDraw();
        }
        private void allhidden_change(object sender, RoutedEventArgs e)
        {
            if (ZoomModel.AnnoListWind.btnqyc.Opacity == 1.0)
            {
                foreach (AnnoBase item in ZoomModel.ObjList)
                {
                    item.isHidden = Visibility.Collapsed;
                    item.IsActive(Visibility.Collapsed);
                    item.MTextBlock.Visibility = Visibility.Collapsed;
                    item.UpdateVisual();
                }
                ZoomModel.AnnoListWind.cbo_mc.SelectedIndex = -1;
                ZoomModel.AnnoListWind.txt_qsr.Text = "";
                ZoomModel.AnnoListWind.txt_xbz.Text = "";
                ZoomModel.AnnoListWind.tbk_info.Text = "";
                ZoomModel.AnnoListWind.btnyc.Opacity = 0.5;
                ZoomModel.AnnoListWind.btnqyc.Opacity = 0.5;
                ZoomModel.AnnoListWind.btnyc.SetValue(ToolTipService.ToolTipProperty, "显示");
                ZoomModel.AnnoListWind.btnqyc.SetValue(ToolTipService.ToolTipProperty, "全部显示");
            }
            else
            {
                ZoomModel.AnnoListWind.btnyc.SetValue(ToolTipService.ToolTipProperty, "隐藏");
                ZoomModel.AnnoListWind.btnqyc.SetValue(ToolTipService.ToolTipProperty, "全部隐藏");
                foreach (AnnoBase item2 in ZoomModel.ObjList)
                {
                    item2.isHidden = Visibility.Visible;
                    item2.isVisble = Visibility.Visible;
                    item2.MTextBlock.Visibility = Visibility.Visible;
                    item2.UpdateVisual();
                }
                ZoomModel.AnnoListWind.btnyc.Opacity = 1.0;
                ZoomModel.AnnoListWind.btnqyc.Opacity = 1.0;
            }
        }
        private void ckb_clinfo(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ZoomModel.AnnoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                if (ZoomModel.AnnoListWind.ckb_clinfo.IsChecked == true && ZoomModel.ObjList[selectedIndex].isHidden == Visibility.Visible)
                {
                    ZoomModel.ObjList[selectedIndex].isVisble = Visibility.Visible;
                }
                else
                {
                    ZoomModel.ObjList[selectedIndex].isVisble = Visibility.Collapsed;
                }
                ReDraw();
            }
        }
        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            Delete();
            AnnoTmaRec_FinishEvent(null, null);
        }
        public void Delete()
        {
            if (ZoomModel.AnnoListWind.cbo_mc.SelectedIndex != -1)
            {
                object obj = ZoomModel.ObjList[ZoomModel.AnnoListWind.cbo_mc.SelectedIndex];
                ((AnnoBase)obj).DeleteItem();
                ZoomModel.AnnoListWind.cbo_mc.ItemsSource = null;
                ZoomModel.AnnoListWind.cbo_mc.ItemsSource = ZoomModel.ObjList;
                if (ZoomModel.ObjList.Count == 0)
                {
                    ZoomModel.AnnoListWind.txt_qsr.Text = "";
                    ZoomModel.AnnoListWind.txt_xbz.Text = "";
                    ZoomModel.AnnoListWind.tbk_info.Text = "";
                }
                else
                {
                    ZoomModel.AnnoListWind.cbo_mc.SelectedIndex = -1;
                }
            }
        }
        private void AnnoTmaRec_FinishEvent(object sender, MouseEventArgs e)
        {
            if (tmaObjList.Count == 0)
            {
                return;
            }
            List<AnnoTmaRect> list = new List<AnnoTmaRect>();
            List<AnnoTmaRect> list2 = tmaObjList.OrderBy(s => s.CurrentStart.Y * ZoomModel.Curscale).ToList();
            Dictionary<int, List<AnnoTmaRect>> dictionary = new Dictionary<int, List<AnnoTmaRect>>();
            List<AnnoTmaRect> list3 = new List<AnnoTmaRect>();
            double num = Math.Abs(tmaObjList[0].CurrentEnd.Y - tmaObjList[0].CurrentStart.Y) / 3.0;
            int num2 = 0;
            for (int i = 0; i < list2.Count; i++)
            {
                if (i < list2.Count - 1)
                {
                    double y = list2[i].CurrentEnd.Y;
                    double y2 = list2[i + 1].CurrentEnd.Y;
                    double num3 = Math.Abs(y - y2);
                    if (num3 > num)
                    {
                        list3.Add(list2[i]);
                        dictionary.Add(num2, list3);
                        num2++;
                        list3 = new List<AnnoTmaRect>();
                    }
                    else
                    {
                        list3.Add(list2[i]);
                    }
                }
                else
                {
                    list3.Add(list2[i]);
                    dictionary.Add(num2, list3);
                }
            }
            foreach (KeyValuePair<int, List<AnnoTmaRect>> item in dictionary)
            {
                List<AnnoTmaRect> collection = item.Value.OrderBy(s => s.CurrentStart.X * ZoomModel.Curscale).ToList();
                list.AddRange(collection);
            }
            Mi_TMAClear_Click(null, null);
            tmaObjList.Clear();
            for (int num4 = list.Count - 1; num4 >= 0; num4--)
            {
                AnnoTmaRect myTmaRectangle = list[num4];
                double x = myTmaRectangle.MsiToCanvas(myTmaRectangle.CurrentStart).X;
                double y3 = myTmaRectangle.MsiToCanvas(myTmaRectangle.CurrentStart).Y;
                double x2 = myTmaRectangle.MsiToCanvas(myTmaRectangle.CurrentEnd).X;
                double y4 = myTmaRectangle.MsiToCanvas(myTmaRectangle.CurrentEnd).Y;
                double num5 = Math.Abs(x - x2);
                double num6 = Math.Abs(y3 - y4);
                x = Math.Min(x, x2);
                y3 = Math.Min(y3, y4);
                double left = x;
                double top = y3;
                double width = num5;
                double height = num6;
                AnnoTmaRect myTmaRectangle2 = new AnnoTmaRect(ZoomModel.ALC, ZoomModel.Canvasboard, msi, ZoomModel.ObjList, ZoomModel.SlideZoom, ZoomModel.Calibration, tmaObjList);
                myTmaRectangle2.DrawRect(left, top, width, height, (num4 + 1).ToString(), myTmaRectangle.AnnotationDescription);
            }
            MulScanImgHelper.Instance.RegisterMsiEvents(); //
            ZoomModel.Nav.IsHitTestVisible = true;
            ZoomModel.AnnoListWind.cbo_mc.SelectedIndex = -1;
        }
        private void Mi_TMAClear_Click(object sender, RoutedEventArgs e)
        {
            List<AnnoBase> list = new List<AnnoBase>();
            foreach (AnnoBase item in ZoomModel.ObjList)
            {
                if (item.AnnotationType == AnnotationType.myTmaLine || item.AnnotationType == AnnotationType.TmaRectangle)
                {
                    list.Add(item);
                }
            }
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                list[i].DeleteItem();
            }
            list.Clear();
        }
        private void btnallsc_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int count = ZoomModel.ObjList.Count;
            for (int i = 0; i < count; i++)
            {
                ZoomModel.ObjList[0].DeleteItem();
            }
            ZoomModel.AnnoListWind.cbo_mc.SelectedIndex = -1;
            ZoomModel.AnnoListWind.cbo_mc.ItemsSource = null;
            ZoomModel.AnnoListWind.cbo_mc.ItemsSource = ZoomModel.ObjList;
        }

        public void DropDownOpened(object sender, EventArgs e)
        {
            ZoomModel.IsDrop = true;
        }

        public void DropDownClosed(object sender, EventArgs e)
        {
            ZoomModel.IsDrop = false;
        }
        private void LineWidthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ZoomModel.AnnoListWind.LineWidthComboBox.SelectedItem != null)
            {
                int selectedIndex = ZoomModel.AnnoListWind.cbo_mc.SelectedIndex;
                if (selectedIndex != -1)
                {
                    ZoomModel.ObjList[selectedIndex].Size = int.Parse((ZoomModel.AnnoListWind.LineWidthComboBox.SelectedItem as ComboBoxItem).Content.ToString());
                    ZoomModel.ObjList[selectedIndex].UpdateVisual();
                }
            }
        }
        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            int selectedIndex = ZoomModel.AnnoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                ZoomModel.ObjList[selectedIndex].BorderBrush = new SolidColorBrush(ZoomModel.AnnoListWind._colorPicker.SelectedColor);
                if (ZoomModel.ObjList[selectedIndex].GetType() == typeof(AnnoPin))
                {
                    ZoomModel.ObjList[selectedIndex].FontColor = new SolidColorBrush(ZoomModel.AnnoListWind._colorPicker.SelectedColor);
                }
                ZoomModel.ObjList[selectedIndex].UpdateVisual();
            }
        }
        private void Rad_Checked(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ZoomModel.AnnoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                string a = ((System.Windows.Controls.RadioButton)sender).Name.ToString();
                if (a == "Rad_1")
                {
                    ZoomModel.ObjList[selectedIndex].PinType = "images/pin_1.png";
                }
                else if (a == "Rad_2")
                {
                    ZoomModel.ObjList[selectedIndex].PinType = "images/pin_2.png";
                }
                else if (a == "Rad_3")
                {
                    ZoomModel.ObjList[selectedIndex].PinType = "images/pin_3.png";
                }
                else if (a == "Rad_4")
                {
                    ZoomModel.ObjList[selectedIndex].PinType = "images/pin_4.png";
                }
                ZoomModel.ObjList[selectedIndex].UpdateVisual();
            }
        }
        private void ShowMs_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ZoomModel.AnnoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                if (ZoomModel.AnnoListWind.ShowMs.IsChecked == true)
                {
                    ZoomModel.ObjList[selectedIndex].isMsVisble = true;
                }
                else
                {
                    ZoomModel.ObjList[selectedIndex].isMsVisble = false;
                }
                ZoomModel.ObjList[selectedIndex].UpdateVisual();
            }
        }

        private void All_ClShow_Click(object sender, RoutedEventArgs e)
        {
            foreach (AnnoBase item in ZoomModel.ObjList)
            {
                item.isVisble = Visibility.Visible;
                item.UpdateVisual();
            }
            if (ZoomModel.AnnoListWind.cbo_mc.SelectedIndex != -1)
            {
                ZoomModel.AnnoListWind.ckb_clinfo.IsChecked = true;
            }
        }
        private void All_ClHidden_Click(object sender, RoutedEventArgs e)
        {
            foreach (AnnoBase item in ZoomModel.ObjList)
            {
                item.isVisble = Visibility.Collapsed;
                item.UpdateVisual();
            }
            ZoomModel.AnnoListWind.ckb_clinfo.IsChecked = false;
        }
        private void All_MsShow_Click(object sender, RoutedEventArgs e)
        {
            foreach (AnnoBase item in ZoomModel.ObjList)
            {
                item.isMsVisble = true;
                item.UpdateVisual();
            }
            if (ZoomModel.AnnoListWind.cbo_mc.SelectedIndex != -1)
            {
                ZoomModel.AnnoListWind.ShowMs.IsChecked = true;
            }
        }
        private void All_MsHidden_Click(object sender, RoutedEventArgs e)
        {
            foreach (AnnoBase item in ZoomModel.ObjList)
            {
                item.isMsVisble = false;
                item.UpdateVisual();
            }
            ZoomModel.AnnoListWind.ShowMs.IsChecked = false;
        }
        private void AnnoListWind_CloseHandler(object sender, RoutedEventArgs e)
        {
            ZoomModel.AnnoListWind.Visibility = Visibility.Collapsed;
            isAnnoManageWind = Visibility.Collapsed;
        }

        private void SaveAnno_Click(object sender, RoutedEventArgs e)
        {
            ZoomModel.AnnoWind._AnnotationBase.Size = int.Parse((ZoomModel.AnnoWind.LineWidthComboBox.SelectedItem as ComboBoxItem).Content.ToString());
            ZoomModel.AnnoWind._AnnotationBase.AnnotationName = ZoomModel.AnnoWind.AnnoName.Text;
            ZoomModel.AnnoWind._AnnotationBase.AnnotationDescription = ZoomModel.AnnoWind.AnnoDes.Text;
            ZoomModel.AnnoWind._AnnotationBase.BorderBrush = new SolidColorBrush(ZoomModel.AnnoWind._colorPicker.SelectedColor);
            ZoomModel.AnnoWind._AnnotationBase.isVisble = ((ZoomModel.AnnoWind.ShowInfo.IsChecked != true) ? Visibility.Collapsed : Visibility.Visible);
            ZoomModel.AnnoWind._AnnotationBase.isMsVisble = ((ZoomModel.AnnoWind.ShowMs.IsChecked == true) ? true : false);
            if (ZoomModel.AnnoWind._AnnotationBase.GetType() == typeof(AnnoPin))
            {
                if (ZoomModel.AnnoWind.Rad_1.IsChecked == true)
                {
                    ZoomModel.AnnoWind._AnnotationBase.PinType = "images/pin_1.png";
                }
                else if (ZoomModel.AnnoWind.Rad_2.IsChecked == true)
                {
                    ZoomModel.AnnoWind._AnnotationBase.PinType = "images/pin_2.png";
                }
                else if (ZoomModel.AnnoWind.Rad_3.IsChecked == true)
                {
                    ZoomModel.AnnoWind._AnnotationBase.PinType = "images/pin_3.png";
                }
                else if (ZoomModel.AnnoWind.Rad_4.IsChecked == true)
                {
                    ZoomModel.AnnoWind._AnnotationBase.PinType = "images/pin_4.png";
                }
                ZoomModel.AnnoWind._AnnotationBase.FontColor = new SolidColorBrush(ZoomModel.AnnoWind._colorPicker.SelectedColor);
            }
            if (ZoomModel.AnnoWind._AnnotationBase.AnnotationType == AnnotationType.DiyCtcRectangle)
            {
                AnnoDIYCtcRect myDiyCtcRectangle = (AnnoDIYCtcRect)ZoomModel.AnnoWind._AnnotationBase;
                System.Windows.Point originStart = myDiyCtcRectangle.OriginStart;
                double num = msi.ZoomableCanvas.Offset.X + originStart.X;
                double num2 = msi.ZoomableCanvas.Offset.Y + originStart.Y;
                double num3 = num * ZoomModel.SlideZoom / ZoomModel.Curscale;
                double num4 = num2 * ZoomModel.SlideZoom / ZoomModel.Curscale;
                double num5 = myDiyCtcRectangle.m_rectangle.Width * ZoomModel.SlideZoom / ZoomModel.Curscale;
                double num6 = myDiyCtcRectangle.m_rectangle.Height * ZoomModel.SlideZoom / ZoomModel.Curscale;
                double size = Math.Sqrt(num5 * num5 + num6 * num6) * ZoomModel.Calibration;
                int num7 = ZoomModel.CtcWind.AddData((int)num3, (int)num4, (int)num5, (int)num6, size);
                AnnoDiyCtcRectDic.Add(num7, myDiyCtcRectangle);
                ZoomModel.AnnoWind._AnnotationBase.FontSize = num7;
                ZoomModel.Nav.IsHitTestVisible = true;
            }
            ZoomModel.AnnoWind._AnnotationBase.UpadteTextBlock();
            ZoomModel.AnnoWind._AnnotationBase.UpdateVisual();
            ZoomModel.AnnoWind.Hide();
        }
        private void CancelAnno_Click(object sender, RoutedEventArgs e)
        {
            ZoomModel.AnnoWind._AnnotationBase.DeleteItem();
            ZoomModel.AnnoListWind.cbo_mc.SelectedIndex = -1;
            ZoomModel.AnnoWind.Hide();
        }

        private void dw(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ZoomModel.AnnoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                AnnoBase annotationBase = ZoomModel.ObjList[selectedIndex];
                if (Setting.IsSynchronous)
                {
                    foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                    {
                        //if ((Mainpage)item.Value != this)
                        //{
                        SynMsiAnno(annotationBase);
                        //}
                    }
                }
                double num = annotationBase.CurrentEnd.X * annotationBase.Zoom - annotationBase.Zoom * annotationBase.CurrentStart.X;
                double num2 = annotationBase.CurrentEnd.Y * annotationBase.Zoom - annotationBase.Zoom * annotationBase.CurrentStart.Y;
                System.Windows.Point point = new System.Windows.Point(annotationBase.Zoom * annotationBase.CurrentStart.X - msi.ActualWidth / 2.0 + num / 2.0, annotationBase.Zoom * annotationBase.CurrentStart.Y - msi.ActualHeight / 2.0 + num2 / 2.0);
                double scale = msi.ZoomableCanvas.Scale;
                int level = msi.Source.GetLevel(annotationBase.Zoom / ZoomModel.SlideZoom);
                int currentLevel = msi._spatialSource.CurrentLevel;
                if (level != currentLevel)
                {
                    msi._spatialSource.CurrentLevel = level;
                }
                if (scale != annotationBase.Zoom / (double)ZoomModel.SlideZoom)
                {
                    double num3 = 4.0;
                    TimeSpan timeSpan = TimeSpan.FromMilliseconds(num3 * 100.0);
                    CubicEase easingFunction = new CubicEase();
                    msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.ScaleProperty, new DoubleAnimation(annotationBase.Zoom / (double)ZoomModel.SlideZoom, timeSpan)
                    {
                        EasingFunction = easingFunction
                    }, HandoffBehavior.Compose);
                    msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.OffsetProperty, new PointAnimation(new System.Windows.Point(point.X, point.Y), timeSpan)
                    {
                        EasingFunction = easingFunction
                    }, HandoffBehavior.Compose);
                }
                else
                {
                    msi.ZoomableCanvas.Offset = new System.Windows.Point(point.X, point.Y);
                    msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                }
            }
        }

        public void SynMsiAnno(AnnoBase ab)
        {
            double num = ab.CurrentEnd.X * ab.Zoom - ab.Zoom * ab.CurrentStart.X;
            double num2 = ab.CurrentEnd.Y * ab.Zoom - ab.Zoom * ab.CurrentStart.Y;
            System.Windows.Point point = new Point(ab.Zoom * ab.CurrentStart.X - msi.ActualWidth / 2.0 + num / 2.0, ab.Zoom * ab.CurrentStart.Y - msi.ActualHeight / 2.0 + num2 / 2.0);
            double scale = msi.ZoomableCanvas.Scale;
            int level = msi.Source.GetLevel(ab.Zoom / (double)ZoomModel.SlideZoom);
            int currentLevel = msi._spatialSource.CurrentLevel;
            if (level != currentLevel)
            {
                msi._spatialSource.CurrentLevel = level;
            }
            if (scale != ab.Zoom / (double)ZoomModel.SlideZoom)
            {
                double num3 = 4.0;
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(num3 * 100.0);
                CubicEase easingFunction = new CubicEase();
                msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.ScaleProperty, new DoubleAnimation(ab.Zoom / (double)ZoomModel.SlideZoom, timeSpan)
                {
                    EasingFunction = easingFunction
                }, HandoffBehavior.Compose);
                msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.OffsetProperty, new PointAnimation(new Point(point.X, point.Y), timeSpan)
                {
                    EasingFunction = easingFunction
                }, HandoffBehavior.Compose);
            }
            else
            {
                msi.ZoomableCanvas.Offset = new System.Windows.Point(point.X, point.Y);
                msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
            }
        }
        public void SetAnnoListRadioButton(bool v)
        {
            ZoomModel.AnnoListWind.Rad_1.IsEnabled = v;
            ZoomModel.AnnoListWind.Rad_2.IsEnabled = v;
            ZoomModel.AnnoListWind.Rad_3.IsEnabled = v;
            ZoomModel.AnnoListWind.Rad_4.IsEnabled = v;
        }
        public void ReDraw()
        {
            foreach (AnnoBase item in ZoomModel.ObjList)
            {
                item.UpdateVisual();
            }
        }

        public void SetAnnoRadioButton(bool v)
        {
            ZoomModel.AnnoWind.Rad_1.IsEnabled = v;
            ZoomModel.AnnoWind.Rad_2.IsEnabled = v;
            ZoomModel.AnnoWind.Rad_3.IsEnabled = v;
            ZoomModel.AnnoWind.Rad_4.IsEnabled = v;
        }

        public void FinishEvent(object sender, MouseEventArgs e)
        {
            MulScanImgHelper.Instance.RegisterMsiEvents(); //
            ZoomModel.AnnoWind._AnnotationBase = (AnnoBase)sender;
            if (ZoomModel.AnnoWind._AnnotationBase.OriginStart == ZoomModel.AnnoWind._AnnotationBase.OriginEnd && ZoomModel.AnnoWind._AnnotationBase.AnnotationType != AnnotationType.Remark)
            {
                ZoomModel.AnnoWind._AnnotationBase.DeleteItem();
                ZoomModel.AnnoWind._AnnotationBase.AnnoControl.CB.SelectedIndex = -1;
            }
            else
            {
                ZoomModel.AnnoWind.AnnoName.Text = ZoomModel.AnnoWind._AnnotationBase.AnnotationName;
                ZoomModel.AnnoWind.AnnoDes.Text = string.Empty;
                ZoomModel.AnnoWind.tbk_info.Text = ZoomModel.AnnoWind._AnnotationBase.CalcMeasureInfo();
                ZoomModel.AnnoWind._colorPicker.SelectedColor = Color.FromArgb(byte.MaxValue, 0, 0, byte.MaxValue);
                ZoomModel.AnnoWind.LineWidthComboBox.SelectedIndex = 1;
                ZoomModel.AnnoWind.Rad_1.IsChecked = true;
                Point mainWindowPoint = GetMainWindowPoint();
                //ZoomModel.AnnoWind.Top = mainWindowPoint.Y + (base.ActualHeight - ZoomModel.AnnoWind.Height) / 2.0;
                //ZoomModel.AnnoWind.Left = mainWindowPoint.X + (base.ActualWidth - ZoomModel.AnnoWind.Width) / 2.0;
                ZoomModel.AnnoWind.Top = mainWindowPoint.Y + (ZoomModel.Bg.ActualHeight - ZoomModel.AnnoWind.Height) / 2.0;
                ZoomModel.AnnoWind.Left = mainWindowPoint.X + (ZoomModel.Bg.ActualWidth - ZoomModel.AnnoWind.Width) / 2.0;
                ZoomModel.AnnoWind.ShowDialog();
            }
            ZoomModel.Nav.IsHitTestVisible = true;
        }

        private Point GetMainWindowPoint()
        {
            Point result = new Point(0.0, 0.0);
            try
            {
                //result = TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0.0, 0.0));
                //result = Application.Current.MainWindow.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0.0, 0.0));
                // ????????????????
                var vec = VisualTreeHelper.GetOffset(Application.Current.MainWindow);
                result = new Point(vec.X, vec.Y);
            }
            catch
            {
                foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                {
                    if (item.Value == this)
                    {
                        result = new Point(((LayoutDocument)item.Key).FloatingLeft, ((LayoutDocument)item.Key).FloatingTop);
                    }
                }
            }
            result.Y += Application.Current.MainWindow.Top;
            result.X += Application.Current.MainWindow.Left;
            return result;
        }
    }
}

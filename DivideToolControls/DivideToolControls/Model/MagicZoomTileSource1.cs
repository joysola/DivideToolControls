using DivideToolControls.DeepZoom;
using DivideToolControls.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DivideToolControls.Model
{
    public class MagicZoomTileSource1 : MultiScaleTileSource
    {
        private IMAGE_INFO_STRUCT InfoStruct;

        private int width;

        private int height;

        private int TileWidth;

        private float Xzoom;

        private MultiScaleImage Msi;

        private new double Angle;

        private int S_shift;

        private int V_shift;

        private int b;

        private int b_r;

        private int b_h;

        private int r;

        private int r_r;

        private int r_h;

        private float dbd;

        private int sp;

        private bool isopenhsl;

        private string kfbName;

        public MagicZoomTileSource1(int imageWidth, int imageHeight, int tileWidth, int overlap, IMAGE_INFO_STRUCT infoStruct, float zoom, MultiScaleImage msi)
            : base(imageWidth, imageHeight, tileWidth, overlap)
        {
            InfoStruct = infoStruct;
            width = imageWidth;
            height = imageHeight;
            Xzoom = zoom;
            TileWidth = tileWidth;
            Msi = msi;
        }

        /// <summary>
        /// 新增了kfbname文件的名称，方便测试
        /// </summary>
        public MagicZoomTileSource1(int imageWidth, int imageHeight, int tileWidth, int overlap, IMAGE_INFO_STRUCT infoStruct, float zoom, MultiScaleImage msi, string kfbName)
            : base(imageWidth, imageHeight, tileWidth, overlap)
        {
            InfoStruct = infoStruct;
            width = imageWidth;
            height = imageHeight;
            Xzoom = zoom;
            TileWidth = tileWidth;
            Msi = msi;
            this.kfbName = kfbName;
        }

        public void SetinfoStruct(IMAGE_INFO_STRUCT infoStruct)
        {
            InfoStruct = infoStruct;
        }

        public void SetAngle(double Angle)
        {
            this.Angle = Angle;
        }

        public void Setisopenhsl(bool isopen)
        {
            isopenhsl = isopen;
        }

        public void SetHSV(int p_S_shift, int p_V_shift, int p_b, int p_b_r, int p_b_h, int p_r, int p_r_r, int p_r_h, float p_k, int p_sp)
        {
            S_shift = p_S_shift;
            V_shift = p_V_shift;
            b = p_b;
            b_r = p_b_r;
            b_h = p_b_h;
            r = p_r;
            r_r = p_r_r;
            r_h = p_r_h;
            dbd = p_k;
            sp = p_sp;
        }

        protected internal override object GetTileLayers(int tileLevel, int tilePositionX, int tilePositionY)
        {
            if (tileLevel > 8)
            {
                if (InfoStruct.DataFilePTR > 0)
                {
                    return LoadImage(InfoStruct.DataFilePTR, tileLevel, tilePositionX, tilePositionY);
                }
                return null;
            }
            return null;
        }

        public override void GetTileLayersAngle(ref double CenterX, ref double CenterY, ref double refAngle, ref double OffsetX, ref double OffsetY)
        {
            if (Msi.ZoomableCanvas != null)
            {
                OffsetX = Msi.ZoomableCanvas.Offset.X;
                OffsetY = Msi.ZoomableCanvas.Offset.Y;
                CenterX = Msi.ActualWidth / 2.0;
                CenterY = Msi.ActualHeight / 2.0;
                refAngle = Angle;
            }
        }


        public Stream LoadImage(int KfbioAddress, int Level, int posx, int posy)
        {
            try
            {
                MemoryStream result = null;
                int nDataLength = 0;
                IMAGE_INFO_STRUCT k = default(IMAGE_INFO_STRUCT);
                k.DataFilePTR = KfbioAddress;
                int num = Math.Max(width, height);
                int num2 = MathHelper.IsInteger((Math.Log(num) / Math.Log(2.0)).ToString()) ? ((int)(Math.Log(num) / Math.Log(2.0))) : ((int)(Math.Log(num) / Math.Log(2.0)) + 1);
                float fScale = (num2 != Level) ? (Xzoom / (float)Math.Pow(2.0, num2 - Level)) : Xzoom;
                try
                {
                    IntPtr datas;
                    if (!isopenhsl)
                    {
                        DllImageFuc.GetImageStreamFunc(ref k, fScale, posx * TileWidth, posy * TileWidth, ref nDataLength, out datas);
                    }
                    else
                    {
                        DllImageFuc.GetHSVImage(ref k, fScale, posx * TileWidth, posy * TileWidth, ref nDataLength, out datas, S_shift, V_shift, b, b_r, b_h, r, r_r, r_h, dbd, sp);
                    }
                    byte[] array = new byte[nDataLength];
                    if (datas != IntPtr.Zero)
                    {
                        Marshal.Copy(datas, array, 0, nDataLength);
                    }
                    DllImageFuc.DeleteImageDataFunc(datas);
                    result = new MemoryStream(array);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("zz" + ex.ToString() + nDataLength);
                }
                return result;
            }
            catch (FileNotFoundException)
            {
            }
            catch (FileFormatException)
            {
            }
            return null;
        }
    }
}

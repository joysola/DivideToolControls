using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DivideToolControls.Helper
{
	public class DllImageFuc
	{
		/// <summary>
		/// 获取文件的头部信息
		/// </summary>
		/// <param name="k">文件地址</param>
		/// <param name="khiImageHeight">图像高度</param>
		/// <param name="khiImageWidth">图像宽度</param>
		/// <param name="khiScanScale">扫描倍率：20还是40</param>
		/// <param name="khiSpendTime">消耗时间</param>
		/// <param name="khiScanTime">扫描时间</param>
		/// <param name="khiImageCapRes"></param>
		/// <param name="TileSize">瓦片图的大小</param>
		/// <returns></returns>
		public bool CkGetHeaderInfoFunc(IMAGE_INFO_STRUCT k, ref int khiImageHeight, ref int khiImageWidth, ref int khiScanScale, ref float khiSpendTime, ref double khiScanTime, ref float khiImageCapRes, ref int TileSize)
		{
			return GetHeaderInfoFunc(k, ref khiImageHeight, ref khiImageWidth, ref khiScanScale, ref khiSpendTime, ref khiScanTime, ref khiImageCapRes, ref TileSize);
		}

		public bool CkUnInitImageFileFunc(ref IMAGE_INFO_STRUCT k)
		{
			return UnInitImageFileFunc(ref k);
		}
		/// <summary>
		/// 读取kfb文件的总大小
		/// </summary>
		/// <param name="k">文件的地址</param>
		/// <param name="p">kfb文件的完整路径</param>
		/// <returns></returns>
		public bool CkInitImageFileFunc(ref IMAGE_INFO_STRUCT k, string p)
		{
			return InitImageFileFunc(ref k, p);
		}

		/// <summary>
		/// 获取二维码信息
		/// </summary>
		public bool CkGetLableInfoFunc(IMAGE_INFO_STRUCT k, out IntPtr datas, ref int a, ref int b, ref int c)
		{
			return GetLableInfoFunc(k, out datas, ref a, ref b, ref c);
		}

		/// <summary>
		/// 删除内存数据
		/// </summary>
		/// <param name="datas"></param>
		/// <returns></returns>
		public bool CkDeleteImageDataFunc(IntPtr datas)
		{
			return DeleteImageDataFunc(datas);
		}

		/// <summary>
		/// 获取瓦片图
		/// </summary>
		/// <param name="k">地址</param>
		/// <param name="fScale">缩放比例</param>
		/// <param name="nImagePosX">瓦片图的坐标：X</param>
		/// <param name="nImagePosY">瓦片图的坐标：Y</param>
		/// <param name="nDataLength">图片的字节长度</param>
		/// <param name="datas">图片的byte</param>
		/// <returns></returns>
		public bool CkGetImageStreamFunc(ref IMAGE_INFO_STRUCT k, float fScale, int nImagePosX, int nImagePosY, ref int nDataLength, out IntPtr datas)
		{
			return GetImageStreamFunc(ref k, fScale, nImagePosX, nImagePosY, ref nDataLength, out datas);
		}
		/// <summary>
		/// 获取版本信息
		/// </summary>
		/// <param name="k">文件的地址</param>
		/// <param name="fScale">最大的层数</param>
		/// <returns></returns>
		public bool CkGetVersionInfoFunc(ref IMAGE_INFO_STRUCT k, ref float fScale)
		{
			return GetVersionInfoFunc(ref k, ref fScale);
		}

		/// <summary>
		/// 获取患者的二维码和样本的缩略图
		/// </summary>
		/// <param name="path">kfb地址</param>
		/// <param name="datas">地址</param>
		public bool CkGetThumnailImagePathFunc(string path, out IntPtr datas, ref int a, ref int b, ref int c)
		{
			try
			{
				return GetThumnailImagePathFunc(path, out datas, ref a, ref b, ref c);
			}
			catch (Exception ex)
			{
				datas = IntPtr.Zero;
				AddLog(ex.ToString());
				return false;
			}
		}

		private void AddLog(string line)
		{
			string str = "Log.txt";
			try
			{
				string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
				FileStream fileStream = new FileStream(baseDirectory + str, FileMode.OpenOrCreate, FileAccess.Write);
				StreamWriter streamWriter = new StreamWriter(fileStream);
				streamWriter.BaseStream.Seek(0L, SeekOrigin.End);
				streamWriter.WriteLine(line);
				streamWriter.Flush();
				streamWriter.Close();
				fileStream.Close();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		[DllImport("ImageOperationLib.dll")]
		public static extern bool GetLableInfoFunc(IMAGE_INFO_STRUCT k, out IntPtr datas, ref int a, ref int b, ref int c);

		[DllImport("ImageOperationLib.dll")]
		public static extern bool GetHeaderInfoFunc(IMAGE_INFO_STRUCT k, ref int khiImageHeight, ref int khiImageWidth, ref int khiScanScale, ref float khiSpendTime, ref double khiScanTime, ref float khiImageCapRes, ref int TileSize);

		[DllImport("ImageOperationLib.dll")]
		public static extern bool InitImageFileFunc(ref IMAGE_INFO_STRUCT k, string p);

		[DllImport("ImageOperationLib.dll")]
		public static extern bool UnInitImageFileFunc(ref IMAGE_INFO_STRUCT k);

		[DllImport("ImageOperationLib.dll")]
		public static extern bool GetImageStreamFunc(ref IMAGE_INFO_STRUCT k, float fScale, int nImagePosX, int nImagePosY, ref int nDataLength, out IntPtr datas);

		[DllImport("ImageOperationLib.dll")]
		public static extern bool DeleteImageDataFunc(IntPtr datas);

		[DllImport("ImageOperationLib.dll")]
		public static extern bool GetThumnailImagePathFunc(string path, out IntPtr datas, ref int a, ref int b, ref int c);

		[DllImport("ImageOperationLib.dll")]
		public static extern bool GetPriviewInfoPathFunc(string path, out IntPtr datas, ref int a, ref int b, ref int c);

		[DllImport("ImageOperationLib.dll")]
		public static extern bool GetLableInfoPathFunc(string path, out IntPtr datas, ref int a, ref int b, ref int c);

		[DllImport("ImageOperationLib.dll")]
		public static extern bool GetVersionInfoFunc(ref IMAGE_INFO_STRUCT k, ref float f);

		[DllImport("ImageOperationLib.dll")]
		public static extern bool GetMachineSerialNumFunc(ref IMAGE_INFO_STRUCT k, IntPtr str);

		[DllImport("ImageOperationLib.dll")]
		public static extern bool GetScanTimeDurationFunc(ref IMAGE_INFO_STRUCT k, ref int nYear, ref int nMonth, ref int nDay, ref int nHour, ref int nMiniter, ref int nSecond, ref int nDurHour, ref int nDurMin, ref int nDurSecond);

		[DllImport("ImageOperationLib.dll")]
		public static extern void GetDllVersionFunc(ref float f);

		/// <summary>
		/// 获取kfb文件的层数信息
		/// </summary>
		/// <param name="k">文件地址</param>
		/// <param name="nCurLevel">当前层数</param>
		/// <param name="nTotalLevel">总层数</param>
		[DllImport("ImageOperationLib.dll")]
		public static extern void GetScanLevelInfoFunc(ref IMAGE_INFO_STRUCT k, ref int nCurLevel, ref int nTotalLevel);

		[DllImport("ImageOperationLib.dll")]
		public static extern void GetImageDataRoiFunc(IMAGE_INFO_STRUCT k, float fScale, int x, int y, int width, int height, out IntPtr datas, ref int nDataLength, bool flag);

		[DllImport("ImageSprocLib.dll")]
		public static extern void ImageMatchRotate(string pSrcImagePath, string pRotateImagePath, ref int nDegree);

		[DllImport("ImageSprocLib.dll")]
		public static extern int TMARec(string pSrcImagePath);

		[DllImport("ImageSprocLib.dll")]
		public static extern int TMARecS(string pSrcImagePath, int Param);

		[DllImport("ImageOperationLib.dll")]
		public static extern bool GetHSVImage(ref IMAGE_INFO_STRUCT kfbPoint, float fScale, int nImagePosX, int nImagePosY, ref int nDataLength, out IntPtr datas, int S_shift, int V_shift, int b, int b_r, int b_h, int r, int r_r, int r_h, float k, int sp);
	}
}

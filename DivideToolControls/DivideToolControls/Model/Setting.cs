using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DivideToolControls.Model
{
	public class Setting
	{
		public static Color STROCK_COLOR = Color.FromArgb(byte.MaxValue, 0, 0, byte.MaxValue);

		public static Color STROCK_COLOR1 = Color.FromArgb(byte.MaxValue, 0, 0, byte.MaxValue);

		public static Color STROCK_COLOR2 = Color.FromArgb(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);

		public static Color STROCK_COLOR3 = Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);

		public static Color STROCK_COLOR4 = Color.FromArgb(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);

		public static Color STROCK_COLOR5 = Color.FromArgb(byte.MaxValue, byte.MaxValue, 0, 0);

		public static Color STROCK_COLOR6 = Color.FromArgb(byte.MaxValue, 0, 0, 0);

		public static Color STROCK_COLOR7 = Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		public static double STROCK_SIZE = 2.0;

		public static double MAX_ZOOMOUT_RATIO = 0.2;

		public static double MAX_ZOOMIN_RATIO = 20.0;

		public static string Line = "直线";

		public static string TmaLine = "Tma直线";

		public static string Arrow = "箭头";

		public static string Rectangle = "矩形";

		public static string CtcRectangle = "CtcRect";

		public static string Ellipse = "椭圆";

		public static string Polygon = "曲线";

		public static string Remark = "标注";

		public static string xAngle = "角度";

		public static string Width = " 宽：";

		public static string TMARectangle = "TMA";

		public static string Half_Width = " 短半轴：";

		public static string Half_Height = " 长半轴：";

		public static string Height = " 高：";

		public static string Length = " 周长：";

		public static string xLength = " 长度：";

		public static string Angle = " 角度：";

		public static string Area = " 面积：";

		public static string Angle_Unit = " 度:";

		public static string Unit = " μm";

		public static string Area_Unit = " μm²";

		public static string Lang = "x";

		public static Color Thumbcolor = Color.FromArgb(byte.MaxValue, 0, 0, byte.MaxValue);

		public static double Thumb_w = 10.0;

		public static double Thumb_c = 15.0;

		public static double Thumb_Mc = 25.0;

		public static int ShotWidth = 200;

		public static int ShotHeight = 200;

		public static int Move_Step = 10;

		public static int Move_Video_Speed = 1;

		public static int Move_Video_Step = 1;

		public static string AnnoDesStr = " 描述：";

		public static string FilePath = null;

		public static bool IsSynchronous = false;

		public static double MaxMagValue = 2.0;

		public static string IsLabel = "0";

		public static string IsNav = "0";

		public static string IsRule = "0";

		public static string IsCase = "0";

		public static string IsSingleFile = "1";

		public static string IsMutiScreen = "1";

		public static string IsHsv = "1";

		public static string Language = "";

		public static bool IsDw = false;

		public static Dictionary<object, object> TabsDic = new Dictionary<object, object>();

		public static int Opacity = 0;

		public static int isCtrl = 0;

		public static string AnalysisPath = "";

		public static string IsRotate = "1";

		public static string IsMagnifier = "1";

		public static string IsOperateball = "1";

		public static double Calibration40 = 0.0;

		public static double Calibration20 = 0.0;

		public static double CalibrationX40 = 0.0;

		public static double CalibrationX20 = 0.0;

		public static double MargPara = 1.0;

		public static int Magnifier = 0;

		public static int IsFloat = 0;

		public static double AngelMsiOffset = 600.0;

		public static string IsLogo = "0";

		public static bool isAnnoChange = false;

		public static Color NumberToRgba(uint num)
		{
			return Color.FromArgb(byte.Parse(((num >> 24) & 0xFF).ToString()), byte.Parse(((num >> 16) & 0xFF).ToString()), byte.Parse(((num >> 8) & 0xFF).ToString()), byte.Parse((num & 0xFF).ToString()));
		}

		public static uint RgbaToNumber(uint a, uint r, uint g, uint b)
		{
			return ((a & 0xFF) << 24) | ((r & 0xFF) << 16) | ((g & 0xFF) << 8) | (b & 0xFF);
		}

		public static Point PointRotate(Point center, Point p1, double angle)
		{
			Point result = default(Point);
			double num = angle * Math.PI / 180.0;
			double x = (p1.X - center.X) * Math.Cos(num) + (p1.Y - center.Y) * Math.Sin(num) + center.X;
			double y = (0.0 - (p1.X - center.X)) * Math.Sin(num) + (p1.Y - center.Y) * Math.Cos(num) + center.Y;
			result.X = x;
			result.Y = y;
			return result;
		}

		public static string Encrypt(string input)
		{
			string s = "saltValue";
			string password = "pwdValue";
			byte[] bytes = Encoding.UTF8.GetBytes(input);
			byte[] bytes2 = Encoding.UTF8.GetBytes(s);
			AesManaged aesManaged = new AesManaged();
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, bytes2);
			aesManaged.BlockSize = aesManaged.LegalBlockSizes[0].MaxSize;
			aesManaged.KeySize = aesManaged.LegalKeySizes[0].MaxSize;
			aesManaged.Key = rfc2898DeriveBytes.GetBytes(aesManaged.KeySize / 8);
			aesManaged.IV = rfc2898DeriveBytes.GetBytes(aesManaged.BlockSize / 8);
			ICryptoTransform transform = aesManaged.CreateEncryptor();
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
			cryptoStream.Write(bytes, 0, bytes.Length);
			cryptoStream.Close();
			string text = Convert.ToBase64String(memoryStream.ToArray());
			return text.Replace("+", "%2B");
		}

		public static string Decrypt(string input)
		{
			string s = "saltValue";
			string password = "pwdValue";
			byte[] array = Convert.FromBase64String(input.Replace("%2B", "+"));
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			AesManaged aesManaged = new AesManaged();
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, bytes);
			aesManaged.BlockSize = aesManaged.LegalBlockSizes[0].MaxSize;
			aesManaged.KeySize = aesManaged.LegalKeySizes[0].MaxSize;
			aesManaged.Key = rfc2898DeriveBytes.GetBytes(aesManaged.KeySize / 8);
			aesManaged.IV = rfc2898DeriveBytes.GetBytes(aesManaged.BlockSize / 8);
			ICryptoTransform transform = aesManaged.CreateDecryptor();
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
			cryptoStream.Write(array, 0, array.Length);
			cryptoStream.Close();
			byte[] array2 = memoryStream.ToArray();
			return Encoding.UTF8.GetString(array2, 0, array2.Length);
		}
	}
}

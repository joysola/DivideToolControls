using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivideToolControls.Extensions
{
    public static class MathExtension
    {
        public static decimal Ceiling(this decimal value)
        {
            return Math.Ceiling(value);
        }

        public static double Ceiling(this double value)
        {
            return Math.Ceiling(value);
        }

        public static decimal Floor(this decimal value)
        {
            return Math.Floor(value);
        }

        public static double Floor(this double value)
        {
            return Math.Floor(value);
        }

        public static byte AtLeast(this byte a, byte b)
        {
            return Math.Max(a, b);
        }

        public static decimal AtLeast(this decimal a, decimal b)
        {
            return Math.Max(a, b);
        }

        public static double AtLeast(this double a, double b)
        {
            return Math.Max(a, b);
        }

        public static float AtLeast(this float a, float b)
        {
            return Math.Max(a, b);
        }

        public static short AtLeast(this short a, short b)
        {
            return Math.Max(a, b);
        }

        public static int AtLeast(this int a, int b)
        {
            return Math.Max(a, b);
        }

        public static long AtLeast(this long a, long b)
        {
            return Math.Max(a, b);
        }

        [CLSCompliant(false)]
        public static sbyte AtLeast(this sbyte a, sbyte b)
        {
            return Math.Max(a, b);
        }

        [CLSCompliant(false)]
        public static ushort AtLeast(this ushort a, ushort b)
        {
            return Math.Max(a, b);
        }

        [CLSCompliant(false)]
        public static uint AtLeast(this uint a, uint b)
        {
            return Math.Max(a, b);
        }

        [CLSCompliant(false)]
        public static ulong AtLeast(this ulong a, ulong b)
        {
            return Math.Max(a, b);
        }

        public static byte AtMost(this byte a, byte b)
        {
            return Math.Min(a, b);
        }

        public static decimal AtMost(this decimal a, decimal b)
        {
            return Math.Min(a, b);
        }

        public static double AtMost(this double a, double b)
        {
            return Math.Min(a, b);
        }

        public static float AtMost(this float a, float b)
        {
            return Math.Min(a, b);
        }

        public static short AtMost(this short a, short b)
        {
            return Math.Min(a, b);
        }

        public static int AtMost(this int a, int b)
        {
            return Math.Min(a, b);
        }

        public static long AtMost(this long a, long b)
        {
            return Math.Min(a, b);
        }

        [CLSCompliant(false)]
        public static sbyte AtMost(this sbyte a, sbyte b)
        {
            return Math.Min(a, b);
        }

        [CLSCompliant(false)]
        public static ushort AtMost(this ushort a, ushort b)
        {
            return Math.Min(a, b);
        }

        [CLSCompliant(false)]
        public static uint AtMost(this uint a, uint b)
        {
            return Math.Min(a, b);
        }

        [CLSCompliant(false)]
        public static ulong AtMost(this ulong a, ulong b)
        {
            return Math.Min(a, b);
        }

        public static byte Clamp(this byte value, byte a, byte b)
        {
            if (a >= b)
            {
                return Math.Max(Math.Min(value, a), b);
            }
            return Math.Min(Math.Max(value, a), b);
        }

        public static decimal Clamp(this decimal value, decimal a, decimal b)
        {
            if (!(a < b))
            {
                return Math.Max(Math.Min(value, a), b);
            }
            return Math.Min(Math.Max(value, a), b);
        }
        /// <summary>
        /// value 、a 、b取大小中间的
        /// </summary>
        /// <param name="value"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Clamp(this double value, double a, double b)
        {
            if (!(a < b))
            {
                return Math.Max(Math.Min(value, a), b);
            }
            return Math.Min(Math.Max(value, a), b);
        }

        public static float Clamp(this float value, float a, float b)
        {
            if (!(a < b))
            {
                return Math.Max(Math.Min(value, a), b);
            }
            return Math.Min(Math.Max(value, a), b);
        }

        public static short Clamp(this short value, short a, short b)
        {
            if (a >= b)
            {
                return Math.Max(Math.Min(value, a), b);
            }
            return Math.Min(Math.Max(value, a), b);
        }
        /// <summary>
        /// 取中间大小
        /// </summary>
        /// <param name="value"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int Clamp(this int value, int a, int b)
        {
            if (a >= b)
            {
                return Math.Max(Math.Min(value, a), b);
            }
            return Math.Min(Math.Max(value, a), b);
        }

        public static long Clamp(this long value, long a, long b)
        {
            if (a >= b)
            {
                return Math.Max(Math.Min(value, a), b);
            }
            return Math.Min(Math.Max(value, a), b);
        }

        [CLSCompliant(false)]
        public static sbyte Clamp(this sbyte value, sbyte a, sbyte b)
        {
            if (a >= b)
            {
                return Math.Max(Math.Min(value, a), b);
            }
            return Math.Min(Math.Max(value, a), b);
        }

        [CLSCompliant(false)]
        public static ushort Clamp(this ushort value, ushort a, ushort b)
        {
            if (a >= b)
            {
                return Math.Max(Math.Min(value, a), b);
            }
            return Math.Min(Math.Max(value, a), b);
        }

        [CLSCompliant(false)]
        public static uint Clamp(this uint value, uint a, uint b)
        {
            if (a >= b)
            {
                return Math.Max(Math.Min(value, a), b);
            }
            return Math.Min(Math.Max(value, a), b);
        }

        [CLSCompliant(false)]
        public static ulong Clamp(this ulong value, ulong a, ulong b)
        {
            if (a >= b)
            {
                return Math.Max(Math.Min(value, a), b);
            }
            return Math.Min(Math.Max(value, a), b);
        }

        public static decimal Round(this decimal value)
        {
            return Math.Round(value);
        }

        public static double Round(this double value)
        {
            return Math.Round(value);
        }

        public static decimal Round(this decimal value, int decimals)
        {
            return Math.Round(value, decimals);
        }

        public static decimal Round(this decimal value, MidpointRounding mode)
        {
            return Math.Round(value, mode);
        }

        public static double Round(this double value, int digits)
        {
            return Math.Round(value, digits);
        }

        public static double Round(this double value, MidpointRounding mode)
        {
            return Math.Round(value, mode);
        }

        public static decimal Round(this decimal value, int decimals, MidpointRounding mode)
        {
            return Math.Round(value, decimals, mode);
        }

        public static double Round(this double value, int digits, MidpointRounding mode)
        {
            return Math.Round(value, digits, mode);
        }

        public static decimal RoundUp(this decimal value, decimal factor)
        {
            decimal num = value % factor;
            if (!(num == 0m))
            {
                if (!(value < 0m))
                {
                    return value + (factor - num);
                }
                return value - num;
            }
            return value;
        }

        public static double RoundUp(this double value, double factor)
        {
            double num = value % factor;
            if (num != 0.0)
            {
                if (!(value < 0.0))
                {
                    return value + (factor - num);
                }
                return value - num;
            }
            return value;
        }

        public static float RoundUp(this float value, float factor)
        {
            float num = value % factor;
            if (num != 0f)
            {
                if (!(value < 0f))
                {
                    return value + (factor - num);
                }
                return value - num;
            }
            return value;
        }

        public static long RoundUp(this long value, long factor)
        {
            long num = value % factor;
            if (num != 0)
            {
                if (value >= 0)
                {
                    return value + (factor - num);
                }
                return value - num;
            }
            return value;
        }

        public static int RoundUp(this int value, int factor)
        {
            int num = value % factor;
            if (num != 0)
            {
                if (value >= 0)
                {
                    return value + (factor - num);
                }
                return value - num;
            }
            return value;
        }

        public static short RoundUp(this short value, short factor)
        {
            int num = value % factor;
            return (short)((num == 0) ? value : ((value < 0) ? (value - num) : (value + (factor - num))));
        }

        public static byte RoundUp(this byte value, byte factor)
        {
            int num = (int)value % (int)factor;
            return (byte)((num == 0) ? value : (value + (factor - num)));
        }

        public static decimal RoundUp(this decimal value)
        {
            return value.RoundUp(1m);
        }

        public static double RoundUp(this double value)
        {
            return value.RoundUp(1.0);
        }

        public static float RoundUp(this float value)
        {
            return value.RoundUp(1f);
        }

        public static long RoundUp(this long value)
        {
            return value.RoundUp(1L);
        }

        public static int RoundUp(this int value)
        {
            return value.RoundUp(1);
        }

        public static short RoundUp(this short value)
        {
            return value.RoundUp((short)1);
        }

        public static byte RoundUp(this byte value)
        {
            return value.RoundUp((byte)1);
        }

        public static decimal RoundDown(this decimal value, decimal factor)
        {
            decimal num = value % factor;
            if (!(num == 0m))
            {
                if (!(value > 0m))
                {
                    return value - (factor + num);
                }
                return value - num;
            }
            return value;
        }

        public static double RoundDown(this double value, double factor)
        {
            double num = value % factor;
            if (num != 0.0)
            {
                if (!(value > 0.0))
                {
                    return value - (factor + num);
                }
                return value - num;
            }
            return value;
        }

        public static float RoundDown(this float value, float factor)
        {
            float num = value % factor;
            if (num != 0f)
            {
                if (!(value > 0f))
                {
                    return value - (factor + num);
                }
                return value - num;
            }
            return value;
        }

        public static long RoundDown(this long value, long factor)
        {
            long num = value % factor;
            if (num != 0)
            {
                if (value <= 0)
                {
                    return value - (factor + num);
                }
                return value - num;
            }
            return value;
        }

        public static int RoundDown(this int value, int factor)
        {
            int num = value % factor;
            if (num != 0)
            {
                if (value <= 0)
                {
                    return value - (factor + num);
                }
                return value - num;
            }
            return value;
        }

        public static short RoundDown(this short value, short factor)
        {
            int num = value % factor;
            return (short)((num == 0) ? value : ((value > 0) ? (value - num) : (value - (factor + num))));
        }

        public static byte RoundDown(this byte value, byte factor)
        {
            int num = (int)value % (int)factor;
            return (byte)(value - num);
        }

        public static decimal RoundDown(this decimal value)
        {
            return value.RoundDown(1m);
        }

        public static double RoundDown(this double value)
        {
            return value.RoundDown(1.0);
        }

        public static float RoundDown(this float value)
        {
            return value.RoundDown(1f);
        }

        public static long RoundDown(this long value)
        {
            return value.RoundDown(1L);
        }

        public static int RoundDown(this int value)
        {
            return value.RoundDown(1);
        }

        public static short RoundDown(this short value)
        {
            return value.RoundDown((short)1);
        }

        public static byte RoundDown(this byte value)
        {
            return value.RoundDown((byte)1);
        }

        public static decimal Truncate(this decimal value)
        {
            return Math.Truncate(value);
        }

        public static double Truncate(this double value)
        {
            return Math.Truncate(value);
        }

        public static decimal Abs(this decimal value)
        {
            return Math.Abs(value);
        }

        public static double Abs(this double value)
        {
            return Math.Abs(value);
        }

        public static float Abs(this float value)
        {
            return Math.Abs(value);
        }

        public static short Abs(this short value)
        {
            return Math.Abs(value);
        }

        public static int Abs(this int value)
        {
            return Math.Abs(value);
        }

        public static long Abs(this long value)
        {
            return Math.Abs(value);
        }

        [CLSCompliant(false)]
        public static sbyte Abs(this sbyte value)
        {
            return Math.Abs(value);
        }

        public static long BigMul(this int a, int b)
        {
            return Math.BigMul(a, b);
        }

        public static int DivRem(this int a, int b, out int result)
        {
            return Math.DivRem(a, b, out result);
        }

        public static long DivRem(this long a, long b, out long result)
        {
            return Math.DivRem(a, b, out result);
        }

        public static double Exp(this double value)
        {
            return Math.Exp(value);
        }

        public static double IEEERemainder(this double x, double y)
        {
            return Math.IEEERemainder(x, y);
        }

        public static double Log(this double value)
        {
            return Math.Log(value);
        }

        public static double Log(this double value, double newBase)
        {
            return Math.Log(value, newBase);
        }

        public static double Log10(this double value)
        {
            return Math.Log10(value);
        }

        public static double Pow(this double x, double y)
        {
            return Math.Pow(x, y);
        }

        public static int Sign(this decimal value)
        {
            return Math.Sign(value);
        }

        public static int Sign(this double value)
        {
            return Math.Sign(value);
        }

        public static int Sign(this float value)
        {
            return Math.Sign(value);
        }

        public static int Sign(this short value)
        {
            return Math.Sign(value);
        }

        public static int Sign(this int value)
        {
            return Math.Sign(value);
        }

        public static int Sign(this long value)
        {
            return Math.Sign(value);
        }

        [CLSCompliant(false)]
        public static int Sign(this sbyte value)
        {
            return Math.Sign(value);
        }

        public static double Sqrt(this double value)
        {
            return Math.Sqrt(value);
        }

        public static bool IsBetween(this decimal value, decimal a, decimal b)
        {
            if (!(a < b))
            {
                if (b <= value)
                {
                    return value <= a;
                }
                return false;
            }
            if (a <= value)
            {
                return value <= b;
            }
            return false;
        }

        public static bool IsBetween(this double value, double a, double b)
        {
            if (!(a < b))
            {
                if (b <= value)
                {
                    return value <= a;
                }
                return false;
            }
            if (a <= value)
            {
                return value <= b;
            }
            return false;
        }

        public static bool IsBetween(this float value, float a, float b)
        {
            if (!(a < b))
            {
                if (b <= value)
                {
                    return value <= a;
                }
                return false;
            }
            if (a <= value)
            {
                return value <= b;
            }
            return false;
        }

        public static bool IsBetween(this long value, long a, long b)
        {
            if (a >= b)
            {
                if (b <= value)
                {
                    return value <= a;
                }
                return false;
            }
            if (a <= value)
            {
                return value <= b;
            }
            return false;
        }

        public static bool IsBetween(this int value, int a, int b)
        {
            if (a >= b)
            {
                if (b <= value)
                {
                    return value <= a;
                }
                return false;
            }
            if (a <= value)
            {
                return value <= b;
            }
            return false;
        }

        public static bool IsBetween(this short value, short a, short b)
        {
            if (a >= b)
            {
                if (b <= value)
                {
                    return value <= a;
                }
                return false;
            }
            if (a <= value)
            {
                return value <= b;
            }
            return false;
        }

        public static bool IsBetween(this byte value, byte a, byte b)
        {
            if (a >= b)
            {
                if (b <= value)
                {
                    return value <= a;
                }
                return false;
            }
            if (a <= value)
            {
                return value <= b;
            }
            return false;
        }

        public static bool IsNaN(this double value)
        {
            return double.IsNaN(value);
        }

        public static bool IsNaN(this float value)
        {
            return float.IsNaN(value);
        }

        public static double GetValueOrDefault(this double value)
        {
            if (!double.IsNaN(value))
            {
                return value;
            }
            return 0.0;
        }

        public static double GetValueOrDefault(this double value, double defaultValue)
        {
            if (!double.IsNaN(value))
            {
                return value;
            }
            return defaultValue;
        }

        public static float GetValueOrDefault(this float value)
        {
            if (!float.IsNaN(value))
            {
                return value;
            }
            return 0f;
        }

        public static float GetValueOrDefault(this float value, float defaultValue)
        {
            if (!float.IsNaN(value))
            {
                return value;
            }
            return defaultValue;
        }

        public static double Acos(this double value)
        {
            return Math.Acos(value);
        }

        public static double Asin(this double value)
        {
            return Math.Asin(value);
        }

        public static double Atan(this double value)
        {
            return Math.Atan(value);
        }

        public static double Atan2(this double y, double x)
        {
            return Math.Atan2(y, x);
        }

        public static double Cos(this double angle)
        {
            return Math.Cos(angle);
        }

        public static double Cosh(this double angle)
        {
            return Math.Cosh(angle);
        }

        public static double Sin(this double angle)
        {
            return Math.Sin(angle);
        }

        public static double Sinh(this double angle)
        {
            return Math.Sinh(angle);
        }

        public static double Tan(this double angle)
        {
            return Math.Tan(angle);
        }

        public static double Tanh(this double angle)
        {
            return Math.Tanh(angle);
        }

        public static IEnumerable<decimal> To(this decimal start, decimal bound)
        {
            decimal step = (start <= bound) ? 1m : (-1m);
            for (decimal i = start; i < bound; i += step)
            {
                yield return i;
            }
        }

        public static IEnumerable<decimal> To(this decimal start, decimal bound, decimal step)
        {
            for (decimal i = start; i < bound; i += step)
            {
                yield return i;
            }
        }

        public static IEnumerable<double> To(this double start, double bound)
        {
            double step = (start <= bound) ? 1.0 : (-1.0);
            for (double i = start; i < bound; i += step)
            {
                yield return i;
            }
        }

        public static IEnumerable<double> To(this double start, double bound, double step)
        {
            for (double i = start; i < bound; i += step)
            {
                yield return i;
            }
        }

        public static IEnumerable<float> To(this float start, float bound)
        {
            float step = (start <= bound) ? 1f : (-1f);
            for (float i = start; i < bound; i += step)
            {
                yield return i;
            }
        }

        public static IEnumerable<float> To(this float start, float bound, float step)
        {
            for (float i = start; i < bound; i += step)
            {
                yield return i;
            }
        }

        public static IEnumerable<long> To(this long start, long bound)
        {
            long step = (start <= bound) ? 1 : (-1);
            for (long i = start; i < bound; i += step)
            {
                yield return i;
            }
        }

        public static IEnumerable<long> To(this long start, long bound, long step)
        {
            for (long i = start; i < bound; i += step)
            {
                yield return i;
            }
        }

        public static IEnumerable<int> To(this int start, int bound)
        {
            int step = (start <= bound) ? 1 : (-1);
            for (int i = start; i < bound; i += step)
            {
                yield return i;
            }
        }

        public static IEnumerable<int> To(this int start, int bound, int step)
        {
            for (int i = start; i < bound; i += step)
            {
                yield return i;
            }
        }

        public static IEnumerable<short> To(this short start, short bound)
        {
            short step = (short)((start <= bound) ? 1 : (-1));
            for (short i = start; i < bound; i = (short)(i + step))
            {
                yield return i;
            }
        }

        public static IEnumerable<short> To(this short start, short bound, short step)
        {
            for (short i = start; i < bound; i = (short)(i + step))
            {
                yield return i;
            }
        }
    }
}

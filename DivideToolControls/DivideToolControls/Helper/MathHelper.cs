using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DivideToolControls.Helper
{
    public class MathHelper
    {
        public static bool IsInteger(string s)
        {
            string pattern = "^\\d*$";
            return Regex.IsMatch(s, pattern);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivideToolControls.Helper
{
    public class ControlHelper
    {
        public static int GetMargin(string V)
        {
            int result = 0;
            switch (V.Length)
            {
                case 1:
                    result = 10;
                    break;
                case 2:
                    result = 15;
                    break;
                case 3:
                    result = 20;
                    break;
                case 4:
                    result = 25;
                    break;
            }
            return result;
        }
    }
}

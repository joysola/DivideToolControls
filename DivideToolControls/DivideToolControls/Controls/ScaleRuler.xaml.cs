using DivideToolControls.Helper;
using DivideToolControls.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// ScaleRuler.xaml 的交互逻辑
    /// </summary>
    public partial class ScaleRuler : UserControl
    {
        private int MinRuleV = 200;
        public ScaleRuler()
        {
            InitializeComponent();
            InitOnce();
        }

        private void InitOnce()
        {
            
        }

        public void UpdateRule()
        {
            double num = (double)ZoomModel.SlideZoom / ZoomModel.Curscale;
            double num2 = (num <= 1.0) ? 1.0 : ((!(num > 1.0) || !(num < 2.0)) ? (num - num % 2.0) : 1.0);
            double UM = 100.0 * num2;
            if (UM < 1000.0)
            {
                if (UM % 5.0 != 0.0)
                {
                    UM += UM - UM % 5.0;
                }
            }
            else if (UM % 5000.0 != 0.0)
            {
                UM = ((!(UM < 5000.0)) ? (UM + (UM / 5000.0 - UM % 5000.0)) : 5000.0);
            }
            double XS = UM / (num * ZoomModel.Calibration);
            CheckLimtRule(ref XS, ref UM);
            UpRuleLayout(XS, UM);
        }
        public void CheckLimtRule(ref double XS, ref double UM)
        {
            if (XS >= (double)(MinRuleV + MinRuleV / 2))
            {
                XS /= 2.0;
                UM /= 2.0;
                if (XS >= (double)(MinRuleV + MinRuleV / 2))
                {
                    CheckLimtRule(ref XS, ref UM);
                }
            }
            if (XS < 150.0)
            {
                XS *= 2.0;
                UM *= 2.0;
            }
        }

        public void UpRuleLayout(double XS, double UM)
        {
            double num = 0.0;
            double num2 = 0.0;
            string empty = string.Empty;
            if (UM >= 1000.0)
            {
                num = UM / 5000.0;
                empty = "mm";
            }
            else
            {
                num = UM / 5.0;
                empty = "μm";
            }
            num2 = XS / 5.0;
            RLine_1.X1 = num2 * 1.0;
            RLine_1.X2 = num2 * 1.0;
            RLine_2.X1 = num2 * 2.0;
            RLine_2.X2 = num2 * 2.0;
            RLine_3.X1 = num2 * 3.0;
            RLine_3.X2 = num2 * 3.0;
            RLine_4.X1 = num2 * 4.0;
            RLine_4.X2 = num2 * 4.0;
            RLine_5.X1 = num2 * 5.0;
            RLine_5.X2 = num2 * 5.0;
            RLine_L.X2 = num2 * 5.0;
            Rtxt_1.Content = Math.Round(num * 1.0, 2);
            Rtxt_2.Content = Math.Round(num * 2.0, 2);
            Rtxt_3.Content = Math.Round(num * 3.0, 2);
            Rtxt_4.Content = Math.Round(num * 4.0, 2);
            Rtxt_5.Content = Math.Round(num * 5.0, 2) + empty;
            Rtxt_1.Margin = new Thickness(num2 * 1.0 - (double)ControlHelper.GetMargin(Math.Round(num * 1.0, 2).ToString()), 10.0, 0.0, 0.0);
            Rtxt_2.Margin = new Thickness(num2 * 2.0 - (double)ControlHelper.GetMargin(Math.Round(num * 2.0, 2).ToString()), 10.0, 0.0, 0.0);
            Rtxt_3.Margin = new Thickness(num2 * 3.0 - (double)ControlHelper.GetMargin(Math.Round(num * 3.0, 2).ToString()), 10.0, 0.0, 0.0);
            Rtxt_4.Margin = new Thickness(num2 * 4.0 - (double)ControlHelper.GetMargin(Math.Round(num * 4.0, 2).ToString()), 10.0, 0.0, 0.0);
            Rtxt_5.Margin = new Thickness(num2 * 5.0 - (double)ControlHelper.GetMargin(Math.Round(num * 5.0, 2).ToString()), 10.0, 0.0, 0.0);
        }

        private void RuleThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Thickness margin = RuleCanvas.Margin;
            RuleCanvas.Margin = new Thickness(margin.Left + e.HorizontalChange, 0.0, 0.0, margin.Bottom - e.VerticalChange);
        }
    }
}

using DivideToolControls.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DivideToolControls
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        // 读取配置文件Config.ini数据
        public App()
        {
            IniFile ifile = new IniFile(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Config.ini");
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string text = System.IO.Path.Combine(folderPath, "KFBIO\\K-Viewer");
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            if (!File.Exists(System.IO.Path.Combine(text, "config.ini")))
            {
                File.Copy(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Config.ini", System.IO.Path.Combine(text, "config.ini"), true);
            }
            else
            {
                IniFile iniFile = new IniFile(System.IO.Path.Combine(text, "config.ini"));
                string text2 = iniFile.IniReadValue("Ver", "Value");
                if (text2.Trim().Length == 0)
                {
                    File.Copy(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Config.ini", System.IO.Path.Combine(text, "config.ini"), true);
                }
                else
                {
                    string a = ifile.IniReadValue("Ver", "Value");
                    if (a != text2)
                    {
                        File.Copy(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Config.ini", System.IO.Path.Combine(text, "config.ini"), true);
                    }
                }
            }
            var Configpath = System.IO.Path.Combine(text, "config.ini");
            ifile = new IniFile(Configpath);
            Setting.IsLabel = ifile.IniReadValue("Switch", "IsLabel");
            Setting.IsCase = ifile.IniReadValue("Switch", "IsCase");
            Setting.IsNav = ifile.IniReadValue("Switch", "IsNav");
            Setting.IsSingleFile = ifile.IniReadValue("Switch", "IsSingleFile");
            Setting.IsMutiScreen = ifile.IniReadValue("Switch", "IsMutiScreen");
            Setting.IsRule = ifile.IniReadValue("Switch", "IsRule");
            Setting.MaxMagValue = double.Parse(ifile.IniReadValue("MagScale", "Value"));
            Setting.Language = ifile.IniReadValue("Language", "Type");
            Setting.IsHsv = ifile.IniReadValue("Switch", "IsHsv");
            Setting.AnalysisPath = ifile.IniReadValue("ToolPath", "AnalysisPath");
            Setting.IsRotate = ifile.IniReadValue("Switch", "IsRotate");
            Setting.IsMagnifier = ifile.IniReadValue("Switch", "IsMagnifier");
            Setting.IsOperateball = ifile.IniReadValue("Switch", "IsOperateball");
            Setting.IsLogo = ifile.IniReadValue("Switch", "IsLogo");
            Setting.Magnifier = int.Parse(ifile.IniReadValue("Magnifier", "Value"));
            string text3 = ifile.IniReadValue("Calibration", "Value40");
            if (text3 == "")
            {
                text3 = "0";
            }
            string text4 = ifile.IniReadValue("Calibration", "Value20");
            if (text4 == "")
            {
                text4 = "0";
            }
            string text5 = ifile.IniReadValue("Calibration", "ValueX40");
            if (text5 == "")
            {
                text5 = "1";
            }
            string text6 = ifile.IniReadValue("Calibration", "ValueX20");
            if (text6 == "")
            {
                text6 = "1";
            }
            Setting.Calibration40 = double.Parse(text3);
            Setting.Calibration20 = double.Parse(text4);
            Setting.CalibrationX40 = double.Parse(text5);
            Setting.CalibrationX20 = double.Parse(text6);
        }
    }
}

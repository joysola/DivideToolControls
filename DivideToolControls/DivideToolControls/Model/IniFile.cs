using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DivideToolControls.Model
{
    public class IniFile
    {
        public string path;

        public IniFile(string INIPath)
        {
            path = INIPath;
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, byte[] retVal, int size, string filePath);

        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, path);
        }

        public string IniReadValue(string Section, string Key)
        {
            StringBuilder stringBuilder = new StringBuilder(255);
            GetPrivateProfileString(Section, Key, "", stringBuilder, 255, path);
            return stringBuilder.ToString();
        }

        public byte[] IniReadValues(string section, string key)
        {
            byte[] array = new byte[255];
            GetPrivateProfileString(section, key, "", array, 255, path);
            return array;
        }

        public void ClearAllSection()
        {
            IniWriteValue(null, null, null);
        }

        public void ClearSection(string Section)
        {
            IniWriteValue(Section, null, null);
        }
    }
}

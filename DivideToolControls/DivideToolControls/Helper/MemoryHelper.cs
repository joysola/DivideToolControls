using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivideToolControls.Helper
{
    public class MemoryHelper
    {
        private static PerformanceCounter performanceCounter;

        public static int getPrivateMemory()
        {
            int num = 0;
            try
            {
                if (performanceCounter == null)
                {
                    AddLog(Process.GetCurrentProcess().ProcessName.ToString());
                    performanceCounter = new PerformanceCounter("Process", "Working Set - Private", Process.GetCurrentProcess().ProcessName);
                }
                return (int)((double)Convert.ToInt64(performanceCounter.NextValue()) / 1024.0);
            }
            catch
            {
                return 0;
            }
        }

        private static void AddLog(string line)
        {
            string str = "Log.txt";
            try
            {
                FileStream fileStream = new FileStream("D:\\" + str, FileMode.OpenOrCreate, FileAccess.Write);
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
    }
}

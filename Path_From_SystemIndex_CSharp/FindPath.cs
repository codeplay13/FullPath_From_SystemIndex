using System;
using System.Runtime.InteropServices;
using System.Data.OleDb;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace full_path_csharp
{
    class FindPath
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public static string getFileNameUsingHandle(IntPtr currentWindowHandle)
        {
            const int size = 256;
            StringBuilder windowTitle = new StringBuilder(size);
            GetWindowText(currentWindowHandle, windowTitle, size);
            string titleText = windowTitle.ToString();
            var foundHyphen = titleText.LastIndexOf('-');
            if (foundHyphen == -1)
            {
                return titleText;   // if hyphen is not there - give entire title
            }
            return titleText.Substring(0, foundHyphen - 1); // if hyphen is there - give string before hyphen
        }

        public static string getFileName()
        {
            const int size = 256;
            StringBuilder windowTitle = new StringBuilder(size);
            IntPtr currentWindowHandle = GetForegroundWindow();
            GetWindowText(currentWindowHandle, windowTitle, size);
            string titleText = windowTitle.ToString();
            Thread.Sleep(500);
            var foundHyphen = titleText.LastIndexOf('-');
            if (foundHyphen == -1)
            {
                return titleText;   // if hyphen is not there - give entire title
            }
            return titleText.Substring(0, foundHyphen - 1); // if hyphen is there - give string before hyphen
        }

        public static void querySystemIndex(string fileName, string classifiedLocation)
        {
            if(!String.IsNullOrEmpty(fileName) && !String.IsNullOrWhiteSpace(fileName))
            {
                var sw = new Stopwatch();
                sw.Start();
                OleDbConnection conn = new OleDbConnection("Provider=Search.CollatorDSO;Extended Properties='Application=Windows';");
                OleDbDataReader rdr = null;
                conn.Open();
                string query = "SELECT System.ItemPathDisplay FROM SYSTEMINDEX WHERE SCOPE='" +
                    classifiedLocation + "' AND System.ItemPathDisplay LIKE '%\\" + fileName + "%'";
                OleDbCommand cmd = new OleDbCommand(query, conn);
                Console.WriteLine(query);
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        Console.WriteLine(rdr[0]);
                    }
                }
                else
                    Console.WriteLine(fileName);
                sw.Stop();
                Console.WriteLine("\n---------- Processed in " + sw.Elapsed.TotalSeconds + " seconds ----------\r\n");
                rdr.Close();
                conn.Close();
            }
        }
    }
}
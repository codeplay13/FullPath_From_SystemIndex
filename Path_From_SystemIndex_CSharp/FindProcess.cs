using System;
using System.Runtime.InteropServices;
using System.Data.OleDb;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace full_path_csharp
{
    class FindProcess
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out int ProcessId);
        public delegate bool WindowEnumProc(IntPtr hwnd, IntPtr lparam);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc callback, IntPtr lParam);

        static Process _realProcess;

        public static string getActiveProcess(IntPtr currentWindowHandle)
        {
            int pid;
            //IntPtr currentWindowHandle = GetForegroundWindow();
            GetWindowThreadProcessId(currentWindowHandle, out pid);
            var foregroundProcess = Process.GetProcessById(pid);
            string app = null;
            if (foregroundProcess.ProcessName == "ApplicationFrameHost")
            {
                foregroundProcess = GetRealProcess(foregroundProcess);
            }
            if (foregroundProcess != null)
            {
                app = foregroundProcess.ProcessName;
            }
            return app;
        }

        public static Process GetRealProcess(Process foregroundProcess)
        {
            EnumChildWindows(foregroundProcess.MainWindowHandle, ChildWindowCallback, IntPtr.Zero);
            return _realProcess;
        }

        public static bool ChildWindowCallback(IntPtr hwnd, IntPtr lparam)
        {
            int pid;
            GetWindowThreadProcessId(hwnd, out pid);
            var process = Process.GetProcessById(pid);
            if (process.ProcessName != "ApplicationFrameHost")
            {
                _realProcess = process;
            }
            return true;
        }
    }
}
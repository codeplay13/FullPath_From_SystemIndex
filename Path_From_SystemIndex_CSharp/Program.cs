using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace full_path_csharp
{
    class Program
    {
        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
        IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr
           hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess,
           uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        // Constants from winuser.h
        const uint EVENT_SYSTEM_FOREGROUND = 3;
        const uint WINEVENT_OUTOFCONTEXT = 0;

        // Need to ensure delegate is not collected while we're using it,
        // storing it in a class field is simplest way to do this.
        static WinEventDelegate procDelegate = new WinEventDelegate(WinEventProc);

        public static void Main()
        {
            // Listen for foreground changes across all processes/threads on current desktop...
            IntPtr hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero,
                    procDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);

            // MessageBox provides the necessary message loop that SetWinEventHook requires.
            MessageBox.Show("Tracking focus, close message box to exit.");
            UnhookWinEvent(hhook);
        }

        static void WinEventProc(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            //Console.WriteLine("Foreground changed to {0:x8}", hwnd.ToInt32());
            FindPath.querySystemIndex(FindPath.getFileNameUsingHandle(hwnd), "file:C:/Users/Abhishek/Dropbox/flowace");
            //Console.WriteLine(FindProcess.getActiveProcess(hwnd));
        }
    }
}
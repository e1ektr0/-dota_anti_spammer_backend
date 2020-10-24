using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GEst
{
    public class LowLevelKeyboardHook
    {
        private const int WhKeyboardLl = 13;
        private const int WmKeydown = 0x0100;
        private const int WmSysKeydown = 0x0104;
        private const int WmKeyup = 0x101;
        private const int WmSysKeyup = 0x105;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public event EventHandler<Keys> OnKeyPressed;
        public event EventHandler<Keys> OnKeyUnpressed;

        private readonly LowLevelKeyboardProc _proc;
        private IntPtr _hookId = IntPtr.Zero;

        public LowLevelKeyboardHook()
        {
            _proc = HookCallback;
        }

        public void HookKeyboard()
        {
            _hookId = SetHook(_proc);
        }

        public void UnHookKeyboard()
        {
            UnhookWindowsHookEx(_hookId);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WhKeyboardLl, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WmKeydown || wParam == (IntPtr)WmSysKeydown)
            {
                var vkCode = Marshal.ReadInt32(lParam);

                OnKeyPressed?.Invoke(this, ((Keys)vkCode));
            }
            else if(nCode >= 0 && wParam == (IntPtr)WmKeyup ||wParam == (IntPtr)WmSysKeyup)
            {
                var vkCode = Marshal.ReadInt32(lParam);

                OnKeyUnpressed?.Invoke(this, ((Keys)vkCode));
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);            
        }
    }
}
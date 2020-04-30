using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HookApplication
{
    class KeyboardHook
    {
        int hHook;

        Win32.HookProc KeyboardHookDelegate;

        public event KeyEventHandler OnKeyDownEvent;

        public event KeyEventHandler OnKeyUpEvent;

        public event KeyPressEventHandler OnKeyPressEvent;

        public KeyboardHook() { }

        public void SetHook()

        {

            KeyboardHookDelegate = new Win32.HookProc(KeyboardHookProc);

            Process cProcess = Process.GetCurrentProcess();

            ProcessModule cModule = cProcess.MainModule;

            var mh = Win32.GetModuleHandle(cModule.ModuleName);

            hHook = Win32.SetWindowsHookEx(Win32.WH_KEYBOARD_LL, KeyboardHookDelegate, mh, 0);

        }

        public void UnHook()

        {

            Win32.UnhookWindowsHookEx(hHook);

        }

        private List<Keys> preKeysList = new List<Keys>();

        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)

        {

            if ((nCode >= 0) && (OnKeyDownEvent != null || OnKeyUpEvent != null || OnKeyPressEvent != null))

            {

                Win32.KeyboardHookStruct KeyDataFromHook = (Win32.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(Win32.KeyboardHookStruct));

                Keys keyData = (Keys)KeyDataFromHook.vkCode;


                if ((OnKeyDownEvent != null || OnKeyPressEvent != null) && (wParam == Win32.WM_KEYDOWN || wParam == Win32.WM_SYSKEYDOWN))

                {

                    if (IsCtrlAltShiftKeys(keyData) && preKeysList.IndexOf(keyData) == -1)

                    {

                        preKeysList.Add(keyData);

                    }

                }

                if (OnKeyDownEvent != null && (wParam == Win32.WM_KEYDOWN || wParam == Win32.WM_SYSKEYDOWN))

                {

                    KeyEventArgs e = new KeyEventArgs(GetDownKeys(keyData));



                    OnKeyDownEvent(this, e);

                }

                if (OnKeyPressEvent != null && wParam == Win32.WM_KEYDOWN)

                {

                    byte[] keyState = new byte[256];

                    Win32.GetKeyboardState(keyState);

                    byte[] inBuffer = new byte[2];

                    if (Win32.ToAscii(KeyDataFromHook.vkCode, KeyDataFromHook.scanCode, keyState, inBuffer, KeyDataFromHook.flags) == 1)

                    {

                        KeyPressEventArgs e = new KeyPressEventArgs((char)inBuffer[0]);

                        OnKeyPressEvent(this, e);

                    }

                }

                //松开控制键

                if ((OnKeyDownEvent != null || OnKeyPressEvent != null) && (wParam == Win32.WM_KEYUP || wParam == Win32.WM_SYSKEYUP))

                {

                    if (IsCtrlAltShiftKeys(keyData))

                    {

                        for (int i = preKeysList.Count - 1; i >= 0; i--)

                        {

                            if (preKeysList[i] == keyData) { preKeysList.RemoveAt(i); }

                        }

                    }

                }

                if (OnKeyUpEvent != null && (wParam == Win32.WM_KEYUP || wParam == Win32.WM_SYSKEYUP))

                {

                    KeyEventArgs e = new KeyEventArgs(GetDownKeys(keyData));

                    OnKeyUpEvent(this, e);

                }

            }

            return Win32.CallNextHookEx(hHook, nCode, wParam, lParam);

        }

        private Keys GetDownKeys(Keys key)

        {

            Keys rtnKey = Keys.None;

            foreach (Keys i in preKeysList)

            {

                if (i == Keys.LControlKey || i == Keys.RControlKey) { rtnKey = rtnKey | Keys.Control; }

                if (i == Keys.LMenu || i == Keys.RMenu) { rtnKey = rtnKey | Keys.Alt; }

                if (i == Keys.LShiftKey || i == Keys.RShiftKey) { rtnKey = rtnKey | Keys.Shift; }

            }

            return rtnKey | key;

        }

        private Boolean IsCtrlAltShiftKeys(Keys key)

        {

            if (key == Keys.LControlKey || key == Keys.RControlKey || key == Keys.LMenu || key == Keys.RMenu || key == Keys.LShiftKey || key == Keys.RShiftKey) { return true; }

            return false;

        }
    }
}

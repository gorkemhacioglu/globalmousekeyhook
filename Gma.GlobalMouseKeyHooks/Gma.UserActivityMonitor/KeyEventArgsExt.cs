﻿using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Gma.UserActivityMonitor.WinApi;

namespace Gma.UserActivityMonitor
{
    public class KeyEventArgsExt : KeyEventArgs
    {
        public KeyEventArgsExt(Keys keyData) : base(keyData)
        {
        }

        internal KeyEventArgsExt(Keys keyData, bool isKeyDown, bool isKeyUp)
            : this(keyData)
        {
            IsKeyDown = isKeyDown;
            IsKeyUp = isKeyUp;
        }

        internal static KeyEventArgsExt FromRawData(int wParam, IntPtr lParam, bool isGlobal)
        {
            return isGlobal ? 
                FromRawDataGlobal(wParam, lParam) : 
                FromRawDataApp(wParam, lParam);
        }

        private static KeyEventArgsExt FromRawDataApp(int wParam, IntPtr lParam)
        {
            //http://msdn.microsoft.com/en-us/library/ms644984(v=VS.85).aspx

            const uint maskKeydown = 0x40000000; // for bit 30
            const uint maskKeyup = 0x80000000; // for bit 31

            uint flags = (uint)lParam; //Marshal.ReadInt32(lParam);
            //bit 30 Specifies the previous key state. The value is 1 if the key is down before the message is sent; it is 0 if the key is up.
            bool wasKeyDown = (flags & maskKeydown) > 0;
            //bit 31 Specifies the transition state. The value is 0 if the key is being pressed and 1 if it is being released.
            bool isKeyReleased = (flags & maskKeyup) > 0;

            Keys keyData = (Keys)wParam;

            bool isKeyDown = !wasKeyDown && !isKeyReleased;
            bool isKeyUp = wasKeyDown && isKeyReleased;

            return new KeyEventArgsExt(keyData, isKeyDown, isKeyUp);
        }

        private static KeyEventArgsExt FromRawDataGlobal(int wParam, IntPtr lParam)
        {
            KeyboardHookStruct keyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
            Keys keyData = (Keys)keyboardHookStruct.VirtualKeyCode;
            bool isKeyDown = Keyboard.IsKeyDown(wParam);
            bool isKeyUp = Keyboard.IsKeyUp(wParam);
            
            return new KeyEventArgsExt(keyData, isKeyDown, isKeyUp);
        }

        public bool IsKeyDown { get; private set; }
        public bool IsKeyUp { get; private set; }
    }
}
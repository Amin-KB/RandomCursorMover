using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RandomCursorMover;

public class Cursor
{
    private  int _seconds;
    private bool _stopMoving = false;

    public Cursor()
    {
        _keyboardProc = HookCallback;
        _hookID = SetHook(_keyboardProc);
    }
    public void SetDuration(int seconds)
    {
        _seconds = seconds;
    }
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int VK_ESCAPE = 0x1B;
    const int SM_CXSCREEN = 0;
    const int SM_CYSCREEN = 1;

    public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private LowLevelKeyboardProc _keyboardProc;
    private IntPtr _hookID = IntPtr.Zero;



    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int X, int Y);


    [DllImport("user32.dll")]
    static extern int GetSystemMetrics(int nIndex);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod,
        uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

  

    public async Task Start()
    {
        Random random = new Random();
        int screenWidth = GetSystemMetrics(SM_CXSCREEN);
        int screenHeight = GetSystemMetrics(SM_CYSCREEN);

        for (int i = 0; i < _seconds; i++)
        {
            
            if (_stopMoving)
            {
                return;
            }
            int randomX = random.Next(0, screenWidth);
            int randomY = random.Next(0, screenHeight);
            SetCursorPos(randomX, randomY);

            Thread.Sleep(1000);
        }
    }



    public IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (var curProcess = Process.GetCurrentProcess())
        using (var curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
        }
    }


    public IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            
            if (vkCode == VK_ESCAPE)
            {
                _stopMoving = true;
                Application.Current.Shutdown();
            }
        }

        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }


    public void MainWindowClosed(object sender, EventArgs e)
    {
        UnhookWindowsHookEx(_hookID);
    }
}
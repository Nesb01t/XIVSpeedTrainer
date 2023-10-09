using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace XIVSpeedTrainer.Libs;

public class Win32
{
    public static IntPtr GetGameHwnd()
    {
        var hwnd = IntPtr.Zero;
        hwnd = FindWindow(null, "最终幻想XIV");
        // GetWindowThreadProcessId(hwnd, out var pid);
        // if (pid == Process.GetCurrentProcess().Id) return hwnd;
        return hwnd;
    }

    public static IntPtr GetProecssHandle(IntPtr hwnd)
    {
        int pid;
        GetWindowThreadProcessId(hwnd, out pid);
        return OpenProcess(0x10 | 0x20 | 0x8 | 0x2, false, (uint)pid);
    }


    public static IntPtr GetModuleBase()
    {
        return Process.GetProcessesByName("ffxiv_dx11")[0].MainModule.BaseAddress;
    }

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

    [DllImport("kernel32.dll")]
    public static extern bool ReadProcessMemory(
        IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool WriteProcessMemory(
        IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);
}

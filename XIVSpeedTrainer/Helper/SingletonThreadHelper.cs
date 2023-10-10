using System;
using System.Threading;

namespace XIVSpeedTrainer.Helper;

public class SingletonThreadHelper
{
    public static bool IsModifying;

    private static Thread Thread = new(ThreadFunc);
    private static bool IsRunning = true;

    private static IntPtr ProcessHandle = IntPtr.Zero;
    private static float Rate = 1.0f;

    public static void ThreadFunc()
    {
        while (IsRunning)
        {
            if (IsModifying && ProcessHandle != IntPtr.Zero &&
                PlayerMemoryHelper.ReadMemAtPlayerSpeed(ProcessHandle) != Rate)
            {
                PlayerMemoryHelper.WriteMemAtPlayerSpeed(ProcessHandle, Rate);
            }
        }
    }

    public static void InitThread(IntPtr hProcess)
    {
        if (ProcessHandle != IntPtr.Zero) return;
        ProcessHandle = hProcess;
        Thread.Start();
        IsModifying = true;
    }

    public static void Dispose()
    {
        IsRunning = false;
    }

    public static void SetRate(float rate)
    {
        Rate = rate;
    }
}

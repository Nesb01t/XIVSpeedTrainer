using System;
using Dalamud.Logging;
using XIVSpeedTrainer.Enums;
using XIVSpeedTrainer.Libs;

namespace XIVSpeedTrainer.Helper;

/// <summary>
/// 内存编辑类
/// </summary>
public class PlayerMemoryHelper
{
    public static float ReadMemAtPlayerSpeed(IntPtr hProcess)
    {
        float value = -1.0f;
        byte[] buffer = new byte[4]; // buffer value
        IntPtr baseAddr = IntPtr.Add(Win32.GetModuleBase(), (int)Constants.OffsetPlayerSpeed);
        if (Win32.ReadProcessMemory(hProcess, baseAddr, buffer, buffer.Length, out int bytesRead))
        {
            value = BitConverter.ToSingle(buffer, 0);
        }

        return value;
    }

    public static bool WriteMemAtPlayerSpeed(IntPtr hProcess, float speedRate)
    {
        const float normalSpeed = 6.0f;
        byte[] buffer = BitConverter.GetBytes(speedRate * normalSpeed); // buffer value
        IntPtr baseAddr = IntPtr.Add(Win32.GetModuleBase(), (int)Constants.OffsetPlayerSpeed);
        if (Win32.WriteProcessMemory(hProcess, baseAddr, buffer, buffer.Length, out int bytesRead))
        {
            PluginLog.Log("write speed access!");
        }

        return true;
    }
}

using System;
using System.Numerics;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using ImGuiNET;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using XIVSpeedTrainer.Libs;

namespace XIVSpeedTrainer.Windows;

public unsafe class MainWindow : Window, IDisposable
{
    private Plugin plugin;

    private IntPtr playerAddress = IntPtr.Zero;
    private BattleChara* battleChara;
    private GameObject* gameObject;

    private int selectedOption = 0;
    private string[] optionsText = { "1.00", "1.05", "1.15", "3.00", "9.99" };
    private IntPtr hwnd;
    private IntPtr hProcess;

    public MainWindow(Plugin plugin) : base(
        "XST 移速测试版", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(500, 100),
            MaximumSize = new Vector2(500, 100)
        };

        this.plugin = plugin;
        this.playerAddress = plugin.ClientState.LocalPlayer.Address;
        this.battleChara =
            (BattleChara*)(void*)plugin.ClientState.LocalPlayer.Address;
        this.gameObject = (GameObject*)(void*)plugin.ClientState.LocalPlayer.Address;
    }

    public void Dispose() { }

    public override void Draw()
    {
        UpdateProcess();
        DrawFunction();
    }

    private void UpdateProcess()
    {
        if (hProcess != 0) return;
        hwnd = Win32.GetGameHwnd();
        hProcess = Win32.GetProecssHandle(hwnd);
    }

    private void DrawFunction()
    {
        ImGui.TextColored(ImGuiColors.HealerGreen, "工具测试用途, 免费使用");
        ImGui.SameLine();
        ImGui.Text("当前移速倍率: " + GetSelectedOption().ToString("F2"));
        ImGui.Separator();
        for (int i = 0; i < optionsText.Length; i++)
        {
            bool isSelected = (selectedOption == i);
            if (ImGui.RadioButton(optionsText[i], isSelected))
            {
                selectedOption = i;
                float rate = GetSelectedOption();
                SetSpeedMemory(rate);
            }

            ImGui.SameLine();
        }
    }

    private void DrawDebugger()
    {
        ImGui.Text(hwnd.ToString());
        ImGui.Text(hProcess.ToString());
        ImGui.Text(GetSpeedMemory().ToString("F2"));
    }

    public float GetSelectedOption()
    {
        switch (selectedOption)
        {
            case 0:
                return 1.0f;
            case 1:
                return 1.05f;
            case 2:
                return 1.15f;
            case 3:
                return 3.0f;
            case 4:
                return 9.99f;
        }

        return 1.0f;
    }


    /// <summary>
    /// 获取移速
    /// </summary>
    /// <returns></returns>
    private float GetSpeedMemory()
    {
        float value = -1.0f;
        byte[] buffer = new byte[4]; // buffer value
        IntPtr baseAddr = IntPtr.Add(Win32.GetModuleBase(), 0x214A218);
        if (Win32.ReadProcessMemory(hProcess, baseAddr, buffer, buffer.Length, out int bytesRead))
        {
            value = BitConverter.ToSingle(buffer, 0);
        }

        return value;
    }

    /// <summary>
    /// 修改移速
    /// </summary>
    /// <returns></returns>
    private bool SetSpeedMemory(float speedRate)
    {
        const float normalSpeed = 6.0f;
        byte[] buffer = BitConverter.GetBytes(speedRate * normalSpeed); // buffer value
        IntPtr baseAddr = IntPtr.Add(Win32.GetModuleBase(), 0x214A218);
        if (Win32.WriteProcessMemory(hProcess, baseAddr, buffer, buffer.Length, out int bytesRead))
        {
            PluginLog.Log("write speed access!");
        }

        return true;
    }
}

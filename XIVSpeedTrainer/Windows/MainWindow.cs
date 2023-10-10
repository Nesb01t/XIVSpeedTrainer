using System;
using System.Numerics;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using ImGuiNET;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using XIVSpeedTrainer.Enums;
using XIVSpeedTrainer.Helper;
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
            MinimumSize = new Vector2(350, 100),
            MaximumSize = new Vector2(350, 100)
        };
        this.plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        InitPlayerInfo();
        InitProcess();
        DrawFunction();
        // DrawDebugger();
    }

    private void InitPlayerInfo()
    {
        if (playerAddress != IntPtr.Zero) return; // init flag
        this.playerAddress = plugin.ClientState.LocalPlayer.Address;
        this.battleChara =
            (BattleChara*)(void*)plugin.ClientState.LocalPlayer.Address;
        this.gameObject = (GameObject*)(void*)plugin.ClientState.LocalPlayer.Address;
    }

    private void InitProcess()
    {
        if (hProcess != 0) return; // init flag
        hwnd = Win32.GetGameHwnd();
        hProcess = Win32.GetProecssHandle(hwnd);
    }

    private void DrawFunction()
    {
        ImGui.TextColored(ImGuiColors.HealerGreen, "工具测试用途, 免费使用");
        ImGui.SameLine();
        ImGui.Text("当前移速倍率: " + Constants.GetSelectedOption(selectedOption).ToString("F2"));
        ImGui.Separator();
        for (int i = 0; i < optionsText.Length; i++)
        {
            bool isSelected = (selectedOption == i);
            if (ImGui.RadioButton(optionsText[i], isSelected))
            {
                selectedOption = i;
                float rate = Constants.GetSelectedOption(selectedOption);
                PlayerMemoryHelper.WriteMemAtPlayerSpeed(hProcess, rate);
            }

            ImGui.SameLine();
        }
    }

    private void DrawDebugger()
    {
        ImGui.Text(hwnd.ToString());
        ImGui.Text(hProcess.ToString());
        ImGui.Text(PlayerMemoryHelper.ReadMemAtPlayerSpeed(hProcess).ToString("F2"));
    }
}

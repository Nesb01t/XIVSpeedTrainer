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

public unsafe class Main : Window, IDisposable
{
    private Plugin plugin;

    private IntPtr playerAddress = IntPtr.Zero;
    private BattleChara* battleChara;
    private GameObject* gameObject;

    private IntPtr hwnd;
    private IntPtr hProcess;

    private int selectedOption;
    private string[] optionsText = { "1.00", "1.05", "1.15", "3.00", "9.99" };


    public Main(Plugin plugin) : base(
        "XST 移速测试版", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(350, 100),
            MaximumSize = new Vector2(350, 100)
        };
        this.plugin = plugin;
    }

    public void Dispose()
    {
        SingletonThreadHelper.Dispose();
    }

    public override void Draw()
    {
        InitPlayerInfo();
        InitProcess();
        InitSingletonThread();
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

    private void InitSingletonThread()
    {
        if (hProcess == 0) return;
        if (playerAddress == IntPtr.Zero) return;
        SingletonThreadHelper.InitThread(hProcess);
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
                HandleRadioButton(i);
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

    private void HandleRadioButton(int i)
    {
        selectedOption = i;
        float rate = Constants.GetSelectedOption(selectedOption);
        SingletonThreadHelper.SetRate(rate);
    }
}

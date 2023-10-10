using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Game.ClientState;
using Dalamud.Interface.Windowing;
using XIVSpeedTrainer;
using XIVSpeedTrainer.Windows;

namespace XIVSpeedTrainer
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "XIVSpeedTrainer";
        private const string CommandName = "/xst";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        public ClientState ClientState { get; init; }
        public WindowSystem WindowSystem = new("XIVSpeedTrainer");

        private Main Main { get; init; }

        public Plugin(
            DalamudPluginInterface pluginInterface,
            CommandManager commandManager,
            ClientState clientState)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            this.ClientState = clientState;

            Main = new Main(this);
            WindowSystem.AddWindow(Main);

            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "usage: command /xst"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUi;
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();

            Main.Dispose();

            this.CommandManager.RemoveHandler(CommandName);
        }

        private void OnCommand(string command, string args)
        {
            Main.IsOpen = true;
        }

        private void DrawUi()
        {
            this.WindowSystem.Draw();
        }
    }
}

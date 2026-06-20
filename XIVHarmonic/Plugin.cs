using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using XIVHarmonic.Windows;

namespace XIVHarmonic;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IPlayerState PlayerState { get; private set; } = null!;
    [PluginService] internal static IObjectTable ObjectTable { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    [PluginService] internal static IFramework Framework { get; private set; } = null!;

    private const string CommandName = "/harmonic";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("XIVHarmonic");
    private MainWindow MainWindow { get; init; }

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        MainWindow = new MainWindow(this);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
            HelpMessage = "Opens the configuration window.",
        });

        PluginInterface.UiBuilder.Draw += WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleMainUi;
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;
        
        Framework.Update += OnFrameworkTick;
        
        OrchestrionIpc.Initialize();
    }

    public void Dispose()
    {
        PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleMainUi;
        PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUi;
        
        Framework.Update -= OnFrameworkTick;
        
        WindowSystem.RemoveAllWindows();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        ToggleMainUi();
    }

    public void ToggleMainUi()
    {
        MainWindow.Toggle();
    }

    public uint[] PlayerEffects()
    {
        return ObjectTable.LocalPlayer == null ? [] : 
                   ObjectTable.LocalPlayer.StatusList.Select(x => x.StatusId).ToArray();
    }

    public bool IsInCombat()
    {
        return ObjectTable.LocalPlayer != null &&
                   (ObjectTable.LocalPlayer.StatusFlags & StatusFlags.InCombat) != 0;
    }

    private bool CheckCondition(Condition condition)
    {
        bool conditionMet = true;
            
        if (condition.weatherTest > 0 &&
            GameData.CurrentWeatherId() != (uint)condition.weatherTest) conditionMet = false;
        else if (condition.areaTest > 0 &&
                 GameData.CurrentAreaId() != (uint)condition.areaTest) conditionMet = false;
        else if (condition.statusTest > 0 &&
                 !PlayerEffects().Contains<uint>((uint)condition.statusTest)) conditionMet = false;
        else if (condition.combatTest == 1 && !IsInCombat()) conditionMet = false;
        else if (condition.combatTest == 2 && IsInCombat()) conditionMet = false;
        
        if (!condition.entityNameTest.IsNullOrEmpty())
        {
            var candidates = ObjectTable.Where(x =>
                x.Name.ToString().Contains(condition.entityNameTest));
            if (!candidates.Any()) conditionMet = false;
            if (condition.entityProximityTest > 0)
            {
                if (ObjectTable.LocalPlayer == null) conditionMet = false;
                else
                {
                    candidates = candidates.Where(x =>
                        Vector3.Distance(ObjectTable.LocalPlayer.Position, x.Position) <= condition.entityProximityTest);
                    if (!candidates.Any()) conditionMet = false;
                }
            }
        }
        return conditionMet;
    }

    private int tickTimer = 0;
    private void OnFrameworkTick(IFramework framework)
    {
        if (tickTimer++ < 15) return;
        tickTimer = 0;
        
        int currentSong = OrchestrionIpc.CurrentSong();
        foreach (var condition in Configuration.Conditions)
        {
            if (CheckCondition(condition))
            {
                if (currentSong != condition.targetSong)
                {
                    OrchestrionIpc.Play(condition.targetSong);
                }
            }
            else
            {
                if (condition.disableIfInactive &&
                    OrchestrionIpc.CurrentSong() == condition.targetSong)
                {
                    OrchestrionIpc.Stop();
                }
            }
        }
    }
}

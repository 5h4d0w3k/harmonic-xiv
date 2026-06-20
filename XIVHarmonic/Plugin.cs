using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Chat;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
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
    [PluginService] internal static IChatGui Chat { get; private set; } = null!;

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
            
        Chat.ChatMessage += OnChatMessage;
        
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
        var conditionMet = true;
            
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
    private uint lastAreaId = 0;
    
    internal void OnFrameworkTick(IFramework framework)
    {
        if (tickTimer++ < Configuration.PollingInterval) return;
        tickTimer = 0;
        
        var currentSong = OrchestrionIpc.CurrentSong();
        var currentAreaId = GameData.CurrentAreaId();
        
        foreach (var condition in Configuration.Conditions)
        {
            if (!condition.chatLogTest.IsNullOrEmpty() ||
                condition.areaLeaveTest > 0)
            {
                // Chat log and area change triggers are handled separately
                continue;
            }
            if (CheckCondition(condition))
            {
                if (currentSong != condition.targetSong)
                {
                    Log.Verbose("Basic condition fired: " + condition.ToString());
                    OrchestrionIpc.Play(condition.targetSong);
                }
            }
            else
            {
                if (condition.disableIfInactive &&
                    OrchestrionIpc.CurrentSong() == condition.targetSong)
                {
                    Log.Verbose("Basic condition stopped: " + condition.ToString());
                    OrchestrionIpc.Stop();
                }
            }
        }

        if (currentAreaId != lastAreaId)
        {
            foreach (var condition in Configuration.Conditions)
            {
                if (condition.areaLeaveTest > 0 &&
                    condition.areaLeaveTest == lastAreaId &&
                    CheckCondition(condition))
                {
                    Log.Verbose("Trigger on area change fired: " + condition.ToString());
                    OrchestrionIpc.Play(condition.targetSong);
                }
            }
            lastAreaId = currentAreaId;
        }
    }

    internal void OnChatMessage(IHandleableChatMessage message)
    {
        foreach (var condition in Configuration.Conditions)
        {
            if (!condition.chatLogTest.IsNullOrEmpty())
            {
                if (message.Message.ToString().Contains(condition.chatLogTest) &&
                    CheckCondition(condition))
                {
                    Log.Verbose("Trigger on chat message fired: " + condition.ToString());
                    OrchestrionIpc.Play(condition.targetSong);
                }
            }
        }
    }
}

using System;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using Lumina.Excel.Sheets;

namespace XIVHarmonic.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly Plugin plugin;

    private readonly string[] combatStates = { "In combat", "Not in combat" };

    public MainWindow(Plugin plugin)
        : base("Harmonic Configuration")
    {
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(600, 400),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.plugin = plugin;
    }

    public void Dispose() { }
    
    private int _weatherTest;
    private int _areaTest;
    private int _statusTest;
    private int _combatTest;
    private int _entityProximityTest;
    private string _entityNameTest = "";
    private string _chatLogTest = "";

    private bool _weatherTestActive;
    private bool _areaTestActive;
    private bool _statusTestActive;
    private bool _combatTestActive;
    private bool _entityProximityTestActive;
    private bool _entityNameTestActive;
    private bool _chatLogTestActive;
    
    private int _targetSong;
    private int _targetSongAction;
    private bool _disableIfInactive;

    private ImGuiTabItemFlags activeConditionFlags = ImGuiTabItemFlags.None;
    
    public override void Draw()
    {
        if (ImGui.BeginTabBar("HarmonicTabs"))
        {
            // Active conditions tab
            if (ImGui.BeginTabItem("Active conditions", activeConditionFlags))
            {
                activeConditionFlags = ImGuiTabItemFlags.None;
                if (ImGui.BeginTable("EntriesTable", 3,
                    ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
                {
                    ImGui.TableSetupColumn("Conditions");
                    ImGui.TableSetupColumn("Actions");
                    ImGui.TableSetupColumn("Status", ImGuiTableColumnFlags.WidthFixed, 50);
                    ImGui.TableHeadersRow();

                    foreach (var condition in plugin.Configuration.Conditions)
                    {
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.Text(condition.ToIfString());
                        ImGui.TableSetColumnIndex(1);
                        ImGui.Text(condition.ToThenString());
                        ImGui.TableSetColumnIndex(2);
                        ImGui.Button("Delete");
                    }

                    ImGui.EndTable();
                }

                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("New condition"))
            {

                ImGui.BeginTable("EntriesTable", 2);
                
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Checkbox("If current weather is: ", ref _weatherTestActive);
                ImGui.TableSetColumnIndex(1);
                ImGui.Combo("##condWeather", ref _weatherTest, GameData.WeatherNames, GameData.WeatherNames.Count);
                ImGui.SameLine();
                ImGui.Button("Current");

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Checkbox("If current area is: ", ref _areaTestActive);
                ImGui.TableSetColumnIndex(1);
                ImGui.Combo("##condArea", ref _areaTest, GameData.AreaNames, GameData.AreaNames.Count);
                ImGui.SameLine();
                ImGui.Button("Current");
                
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Checkbox("If affected by status: ", ref _statusTestActive);
                ImGui.TableSetColumnIndex(1);
                ImGui.Combo("##condStatus", ref _statusTest, GameData.StatusNames, GameData.StatusNames.Count);
                ImGui.SameLine();
                ImGui.Button("Current");
                
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Checkbox("If in combat state:", ref _combatTestActive);
                ImGui.TableSetColumnIndex(1);
                ImGui.Combo("##condCombat", ref _combatTest, combatStates, combatStates.Length);
                
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Checkbox("If entity is in proximity:", ref _entityNameTestActive);
                ImGui.TableSetColumnIndex(1);
                ImGui.InputText("##condEntityName", ref _entityNameTest, 256);
                
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Checkbox("within this distance:", ref _entityProximityTestActive);
                ImGui.TableSetColumnIndex(1);
                ImGui.InputInt("##condEntityDist", ref _entityProximityTest, 1);
                
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Checkbox("If chat log message contains:", ref _chatLogTestActive);
                ImGui.TableSetColumnIndex(1);
                ImGui.InputText("##condChatLog", ref _chatLogTest, 256);
                
                ImGui.EndTable();
                
                ImGui.Separator();
                
                ImGui.RadioButton("Then play the following track: ", ref _targetSongAction, 0);
                ImGui.SameLine();
                ImGui.Combo("##condStatus", ref _targetSong, GameData.SongNames, GameData.SongNames.Count);

                ImGui.RadioButton("Then stop playing custom tracks", ref _targetSongAction, 1);
                
                ImGui.Checkbox("Stop playing once condition is not met", ref _disableIfInactive);
                
                ImGui.Separator();
                
                if (ImGui.Button("Save"))
                {
                    Condition cond = new();
                    if (_weatherTestActive) cond.weatherTest = (int) GameData.WeatherIds[_weatherTest];
                    if (_areaTestActive) cond.areaTest = (int) GameData.AreaIds[_areaTest];
                    if (_statusTestActive) cond.statusTest = (int) GameData.StatusIds[_statusTest];
                    if (_combatTestActive) cond.combatTest = _combatTest;
                    if (_entityNameTestActive) cond.entityNameTest = _entityNameTest;
                    if (_entityProximityTestActive) cond.entityProximityTest = _entityProximityTest;
                    if (_chatLogTestActive) cond.chatLogTest = _chatLogTest;
                    plugin.Configuration.Conditions.Add(cond);
                    plugin.Configuration.Save();
                    activeConditionFlags = ImGuiTabItemFlags.SetSelected;
                }

                ImGui.EndTabItem();
            }

            // Configuration tab
            if (ImGui.BeginTabItem("Settings"))
            {
                // todo
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
    }
}

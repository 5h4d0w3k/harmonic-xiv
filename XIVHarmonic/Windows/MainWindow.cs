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
    
    public override void Draw()
    {
        if (ImGui.BeginTabBar("HarmonicTabs"))
        {
            // Active conditions tab
            if (ImGui.BeginTabItem("Active conditions"))
            {
                if (ImGui.BeginTable("EntriesTable", 3,
                    ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
                {
                    ImGui.TableSetupColumn("Condition", ImGuiTableColumnFlags.WidthStretch, 0.6f);
                    ImGui.TableSetupColumn("Action");
                    ImGui.TableSetupColumn("Status", ImGuiTableColumnFlags.WidthFixed, 100);
                    ImGui.TableHeadersRow();

                    for (int i = 0; i < 33; i++)
                    {
                        ImGui.TableNextRow();

                        ImGui.TableSetColumnIndex(0);
                        ImGui.Text($"If weather is: Rain\nIf area is: gdfklgjdfg{i}\nIf status is: In battle");

                        ImGui.TableSetColumnIndex(1);
                        ImGui.Text("Play track #66 (Bee My Honey)\nStop playing if conditions not met");

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

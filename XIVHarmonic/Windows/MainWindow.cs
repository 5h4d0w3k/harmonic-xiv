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
    
    private static string _conditionString = string.Empty;

    private static bool _checkbox1;
    private static bool _checkbox2;
    private static bool _checkbox3;
    
    private static int _condWeather;
    private static int _condArea;
    private static int _condStatus;
    private static int _condDistance = 999;
    private static int _condAction;
    
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
                ImGui.Checkbox("If current weather is: ", ref _checkbox1);
                ImGui.TableSetColumnIndex(1);
                ImGui.Combo("##condWeather", ref _condWeather, GameData.WeatherNames, GameData.WeatherNames.Count);
                ImGui.SameLine();
                ImGui.Button("Current");

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Checkbox("If current area is: ", ref _checkbox2);
                ImGui.TableSetColumnIndex(1);
                ImGui.Combo("##condArea", ref _condArea, GameData.AreaNames, GameData.AreaNames.Count);
                ImGui.SameLine();
                ImGui.Button("Current");
                
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Checkbox("If affected by status: ", ref _checkbox3);
                ImGui.TableSetColumnIndex(1);
                ImGui.Combo("##condStatus", ref _condStatus, GameData.StatusNames, GameData.StatusNames.Count);
                ImGui.SameLine();
                ImGui.Button("Current");
                
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Checkbox("If in combat state:", ref _checkbox2);
                ImGui.TableSetColumnIndex(1);
                ImGui.Combo("##condCombat", ref _condStatus, combatStates, combatStates.Length);
                
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Checkbox("If entity is in proximity:", ref _checkbox2);
                ImGui.TableSetColumnIndex(1);
                ImGui.InputText("##condEntityName", ref _conditionString, 256);
                
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Checkbox("within this distance:", ref _checkbox2);
                ImGui.TableSetColumnIndex(1);
                ImGui.InputInt("##condEntityDist", ref _condDistance, 1);
                
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Checkbox("If chat log message contains:", ref _checkbox2);
                ImGui.TableSetColumnIndex(1);
                ImGui.InputText("##condChatLog", ref _conditionString, 256);
                
                ImGui.EndTable();
                
                ImGui.Separator();
                
                ImGui.RadioButton("Then play the following track: ", ref _condAction, 0);
                ImGui.SameLine();
                ImGui.Combo("##condStatus", ref _condStatus, GameData.SongNames, GameData.SongNames.Count);

                ImGui.RadioButton("Then stop playing custom tracks", ref _condAction, 1);
                
                ImGui.Checkbox("Stop playing once condition is not met", ref _checkbox2);
                
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
                ImGui.Checkbox("Checkbox 1", ref _checkbox1);
                ImGui.Checkbox("Checkbox 2", ref _checkbox2);
                ImGui.Checkbox("Checkbox 3", ref _checkbox3);

                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
    }
}

using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Lumina.Excel.Sheets;

namespace XIVHarmonic.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly Plugin plugin;

    public MainWindow(Plugin plugin)
        : base("Harmonic Configuration")
    {
        SizeConstraints = new WindowSizeConstraints
        {
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
                // todo
                ImGui.InputText("Condition string", ref _conditionString, 256);
                if (ImGui.Button("Save"))
                {
                }

                ImGui.EndTabItem();
            }

            // Configuration tab
            if (ImGui.BeginTabItem("Configuration"))
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

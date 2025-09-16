using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICE.Ui.DebugWindowTabs
{
    internal class Table_GatheringInfo
    {
        private static string MissionSearchText = "";

        public static unsafe void Draw()
        {
            ImGui.SetNextItemWidth(250);
            ImGui.InputText("Search by Name", ref MissionSearchText, 100);

            ImGuiTableFlags tableFlags = ImGuiTableFlags.RowBg |
                            ImGuiTableFlags.Borders |
                            ImGuiTableFlags.SizingFixedFit |
                            ImGuiTableFlags.Resizable |           // Allow column resizing
                            ImGuiTableFlags.Reorderable |         // Allow column reordering
                            ImGuiTableFlags.Hideable;             // Allow hiding columns via right-click

            if (ImGui.BeginTable("Mission_GatheringInfo", 8, tableFlags))
            {
                ImGui.TableSetupColumn("Key");
                ImGui.TableSetupColumn("Mission Name");
                for (int i = 1; i < 4; i++)
                {
                    ImGui.TableSetupColumn($"Gather Item [{i}]");
                    ImGui.TableSetupColumn($"Amount [{i}]");
                }
                ImGui.TableHeadersRow();

                foreach (var entry in CosmicHelper.SheetMissionDict.Where(x => x.Value.Jobs.Overlaps(CosmicHelper.GatheringJobList)))
                {
                    if (!string.IsNullOrEmpty(MissionSearchText) 
                     && !entry.Value.Name.Replace(" ", "").Contains(MissionSearchText.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    ImGui.TableNextRow();

                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text($"[{entry.Key}]");

                    ImGui.TableNextColumn();
                    ImGui.Text($"{entry.Value.Name}");

                    foreach (var item in entry.Value.Gathering_Min)
                    {
                        ImGui.TableNextColumn();
                        var itemName = Svc.Data.GetExcelSheet<Item>().GetRow(item.Key).Name;
                        ImGui.Text($"{itemName}");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text($"Id: {item.Key}");
                            ImGui.EndTooltip();
                        }

                        ImGui.TableNextColumn();
                        ImGui.Text($"{item.Value}");
                    }


                }

                ImGui.EndTable();
            }
        }
    }
}

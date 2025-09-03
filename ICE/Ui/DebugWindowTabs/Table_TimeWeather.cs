using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICE.Ui.DebugWindowTabs
{
    internal class Table_TimeWeather
    {
        public static unsafe void Draw()
        {
            var timeSheet = Svc.Data.GetExcelSheet<WKSMissionLotterySpecialCond>();

            if (ImGui.BeginTable($"WKSMission Time Sheet", 4, ImGuiTableFlags.SizingFixedFit))
            {
                ImGui.TableSetupColumn("Key");
                ImGui.TableSetupColumn("Unknown 0");
                ImGui.TableSetupColumn("Unknown 1");
                ImGui.TableSetupColumn("Unknown 2");

                ImGui.TableHeadersRow();

                foreach (var entry in timeSheet)
                {
                    ImGui.TableNextRow();

                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text($"{entry.RowId}");

                    ImGui.TableNextColumn();
                    ImGui.Text($"{entry.Unknown0}");

                    ImGui.TableNextColumn();
                    ImGui.Text($"{entry.Unknown1}");

                    ImGui.TableNextColumn();
                    ImGui.Text($"{entry.Unknown2}");

                }

                ImGui.EndTable();
            }
        }
    }
}

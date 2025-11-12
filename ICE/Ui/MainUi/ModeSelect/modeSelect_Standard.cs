using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ICE.Ui.MainUi.ModeSelect
{
    internal class modeSelect_Standard
    {
        private string RankSelected = "Critical";

        public static void Draw()
        {
            using var style = ImRaii.PushStyle(ImGuiStyleVar.ChildRounding, 10).Push(ImGuiStyleVar.ChildBorderSize, 1);

            // Header at the top
            using (var headerChild = ImRaii.Child("##modeSelect_StandardHeader", new Vector2(0, 45), true, ImGuiWindowFlags.NoScrollbar))
            {
                if (!headerChild.Success) return;

                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 5);

                ImGuiEx.IconWithText(FontAwesomeIcon.List, "Standard Mode");
            }

            using (var bodyChild = ImRaii.Child("##modeSelect_Body", new Vector2(0, -1), true))
            {
                if (!bodyChild.Success) return;

                foreach (var missionType in modeSelect_TableInfo.missionList)
                {
                    missionType.Value.Clear();
                }

                foreach (var mission in CosmicHelper.SheetMissionDict)
                {
                    var Jobs = mission.Value.Jobs;
                    var territoryId = mission.Value.TerritoryId;
                    uint selectedJob = C.SelectedJob;
                    bool sinusEnabled = C.ShowSinusMissions;
                    bool phaennaEnabled = C.ShowPhaennaMissions;

                    if (!Jobs.Contains(selectedJob))
                        continue;

                    if (!sinusEnabled && territoryId == 1237)
                        continue;

                    if (!phaennaEnabled && territoryId == 1291)
                        continue;

                    if (mission.Value.Attributes.HasFlag(MissionAttributes.Critical))
                        modeSelect_TableInfo.missionList["Critical"].Add(new modeSelect_TableInfo.Mission { id = mission.Key, enabled = C.MissionConfig[mission.Key].Enabled });
                    else if (mission.Value.Attributes.HasFlag(MissionAttributes.ProvisionalWeather))
                        modeSelect_TableInfo.missionList["Weather"].Add(new modeSelect_TableInfo.Mission { id = mission.Key, enabled = C.MissionConfig[mission.Key].Enabled });
                    else if (mission.Value.Attributes.HasFlag(MissionAttributes.ProvisionalTimed))
                        modeSelect_TableInfo.missionList["Timed"].Add(new modeSelect_TableInfo.Mission { id = mission.Key, enabled = C.MissionConfig[mission.Key].Enabled });
                    else if (mission.Value.Attributes.HasFlag(MissionAttributes.ProvisionalSequential))
                        modeSelect_TableInfo.missionList["Sequence"].Add(new modeSelect_TableInfo.Mission { id = mission.Key, enabled = C.MissionConfig[mission.Key].Enabled });
                    else if (mission.Value.Rank > 3)
                        modeSelect_TableInfo.missionList["ARank"].Add(new modeSelect_TableInfo.Mission { id = mission.Key, enabled = C.MissionConfig[mission.Key].Enabled });
                    else if (mission.Value.Rank == 3)
                        modeSelect_TableInfo.missionList["BRank"].Add(new modeSelect_TableInfo.Mission { id = mission.Key, enabled = C.MissionConfig[mission.Key].Enabled });
                    else if (mission.Value.Rank == 2)
                        modeSelect_TableInfo.missionList["CRank"].Add(new modeSelect_TableInfo.Mission { id = mission.Key, enabled = C.MissionConfig[mission.Key].Enabled });
                    else if (mission.Value.Rank == 1)
                        modeSelect_TableInfo.missionList["DRank"].Add(new modeSelect_TableInfo.Mission { id = mission.Key, enabled = C.MissionConfig[mission.Key].Enabled });

                    if (C.MissionConfig.ContainsKey(mission.Key) && C.MissionConfig[mission.Key].Enabled)
                    {
                        modeSelect_TableInfo.missionList["All Enabled"].Add(new modeSelect_TableInfo.Mission { id = mission.Key, enabled = C.MissionConfig[mission.Key].Enabled });
                    }
                }

                int criticalEnabled = modeSelect_TableInfo.missionList.ContainsKey("Critical") ? modeSelect_TableInfo.missionList["Critical"].Count(mission => mission.enabled) : 0;
                int sequenceEnabled = modeSelect_TableInfo.missionList.ContainsKey("Sequence") ? modeSelect_TableInfo.missionList["Sequence"].Count(mission => mission.enabled) : 0;
                int weatherEnabled = modeSelect_TableInfo.missionList.ContainsKey("Weather") ? modeSelect_TableInfo.missionList["Weather"].Count(mission => mission.enabled) : 0;
                int timedEnabled = modeSelect_TableInfo.missionList.ContainsKey("Timed") ? modeSelect_TableInfo.missionList["Timed"].Count(mission => mission.enabled) : 0;
                int aRankEnabled = modeSelect_TableInfo.missionList.ContainsKey("ARank") ? modeSelect_TableInfo.missionList["ARank"].Count(mission => mission.enabled) : 0;
                int bRankEnabled = modeSelect_TableInfo.missionList.ContainsKey("BRank") ? modeSelect_TableInfo.missionList["BRank"].Count(mission => mission.enabled) : 0;
                int cRankEnabled = modeSelect_TableInfo.missionList.ContainsKey("CRank") ? modeSelect_TableInfo.missionList["CRank"].Count(mission => mission.enabled) : 0;
                int dRankEnabled = modeSelect_TableInfo.missionList.ContainsKey("DRank") ? modeSelect_TableInfo.missionList["DRank"].Count(mission => mission.enabled) : 0;
                int allEnabled = modeSelect_TableInfo.missionList.ContainsKey("All Enabled") ? modeSelect_TableInfo.missionList["All Enabled"].Count(mission => mission.enabled) : 0;

                modeSelect_TableInfo.DrawTabButton($"All Enabled [{allEnabled}]", "main_AllEnabled");
                ImGui.SameLine(0, 5);
                modeSelect_TableInfo.DrawTabButton($"Critical [{criticalEnabled}]", "main_Critical");
                ImGui.SameLine(0, 5);
                modeSelect_TableInfo.DrawTabButton($"Sequence [{sequenceEnabled}]", "main_Sequence");
                ImGui.SameLine(0, 5);
                modeSelect_TableInfo.DrawTabButton($"Weather [{weatherEnabled}]", "main_Weather");
                ImGui.SameLine(0, 5);
                modeSelect_TableInfo.DrawTabButton($"Timed [{timedEnabled}]", "main_Timed");
                ImGui.SameLine(0, 5);
                modeSelect_TableInfo.DrawTabButton($"A Rank [{aRankEnabled}]", "main_ARank");
                ImGui.SameLine(0, 5);
                modeSelect_TableInfo.DrawTabButton($"B Rank [{bRankEnabled}]", "main_BRank");
                ImGui.SameLine(0, 5);
                modeSelect_TableInfo.DrawTabButton($"C Rank [{cRankEnabled}]", "main_CRank");
                ImGui.SameLine(0, 5);
                modeSelect_TableInfo.DrawTabButton($"D Rank [{dRankEnabled}]", "main_DRank");

                ImGui.Separator();

                var enabledTabs = modeSelect_TableInfo.selectedTabs;
                if (enabledTabs.Contains("main_AllEnabled"))
                    modeSelect_TableInfo.DrawMissionTable("All Enabled", modeSelect_TableInfo.SortMissionList(modeSelect_TableInfo.missionList["All Enabled"]));
                if (enabledTabs.Contains("main_Critical"))
                    modeSelect_TableInfo.DrawMissionTable("Critical", modeSelect_TableInfo.SortMissionList(modeSelect_TableInfo.missionList["Critical"]));
                if (enabledTabs.Contains("main_Sequence"))
                    modeSelect_TableInfo.DrawMissionTable("Sequence", modeSelect_TableInfo.SortMissionList(modeSelect_TableInfo.missionList["Sequence"]));
                if (enabledTabs.Contains("main_Weather"))
                    modeSelect_TableInfo.DrawMissionTable("Weather", modeSelect_TableInfo.SortMissionList(modeSelect_TableInfo.missionList["Weather"]));
                if (enabledTabs.Contains("main_Timed"))
                    modeSelect_TableInfo.DrawMissionTable("Timed", modeSelect_TableInfo.SortMissionList(modeSelect_TableInfo.missionList["Timed"]));
            }
        }
    }
}

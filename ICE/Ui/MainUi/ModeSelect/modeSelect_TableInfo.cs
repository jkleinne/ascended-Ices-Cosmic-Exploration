using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using ECommons;
using FFXIVClientStructs.FFXIV.Client.Game.WKS;
using ICE.Utilities.Cosmic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace ICE.Ui.MainUi.ModeSelect
{
    internal class modeSelect_TableInfo
    {
        public static HashSet<string> selectedTabs = new HashSet<string>();
        public static uint selectedMission = 0;

        public static Dictionary<string, bool> headerStates = new();

        public static Dictionary<string, List<Mission>> missionList = new()
        {
            ["Critical"] = new List<Mission>(),
            ["Weather"] = new List<Mission>(),
            ["Timed"] = new List<Mission>(),
            ["Sequence"] = new List<Mission>(),
            ["ARank"] = new List<Mission>(),
            ["BRank"] = new List<Mission>(),
            ["CRank"] = new List<Mission>(),
            ["DRank"] = new List<Mission>(),
            ["All Enabled"] = new List<Mission>(),
        };

        public class Mission
        {
            public uint id;
            public bool enabled;
        }

        public static List<Mission> SortMissionList(List<Mission> missions)
        {
            int sortOption = C.TableSortOption;
            var missionInfo = CosmicHelper.SheetMissionDict;

            switch (sortOption)
            {
                case 0: // Sorting by Id
                    return missions.ToList();
                case 1: // Name 
                    return missions.OrderBy(m => missionInfo[m.id].Name).ToList();
                case 2: // Cosmo Credits
                    return missions.OrderByDescending(m => missionInfo[m.id].CosmoCredit).ToList();
                case 3: // Lunar Credits
                    return missions.OrderByDescending(m => missionInfo[m.id].LunarCredit).ToList();
                case 4: // Exp Type 1:
                    return missions.OrderByDescending(m => missionInfo[m.id].RelicXpInfo
                                                     .Where(exp => exp.Key == 1)
                                                     .Sum(exp => exp.Value)).ToList();
                case 5: // Exp Type 2:
                    return missions.OrderByDescending(m => missionInfo[m.id].RelicXpInfo
                                                     .Where(exp => exp.Key == 2)
                                                     .Sum(exp => exp.Value)).ToList();
                case 6: // Exp Type 3:
                    return missions.OrderByDescending(m => missionInfo[m.id].RelicXpInfo
                                                     .Where(exp => exp.Key == 3)
                                                     .Sum(exp => exp.Value)).ToList();
                case 7: // Exp Type 4:
                    return missions.OrderByDescending(m => missionInfo[m.id].RelicXpInfo
                                                     .Where(exp => exp.Key == 4)
                                                     .Sum(exp => exp.Value)).ToList();
                case 8: // Exp Type 5:
                    return missions.OrderByDescending(m => missionInfo[m.id].RelicXpInfo
                                                     .Where(exp => exp.Key == 5)
                                                     .Sum(exp => exp.Value)).ToList();
                case 9: // Map Location
                    return missions.OrderBy(m => missionInfo[m.id].MarkerId).ToList();
                default:
                    return missions.ToList();
            }
        }

        public static void DrawCollapsibleHeader(string id, string label, float spacing = 4f, Vector4? borderColor = null, Vector4? backgroundColor = null)
        {
            const float padding = 6.0f;
            const float borderRadius = 2.0f;

            // Initialize header state if needed
            if (!headerStates.ContainsKey(id))
                headerStates[id] = false;

            // Calculate dimensions
            var drawList = ImGui.GetWindowDrawList();
            var cursorPos = ImGui.GetCursorScreenPos();
            var windowWidth = ImGui.GetContentRegionAvail().X;
            var textSize = ImGui.CalcTextSize(label);
            var bgHeight = textSize.Y + padding * 2;

            // Define header bounds
            var headerRectMin = cursorPos;
            var headerRectMax = new Vector2(cursorPos.X + windowWidth, cursorPos.Y + bgHeight);

            // Use provided colors or defaults
            var bgColor = backgroundColor ?? new Vector4(0.2f, 0.2f, 0.2f, 1f);
            var borderCol = borderColor ?? ImGuiColors.ParsedGold;

            // Draw background and border
            drawList.AddRectFilled(headerRectMin, headerRectMax, ImGui.GetColorU32(bgColor), borderRadius);
            drawList.AddRect(headerRectMin, headerRectMax, ImGui.GetColorU32(borderCol), borderRadius);

            // Draw centered label
            var textPos = new Vector2(
                cursorPos.X + (windowWidth - textSize.X) * 0.5f,
                cursorPos.Y + padding
            );
            drawList.AddText(textPos, ImGui.GetColorU32(new Vector4(1f, 1f, 1f, 1f)), label);

            // Handle interaction
            ImGui.SetCursorScreenPos(cursorPos);
            ImGui.PushID(id);
            ImGui.InvisibleButton("##header", new Vector2(windowWidth, bgHeight));
            if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                headerStates[id] = !headerStates[id];
            ImGui.PopID();

            // Move cursor past header
            ImGui.SetCursorScreenPos(new Vector2(cursorPos.X, cursorPos.Y + bgHeight + spacing));
        }

        public static void DrawCollapsibleSection(string id, string label, int enabled, List<Mission> missions)
        {
            DrawCollapsibleHeader(id, $"{label} | Enabled: {enabled}");
            if (headerStates.TryGetValue(id, out var isOpen) && isOpen)
            {
                // MissionInfoV2(id, SortMissionList(missions));
            }
        }

        public static bool DrawTabButton(string label, string tabIndex)
        {
            if (selectedTabs.Contains(tabIndex))
            {
                var activeColor = ImGui.GetStyle().Colors[(int)ImGuiCol.TabActive];
                ImGui.PushStyleColor(ImGuiCol.Button, activeColor);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, activeColor);
            }

            bool clicked = ImGui.Button(label);

            if (selectedTabs.Contains(tabIndex))
                ImGui.PopStyleColor(2);

            if (clicked)
            {
                if (selectedTabs.Contains(tabIndex))
                    selectedTabs.Remove(tabIndex);
                else
                    selectedTabs.Add(tabIndex);
            }

            return clicked;
        }

        public static void DrawMissionTable(string tableName, List<Mission> missions)
        {
            var availableSpace = ImGui.GetContentRegionAvail().X;
            var textSize = ImGui.CalcTextSize(tableName);
            var headerPadding = new Vector2(10, 5);
            var headerHeight = textSize.Y + headerPadding.Y * 2;

            using (ImRaii.Child($"Table Header: {tableName}", new Vector2(availableSpace, headerHeight), true, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                var centeredPosX = (availableSpace - textSize.X) / 2;

                ImGui.SetCursorPosY(headerPadding.Y);
                ImGui.SetCursorPosX(centeredPosX);
                ImGui.Text($"{tableName} Missions");
            }

            uint selectedJob = C.SelectedJob;
            int totalColumns = 17; // Enabled, Manual, ID, Completion Status, Mission Name, Cosmo, Lunar, I, II, III, IV, Turnin, Gather, Notes

            ImGuiTableFlags tableFlags = ImGuiTableFlags.RowBg |
                                        ImGuiTableFlags.Borders |
                                        ImGuiTableFlags.Reorderable |         // Allow column reordering
                                        ImGuiTableFlags.Hideable |             // Allow hiding columns via right-click
                                        ImGuiTableFlags.SizingFixedFit;

            bool hasGathering = false;
            bool hasToken = false;
            foreach (var mission in missions)
            {
                var id = mission.id;
                var missionInfo = CosmicHelper.SheetMissionDict[id];


                if (missionInfo.Jobs.Overlaps(CosmicHelper.GatheringJobList))
                    hasGathering = true;
                if (missionInfo.RewardItemAmount > 0)
                    hasToken = true;
            }

            if (ImGui.BeginTable($"MissionList###{tableName}_{selectedJob}", totalColumns, tableFlags))
            {
                float padding = 10f;

                #region Table Column Setup

                // Setup ALL columns
                ImGui.TableSetupColumn("Enabled");
                ImGui.TableSetupColumn("Manual");
                ImGui.TableSetupColumn("ID");
                ImGui.TableSetupColumn("✓");
                ImGui.TableSetupColumn("Mission Name");
                ImGui.TableSetupColumn("Cosmo");
                ImGui.TableSetupColumn("Lunar");
                ImGui.TableSetupColumn("Score");

                // XP columns
                float xpWidth = ImGui.CalcTextSize("III").X + padding;
                ImGui.TableSetupColumn("I");
                ImGui.TableSetupColumn("II");
                ImGui.TableSetupColumn("III");
                ImGui.TableSetupColumn("IV");
                ImGui.TableSetupColumn("V");

                ImGui.TableSetupColumn("Turnin Mode");
                ImGui.TableSetupColumn("Gathering Profile");
                ImGui.TableSetupColumn("Mission Notes");
                ImGui.TableSetupColumn("Reward Item");

                if (!hasGathering)
                    ImGui.TableSetColumnEnabled(14, false);
                if (!hasToken)
                    ImGui.TableSetColumnEnabled(16, false);

                #endregion

                #region Custom Headers

                ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

                // Column 0: Enabled
                ImGui.TableSetColumnIndex(0);
                ImGui.TableHeader("Enabled");
                if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    ImGui.OpenPopup("Enabled Options");
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Enable/disable mission for automation");
                    ImGui.Text($"Left click for options");
                    ImGui.EndTooltip();
                }
                if (ImGui.BeginPopup("Enabled Options"))
                {
                    if (ImGui.Button("Enable All"))
                    {
                        foreach (var mission in missions)
                        {
                            C.MissionConfig[mission.id].Enabled = true;
                            if (GetOnlyPreviousMissionsRecursive(mission.id).Count > 0)
                            {
                                foreach (var prevMission in GetOnlyPreviousMissionsRecursive(mission.id))
                                {
                                    var prevMissionConfig = C.MissionConfig[prevMission];
                                    prevMissionConfig.Enabled = true;
                                }
                            }
                        }
                        C.Save();
                    }

                    if (ImGui.Button("Disable All"))
                    {
                        foreach (var mission in missions)
                        {
                            C.MissionConfig[mission.id].Enabled = false;
                        }
                        C.Save();
                    }

                    ImGui.EndPopup();
                }

                // Column 1: Manual
                ImGui.TableSetColumnIndex(1);
                ImGui.AlignTextToFramePadding();
                ImGui.TableHeader("Manual");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Manual mode - requires manual intervention");
                    ImGui.EndTooltip();
                }

                // Column 2: ID
                ImGui.TableSetColumnIndex(2);
                ImGui.TableHeader("ID");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Mission ID number");
                    ImGui.EndTooltip();
                }

                // Column 3: Completed (with Unicode checkmark)
                ImGui.TableSetColumnIndex(3);
                ImGui.TableHeader("✓");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Mission completion status");
                    ImGui.EndTooltip();
                }

                // Column 4: Mission Name
                ImGui.TableSetColumnIndex(4);
                ImGui.TableHeader("Mission Name");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Click mission name to view details");
                    ImGui.EndTooltip();
                }

                // Continue this pattern for all your columns...
                // Column 5: Cosmo
                ImGui.TableSetColumnIndex(5);
                ImGui.TableHeader("Cosmo");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Cosmic Credits reward");
                    ImGui.EndTooltip();
                }

                // Column 6: Lunar
                ImGui.TableSetColumnIndex(6);
                ImGui.TableHeader("Lunar");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Lunar Credits reward");
                    ImGui.EndTooltip();
                }

                // Column 7: Score
                ImGui.TableSetColumnIndex(7);
                ImGui.TableHeader("Score");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Class Score reward");
                    ImGui.EndTooltip();
                }

                // XP Columns (8-12)
                string[] xpLabels = { "I", "II", "III", "IV", "V" };
                for (int i = 0; i < 5; i++)
                {
                    ImGui.TableSetColumnIndex(8 + i);
                    ImGui.TableHeader(xpLabels[i]);
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text($"Relic XP Type {xpLabels[i]} reward");
                        ImGui.EndTooltip();
                    }
                }

                // Column 13: Turnin Mode
                ImGui.TableSetColumnIndex(13);
                ImGui.TableHeader("Turnin Mode");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Configure mission turnin settings");
                    ImGui.EndTooltip();
                }

                // Column 14: Gathering Profile
                ImGui.TableSetColumnIndex(14);
                ImGui.TableHeader("Gathering Profile");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Select gathering profile for gather missions");
                    ImGui.EndTooltip();
                }

                // Column 15: Mission Notes
                ImGui.TableSetColumnIndex(15);
                ImGui.TableHeader("Mission Notes");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Additional mission information and requirements");
                    ImGui.EndTooltip();
                }

                // Column 16: Reward Item
                ImGui.TableSetColumnIndex(16);
                ImGui.TableHeader("Token");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Tokens that can be earned from this mission");
                    ImGui.EndTooltip();
                }

                #endregion

                foreach (var entry in missions)
                {
                    var Id = entry.id;
                    var missionConfig = C.MissionConfig[Id];
                    var missionInfo = CosmicHelper.SheetMissionDict[Id];

                    bool craftMission = missionInfo.Attributes.HasFlag(MissionAttributes.Craft);
                    bool gatherMission = missionInfo.Attributes.HasFlag(MissionAttributes.Gather);
                    bool fishMission = missionInfo.Attributes.HasFlag(MissionAttributes.Fish);
                    bool critical = missionInfo.Attributes.HasFlag(MissionAttributes.Critical);

                    bool dualclass = craftMission && (gatherMission || fishMission);
                    bool unsupported = UnsupportedMissions.Ids.Contains(Id);
                    bool hideUnsupported = C.HideUnsupportedMissions;

                    if (unsupported && hideUnsupported)
                        continue;

                    ImGui.TableNextRow();

                    // Mission Enable/Disable Checkbox
                    ImGui.PushID(Id);

                    // Enable | Disable Mission Selection
                    ImGui.TableSetColumnIndex(0);
                    bool enabled = missionConfig.Enabled;
                    if (Table_CenterCheckbox("##EnableMission", ref enabled))
                    {
                        missionConfig.Enabled = enabled;
                        if (missionConfig.Enabled == true)
                        {
                            if (GetOnlyPreviousMissionsRecursive(Id).Count >0)
                            {
                                foreach (var prevMission in GetOnlyPreviousMissionsRecursive(Id))
                                {
                                    var prevMissionConfig = C.MissionConfig[prevMission];
                                    prevMissionConfig.Enabled = true;
                                }
                            }
                        }

                        C.Save();
                    }
                    if (ImGui.IsItemClicked())
                    {
                        selectedMission = Id;
                    }


                    // Manual mode checkbox
                    ImGui.TableNextColumn();
                    bool manualMode = missionConfig.ManualMode;
                    if (Table_CenterCheckbox("##Manual Mode", ref manualMode))
                    {
                        missionConfig.ManualMode = manualMode;
                        C.Save();
                    }
                    if (ImGui.IsItemClicked())
                    {
                        selectedMission = Id;
                    }

                    // Mission ID
                    ImGui.TableNextColumn();
                    Table_FullCenterText(Id.ToString());

                    // Completion Status
                    ImGui.TableNextColumn();
                    CompletionStatus_Formatted(Id);

                    // Mission Name
                    ImGui.TableNextColumn();
                    if (unsupported)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.0f, 0.0f, 1.0f)); // Red color (RGBA)
                        ImGuiEx.IconWithTooltip(FontAwesomeIcon.ExclamationTriangle, "This is currently not supported yet. I'm working on bringing it over.\n" +
                                                "It's just taking me time");
                        ImGui.PopStyleColor();
                        ImGui.SameLine();
                    }

                    ImGui.Text(missionInfo.Name);
                    if (ImGui.IsItemClicked())
                    {
                        selectedMission = Id;
                    }
                    if (missionInfo.MarkerId != 0)
                    {
                        ImGui.SameLine();
                        ImGui.PushFont(UiBuilder.IconFont);
                        ImGui.Text(FontAwesomeIcon.Flag.ToIconString());
                        ImGui.PopFont();
                        if (ImGui.IsItemClicked())
                        {
                            selectedMission = Id;
                            Utils.SetGatheringRing(missionInfo.TerritoryId, (int)missionInfo.MapPosition.X, (int)missionInfo.MapPosition.Y, missionInfo.Radius, missionInfo.Name);
                        }
                    }

                    // Cosmo/Lunar Credits
                    ImGui.TableNextColumn();
                    Table_FullCenterText(missionInfo.CosmoCredit.ToString());

                    ImGui.TableNextColumn();
                    Table_FullCenterText(missionInfo.LunarCredit.ToString());

                    ImGui.TableNextColumn();
                    Table_FullCenterText(missionInfo.ClassScore.ToString());

                    // XP Columns
                    for (int i = 1; i < 6; i++)
                    {
                        ImGui.TableNextColumn();
                        var expReward = missionInfo.RelicXpInfo.Where(exp => exp.Key == i).FirstOrDefault();
                        var relicXp = expReward.Value.ToString();

                        if (relicXp == "0")
                        {
                            relicXp = "-";
                        }

                        Table_FullCenterText(relicXp);
                    }

                    // Mission Turnin Settings
                    ImGui.TableNextColumn();
                    if (missionInfo.Attributes.HasFlag(MissionAttributes.ScoreTimeRemaining))
                    {
                        Table_VertCenterText("Auto");
                        if (missionConfig.AutoTurnin == false)
                        {
                            missionConfig.AutoTurnin = true;
                            missionConfig.TurninGold = false;
                            missionConfig.TurninSilver = false;
                            missionConfig.TurninBronze = false;

                            C.Save();
                        }
                    }
                    else
                    {
                        if (Table_ButtonCentered("Select Turnin"))
                        {
                            ImGui.OpenPopup("Mission Turnin Settings");
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            if (missionConfig.AutoTurnin)
                                ImGui.Text($"Auto Turnin - True");
                            else
                            {
                                if (missionConfig.TurninGold)
                                    ImGui.Text($"Gold Enabled");
                                if (missionConfig.TurninSilver)
                                    ImGui.Text($"Silver Enabled");
                                if (missionConfig.TurninBronze)
                                    ImGui.Text($"Bronze Enabled");
                            }

                            ImGui.EndTooltip();
                        }

                        if (ImGui.BeginPopup("Mission Turnin Settings"))
                        {
                            bool anyTurnin = missionConfig.AutoTurnin;
                            bool goldTurnin = missionConfig.TurninGold;
                            bool silverTurnin = missionConfig.TurninSilver;
                            bool bronzeTurnin = missionConfig.TurninBronze;

                            ImGui.Text("Select Turnin Options");
                            ImGui.Dummy(new Vector2(0, 2));

                            if (ImGui.Checkbox("Auto", ref anyTurnin))
                            {
                                if (anyTurnin)
                                {
                                    missionConfig.TurninGold = false;
                                    missionConfig.TurninSilver = false;
                                    missionConfig.TurninBronze = false;

                                    missionConfig.AutoTurnin = anyTurnin;
                                }
                                else
                                {
                                    if (!(bronzeTurnin && silverTurnin && goldTurnin))
                                    {
                                        missionConfig.AutoTurnin = true;
                                    }
                                }

                                C.Save();
                            }
                            ImGuiEx.HelpMarker("This option will strive to get the best result, but will turn in any result if necessary without stopping.");

                            ImGui.Separator();

                            if (ImGui.Checkbox("Gold", ref goldTurnin))
                            {
                                if (anyTurnin && goldTurnin)
                                    missionConfig.AutoTurnin = false;

                                missionConfig.TurninGold = goldTurnin;
                                C.Save();
                            }
                            if (ImGui.Checkbox("Silver", ref silverTurnin))
                            {
                                if (anyTurnin && silverTurnin)
                                    missionConfig.AutoTurnin = false;

                                missionConfig.TurninSilver = silverTurnin;
                                C.Save();
                            }
                            if (ImGui.Checkbox("Bronze", ref bronzeTurnin))
                            {
                                if (anyTurnin && bronzeTurnin)
                                    missionConfig.AutoTurnin = false;

                                missionConfig.TurninBronze = bronzeTurnin;
                                C.Save();
                            }

                            ImGui.EndPopup();
                        }
                    }
                    bool gatherProfile = missionInfo.Attributes.HasFlag(MissionAttributes.Gather);
                    bool collectable = missionInfo.Attributes.HasFlag(MissionAttributes.Collectables) || missionInfo.Attributes.HasFlag(MissionAttributes.ReducedItems);

                    // Gather Mission Profile Settings
                    ImGui.TableNextColumn();
                    if (gatherProfile && !collectable)
                    {
                        string profileName = "???";
                        var profileSettings = C.GatherSettings.Where(x => x.Id == missionConfig.GatherProfileId).FirstOrDefault();

                        if (profileSettings != null)
                        {
                            profileName = profileSettings.Name;
                        }
                        else
                        {
                            profileName = "???";
                        }

                        if (Table_ButtonCentered($"{profileName}##GatherProfile_{profileName}"))
                        {
                            ImGui.OpenPopup("Selecting Gathering Profile");
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text("Select profile to use");
                            ImGui.EndTooltip();
                        }
                        if (ImGui.BeginPopup("Selecting Gathering Profile"))
                        {
                            ImGui.Text($"Currently Selected: {profileName}");
                            ImGui.Separator();

                            foreach (var profile in C.GatherSettings)
                            {
                                if (profile != null)
                                {
                                    var profileId = profile.Id;
                                    bool profileSelected = missionConfig.GatherProfileId == profileId;
                                    if (ImGui.RadioButton(profile.Name, profileSelected))
                                    {
                                        missionConfig.GatherProfileId = profileId;
                                        C.Save();
                                    }
                                }
                            }

                            ImGui.EndPopup();
                        }
                    }
                    else if (missionInfo.Attributes.HasFlag(MissionAttributes.Fish))
                    {
                        if (Table_ButtonCentered($"Select Profile##Select_Fishing_Profile"))
                        {
                            ImGui.OpenPopup("Select Fishing Profile");
                        }
                        if (ImGui.BeginPopup("Select Fishing Profile"))
                        {
                            ImGui.Text($"Fishing profile: {missionInfo.Name}");
                            ImGui.Separator();
                            bool builtInPreset = missionConfig.Use_BuildinPreset;
                            if (ImGui.Checkbox("Use Built In Preset", ref builtInPreset))
                            {
                                missionConfig.Use_BuildinPreset = builtInPreset;
                                C.Save();
                            }
                            ImGuiEx.HelpMarker("Having this enabled means it will use the default preset that is included with the plugin for autohook. \n" +
                                               "If you would like to use one that you already have in autohook, you can un-checkmark this and type the name of it below");
                            using (ImRaii.Disabled(builtInPreset))
                            {
                                string presetName = missionConfig.AutoHookPresetName;
                                ImGui.SetNextItemWidth(200);
                                if (ImGui.InputText("Preset Name", ref presetName))
                                {
                                    missionConfig.AutoHookPresetName = presetName;
                                    C.Save();
                                }
                            }

                            ImGui.EndPopup();
                        }
                    }

                    ImGui.TableNextColumn();
                    int notesCount = 0;

                    if (missionInfo.Attributes.HasFlag(MissionAttributes.ProvisionalTimed))
                    {
                        Table_FontCenter(FontAwesomeIcon.Clock);
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text($"{missionInfo.StartTime}:00 - {missionInfo.EndTime-1}:59");
                            ImGui.EndTooltip();
                        }
                        notesCount++;
                    }
                    if (missionInfo.Attributes.HasFlag(MissionAttributes.ProvisionalSequential))
                    {
                        Table_FontCenter(FontAwesomeIcon.ListOl);
                        if (ImGui.IsItemHovered())
                        {
                            var prevMissions = GetOnlyPreviousMissionsRecursive(Id);

                            ImGui.BeginTooltip();
                            ImGui.Text("Sequence Missions");
                            ImGui.Separator();
                            for (int i = 0; i < prevMissions.Count; i++)
                            {
                                var prevMission = prevMissions[i];
                                ImGui.Text($"{i + 1}: [{prevMission}] - {CosmicHelper.SheetMissionDict[prevMission].Name}");
                            }
                            ImGui.EndTooltip();
                        }
                        notesCount++;
                    }
                    if (missionInfo.Attributes.HasFlag(MissionAttributes.ProvisionalWeather))
                    {
                        if (notesCount > 0)
                            ImGui.SameLine(0, 2);

                        if (CosmicHelper.WeatherIds.ContainsKey(missionInfo.Weather))
                        {
                            ISharedImmediateTexture? weatherIcon = CosmicHelper.WeatherIconDict[missionInfo.Weather];
                            Vector2 ImageSize = new Vector2(23, 23);
                            ImGui.Image(weatherIcon.GetWrapOrEmpty().Handle, ImageSize);
                        }
                        else
                        {
                            Table_FontCenter(FontAwesomeIcon.Cloud);
                        }

                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text($"Weather: {missionInfo.Weather}");
                            ImGui.EndTooltip();
                        }
                        notesCount++;
                    }
                    if (CosmicHelper.MissionUnlock.TryGetValue(Id, out var unlock))
                    {
                        if (notesCount > 0)
                            ImGui.SameLine(0, 2);

                        if (Svc.Texture.GetFromGame("ui/uld/WKSMission_hr1.tex") is { } tex)
                        {
                            if (tex.TryGetWrap(out var wrap, out var exc))
                            {
                                ImGui.Image(wrap.Handle, new Vector2(23, 23), new Vector2(0.2347f, 0.3500f), new Vector2(0.2959f, 0.6500f));
                            }
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text("The following missions are required to have gold before you can do this one");
                            foreach (var mission in unlock)
                            {
                                CompletionStatus_Normal(mission);
                                ImGui.SameLine();
                                ImGui.Text($"[{mission}] - {CosmicHelper.SheetMissionDict[mission].Name}");
                            }
                            ImGui.EndTooltip();
                        }
                        notesCount++;

                    }
                    if (missionInfo.Jobs.Count > 1)
                    {
                        if (notesCount > 0)
                            ImGui.SameLine(0, 2);

                        ISharedImmediateTexture? job1Icon = CosmicHelper.JobIconDict[missionInfo.Jobs.First()];
                        ISharedImmediateTexture? job2Icon = CosmicHelper.JobIconDict[missionInfo.Jobs.Last()];
                        Vector2 imageSize = new Vector2(23, 23);

                        ImGui.Image(job1Icon.GetWrapOrEmpty().Handle, imageSize);
                        ImGui.SameLine(0, 2);
                        ImGui.Image(job2Icon.GetWrapOrEmpty().Handle, imageSize);
                        notesCount++;
                    }
                    if (CosmicHelper.CustomMissionNotes.TryGetValue(Id, out var notes))
                    {
                        if (notesCount > 0)
                            ImGui.SameLine();
                        ImGuiEx.Icon(FontAwesomeIcon.Trophy);
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text(notes.NoteInfo);
                            ImGui.Text($"Average Score Per Minute: {notes.SPM:N2}");

                            ImGui.EndTooltip();
                        }
                    }

                    ImGui.TableNextColumn();
                    string itemAmount = missionInfo.RewardItemAmount > 0 ? $"{missionInfo.RewardItemAmount}" : "-";
                    Table_VertCenterText(itemAmount);

                    ImGui.PopID();
                }

                ImGui.EndTable();
            }
        }

        #region Table Tools

        // Center a checkbox both horizontally and vertically in the current table cell
        private static bool Table_CenterCheckbox(string id, ref bool value)
        {
            var cursorPos = ImGui.GetCursorPos();
            var availWidth = ImGui.GetContentRegionAvail().X;

            var checkboxSize = ImGui.GetFrameHeight();
            ImGui.SetCursorPosX(cursorPos.X + (availWidth - checkboxSize) * 0.5f);
            ImGui.AlignTextToFramePadding();

            return ImGui.Checkbox($"##{id}", ref value);
        }
        private static void Table_DrawCenteredImage(ISharedImmediateTexture textureId, Vector2 imageSize)
        {
            var cursorPos = ImGui.GetCursorPos();
            var availWidth = ImGui.GetContentRegionAvail().X;

            ImGui.SetCursorPosX(cursorPos.X + (availWidth - imageSize.X) * 0.5f);
            ImGui.AlignTextToFramePadding();

            ImGui.Image(textureId.GetWrapOrEmpty().Handle, imageSize);
        }
        private static void Table_VertCenterText(string text)
        {
            ImGui.AlignTextToFramePadding();
            ImGui.TextUnformatted(text);
        }
        private static void Table_FullCenterText(string text)
        {
            var cursorPosX = ImGui.GetCursorPosX();
            var availWidth = ImGui.GetContentRegionAvail().X;
            var textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX(cursorPosX + (availWidth - textWidth) * 0.5f);
            ImGui.AlignTextToFramePadding();

            ImGui.TextUnformatted(text);
        }
        private static void Table_FullCenterText(string icon, Vector4 color)
        {
            var cursorPosX = ImGui.GetCursorPosX();
            var availWidth = ImGui.GetContentRegionAvail().X;
            var textWidth = ImGui.CalcTextSize(icon).X;

            ImGui.SetCursorPosX(cursorPosX + (availWidth - textWidth) * 0.5f);
            ImGui.AlignTextToFramePadding();

            FontAwesome.Print(color, icon);
        }
        private static bool Table_ButtonCentered(string label, Vector2? buttonSize = null)
        {
            var cursorPosX = ImGui.GetCursorPosX();
            var availWidth = ImGui.GetContentRegionAvail().X;

            Vector2 actualButtonSize;
            if (buttonSize.HasValue)
            {
                actualButtonSize = buttonSize.Value;
            }
            else
            {
                var textSize = ImGui.CalcTextSize(label);
                var framePadding = ImGui.GetStyle().FramePadding;
                actualButtonSize = new Vector2(textSize.X + framePadding.X * 2, textSize.Y + framePadding.Y * 2);
            }

            ImGui.SetCursorPosX(cursorPosX + (availWidth - actualButtonSize.X) * 0.5f);
            return ImGui.Button(label, actualButtonSize);
        }
        private static void Table_CenterHeaderText(string text)
        {
            var cursorPosX = ImGui.GetCursorPosX();
            var availWidth = ImGui.GetContentRegionAvail().X;
            var textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX(cursorPosX + (availWidth - textWidth) * 0.5f);
            ImGui.AlignTextToFramePadding();

            ImGui.TableHeader(text);
        }
        private static void Table_FontCenter(FontAwesomeIcon icon)
        {
            ImGui.AlignTextToFramePadding();
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.Text(icon.ToIconString());
            ImGui.PopFont();
        }

        public static List<uint> GetOnlyPreviousMissionsRecursive(uint missionId)
        {
            if (!CosmicHelper.SheetMissionDict.TryGetValue(missionId, out var missionInfo) || missionInfo.PreviousMissions.Contains(0))
                return [];

            var chain = GetOnlyPreviousMissionsRecursive(missionInfo.PreviousMissions.First());
            chain.Add(missionInfo.PreviousMissions.First());
            return chain;
        }
        private static List<uint> GetOnlyNextMissionsRecursive(uint missionId)
        {
            uint? nextMissionId = CosmicHelper.SheetMissionDict
                .Where(m => m.Value.PreviousMissions.First() == missionId)
                .Select(m => (uint?)m.Key)
                .FirstOrDefault();

            if (!nextMissionId.HasValue)
                return [];

            var chain = new List<uint> { nextMissionId.Value };
            chain.AddRange(GetOnlyNextMissionsRecursive(nextMissionId.Value));
            return chain;
        }
        private static unsafe void CompletionStatus_Formatted(uint id)
        {
            var managerPtr = WKSManager.Instance();
            if (managerPtr == null) return;

            var manager = (WKSManagerCustom*)managerPtr;
            var isCompleted = manager->IsMissionCompleted(id);
            var isGold = manager->IsMissionGolded(id);

            float availableWidth = ImGui.GetContentRegionAvail().X;

            if (isCompleted)
            {
                if (isGold)
                {
                    // Center the image
                    float imageWidth = 23f;
                    float offsetX = (availableWidth - imageWidth) * 0.5f;

                    if (offsetX > 0)
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + offsetX);

                    if (Svc.Texture.GetFromGame("ui/uld/WKSMission_hr1.tex") is { } tex)
                    {
                        if (tex.TryGetWrap(out var wrap, out var exc))
                        {
                            var cursorPos = ImGui.GetCursorPos();
                            var availWidth = ImGui.GetContentRegionAvail().X;
                            var availHeight = ImGui.GetFrameHeight();

                            // Center horizontally
                            ImGui.SetCursorPosX(cursorPos.X + (availWidth - 23) * 0.5f);

                            // Center vertically
                            ImGui.SetCursorPosY(cursorPos.Y + (availHeight - 23) * 0.5f);
                            ImGui.Image(wrap.Handle, new Vector2(23, 23), new Vector2(0.2347f, 0.3500f), new Vector2(0.2959f, 0.6500f));
                        }
                    }
                }
                else
                {
                    ImGui.AlignTextToFramePadding();
                    Table_FullCenterText(FontAwesome.Check, EColor.Green);
                }
            }
            else
            {
                ImGui.AlignTextToFramePadding();
                Table_FullCenterText(FontAwesome.Cross, EColor.Red);
            }
        }
        private static unsafe void CompletionStatus_Normal(uint id)
        {
            var managerPtr = WKSManager.Instance();
            if (managerPtr == null) return;

            var manager = (WKSManagerCustom*)managerPtr;
            var isCompleted = manager->IsMissionCompleted(id);
            var isGold = manager->IsMissionGolded(id);

            var containerSize = new Vector2(23, 23);

            // Create a consistent container for all elements
            var cursorPos = ImGui.GetCursorPos();
            ImGui.InvisibleButton("##status_container", containerSize);
            ImGui.SetCursorPos(cursorPos);

            if (isCompleted)
            {
                if (isGold)
                {
                    if (Svc.Texture.GetFromGame("ui/uld/WKSMission_hr1.tex") is { } tex)
                    {
                        if (tex.TryGetWrap(out var wrap, out var exc))
                        {
                            ImGui.Image(wrap.Handle, containerSize, new Vector2(0.2347f, 0.3500f), new Vector2(0.2959f, 0.6500f));
                        }
                    }
                }
                else
                {
                    // Center the FontAwesome icon within the container
                    var textSize = ImGui.CalcTextSize(FontAwesome.Check);
                    var offset = (containerSize - textSize) * 0.5f;
                    offset += new Vector2(-2f, 1f);
                    ImGui.SetCursorPos(cursorPos + offset);
                    FontAwesome.Print(EColor.Green, FontAwesome.Check);
                }
            }
            else
            {
                var textSize = ImGui.CalcTextSize(FontAwesome.Cross);
                var offset = (containerSize - textSize) * 0.5f;
                offset += new Vector2(-2f, 1f);
                ImGui.SetCursorPos(cursorPos + offset);
                FontAwesome.Print(EColor.Red, FontAwesome.Cross);
            }

            // Reset cursor to after the container
            ImGui.SetCursorPos(cursorPos + new Vector2(containerSize.X, 0));
        }
        private static void UpdateSelectedMission(uint missionId)
        {
            if (ImGui.IsItemClicked())
            {
                selectedMission = missionId;
            }
        }
        private static void DrawColoredStar(TurninState state)
        {
            Vector4 color = state switch
            {
                TurninState.Bronze => new Vector4(0.8f, 0.5f, 0.3f, 1.0f),  // Bronze
                TurninState.Silver => new Vector4(0.75f, 0.75f, 0.75f, 1.0f), // Silver
                TurninState.Gold => new Vector4(1.0f, 0.84f, 0.0f, 1.0f),    // Gold
                TurninState.Critical => new Vector4(1.0f, 0.84f, 0.0f, 1.0f), // Gold
                _ => new Vector4(0, 0, 0, 0) // Transparent/none
            };

            if (state != TurninState.None)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, color);
                ImGui.PushFont(UiBuilder.IconFont); // Make sure you're using the icon font
                ImGui.Text(FontAwesomeIcon.Star.ToIconString());
                ImGui.PopFont();
                ImGui.PopStyleColor();
            }
        }

        #endregion
    }
}

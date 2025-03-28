using UnityEditor;
using UnityEngine;
using GreatGames.CaseLib.Utility;

namespace GreatGames.CaseLib.EditorTools
{
    public static class LevelEditorUI
    {
        public static void DrawColorSelection(LevelConfigSO levelConfig, ref LevelEditorState state)
        {
            state.GameMode = (GameMode)EditorGUILayout.EnumPopup("Game Mode", state.GameMode);

            if (state.GameMode == GameMode.SlinkyGame)
            {
                EditorGUILayout.LabelField("Select Slinky Color", EditorStyles.boldLabel);
                state.SelectedColor = (ItemColor)EditorGUILayout.EnumPopup("Slinky Color", state.SelectedColor);
            }
            else if (state.GameMode == GameMode.PassengerGame)
            {
                state.SelectedSpawnType = (SpawnType)EditorGUILayout.EnumPopup("Spawn Type", state.SelectedSpawnType);

                if (state.SelectedSpawnType == SpawnType.Bus ||
                    state.SelectedSpawnType == SpawnType.Door ||
                    state.SelectedSpawnType == SpawnType.Block)
                {
                    state.SelectedColor = (ItemColor)EditorGUILayout.EnumPopup("Color", state.SelectedColor);

                    if (state.SelectedSpawnType == SpawnType.Door)
                    {
                        state.SelectedExitDirection = (DoorExitDirection)EditorGUILayout.EnumPopup("Enter Direction", state.SelectedExitDirection);
                    }
                }
            }

            GUILayout.Space(10);
        }

        public static void DrawListView(LevelConfigSO levelConfig, LevelEditorState state)
        {
            if (state.GameMode == GameMode.SlinkyGame)
            {
                EditorGUILayout.LabelField("Slinky List", EditorStyles.boldLabel);
                foreach (var slinky in levelConfig.Slinkies)
                {
                    string startSlotStr = FormatSlot(slinky.StartSlot, levelConfig.UpperGridSize);
                    string endSlotStr = FormatSlot(slinky.EndSlot, levelConfig.UpperGridSize);
                    EditorGUILayout.LabelField($"Start: {startSlotStr}  End: {endSlotStr}  Color: {slinky.Color}");
                }
            }
            else
            {
                switch (state.SelectedSpawnType)
                {
                    case SpawnType.Bus:
                        EditorGUILayout.LabelField("Bus List", EditorStyles.boldLabel);
                        foreach (var bus in levelConfig.Buses)
                        {
                            for (int i = 0; i < bus.Slots.Count; i++)
                            {
                                string slotStr = FormatSlot(bus.Slots[i], levelConfig.UpperGridSize);
                                string colorStr = bus.Colors.Count > i ? bus.Colors[i].ToString() : "Unknown";
                                EditorGUILayout.LabelField($"Slot: {slotStr}  Color: {colorStr}");
                            }
                            EditorGUILayout.Space();
                        }
                        break;
                    case SpawnType.Door:
                        EditorGUILayout.LabelField("Door List", EditorStyles.boldLabel);
                        foreach (var door in levelConfig.Doors)
                        {
                            string slotStr = FormatSlot(door.SlotIndex, levelConfig.UpperGridSize);
                            string colorList = string.Join(", ", door.IncomingColors);
                            string countList = string.Join(", ", door.IncomingCounts);
                            string doorDir = door.EnterDirection.ToString();
                            EditorGUILayout.LabelField($"Door At: {slotStr} Colors: {colorList}  Counts: {countList}  Dir: {doorDir}");
                        }
                        break;
                    case SpawnType.Block:
                        EditorGUILayout.LabelField("Block List", EditorStyles.boldLabel);
                        foreach (var block in levelConfig.Blocks)
                        {
                            string slotStr = FormatSlot(block.SlotIndex, levelConfig.UpperGridSize);
                            EditorGUILayout.LabelField($"Blocked Slot: {slotStr}");
                        }
                        break;
                }
            }
        }
        private static string FormatSlot(int slotIndex, Vector2Int gridSize)
        {
            int x = slotIndex % gridSize.x;
            int y = slotIndex / gridSize.x;
            return $"{x},{y}";
        }
    }
}

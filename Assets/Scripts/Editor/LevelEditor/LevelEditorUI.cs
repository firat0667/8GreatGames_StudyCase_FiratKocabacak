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
                DrawSlinkyColor(ref state);
            }
            else if (state.GameMode == GameMode.PassengerGame)
            {
                DrawPassengerOptions(levelConfig, ref state);
            }

            GUILayout.Space(10);
        }

        private static void DrawSlinkyColor(ref LevelEditorState state)
        {
            EditorGUILayout.LabelField("Select Slinky Color", EditorStyles.boldLabel);
            state.SelectedColor = (ItemColor)EditorGUILayout.EnumPopup("Slinky Color", state.SelectedColor);
        }

        private static void DrawPassengerOptions(LevelConfigSO levelConfig, ref LevelEditorState state)
        {
            state.SelectedSpawnType = (SpawnType)EditorGUILayout.EnumPopup("Spawn Type", state.SelectedSpawnType);
            state.SelectedColor = (ItemColor)EditorGUILayout.EnumPopup("Color", state.SelectedColor);

            if (state.SelectedSpawnType == SpawnType.Door || state.SelectedSpawnType == SpawnType.Bus)
            {
                state.SelectedExitDirection = (Direction)EditorGUILayout.EnumPopup("Enter Direction", state.SelectedExitDirection);
            }

            if (state.SelectedSpawnType == SpawnType.Door)
            {
                DrawDoorFormations(levelConfig);
            }
        }

        private static void DrawDoorFormations(LevelConfigSO levelConfig)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Passenger Formation Settings (per door)", EditorStyles.boldLabel);

            SerializedObject so = new SerializedObject(levelConfig);
            SerializedProperty doorsProp = so.FindProperty("Doors");

            for (int i = 0; i < doorsProp.arraySize; i++)
            {
                var doorProp = doorsProp.GetArrayElementAtIndex(i);
                SerializedProperty formationProp = doorProp.FindPropertyRelative("Formation");
                if (formationProp != null)
                {
                    string label = $"Door {i + 1} Formation";
                    EditorGUILayout.PropertyField(formationProp, new GUIContent(label));
                }

            }

            so.ApplyModifiedProperties();
        }

        public static void DrawListView(LevelConfigSO levelConfig, LevelEditorState state)
        {
            switch (state.GameMode)
            {
                case GameMode.SlinkyGame:
                    DrawSlinkyList(levelConfig);
                    break;
                case GameMode.PassengerGame:
                    DrawPassengerLists(levelConfig, state);
                    break;
            }
        }

        private static void DrawSlinkyList(LevelConfigSO levelConfig)
        {
            EditorGUILayout.LabelField("Slinky List", EditorStyles.boldLabel);
            foreach (var slinky in levelConfig.Slinkies)
            {
                string startSlot = FormatSlot(slinky.StartSlot, levelConfig.UpperGridSize);
                string endSlot = FormatSlot(slinky.EndSlot, levelConfig.UpperGridSize);
                EditorGUILayout.LabelField($"Start: {startSlot}  End: {endSlot}  Color: {slinky.Color}");
            }
        }

        private static void DrawPassengerLists(LevelConfigSO levelConfig, LevelEditorState state)
        {
            switch (state.SelectedSpawnType)
            {
                case SpawnType.Bus:
                    DrawBusList(levelConfig);
                    break;
                case SpawnType.Door:
                    DrawDoorList(levelConfig);
                    break;
                case SpawnType.Block:
                    DrawBlockList(levelConfig);
                    break;
            }
        }

        private static void DrawBusList(LevelConfigSO levelConfig)
        {
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
        }

        private static void DrawDoorList(LevelConfigSO levelConfig)
        {
            EditorGUILayout.LabelField("Door List", EditorStyles.boldLabel);
            foreach (var door in levelConfig.Doors)
            {
                string slotStr = FormatSlot(door.SlotIndex, levelConfig.UpperGridSize);
                string colorList = string.Join(", ", door.IncomingColors);
                string countList = string.Join(", ", door.IncomingCounts);
                string dir = door.EnterDirection.ToString();
                string pathName = door.PathLineObject ? door.PathLineObject.name : "None";

                EditorGUILayout.LabelField($"Slot: {slotStr} | Colors: {colorList} | Counts: {countList} | Dir: {dir} | Formation: {pathName}");
            }
        }

        private static void DrawBlockList(LevelConfigSO levelConfig)
        {
            EditorGUILayout.LabelField("Block List", EditorStyles.boldLabel);
            foreach (var block in levelConfig.Blocks)
            {
                string slotStr = FormatSlot(block.SlotIndex, levelConfig.UpperGridSize);
                EditorGUILayout.LabelField($"Blocked Slot: {slotStr}");
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

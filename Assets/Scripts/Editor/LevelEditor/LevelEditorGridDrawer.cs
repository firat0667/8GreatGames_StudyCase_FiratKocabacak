using System.Linq;
using UnityEngine;

namespace GreatGames.CaseLib.EditorTools
{
    public static class LevelEditorGridDrawer
    {
        public static void DrawGrid(LevelConfigSO config, Vector2Int gridSize, bool isUpperGrid, LevelEditorState state)
        {
            GUILayout.BeginVertical();

            for (int y = 0; y < gridSize.y; y++)
            {
                GUILayout.BeginHorizontal();
                for (int x = 0; x < gridSize.x; x++)
                {
                    int slotIndex = x + y * gridSize.x;
                    string coordLabel = $"{x},{y}";
                    string slotLabel = GetSlotLabel(config, slotIndex);
                    string displayLabel = string.IsNullOrEmpty(slotLabel) ? coordLabel : $"{slotLabel}\n{coordLabel}";
                    Color buttonColor = LevelEditorUtils.GetButtonColor(config, slotIndex);

                    GUI.backgroundColor = buttonColor;
                    if (isUpperGrid && GUILayout.Button(displayLabel, GUILayout.Width(40), GUILayout.Height(40)))
                    {
                        HandleGridClick(config, slotIndex, state);
                    }
                    GUI.backgroundColor = Color.white;
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.Space(10);
        }

        private static string GetSlotLabel(LevelConfigSO config, int slotIndex)
        {
            if (config.Slinkies.Exists(s => s.StartSlot == slotIndex || s.EndSlot == slotIndex)) return "SLNK";
            if (config.Buses.Any(b => b.Slots.Contains(slotIndex))) return "BUS";
            if (config.Doors.Exists(d => d.SlotIndex == slotIndex)) return "DOOR";
            if (config.Blocks.Exists(b => b.SlotIndex == slotIndex)) return "BLCK";
            return "";
        }

        private static void HandleGridClick(LevelConfigSO config, int slotIndex, LevelEditorState state)
        {
            if (state.GameMode == GameMode.SlinkyGame)
            {
                if (state.SelectedStartSlot == -1)
                {
                    state.SelectedStartSlot = slotIndex;
                }
                else if (state.SelectedEndSlot == -1)
                {
                    state.SelectedEndSlot = slotIndex;
                    LevelEditorHandlers.HandleSlinkyPlacement(config, state.SelectedStartSlot, state.SelectedEndSlot, state.SelectedColor);
                    state.SelectedStartSlot = -1;
                    state.SelectedEndSlot = -1;
                }
            }
            else
            {
                switch (state.SelectedSpawnType)
                {
                    case SpawnType.Bus:
                        if (!state.SelectedBusSlots.Contains(slotIndex))
                        {
                            state.SelectedBusSlots.Add(slotIndex);
                            state.SelectedBusColors.Add(state.SelectedColor);
                        }

                        if (state.SelectedBusSlots.Count == 3)
                        {
                            LevelEditorHandlers.HandleBusPlacement(config, state.SelectedBusSlots, state.SelectedBusColors);
                            state.SelectedBusSlots.Clear();
                            state.SelectedBusColors.Clear();
                        }
                        break;
                    case SpawnType.Door:
                        LevelEditorHandlers.HandleDoorPlacement(config, slotIndex, state.SelectedColor, state.SelectedExitDirection);
                        break;
                    case SpawnType.Block:
                        LevelEditorHandlers.HandleBlockPlacement(config, slotIndex);
                        break;
                }
            }
        }
    }
}

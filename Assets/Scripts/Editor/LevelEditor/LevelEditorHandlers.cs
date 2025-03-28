using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Slinky;
using GreatGames.CaseLib.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GreatGames.CaseLib.EditorTools
{
    public static class LevelEditorHandlers
    {
        public static void HandleBusPlacement(LevelConfigSO levelConfig, List<int> selectedSlots, List<ItemColor> selectedColors)
        {
            if (selectedSlots.Count != 3) return;

            Undo.RegisterCompleteObjectUndo(levelConfig, "Add Bus");
            levelConfig.Buses.Add(new BusData
            {
                Slots = new List<int>(selectedSlots),
                Colors = new List<ItemColor>(selectedColors)
            });

            Debug.Log($"Bus created on slots: {string.Join(", ", selectedSlots)}");

            EditorUtility.SetDirty(levelConfig);
        }

        public static void HandleDoorPlacement(LevelConfigSO levelConfig, int slotIndex, ItemColor color, DoorExitDirection direction)
        {
            Undo.RegisterCompleteObjectUndo(levelConfig, "Add Door");

            var door = new DoorData
            {
                SlotIndex = slotIndex,
                IncomingColors = new List<ItemColor> { color },
                IncomingCounts = new List<int> { 4 },
                EnterDirection = direction
            };

            levelConfig.Doors.Add(door);
            EditorUtility.SetDirty(levelConfig);
        }

        public static void HandleBlockPlacement(LevelConfigSO levelConfig, int slotIndex)
        {
            if (levelConfig.Blocks.Any(b => b.SlotIndex == slotIndex)) return;

            Undo.RegisterCompleteObjectUndo(levelConfig, "Add Block");
            levelConfig.Blocks.Add(new BlockData { SlotIndex = slotIndex });
            EditorUtility.SetDirty(levelConfig);
        }

        public static void HandleSlinkyPlacement(LevelConfigSO levelConfig, int startSlot, int endSlot, ItemColor color)
        {
            Undo.RegisterCompleteObjectUndo(levelConfig, "Add Slinky");

            levelConfig.Slinkies.Add(new SlinkyData(startSlot, endSlot, color));

            Debug.Log($"Adding Slinky from {startSlot} to {endSlot}");
            EditorUtility.SetDirty(levelConfig);
        }
    }
}

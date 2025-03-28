using GreatGames.CaseLib.Utility;
using UnityEngine;

namespace GreatGames.CaseLib.EditorTools
{
    public static class LevelEditorUtils
    {
        public static Color GetButtonColor(LevelConfigSO config, int slotIndex)
        {
            var slinky = config.Slinkies.Find(s => s.StartSlot == slotIndex || s.EndSlot == slotIndex);
            if (slinky != null)
                return ItemColorUtility.GetColor(slinky.Color);

            var bus = config.Buses.Find(b => b.Slots.Contains(slotIndex));
            if (bus != null)
            {
                int colorIndex = bus.Slots.IndexOf(slotIndex);
                if (colorIndex >= 0 && colorIndex < bus.Colors.Count)
                    return ItemColorUtility.GetColor(bus.Colors[colorIndex]);
                return Color.gray;
            }

            var door = config.Doors.Find(d => d.SlotIndex == slotIndex);
            if (door != null) return Color.cyan;

            var block = config.Blocks.Find(b => b.SlotIndex == slotIndex);
            if (block != null) return Color.black;

            return Color.white;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Patterns;

namespace GreatGames.CaseLib.Managers
{
    public class DoorManager : FoundationSingleton<DoorManager>, IFoundationSingleton
    {
        private readonly List<DoorController> _doors = new();

        public bool Initialized { get; set; }

        public void RegisterDoor(DoorController door)
        {
            if (!_doors.Contains(door))
            {
                _doors.Add(door);
            }
        }

        public void UnregisterDoor(DoorController door)
        {
            if (_doors.Contains(door))
                _doors.Remove(door);
        }

        public DoorController GetDoorAt(GameKey key)
        {
            return _doors.FirstOrDefault(d => d.OccupiedGridKeys.Contains(key));
        }

        public List<DoorController> GetAllDoors()
        {
            return _doors;
        }
        public void ClearAll()
        {
            _doors.Clear();
        }
        public void DebugAllDoors()
        {
            Debug.Log($"📦 [DoorManager] total door: {_doors.Count}");
            foreach (var door in _doors)
            {
                Debug.Log($"{door.SlotIndex.ValueAsString} | passenger: {door.GetPassengers().Count}");
            }
        }
    }
}

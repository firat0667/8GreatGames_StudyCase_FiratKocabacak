using GreatGames.CaseLib.Definitions;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Managers;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Utility;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace GreatGames.CaseLib.Passenger
{
    public class PassengerGameBuilder : FoundationSingleton<PassengerGameBuilder>, IFoundationSingleton
    {
        [SerializeField] private GameObject _doorPrefab;
        [SerializeField] private GameObject _blockPrefab;
        [SerializeField] private GameObject _passengerPrefab;

        [SerializeField] private GameObject _busHeadPrefab;
        [SerializeField] private GameObject _busMidPrefab;
        [SerializeField] private GameObject _busTailPrefab;

        private GridManager _gridManager;

        [SerializeField] private GameObject _wallPrefab;

        [SerializeField] private float _distanceFromDoor = 1.5f;
        [SerializeField] private float _passengerYOffset = 0f;

        public float DistanceFromDoor => _distanceFromDoor;
        public float PassengerYOffset => _passengerYOffset;

        public bool Initialized { get; set; }

        [SerializeField] BusMoverSettingsSO _busMoverSettingsSO;
        private Transform _levelContainer;
        private void Start()
        {
            _gridManager = GridManager.Instance;
        }

        public void BuildLevel()
        {
            if (_levelContainer != null)
            {
                Destroy(_levelContainer.gameObject);
            }
            _levelContainer = new GameObject("LevelContainer").transform;
            _levelContainer.SetParent(transform);
            BuildBlocks();
            BuildDoors();
            BuildBuses();
            BuildPassengers();
            BuildWalls();
        }
        public void ClearLevel()
        {
            DoorManager.Instance.ClearAll();
            Destroy(_levelContainer.gameObject); 
            _levelContainer = null;
        }

        private void BuildBlocks()
        {
            foreach (var block in _gridManager.LevelData.Blocks)
            {
                GameKey key = _gridManager.UpperGrid.CreateKeyFromIndex(block.SlotIndex);
                Vector3 pos = _gridManager.GetSlotPosition(key, true);
                var go = Instantiate(_blockPrefab, pos, Quaternion.identity, _levelContainer);
                ComponentUtils.SetNameBySlotType(go, SlotType.Block, key);
                var dummy = new DummyBlockItem(go);
                _gridManager.UpperGrid.PlaceItem(key, dummy, force: true);
            }
        }

        private void BuildDoors()
        {
            foreach (var door in _gridManager.LevelData.Doors)
                DoorBuilder.Build(door, _gridManager, _doorPrefab, _levelContainer);
        }

        private void BuildPassengers()
        {
            foreach (var door in _gridManager.LevelData.Doors)
                PassengerSpawner.SpawnPassengers(
                    door,
                    _gridManager,
                    _passengerPrefab,
                    _levelContainer,
                    DistanceFromDoor,
                    PassengerYOffset
                );
        }

        private void BuildWalls()
        {
            WallBuilder.BuildForGrid(_gridManager.UpperGrid, _gridManager, _wallPrefab, _levelContainer);
        }
        private void BuildBuses()
        {
            var busList = new List<BusData>(_gridManager.LevelData.Buses); 

            foreach (var busData in busList)
            {
                if (busData.Slots == null || busData.Slots.Count != 3)
                {
                    continue;
                }
                  

                List<GameKey> slotKeys = new();
                foreach (int index in busData.Slots)
                    slotKeys.Add(_gridManager.UpperGrid.CreateKeyFromIndex(index));

                Vector3 headPos = _gridManager.GetSlotPosition(slotKeys[0], true);
                GameObject parentGO = new GameObject($"Bus_{slotKeys[0].ValueAsString}");
                parentGO.transform.SetParent(_levelContainer);
                parentGO.transform.position = headPos;

                GameObject busGO = new GameObject("BusRoot");
                busGO.transform.SetParent(parentGO.transform, false);

                var bus = busGO.AddComponent<BusController>();
                _gridManager.RegisterItem(bus);
                bus.BusMoveSettings = _busMoverSettingsSO;
                bus.Initialize(
                    keys: slotKeys,
                    colors: busData.Colors,
                    directions: busData.Directions,
                    gridManager: _gridManager,
                    headPrefab: _busHeadPrefab,
                    midPrefab: _busMidPrefab,
                    tailPrefab: _busTailPrefab
                );
                foreach (var key in slotKeys)
                {
                    GridManager.Instance.PlaceMultiSlotItem(key, bus, force: true);
                }
            }
        }

    }
}

namespace GreatGames.CaseLib.Utility
{
    public static class ComponentUtils
    {
        public static bool TryGetComponentInChildren<T>(this GameObject obj, out T result) where T : Component
        {
            result = obj.GetComponentInChildren<T>();
            return result != null;
        }

        public static void SetNameBySlotType(GameObject obj, SlotType type, GameKey key)
        {
            string prefix = type switch
            {
                SlotType.Door => "D",
                SlotType.Block => "B",
                _ => "U"
            };

            obj.name = $"{prefix}_{key.ValueAsString}";
        }
    }
}
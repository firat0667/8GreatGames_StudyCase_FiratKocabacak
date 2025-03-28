using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GreatGames.CaseLib.Passenger
{
    public class PassengerGameBuilder : FoundationSingleton<PassengerGameBuilder>, IFoundationSingleton
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject _doorPrefab;
        [SerializeField] private GameObject _blockPrefab;
        [SerializeField] private GameObject _passengerPrefab;

        [Header("Bus Prefabs")]
        [SerializeField] private  GameObject _busHeadPrefab;
        [SerializeField] private  GameObject _busMidPrefab;
        [SerializeField] private  GameObject _busTailPrefab;

        [Header("Dependencies")]
        private GridManager _gridManager;

        [SerializeField] private GameObject _wallPrefab;

        public bool Initialized { get; set; }

        private void Start()
        {
            _gridManager = GridManager.Instance;
        }
        public void BuildLevel()
        {
            CreateByData(_gridManager.LevelData.Blocks, _blockPrefab);
            CreateByData(_gridManager.LevelData.Doors, _doorPrefab, true);
            CreateBuses();
            CreatePassengers();
            BuildWalls();
        }

        private void CreateByData<T>(List<T> dataList, GameObject prefab, bool useColor = false) where T : ISlotData
        {
            foreach (var data in dataList)
            {
                GameKey key = _gridManager.UpperGrid.CreateKeyFromIndex(data.SlotIndex);

                if (data is not DoorData)
                {
                    Vector3 pos = _gridManager.GetSlotPosition(key, true);
                    Instantiate(prefab, pos, Quaternion.identity, transform);
                    continue;
                }

                DoorData door = data as DoorData;
                Vector3 centerPos = _gridManager.GetSlotPosition(key, true);
                Vector3 offset = GetDirectionOffset(door.EnterDirection) * 0.5f;
                Quaternion rotation = GetRotationFromDirection(door.EnterDirection);
                Vector3 finalPos = centerPos + offset;

                var go = Instantiate(prefab, finalPos, rotation, transform);
                if (useColor && door.IncomingColors.Count > 0)
                {
                    if (go.TryGetComponent<Renderer>(out var renderer) ||
                        go.TryGetComponentInChildren<Renderer>(out renderer))
                    {
                        renderer.material.color = ItemColorUtility.GetColor(door.IncomingColors[0]);
                    }

                    for (int i = 0; i < door.IncomingColors.Count && i < go.transform.childCount; i++)
                    {
                        var child = go.transform.GetChild(i);
                        if (child.TryGetComponent<Renderer>(out var childRenderer))
                        {
                            childRenderer.material.color = ItemColorUtility.GetColor(door.IncomingColors[i]);
                        }
                    }
                }
            }
        }

        private void CreateBuses()
        {
            foreach (var busData in _gridManager.LevelData.Buses)
            {
                if (busData.Slots == null || busData.Slots.Count != 3)
                {
                    continue;
                }

                List<GameKey> slotKeys = new();
                foreach (int index in busData.Slots)
                {
                    slotKeys.Add(_gridManager.UpperGrid.CreateKeyFromIndex(index));
                }
                Vector3 headPos = _gridManager.GetSlotPosition(slotKeys[0], true);
                GameObject busGO = new GameObject($"Bus_{slotKeys[0].ValueAsString}");
                busGO.transform.SetParent(transform);
                busGO.transform.position = headPos;

                var controller = busGO.AddComponent<BusController>();
                controller.Initialize(
                    keys: slotKeys,
                    colors: busData.Colors,
                    directions: busData.Directions,
                    gridManager: _gridManager,
                    headPrefab: _busHeadPrefab,
                    midPrefab: _busMidPrefab,
                    tailPrefab: _busTailPrefab
                );
            }
        }

        private void BuildWalls()
        {
            WallBuilder.BuildForGrid(_gridManager.UpperGrid, _gridManager, _wallPrefab, transform);
        }


        private void CreatePassengers()
        {
            foreach (var door in _gridManager.LevelData.Doors)
            {
                GameKey doorKey = _gridManager.UpperGrid.CreateKeyFromIndex(door.SlotIndex);
                Vector3 doorPos = _gridManager.GetSlotPosition(doorKey, true);
                Vector3 offset = GetDirectionOffset(door.EnterDirection);

                float spacing = 0.5f;
                int count = 0;

                for (int i = 0; i < door.IncomingColors.Count; i++)
                {
                    var color = door.IncomingColors[i];
                    var passengerCount = door.IncomingCounts[i];

                    for (int j = 0; j < passengerCount; j++)
                    {
                        Vector3 spawnPos = doorPos + offset * (count + 2) * spacing;
                        Quaternion rotation = GetRotationFromDirection(door.EnterDirection);
                        GameObject go = Instantiate(_passengerPrefab, spawnPos, rotation, transform);

                        var controller = go.GetComponent<PassengerController>();
                        controller.SpawnAtOffset(doorPos, offset, count, color);

                        count++;
                    }
                }
            }
        }
        private Vector3 GetDirectionOffset(Direction dir)
        {
            return dir switch
            {
                Direction.Up => Vector3.forward,
                Direction.Down => Vector3.back,
                Direction.Left => Vector3.left,
                Direction.Right => Vector3.right,
                _ => Vector3.forward
            };
        }
        private Quaternion GetRotationFromDirection(Direction dir)
        {
            return dir switch
            {
                Direction.Up => Quaternion.Euler(0, 0, 0),
                Direction.Down => Quaternion.Euler(0, 180, 0),
                Direction.Left => Quaternion.Euler(0, -90, 0),
                Direction.Right => Quaternion.Euler(0, 90, 0),
                _ => Quaternion.identity
            };
        }

    }

}
public static class ComponentUtils
{
    public static bool TryGetComponentInChildren<T>(this GameObject obj, out T result) where T : Component
    {
        result = obj.GetComponentInChildren<T>();
        return result != null;
    }
}

using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Slinky;
using GreatGames.CaseLib.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GreatGames.CaseLib.Passenger
{
    public class PassengerGameBuilder : FoundationSingleton<PassengerGameBuilder>, IFoundationSingleton
    {
        [Header("Prefabs")]
        public GameObject DoorPrefab;
        public GameObject BlockPrefab;
        public GameObject PassengerPrefab;

        [Header("Bus Prefabs")]
        public GameObject BusHeadPrefab;
        public GameObject BusMidPrefab;
        public GameObject BusTailPrefab;

        [Header("Dependencies")]
        private GridManager _gridManager;

        public bool Initialized { get; set; }

        private void Start()
        {
            _gridManager = GridManager.Instance;
        }
        public void BuildLevel()
        {
            CreateByData(_gridManager.LevelData.Blocks, BlockPrefab);
            CreateByData(_gridManager.LevelData.Doors, DoorPrefab, true);
            CreateBuses();
            CreatePassengers();
        }

        private void CreateByData<T>(List<T> dataList, GameObject prefab, bool useColor = false) where T : ISlotData
        {
            foreach (var data in dataList)
            {
                GameKey key = _gridManager.UpperGrid.CreateKeyFromIndex(data.SlotIndex);
                Vector3 pos = _gridManager.GetSlotPosition(key, true);
                var go = Instantiate(prefab, pos, Quaternion.identity, transform);

                if (useColor)
                {
                    if (data is DoorData door && door.IncomingColors.Count > 0)
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
        }

        private void CreateBuses()
        {
            foreach (var bus in _gridManager.LevelData.Buses)
            {
                if (bus.Slots == null || bus.Slots.Count != 3)
                {
                    Debug.LogWarning("Her otobüs tam olarak 3 slot içermeli! Bu otobüs atlandı.");
                    continue;
                }

                for (int i = 0; i < bus.Slots.Count; i++)
                {
                    GameKey slotKey = _gridManager.UpperGrid.CreateKeyFromIndex(bus.Slots[i]);
                    Vector3 worldPos = _gridManager.GetSlotPosition(slotKey, true);

                    GameObject segmentPrefab = GetBusSegmentPrefab(i);
                    Quaternion rotation = GetBusSegmentRotation(i);

                    if (segmentPrefab == null)
                    {
                        Debug.LogError($"Otobüs prefabı bulunamadı! Index: {i}");
                        continue;
                    }

                    GameObject segment = Instantiate(segmentPrefab, worldPos, rotation, transform);

                    // Renk ata
                    if (i < bus.Colors.Count)
                    {
                        var renderer = segment.GetComponentInChildren<Renderer>();
                        if (renderer != null)
                        {
                            renderer.material.color = ItemColorUtility.GetColor(bus.Colors[i]);
                        }
                    }


                    // İleride initialize falan eklenirse burada verilebilir
                }
            }
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
                        Vector3 spawnPos = doorPos + offset * (count + 1) * spacing;
                        GameObject go = Instantiate(PassengerPrefab, spawnPos, Quaternion.identity, transform);
                        var controller = go.GetComponent<PassengerController>();
                        controller.Initialize(doorKey, ItemColorUtility.GetColor(color));
                        count++;
                    }
                }
            }
        }
        private Vector3 GetDirectionOffset(DoorExitDirection dir)
        {
            return dir switch
            {
                DoorExitDirection.Up => Vector3.forward,
                DoorExitDirection.Down => Vector3.back,
                DoorExitDirection.Left => Vector3.left,
                DoorExitDirection.Right => Vector3.right,
                _ => Vector3.forward
            };
        }
        private GameObject GetBusSegmentPrefab(int index)
        {
            return index switch
            {
                0 => BusHeadPrefab,
                1 => BusMidPrefab,
                2 => BusTailPrefab,
                _ => null
            };
        }

        private Quaternion GetBusSegmentRotation(int index)
        {
            return index switch
            {
                0 => Quaternion.Euler(0, 0f, 0f),     
                1 => Quaternion.Euler(0, 0f, 0f),      
                2 => Quaternion.Euler(0, 180f, 0f),     
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

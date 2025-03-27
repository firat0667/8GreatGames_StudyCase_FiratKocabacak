using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Slinky;
using GreatGames.CaseLib.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
public enum GameMode
{
    SlinkyGame,
    PassengerGame
}
public enum SpawnType
{
    Bus,
    Door,
    Block,
}

namespace GreatGames.CaseLib.EditorTools
{
    [CustomEditor(typeof(LevelConfigSO))]
    public class LevelConfigEditor : Editor
    {
        private LevelConfigSO _levelConfig;
        private int _selectedStartSlot = -1;
        private int _selectedEndSlot = -1;
        private ItemColor _selectedColor = ItemColor.Red;
        private float _cellSize = 40.0f;
        private float _buttonWidth = 40.0f;
        private GameMode _gameMode = GameMode.SlinkyGame;
        private SpawnType _selectedSpawnType = SpawnType.Bus;
        [Header("Bus")]
        private List<int> _selectedBusSlots = new List<int>();
        private List<ItemColor> _selectedBusColors = new List<ItemColor>();


        private void OnEnable()
        {
            _levelConfig = (LevelConfigSO)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Level Editor", EditorStyles.boldLabel);
            DrawColorSelection();
            DrawGrid(_levelConfig.UpperGridSize, true);
            DrawGrid(_levelConfig.LowerGridSize, false);
            if (_gameMode == GameMode.SlinkyGame)
            {
                EditorGUILayout.LabelField("Slinky List", EditorStyles.boldLabel);
                foreach (var slinky in _levelConfig.Slinkies)
                {
                    string startSlotStr = GetSlotIndexString(slinky.StartSlot, _levelConfig.UpperGridSize);
                    string endSlotStr = GetSlotIndexString(slinky.EndSlot, _levelConfig.UpperGridSize);
                    EditorGUILayout.LabelField($"Start: {startSlotStr}  End: {endSlotStr} | Color: {slinky.Color}");
                }
            }
            else
            {
                if (_selectedSpawnType == SpawnType.Bus)
                {
                    EditorGUILayout.LabelField("Bus List", EditorStyles.boldLabel);
                    foreach (var bus in _levelConfig.Buses)
                    {
                        for (int i = 0; i < bus.Slots.Count; i++)
                        {
                            string slotStr = GetSlotIndexString(bus.Slots[i], _levelConfig.UpperGridSize);
                            string colorStr = bus.Colors.Count > i ? bus.Colors[i].ToString() : "Unknown";
                            EditorGUILayout.LabelField($"Slot: {slotStr} | Color: {colorStr}");
                        }
                        EditorGUILayout.Space();
                    }
                }

                else if (_selectedSpawnType == SpawnType.Door)
                {
                    EditorGUILayout.LabelField("Door List", EditorStyles.boldLabel);
                    foreach (var door in _levelConfig.Doors)
                    {
                        string slotStr = GetSlotIndexString(door.SlotIndex, _levelConfig.UpperGridSize);
                        string colorList = string.Join(", ", door.IncomingColors);
                        string countList = string.Join(", ", door.IncomingCounts);
                        EditorGUILayout.LabelField($"Door At: {slotStr} | Colors: {colorList} | Counts: {countList}");
                    }
                }
                else if (_selectedSpawnType == SpawnType.Block)
                {
                    EditorGUILayout.LabelField("Block List", EditorStyles.boldLabel);
                    foreach (var block in _levelConfig.Blocks)
                    {
                        string slotStr = GetSlotIndexString(block.SlotIndex, _levelConfig.UpperGridSize);
                        EditorGUILayout.LabelField($"Blocked Slot: {slotStr}");
                    }
                }
            }

            GUILayout.Space(10);
            DrawDefaultInspector();
        }

        private void DrawColorSelection()
        {
            _gameMode = (GameMode)EditorGUILayout.EnumPopup("Game Mode", _gameMode);

            if (_gameMode == GameMode.SlinkyGame)
            {
                EditorGUILayout.LabelField("Select Slinky Color", EditorStyles.boldLabel);
                _selectedColor = (ItemColor)EditorGUILayout.EnumPopup("Slinky Color", _selectedColor);
            }
            else if (_gameMode == GameMode.PassengerGame)
            {
                _selectedSpawnType = (SpawnType)EditorGUILayout.EnumPopup("Spawn Type", _selectedSpawnType);

                if (_selectedSpawnType == SpawnType.Bus || _selectedSpawnType == SpawnType.Door || _selectedSpawnType == SpawnType.Block)
                {
                    _selectedColor = (ItemColor)EditorGUILayout.EnumPopup("Color", _selectedColor);
                }
            }

            GUILayout.Space(10);
        }

        private void DrawGrid(Vector2Int gridSize, bool isUpperGrid)
        {
            GUILayout.BeginVertical();

            for (int y = 0; y < gridSize.y; y++)
            {
                GUILayout.BeginHorizontal();
                for (int x = 0; x < gridSize.x; x++)
                {
                    int slotIndex = GetSlotIndex(x, y, gridSize);
                    string slotLabel = $"{x},{y}";
                    if (_levelConfig.Slinkies.Exists(s => s.StartSlot == slotIndex || s.EndSlot == slotIndex))
                    {
                        slotLabel = "SLNK";
                    }
                    else if (_levelConfig.Buses.Any(b => b.Slots.Contains(slotIndex)))
                    {
                        slotLabel = "BUS";
                    }
                    else if (_levelConfig.Doors.Exists(d => d.SlotIndex == slotIndex))
                    {
                        slotLabel = "DOOR";
                    }
                    else if (_levelConfig.Blocks.Exists(b => b.SlotIndex == slotIndex))
                    {
                        slotLabel = "BLCK";
                    }
                    Color buttonColor = Color.white;

                    var slinky = _levelConfig.Slinkies.Find(s => s.StartSlot == slotIndex || s.EndSlot == slotIndex);
                    if (slinky != null)
                    {
                        buttonColor = ItemColorUtility.GetColor(slinky.Color);
                    }
                    else
                    {
                        var bus = _levelConfig.Buses.Find(b => b.Slots.Contains(slotIndex));
                        if (bus != null)
                        {
                            int colorIndex = bus.Slots.IndexOf(slotIndex);
                            if (colorIndex >= 0 && colorIndex < bus.Colors.Count)
                            {
                                buttonColor = ItemColorUtility.GetColor(bus.Colors[colorIndex]);
                            }
                            else
                            {
                                buttonColor = Color.gray; 
                            }
                        }
                        else
                        {
                            var door = _levelConfig.Doors.Find(d => d.SlotIndex == slotIndex);
                            if (door != null)
                            {
                                buttonColor = Color.cyan;
                            }
                            else
                            {
                                var block = _levelConfig.Blocks.Find(b => b.SlotIndex == slotIndex);
                                if (block != null)
                                {
                                    buttonColor = Color.black;
                                }
                            }
                        }
                    }

                    GUI.backgroundColor = buttonColor;
                    if (isUpperGrid && GUILayout.Button(slotLabel, GUILayout.Width(_buttonWidth), GUILayout.Height(_cellSize)))
                    {
                        if (_gameMode == GameMode.SlinkyGame)
                        {
                            HandleSlotClick(slotIndex, isUpperGrid);
                        }
                        else
                        {
                            if (_selectedSpawnType == SpawnType.Bus)
                            {
                                HandleBusPlacement(slotIndex);
                            }
                            else if (_selectedSpawnType == SpawnType.Door)
                            {
                                HandleDoorPlacement(slotIndex);
                            }
                            else if (_selectedSpawnType == SpawnType.Block)
                            {
                                HandleBlockPlacement(slotIndex);
                            }
                        }
          
                    }
                    GUI.backgroundColor = Color.white;
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.Space(10);
        }

        private void HandleSlotClick(int slotIndex, bool isUpperGrid)
        {
            if (!isUpperGrid) return;

            if (_selectedStartSlot == -1)
            {
                _selectedStartSlot = slotIndex;
            }
            else if (_selectedEndSlot == -1)
            {
                _selectedEndSlot = slotIndex;

                string startSlotStr = GetSlotIndexString(_selectedStartSlot, _levelConfig.UpperGridSize);
                string endSlotStr = GetSlotIndexString(_selectedEndSlot, _levelConfig.UpperGridSize);

                Debug.Log($"Selected Start Slot: {startSlotStr}, End Slot: {endSlotStr}"); 

                AddSlinkyToLevel(_selectedStartSlot, _selectedEndSlot, isUpperGrid);

                _selectedStartSlot = -1;
                _selectedEndSlot = -1;
            }
        }
        private void HandleBusPlacement(int clickedSlot)
        {
            if (_selectedBusSlots.Contains(clickedSlot))
                return;

            _selectedBusSlots.Add(clickedSlot);
            _selectedBusColors.Add(_selectedColor); 

            if (_selectedBusSlots.Count == 3)
            {
                Undo.RegisterCompleteObjectUndo(_levelConfig, "Add Bus");

                _levelConfig.Buses.Add(new BusData
                {
                    Slots = new List<int>(_selectedBusSlots),
                    Colors = new List<ItemColor>(_selectedBusColors)
                });

                Debug.Log($"Bus created on slots: {string.Join(", ", _selectedBusSlots)}");

                _selectedBusSlots.Clear();
                _selectedBusColors.Clear();

                EditorUtility.SetDirty(_levelConfig);
            }
        }
        private void HandleDoorPlacement(int slotIndex)
        {
            Undo.RegisterCompleteObjectUndo(_levelConfig, "Add Door");

            var door = new DoorData
            {
                SlotIndex = slotIndex,
                IncomingColors = new List<ItemColor> { _selectedColor },
                IncomingCounts = new List<int> { 4 }
            };

            _levelConfig.Doors.Add(door);

            EditorUtility.SetDirty(_levelConfig);
        }
        private void HandleBlockPlacement(int slotIndex)
        {
            if (_levelConfig.Blocks.Any(b => b.SlotIndex == slotIndex)) return;

            Undo.RegisterCompleteObjectUndo(_levelConfig, "Add Block");
            _levelConfig.Blocks.Add(new BlockData { SlotIndex = slotIndex });
            EditorUtility.SetDirty(_levelConfig);
        }

        private void AddSlinkyToLevel(int startSlot, int endSlot, bool isUpperGrid)
        {
            Undo.RegisterCompleteObjectUndo(_levelConfig, "Add Slinky");

            string startSlotStr = GetSlotIndexString(startSlot, _levelConfig.UpperGridSize);
            string endSlotStr = GetSlotIndexString(endSlot, _levelConfig.UpperGridSize);

            Debug.Log($"Adding Slinky from {startSlotStr} to {endSlotStr}");

            _levelConfig.Slinkies.Add(new SlinkyData(startSlot, endSlot, _selectedColor));

            EditorUtility.SetDirty(_levelConfig);
        }
        private string GetSlotIndexString(int slotIndex, Vector2Int gridSize)
        {
            int x = slotIndex % gridSize.x; 
            int y = slotIndex / gridSize.x; 

            return $"{x},{y}";
        }


        private int GetSlotIndex(int x, int y, Vector2Int gridSize)
        {
            return x + y * gridSize.x;
        }
    }
}

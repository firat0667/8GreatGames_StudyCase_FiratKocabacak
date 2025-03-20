using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Utility;
using UnityEditor;
using UnityEngine;

namespace GreatGames.CaseLib.EditorTools
{
    [CustomEditor(typeof(LevelConfigSO))]
    public class LevelConfigEditor : Editor
    {
        private LevelConfigSO _levelConfig;
        private int _selectedStartSlot = -1;
        private int _selectedEndSlot = -1;
        private SlinkyColor _selectedColor = SlinkyColor.Red;
        private float _cellSize = 40.0f;
        private float _buttonWidth = 40.0f;

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
            EditorGUILayout.LabelField("Slinky List", EditorStyles.boldLabel);
            foreach (var slinky in _levelConfig.Slinkies)
            {
                string startSlotStr = GetSlotIndexString(slinky.StartSlot, _levelConfig.UpperGridSize);
                string endSlotStr = GetSlotIndexString(slinky.EndSlot, _levelConfig.UpperGridSize);

                EditorGUILayout.LabelField($"Start: {startSlotStr} → End: {endSlotStr} | Color: {slinky.Color}");
            }
            GUILayout.Space(10);
            DrawDefaultInspector();
        }

        private void DrawColorSelection()
        {
            EditorGUILayout.LabelField("Select Slinky Color", EditorStyles.boldLabel);
            _selectedColor = (SlinkyColor)EditorGUILayout.EnumPopup("Slinky Color", _selectedColor);
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

                    SlinkyData slinky = _levelConfig.Slinkies.Find(s => s.StartSlot == slotIndex || s.EndSlot == slotIndex);
                    Color buttonColor = slinky != null ? SlinkyColorUtility.GetColor(slinky.Color) : Color.white;

                    GUI.backgroundColor = buttonColor;
                    if (isUpperGrid && GUILayout.Button(slotLabel, GUILayout.Width(_buttonWidth), GUILayout.Height(_cellSize)))
                    {
                        HandleSlotClick(slotIndex, isUpperGrid);
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

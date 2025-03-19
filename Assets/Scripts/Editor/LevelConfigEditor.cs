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
                AddSlinkyToLevel(_selectedStartSlot, _selectedEndSlot, isUpperGrid);

                _selectedStartSlot = -1;
                _selectedEndSlot = -1;
            }
        }

        private void AddSlinkyToLevel(int startSlot, int endSlot, bool isUpperGrid)
        {
            Undo.RegisterCompleteObjectUndo(_levelConfig, "Add Slinky");

            _levelConfig.Slinkies.Add(new SlinkyData(startSlot, endSlot, _selectedColor));

            EditorUtility.SetDirty(_levelConfig);
        }

        private int GetSlotIndex(int x, int y, Vector2Int gridSize)
        {
            return x + y * gridSize.x;
        }
    }
}

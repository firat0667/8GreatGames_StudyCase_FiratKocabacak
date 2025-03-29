using UnityEditor;
using UnityEngine;

namespace GreatGames.CaseLib.EditorTools
{
    [CustomEditor(typeof(LevelConfigSO))]
    public class LevelConfigEditor : Editor
    {
        private LevelConfigSO _levelConfig;
        private LevelEditorState _state;

        private void OnEnable()
        {
            _levelConfig = (LevelConfigSO)target;
            _state = new LevelEditorState();
        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Level Editor", EditorStyles.boldLabel);

            LevelEditorUI.DrawColorSelection(_levelConfig, ref _state);

            LevelEditorGridDrawer.DrawGrid(_levelConfig, _levelConfig.UpperGridSize, true, _state);
            LevelEditorGridDrawer.DrawGrid(_levelConfig, _levelConfig.LowerGridSize, false, _state);

            LevelEditorUI.DrawListView(_levelConfig, _state);

            GUILayout.Space(10);
            DrawDefaultInspector();
        }
    }
}

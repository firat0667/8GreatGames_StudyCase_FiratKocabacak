using GreatGames.CaseLib.Patterns;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GreatGames.CaseLib.UI
{

    [System.Serializable]
    public class PanelEntry
    {
    public GameState panelType;
    public GameObject panelObject;
    }
    public class UIManager : FoundationSingleton<UIManager>, IFoundationSingleton
    {
        public bool Initialized { get; set; }

        [SerializeField] private List<PanelEntry> panels;

        private Stack<GameObject> _panelStack = new Stack<GameObject>();
        private Dictionary<GameState, GameObject> _panelMap;

        [SerializeField] private Button _retryButton;

        [SerializeField] private Button _nextButton;

        private LevelManager _levelManager;

        private void Awake()
        {
            _panelMap = new Dictionary<GameState, GameObject>();

            foreach (var entry in panels)
            {
                if (!_panelMap.ContainsKey(entry.panelType))
                {
                    _panelMap.Add(entry.panelType, entry.panelObject);
                    entry.panelObject.SetActive(false);
                }
            }
        }
        private void Start()
        {
            _levelManager = LevelManager.Instance;
            _retryButton.onClick.AddListener(OnRetryLevelButtonPressed);
            _nextButton.onClick.AddListener(OnNextLevelButtonPressed);
        }
        public void PushPanel(GameState panelType)
        {
            if (_panelMap.TryGetValue(panelType, out var panel))
            {
                panel.SetActive(true);
                _panelStack.Push(panel);
            }
            else
            {
                Debug.LogWarning($"Panel notFound: {panelType}");
            }
        }

        public void PopPanel()
        {
            if (_panelStack.Count > 0)
            {
                var top = _panelStack.Pop();
                top.SetActive(false);
            }
        }
        public void ClearAllPanels()
        {
            while (_panelStack.Count > 0)
            {
                var panel = _panelStack.Pop();
                panel.SetActive(false);
            }
        }
        public void ShowLevelDonePanel() => PushPanel(GameState.LevelDone);
        public void ShowLevelFailedPanel() => PushPanel(GameState.LevelFailed);
        public void OnNextLevelButtonPressed() => _levelManager.LoadNextLevel();
        public void OnRetryLevelButtonPressed() => _levelManager.RestartLevel();
    }
}

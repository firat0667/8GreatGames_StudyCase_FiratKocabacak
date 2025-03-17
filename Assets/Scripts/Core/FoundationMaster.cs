using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Patterns;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GreatGames.CaseLib.Patterns
{
    public class FoundationMaster : MonoBehaviour
    {
        [Header("This is the master for Foundation singletons.")]
        [SerializeField] private bool _findAllSingletonsAtAwake = true;

        public static FoundationMaster Instance => _instance;
        private static FoundationMaster _instance;

        public readonly Dictionary<Key.GameKey, IFoundationSingleton> _singletons = new();

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }

            if (_findAllSingletonsAtAwake)
            {
                IFoundationSingleton[] singletons = FindObjectsOfType<MonoBehaviour>().OfType<IFoundationSingleton>().ToArray();

                foreach (var singleton in singletons)
                {
                    singleton.Init();

                    var name = singleton.GetType().Name;
                    var key = new Key.GameKey(name);

                    if (!_singletons.TryAdd(key, singleton))
                    {
                        Debug.LogWarning($"Key for {singleton.GetType()} failed to add.");
                    }
                    else
                    {
                        Debug.Log($"Key for {singleton.GetType()} added to {gameObject.name}.");
                    }
                }
            }

        }
    }

}
using UnityEngine;

namespace GreatGames.CaseLib.Patterns
{
    /// <summary>
    /// DI for FoundationSingletons.
    /// </summary>
    public interface IFoundationSingleton : DI.IInitializable
    {
    }

    /// <summary>
    /// Singleton using the Foundation system.
    /// </summary>
    public abstract class FoundationSingleton<T> : MonoBehaviour where T : MonoBehaviour, IFoundationSingleton
    {
        public static T Instance => _instance;
        private static T _instance;

        public void Init()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this as T;
            }
        }
    }


}
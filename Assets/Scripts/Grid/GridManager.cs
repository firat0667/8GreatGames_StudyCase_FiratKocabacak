using UnityEngine;
using System.Collections.Generic;
using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Signals;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Key;



namespace GreatGames.CaseLib.Grid
{
    public class GridManager : MonoBehaviour, IFoundationSingleton, IInitializable
    {

        private Dictionary<GameKey, GridData> _gridSlots = new Dictionary<GameKey, GridData>();
        public BasicSignal OnGridUpdated { get; private set; }
        public bool Initialized { get; set; }

        public void Init()
        {
            OnGridUpdated = new BasicSignal();
        }

        public void InitializeGrid(int width, int height)
        {
            _gridSlots.Clear();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    GameKey key = new GameKey($"Slot: _{x}_{y}");
                    Vector3 worldPos = new Vector3(x, 0, -y);
                    _gridSlots[key] = new GridData(key.Value, worldPos);
                }
            }
            OnGridUpdated.Emit(); 
        }

        public bool TryPlaceSlinky(GameKey slotKey, SlinkyData slinky)
        {
            if (!_gridSlots.ContainsKey(slotKey) || _gridSlots[slotKey].IsOccupied) return false;

            _gridSlots[slotKey].SetOccupied(true);
            OnGridUpdated.Emit(); 
            return true;
        }

        public bool IsSlotEmpty(GameKey key)
        {
            return _gridSlots.ContainsKey(key) && !_gridSlots[key].IsOccupied;
        }

        public Vector3 GetSlotPosition(GameKey key)
        {
            return _gridSlots.ContainsKey(key) ? _gridSlots[key].Position : Vector3.zero;
        }
    }
}

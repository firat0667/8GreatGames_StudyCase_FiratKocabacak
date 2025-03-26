using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Slinky;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GreatGames.CaseLib.Managers
{
    public class SlinkyManager : FoundationSingleton<SlinkyManager>, IFoundationSingleton
    {
        [SerializeField] private Transform _slinkyParent;
        [SerializeField] private GameObject _slinkyPrefab;

        private GridManager _gridManager;

        public bool Initialized { get; set; }

        private void Start()
        {
            _gridManager = GridManager.Instance;
        }

        public void SpawnSlinkies(List<SlinkyData> slinkyDataList)
        {
            var startGrid = _gridManager.UpperGrid;

            foreach (var data in slinkyDataList)
            {
                GameKey startKey = GameKeyExtensions.CreateFromIndex(data.StartSlot, startGrid.Size.x, true);
                GameKey endKey = GameKeyExtensions.CreateFromIndex(data.EndSlot, startGrid.Size.x, true);

                Vector3 startPos = startGrid.GetWorldPosition(startKey);
                Vector3 endPos = startGrid.GetWorldPosition(endKey);

                GameObject startSlotObject = startGrid.GetSlotObject(startKey);
                GameObject endSlotObject = startGrid.GetSlotObject(endKey);

                GameObject container = new GameObject($"Slinky_{startKey.ValueAsString}");
                container.transform.SetParent(_slinkyParent);
                container.transform.position = startPos;

                GameObject slinkyGO = Instantiate(_slinkyPrefab, startPos, Quaternion.identity, container.transform);
                var slinky = slinkyGO.GetComponent<SlinkyController>();

                slinky.Initialize(startPos, endPos, _gridManager, data.Color, startSlotObject, endSlotObject, container.transform);

                startGrid.PlaceItem(startKey, slinky);
                startGrid.PlaceItem(endKey, slinky);

                _gridManager.RegisterItem(slinky);
            }
        }

        public List<ISlotItem> GetSlinkiesInLowerGrid()
        {
            return _gridManager.GetAllItemsInLowerGrid();
        }
        public bool IsAnyItemMoving()
        {
            return GetSlinkiesInLowerGrid()
                .OfType<SlinkyController>()
                .Any(s => s.IsMoving);
        }
        public void ShiftAllSlinkies()
        {
            var mover = new SlinkyMover(GridManager.Instance.LowerGrid, GridManager.Instance);
            mover.ShiftRemainingSlinkies();
        }

    }
}

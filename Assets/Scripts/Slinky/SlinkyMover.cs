using DG.Tweening;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Managers;
using GreatGames.CaseLib.Match;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

namespace GreatGames.CaseLib.Slinky
{
    public class SlinkyMover
    {
        private readonly GridStructure _grid;
        private readonly GridManager _gridManager;

        public SlinkyMover(GridStructure grid, GridManager manager)
        {
            _grid = grid;
            _gridManager = manager;
        }

        public static void Move(SlinkyController slinky, GameKey targetKey, SlinkyController initiator = null, bool shifting = false)
        {
            if (slinky == null || targetKey == null || slinky.IsMoving)
                return;

            var gridManager = GridManager.Instance;
         
            Vector3 targetPosition = gridManager.GetSlotPosition(targetKey, false);

            float moveTime = shifting
                ? slinky.ShiftingDuration * slinky.MoveTimeMultiplierValue
                : slinky.MoveDurationValue * slinky.MoveTimeMultiplierValue;

            float delayBetweenSegments = slinky.DelayBetweenSegments;
            float arcHeight = slinky.ArcHeight;
            Ease movementEase = slinky.MovementEase;

            var segments = slinky.Segments;
            for (int i = 0; i < segments.Count; i++)
            {
                int index = i;
                float delay = i * delayBetweenSegments;
                Vector3 startPos = segments[index].position;
                Vector3 midPoint = (startPos + targetPosition) / 2 + Vector3.up * arcHeight;

                segments[index].DOPath(new Vector3[] { midPoint, targetPosition }, moveTime, PathType.CatmullRom)
                    .SetDelay(delay)
                    .SetEase(movementEase)
                    .SetRelative(false);
            }

            DOVirtual.DelayedCall(moveTime + delayBetweenSegments * segments.Count, () =>
            {
                slinky.SetIsMoving(false);
                slinky.OnMovementComplete?.Emit();
              //  MatchManager.Instance.CheckForMatch();
                var mover = new SlinkyMover(gridManager.LowerGrid, gridManager);
            });
            gridManager.UpdateItemSlot(slinky, targetKey);
            gridManager.PlaceItem(targetKey, slinky, false);


            foreach (Transform segment in segments)
            {
                var hinge = segment.GetComponent<HingeJoint>();
                if (hinge != null) Object.Destroy(hinge);

                var rb = segment.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;
            }

            slinky.SetIsMoving(true);
        }

        public bool ShiftUntilFit(GameKey targetKey, SlinkyController self)
        {
            var slots = _grid.GetAllSlots().OrderByDescending(kvp => kvp.Key.ToVector2Int().x).ToList();
            int index = slots.FindIndex(kvp => kvp.Key == targetKey);
            if (index == -1) return false;

            int rightmostFreeIndex = -1;
            for (int i = index + 1; i < slots.Count; i++)
            {
                if (!slots[i].Value.IsOccupied)
                {
                    rightmostFreeIndex = i;
                    break;
                }
            }
            if (rightmostFreeIndex == -1) return false;

            bool anyMoving = SlinkyManager.Instance.IsAnyItemMoving();

            for (int i = rightmostFreeIndex; i > index; i--)
            {
                var from = slots[i - 1];
                var to = slots[i];

                var slinky = from.Value.Item as SlinkyController;

                if (slinky == null || slinky == self) continue;

                _grid.ClearSlot(from.Key);
                _gridManager.PlaceItem(to.Key, slinky, false);
                slinky.OccupiedGridKeys.Clear();
                slinky.OccupiedGridKeys.Add(to.Key);

                if (anyMoving)
                {
                    Teleport(slinky, to.Key);
                }
                else
                {
                    Move(slinky, to.Key, null, true);
                }
            }

            return true;
        }


        public void ShiftRemainingSlinkies(SlinkyController initiator = null)
        {
            var gridManager = GridManager.Instance;

            var emptySlots = _grid.GetAllSlots()
                .Where(kvp => !kvp.Value.IsOccupied)
                .OrderBy(kvp => kvp.Key.ToVector2Int().x) 
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var slot in emptySlots)
                Debug.Log($" - {slot.ValueAsString}");

            var slinkies = SlinkyManager.Instance.GetSlinkiesInLowerGrid()
                .OfType<SlinkyController>()
                .OrderBy(s => s.OccupiedGridKeys[0].ToVector2Int().x) 
                .ToList();

            if (initiator != null)
                slinkies.Remove(initiator);


            foreach (var slinky in slinkies)
            {
                var currentX = slinky.OccupiedGridKeys[0].ToVector2Int().x;

                var targetSlot = emptySlots
                    .Where(slot => slot.ToVector2Int().x > currentX)
                    .OrderBy(slot => slot.ToVector2Int().x)
                    .FirstOrDefault();

                if (targetSlot == null)
                    continue;
                var currentSlot = slinky.SlotIndex;
                _grid.ClearSlot(currentSlot);
                _gridManager.PlaceItem(targetSlot, slinky, false);
                Move(slinky, targetSlot);
                emptySlots.Remove(targetSlot);
            }
        }
        public static void Teleport(SlinkyController slinky, GameKey targetKey)
        {
            if (slinky == null || targetKey == null) return;

            var gridManager = GridManager.Instance;
            Vector3 targetPosition = gridManager.GetSlotPosition(targetKey, false);

            foreach (var segment in slinky.Segments)
            {
                segment.position = targetPosition;

                var hinge = segment.GetComponent<HingeJoint>();
                if (hinge != null) Object.Destroy(hinge);

                var rb = segment.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;
            }

            slinky.SetIsMoving(false);
            slinky.OnMovementComplete?.Emit();

            gridManager.UpdateItemSlot(slinky, targetKey);
            gridManager.PlaceItem(targetKey, slinky, false);
        }

    }
}
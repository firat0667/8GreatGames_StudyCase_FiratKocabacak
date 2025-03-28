using UnityEngine;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;

namespace GreatGames.CaseLib.Passenger
{
    public static class WallBuilder
    {
        public static void BuildForGrid(GridStructure grid, GridManager gridManager, GameObject wallPrefab, Transform parent)
        {
            Vector2Int gridSize = grid.Size;
            float slotSize = 1f;
            Vector3 half = Vector3.one * slotSize / 2f;

            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    GameKey key = grid.CreateKeyFromIndex(x + y * gridSize.x);
                    Vector3 centerPos = gridManager.GetSlotPosition(key, true);

                    bool hasLeft = x > 0;
                    bool hasRight = x < gridSize.x - 1;
                    bool hasDown = y > 0;
                    bool hasUp = y < gridSize.y - 1;

                    if (!hasLeft)
                        PlaceWall(centerPos + Vector3.left * half.x, Quaternion.Euler(0, 0, 0), wallPrefab, parent);
                    if (!hasRight)
                        PlaceWall(centerPos + Vector3.right * half.x, Quaternion.Euler(0, 0, 0), wallPrefab, parent);
                    if (!hasDown)
                        PlaceWall(centerPos + Vector3.forward * half.z, Quaternion.Euler(0, 90, 0), wallPrefab, parent);
                    if (!hasUp)
                        PlaceWall(centerPos + Vector3.back * half.z, Quaternion.Euler(0, 90, 0), wallPrefab, parent);
                }
            }
        }

        private static void PlaceWall(Vector3 pos, Quaternion rot, GameObject prefab, Transform parent)
        {
            Object.Instantiate(prefab, pos, rot, parent);
        }
    }
}

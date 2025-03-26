using GreatGames.CaseLib.Grid;
using UnityEngine;

public class GridBuilder
{
    public (GridStructure upper, GridStructure lower) Build(
        LevelConfigSO config,
        GameObject prefab,
        Transform parent,
        Vector3 upperOffset,
        Vector3 lowerOffset)
    {
        AlignGridOffsets(config.UpperGridSize, config.LowerGridSize, ref upperOffset, ref lowerOffset);

        var upper = new GridStructure(config.UpperGridSize, upperOffset, prefab, true);
        var lower = new GridStructure(config.LowerGridSize, lowerOffset, prefab, false);

        upper.InitializeGrid(config.UpperGridSize, parent);
        lower.InitializeGrid(config.LowerGridSize, parent);

        return (upper, lower);
    }

    private void AlignGridOffsets(Vector2Int upperSize, Vector2Int lowerSize, ref Vector3 upperOffset, ref Vector3 lowerOffset)
    {
        int diff = upperSize.x - lowerSize.x;

        if (diff > 0)
            lowerOffset.x = diff / 2f;
        else if (diff < 0)
            upperOffset.x = -diff / 2f;
    }
}

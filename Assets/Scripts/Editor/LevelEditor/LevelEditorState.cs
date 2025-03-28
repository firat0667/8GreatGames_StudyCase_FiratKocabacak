using GreatGames.CaseLib.Utility;
using System.Collections.Generic;
public enum GameMode
{
    SlinkyGame,
    PassengerGame
}

public enum SpawnType
{
    Bus,
    Door,
    Block
}
public class LevelEditorState
{
    public GameMode GameMode;
    public SpawnType SelectedSpawnType;
    public ItemColor SelectedColor = ItemColor.Red;

    public List<int> SelectedBusSlots = new();
    public List<ItemColor> SelectedBusColors = new();

    public Direction SelectedExitDirection = Direction.Up;
    public List<Direction> SelectedBusDirections = new();

    public int SelectedStartSlot = -1;
    public int SelectedEndSlot = -1;
}

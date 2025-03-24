namespace GreatGames.CaseLib.DI
{
    public class Container : IContainer
    {
        public object Value { get; set; }

        public Container(object cargo)
        {
            Value = cargo;
        }
    }
    public interface IContainer
    {
        object Value { get; set; }
    }
}
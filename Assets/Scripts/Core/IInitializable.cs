namespace GreatGames.CaseLib.DI
{
    /// <summary>
    /// Can be initialized.
    /// </summary>
    public interface IInitializable
    {
        /// <summary> Signifies if the class is ready to use. </summary>
        bool Initialized { get; set; }

        /// <summary> Initialize to use. </summary>
        void Init()
        {
            Initialized = true;
        }

        void Init(Container container)
        {
            Init();
        }

        void Init(Container containerOne, Container containerTwo)
        {
            Init();
        }

        void Init<T>(T genericType)
        {
            Init();
        }
    }
}
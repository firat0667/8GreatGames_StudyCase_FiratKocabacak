namespace GreatGames.CaseLib.DI // Dependecy Injection system
{ 
    // Can be initialized.
    public interface IInitializable
    {
        // Signifies if the class is ready to use
        bool Initialized { get; set; }

        // Initialize to use
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
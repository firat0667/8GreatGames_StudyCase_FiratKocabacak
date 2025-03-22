namespace GreatGames.CaseLib.Diagnostics
{
    public interface IStopWatch
    {
        int TimeInSeconds { get; }
        int FixedTimeInSeconds { get; }

        void Tick();
        void FixedTick();
        void StopClock();
        void StartClock();
        void RestartClock();
    }
}

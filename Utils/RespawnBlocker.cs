using Exiled.API.Features;
using ServerHandler = Exiled.Events.Handlers.Server;

namespace VeryUsualDay.Utils
{
    public static class RespawnBlocker
    {
        public static void Enable()
        {
            ServerHandler.RoundStarted += OnRoundStarted;
            Respawn.PauseWaves();
        }
        private static void OnRoundStarted()
        {
            Respawn.PauseWaves();
        }
        public static void Disable()
        {
            ServerHandler.RoundStarted -= OnRoundStarted;
            Respawn.ResumeWaves();
        }
    }
}

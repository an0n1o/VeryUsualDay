using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using UnityEngine;
using VeryUsualDay.Utils;
using VeryUsualDay.Abilities.Scp682Event;

namespace VeryUsualDay.Handlers
{
    public static class Server
    {
        public static void OnWaitingForPlayers()
        {
            VeryUsualDay.Instance.IsEnabledInRound = false;
            VeryUsualDay.Instance.IsLunchtimeActive = false;
            VeryUsualDay.Instance.IsDboysSpawnAllowed = false;
            VeryUsualDay.Instance.Is008Leaked = false;
            VeryUsualDay.Instance.Is682EventActive = false;
            VeryUsualDay.Instance.CurrentCode = VeryUsualDay.Codes.Green;
            VeryUsualDay.Instance.BuoCounter = 0;
            VeryUsualDay.Instance.OssCounter = 0;
            VeryUsualDay.Instance.SpawnedDboysCounter = 1;
            VeryUsualDay.Instance.SpawnedWorkersCounter = 1;
            VeryUsualDay.Instance.SpawnedScientistCounter = 1;
            VeryUsualDay.Instance.SpawnedSecurityCounter = 1;
            VeryUsualDay.Instance.ScpPlayers.Clear();
            VeryUsualDay.Instance.Zombies.Clear();
            VeryUsualDay.Instance.JoinedDboys.Clear();
            VeryUsualDay.Instance.DBoysQueue.Clear();
            VeryUsualDay.Instance.ChaosRooms.Clear();
            VeryUsualDay.Instance.Shakheds.Clear();
            Scp682EventAbilityManager.Reset();
            // Timing.KillCoroutines("_avel");
            Timing.KillCoroutines("_008_poisoning");
            Timing.KillCoroutines("_joining");
            Timing.KillCoroutines("_prisonTimer");
            Timing.KillCoroutines("_chaos");
        }

        public static void OnRoundStarted()
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound) return;
            Timing.CallDelayed(5f, () => // this shit is broken as fuck
            {
                VeryUsualDay.Instance.SupplyBoxCoords = Room.Get(RoomType.EzGateB).Position + new Vector3(-6.193f, 2.243f, -5.901f);
            });
        }
    }
}

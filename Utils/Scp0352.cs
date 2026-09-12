using Exiled.API.Features;
using MEC;
using UnityEngine;
using VeryUsualDay.Abilities.Medic;

namespace VeryUsualDay.Utils
{
    public class Scp0352
    {
        private Player User;

        public Scp0352(Player player)
        {
            User = player;
            _spawn();
        }

        private void _spawn()
        {
            MedicReviveAbility.MarkCustomScpSpawn(User);
            User.CustomInfo = "<b><color=#960018>SCP-035-2</color></b>";
            User.MaxHealth = 350f;
            User.Health = 350f;
            User.Scale = new Vector3(1f, 1f, 1f);
            User.IsGodModeEnabled = false;
            User.Broadcast(10, "Вы теперь подчиняетесь SCP-035.");
            VeryUsualDay.Instance.ScpPlayers.Add(User.Id, VeryUsualDay.Scps.Scp0352);
        }
    }
}
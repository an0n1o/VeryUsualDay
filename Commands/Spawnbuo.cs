using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using VeryUsualDay.Utils;

namespace VeryUsualDay.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Spawnbuo : ICommand
    {
        public string Command => "spawnbuo";

        public string[] Aliases => new string[] { };

        public string Description => "Позволяет заспавнить бойцов БУО. Использование: spawnbuo <id через пробел>. Для FX.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound)
            {
                response = "Режим FX не включён";
                return false;
            }
            if (arguments.Count < 1)
            {
                response = "Формат команды: spawnbuo <id через пробел>";
                return false;
            }
            VeryUsualDay.Instance.BuoCounter += 1;
            var peopleCounter = 1;
            foreach (var id in arguments.ToArray())
            {
                if (!Player.TryGet(int.Parse(id), out var player)) continue;
                var randomRole = UnityEngine.Random.Range(0f, 1f);
                if (randomRole <= 0.5f)
                {
                    player.Role.Set(RoleTypeId.ChaosConscript, RoleSpawnFlags.AssignInventory);
                    Timing.CallDelayed(2f, () =>
                    {
                        SpecialUnitManager.Apply(
    player,
    VeryUsualDay.Instance.Config.BuoPrivateInventory,
    VeryUsualDay.Instance.Config.BuoEffects["Боец"],
    375f,
    $"БУО #{VeryUsualDay.Instance.BuoCounter} - ##-{peopleCounter}",
    "(Боец БУО)",
    "<color=#708090><b>Вы стали бойцом <color=#138808>Боевого Ударного Отряда<color=#708090>. Спасите <color=#ffd800>сотрудников фонда<color=#708090>, устраните <color=red>угрозу<color=#708090> в комплексе и <color=#120a8f>выполните миссию<color=#708090>!");

                        player.AddAmmo(AmmoType.Ammo44Cal, 16);
                        player.AddAmmo(AmmoType.Ammo12Gauge, 28);
                        peopleCounter += 1;
                    });
                }
                else if (randomRole <= 0.75f)
                {
                    player.Role.Set(RoleTypeId.ChaosRifleman, RoleSpawnFlags.AssignInventory);
                    Timing.CallDelayed(2f, () =>
                    {
                        SpecialUnitManager.Apply(
    player,
    VeryUsualDay.Instance.Config.BuoSergeantInventory,
    VeryUsualDay.Instance.Config.BuoEffects["Сержант"],
    425f,
    $"БУО #{VeryUsualDay.Instance.BuoCounter} - ##-{peopleCounter}",
    "(Сержант БУО)",
    "<color=#708090><b>Вы стали сержантом <color=#138808>Боевого Ударного Отряда<color=#708090>. Спасите <color=#ffd800>сотрудников фонда<color=#708090>, устраните <color=red>угрозу<color=#708090> в комплексе и <color=#120a8f>выполните миссию<color=#708090>!");

                        player.AddAmmo(AmmoType.Ammo44Cal, 16);
                        player.AddAmmo(AmmoType.Nato556, 100);
                        peopleCounter += 1;
                    });
                }
                else if (randomRole <= 0.90f)
                {
                    player.Role.Set(RoleTypeId.ChaosMarauder, RoleSpawnFlags.AssignInventory);
                    Timing.CallDelayed(2f, () =>
                    {
                        InventoryLimitsManager.ApplyBuo(player);

                        SpecialUnitManager.Apply(
    player,
    VeryUsualDay.Instance.Config.BuoJaggerInventory,
    VeryUsualDay.Instance.Config.BuoEffects["Джаггернаут"],
    475f,
    $"БУО #{VeryUsualDay.Instance.BuoCounter} - ##-{peopleCounter}",
    "(Джаггернаут БУО)",
    "<color=#708090><b>Вы стали джаггернаутом <color=#138808>Боевого Ударного Отряда<color=#708090>. Спасите <color=#ffd800>сотрудников фонда<color=#708090>, устраните <color=red>угрозу<color=#708090> в комплексе и <color=#120a8f>выполните миссию<color=#708090>!");

                        player.AddAmmo(AmmoType.Ammo44Cal, 16);
                        player.AddAmmo(AmmoType.Nato762, 200);
                        peopleCounter += 1;
                    });
                }
                else
                {
                    player.Role.Set(RoleTypeId.ChaosRepressor, RoleSpawnFlags.AssignInventory);
                    Timing.CallDelayed(2f, () =>
                    {
                        InventoryLimitsManager.ApplyBuo(player);

                        SpecialUnitManager.Apply(
    player,
    VeryUsualDay.Instance.Config.BuoTerminatorInventory,
    VeryUsualDay.Instance.Config.BuoEffects["Ликвидатор"],
    525f,
    $"БУО #{VeryUsualDay.Instance.BuoCounter} - ##-{peopleCounter}",
    "(Ликвидатор БУО)",
    "<color=#708090><b>Вы стали ликвидатором <color=#138808>Боевого Ударного Отряда<color=#708090>. Спасите <color=#ffd800>сотрудников фонда<color=#708090>, устраните <color=red>угрозу<color=#708090> в комплексе и <color=#120a8f>выполните миссию<color=#708090>!");

                        player.AddAmmo(AmmoType.Nato556, 120);
                        peopleCounter += 1;
                    });
                }
            }
            response = "Бойцы БУО заспавнены!";
            return true;
        }
    }
}

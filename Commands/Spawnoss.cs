using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using UnityEngine;
using VeryUsualDay.Utils;
using Random = UnityEngine.Random;

namespace VeryUsualDay.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Spawnoss : ICommand
    {
        public string Command => "spawnoss";

        public string[] Aliases => new string[] { };

        public string Description =>
            "Позволяет заспавнить бойцов ОСС. Использование: spawnoss <id через пробел>. Для FX.";

        public bool Execute(
            ArraySegment<string> arguments,
            ICommandSender sender,
            out string response)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound)
            {
                response = "Режим FX не включён";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Формат команды: spawnoss <id через пробел>";
                return false;
            }

            int squadNumber = ++VeryUsualDay.Instance.OssCounter;
            int peopleCounter = 1;
            int spawnedPlayers = 0;

            foreach (string argument in arguments.ToArray())
            {
                if (!int.TryParse(argument, out int playerId) ||
                    !Player.TryGet(playerId, out Player player))
                {
                    continue;
                }

                int memberNumber = peopleCounter++;
                int randomRole = Random.Range(1, 101);
                Config config = VeryUsualDay.Instance.Config;

                if (randomRole <= 45)
                {
                    SpawnMember(
                        player,
                        RoleTypeId.NtfPrivate,
                        config.OssPrivateInventory,
                        config.OssEffects["Боец"],
                        1300f,
                        new Vector3(1.1f, 1.1f, 1.1f),
                        squadNumber,
                        memberNumber,
                        "Боец",
                        "бойцом",
                        true,
                        AmmoType.Nato556,
                        AmmoType.Nato762,
                        AmmoType.Ammo44Cal);
                }
                else if (randomRole <= 70)
                {
                    SpawnMember(
                        player,
                        RoleTypeId.NtfSergeant,
                        config.OssSergeantInventory,
                        config.OssEffects["Сержант"],
                        1600f,
                        new Vector3(1.1f, 1.1f, 1.1f),
                        squadNumber,
                        memberNumber,
                        "Сержант",
                        "сержантом",
                        true,
                        AmmoType.Nato556,
                        AmmoType.Nato762);
                }
                else if (randomRole <= 85)
                {
                    SpawnMember(
                        player,
                        RoleTypeId.NtfSergeant,
                        config.OssJaggerInventory,
                        config.OssEffects["Джаггернаут"],
                        2200f,
                        new Vector3(1.1f, 1.1f, 1.1f),
                        squadNumber,
                        memberNumber,
                        "Джаггернаут",
                        "джаггернаутом",
                        true,
                        AmmoType.Nato762);
                }
                else if (randomRole <= 95)
                {
                    SpawnMember(
                        player,
                        RoleTypeId.NtfSpecialist,
                        config.OssTerminatorInventory,
                        config.OssEffects["Ликвидатор"],
                        1700f,
                        new Vector3(1.1f, 1.1f, 1.1f),
                        squadNumber,
                        memberNumber,
                        "Ликвидатор",
                        "ликвидатором",
                        true,
                        AmmoType.Nato556,
                        AmmoType.Nato762);
                }
                else
                {
                    SpawnMember(
                        player,
                        RoleTypeId.NtfCaptain,
                        config.OssCaptainInventory,
                        config.OssEffects["Капитан"],
                        1900f,
                        new Vector3(1.1f, 1.2f, 1.1f),
                        squadNumber,
                        memberNumber,
                        "Капитан",
                        "капитаном",
                        true,
                        AmmoType.Nato556,
                        AmmoType.Nato762);
                }

                spawnedPlayers++;
            }

            if (spawnedPlayers == 0)
            {
                response = "Не найдено ни одного игрока с указанными ID.";
                return false;
            }

            response = $"Бойцы ОСС заспавнены: {spawnedPlayers}.";
            return true;
        }

        private static void SpawnMember(
            Player player,
            RoleTypeId baseRole,
            List<ItemType> inventory,
            Dictionary<EffectType, byte> effects,
            float health,
            Vector3 scale,
            int squadNumber,
            int memberNumber,
            string rankName,
            string rankInstrumental,
            bool allowThreeFirearms,
            params AmmoType[] requiredAmmo)
        {
            player.Role.Set(
                baseRole,
                RoleSpawnFlags.AssignInventory);

            Timing.CallDelayed(2f, () =>
            {
                SpecialUnitManager.Apply(
                    player,
                    inventory,
                    effects,
                    health,
                    $"ОСС #{squadNumber} - ##-{memberNumber}",
                    $"({rankName} ОСС)",
                    $"<color=#708090><b>Вы стали {rankInstrumental} " +
                    "<color=#138808>ОСС<color=#708090>. Спасите " +
                    "<color=#ffd800>сотрудников фонда<color=#708090>, устраните " +
                    "<color=red>угрозу<color=#708090> в комплексе и " +
                    "<color=#120a8f>выполните миссию<color=#708090>!",
                    allowThreeFirearms);

                player.Scale = scale;

                InventoryLimitsManager.FillAmmoToLimit(
                    player,
                    requiredAmmo);
            });
        }
    }
}
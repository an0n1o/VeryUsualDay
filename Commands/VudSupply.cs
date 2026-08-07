using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using UnityEngine;

namespace VeryUsualDay.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class VudSupply : ICommand
    {
        public string Command => "vudsupply";
        public string[] Aliases => new string[] { };
        public string Description => "Поставка припасов на FXе.";
        private Vector3 _coords;
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound)
            {
                response = "Режим FX не включён";
                return false;
            }
            
            if (arguments.Count < 1)
            {
                response =
                    "Формат команды - vudsupply <med/emf/scp/food/security> <type (for scp)> <amount (for scp/food)>";
                return false;
            }
            var args = arguments.ToArray();
            
            switch (args[0])
            {
                case "med":
                    for (var i = 0; i < 15; i++)
                    {
                        Pickup.CreateAndSpawn(ItemType.Adrenaline, VeryUsualDay.Instance.SupplyBoxCoords,
                            new Quaternion());
                    }
                    for (var i = 0; i < 20; i++)
                    {
                        Pickup.CreateAndSpawn(ItemType.Painkillers, VeryUsualDay.Instance.SupplyBoxCoords,
                            new Quaternion());
                    }
                    Exiled.API.Features.Cassie.MessageTranslated(
                        message: "pitch_0.4 .G1 . . .G1 .G1 pitch_1.00 . . . . . . . . . . . . . .",
                        translation: "<b><color=#EE7600>[Заказ прибыл]</color>: медикаменты </color></b>",
                        isNoisy: false, isSubtitles: true, isHeld: false);
                    break;
                case "scp":
                    if (arguments.Count < 3)
                    {
                        response = "Формат команды - vudsupply scp <type> <amount>";
                        return false;
                    }
                    switch (args[1])
                    {
                        case "500":
                            for (var i = 0; i < int.Parse(args[2]); i++)
                            {
                                Pickup.CreateAndSpawn(ItemType.SCP500, VeryUsualDay.Instance.SupplyBoxCoords, new Quaternion());
                            }
                            break;
                        case "1853":
                            for (var i = 0; i < int.Parse(args[2]); i++)
                            {
                                Pickup.CreateAndSpawn(ItemType.SCP1853, VeryUsualDay.Instance.SupplyBoxCoords, new Quaternion());
                            }
                            break;
                        case "207":
                            for (var i = 0; i < int.Parse(args[2]); i++)
                            {
                                Pickup.CreateAndSpawn(ItemType.SCP207, VeryUsualDay.Instance.SupplyBoxCoords, new Quaternion());
                            }
                            break;
                        default:
                            response = "Формат команды - vudsupply scp <type> <amount>";
                            return false;
                    }
                    break;
                case "food":
                    if (arguments.Count < 2)
                    {
                        response = "Формат команды - vudsupply food <amount>";
                        return false;
                    }
                    for (var i = 0; i < int.Parse(args[1]); i++)
                    {
                        Pickup.CreateAndSpawn(ItemType.Medkit, VeryUsualDay.Instance.SupplyBoxCoords, new Quaternion());
                    }
                    Exiled.API.Features.Cassie.MessageTranslated(
                       message: "pitch_0.4 .G1 . . .G1 .G1 pitch_1.00 . . . . . . . . . . . . . .",
                       translation: "<b><color=#EE7600>[Заказ прибыл]</color>: питание </color></b>",
                       isNoisy: false, isSubtitles: true, isHeld: false);
                    break;
                case "security":
                    _coords = Room.Get(RoomType.LczArmory).Position;
                    _coords.y += 2f;
                    for (var i = 0; i < 3; i++)
                    {
                        Pickup.CreateAndSpawn(ItemType.Radio, _coords, new Quaternion());
                    }
                    for (var i = 0; i < 2; i++)
                    {
                        Pickup.CreateAndSpawn(ItemType.GunCOM15, _coords, new Quaternion());
                    }
                    for (var i = 0; i < 2; i++)
                    {
                        Pickup.CreateAndSpawn(ItemType.ArmorLight, _coords, new Quaternion());
                    }
                    for (var i = 0; i < 10; i++)
                    {
                        Pickup.CreateAndSpawn(ItemType.Ammo9x19, _coords, new Quaternion());
                    }
                    Exiled.API.Features.Cassie.MessageTranslated(
                        message: "pitch_0.4 .G1",
                        translation: "<b><color=#727472>[СБ]</color>: запасы были пополнены.",
                        isNoisy: false, isSubtitles: true, isHeld: false);
                    break;
                default:
                    response = "Формат команды - vudsupply <med/emf/scp/food/security> <type (for scp)> <amount (for scp/food)>";
                    return false;
            }

            response = "Поставка произведена успешно.";
            return true;
        }
    }
}
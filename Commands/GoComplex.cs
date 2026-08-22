using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using UnityEngine;

namespace VeryUsualDay.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class GoComplex : ICommand
    {
        public string Command => "gocomplex";
        public string[] Aliases => new [] { "gocm" };
        public string Description => "Для FX. Отправляет людей на поверхность и кидает CASSIE.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound)
            {
                response = "Режим КМ не включён";
                return false;
            }
            if (arguments.Count < 1)
            {
                response = "Формат команды: gocomplex <id через пробел>.";
                return false;
            }
            foreach (var id in arguments.ToArray())
            {
                if (!Player.TryGet(int.Parse(id), out var player))
                {
                    response = $"Человека с ID {id} нету на сервере.";
                    return false;
                }
                var pos = VeryUsualDay.Instance.SpawnPosition;
                pos.x -= 2f;
                pos.y += 1f;
                player.Teleport(pos);
            }
            Exiled.API.Features.Cassie.MessageTranslated(message: ".G1 . . . . . $PITCH_0.2 .G5 . . . . .", translation: "<color=#eb8f34>[</color><color=#808080>📷</color><color=#eb8f34>] </color><color=#eb8f34>Система <size=0><size=25>Контроля Входа</color>\r\n\r\n<b>|<color=#008080>🚨</color><color=#BC8F8F>👤</color><b>|</color></b></b> Сотрудники прибыли в комплекс.", isNoisy: false, isSubtitles: true, isHeld: false);
            response = "Персонал заспавнен";
            return true;
        }
    }
}
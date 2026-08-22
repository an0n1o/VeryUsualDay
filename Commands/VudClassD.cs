using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using UnityEngine;

namespace VeryUsualDay.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class VudClassD : ICommand
    {
        public string Command => "vudclassd";
        public string[] Aliases => new string[] { };
        public string Description => "Спавнит испытуемого на FX.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound)
            {
                response = "Режим FX не включён!";
                return false;
            }
            if (arguments.Count < 1)
            {
                response = "Формат команды: vudclassd <id через пробел>.";
                return false;
            }
            var cnt = 0;
            foreach (var id in arguments.ToArray())
            {
                if (Player.TryGet(id, out var dboy))
                {
                    var classd = new Utils.ClassD(dboy);
                    cnt += 1;
                }
                else
                {
                    response = "Не удалось найти игрока с таким ID!";
                    return false;
                }
            }
                Exiled.API.Features.Cassie.MessageTranslated(message: ".G1 . $PITCH_0.2 . . . $PITCH_1.0 .G1", translation: $"<color=#eb8f34>[</color><color=#808080>📷</color><color=#eb8f34>] </color><color=#eb8f34>Система <size=0><size=25>Контроля Входа</color>\r\n\r\n<b>|<color=#FF4500>🚨</color><color=#FF4500>👤</color><b>|</color></b></b> Персонал <color=#FF4500>Расходного класса</color> прибыл в учреждение <b>|<color=#FF4500>👤<b><color=#BDB76B>{cnt}</color></color><b>| Количество субъектов выведено на пейджер</b> |<color=#FF4500>🚚</color>| Место выгрузки: <u>Блок Испытуемых</u>", isNoisy: false, isSubtitles: true, isHeld: false);
            response = "Испытуемый заспавнен успешно!";
            return true;
        }
    }
}

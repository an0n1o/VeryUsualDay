using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;

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
            if (cnt == 1)
            {
                Exiled.API.Features.Cassie.MessageTranslated(message: ".G1 . . . . . $PITCH_0.65 .G5 .G5 . . .", translation: "<b><color=#FF0090>O5</color> >>> <color=#008080>Комплекс</color></b>: один испытуемый прибыл в блок-D.", isNoisy: false, isSubtitles: true, isHeld: false);
                response = "Испытуемый заспавнен успешно!";
            }
            else
            {
                Exiled.API.Features.Cassie.MessageTranslated(message: ".G1 . . . . . $PITCH_0.65 .G5 .G5 . . .", translation: $"<b><color=#FF0090>O5</color> >>> <color=#008080>Комплекс</color></b>: <b><color=#50C878>{cnt}</b></color> испытуемых прибыло в блок-D.", isNoisy: false, isSubtitles: true, isHeld: false);
                response = "Испытуемые заспавнены успешно!";
            }
            return true;
        }
    }
}

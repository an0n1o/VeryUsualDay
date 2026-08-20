using System;
using CommandSystem;

namespace VeryUsualDay.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class GateCommand : ICommand
    {
        public string Command => "gateguard";

        public string[] Aliases => new string[] { "gg" };

        public string Description =>
            "Активирует и деактивирует протокол Стражник Врат " +
            "(Автоматически деактивируется при объявлении ЗК).";

        public bool Execute(
            ArraySegment<string> arguments,
            ICommandSender sender,
            out string response)
        {
            if (VeryUsualDay.Instance == null ||
                !VeryUsualDay.Instance.IsEnabledInRound)
            {
                response = "Режим FX не включён.";
                return false;
            }

            if (!VeryUsualDay.Instance.IsGateGuardEnabled)
            {
                VeryUsualDay.Instance.EnableGateGuard();

                global::Exiled.API.Features.Cassie.MessageTranslated(
                    message: "$PITCH_0.2 .G2 .G3 $PITCH_0.2 $SLEEP_8 .G3",
                    translation:
                        @"[<color=#eb8f34>🔊</color>] <b><color=#eb8f34>А.С.К.К.</color></b>

<b><color=#26b00b>|🚪🚪А🔫|</color></b> Активирован протокол <color=#369929>""Стражник Врат""</color>. Ворота А могут быть открыты с предъявлением допуска БУО. <b><color=#7a7a7a>|📟</color><color=#aba532>🏢</color><color=#7a7a7a>| Инициатор: </color></b><b><color=#aba532>Штаб FSC-144-03</color></b>",
                    isHeld: false,
                    isNoisy: false,
                    isSubtitles: true);

                response = "Протокол Стражник Врат активирован.";
            }
            else
            {
                VeryUsualDay.Instance.DisableGateGuard();

                response = "Протокол Стражник Врат деактивирован.";
            }

            return true;
        }
    }
}

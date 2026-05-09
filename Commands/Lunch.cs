using System;
using CommandSystem;
using Exiled.API.Features;
using MEC;

namespace VeryUsualDay.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Lunch : ICommand
    {
        public string Command => "lunch";
        public string[] Aliases => new string[] { };
        public string Description => "Начинает или принудительно заканчивает обед. Сделано для FX.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (VeryUsualDay.Instance.IsLunchtimeActive)
            {
                Exiled.API.Features.Cassie.Message("<b><color=#EE7600>Перерыв окончен!</color></b> <size=0> pitch_0.4 .G3 pitch_1.0 . . . .", isNoisy: false, isSubtitles: true);
                VeryUsualDay.Instance.IsLunchtimeActive = false;
                response = "Обед отменён досрочно!";
                return true;
            }

            VeryUsualDay.Instance.IsLunchtimeActive = true;
            Exiled.API.Features.Cassie.Message("<b><color=#EE7600>[Обеденный перерыв]: пять минут.</color></b> <size=0> pitch_0.4 .G1 . . .G1 .G1", isNoisy: false, isSubtitles: true);
            Timing.CallDelayed(300f, () =>
            {
                if (!VeryUsualDay.Instance.IsLunchtimeActive) return;
                Exiled.API.Features.Cassie.Message("<b><color=#EE7600>Перерыв окончен!</color></b> <size=0> pitch_0.4 .G3 pitch_1.0 . . . .", isNoisy: false, isSubtitles: true);
                VeryUsualDay.Instance.IsLunchtimeActive = false;
            });
            response = "Обед объявлен!";
            return true;
        }
    }
}

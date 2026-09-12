using System;
using CommandSystem;
using Exiled.API.Features;

namespace VeryUsualDay.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class CheckCodePlayer : ICommand
    {
        public string Command => "code";
        public string[] Aliases => new string[] { };
        public string Description => "Показывает текущий код FX.";

        public bool Execute(
        ArraySegment<string> arguments,
        ICommandSender sender,
        out string response)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound)
            {
                response = "Режим FX не включён!";
                return false;
            }

            string lunchStatus = "";

            if ((int)VeryUsualDay.Instance.CurrentCode < 2)
            {
                lunchStatus = VeryUsualDay.Instance.IsLunchtimeActive
                    ? " Статус обеда: активен."
                    : " Статус обеда: неактивен.";
            }

            string teslaStatus = VeryUsualDay.Instance.IsTeslaEnabled
                ? " Тесла-ворота: включены."
                : " Тесла-ворота: выключены.";

            response =
                $"Текущий код - {VeryUsualDay.Instance.CurrentCode.Description()}." +
                lunchStatus +
                teslaStatus;

            Player player = Player.Get(sender);

            if (player != null)
            {
                string codeColor =
                    GetCodeColor(VeryUsualDay.Instance.CurrentCode);

                string coloredLunchStatus = "";

                if ((int)VeryUsualDay.Instance.CurrentCode < 2)
                {
                    coloredLunchStatus =
                        VeryUsualDay.Instance.IsLunchtimeActive
                            ? " Статус обеда: <color=#55FF55>активен</color>."
                            : " Статус обеда: <color=#888888>неактивен</color>.";
                }

                string coloredTeslaStatus =
                    VeryUsualDay.Instance.IsTeslaEnabled
                        ? " Тесла-ворота: <color=#00E5FF>включены</color>."
                        : " Тесла-ворота: <color=#55FF55>выключены</color>.";

                string broadcast =
                    $"<size=30>Текущий код - <color={codeColor}>{VeryUsualDay.Instance.CurrentCode.Description()}</color>." +
                    coloredLunchStatus +
                    coloredTeslaStatus +
                    "</size>";

                player.Broadcast(5, broadcast);
            }

            return true;
        }

        private static string GetCodeColor(VeryUsualDay.Codes code)
        {
            switch (code)
            {
                case VeryUsualDay.Codes.Green:
                    return "#55FF55";

                case VeryUsualDay.Codes.Emerald:
                    return "#00C78C";

                case VeryUsualDay.Codes.Blue:
                    return "#4DA6FF";

                case VeryUsualDay.Codes.Orange:
                    return "#FF9900";

                case VeryUsualDay.Codes.Yellow:
                    return "#FFD700";

                case VeryUsualDay.Codes.Red:
                    return "#FF4444";

                case VeryUsualDay.Codes.Clean:
                    return "#555555";

                default:
                    return "#FFFFFF";
            }
        }
    }
}

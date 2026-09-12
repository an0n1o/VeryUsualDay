using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using VeryUsualDay.Utils;

namespace VeryUsualDay.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Spawn682Event : ICommand
    {
        public string Command => "spawn682event";

        public string[] Aliases => new string[] { };

        public string Description =>
            "Работает при FX. Спавнит ивентового SCP-682.";

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

            var args = arguments.ToArray();

            if (args.Length < 1 ||
                !int.TryParse(args[0], out int id))
            {
                response = "Использование: spawn682event [ID игрока]";
                return false;
            }

            if (!Player.TryGet(id, out var scp682event))
            {
                response = "Не удалось найти игрока с таким ID!";
                return false;
            }

            if (VeryUsualDay.Instance.ScpPlayers.ContainsKey(id))
            {
                var human = new TutorialHuman(scp682event);
                response = "SCP удалён!";
                return true;
            }

            var scp = new Scp682Event(scp682event);

            response = "Ивентовый SCP-682 создан!";
            return true;
        }
    }
}
using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using VeryUsualDay.Utils;

namespace VeryUsualDay.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Spawn01923 : ICommand
    {
        public string Command => "spawn019-2-3";
        public string[] Aliases => new string[] { };
        public string Description => "Работает при FX. Спавнит SCP-019-2 (3 вариант).";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound)
            {
                response = "Режим FX не включён!";
                return false;
            }

            foreach (var id in arguments.ToArray())
            {
                if (!Player.TryGet(int.Parse(id), out var player)) continue;
                var scp = new Scp01923(player);
            }

            response = "Игроки заспавнены.";
            return true;
        }
    }
}

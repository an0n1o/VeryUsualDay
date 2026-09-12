using System;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using VeryUsualDay.Utils;

namespace VeryUsualDay.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class GetItem : ICommand
    {
        public string Command => "getitem";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Попытаться найти случайные предметы в текущем помещении.";

        public bool Execute(
            ArraySegment<string> arguments,
            ICommandSender sender,
            out string response)
        {
            CommandSender commandSender = sender as CommandSender;

            if (commandSender == null)
            {
                response = "Команда может использоваться только игроком.";
                return false;
            }

            Player player = Player.Get(commandSender.SenderId);

            if (player == null)
            {
                response = "Не удалось определить игрока.";
                return false;
            }

            return ClassDRoomRandomItemsManager.TryGiveRandomItems(
                player,
                out response);
        }
    }
}
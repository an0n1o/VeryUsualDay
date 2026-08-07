using System;
using CommandSystem;
using Exiled.API.Features;

namespace VeryUsualDay.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SupplyCassie : ICommand
    {
        public string Command => "supplycassie";
        public string[] Aliases => new string[] { };
        public string Description => "Вызывает CASSIE о доставке SCP. Только для FX.";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound)
            {
                response = "Режим FX не включён";
                return false;
            }
            Exiled.API.Features.Cassie.MessageTranslated(message: "$PITCH_0.4 .G1 . . .G1 .G1 $PITCH_1.00 . . . . . . . . . . . . . .", translation: "<b><color=#EE7600>[Заказ прибыл]</color>: аномальные объекты </color></b>", isNoisy: false, isSubtitles: true, isHeld: false);
            response = "CASSIE успешно вызвано.";
            return true;
        }
    }
}
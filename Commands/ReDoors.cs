using System;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;

namespace VeryUsualDay.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ReDoors : ICommand
    {
        public string Command => "redoors";
        public string[] Aliases => new string[] { };
        public string Description => "Позволяет рестартнуть систему дверей (FX).";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound)
            {
                response = "Режим FX не включён!";
                return false;
            }
            foreach (var door in Door.List)
            {
                if (!door.IsElevator && door.Type != DoorType.SurfaceGate)
                {
                    door.IsOpen = false;
                }
            }
            Exiled.API.Features.Cassie.MessageTranslated(message: "pitch_0.6 .g1 pitch_1.0 . . . . . . . . . . . . .", translation: "<b><color=#727472>[Обновление]</b></color> система дверей была перезапущена.", isNoisy: false, isSubtitles: true, isHeld: false);
            response = "Система дверей перезапущена!";
            return true;
        }
    }
}

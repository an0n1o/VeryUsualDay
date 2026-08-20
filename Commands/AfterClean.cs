using System;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;

namespace VeryUsualDay.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AfterClean : ICommand
    {
        public string Command => "afterclean";
        public string[] Aliases => new string[] { "aclean" };
        public string Description => "Команда для разблокировки дверей и включения света после Кода Очистки. (оставляет некоторые двери заблокированными). Работает только на Foundation-X.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound)
            {
                response = "Режим FX не включён";
                return false;
            }
            if (VeryUsualDay.Instance.CurrentCode != VeryUsualDay.Codes.Clean)
            {
                response = "Код Очистки не активен.";
                return false;
            }

            foreach (var door in Door.List)
            {
                if (door.Type != DoorType.Scp096 &&
                    door.Type != DoorType.Scp173Gate &&
                    door.Type != DoorType.SurfaceGate &&
                    door.Type != DoorType.HIDChamber &&
                    door.Type != DoorType.NukeSurface &&
                    door.Type != DoorType.GateB &&
                    door.Type != DoorType.Scp914Gate)
                {
                    door.Unlock();
                }
            }

            Map.TurnOnAllLights(new[]
            {
                ZoneType.LightContainment,
                ZoneType.HeavyContainment,
                ZoneType.Entrance,
                ZoneType.Surface

             });

            VeryUsualDay.Instance.IsCleanCountdownActive = false;

            response = "Комплекс готов к штатному режиму.";
            return true;
        }
    }
}

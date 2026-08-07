using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using MEC;
using PlayerRoles;

namespace VeryUsualDay.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Recontain008 : ICommand
    {
        public string Command => "recontain008";
        public string[] Aliases => new string[] { };
        public string Description => "Восстанавливает ОУС SCP-008 от имени игрока. Исп: recontain008 <ID>";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!VeryUsualDay.Instance.Is008Leaked)
            {
                response = "ОУС SCP-008 не нарушены!";
                return false;
            }
            var playerSender = Player.Get(arguments.ToArray()[0]);
            if (playerSender.CurrentRoom.Type != RoomType.Hcz106)
            {
                response = "Вы не находитесь в К.С. SCP-008.";
                return false;
            }
            if (Door.Get(DoorType.Scp106Primary).IsLocked)
            {
                response = "Для ВОУС необходимо, чтобы дверь в К.С. SCP-008 была разблокирована.";
                return false;
            }
            if (playerSender.Role.Team != Team.FoundationForces && playerSender.Role.Team != Team.ChaosInsurgency)
            {
                response = "Восстановить ОУС SCP-008 может только человек с ролью Охраны/МОГ/ПХ (рп-отыгровка: вам не хватило силы/знаний)";
                return false;
            }
            if (Math.Abs(Room.Get(RoomType.Hcz106).Position.z - playerSender.Position.z) < 19 && Math.Abs(Room.Get(RoomType.Hcz106).Position.x - playerSender.Position.x) < 18)
            {
                response = "Вы не находитесь в К.С. SCP-008.";
                return false;
            }
            Timing.CallDelayed(1f, () =>
            {
                Exiled.API.Features.Cassie.MessageTranslated(message: "pitch_0.1 .G2 . pitch_1.0 . . . . . .", translation: "<b><color=#727472>[ВОУС]</color></b>: Объект-008 был перекрыт, распространение патогена прекращено.", isSubtitles: true, isNoisy: false, isHeld: false);
            });
            Timing.KillCoroutines("_008_poisoning");
            VeryUsualDay.Instance.Is008Leaked = false;
            response = "ВОУС SCP-008 прошло успешно.";
            return true;
        }
    }
}
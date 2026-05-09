using System;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using MEC;

namespace VeryUsualDay.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Patogen008 : ICommand
    {
        public string Command => "patogen008";
        public string[] Aliases => new string[] { };
        public string Description => "Вызывает НОУС SCP-008. FX.";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound)
            {
                response = "Режим СОД не включён!";
                return false;
            }
            if (VeryUsualDay.Instance.Is008Leaked)
            {
                Timing.KillCoroutines("_008_poisoning");
                Door.Get(DoorType.Scp106Primary).Unlock();
                VeryUsualDay.Instance.Is008Leaked = false;
                Exiled.API.Features.Cassie.DelayedMessage("<b><color=#727472>[ВОУС]</color></b>: Объект-008 был перекрыт, распространение патогена прекращено. <size=0> pitch_0.1 .G2 . pitch_1.0 . . . . . . ", 1f, isSubtitles: true, isNoisy: false);
                response = "Распространение SCP-008 прекращено.";
            }
            else
            {
                Timing.RunCoroutine(VeryUsualDay.Instance._008_poisoning(), "_008_poisoning");
                Door.Get(DoorType.Scp106Primary).Lock(float.PositiveInfinity, DoorLockType.AdminCommand);
                VeryUsualDay.Instance.Is008Leaked = true;
                Exiled.API.Features.Cassie.DelayedMessage("<b><color=#C50000>[ВНИМАНИЕ]</color></b> В зонах содержания зафиксировано распространение аномальной инфекции. Заражение перешло в активную стадию. Всем боевым единицам ликвидировать аномалии <size=0> pitch_0.2 .G1 .G1 . .G6 .", 1f, isSubtitles: true, isNoisy: false);
                response = "Распространение SCP-008 начато.";
            }
            return true;
        }
    }
}
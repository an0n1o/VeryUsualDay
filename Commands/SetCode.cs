using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using MEC;
using PlayerRoles;
namespace VeryUsualDay.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SetCode : ICommand
    {
        public string Command => "setcode";
        public string[] Aliases => new [] { "code" };
        public string Description => "Установить код в комплексе. Используется для FX.";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound)
            {
                response = "Режим FX не включён!";
                return false;
            }
            if (VeryUsualDay.Instance.IsCleanCountdownActive)
            {
                response = "Невозможно изменить рабочий режим без подготовки к штатному режиму.";
                return false;
            }
            if (arguments.Count != 1)
            {
                response = "Формат команды: setcode <название>. Допустимые названия: green, emerald, blue, orange, yellow, red, clean.";
                return false;
            }
            switch (arguments.ToArray()[0])
            {
                case "green":
                    VeryUsualDay.Instance.CurrentCode = VeryUsualDay.Codes.Green;
                    Exiled.API.Features.Cassie.MessageTranslated(message: "$PITCH_0.1 .G2 . $PITCH_1.0 . . . . . . . . . . . . . .", translation: "<b><color=#727472>[Рабочий режим]</color></b>: объявлен <color=#32CD32>Зелёный Код</color>. Сотрудникам работать в штатном режиме.", isSubtitles: true, isNoisy: false, isHeld: false);
                    foreach (var ragdoll in Ragdoll.List.ToList())
                    {
                        ragdoll.Destroy();
                    }
                    response = "Установлен код \"Зелёный\"!";
                    return true;
                case "emerald":
                    VeryUsualDay.Instance.CurrentCode = VeryUsualDay.Codes.Emerald;
                    Exiled.API.Features.Cassie.MessageTranslated(message: "$PITCH_0.35 .G3 .G3 .G1 .G2 . $PITCH_1.0 . . . . . . . . . . . . . .", translation: "<b><color=#727472>[Рабочий режим]</color></b>: объявлен <color=#50C878>Изумрудный Код</color>. Замечены сбои в системе. Возможны поломки или нарушения в зонах содержания. Службе Безопасности быть на готове.\r\n", isSubtitles: true, isNoisy: false, isHeld: false);
                    foreach (var ragdoll in Ragdoll.List.ToList())
                    {
                        ragdoll.Destroy();
                    }
                    response = "Установлен код \"Изумрудный\"!";
                    return true;
                case "blue":
                    VeryUsualDay.Instance.CurrentCode = VeryUsualDay.Codes.Blue;
                    Exiled.API.Features.Cassie.MessageTranslated(message: "$PITCH_0.1 .G1 .G2 . $PITCH_1.0 . . . . . . . . . . . . . .", translation: "<b><color=#727472>[Рабочий режим]</color></b>: объявлен <color=#005EBC>Синий Код</color>. Зафиксированы малые нарушения. Персоналу следует принимать меры предосторожности.", isSubtitles: true, isNoisy: false, isHeld: false);
                    response = "Установлен код \"Синий\"!";
                    return true;
                case "orange":
                    VeryUsualDay.Instance.CurrentCode = VeryUsualDay.Codes.Orange;
                    Exiled.API.Features.Cassie.MessageTranslated(message: "$PITCH_0.15 .G6 $PITCH_0.08 .G1 .G3 . $PITCH_1.0 . . . . . . . . . . . . . .", translation: "<b><color=#727472>[Рабочий режим]</color></b>: объявлен <color=#EE7600>Оранжевый Код</color>. В комплексе зафиксированы нарушения, превышающие слабый уровень опасности. Всем боевым единицам приступить к ликвидации угрозы или принять меры для восстановления безопасной обстановки. <b><color=#002DB3>ГОР</color></b> Разрешено войти в подземную часть.", isSubtitles: true, isNoisy: false, isHeld: false);
                    response = "Установлен код \"Оранжевый\"!";
                    return true;
                case "yellow":
                    VeryUsualDay.Instance.CurrentCode = VeryUsualDay.Codes.Yellow;
                    Exiled.API.Features.Cassie.MessageTranslated(message: "$PITCH_0.1 .G3 .G1 . $PITCH_1.0 . . . . . . . . . . . . . .", translation: "<b><color=#727472>[Рабочий режим]</color></b>: объявлен <color=#EFC01A>Жёлтый Код</color>. Возможно включение <b><color=#FD8272>Тесла-Ворот</b></color>. Службе безопасности приступить к ликвидации угрозы или принять меры для восстановления безопасной обстановки. <b><color=#002DB3>ГОР</color></b> Разрешено войти в подземную часть.", isSubtitles: true, isNoisy: false, isHeld: false);
                    response = "Установлен код \"Жёлтый\"!";
                    return true;
                case "red":
                    VeryUsualDay.Instance.CurrentCode = VeryUsualDay.Codes.Red;
                    Exiled.API.Features.Cassie.MessageTranslated(message: "$PITCH_0.1 .G5 . .G5 . .G5 . .G1 . $PITCH_1.0 . . . . . . . . . . . . . .", translation: "<b><color=#727472>[Рабочий режим]</color></b>: объявлен <color=#C50000>Красный Код</color>. Всем мирным сотрудникам пройти на поверхность до устранения основных угроз. Всем боевым единицам принять действия устранения опасности.", isSubtitles: true, isNoisy: false, isHeld: false);
                    response = "Установлен код \"Красный\"!";
                    return true;
                case "clean":
                    VeryUsualDay.Instance.CurrentCode = VeryUsualDay.Codes.Clean;
                    VeryUsualDay.Instance.IsCleanCountdownActive = true;
                    Exiled.API.Features.Cassie.MessageTranslated(message: "$PITCH_0.25 .G1 $PITCH_1.0 . . . $PITCH_0.25 .G1 $PITCH_1.0 . . . $PITCH_0.25 .G1 $PITCH_1.0 . . . $PITCH_0.20 .G1 $PITCH_0.15 .G1 $PITCH_0.10 .G3 $PITCH_0.05 . $PITCH_0.03 .G6   $PITCH_1.0 . . . . . . . . . . . . . .", translation: "<b><color=#727472>[Рабочий режим]</color></b>: объявлен <color=#00FFFF>Код Очистки</color>. Всему персоналу забраться в доступные укрытия (<color=#EFC01A>ГР18/Камера079</color>) во избежание смертельного отравления газом. Минимальное время для исполнения - <color=#EE7600>5 минут</color>. Пробравшиеся в помещения аномалии погибнут. Нахождение на поверхности не спасёт.", isSubtitles: true, isNoisy: false, isHeld: false);
                    response = "Установлен код \"Очистка\"!";
                    Timing.CallDelayed(300f, () =>
                    {

                           foreach (var door in Door.List)
                            {
                                door.IsOpen = false;
                                door.Lock(DoorLockType.AdminCommand);
                            }

                        Map.TurnOffAllLights(float.MaxValue);

                        foreach (var player in Player.List)
                        {
                            if (player.Role.Type == RoleTypeId.Tutorial &&
                                player.CustomInfo == "Человек")
                                continue;

                            if (player.CurrentRoom == null)
                                continue;

                            if (player.CurrentRoom.Type == RoomType.Hcz079 ||
                                player.CurrentRoom.Type == RoomType.LczGlassBox)
                                continue;

                            player.EnableEffect(EffectType.Decontaminating);
                        }

                        Exiled.API.Features.Cassie.MessageTranslated(message: "$PITCH_0.01 .G6 .", translation: "<b><color=#960018>[ЛОКДАУН]</b></color>", isSubtitles: true, isNoisy: false, isHeld: false);
                    });
                    return true;
                default:
                    response = "Формат команды: setcode <название>. Допустимые названия: green, emerald, blue, yellow, red, clean.";
                    return false;
            }
        }
    }
}
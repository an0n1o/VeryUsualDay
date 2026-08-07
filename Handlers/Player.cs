using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Jailbird;
using MEC;
using PlayerRoles;
using UnityEngine;
using VeryUsualDay.Abilities.Scp035;
using VeryUsualDay.Utils;

namespace VeryUsualDay.Handlers
{
    public static class Player
    {
        public static void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (VeryUsualDay.Instance.IsTeslaEnabled) return;
            ev.IsTriggerable = false;
            ev.IsAllowed = false;
        }

        public static void OnHandcuffing(HandcuffingEventArgs ev)
        {
            if (VeryUsualDay.Instance.IsEnabledInRound) return;
            ev.IsAllowed = false;
            ev.Target.Handcuff();
        }
        
        public static void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound) return;
            ev.Player.SessionVariables.Remove("vudmood");
            if (ev.NewRole != RoleTypeId.Spectator && ev.NewRole.GetSide() != Side.Scp && (ev.NewRole != RoleTypeId.Tutorial || ev.Player.CustomName.Split(' ')[0] == "Агент"))
            {
                ev.Player.SessionVariables.Add("vudmood", "Полностью здоров");
            }
            Timing.CallDelayed(5f, () =>
            {
                if (ev.Player.TryGetSessionVariable("isInPrison", out bool prisonState) && prisonState) return;
                if (ev.NewRole != RoleTypeId.Spectator ||
                    ev.Reason == SpawnReason.ForceClass) return;

                if (
                    VeryUsualDay.Instance.ScpPlayers.ContainsKey(ev.Player.Id) &&
                    VeryUsualDay.Instance.ScpPlayers[ev.Player.Id] == VeryUsualDay.Scps.Scp035 &&
                    ev.Player.TryGetSessionVariable("haveBody", out bool haveBody) && haveBody)
                {
                    ev.Player.SessionVariables["haveBody"] = false;
                    ev.Player.Role.Set(RoleTypeId.Scp0492, reason: SpawnReason.ForceClass, spawnFlags: RoleSpawnFlags.AssignInventory);
                    Timing.CallDelayed(1f, () =>
                    {
                        ev.Player.MaxHealth = 6000f;
                        ev.Player.Health = (float)ev.Player.SessionVariables["previousHp"];
                        ev.Player.Scale = new Vector3(1f, 0.1f, 1f);
                        ev.Player.EnableEffect(EffectType.DamageReduction);
                        ev.Player.ChangeEffectIntensity(EffectType.DamageReduction, 40);
                        ev.Player.EnableEffect(EffectType.Slowness);
                        ev.Player.ChangeEffectIntensity(EffectType.Slowness, 70);
                        ev.Player.EnableEffect(EffectType.SilentWalk);
                        ev.Player.ChangeEffectIntensity(EffectType.SilentWalk, 10);
                        ev.Player.IsGodModeEnabled = false;
                        ev.Player.CustomName = "Объект";
                    });
                    return;
                }
                if (VeryUsualDay.Instance.Is008Leaked)
                {
                    if (ev.Player.Role.Type == RoleTypeId.Scp0492)
                    {
                        return;
                    }
                }

                if (ev.Reason == SpawnReason.Died || ev.Reason == SpawnReason.Destroyed)
                {
                    var bc =
                        "<b>Вы были перемещены в <color=purple>Обучение</color>, т.к. ивент находится в процессе.</b>";
                    ev.Player.Role.Set(RoleTypeId.Tutorial, reason: SpawnReason.ForceClass);
                    ev.Player.Broadcast(10, bc);
                }
                else
                {
                    var bc =
                        "<b>Добро пожаловать на происходящее <color=#02EDAA>RP-событие</color> \"<color=#ED3502>Слишком Обычный День в Зоне-03</color>\"!\n<color=yellow>(Для чтения инструкции - нажмите на \"Ё\")<b></color> \n\n------------------------------[📁ГЛАВНОЕ 📁]-----------------------------\n1.  Если вы находитесь в лобби (башня) в роли обучения - вы ожидаете роль\". Ведущий ивента к вам рано или поздно обратиться для назначения на роль.\n2. Вы можете ознакомиться с правилами сервера в разделе \"Server Info\" (на клавишу \"Ё\")\n3. .help - список команд сервера \n3. Иногда может быть доступна команда .classd, с которой вы можете занять очередь на прибытие как Класс-D без помощи ведущего\n4. Вы можете проверить текущий статус комплекса командой .code. Обычно в комплекс в качестве сотрудника можно прибыть только в Зелёный Код. Если вы видите другой - возможно в зоне объявлена тревога \n\n-----------------------------[📁 ОПИСАНИЕ 📁]-----------------------------\nВы находитесь на ивенте с ролевой отыгровкой, в котором проходит рабочий день в одной из зон Фонда SCP. Данная зона именуется как \"Зона-03\". Здесь действует свой лор и может отличать от вселенных SCP Foundation и SCP Secret Laboratory.\n\nПрибудьте в комплекс, как Учёный, Охранник, Рабочий или Класс-D и действуйте по установленному на территории зоны порядку. \n\n------------------------------[📖БАЗОВЫЕ ПРАВИЛА📖]------------------------------\nОсновной список правил можно найти в Server Info, но вот вам основные:\n\n1. [RP Атмосфера] Требования к соблюдению рп не очень высокие, но не должны разрушать атмосферу самой игры. Если вы хотите исполнить что-то особенное - обратитесь к ведущему с этим вопросом \n\n2. [RandomDeathMatch] Запрещено убийство персонала вне специально одобренных ведущим ситуаций или если вы не являетесь вражеской для фонда ролью.\n\n3. [Здравый Смысл] Иногда игроку достаточно сказать \"не будь плохим человеком\". Но следует знать, что вам просто следует разводить негативный опыт вне ролевой игры или пытаться препятствовать рп игре.";
                    ev.Player.Role.Set(RoleTypeId.Tutorial, reason: SpawnReason.ForceClass);
                    ev.Player.Broadcast(10, bc);
                }
            });
        }

        public static void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound) return;
            if (VeryUsualDay.Instance.ScpPlayers.ContainsKey(ev.Player.Id))
            {
                if (VeryUsualDay.Instance.ScpPlayers[ev.Player.Id] == VeryUsualDay.Scps.Scp0762)
                {
                    ev.IsAllowed = false;
                }

                if (VeryUsualDay.Instance.ScpPlayers[ev.Player.Id] == VeryUsualDay.Scps.Scp035Old &&
                    VeryUsualDay.Instance.Config.Scp035ForbiddenItems.Contains(ev.Pickup.Type))
                {
                    ev.IsAllowed = false;
                }
            }

            switch (ev.Player.Role.Type)
            {
                case RoleTypeId.Scientist
                    when VeryUsualDay.Instance.Config.ForbiddenForScientists.Contains(ev.Pickup.Type):
                case RoleTypeId.FacilityGuard
                    when VeryUsualDay.Instance.Config.ForbiddenForSecurity.Contains(ev.Pickup.Type):
                case RoleTypeId.ClassD when ev.Player.CustomName.ToLower().Contains("рабочий") &&
                                            VeryUsualDay.Instance.Config.ForbiddenForWorkers.Contains(ev.Pickup.Type):
                case RoleTypeId.ClassD when !ev.Player.CustomName.ToLower().Contains("рабочий") &&
                                            VeryUsualDay.Instance.Config.ForbiddenForClassD.Contains(ev.Pickup.Type):
                case RoleTypeId.Tutorial when ev.Player.CustomName.ToLower().Contains("агент") &&
                                              VeryUsualDay.Instance.Config.ForbiddenForAgency.Contains(ev.Pickup.Type):
                    ev.IsAllowed = false;
                    break;
            }

            if ((VeryUsualDay.Instance.CurrentCode != VeryUsualDay.Codes.Green &&
                 VeryUsualDay.Instance.CurrentCode != VeryUsualDay.Codes.Emerald) ||
                ev.Player.Role.Type != RoleTypeId.ClassD || ev.Player.CustomName.ToLower().Contains("рабочий") ||
                !ev.Pickup.Type.IsWeapon() || VeryUsualDay.Instance.ScpPlayers.ContainsKey(ev.Player.Id)) return;
            VeryUsualDay.Instance.CurrentCode = VeryUsualDay.Codes.Blue;
            Exiled.API.Features.Cassie.MessageTranslated(
               message: "pitch_0.1 .G1 .G2 . pitch_1.0 . . . . . . . . . . . . . .",
               translation: "<b><color=#727472>[Рабочий режим]</color></b>: объявлен <color=#005EBC>Синий Код</color>. Зафиксированы малые нарушения. Персоналу следует принимать меры предосторожности.",
               isNoisy: false, isSubtitles: true, isHeld: false);
        }

        public static void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound) return;
            if (!VeryUsualDay.Instance.ScpPlayers.ContainsKey(ev.Player.Id)) return;
            if (VeryUsualDay.Instance.ScpPlayers[ev.Player.Id] == VeryUsualDay.Scps.Scp0762)
            {
                ev.IsAllowed = false;
            }

            if (VeryUsualDay.Instance.ScpPlayers[ev.Player.Id] == VeryUsualDay.Scps.Scp035Old &&
                ev.Item.Type == ItemType.GunRevolver)
            {
                ev.IsAllowed = false;
            }
        }

        public static void OnHurting(HurtingEventArgs ev)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound) return;
            try
            {
                if (!VeryUsualDay.Instance.ScpPlayers.TryGetValue(ev.Attacker.Id, out var avel) ||
                    avel != VeryUsualDay.Scps.Scp0762) return; // checks if attacker is not avel
                if (ev.Player != null && Random.Range(0, 100) < 40) ev.Attacker.Heal(25f);
                if (ev.Attacker.CurrentItem.As<Jailbird>()?.WearState != JailbirdWearState.AlmostBroken) return;
                ev.Attacker.CurrentItem?.Destroy();
                var jailbird = ev.Attacker.AddItem(ItemType.Jailbird);
                Timing.CallDelayed(0.1f, () => { ev.Attacker.CurrentItem = jailbird; });
            }
            catch
            {
                // ignored, just stfu about how bad it is
            }
        }

        public static void OnDying(DyingEventArgs ev)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound) return;
            if (VeryUsualDay.Instance.CurrentCode == VeryUsualDay.Codes.Green ||
                VeryUsualDay.Instance.CurrentCode == VeryUsualDay.Codes.Emerald)
            {
                ev.Player.ClearInventory(destroy: true);
            }
        }
        
        public static void OnDied(DiedEventArgs ev)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound) return;
            if (ev.Player.TryGetSessionVariable("haveBody", out bool haveBody) && haveBody) return;
            ev.Player.CustomInfo = "Человек";
            ev.Player.MaxHealth = 100f;
            ev.Player.Scale = new Vector3(1f, 1f, 1f);
            ev.Player.SessionVariables.Remove("vudmood");
            
            if (VeryUsualDay.Instance.Zombies.Contains(ev.Player.Id))
            {
                ev.Player.UnMute();
                VeryUsualDay.Instance.Zombies.Remove(ev.Player.Id);
            }
            else if (!VeryUsualDay.Instance.ScpPlayers.ContainsKey(ev.Player.Id) && VeryUsualDay.Instance.Is008Leaked)
            {
                Timing.CallDelayed(2f, () =>
                {
                    var scp = new Scp0082(ev.Player, true);
                });
            }
            if (VeryUsualDay.Instance.ScpPlayers.ContainsKey(ev.Player.Id))
            {
                if (VeryUsualDay.Instance.ScpPlayers[ev.Player.Id] == VeryUsualDay.Scps.Scp0082) ev.Player.UnMute();
                VeryUsualDay.Instance.ScpPlayers.Remove(ev.Player.Id);
            }
            
            if (VeryUsualDay.Instance.DBoysQueue.Contains(ev.Player.Id))
            {
                VeryUsualDay.Instance.DBoysQueue.Remove(ev.Player.Id);
            }
            if (VeryUsualDay.Instance.JoinedDboys.Contains(ev.Player.Id))
            {
                VeryUsualDay.Instance.JoinedDboys.Remove(ev.Player.Id);
            }
            if (VeryUsualDay.Instance.Shakheds.Contains(ev.Player.Id))
            {
                VeryUsualDay.Instance.Shakheds.Remove(ev.Player.Id);
            }
            if (VeryUsualDay.Instance.CurrentCode == VeryUsualDay.Codes.Green ||
                VeryUsualDay.Instance.CurrentCode == VeryUsualDay.Codes.Emerald)
            {
                Timing.CallDelayed(5f, () => { Ragdoll.GetLast(ev.Player).Destroy(); });
            }
        }

        public static void OnLeft(LeftEventArgs ev)
        {
            if (ev.Player.TryGetSessionVariable("serverSettings", out List<SettingBase> settings))
            {
                SettingBase.Unregister(ev.Player, settings);
            }
            if (ev.Player.TryGetSessionVariable("isInPrison", out bool prisonState) && prisonState)
            {
                ev.Player.TryGetSessionVariable("prisonReason", out string reason);
                ev.Player.TryGetSessionVariable("prisonTime", out int time);
                PrisonController.SendToPrison(ev.Player, time, reason);
            }
            if (VeryUsualDay.Instance.Zombies.Contains(ev.Player.Id))
            {
                ev.Player.UnMute();
                VeryUsualDay.Instance.Zombies.Remove(ev.Player.Id);
            }
            if (!VeryUsualDay.Instance.IsEnabledInRound) return;
            if (VeryUsualDay.Instance.ScpPlayers.ContainsKey(ev.Player.Id))
            {
                VeryUsualDay.Instance.ScpPlayers.Remove(ev.Player.Id);
            }
            if (VeryUsualDay.Instance.DBoysQueue.Contains(ev.Player.Id))
            {
                VeryUsualDay.Instance.DBoysQueue.Remove(ev.Player.Id);
            }
            if (VeryUsualDay.Instance.JoinedDboys.Contains(ev.Player.Id))
            {
                VeryUsualDay.Instance.JoinedDboys.Remove(ev.Player.Id);
            }
            if (VeryUsualDay.Instance.Shakheds.Contains(ev.Player.Id))
            {
                VeryUsualDay.Instance.Shakheds.Remove(ev.Player.Id);
            }
        }

        public static void OnShooting(ShootingEventArgs ev)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound) return;
            if (!VeryUsualDay.Instance.ScpPlayers.TryGetValue(ev.Player.Id, out var player)) return;
            if (player == VeryUsualDay.Scps.Scp035Old && ev.Firearm.Type == ItemType.GunRevolver)
            {
                ev.Firearm.MagazineAmmo += 1;
            }
        }

        public static void OnUsingItem(UsingItemEventArgs ev)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound) return;
            if (!VeryUsualDay.Instance.ScpPlayers.TryGetValue(ev.Player.Id, out var player)) return;
            if ((player != VeryUsualDay.Scps.Scp035Old || ev.Usable.Type != ItemType.SCP500) &&
                ev.Usable.Type != ItemType.SCP207 && ev.Usable.Type != ItemType.AntiSCP207) return;
            ev.IsAllowed = false;
            ev.Item?.Destroy();
        }

        public static void OnVerified(VerifiedEventArgs ev)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound) return;
            if (VeryUsualDay.Instance.Config.AuthToken != "")
            {
                var userData = (ITuple)PrisonController.CheckIfPlayerInPrison(ev.Player);
                if ((bool)userData[0])
                {
                    ev.Player.SessionVariables.Add("isInPrison", true);
                    ev.Player.SessionVariables.Add("prisonTime", (int)userData[1]);
                    ev.Player.SessionVariables.Add("prisonReason", (string)userData[2]);
                    ev.Player.Role.Set(RoleTypeId.Tutorial);
                    Timing.CallDelayed(1f, () =>
                    {
                        ev.Player.Mute();
                        ev.Player.EnableEffect(EffectType.SilentWalk, 255);
                        ev.Player.Teleport(VeryUsualDay.PrisonPosition);
                    });
                }
            }

            var settings = new List<SettingBase>
            {
                VeryUsualDay.SettingsHeader,
                new MemeticsAbility().Setting,
                new BodyTakeoverAbility().Setting
            };
            ev.Player.SessionVariables["serverSettings"] = settings;
            SettingBase.Register(ev.Player, settings);
        }

        public static void OnHurt(HurtEventArgs ev) 
        {
            if (VeryUsualDay.Instance.ScpPlayers.ContainsKey(ev.Player.Id) &&
                VeryUsualDay.Instance.ScpPlayers[ev.Player.Id] == VeryUsualDay.Scps.Scp035 &&
                (!ev.Player.TryGetSessionVariable("haveBody", out bool haveBody) || !haveBody))
            {
                if (ev.Player.Health <= 2000f)
                {
                    ev.Player.IsGodModeEnabled = true;
                    Exiled.API.Features.Cassie.MessageTranslated(message: "pitch_0.1 .G2 . pitch_1.0 . . . . . . . . . . . . . .", translation: "<b><color=#727472>[ВОУС]</color></b>: Объект-035 ослаб и доступен для транспортировки в камеру содержания", isNoisy: false, isSubtitles: true, isHeld: false);
                    ev.Player.Broadcast(5, "<b>Вы ослабли и не можете навредить людям!</b>");
                }
                return;
            }
            if (VeryUsualDay.Instance.Shakheds.Contains(ev.Player.Id) && VeryUsualDay.Instance.Config.BlowingDamageTypes.Contains(ev.DamageHandler.Type))
            {
                VeryUsualDay.Instance.Shakheds.Remove(ev.Player.Id);
                while (!ev.Player.IsDead)
                {
                    ev.Player.Explode(ProjectileType.FragGrenade);
                }
            }
            if (!VeryUsualDay.Instance.IsEnabledInRound ||
                !ev.Player.TryGetSessionVariable("vudmood", out string _)) return;
            if (ev.Player.Health <= ev.Player.MaxHealth * 0.1)
            {
                ev.Player.SessionVariables.Remove("vudmood");
                ev.Player.SessionVariables.Add("vudmood", "Ужасно сильно ранен");
            }
            if (ev.Player.Health <= ev.Player.MaxHealth * 0.3)
            {
                ev.Player.SessionVariables.Remove("vudmood");
                ev.Player.SessionVariables.Add("vudmood", "Сильно ранен");
            }
            else if (ev.Player.Health <= ev.Player.MaxHealth * 0.5)
            {
                ev.Player.SessionVariables.Remove("vudmood");
                ev.Player.SessionVariables.Add("vudmood", "Ранен");
            }
            else
            {
                ev.Player.SessionVariables.Remove("vudmood");
                ev.Player.SessionVariables.Add("vudmood", "Полностью здоров");
            }
        }

        public static void OnHealed(HealedEventArgs ev)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound ||
                !ev.Player.TryGetSessionVariable("vudmood", out string _)) return;
            if (ev.Player.Health <= ev.Player.MaxHealth * 0.1)
            {
                ev.Player.SessionVariables.Remove("vudmood");
                ev.Player.SessionVariables.Add("vudmood", "Ужасно сильно ранен");
            }
            if (ev.Player.Health <= ev.Player.MaxHealth * 0.3)
            {
                ev.Player.SessionVariables.Remove("vudmood");
                ev.Player.SessionVariables.Add("vudmood", "Сильно ранен");
            }
            else if (ev.Player.Health <= ev.Player.MaxHealth * 0.5)
            {
                ev.Player.SessionVariables.Remove("vudmood");
                ev.Player.SessionVariables.Add("vudmood", "Ранен");
            }
            else
            {
                ev.Player.SessionVariables.Remove("vudmood");
                ev.Player.SessionVariables.Add("vudmood", "Полностью здоров");
            }
        }

        public static void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound) return;
            if (ev.Player.Role.Type == RoleTypeId.Scp0492 && ev.Door.IsOpen)
            {
                ev.IsAllowed = false;
            }
        }

        public static void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound) return;
            if (ev.Player.TryGetSessionVariable("isUnderMemetics", out bool isUnderMemetics) && isUnderMemetics &&
                ev.Item.IsFirearm)
            {
                ev.IsAllowed = false;
            }
        }
    }
}
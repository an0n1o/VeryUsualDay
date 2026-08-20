using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.API.Features.Items.Keycards;
using Exiled.API.Features.Pickups;
using Interactables.Interobjects.DoorUtils;
using Exiled.API.Features.Doors;
using MEC;
using Newtonsoft.Json;
using PlayerRoles;
using UnityEngine;
using Player = VeryUsualDay.Handlers.Player;
using PlayerHandler = Exiled.Events.Handlers.Player;
using Random = UnityEngine.Random;
using Server = VeryUsualDay.Handlers.Server;
using ServerHandler = Exiled.Events.Handlers.Server;

namespace VeryUsualDay
{
    public class VeryUsualDay : Plugin<Config>
    {
        public static VeryUsualDay Instance { get; private set; }

        public override string Author => "JustMarfix";
        public override string Name => "VeryUsualDay (FX Version)";

        public override Version Version => new Version(5, 3, 1);

        public bool IsEnabledInRound { get; set; }
        public bool IsLunchtimeActive { get; set; }
        public bool IsDboysSpawnAllowed { get; set; }
        public bool IsTeslaEnabled { get; set; }
        public bool Is008Leaked { get; set; }
        public bool IsGateGuardEnabled { get; set; }
        public bool GateBusy { get; set; }
        public bool IsCleanCountdownActive { get; set; } = false;
        public List<int> JoinedDboys { get; set; } = new List<int>();
        public List<int> DBoysQueue { get; set; } = new List<int>();
        public List<int> Shakheds { get; set; } = new List<int>();
        public List<int> Zombies { get; set; } = new List<int>();
        public List<RoomType> ChaosRooms { get; set; } = new List<RoomType>();
        public int BuoCounter { get; set; } = 1;
        public int SpawnedDboysCounter { get; set; } = 1;
        public int SpawnedWorkersCounter { get; set; } = 1;
        public int SpawnedScientistCounter { get; set; } = 1;
        public int SpawnedSecurityCounter { get; set; } = 1;
        
        private readonly Vector3 _armedPersonnelTowerCoords = new Vector3(-16f, 315f, -32f);
        private readonly Vector3 _civilianPersonnelTowerCoords = new Vector3(44.4f, 315f, -51.6f);
        public readonly Vector3 SpawnPosition = new Vector3(139.487f, 296.7f, -16.762f);
        public static readonly Vector3 PrisonPosition = new Vector3(130.233f, 293.766f, 21.049f);
        public Vector3 SupplyBoxCoords = new Vector3();
        public Vector3 VaseCoords = new Vector3();

        public static readonly HeaderSetting SettingsHeader = new HeaderSetting(100, "Foundation-X");
        
        public Pickup Vase;

        public readonly List<Vector3> EmfSupplyCoords = new List<Vector3>
        {
            new Vector3(-10.682f, -96.5f, -32.115f),
            new Vector3(-6.873f, -96.5f, -32f)
        };
        
        public enum Codes
        {
            [Description("Зелёный")]
            Green,
            [Description("Изумрудный")]
            Emerald,
            [Description("Синий")]
            Blue,
            [Description("Оранжевый")]
            Orange,
            [Description("Жёлтый")]
            Yellow,
            [Description("Красный")]
            Red,
            [Description("Очистка")]
            Clean
        }

        public enum Scps
        {
            Scp0082,
            Scp01921,
            Scp01922,
            Scp01923,
            Scp035,
            Scp035Old,
            Scp0352,
            Scp049,
            Scp0762,
            Scp372,
            Scp682,
            Scp966,
            Scp999,
        }

        public Codes CurrentCode { get; set; } = Codes.Green;
        public Dictionary<int, Scps> ScpPlayers { get; set; } = new Dictionary<int, Scps>();

        private static void _registerEvents()
        {
            PlayerHandler.ChangingRole += Player.OnChangingRole;
            PlayerHandler.PickingUpItem += Player.OnPickingUpItem;
            PlayerHandler.DroppingItem += Player.OnDroppingItem;
            PlayerHandler.Hurting += Player.OnHurting;
            PlayerHandler.Dying += Player.OnDying;
            PlayerHandler.Died += Player.OnDied;
            PlayerHandler.Left += Player.OnLeft;
            PlayerHandler.Shooting += Player.OnShooting;
            PlayerHandler.UsingItem += Player.OnUsingItem;
            PlayerHandler.TriggeringTesla += Player.OnTriggeringTesla;
            PlayerHandler.Verified += Player.OnVerified;
            PlayerHandler.Hurt += Player.OnHurt;
            PlayerHandler.Healed += Player.OnHealed;
            PlayerHandler.Handcuffing += Player.OnHandcuffing;
            PlayerHandler.InteractingDoor += Player.OnInteractingDoor;
            PlayerHandler.ChangingItem += Player.OnChangingItem;
            PlayerHandler.ReceivingEffect += Player.OnReceivingEffect;
            PlayerHandler.SentValidCommand += Player.OnSentValidCommand;

            ServerHandler.WaitingForPlayers += Server.OnWaitingForPlayers;
            ServerHandler.RoundStarted += Server.OnRoundStarted;

            // Exiled.Events.Handlers.Scp049.Attacking += Handlers.Scp049.OnAttacking;
        }

        private static void _unregisterEvents()
        {
            PlayerHandler.ChangingRole -= Player.OnChangingRole;
            PlayerHandler.PickingUpItem -= Player.OnPickingUpItem;
            PlayerHandler.DroppingItem -= Player.OnDroppingItem;
            PlayerHandler.Hurting -= Player.OnHurting;
            PlayerHandler.Dying -= Player.OnDying;
            PlayerHandler.Died -= Player.OnDied;
            PlayerHandler.Left -= Player.OnLeft;
            PlayerHandler.Shooting -= Player.OnShooting;
            PlayerHandler.UsingItem -= Player.OnUsingItem;
            PlayerHandler.TriggeringTesla -= Player.OnTriggeringTesla;
            PlayerHandler.Verified -= Player.OnVerified;
            PlayerHandler.Hurt -= Player.OnHurt;
            PlayerHandler.Healed -= Player.OnHealed;
            PlayerHandler.Handcuffing -= Player.OnHandcuffing;
            PlayerHandler.InteractingDoor -= Player.OnInteractingDoor;
            PlayerHandler.ChangingItem -= Player.OnChangingItem;
            PlayerHandler.ReceivingEffect -= Player.OnReceivingEffect;
            PlayerHandler.SentValidCommand += Player.OnSentValidCommand;

            ServerHandler.WaitingForPlayers -= Server.OnWaitingForPlayers;
            ServerHandler.RoundStarted -= Server.OnRoundStarted;

            // Exiled.Events.Handlers.Scp049.Attacking -= Handlers.Scp049.OnAttacking;
        }

        public override void OnEnabled()
        {
            Instance = this;
            if (Instance.Config.AuthToken == "")
            {
                Log.Error("AuthToken пуст - функционал тюрьмы и БД будет недоступен.");
            }
            _registerEvents();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;
            _unregisterEvents();
            base.OnDisabled();
        }
        
        public IEnumerator<float> _008_poisoning()
        {
            for (;;)
            {
                foreach (var target in Exiled.API.Features.Player.Get(player => player.Zone == ZoneType.HeavyContainment && !Instance.Config.DoNotPoisonRoles.Contains(player.Role.Type) && player.Role.Team != Team.SCPs && (player.CustomInfo is null || !player.CustomInfo.ToLower().Contains("scp"))))
                {
                    target.EnableEffect(EffectType.Poisoned);
                }
                yield return Timing.WaitForSeconds(10f);
            }
        }

        public IEnumerator<float> _joining()
        {
            for (;;)
            {
                if (CurrentCode == Codes.Green)
                {
                    var counter = 0;
                    foreach (var i in DBoysQueue.ToList())
                    {
                        if (!Exiled.API.Features.Player.TryGet(i, out var dboy)) continue;
                        if ((dboy.CustomInfo == "Человек" || dboy.CustomInfo is null) && dboy.Role.Type == RoleTypeId.Tutorial)
                        {
                            if (JoinedDboys.Count >= 5)
                            {
                                break;
                            }
                            if (counter == 3)
                            {
                                break;
                            }
                            dboy.CustomName = $"Испытуемый - ##-{SpawnedDboysCounter}";
                            JoinedDboys.Add(i);
                            dboy.Role.Set(RoleTypeId.ClassD);
                            Timing.CallDelayed(1f, () =>
                            {
                                dboy.ClearInventory();
                                dboy.Handcuff();
                                dboy.Broadcast(10, "<b>Вы стали <color=#EE7600>Испытуемым</color>! Можете сотрудничать с <color=#120a8f>фондом</color> или принимать попытки <color=#ff2b2b>побега</color> при первой возможности. </b>");
                            });
                            counter += 1;
                            SpawnedDboysCounter += 1;
                        }
                        DBoysQueue.Remove(i);
                    }
                    switch (counter)
                    {
                        case 0:
                            break;
                        case 1:
                            Exiled.API.Features.Cassie.MessageTranslated(message: ". . . . . . . . . . . . . . . . . . . . . .", translation: "<b><color=#FF0090>O5</color> >>> <color=#008080>Комплекс</color></b>: один испытуемый прибыл в блок D.", isNoisy: false, isSubtitles: true, isHeld: false);
                            break;
                        case 2:
                            Exiled.API.Features.Cassie.MessageTranslated(message: ". . . . . . . . . . . . . . . . . . . . . .", translation: "<b><color=#FF0090>O5</color> >>> <color=#008080>Комплекс</color></b>: двое испытуемых прибыло в блок D.", isNoisy: false, isSubtitles: true, isHeld: false);
                            break;
                        case 3:
                            Exiled.API.Features.Cassie.MessageTranslated(message: ". . . . . . . . . . . . . . . . . . . . . .", translation: "<b><color=#FF0090>O5</color> >>> <color=#008080>Комплекс</color></b>: трое испытуемых прибыло в блок D.", isNoisy: false, isSubtitles: true, isHeld: false);
                            break;
                    }
                }
                yield return Timing.WaitForSeconds(300f);
            }
        }
        
        public IEnumerator<float> _prisonTimer()
        {
            for (;;)
            {
                foreach (var player in Exiled.API.Features.Player.List)
                {
                    if (player.TryGetSessionVariable("isInPrison", out bool prisonState) && prisonState)
                    {
                        player.TryGetSessionVariable("prisonTime", out Int32 time);
                        // Log.Info($"Игроку {player.UserId} осталось находиться в тюрьме {time} секунд.");
                        time -= 1;
                        player.SessionVariables.Remove("prisonTime");
                        player.SessionVariables.Add("prisonTime", time);
                        if (time == 0)
                        {
                            PrisonController.SendToPrison(player, 0, "Выпущен из тюрьмы.");
                        }
                    }
                }
                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> _chaos()
        {
            for (;;)
            {
                foreach (var roomType in ChaosRooms)
                {
                    var room = Room.Get(roomType);
                    foreach (var door in room.Doors.Where(p => !p.IsElevator && !p.IsGate && !p.IsKeycardDoor && !p.IsLocked).Shuffle())
                    {
                        Timing.CallDelayed(Random.Range(0f, 1.5f), () =>
                        {
                            door.IsOpen = !door.IsOpen;
                        });
                    }

                    foreach (var player in room.Players)
                    {
                        if (Random.Range(0, 100) < 50)
                        {
                            var score = Random.Range(0, 100);
                            if (score < 25)
                            {
                                player.PlayGunSound(FirearmType.Crossvec, (byte)Random.Range(175, 256));
                            }
                            else if (score < 50)
                            {
                                player.PlayGunSound(FirearmType.FSP9, (byte)Random.Range(175, 256));
                            }
                            else if (score < 75)
                            {
                                player.PlayGunSound(FirearmType.E11SR, (byte)Random.Range(175, 256));
                            }
                            else 
                            {
                                player.PlayGunSound(FirearmType.Shotgun, (byte)Random.Range(175, 256));
                            }
                        }
                        else if (Random.Range(0, 100) < 30)
                        {
                            player.PlayBeepSound();
                        }

                        else if (Random.Range(0, 100) < 30)
                        {
                            player.PlayShieldBreakSound();
                        }
                        else
                        {
                            player.EnableEffect(EffectType.Scanned, duration: 10f);
                        }
                    }
                }
                yield return Timing.WaitForSeconds(0.75f);
            }
        }

        /*
        public IEnumerator<float> _avel()
        {
            for (;;)
            {
                foreach (var pair in ScpPlayers.Where(p => p.Value == Scps.Scp0762))
                {
                    try
                    {
                        if (Exiled.API.Features.Player.TryGet(pair.Key, out var avel) && avel.Health < 5000)
                        {
                            avel.Heal(50f);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
                yield return Timing.WaitForSeconds(7f);
            }
        }*/

        private static async Task<string> HttpGetUser(string steamid)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Instance.Config.AuthToken);
                var content = await client.GetStringAsync($"{Instance.Config.BaseApiUrl}/get_user/{steamid}");
                return content;
            }
        }

        private async void SetUserRole(Exiled.API.Features.Player player)
        {
            var jsonString = await HttpGetUser(player.UserId);
            var json = JsonConvert.DeserializeObject<List<string>>(jsonString);
            if (json.Count == 0)
            {
                return;
            }

            var splitted = json[1].Split(' ');
            player.CustomInfo = new StringBuilder().Append($"\"{splitted[0]}\" ").Append(string.Join(" ", splitted.Skip(1))).ToString(); // add quotes to the name and conc. to the main string
            player.CustomName = json[2];
            switch (json[3])
            {
                case "СБ":
                    player.Role.Set(RoleTypeId.FacilityGuard, RoleSpawnFlags.None);
                    Timing.CallDelayed(2f, () =>
                    {
                        foreach (var item in Instance.Config.SecurityItems[json[4]])
                        {
                            player.AddItem(item);
                        }
                        foreach (var ammo in Instance.Config.SecurityAmmo[json[4]])
                        {
                            player.AddAmmo(ammo, 60);
                        }
                        player.MaxHealth = Instance.Config.SecurityHealth[json[4]];
                        player.Health = Instance.Config.SecurityHealth[json[4]];
                        
                        player.EnableEffect(EffectType.DamageReduction);
                        player.ChangeEffectIntensity(EffectType.DamageReduction, 30);
                        player.EnableEffect(EffectType.BodyshotReduction);
                        player.ChangeEffectIntensity(EffectType.BodyshotReduction, 30);
                        
                        player.Teleport(_armedPersonnelTowerCoords);
                    });
                    break;
                case "НС":
                    player.Role.Set(RoleTypeId.Scientist, RoleSpawnFlags.None);
                    Timing.CallDelayed(2f, () =>
                    {
                        foreach (var item in Instance.Config.ScientificItems[json[4]])
                        {
                            player.AddItem(item);
                        }

                        if (Instance.Config.ScienceHealth.ContainsKey(json[4]))
                        {
                            player.MaxHealth = Instance.Config.ScienceHealth[json[4]];
                            player.Health = Instance.Config.ScienceHealth[json[4]];
                        }
                        else
                        {
                            player.MaxHealth = 175f;
                            player.Health = 175f;
                        }

                        player.EnableEffect(EffectType.DamageReduction);
                        player.ChangeEffectIntensity(EffectType.DamageReduction, 15);
                        player.EnableEffect(EffectType.BodyshotReduction);
                        player.ChangeEffectIntensity(EffectType.BodyshotReduction, 15);
                        
                        player.Teleport(_civilianPersonnelTowerCoords);
                    });
                    break;
                case "Рабочие":
                    player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);
                    Timing.CallDelayed(2f, () =>
                    {
                        foreach (var item in Instance.Config.WorkersItems[json[4]])
                        {
                            player.AddItem(item);
                        }
                        foreach (var pair in Instance.Config.WorkersEffects[json[4]])
                        {
                            player.EnableEffect(pair.Key);
                            player.ChangeEffectIntensity(pair.Key, pair.Value);
                        }
                        player.MaxHealth = Instance.Config.WorkersHealth[json[4]];
                        player.Health = Instance.Config.WorkersHealth[json[4]];
                        player.Teleport(_civilianPersonnelTowerCoords);
                    });
                    break;
                case "ГОР":
                    player.Role.Set(Instance.Config.EmfRoles[json[4]], RoleSpawnFlags.None);
                    Timing.CallDelayed(2f, () =>
                    {
                        foreach (var item in Instance.Config.EmfItems[json[4]])
                        {
                            player.AddItem(item);
                        }
                        foreach (var ammo in Instance.Config.EmfAmmo[json[4]])
                        {
                            player.AddAmmo(ammo, 60);
                        }
                        foreach (var pair in Instance.Config.EmfEffects[json[4]])
                        {
                            player.EnableEffect(pair.Key);
                            player.ChangeEffectIntensity(pair.Key, pair.Value);
                        }
                        player.MaxHealth = Instance.Config.EmfHealth[json[4]];
                        player.Health = Instance.Config.EmfHealth[json[4]];
                        player.Teleport(_armedPersonnelTowerCoords);
                    });
                    break;
                case "ОВБ":
                    player.Role.Set(RoleTypeId.Tutorial, RoleSpawnFlags.None);
                    Timing.CallDelayed(2f, () =>
                    {
                        player.Scale = new Vector3(1.3f, 0.8f, 1f);
                        foreach (var item in Instance.Config.AgencyItems[json[4]])
                        {
                            player.AddItem(item);
                        }
                        foreach (var ammo in Instance.Config.AgencyAmmo[json[4]])
                        {
                            player.AddAmmo(ammo, 60);
                        }
                        foreach (var pair in Instance.Config.OVBEffects[json[4]])
                        {
                            player.EnableEffect(pair.Key);
                            player.ChangeEffectIntensity(pair.Key, pair.Value);
                        }
                        player.MaxHealth = Instance.Config.AgencyHealth;
                        player.Health = Instance.Config.AgencyHealth;
                        player.IsGodModeEnabled = false;
                        player.Teleport(_armedPersonnelTowerCoords);
                    });
                    break;
                case "Административный персонал":
                    player.Role.Set(RoleTypeId.Scientist, RoleSpawnFlags.None);
                    Timing.CallDelayed(2f, () =>
                    {
                        foreach (var item in Instance.Config.AdministrativeItems[json[4]])
                        {
                            player.AddItem(item);
                        }
                        player.MaxHealth = Instance.Config.AdministrativeHealth[json[4]];
                        player.Health = Instance.Config.AdministrativeHealth[json[4]];
                        player.Teleport(_civilianPersonnelTowerCoords);
                    });
                    break;
                case "Испытуемые":
                    player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);
                    Timing.CallDelayed(2f, () =>
                    {
                        player.ClearInventory();
                        player.Handcuff();
                        player.MaxHealth = 130f;
                        player.Health = 130f;
                    });
                    break;
            }

            ColorUtility.TryParseHtmlString(json[9], out var permColor);
            ColorUtility.TryParseHtmlString(json[10], out var cardColor);
            switch (json[5])
            {
                case "Site02":
                    player.AddItem(Site02Keycard.Create(
                        keycardLevels: new KeycardLevels(int.Parse(json[6]), int.Parse(json[7]), int.Parse(json[8])),
                        permissionsColor: permColor,
                        "Ключ_карта",
                        cardColor,
                        player.CustomInfo,
                        json[11],
                        Color.black,
                        1
                    ));
                    break;
                case "Management":
                    player.AddItem(ManagementKeycard.Create(
                        keycardLevels: new KeycardLevels(int.Parse(json[6]), int.Parse(json[7]), int.Parse(json[8])),
                        permissionsColor: permColor,
                        "Ключ_карта",
                        cardColor,
                        player.CustomInfo,
                        Color.black
                    ));
                    break;
                case "TaskForce":
                    player.AddItem(TaskForceKeycard.Create(
                        keycardLevels: new KeycardLevels(int.Parse(json[6]), int.Parse(json[7]), int.Parse(json[8])),
                        permissionsColor: permColor,
                        "Ключ_карта",
                        cardColor,
                        json[11],
                        json[12],
                        1
                    ));
                    break;
                case "MetalCase":
                    player.AddItem(MetalKeycard.Create(
                        keycardLevels: new KeycardLevels(int.Parse(json[6]), int.Parse(json[7]), int.Parse(json[8])),
                        permissionsColor: permColor,
                        "Ключ_карта",
                        cardColor,
                        player.CustomInfo,
                        json[11],
                        Color.black,
                        1,
                        json[12]
                    ));
                    break;
            }

            player.EnableEffect(EffectType.SoundtrackMute);
        }

        public void RoleDistribution()
        {
            foreach (var player in Exiled.API.Features.Player.Get(RoleTypeId.Tutorial))
            {
                if ((!player.TryGetSessionVariable("isInPrison", out bool prisonState) || !prisonState) && (player.CustomInfo == "Человек" || player.CustomInfo is null))
                {
                    SetUserRole(player);
                }
            }
        }
        public void EnableGateGuard()
        {
            IsGateGuardEnabled = true;
            GateBusy = false;

            foreach (var door in Door.List)
            {
                if (door.Type != DoorType.GateA)
                    continue;

                door.Lock(DoorLockType.AdminCommand);
                door.IsOpen = false;
            }
        }

        public void DisableGateGuard()
        {
            IsGateGuardEnabled = false;
            GateBusy = false;

            foreach (var door in Door.List)
            {
                if (door.Type == DoorType.GateA)
                {
                    door.Unlock();
                }
            }
        }
    }
}

using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Interfaces;
using PlayerRoles;

namespace VeryUsualDay
{
    public class Config : IConfig
    {
        [Description("Плагин включён? (bool)")]
        public bool IsEnabled { get; set; } = true;

        [Description("Debug включён? (bool)")]
        public bool Debug { get; set; } = false;

        [Description("Токен для авторизации на API")]
        public string AuthToken { get; set; } = "";

        [Description("BaseUrl для API")]
        public string BaseApiUrl { get; set; } = "http://mail.justmarfix.ru:9000";

        [Description("Список вещей для СБ (Dictionary<string, List<ItemType>>)")]
        public Dictionary<string, List<ItemType>> SecurityItems { get; set; } = new Dictionary<string, List<ItemType>>
        {
            {
                "Стажёр",
                new List<ItemType>
                {
                    ItemType.GunCOM18,
                    ItemType.KeycardJanitor,
                    ItemType.ArmorLight,
                    ItemType.Radio,
                    ItemType.Medkit,
                    ItemType.GrenadeFlash,
                    ItemType.Adrenaline
                }
            },
            {
                "Рядовой",
                new List<ItemType>
                {
                    ItemType.ArmorLight,
                    ItemType.Radio,
                    ItemType.GrenadeFlash,
                    ItemType.Medkit,
                    ItemType.GunFSP9
                }
            },
            {
                "Младший сержант",
                new List<ItemType>
                {
                    ItemType.ArmorCombat,
                    ItemType.Radio,
                    ItemType.GunCrossvec,
                    ItemType.GunRevolver,
                    ItemType.GrenadeFlash,
                    ItemType.Medkit,
                    ItemType.Adrenaline
                }
            },
            {
                "Сержант",
                new List<ItemType>
                {
                    ItemType.ArmorCombat,
                    ItemType.GunCrossvec,
                    ItemType.GunA7,
                    ItemType.Radio,
                    ItemType.Medkit,
                    ItemType.GrenadeFlash,
                    ItemType.Adrenaline
                }
            },
            {
                "Старший сержант",
                new List<ItemType>
                {
                    ItemType.ArmorCombat,
                    ItemType.GunCrossvec,
                    ItemType.GunAK,
                    ItemType.Radio,
                    ItemType.Medkit,
                    ItemType.Adrenaline,
                    ItemType.GrenadeFlash
                }
            },
            {
                "Лейтенант",
                new List<ItemType>
                {
                    ItemType.ArmorHeavy,
                    ItemType.GunE11SR,
                    ItemType.GunCrossvec,
                    ItemType.GrenadeFlash,
                    ItemType.Radio,
                    ItemType.Medkit,
                    ItemType.Adrenaline
                }
            },
            {
                "Старший лейтенант",
                new List<ItemType>
                {
                    ItemType.GunE11SR,
                    ItemType.ArmorHeavy,
                    ItemType.GunAK,
                    ItemType.GrenadeFlash,
                    ItemType.Radio,
                    ItemType.Medkit,
                    ItemType.Adrenaline
                }
            },
            {
                "Глава",
                new List<ItemType>
                {
                    ItemType.GunE11SR,
                    ItemType.ArmorHeavy,
                    ItemType.GunShotgun,
                    ItemType.SCP500,
                    ItemType.Medkit,
                    ItemType.Adrenaline,
                    ItemType.Radio,
                }
            }
        };
        [Description("Список патронов для СБ (выдаётся 60 шт.) (Dictionary<string, List<AmmoType>>)")]
        public Dictionary<string, List<AmmoType>> SecurityAmmo { get; set; } = new Dictionary<string, List<AmmoType>>
        {
            {
                "Стажёр",
                new List<AmmoType>
                {
                    AmmoType.Nato9
                }
            },
            {
                "Рядовой",
                new List<AmmoType>
                {
                    AmmoType.Nato9
                }
            },
            {
                "Младший сержант",
                new List<AmmoType>
                {
                    AmmoType.Nato9
                }
            },
            {
                "Сержант",
                new List<AmmoType>
                {
                    AmmoType.Nato9
                }
            },
            {
                "Старший сержант",
                new List<AmmoType>
                {
                    AmmoType.Nato9,
                    AmmoType.Ammo44Cal
                }
            },
            {
                "Лейтенант",
                new List<AmmoType>
                {
                    AmmoType.Nato556
                }
            },
            {
                "Старший лейтенант",
                new List<AmmoType>
                {
                    AmmoType.Nato556,
                    AmmoType.Nato9
                }
            },
            {
                "Глава",
                new List<AmmoType>
                {
                    AmmoType.Ammo12Gauge,
                    AmmoType.Nato556
                }
            }
        };
        [Description("Максимальное здоровье СБ (Dictionary<string, float>)")]
        public Dictionary<string, float> SecurityHealth { get; set; } = new Dictionary<string, float>
        {
            {
                "Стажёр",
                175f
            },
            {
                "Рядовой",
                200f
            },
            {
                "Младший сержант",
                225f
            },
            {
                "Сержант",
                250f
            },
            {
                "Старший сержант",
                275f
            },
            {
                "Лейтенант",
                300f
            },
            {
                "Старший лейтенант",
                325f
            },
            {
                "Глава",
                450f
            }
        };

        [Description("Список вещей для Научных Сотрудников (Dictionary<string, List<ItemType>>)")]
        public Dictionary<string, List<ItemType>> ScientificItems { get; set; } = new Dictionary<string, List<ItemType>>
        {
            {
                "Стажёр",
                new List<ItemType>
                {
                    ItemType.KeycardJanitor,
                    ItemType.Flashlight,
                    ItemType.Radio
                }
            },
            {
                "Исследователь",
                new List<ItemType>
                {
                    ItemType.KeycardZoneManager,
                    ItemType.Flashlight,
                    ItemType.Adrenaline,
                    ItemType.Radio
                }
            },
            {
                "Медик",
                new List<ItemType>
                {
                    ItemType.KeycardZoneManager,
                    ItemType.Painkillers,
                    ItemType.Adrenaline,
                    ItemType.Radio
                }
            },
            {
                "Инженер",
                new List<ItemType>
                {
                    ItemType.Radio,
                    ItemType.Painkillers,
                    ItemType.Adrenaline,
                    ItemType.Medkit,
                    ItemType.Flashlight
                }
            },
            {
                "Психолог",
                new List<ItemType>
                {
                    ItemType.Adrenaline,
                    ItemType.Adrenaline,
                    ItemType.Painkillers,
                    ItemType.Radio
                }
            },
            {
                "Научный руководитель",
                new List<ItemType>
                {
                    ItemType.Radio,
                    ItemType.Painkillers,
                    ItemType.Adrenaline,
                    ItemType.Medkit
                }
            },
            {
                "Глава",
                new List<ItemType>
                {
                    ItemType.Radio,
                    ItemType.SCP500,
                    ItemType.GunCOM18,
                    ItemType.ArmorCombat,
                    ItemType.Medkit
                }
            }
        };

        public Dictionary<string, float> ScienceHealth { get; set; } = new Dictionary<string, float>
        {
            {
                "Глава",
                200f
            }
        };
        [Description("Список вещей для Рабочих (Dictionary<string, List<ItemType>>)")]
        public Dictionary<string, List<ItemType>> WorkersItems { get; set; } = new Dictionary<string, List<ItemType>>
        {
            {
                "Рабочий",
                new List<ItemType>
                {
                    ItemType.Flashlight,
                    ItemType.Painkillers,
                    ItemType.Medkit,
                    ItemType.Adrenaline
                }
            },
            {
                "Старший рабочий",
                new List<ItemType>
                {
                    ItemType.Flashlight,
                    ItemType.Painkillers,
                    ItemType.Medkit,
                    ItemType.Adrenaline,
                    ItemType.Radio
                }
            }
        };
        
        [Description("Максимальное здоровье рабочих (Dictionary<string, float>)")]
        public Dictionary<string, float> WorkersHealth { get; set; } = new Dictionary<string, float>
        {
            {
                "Рабочий",
                175f
            },
            {
                "Старший рабочий",
                175f
            }
        };

        [Description("Список эффектов для Рабочих (Dictionary<string, Dictionary<EffectType, byte>>)")]
        public Dictionary<string, Dictionary<EffectType, byte>> WorkersEffects { get; set; } =
            new Dictionary<string, Dictionary<EffectType, byte>>
            {
                {
                    "Рабочий",
                    new Dictionary<EffectType, byte>
                    {
                        {
                            EffectType.MovementBoost,
                            5
                        },
                        {
                            EffectType.BodyshotReduction,
                            15
                        },
                        {
                            EffectType.DamageReduction,
                            15
                        }
                    }
                },
                {
                    "Старший рабочий",
                    new Dictionary<EffectType, byte>
                    {
                        {
                            EffectType.MovementBoost,
                            5
                        },
                        {
                            EffectType.BodyshotReduction,
                            15
                        },
                        {
                            EffectType.DamageReduction,
                            15
                        }
                    }
                },
            };
        
        [Description("Список ролей для ГОР (Dictionary<string, RoleTypeId>)")]
        public Dictionary<string, RoleTypeId> EmfRoles { get; set; } = new Dictionary<string, RoleTypeId>
        {
            {
                "Боец",
                RoleTypeId.NtfPrivate
            },
            {
                "Джаггернаут",
                RoleTypeId.NtfPrivate
            },
            {
                "Лейтенант",
                RoleTypeId.NtfSergeant
            },
            {
                "Экзобоец",
                RoleTypeId.NtfSpecialist
            },
            {
                "Глава",
                RoleTypeId.NtfCaptain
            }
        };
        [Description("Список вещей для ГОР (Dictionary<string, List<ItemType>>)")]
        public Dictionary<string, List<ItemType>> EmfItems { get; set; } = new Dictionary<string, List<ItemType>>
        {
            {
                "Боец",
                new List<ItemType>
                {
                    ItemType.ArmorCombat,
                    ItemType.GunE11SR,
                    ItemType.Medkit,
                    ItemType.Radio,
                    ItemType.Adrenaline,
                    ItemType.Painkillers
                }
            },
            {
                "Джаггернаут",
                new List<ItemType>
                {
                    ItemType.ArmorHeavy,
                    ItemType.GunLogicer,
                    ItemType.Medkit,
                    ItemType.Medkit,
                    ItemType.Radio,
                    ItemType.Adrenaline
                }
            },
            {
                "Лейтенант",
                new List<ItemType>
                {
                    ItemType.ArmorHeavy,
                    ItemType.GunCrossvec,
                    ItemType.GunE11SR,
                    ItemType.Medkit,
                    ItemType.Radio,
                    ItemType.Adrenaline
                }
            },
            {
                "Экзобоец",
                new List<ItemType>
                {
                    ItemType.ArmorHeavy,
                    ItemType.GunCom45,
                    ItemType.Adrenaline,
                    ItemType.SCP500,
                    ItemType.GunShotgun,
                    ItemType.GunCOM18,
                    ItemType.Radio
                }
            },
            {
                "Глава",
                new List<ItemType>
                {
                    ItemType.GunFRMG0,
                    ItemType.GunShotgun,
                    ItemType.SCP500,
                    ItemType.Medkit,
                    ItemType.Adrenaline,
                    ItemType.Radio,
                    ItemType.ArmorHeavy
                }
            }
        };
        [Description("Максимальное здоровье ГОР (Dictionary<string, float>)")]
        public Dictionary<string, float> EmfHealth { get; set; } = new Dictionary<string, float>
        {
            {
                "Боец",
                450f
            },
            {
                "Джаггернаут",
                600f
            },
            {
                "Лейтенант",
                550f
            },
            {
                "Экзобоец",
                650f
            },
            {
                "Глава",
                700f
            }
        };
        
        [Description("Список патронов для ГОР (выдаётся 60 шт.) (Dictionary<string, List<AmmoType>>)")]
        public Dictionary<string, List<AmmoType>> EmfAmmo { get; set; } = new Dictionary<string, List<AmmoType>>
        {
            {
                "Боец",
                new List<AmmoType>
                {
                    AmmoType.Nato556
                }
            },
            {
                "Джаггернаут",
                new List<AmmoType>
                {
                    AmmoType.Nato762,
                    AmmoType.Nato762
                }
            },
            {
                "Лейтенант",
                new List<AmmoType>
                {
                    AmmoType.Nato556,
                    AmmoType.Nato9
                }
            },
            {
                "Экзобоец",
                new List<AmmoType>
                {
                    AmmoType.Nato9,
                    AmmoType.Nato9,
                    AmmoType.Nato9,
                    AmmoType.Ammo12Gauge
                }
            },
            {
                "Глава",
                new List<AmmoType>
                {
                    AmmoType.Ammo12Gauge,
                    AmmoType.Nato556,
                    AmmoType.Ammo12Gauge,
                    AmmoType.Nato556
                }
            }
        };

        [Description("Список эффектов для ГОР (Dictionary<string, Dictionary<EffectType, byte>>)")]
        public Dictionary<string, Dictionary<EffectType, byte>> EmfEffects { get; set; } =
            new Dictionary<string, Dictionary<EffectType, byte>>
            {
                {
                    "Боец",
                    new Dictionary<EffectType, byte>
                    {
                        {
                            EffectType.BodyshotReduction,
                            40
                        },
                        {
                            EffectType.DamageReduction,
                            40
                        }
                    }
                },
                {
                    "Джаггернаут",
                    new Dictionary<EffectType, byte>
                    {
                        {
                            EffectType.BodyshotReduction,
                            40
                        },
                        {
                            EffectType.DamageReduction,
                            40
                        }
                    }
                },
                {
                    "Лейтенант",
                    new Dictionary<EffectType, byte>()
                },
                {
                    "Экзобоец",
                    new Dictionary<EffectType, byte>
                    {
                        {
                            EffectType.Vitality,
                            1
                        },
                        {
                            EffectType.BodyshotReduction,
                            40
                        },
                        {
                            EffectType.DamageReduction,
                            40
                        },
                        {
                            EffectType.MovementBoost,
                            7
                        }
                    }
                },
                {
                    "Глава",
                    new Dictionary<EffectType, byte>
                    {
                        {
                            EffectType.BodyshotReduction,
                            40
                        },
                        {
                            EffectType.DamageReduction,
                            40
                        },
                        {
                            EffectType.MovementBoost,
                            15
                        }
                    }
                }
            };
        
        [Description("Список вещей для Агентов (Dictionary<string, List<ItemType>>)")]
        public Dictionary<string, List<ItemType>> AgencyItems { get; set; } = new Dictionary<string, List<ItemType>>
        {
            {
                "Младший агент",
                new List<ItemType>
                {
                    ItemType.ArmorCombat,
                    ItemType.GunFSP9,
                    ItemType.GunCOM18,
                    ItemType.Radio,
                    ItemType.Medkit,
                    ItemType.Adrenaline
                }
            },
            {
                "Агент",
                new List<ItemType>
                {
                    ItemType.ArmorCombat,
                    ItemType.Radio,
                    ItemType.GunCrossvec,
                    ItemType.GunCOM18,
                    ItemType.Medkit,
                    ItemType.GrenadeFlash
                }
            },
            {
                "Старший агент",
                new List<ItemType>
                {
                    ItemType.ArmorHeavy,
                    ItemType.GunCrossvec,
                    ItemType.GunE11SR,
                    ItemType.Radio,
                    ItemType.Medkit,
                    ItemType.Painkillers,
                }
            }
        };
        [Description("Здоровье Агентов (float)")]
        public float AgencyHealth { get; set; } = 350f;

        [Description("Список патронов для Агентов (выдаётся 60 шт.) (Dictionary<string, List<AmmoType>>)")]
        public Dictionary<string, List<AmmoType>> AgencyAmmo { get; set; } = new Dictionary<string, List<AmmoType>>
        {
            {
                "Младший агент",
                new List<AmmoType>
                {
                    AmmoType.Nato9,
                    AmmoType.Nato9
                }
            },
            {
                "Агент",
                new List<AmmoType>
                {
                    AmmoType.Nato9,
                    AmmoType.Nato9
                }
            },
            {
                "Старший агент",
                new List<AmmoType>
                {
                    AmmoType.Nato9,
                    AmmoType.Nato556
                }
            }
        };

        [Description("Список эффектов для ОВБ (Dictionary<string, Dictionary<EffectType, byte>>)")]
        public Dictionary<string, Dictionary<EffectType, byte>> OVBEffects { get; set; } =
            new Dictionary<string, Dictionary<EffectType, byte>>
            {
                {
                    "Младший агент",
                    new Dictionary<EffectType, byte>
                    {
                        {
                            EffectType.BodyshotReduction,
                            30
                        },
                        {
                            EffectType.DamageReduction,
                            30
                        }
                    }
                },
                {
                    "Агент",
                    new Dictionary<EffectType, byte>
                    {
                        {
                            EffectType.BodyshotReduction,
                            30
                        },
                        {
                            EffectType.DamageReduction,
                            30
                        }
                    }
                },
                                {
                    "Старший агент",
                    new Dictionary<EffectType, byte>
                    {
                        {
                            EffectType.BodyshotReduction,
                            30
                        },
                        {
                            EffectType.DamageReduction,
                            30
                        }
                    }
                }
            };

        [Description("Список вещей для административного персонала (Dictionary<string, List<ItemType>>)")]
        public Dictionary<string, List<ItemType>> AdministrativeItems { get; set; } = new Dictionary<string, List<ItemType>>
        {
            {
                "Менеджер зоны",
                new List<ItemType>
                {
                    ItemType.KeycardO5,
                    ItemType.Radio,
                    ItemType.Painkillers,
                    ItemType.GunCOM15,
                    ItemType.ArmorLight
                }
            },
            {
                "Директор зоны",
                new List<ItemType>
                {
                    ItemType.KeycardO5,
                    ItemType.Radio,
                    ItemType.Painkillers,
                    ItemType.GunCOM18,
                    ItemType.ArmorHeavy
                }
            }
        };

        [Description("Максимальное здоровье административного персонала (Dictionary<string, float>)")]
        public Dictionary<string, float> AdministrativeHealth { get; set; } = new Dictionary<string, float>
        {
            {
                "Менеджер зоны",
                400f
            },
            {
                "Директор зоны",
                400f
            }
        };
        
        [Description("Список вещей, которые не могут поднимать Научные Сотрудники (List<ItemType>)")]
        public List<ItemType> ForbiddenForScientists { get; set; } = new List<ItemType>
        {
            ItemType.GunCrossvec,
            ItemType.GunFRMG0,
            ItemType.GunE11SR,
            ItemType.GunAK,
            ItemType.GunLogicer,
            ItemType.GunShotgun,
            ItemType.GunA7,
            ItemType.GunCom45,
            ItemType.MicroHID
        };

        [Description("Список вещей, которые не могут поднимать Рабочие (List<ItemType>)")]
        public List<ItemType> ForbiddenForWorkers { get; set; } = new List<ItemType>
        {
            ItemType.GunCrossvec,
            ItemType.GunFRMG0,
            ItemType.GunFSP9,
            ItemType.GunE11SR,
            ItemType.GunAK,
            ItemType.GunLogicer,
            ItemType.GunShotgun,
            ItemType.GunA7,
            ItemType.MicroHID
        };
        
        [Description("Список вещей, которые не могут поднимать сотрудники класса Д (List<ItemType>)")]
        public List<ItemType> ForbiddenForClassD { get; set; } = new List<ItemType>
        {
            ItemType.GunLogicer,
            ItemType.GunFRMG0,
            ItemType.MicroHID
        };
        
        [Description("Список вещей, которые не могут поднимать сотрудники СБ (List<ItemType>)")]
        public List<ItemType> ForbiddenForSecurity { get; set; } = new List<ItemType>
        {
            ItemType.MicroHID
        };
        
        [Description("Список вещей, которые не могут поднимать сотрудники Агентства (List<ItemType>)")]
        public List<ItemType> ForbiddenForAgency { get; set; } = new List<ItemType>
        {
            ItemType.GunLogicer,
            ItemType.GunFRMG0,
            ItemType.MicroHID
        };

        [Description("Инвентарь бойца БУО (List<ItemType>)")]
        public List<ItemType> BuoPrivateInventory { get; set; } = new List<ItemType>
        {
            ItemType.GunShotgun,
            ItemType.GunRevolver,
            ItemType.Medkit,
            ItemType.Painkillers,
            ItemType.ArmorCombat,
            ItemType.Radio,
            ItemType.KeycardMTFOperative
        };
        
        [Description("Инвентарь сержанта БУО (List<ItemType>)")]
        public List<ItemType> BuoSergeantInventory { get; set; } = new List<ItemType>
        {
            ItemType.GunE11SR,
            ItemType.GunRevolver,
            ItemType.Medkit,
            ItemType.Painkillers,
            ItemType.ArmorCombat,
            ItemType.Radio,
            ItemType.KeycardMTFOperative
        };
        
        [Description("Инвентарь джаггернаута БУО (List<ItemType>)")]
        public List<ItemType> BuoJaggerInventory { get; set; } = new List<ItemType>
        {
            ItemType.GunLogicer,
            ItemType.GunRevolver,
            ItemType.Medkit,
            ItemType.Painkillers,
            ItemType.ArmorCombat,
            ItemType.Radio,
            ItemType.KeycardMTFOperative
        };
        
        [Description("Инвентарь ликвидатора БУО (List<ItemType>)")]
        public List<ItemType> BuoTerminatorInventory { get; set; } = new List<ItemType>
        {
            ItemType.Painkillers,
            ItemType.Painkillers,
            ItemType.Radio,
            ItemType.GunFRMG0,
            ItemType.Adrenaline,
            ItemType.SCP500,
            ItemType.ArmorHeavy,
            ItemType.KeycardMTFOperative
        };

        [Description("Список ролей, на которые не распростроняется инфекция SCP-008 (List<RoleTypeId>)")]
        public List<RoleTypeId> DoNotPoisonRoles { get; set; } = new List<RoleTypeId>
        {
            RoleTypeId.ChaosConscript,
            RoleTypeId.ChaosRifleman,
            RoleTypeId.ChaosMarauder,
            RoleTypeId.ChaosRepressor,
            RoleTypeId.NtfPrivate,
            RoleTypeId.NtfSpecialist,
            RoleTypeId.NtfSergeant,
            RoleTypeId.NtfCaptain
        };

        [Description("Список вещей, которые нельзя брать SCP-035 (List<ItemType>)")]
        public List<ItemType> Scp035ForbiddenItems { get; set; } = new List<ItemType>
        {
            ItemType.GunA7,
            ItemType.GunAK,
            ItemType.GunCom45,
            ItemType.GunCrossvec,
            ItemType.GunE11SR,
            ItemType.GunFRMG0,
            ItemType.GunLogicer,
            ItemType.Painkillers,
            ItemType.Medkit
        };

        [Description("Список вещей, у которых есть иммунитет к чистке командой vudclear (List<ItemType>)")]
        public List<ItemType> ClearImmunityItems { get; set; } = new List<ItemType>
        {
            ItemType.Medkit,
            ItemType.Adrenaline,
            ItemType.Painkillers,
            ItemType.MicroHID,
            ItemType.GunE11SR,
            ItemType.GunCOM15,
            ItemType.Radio,
            ItemType.Ammo9x19,
            ItemType.Ammo12gauge,
            ItemType.Ammo44cal,
            ItemType.Ammo556x45,
            ItemType.Ammo762x39,
            ItemType.SCP500,
            ItemType.SCP1853,
            ItemType.SCP244a,
            ItemType.SCP244b,
            ItemType.SCP207
        };

        [Description("Список DamageType, которые взрывают пояс шахида (List<DamageType>)")]
        public List<DamageType> BlowingDamageTypes { get; set; } = new List<DamageType>
        {
            DamageType.Firearm,
            DamageType.Com15,
            DamageType.Com18,
            DamageType.Com45,
            DamageType.Crossvec,
            DamageType.Explosion,
            DamageType.Frmg0,
            DamageType.Fsp9,
            DamageType.Logicer,
            DamageType.Revolver,
            DamageType.Shotgun,
            DamageType.AK,
            DamageType.E11Sr
        };
        
        public string CustomHelp { get; set; } = @"
  <color=#FFFFFF>---------------</color><color=#FEA831>FOUNDATION-X</color><color=#FFFFFF>---------------</color>
  <color=#FEA831>ВСЕ КОМАНДЫ ВВОДЯТСЯ В КОНСОЛЬ НА [`] ИЛИ [~] С ТОЧКОЙ (ПРИМЕР - .help)</color>
  <color=#FEA831>[help]</color><color=#FFFFFF> - Это сообщение.</color>
  <color=#FEA831>[vcuff]</color><color=#FFFFFF> - Связать человека напротив. Доступно для ГОР, Главы Охраны и Агентов ОВБ. Нужно оружие в руках.</color>
  <color=#FEA831>[classd]</color><color=#FFFFFF> - Встать в очередь на спавн за Испытуемого. Доступно в башне.</color>
  <color=#FEA831>[checkcode]</color><color=#FFFFFF> - Узнать текущий код.</color>
  <color=#FEA831>[boom]</color><color=#FFFFFF> - Взорваться. Доступно, если на вас надет пояс шахида.</color>
  <color=#FFFFFF>---------------</color><color=#FEA831>FOUNDATION-X</color><color=#FFFFFF>---------------</color>";
    }
}

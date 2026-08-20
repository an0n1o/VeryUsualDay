using System;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using UnityEngine;

namespace VeryUsualDay.Abilities
{
    public abstract class BaseAbility
    {
        public readonly SettingBase Setting;

        private DateTime _cooldown;

        protected string Label { get; private set; }

        public abstract TimeSpan CooldownTime { get; set; }

        protected bool IsInCooldown
        {
            get => DateTime.UtcNow - _cooldown < CooldownTime;
            set
            {
                if (value)
                    _cooldown = DateTime.UtcNow;
                else
                    _cooldown = DateTime.UtcNow - CooldownTime;
            }
        }

        internal BaseAbility(
            int id,
            string label,
            KeyCode suggestedKey,
            string description)
        {
            Label = label;

            Setting = new KeybindSetting(
                id: id,
                label: label,
                suggested: suggestedKey,
                preventInteractionOnGUI: true,
                allowSpectatorTrigger: false,
                hintDescription: description,
                collectionId: 255,
                header: null,
                onChanged: (p, s) =>
                {
                    if (s.Id != id)
                        return;

                    var keybind = s as KeybindSetting;

                    if (keybind == null)
                        return;

                    if (!keybind.IsPressed)
                        return;

                    HandleUsing(p);
                });
        }

        protected abstract void HandleUsing(Player player);
    }
}
#region Copyright & License Information
/*
 * Copyright 2007-2017 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using System;
using System.Drawing;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.MW.Traits;
using OpenRA.Traits;

namespace OpenRA.Mods.MW.Traits
{
    [Desc("Plays an audio notification and shows a radar ping when a harvester is attacked.",
        "Attach this to the player actor.")]
    public class AcolyteAttackNotifierInfo : ITraitInfo
    {
        [Desc("Minimum duration (in seconds) between notification events.")]
        public readonly int NotifyInterval = 30;

        public readonly Color RadarPingColor = Color.Red;

        [Desc("Length of time (in ticks) to display a location ping in the minimap.")]
        public readonly int RadarPingDuration = 10 * 25;

        [Desc("The audio notification type to play.")]
        public string Notification = "HarvesterAttack";

        public object Create(ActorInitializer init) { return new AcolyteAttackNotifier(init.Self, this); }
    }

    public class AcolyteAttackNotifier : INotifyDamage
    {
        readonly RadarPings radarPings;
        readonly AcolyteAttackNotifierInfo info;

        int lastAttackTime;

        public AcolyteAttackNotifier(Actor self, AcolyteAttackNotifierInfo info)
        {
            radarPings = self.World.WorldActor.TraitOrDefault<RadarPings>();
            this.info = info;
            lastAttackTime = -info.NotifyInterval * 25;
        }

        void INotifyDamage.Damaged(Actor self, AttackInfo e)
        {
            // Don't track self-damage
            if (e.Attacker != null && e.Attacker.Owner == self.Owner)
                return;

            // Only track last hit against our harvesters
            if (!self.Info.HasTraitInfo<AcolytePreyInfo>())
                return;

            if (self.World.WorldTick - lastAttackTime > info.NotifyInterval * 25)
            {
                Game.Sound.PlayNotification(self.World.Map.Rules, self.Owner, "Speech", info.Notification, self.Owner.Faction.InternalName);

                if (radarPings != null)
                    radarPings.Add(() => self.Owner.IsAlliedWith(self.World.RenderPlayer), self.CenterPosition, info.RadarPingColor, info.RadarPingDuration);
            }

            lastAttackTime = self.World.WorldTick;
        }
    }
}
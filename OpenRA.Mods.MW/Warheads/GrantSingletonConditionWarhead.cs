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

using System.Collections.Generic;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Warheads;
using OpenRA.Traits;

namespace OpenRA.Mods.MW.Warheads
{
    public class GrantSingletonConditionWarhead : Warhead
    {
        [FieldLoader.Require]
        [Desc("The condition to apply. Must be included in the target actor's ExternalConditions list.")]
        public readonly string Condition = null;

        [Desc("Duration of the condition (in ticks). Set to 0 for a permanent condition.")] public readonly int Duration = 0;

        public readonly WDist Range = WDist.FromCells(1);

        public readonly WDist ExtraScanRange = WDist.FromCells(3);

        public override void DoImpact(Target target, Actor firedBy, IEnumerable<int> damageModifiers)
        {
            var actors = target.Type == TargetType.Actor
                ? new[] { target.Actor }
                : firedBy.World.FindActorsInCircle(target.CenterPosition, ExtraScanRange)
                .Where(a =>
                    {
                        var activeShapes = a.TraitsImplementing<HitShape>().Where(Exts.IsTraitEnabled);
                        var directHit = activeShapes.Any(i => i.Info.Type.DistanceFromEdge(target.CenterPosition, a).LengthSquared <= Range.LengthSquared);

                        if (directHit)
                        {
                            return true;
                        }

                        return false;
                    });

            if (actors.Any())
            {
                var a = actors.ClosestTo(target.CenterPosition);
                if (IsValidAgainst(a, firedBy))
                {
                    var external = a.TraitsImplementing<ExternalCondition>()
                        .FirstOrDefault(t => t.Info.Condition == Condition && t.CanGrantCondition(a, firedBy));

                    if (external != null)
                    {
                        external.GrantCondition(a, firedBy, Duration);
                    }
                }
            }
        }
    }
}

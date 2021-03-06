﻿#region Copyright & License Information
/*
 * Copyright 2007-2017 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using OpenRA.Mods.Common.Widgets;
using OpenRA.Traits;
using OpenRA.Widgets;

namespace OpenRA.Mods.MW.Widgets.Logic
{
    public class IngamePopulationCounterLogic : ChromeLogic
    {
        const float DisplayFracPerFrame = .07f;
        const int DisplayDeltaPerFrame = 37;

        readonly Player player;
        readonly string cashLabel;

        int displayResources;
        string displayLabel;

        [ObjectCreator.UseCtor]
        public IngamePopulationCounterLogic(Widget widget, World world)
        {
            var pop = widget.Get<LabelWithTooltipWidget>("POP");

            player = world.LocalPlayer;

            cashLabel = pop.Text;
            displayLabel = cashLabel.F(displayResources);

            pop.GetText = () => displayLabel;

            displayResources = player.PlayerActor.Trait<PlayerCivilization>().Peasantpopulationvar;
        }

        public override void Tick()
        {
            if (player.Faction.InternalName != "ded")
                displayLabel = "Peasants: " + cashLabel.F(player.PlayerActor.Trait<PlayerCivilization>().Peasantpopulationvar);
            else
                displayLabel = "Souls: " + cashLabel.F(player.PlayerActor.Trait<PlayerCivilization>().Peasantpopulationvar);
        }
    }
}

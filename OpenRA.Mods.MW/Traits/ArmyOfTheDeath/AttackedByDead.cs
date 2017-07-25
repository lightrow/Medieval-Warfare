﻿using OpenRA.Traits;

namespace OpenRA.Mods.Mw.Traits
{
	[Desc("How much the unit is worth in Peasants.")]
	public class AttackedByDeadInfo : TraitInfo<AttackedByDead> { }

	public class AttackedByDead
	{
		public int AttackCount;
	}
}
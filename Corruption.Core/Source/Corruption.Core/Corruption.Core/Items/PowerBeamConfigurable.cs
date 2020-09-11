using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Core.Items
{
    public class PowerBeamConfigurable : OrbitalStrike
    {
		private const int FiresStartedPerTick = 4;

		private static readonly IntRange FlameDamageAmountRange = new IntRange(65, 100);

		private static readonly IntRange CorpseFlameDamageAmountRange = new IntRange(5, 10);

		private CompOrbitalBeam BeamComp => this.TryGetComp<CompOrbitalBeam>();

		private CompProperties_OrbitalBeamConfigurable BeamProps => this.BeamComp.Props as CompProperties_OrbitalBeamConfigurable;

		private static List<Thing> tmpThings = new List<Thing>();

		public override void StartStrike()
		{
			base.StartStrike();
			if (this.BeamProps.drawGlowMote)
			{
				MoteMaker.MakePowerBeamMote(base.Position, base.Map);
			}
		}

		public override void Tick()
		{
			base.Tick();
			if (!base.Destroyed)
			{
				for (int i = 0; i < 4; i++)
				{
					StartRandomFireAndDoFlameDamage();
				}
			}
		}

		private void StartRandomFireAndDoFlameDamage()
		{
			IntVec3 c = (from x in GenRadial.RadialCellsAround(base.Position, BeamComp.Props.width, useCenter: true)
						 where x.InBounds(base.Map)
						 select x).RandomElementByWeight((IntVec3 x) => 1f - Mathf.Min(x.DistanceTo(base.Position) / BeamComp.Props.width, 1f) + 0.05f);
			FireUtility.TryStartFireIn(c, base.Map, Rand.Range(0.1f, 0.925f));
			tmpThings.Clear();
			tmpThings.AddRange(c.GetThingList(base.Map));
			for (int i = 0; i < tmpThings.Count; i++)
			{
				int num = (tmpThings[i] is Corpse) ? BeamProps.corpseDamageRange.RandomInRange : BeamProps.damageRange.RandomInRange;
				Pawn pawn = tmpThings[i] as Pawn;
				BattleLogEntry_DamageTaken battleLogEntry_DamageTaken = null;
				if (pawn != null)
				{
					battleLogEntry_DamageTaken = new BattleLogEntry_DamageTaken(pawn, RulePackDefOf.DamageEvent_PowerBeam, instigator as Pawn);
					Find.BattleLog.Add(battleLogEntry_DamageTaken);
				}
				tmpThings[i].TakeDamage(new DamageInfo(RimWorld.DamageDefOf.Flame, num, 0f, -1f, instigator, null, weaponDef)).AssociateWithLog(battleLogEntry_DamageTaken);
			}
			tmpThings.Clear();
		}
	}

	public class CompProperties_OrbitalBeamConfigurable : CompProperties_OrbitalBeam
	{
		public bool drawGlowMote = false;
		public IntRange damageRange =  new IntRange(25, 50);
		public IntRange corpseDamageRange =  new IntRange(5, 10);
	}
}

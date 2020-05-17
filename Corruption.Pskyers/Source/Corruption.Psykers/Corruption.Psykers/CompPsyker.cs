using Corruption.Core.Soul;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Corruption.Psykers
{
    public class CompPsyker : ThingComp
    {
        private List<AbilityDef> LearnedAbilities = new List<AbilityDef>();

        public PsykerDisciplineDef MainDiscipline = PsykerDisciplineDefOf.Initiate;

        public PsykerDisciplineDef minorDiscipline;

        public Pawn Pawn => this.parent as Pawn;

        public Trait PsykerPowerTrait
        {
            get
            {
                Trait psykerTrait = Pawn?.story.traits.GetTrait(SoulTraitDefOf.Psyker);
                if (psykerTrait != null)
                {
                    return psykerTrait;
                }
                return null;
            }
        }

        public bool HasLearnedAbility(AbilityDef def)
        {
            return this.LearnedAbilities.Contains(def);
        }

        public float PsykerXP;

        public void AddXP(float amount)
        {
            int adjustedXP = (int)(amount);
            this.PsykerXP += Math.Abs(adjustedXP);
        }

        public bool TryLearnPower(AbilityDef def)
        {
            if (this.LearnedAbilities.Contains(def))
            {
                return false;
            }

            //if (this.LearnedAbilities.Where(x => x.level == def.level).Count() > PsykerUtility.PsykerAbilitieSlots[def.level])
            //{
            //    return false;
            //}
            this.LearnedAbilities.Add(def);
            this.Pawn.abilities.GainAbility(def);

            return true;
        }

        public bool TryLearnPower(PsykerLearnablePower learnablePower)
        {
            float previousXP = this.PsykerXP;
            this.PsykerXP = this.PsykerXP - learnablePower.cost;
            if (this.PsykerXP < 0)
            {
                this.PsykerXP = previousXP;
                Messages.Message("PsykerLearnXPShortage".Translate(), null, MessageTypeDefOf.RejectInput);

                return false;
            }

            if (learnablePower.replacesPerequisite)
            {
                this.Pawn.abilities.RemoveAbility(learnablePower.perequesiteAbility);
            }

            return this.TryLearnPower(learnablePower.ability);

        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            Command_Action command = new Command_Action();
            command.defaultLabel = "PsykerPowerManagement".Translate();
            command.defaultDesc = "PsykerPowerManagementDescr".Translate();
            command.icon = PsykerUtility.PsykerIcon;
            command.action = delegate
            {
                if (this.parent == null) Log.Message("No parent???");
                Find.WindowStack.Add(new Window_Psyker(this));
            };
            yield return command;

        }

        public void ExposeData()
        {
            Scribe_Values.Look<float>(ref this.PsykerXP, "psykerPX");
            Scribe_Collections.Look<AbilityDef>(ref this.LearnedAbilities, "learnedAbilities", LookMode.Deep);
            Scribe_Defs.Look<PsykerDisciplineDef>(ref this.MainDiscipline, "chosenDiscipline");
        }


    }
}

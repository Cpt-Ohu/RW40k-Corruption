using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corruption.Core.Abilities
{
    public interface IAbilityLearner 
    {
        bool TryLearnAbility(AbilityDef def);
        bool TryLearnAbility(LearnableAbility learnablePower);
        bool HasLearnedAbility(AbilityDef def);
        bool LearningRequirementsMet(LearnableAbility selectedPower);
    }
}

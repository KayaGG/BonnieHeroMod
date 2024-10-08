global using static BonnieHeroMod.Extensions;
using System.Diagnostics.CodeAnalysis;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
namespace BonnieHeroMod;

public static class Extensions
{
    public static bool GetBonnieData(this Tower? tower, out BonnieData exists)
    {
        if(tower == null)
        {
            exists = default;
            return false;
        }

        var mutator = tower.GetMutator(MutatorName)?.Cast<SupportRemoveFilterOutTag.MutatorTower>();

        if (mutator is null)
        {
            exists = default;
            return false;
        }

        exists = BonnieData.Parse(mutator.removeScriptsWithSupportMutatorId);
        return true;
    }

    public static void CreateBonnieData(this Tower tower, BonnieData bonnieData = default)
    {
        var mutator = tower.GetMutator(MutatorName)?.TryCast<SupportRemoveFilterOutTag.MutatorTower>();
        if (mutator is null)
            tower.AddMutator(new SupportRemoveFilterOutTag.MutatorTower(MutatorName, bonnieData.ToJson(), null));
        else
            mutator.removeScriptsWithSupportMutatorId = bonnieData.ToJson();
    }

    public static void SetBonnieData(this Tower tower, BonnieData data)
    {
        if (!tower.GetBonnieData(out _))
        {
            tower.CreateBonnieData(data);
            return;
        }
        tower.GetMutator(MutatorName).Cast<SupportRemoveFilterOutTag.MutatorTower>().removeScriptsWithSupportMutatorId = data.ToJson();
    }
}
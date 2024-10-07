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

        var mutator = tower.GetMutator(MutatorName).Cast<SupportRemoveFilterOutTag.MutatorTower>();

        if (mutator is null)
        {
            exists = default;
            return false;
        }

        exists = BonnieData.Parse(mutator.removeScriptsWithSupportMutatorId);
        return true;
    }

    public static BonnieData GetOrCreateBonnieData(this Tower tower)
    {
        if (tower.GetBonnieData(out var data))
        {
            return data;
        }

        var newData = new BonnieData();
        tower.AddMutator(new SupportRemoveFilterOutTag.MutatorTower(MutatorName, newData.ToJson(), null));
        return newData;
    }

    public static void SetBonnieData(this Tower tower, BonnieData data)
    {
        tower.GetOrCreateBonnieData().CurrentTier = data.CurrentTier;
        tower.GetOrCreateBonnieData().MaxTier = data.MaxTier;
        tower.GetOrCreateBonnieData().Bank = data.Bank;
    }

    public static void SaveBonnieData(this Tower tower, BonnieData data)
    {
        tower.GetMutator(MutatorName).Cast<SupportRemoveFilterOutTag.MutatorTower>().removeScriptsWithSupportMutatorId = data.ToJson();
    }
}
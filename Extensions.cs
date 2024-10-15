global using static BonnieHeroMod.Extensions;
using System.Diagnostics.CodeAnalysis;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;

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

    public static void SellCarts(this Tower tower)
    {
        if (tower.GetBonnieData(out BonnieData bonnieData))
        {
            InGame.instance.SetCash(InGame.instance.GetCash() + bonnieData.SellAmount);
            bonnieData.SellAmount = 0; //set "bank" to 0
            bonnieData.CurrentTier = 0; //set cart tier to 0
            TowerSelectionMenu.instance.selectedTower.tower.SetBonnieData(bonnieData);
        }
    }
}
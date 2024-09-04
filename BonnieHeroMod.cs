using MelonLoader;
using BTD_Mod_Helper;
using BonnieHeroMod;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Objects;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity;
using BTD_Mod_Helper.Extensions;
using BTD_Mod_Helper.Api.Towers;

[assembly: MelonInfo(typeof(BonnieHeroMod.BonnieHeroMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BonnieHeroMod;

public class BonnieHeroMod : BloonsTD6Mod
{
    public override void OnTowerCreated(Tower tower, Entity target, Model modelToUse)
    { 
        MelonLogger.Msg(tower.model.name + " created");
        if (tower.model.name != "BonnieHeroMod-BonnieHero")
        {
            MelonLogger.Msg(tower.model.name + " above level 1, patching in behaviors");
        }
    }

    /*public override void OnTowerUpgraded(Tower tower, string upgradeName, TowerModel newBaseTowerModel)
    {
        base.OnTowerUpgraded(tower, upgradeName, newBaseTowerModel);
        MelonLogger.Msg(tower.parentTowerId);
        MelonLogger.Msg(tower.model.name + " upgraded to " + upgradeName);
        if (upgradeName == "BonnieHeroMod-BonnieHero 2")
        {
            MelonLogger.Msg("HOLY FUCK FIRING");
            //tower.AddTowerBehavior<Il2CppAssets.Scripts.Simulation.Bloons.Behaviors.SpawnBloonsAction>();
            
        }
    }*/

}
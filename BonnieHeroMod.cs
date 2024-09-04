using MelonLoader;
using BTD_Mod_Helper;
using BonnieHeroMod;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Objects;
using Il2CppAssets.Scripts.Models;

[assembly: MelonInfo(typeof(BonnieHeroMod.BonnieHeroMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BonnieHeroMod;

public class BonnieHeroMod : BloonsTD6Mod
{
    public override void OnTowerCreated(Tower tower, Entity target, Model modelToUse)
    { 
        MelonLogger.Msg(tower.model.name + " created");
        /*if (tower)
        {

        }*/
    }
}
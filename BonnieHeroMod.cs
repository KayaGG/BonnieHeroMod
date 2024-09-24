using MelonLoader;
using BTD_Mod_Helper;
using BonnieHeroMod;
using BTD_Mod_Helper.Api;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Objects;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity;
using BTD_Mod_Helper.Extensions;
using BTD_Mod_Helper.Api.Towers;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity.Towers.Weapons;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using UnityEngine;
using Vector3 = Il2CppAssets.Scripts.Simulation.SMath.Vector3;
using Il2CppAssets.Scripts.Simulation.Track.RoundManagers;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu.TowerSelectionMenuThemes;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using BTD_Mod_Helper.Api.Components;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using static BTD_Mod_Helper.Api.Enums.VanillaSprites;
using BTD_Mod_Helper.Api.Enums;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using static Il2CppAssets.Scripts.Utils.ObjectCache;

[assembly: MelonInfo(typeof(BonnieHeroMod.BonnieHeroMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BonnieHeroMod;

[HarmonyPatch]
public class BonnieHeroMod : BloonsTD6Mod
{
    public override void OnTowerUpgraded(Tower tower, string upgradeName, TowerModel newBaseTowerModel)
    {
        base.OnTowerUpgraded(tower, upgradeName, newBaseTowerModel);
        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
        {
            var towerLogic = tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>();
            switch (tower.towerModel.tier)
            {
                case 2:
                    BonnieLogic.BonnieUI.Init(TowerSelectionMenu.instance);
                    tower.AddMutator(new RangeSupport.MutatorTower(false, "MinecartTier", 0, 0, null));
                    break;
                case 5:
                    towerLogic.glueLevel = 15;
                    BonnieLogic.BonnieUI.UpdateUI();
                    break;
                case 8:
                    towerLogic.glueLevel = 20;
                    BonnieLogic.BonnieUI.UpdateUI();
                    break;
                case 11:
                    towerLogic.glueLevel = 25;
                    BonnieLogic.BonnieUI.UpdateUI();
                    break;
                case 14:
                    towerLogic.glueLevel = 30;
                    BonnieLogic.BonnieUI.UpdateUI();
                    break;
                case 17:
                    towerLogic.glueLevel = 35;
                    BonnieLogic.BonnieUI.UpdateUI();
                    break;
                case 20:
                    towerLogic.glueLevel = 40;
                    BonnieLogic.BonnieUI.UpdateUI();
                    break;
            }
        }
    }

    public override void OnTowerCreated(Tower tower, Entity target, Model modelToUse)
    {
        MelonLogger.Warning("THIS IS AN UNFINISHED VERSION OF BonnieHeroMod GLITCHES WILL OCCUR");
        base.OnTowerCreated(tower, target, modelToUse);
        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
        {
            if (tower.towerModel.tier > 0)
            {
                BonnieLogic.BonnieUI.Init(TowerSelectionMenu.instance);
                tower.AddMutator(new RangeSupport.MutatorTower(false, "MinecartTier", 0, 0, null));
            }
        }
    }

    public override void OnTowerSold(Tower tower, float amount)
    {
        base.OnTowerSold(tower, amount);

        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
        {
            if (tower.towerModel.tier > 0)
            {
                var towerLogic = tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>();
                BonnieLogic.CartSellLogic();
                towerLogic.glueLevel = 10;
                
            }
        }
    }


    public override void OnTowerSelected(Tower tower)
    {
        base.OnTowerSelected(tower);
        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
        {
            if (BonnieLogic.BonnieUI.bonniePanel != null)
            {
                BonnieLogic.BonnieUI.BonnieUIToggle(true);
            }
        }
    }

    public override void OnTowerDeselected(Tower tower)
    {
        base.OnTowerDeselected(tower);
        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
        {
            if (BonnieLogic.BonnieUI.bonniePanel != null)
            {
                BonnieLogic.BonnieUI.BonnieUIToggle(false);
            }
        }
    }

    public override void OnRoundStart()
    {
        base.OnRoundStart();
    }
}

//Code by doombubbles
/*[HarmonyPatch(typeof(TSMThemeBananaFarm), nameof(TSMThemeBananaFarm.UpdateFromSimInfo))]
internal static class TSMThemeBananaFarm_UpdateFromSimInfo
{
    [HarmonyPrefix]
    internal static void Prefix(TowerToSimulation tower, ref int __state)
    {
        __state = tower.Def.tiers[1];
        tower.Def.tiers[1] = 4;
    }

    [HarmonyPostfix]
    internal static void Postfix(TSMThemeBananaFarm __instance, TowerToSimulation tower, ref int __state)
    {
        tower.Def.tiers[1] = __state;
    }
}*/
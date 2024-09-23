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
            if (tower.towerModel.tier == 2)
            {
                BonnieUI.Init(TowerSelectionMenu.instance);
                tower.AddMutator(new RangeSupport.MutatorTower(false, "MinecartTier", 0, 0, null));
            }
        }
    }

    public override void OnTowerCreated(Tower tower, Entity target, Model modelToUse)
    {
        base.OnTowerCreated(tower, target, modelToUse);
        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
        {
            if (tower.towerModel.tier > 0)
            {
                BonnieUI.Init(TowerSelectionMenu.instance);
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
                BonnieUI.CartSellLogic();
            }
        }
    }


    public override void OnTowerSelected(Tower tower)
    {
        base.OnTowerSelected(tower);
        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
        {
            if (BonnieUI.bonniePanel != null)
            {
                BonnieUI.BonnieUIToggle(true);
            }
        }
    }

    public override void OnTowerDeselected(Tower tower)
    {
        base.OnTowerDeselected(tower);
        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
        {
            if (BonnieUI.bonniePanel != null)
            {
                BonnieUI.BonnieUIToggle(false);
            }
        }
    }

    public override void OnRoundStart()
    {
        base.OnRoundStart();
    }

    public class BonnieUI
    {

        public static TowerSelectionMenu menu;

        public static ModHelperPanel bonniePanel;
        private static ModHelperText cartTier;
        private static ModHelperButton cartUpgrade;
        private static ModHelperText upgradeText;
        private static ModHelperButton cartSell;
        private static ModHelperText sellText;


        public static void Init(TowerSelectionMenu tsm)
        {
            if (bonniePanel == null)
            {
                TaskScheduler.ScheduleTask(() =>
                {
                    MelonLogger.Msg("BonnieUI Init");
                    menu = tsm;

                    bonniePanel = TowerSelectionMenu.instance.themeManager.currentTheme.gameObject.AddModHelperPanel(new Info("BonniePanel", InfoPreset.FillParent));

                    cartTier = bonniePanel.AddText(new Info("CartLevel", 250, -75, 270, 135), "Cart tier: " + menu.selectedTower.tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>().multiplier);

                    cartUpgrade = bonniePanel.AddButton(new Info("CartUpgrade", -225, -580, 270, 135), VanillaSprites.GreenBtnLong,
                        new System.Action(() =>
                        {
                            CartUpgradeLogic();
                        }));
                    upgradeText = cartUpgrade.AddText(new Info("CartUpgradeText", 0, 0, 270, 135), "Upgrade (" + 240 + ")");

                    //cartUpgradeText = bonniePanel.AddText(new Info("CartUpgradeText", -225, -580, 270, 135), "Upgrade");

                    cartSell = bonniePanel.AddButton(new Info("CartSell", 225, -580, 270, 135), VanillaSprites.RedBtnLong,
                        new System.Action(() =>
                        {
                            CartSellLogic();
                        }));
                    sellText = cartSell.AddText(new Info("CartSellText", 0, 0, 270, 135), "Sell (" + menu.selectedTower.tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>().additive + ")", 40);

                    menu.selectedTower.tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>().glueLevel = 10; //this shit is the fucking max tier oh my god

                }, () => TowerSelectionMenu.instance.themeManager.currentTheme != null);
            }  
        }

        public static void BonnieUIToggle(bool state)
        {
            bonniePanel.SetActive(state);
        }

        public static void CartUpgradeLogic()
        {
            var towerLogic = menu.selectedTower.tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>();
            var maxTier = towerLogic.glueLevel;
            if (towerLogic.multiplier <= maxTier) //rearrange if statement so that logic works properly, cant be fucked rn
            {
                MelonLogger.Msg("if");

                towerLogic.multiplier++; //upgrade cart tier by 1
                towerLogic.additive += menu.selectedTower.tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>().priority * 0.7f; //add cash to "bank"
                InGame.instance.SetCash(InGame.instance.GetCash() - towerLogic.priority);

                upgradeText.SetText("Upgrade (" + towerLogic.priority + ")");

                cartTier.SetText("Cart tier: " + towerLogic.multiplier);
                sellText.SetText("Sell (" + towerLogic.additive + ")");

                switch (towerLogic.multiplier)
                {
                    case < 5:
                        towerLogic.priority = 240;
                        MelonLogger.Msg("Tier Group 1, price is " + towerLogic.priority);
                        break;
                    case < 10:
                        towerLogic.priority = 1000;
                        MelonLogger.Msg("Tier Group 2, price is " + towerLogic.priority);
                        break;
                }
            }
            else
            {
                MelonLogger.Msg("else");
                upgradeText.SetText("Max Tier Reached");
            }
        }

        public static void CartSellLogic()
        {
            var towerLogic = menu.selectedTower.tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>();

            towerLogic.priority = 240;
            towerLogic.multiplier = 0; //set cart tier to 0
            InGame.instance.SetCash(InGame.instance.GetCash() + towerLogic.additive);
            towerLogic.additive = 0; //set "bank" to 0

            upgradeText.SetText("Upgrade " + towerLogic.priority);

            cartTier.SetText("Cart tier: " + towerLogic.multiplier);
            sellText.SetText("Sell (" + towerLogic.additive + ")");
        }
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
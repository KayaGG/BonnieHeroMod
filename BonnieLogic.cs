using BTD_Mod_Helper.Api.Components;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using System;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Enums;
using HarmonyLib;
using BTD_Mod_Helper.Api.Helpers;


namespace BonnieHeroMod
{
    [HarmonyPatch]
    //todo fix ui init edgecase bugs, carts retain value but UI does not initialize properly
    public class BonnieLogic
    {
        public class BonnieUI
        {
            public static TowerSelectionMenu menu;

            public static ModHelperPanel bonniePanel;
            public static ModHelperText cartTier;
            public static ModHelperButton cartUpgrade;
            public static ModHelperText upgradeText;
            public static ModHelperButton cartSell;
            public static ModHelperText sellText;

            public static void Init(TowerSelectionMenu tsm)
            {
                if (TowerSelectionMenu.instance.selectedTower != null)
                {
                    if (TowerSelectionMenu.instance.selectedTower.tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
                    {
                        if (bonniePanel == null)
                        {
                            BTD_Mod_Helper.Api.TaskScheduler.ScheduleTask(() =>
                            {
                                menu = tsm;

                                bonniePanel = TowerSelectionMenu.instance.themeManager.currentTheme.gameObject.AddModHelperPanel(new Info("BonniePanel", InfoPreset.FillParent));

                                var cartWorth = 25f;


                                for (int i = 0; i < menu.selectedTower.tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>().multiplier; i++)
                                {
                                    switch (i)
                                    {
                                        case < 5:
                                            cartWorth += 15f;
                                            break;
                                        case < 10:
                                            cartWorth += 40f;
                                            break;
                                        case < 15:
                                            cartWorth += 100f;
                                            break;
                                        case < 20:
                                            cartWorth += 160f;
                                            break;
                                        case < 25:
                                            cartWorth += 280f;
                                            break;
                                        case < 30:
                                            cartWorth += 400f;
                                            break;
                                        case < 35:
                                            cartWorth += 600f;
                                            break;
                                        case < 40:
                                            cartWorth += 800f;
                                            break;
                                    }
                                }

                                cartTier = bonniePanel.AddText(new Info("CartTier", 220, -120, 400, 145), "Cart tier: " + menu.selectedTower.tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>().multiplier + "\n Worth: " + cartWorth, 40, Il2CppTMPro.TextAlignmentOptions.TopRight);

                                cartUpgrade = bonniePanel.AddButton(new Info("CartUpgrade", -225, -580, 300, 145), VanillaSprites.GreenBtnLong,
                                    new System.Action(() =>
                                    {
                                        CartUpgradeLogic();
                                    }));
                                upgradeText = cartUpgrade.AddText(new Info("CartUpgradeText", 0, 5, 300, 145), "Upgrade \n(" + 0 + ")");

                                cartSell = bonniePanel.AddButton(new Info("CartSell", 225, -580, 300, 145), VanillaSprites.RedBtnLong,
                                    new System.Action(() =>
                                    {
                                        CartSellLogic();
                                    }));
                                sellText = cartSell.AddText(new Info("CartSellText", 0, 5, 300, 135), "Sell \n(" + menu.selectedTower.tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>().additive + ")", 40);

                                UpdateUI();
                            }, () => TowerSelectionMenu.instance.themeManager.currentTheme != null);
                        }
                    }
                }
            }

            public static void UpdateUI()
            {
                if (TowerSelectionMenu.instance.selectedTower != null)
                {
                    if (TowerSelectionMenu.instance.selectedTower.tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
                    {
                        var nextUpgradePrice = 0;
                        var cartWorth = 25f;
                        var gameModel = InGame.instance.GetGameModel();
                        var mods = InGame.instance.GetGameModel().AllMods.ToIl2CppList();

                        for (int i = 0; i < menu.selectedTower.tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>().multiplier; i++)
                        {
                            switch (i)
                            {
                                case < 5:
                                    cartWorth += 15f;
                                    break;
                                case < 10:
                                    cartWorth += 40f;
                                    break;
                                case < 15:
                                    cartWorth += 100f;
                                    break;
                                case < 20:
                                    cartWorth += 160f;
                                    break;
                                case < 25:
                                    cartWorth += 280f;
                                    break;
                                case < 30:
                                    cartWorth += 400f;
                                    break;
                                case < 35:
                                    cartWorth += 600f;
                                    break;
                                case < 40:
                                    cartWorth += 800f;
                                    break;
                            }
                        }

                        switch (menu.selectedTower.tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>().multiplier + 1)
                        {
                            case <= 5:
                                nextUpgradePrice = CostHelper.CostForDifficulty(300, mods);
                                break;
                            case <= 10:
                                nextUpgradePrice = CostHelper.CostForDifficulty(1000, mods);
                                break;
                            case <= 15:
                                nextUpgradePrice = CostHelper.CostForDifficulty(2700, mods);
                                break;
                            case <= 20:
                                nextUpgradePrice = CostHelper.CostForDifficulty(4800, mods);
                                break;
                            case <= 25:
                                nextUpgradePrice = CostHelper.CostForDifficulty(8800, mods);
                                break;
                            case <= 30:
                                nextUpgradePrice = CostHelper.CostForDifficulty(13000, mods);
                                break;
                            case <= 35:
                                nextUpgradePrice = CostHelper.CostForDifficulty(21000, mods);
                                break;
                            case <= 40:
                                nextUpgradePrice = CostHelper.CostForDifficulty(29000, mods);
                                break;
                        }

                        cartTier.SetText("Cart tier: " + menu.selectedTower.tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>().multiplier + "\n Worth: " + cartWorth);
                        sellText.SetText("Sell \n(" + menu.selectedTower.tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>().additive + ")");
                        if (menu.selectedTower.tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>().multiplier != menu.selectedTower.tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>().glueLevel)
                        {
                            upgradeText.SetText("Upgrade \n(" + nextUpgradePrice + ")");
                        }
                        else
                        {
                            upgradeText.SetText("Max Tier Reached");
                        }
                    }
                }
            }

            public static void BonnieUIToggle(bool state)
            {
                bonniePanel.SetActive(state);
                if (state)
                {
                    UpdateUI();
                }
            }
        }
        public static void CartUpgradeLogic()
        {
            var towerLogic = BonnieUI.menu.selectedTower.tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>();
            var currentUpgradePrice = 0;
            var gameModel = InGame.instance.GetGameModel();
            var mods = InGame.instance.GetGameModel().AllMods.ToIl2CppList();

            if (towerLogic.multiplier < towerLogic.glueLevel)
            {
                // CURRENT Upgrade price switch case
                switch (towerLogic.multiplier)
                {
                    case < 5:
                        currentUpgradePrice = CostHelper.CostForDifficulty(300, mods); 
                        break;
                    case < 10:
                        currentUpgradePrice = CostHelper.CostForDifficulty(1000, mods);
                        break;
                    case < 15:
                        currentUpgradePrice = CostHelper.CostForDifficulty(2700, mods);
                        break;
                    case < 20:
                        currentUpgradePrice = CostHelper.CostForDifficulty(4800, mods);
                        break;
                    case < 25:
                        currentUpgradePrice = CostHelper.CostForDifficulty(8800, mods);
                        break;
                    case < 30:
                        currentUpgradePrice = CostHelper.CostForDifficulty(13300, mods);
                        break;
                    case < 35:
                        currentUpgradePrice = CostHelper.CostForDifficulty(21000, mods);
                        break;
                    case < 40:
                        currentUpgradePrice = CostHelper.CostForDifficulty(29000, mods);
                        break;
                }

                if (currentUpgradePrice < InGame.instance.GetCash())
                {
                    InGame.instance.SetCash(InGame.instance.GetCash() - currentUpgradePrice);
                    towerLogic.multiplier++; //upgrade cart tier by 1
                    towerLogic.additive += (float)Math.Floor(currentUpgradePrice * 0.7); //add cash to "bank"

                    BonnieUI.UpdateUI();
                }
            }
        }

        public static void CartSellLogic()
        {
            var towerLogic = BonnieUI.menu.selectedTower.tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>();
            towerLogic.multiplier = 0; //set cart tier to 0
            InGame.instance.SetCash(InGame.instance.GetCash() + towerLogic.additive);
            towerLogic.additive = 0; //set "bank" to 0

            BonnieUI.UpdateUI();
        }
    }
}

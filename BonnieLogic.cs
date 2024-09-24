using BTD_Mod_Helper.Api.Components;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Enums;

namespace BonnieHeroMod
{
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

                                var cartWorth = 50f;

                                for (int i = 0; i < menu.selectedTower.tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>().multiplier; i++)
                                {
                                    switch (i)
                                    {
                                        case < 5:
                                            cartWorth += 10f;
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
                                upgradeText = cartUpgrade.AddText(new Info("CartUpgradeText", 0, 5, 300, 145), "Upgrade \n(" + 240 + ")");

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
                        var nextUpgradePrice = 0f;
                        var cartWorth = 50f;

                        for (int i = 0; i < menu.selectedTower.tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>().multiplier; i++)
                        {
                            switch (i)
                            {
                                case < 5:
                                    cartWorth += 10f;
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
                                nextUpgradePrice = 240f;
                                break;
                            case <= 10:
                                nextUpgradePrice = 1000f;
                                break;
                            case <= 15:
                                nextUpgradePrice = 2700f;
                                break;
                            case <= 20:
                                nextUpgradePrice = 4800f;
                                break;
                            case <= 25:
                                nextUpgradePrice = 8800f;
                                break;
                            case <= 30:
                                nextUpgradePrice = 13000f;
                                break;
                            case <= 35:
                                nextUpgradePrice = 21000f;
                                break;
                            case <= 40:
                                nextUpgradePrice = 29000f;
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
            var currentUpgradePrice = 0f;

            if (towerLogic.multiplier < towerLogic.glueLevel)
            {
                // CURRENT Upgrade price switch case
                switch (towerLogic.multiplier)
                {
                    case < 5:
                        currentUpgradePrice = 240f;
                        break;
                    case < 10:
                        currentUpgradePrice = 1000f;
                        break;
                    case < 15:
                        currentUpgradePrice = 2700f;
                        break;
                    case < 20:
                        currentUpgradePrice = 4800f;
                        break;
                    case < 25:
                        currentUpgradePrice = 8800f;
                        break;
                    case < 30:
                        currentUpgradePrice = 13000f;
                        break;
                    case < 35:
                        currentUpgradePrice = 21000f;
                        break;
                    case < 40:
                        currentUpgradePrice = 29000f;
                        break;
                }

                if (currentUpgradePrice < InGame.instance.GetCash())
                {
                    InGame.instance.SetCash(InGame.instance.GetCash() - currentUpgradePrice);
                    towerLogic.multiplier++; //upgrade cart tier by 1
                    towerLogic.additive += currentUpgradePrice * 0.7f; //add cash to "bank"

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

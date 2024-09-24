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
        public static RangeSupport.MutatorTower towerLogic = BonnieUI.menu.selectedTower.tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>();

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
                }
            }

            public static void UpdateUI()
            {
                var nextUpgradePrice = 0f;

                switch (towerLogic.multiplier + 1)
                {
                    case < 5:
                        nextUpgradePrice = 240f;
                        break;
                    case < 10:
                        nextUpgradePrice = 1000f;
                        break;
                    case < 15:
                        nextUpgradePrice = 2700f;
                        break;
                    case < 20:
                        nextUpgradePrice = 4800f;
                        break;
                    case < 25:
                        nextUpgradePrice = 8800f;
                        break;
                    case < 30:
                        nextUpgradePrice = 13000f;
                        break;
                    case < 35:
                        nextUpgradePrice = 21000f;
                        break;
                    case < 40:
                        nextUpgradePrice = 29000f;
                        break;
                }

                cartTier.SetText("Cart tier: " + towerLogic.multiplier);
                sellText.SetText("Sell (" + towerLogic.additive + ")");
                if (towerLogic.multiplier != towerLogic.glueLevel)
                {
                    upgradeText.SetText("Upgrade (" + nextUpgradePrice + ")");
                }
                else
                {
                    upgradeText.SetText("Max Tier Reached");
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
            var currentUpgradePrice = 0f;
            var nextUpgradePrice = 0f;

            MelonLogger.Msg("Cart tier initial value at run: " + towerLogic.multiplier);

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

                MelonLogger.Msg("Upgrade price: " + currentUpgradePrice);
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
            towerLogic.multiplier = 0; //set cart tier to 0
            InGame.instance.SetCash(InGame.instance.GetCash() + towerLogic.additive);
            towerLogic.additive = 0; //set "bank" to 0

            BonnieUI.UpdateUI();
        }
    }
}

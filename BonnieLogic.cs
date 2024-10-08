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
    public static class BonnieLogic
    {
        public static void CartUpgradeLogic()
        {
            if (TowerSelectionMenu.instance.selectedTower.tower.GetBonnieData(out var towerLogic))
            {
                var currentUpgradePrice = 0;
                var mods = InGame.instance.GetGameModel().AllMods.ToIl2CppList();

                if (towerLogic.CurrentTier < towerLogic.MaxTier)
                {
                    // CURRENT Upgrade price switch case
                    currentUpgradePrice = towerLogic.CurrentTier switch
                    {
                        < 5 => CostHelper.CostForDifficulty(300, mods),
                        < 10 => CostHelper.CostForDifficulty(1000, mods),
                        < 15 => CostHelper.CostForDifficulty(2700, mods),
                        < 20 => CostHelper.CostForDifficulty(4800, mods),
                        < 25 => CostHelper.CostForDifficulty(8800, mods),
                        < 30 => CostHelper.CostForDifficulty(13300, mods),
                        < 35 => CostHelper.CostForDifficulty(21000, mods),
                        < 40 => CostHelper.CostForDifficulty(29000, mods),
                        _ => currentUpgradePrice
                    };

                    if (currentUpgradePrice < InGame.instance.GetCash())
                    {
                        InGame.instance.SetCash(InGame.instance.GetCash() - currentUpgradePrice);
                        towerLogic.CurrentTier++; //upgrade cart tier by 1
                        towerLogic.Bank += (float)Math.Floor(currentUpgradePrice * 0.7); //add cash to "bank"
                    }

                    TowerSelectionMenu.instance.selectedTower.tower.SetBonnieData(towerLogic);
                }
            }
        }

        public static void CartSellLogic(BonnieData towerLogic)
        {
            InGame.instance.SetCash(InGame.instance.GetCash() + towerLogic.Bank);
            towerLogic.Bank = 0; //set "bank" to 0
            towerLogic.CurrentTier = 0; //set cart tier to 0
            TowerSelectionMenu.instance.selectedTower.tower.SetBonnieData(towerLogic);
        }
    }
}

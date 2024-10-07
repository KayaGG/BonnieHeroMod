﻿using System;
using System.Collections;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu.TowerSelectionMenuThemes;
using Il2CppInterop.Runtime.Attributes;
using MelonLoader;
using UnityEngine;

namespace BonnieHeroMod;

[RegisterTypeInIl2Cpp(false)]
[HarmonyPatch]
public class BonnieUI : MonoBehaviour
{
    private TowerSelectionMenu _menu = null!;
    private ModHelperPanel bonniePanel;
    private ModHelperText cartTier;
    private ModHelperButton cartUpgrade;
    private ModHelperText upgradeText;
    private ModHelperButton cartSell;
    private ModHelperText sellText;

    public void SyncUI()
    {
        if (_menu?.selectedTower == null || _menu.selectedTower.tower.towerModel.baseId != ModContent.TowerID<BonnieHero>())
        {
            bonniePanel.SetActive(false);
            return;
        }

        if(!_menu.selectedTower.tower.GetBonnieData(out var towerLogic))
        {
            bonniePanel.SetActive(false);
            return;
        }

        bonniePanel.SetActive(true);


        var nextUpgradePrice = 0;
        var cartWorth = 25f;
        var mods = InGame.instance.GetGameModel().AllMods.ToIl2CppList();

        for (int i = 0;
             i < towerLogic.CurrentTier;
             i++)
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

        nextUpgradePrice = (towerLogic.CurrentTier + 1) switch
            {
                <= 5 => CostHelper.CostForDifficulty(300, mods),
                <= 10 => CostHelper.CostForDifficulty(1000, mods),
                <= 15 => CostHelper.CostForDifficulty(2700, mods),
                <= 20 => CostHelper.CostForDifficulty(4800, mods),
                <= 25 => CostHelper.CostForDifficulty(8800, mods),
                <= 30 => CostHelper.CostForDifficulty(13000, mods),
                <= 35 => CostHelper.CostForDifficulty(21000, mods),
                <= 40 => CostHelper.CostForDifficulty(29000, mods),
                _ => nextUpgradePrice
            };

        cartTier.SetText("Cart tier: " + towerLogic
            .CurrentTier + "\n Worth: " + cartWorth);
        sellText.SetText("Sell \n(" + towerLogic.Bank + ")");
        if (towerLogic.CurrentTier !=
            towerLogic.MaxTier)
        {
            upgradeText.SetText("Upgrade \n(" + nextUpgradePrice + ")");
        }
        else
        {
            upgradeText.SetText("Max Tier Reached");
        }
    }


    [HideFromIl2Cpp]
    [HarmonyPatch(typeof(MenuThemeManager), nameof(MenuThemeManager.SetTheme))]
    [HarmonyPostfix]
    private static void MenuThemeManager_SetTheme(
        MenuThemeManager __instance, BaseTSMTheme newTheme)
    {
        if (!__instance.selectionMenu.Is(out TowerSelectionMenu towerSelectionMenu))
            return;

        var bonnieUI = newTheme.gameObject.GetComponent<BonnieUI>();

        if (bonnieUI == null)
            newTheme.gameObject.AddComponent<BonnieUI>().Setup(towerSelectionMenu);
        else
            bonnieUI.SyncUI();
    }

    [HideFromIl2Cpp]
    [HarmonyPatch(typeof(MenuThemeManager), nameof(MenuThemeManager.UpdateTheme))]
    [HarmonyPostfix]
    private static void TowerSelectionMenu_SelectionChangedr(
        MenuThemeManager __instance)
    {
        if (!__instance.selectionMenu.Is(out TowerSelectionMenu towerSelectionMenu))
            return;

        if (!__instance.currentTheme.Is(out BaseTSMTheme theme))
            return;

        var bonnieUI = theme.gameObject.GetComponent<BonnieUI>();

        if (bonnieUI == null)
            theme.gameObject.AddComponent<BonnieUI>().Setup(towerSelectionMenu);
        else
            bonnieUI.SyncUI();
    }

    private void Setup(TowerSelectionMenu menu)
    {
        _menu = menu;
        bonniePanel = gameObject.AddModHelperPanel(new Info("BonniePanel",
            InfoPreset.FillParent));

        cartTier = bonniePanel.AddText(new Info("CartTier", 220, -120, 400, 145),
            "Cart tier: " +
            "\n Worth: ", 40, Il2CppTMPro.TextAlignmentOptions.TopRight);

        cartUpgrade = bonniePanel.AddButton(new Info("CartUpgrade", -225, -580, 300, 145), VanillaSprites.GreenBtnLong,
            new Action(() =>
            {
                BonnieLogic.CartUpgradeLogic();
                SyncUI();
            }));
        upgradeText = cartUpgrade.AddText(new Info("CartUpgradeText", 0, 5, 300, 145), "Upgrade \n(" + 0 + ")");

        cartSell = bonniePanel.AddButton(new Info("CartSell", 225, -580, 300, 145), VanillaSprites.RedBtnLong,
            new Action(() =>
            {
                BonnieLogic.CartSellLogic();
                SyncUI();
            }));
        sellText = cartSell.AddText(new Info("CartSellText", 0, 5, 300, 135),
            "Sell \n(" + ")", 40);

        SyncUI();
    }


}
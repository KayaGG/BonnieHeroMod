using System.Collections.Generic;
using MelonLoader;
using BTD_Mod_Helper;
using BonnieHeroMod;
using BTD_Mod_Helper.Api;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Objects;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using Il2CppAssets.Scripts.Models.Profile;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.RightMenu;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.StoreMenu;

[assembly: MelonInfo(typeof(BonnieHeroMod.BonnieHeroMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.Author)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BonnieHeroMod;

[HarmonyPatch]
public partial class BonnieHeroMod : BloonsTD6Mod
{
    public override void OnTowerCreated(Tower tower, Entity target, Model modelToUse)
    {
        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
        {
            tower.CreateBonnieData(new BonnieData());
        }
    }

    public override void OnTowerSold(Tower tower, float amount)
    {
        tower.SellCarts();
    }

    public override void OnTowerSaved(Tower tower, TowerSaveDataModel saveData)
    {
        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>() && tower.GetBonnieData(out var towerLogic))
        {
            saveData.metaData[MutatorName] = towerLogic.ToJson();
        }
    }

    public override void OnTowerLoaded(Tower tower, TowerSaveDataModel saveData)
    {
        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>() && tower.mutators != null)
        {
            tower.RemoveMutatorsById(MutatorName);
            var bonnieData = BonnieData.Parse(saveData.metaData[MutatorName]);
            tower.SetBonnieData(bonnieData);
        }
    }


    /// <inheritdoc />
    public override void OnNewGameModel(GameModel result, IReadOnlyList<ModModel> mods)
    {
        base.OnNewGameModel(result, mods);
        CurrentMods = mods;
    }

    public static IReadOnlyList<ModModel> CurrentMods { get; set; } = [ ];

#if false
    public override void OnMatchStart()
    {
        string newHero = ModContent.TowerID<BonnieHero>();
        var towerInventory = InGame.instance.GetTowerInventory();
        var unlockedHeroes = Game.instance.GetPlayerProfile().unlockedHeroes;
        foreach (var unlockedHero in unlockedHeroes)
        {
            towerInventory.towerMaxes[unlockedHero] = 0;
        }

        towerInventory.towerMaxes[newHero] = 1;

        var disallowSelectingDifferentTowers = ShopMenu.instance.disallowSelectingDifferentTowers;
        ShopMenu.instance.disallowSelectingDifferentTowers = false;
        ShopMenu.instance.RebuildTowerSet();
        ShopMenu.instance.disallowSelectingDifferentTowers = disallowSelectingDifferentTowers;
        foreach (var button in ShopMenu.instance.ActiveTowerButtons)
        {
            button.Cast<TowerPurchaseButton>().Update();
        }
    }
#endif
}
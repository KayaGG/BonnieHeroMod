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
public class BonnieHeroMod : BloonsTD6Mod
{
    public override void OnTowerUpgraded(Tower tower, string upgradeName, TowerModel newBaseTowerModel)
    {

        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
        {
            tower.GetBonnieData(out var towerLogic);
            switch (tower.towerModel.tier)
            {
                case 5:
                    towerLogic.MaxTier = 15;
                    break;
                case 8:
                    towerLogic.MaxTier = 20;
                    break;
                case 11:
                    towerLogic.MaxTier = 25;
                    break;
                case 14:
                    towerLogic.MaxTier = 30;
                    break;
                case 17:
                    towerLogic.MaxTier = 35;
                    break;
                case 20:
                    towerLogic.MaxTier = 40;
                    break;
            }
            tower.SetBonnieData(towerLogic);
        }
    }

    public override void OnTowerCreated(Tower tower, Entity target, Model modelToUse)
    {
        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
        {
            if (tower.towerModel.tier > 0)
            {
                tower.CreateBonnieData(new BonnieData());
            }
        }
    }

    public override void OnTowerSold(Tower tower, float amount)
    {
        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
        {
            if (tower.GetBonnieData(out var towerLogic))
            {
                BonnieLogic.CartSellLogic(towerLogic);
            }
        }
    }

    public override void OnTowerSaved(Tower tower, TowerSaveDataModel saveData)
    {
        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>() && tower.GetBonnieData(out var towerLogic))
        {
            saveData.metaData[MutatorName] = towerLogic.ToJson();
            //todo
        }
    }

    public override void OnTowerLoaded(Tower tower, TowerSaveDataModel saveData)
    {
        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
        {
            if (tower.mutators != null)
            {
                tower.RemoveMutatorsById(MutatorName);
                var bonnieData = BonnieData.Parse(saveData.metaData[MutatorName]);
                tower.SetBonnieData(bonnieData);
                //todo
            }
        }
    }

#if DEBUG
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

    [HarmonyPatch(typeof(Ability), nameof(Ability.Activate))]
    public static class BonnieAbility
    {
        [HarmonyPostfix]
        public static void baPostfix(Ability __instance)
        {
            var bonnieHero = InGame.instance.GetTowers().Find(tower => tower.towerModel.baseId == ModContent.TowerID<BonnieHero>());
            if (bonnieHero != null)
            {
                if (__instance.abilityModel.name == "AbilityModel_MassDetonation")
                {
                    var bloons = InGame.instance.GetBloons();
                    if (bloons != null)
                    {
                        foreach (var bloon in bloons)
                        {
                            if (bloon.bloonModel.baseId == ModContent.BloonID<BloonstoneCart>())
                            {
                                var attackModel = bonnieHero.towerModel.GetAttackModel();
                                var dynamite = attackModel.weapons[0].projectile;
                                var explosion = dynamite.GetBehavior<CreateProjectileOnContactModel>().projectile;

                                var cartExplosionProjectile = InGame.instance.GetMainFactory().CreateEntityWithBehavior<Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Projectile, ProjectileModel>(explosion);

                                cartExplosionProjectile.Position.X = bloon.Position.X;
                                cartExplosionProjectile.Position.Y = bloon.Position.Y;
                                cartExplosionProjectile.Position.Z = bloon.Position.Z;

                                cartExplosionProjectile.owner = InGame.instance.GetUnityToSimulation().MyPlayerNumber;

                                bloon.Degrade(false, bonnieHero, false);
                            }
                        }
                    }
                }
            }
        }
    }
}

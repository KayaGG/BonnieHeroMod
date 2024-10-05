using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Enums;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using MelonLoader;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Behaviors;
using UnityEngine;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;

namespace BonnieHeroMod;

[HarmonyPatch]
public class BloonstoneCart : ModBloon
{
    public override string BaseBloon => BloonType.Red;
    public override string Name => "BloonstoneCart";
    public override void ModifyBaseBloonModel(BloonModel bloonModel)
    {
        var badImmunity = Game.instance.model.GetBloon("Bad").GetBehavior<BadImmunityModel>().Duplicate();
        bloonModel.display = new Il2CppNinjaKiwi.Common.ResourceUtils.PrefabReference("4fbadff7298bb2a4b9dfe597bb0fd6d1");
        bloonModel.damageDisplayStates = new DamageStateModel[] { };
        bloonModel.tags = new string[] {"NA"};
        bloonModel.leakDamage = 0;
        bloonModel.maxHealth = 4;
        bloonModel.speed = 50f;
        bloonModel.isMoab = true;
        bloonModel.disallowCosmetics = true;
        bloonModel.RemoveAllChildren();
        bloonModel.AddBehavior(badImmunity);
        //bloonModel.GetBehavior<DistributeCashModel>().cash = 50;
        //bloonModel.GetBehavior<CarryProjectileModel>().projectile = farm.GetWeapon().projectile.Duplicate();
    }

    [HarmonyPatch(typeof(BloonManager), nameof(BloonManager.BloonDegrade))]
    public class CartDegradeManager
    {
        [HarmonyPostfix]
        private static void BloonDestroyPostfix(Bloon bloon)
        {
            if (bloon.bloonModel.baseId == ModContent.BloonID<BloonstoneCart>())
            {
                var bonnieHero = InGame.instance.GetTowers().Find(tower => tower.towerModel.baseId == ModContent.TowerID<BonnieHero>());
                if (bonnieHero == null)
                {
                    MelonLogger.Error("bonnie is null");
                    return;
                }
                var towerLogic = bonnieHero.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>();
                var cashProjectile = Game.instance.model.GetTower("BananaFarm", 5).GetDescendant<WeaponModel>().projectile.Duplicate();
                cashProjectile.RemoveBehavior<AgeModel>();

                var cartWorth = 25f;

                for (int i = 0; i < towerLogic.multiplier; i++)
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


                cashProjectile.GetBehavior<CashModel>().minimum = cartWorth;
                cashProjectile.GetBehavior<CashModel>().maximum = cartWorth;

                var ageModel = new AgeModel("BloonstoneCash_AgeModel", 999999f, 999999, true, null);

                cashProjectile.AddBehavior(ageModel);

                var projectile = InGame.instance.GetMainFactory().CreateEntityWithBehavior<Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Projectile, ProjectileModel>(
                    cashProjectile);
                var arriveAtTarget = projectile.GetProjectileBehavior<ArriveAtTarget>();
                const uint dropRange = 20;
                arriveAtTarget.SetStartPosition(new Il2CppAssets.Scripts.Simulation.SMath.Vector3(bloon.Position.X,
                    bloon.Position.Y, 100));

                var targetPos = new Il2CppAssets.Scripts.Simulation.SMath.Vector3(bloon.Position.X + Random.Range(-dropRange, dropRange),
                    bloon.Position.Y + Random.Range(-dropRange, dropRange), 100);
                arriveAtTarget.targetPos = targetPos;

                projectile.Position.X = bloon.Position.X;
                projectile.Position.Y = bloon.Position.Y;
                projectile.Position.Z = 20;

                projectile.direction.X = 0;
                projectile.direction.Y = 0;
                projectile.direction.Z = 0;
                projectile.owner = InGame.instance.GetUnityToSimulation().MyPlayerNumber;
                projectile.target = new Target(targetPos);

                projectile.emittedFrom = new Il2CppAssets.Scripts.Simulation.SMath.Vector3(bloon.Position.X,
                    bloon.Position.Y, 100);

                projectile.EmittedBy = bonnieHero;
                projectile.lifespan = 999999;

                

                //bloon.Destroy();
            }
        }
    }
  

        
        
    [HarmonyPatch(typeof(BloonManager), nameof(BloonManager.BloonSpawned))]
    [HarmonyPrefix]
    private static void Bloon_Spawn(Bloon bloon)
    {
        if (bloon.bloonModel.baseId == ModContent.BloonID<BloonstoneCart>())
        {
            var bonnieHero = InGame.instance.GetTowers().Find(tower => tower.towerModel.baseId == ModContent.TowerID<BonnieHero>());
            if (bonnieHero == null)
            {
                MelonLogger.Error("bonnie is null");
                return;
            }
            var towerLogic = bonnieHero.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>();

            for (int i = 0; i < towerLogic.multiplier; i++)
            {
                switch (i)
                {
                    case < 5:
                        bloon.health += 1;
                        break;
                    case < 10:
                        bloon.health += 2;
                        break;
                    case < 15:
                        bloon.health += 7;
                        break;
                    case < 20:
                        bloon.health += 60;
                        break;
                    case < 25:
                        bloon.health += 120;
                        break;
                    case < 30:
                        bloon.health += 280;
                        break;
                    case < 35:
                        bloon.health += 400;
                        break;
                    case < 40:
                        bloon.health += 600;
                        break;
                }
            }
        }
    }
    [HarmonyPatch(typeof(Bloon), nameof(Bloon.Process))]
    [HarmonyPostfix]
    private static void CartRotation(Bloon __instance)
    {

        if (__instance.bloonModel.baseId == ModContent.BloonID<BloonstoneCart>())
        {
            if (__instance.GetUnityDisplayNode() != null)
            {
                __instance.Rotation += 225;
            }
        }

    }

    //[HarmonyPatch(typeof(ModifyTowerCashModModel), nameof(ModifyTowerCashModModel.))]

    [HarmonyPatch(typeof(InGame), nameof(InGame.RoundStart))]
    [HarmonyPostfix]
    private static void SpawnCarts()
    {
        var bonnieHero = InGame.instance.GetTowers().Find(tower => tower.towerModel.baseId == ModContent.TowerID<BonnieHero>());
        if (bonnieHero != null)
        {
            if (bonnieHero.towerModel.tier > 0)
            {
                InGame.instance.SpawnBloons(ModContent.BloonID<BloonstoneCart>(), 3, 480);
            }
        }
    }
}
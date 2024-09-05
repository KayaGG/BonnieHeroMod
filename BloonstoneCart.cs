using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Enums;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using MelonLoader;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation.SMath;
using UnityEngine;
using Il2CppAssets.Scripts.Simulation.Towers;

namespace BonnieHeroMod;

[HarmonyPatch]
public class BloonstoneCart : ModBloon
{
    public override string BaseBloon => BloonType.Red;
    public override string Name => "BloonstoneCart";
    public override void ModifyBaseBloonModel(BloonModel bloonModel)
    {
        //var farm = Game.instance.model.GetTowerWithName(TowerType.BananaFarm);
        bloonModel.leakDamage = 0;
        bloonModel.maxHealth = 5;
        //bloonModel.GetBehavior<DistributeCashModel>().cash = 50;
        //bloonModel.GetBehavior<CarryProjectileModel>().projectile = farm.GetWeapon().projectile.Duplicate();
    }

    [HarmonyPatch(typeof(BloonManager), nameof(BloonManager.BloonDegrade))]
    [HarmonyPostfix]
    private static void Bloon_Degrade(Bloon bloon)
    {
        MelonLogger.Msg("test");
        if (bloon.bloonModel.baseId == ModContent.BloonID<BloonstoneCart>())
        {
            var bonnieHero = InGame.instance.GetTowers().Find(tower => tower.towerModel.baseId == ModContent.TowerID<BonnieHero>());
            if (bonnieHero == null)
            {
                MelonLogger.Error("bonnie is null");
                return;
            }

            var cashProjectile = Game.instance.model.GetTower("BananaFarm", 5).GetDescendant<WeaponModel>().projectile.Duplicate();
            cashProjectile.RemoveBehavior<AgeModel>();

            cashProjectile.GetBehavior<CashModel>().minimum = 50;
            cashProjectile.GetBehavior<CashModel>().maximum = 50;

            var projectile = InGame.instance.GetMainFactory().CreateEntityWithBehavior<Projectile, ProjectileModel>(
                cashProjectile);
            var arriveAtTarget = projectile.GetProjectileBehavior<ArriveAtTarget>();
            const uint dropRange = 100;
            arriveAtTarget.SetStartPosition(new Il2CppAssets.Scripts.Simulation.SMath.Vector3(bloon.Position.X,
                bloon.Position.Y, 100));

            arriveAtTarget.targetPos = new Il2CppAssets.Scripts.Simulation.SMath.Vector3(bloon.Position.X + Random.Range(-dropRange, dropRange),
                bloon.Position.Y + Random.Range(-dropRange, dropRange), 100);

            projectile.Position.X = bloon.Position.X;
            projectile.Position.Y = bloon.Position.Y;
            projectile.Position.Z = 20;

            projectile.direction.X = 0;
            projectile.direction.Y = 0;
            projectile.direction.Z = 0;
            projectile.owner = InGame.instance.GetUnityToSimulation().MyPlayerNumber;
            projectile.target = new Target(new Il2CppAssets.Scripts.Simulation.SMath.Vector3(bloon.Position.X,
                bloon.Position.Y, 100));

            projectile.emittedFrom = new Il2CppAssets.Scripts.Simulation.SMath.Vector3(bloon.Position.X,
                bloon.Position.Y, 100);

            projectile.EmittedBy = bonnieHero;
            projectile.lifespan = 999999;


            MelonLogger.Msg("projectile spawned");
        }
    }
}
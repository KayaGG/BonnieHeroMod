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

[assembly: MelonInfo(typeof(BonnieHeroMod.BonnieHeroMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BonnieHeroMod;

[HarmonyPatch]
public class BonnieHeroMod : BloonsTD6Mod
{
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

            var projectile = InGame.instance.GetMainFactory().CreateEntityWithBehavior<Projectile, ProjectileModel>(
                cashProjectile);
            var arriveAtTarget = projectile.GetProjectileBehavior<ArriveAtTarget>();
            const uint dropRange = 100;
            arriveAtTarget.SetStartPosition(new Vector3(bloon.Position.X,
                bloon.Position.Y, 100));

            arriveAtTarget.targetPos = new Vector3(bloon.Position.X + Random.Range(-dropRange, dropRange),
                bloon.Position.Y + Random.Range(-dropRange, dropRange), 100);

            projectile.Position.X = bloon.Position.X;
            projectile.Position.Y = bloon.Position.Y;
            projectile.Position.Z = 20;

            projectile.direction.X = 0;
            projectile.direction.Y = 0;
            projectile.direction.Z = 0;
            projectile.owner = InGame.instance.GetUnityToSimulation().MyPlayerNumber;
            projectile.target = new Target(new Vector3(bloon.Position.X,
                bloon.Position.Y, 100));

            projectile.emittedFrom = new Vector3(bloon.Position.X,
                bloon.Position.Y, 100);

            projectile.EmittedBy = bonnieHero;
            projectile.lifespan = 999999;


            MelonLogger.Msg("projectile spawned");
        }
    }
}
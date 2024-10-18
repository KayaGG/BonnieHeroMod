using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.SMath;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using UnityEngine;
using static BTD_Mod_Helper.Api.ModContent;

namespace BonnieHeroMod;

[HarmonyPatch]
public static class Patches
{
    private static void CreateCashProjectile(Tower bonnieHero, Vector3Boxed position, float amount)
    {
        var cashProjectile = Game.instance.model.GetTower("BananaFarm", 5).GetDescendant<WeaponModel>().projectile
            .Duplicate();
        cashProjectile.RemoveBehavior<AgeModel>();

        cashProjectile.GetBehavior<CashModel>().minimum = amount;
        cashProjectile.GetBehavior<CashModel>().maximum = amount;

        var ageModel = new AgeModel("BloonstoneCash_AgeModel", 999999f, 999999, true, null);

        cashProjectile.AddBehavior(ageModel);

        var projectile = InGame.instance.GetMainFactory()
            .CreateEntityWithBehavior<Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Projectile, ProjectileModel>(
                cashProjectile);
        var arriveAtTarget = projectile.GetProjectileBehavior<ArriveAtTarget>();
        const uint dropRange = 20;
        arriveAtTarget.SetStartPosition(new Il2CppAssets.Scripts.Simulation.SMath.Vector3(position.X,
            position.Y, 100));

        var targetPos = new Il2CppAssets.Scripts.Simulation.SMath.Vector3(
            position.X + Random.Range(-dropRange, dropRange),
            position.Y + Random.Range(-dropRange, dropRange), 100);
        arriveAtTarget.targetPos = targetPos;

        projectile.Position.X = position.X;
        projectile.Position.Y = position.Y;
        projectile.Position.Z = 20;

        projectile.Direction.X = 0;
        projectile.Direction.Y = 0;
        projectile.Direction.Z = 0;
        projectile.owner = InGame.instance.GetUnityToSimulation().MyPlayerNumber;
        projectile.target = new Target(targetPos);

        projectile.emittedFrom = new Il2CppAssets.Scripts.Simulation.SMath.Vector3(position.X,
            position.Y, 100);

        projectile.EmittedBy = bonnieHero;
        projectile.Lifespan = 999999;
    }

    [HarmonyPatch(typeof(SupportRemoveFilterOutTag.MutatorTower),
        nameof(SupportRemoveFilterOutTag.MutatorTower.Mutate))]
    [HarmonyPrefix]
    private static bool SupportRemoveFilterOutTag_MutatorTower_Mutate(SupportRemoveFilterOutTag.MutatorTower __instance,
        Model model, ref bool __result)
    {
        if (__instance.id == MutatorName)
        {
            __result = true;
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(Bloon), nameof(Bloon.Degrade))]
    [HarmonyPrefix]
    private static void BloonDestroyPostfix(Bloon __instance)
    {
        if (__instance.bloonModel.baseId != BloonID<BloonstoneCart>()) return;

        var bonnieHero = InGame.instance.GetTowers().Find(tower => tower.towerModel.baseId == TowerID<BonnieHero>());
        if (bonnieHero == null || !bonnieHero.GetBonnieData(out var towerLogic))
        {
            return;
        }

        var cartWorth = 25f;

        for (int i = 0; i < towerLogic.CurrentTier; i++)
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

        CreateCashProjectile(bonnieHero, __instance.Position, cartWorth);
    }

    [HarmonyPatch(typeof(Bloon), nameof(Bloon.OnSpawn))]
    [HarmonyPrefix]
    private static void Bloon_OnSpawn(Bloon __instance)
    {
        if (__instance.bloonModel.baseId == BloonID<BloonstoneCart>())
        {
            var bonnieHero = InGame.instance.GetTowers()
                .Find(tower => tower.towerModel.baseId == TowerID<BonnieHero>());
            if (bonnieHero == null || !bonnieHero.GetBonnieData(out var towerLogic))
            {
                return;
            }

            for (int i = 0; i < towerLogic.CurrentTier; i++)
            {
                switch (i)
                {
                    case < 5:
                        __instance.health += 1;
                        break;
                    case < 10:
                        __instance.health += 2;
                        break;
                    case < 15:
                        __instance.health += 7;
                        break;
                    case < 20:
                        __instance.health += 60;
                        break;
                    case < 25:
                        __instance.health += 120;
                        break;
                    case < 30:
                        __instance.health += 280;
                        break;
                    case < 35:
                        __instance.health += 400;
                        break;
                    case < 40:
                        __instance.health += 600;
                        break;
                }
            }
        }

        if (__instance.bloonModel.baseId == BloonID<BEAST>())
        {
            var bonnieHero = InGame.instance.GetTowers()
                .Find(tower => tower.towerModel.baseId == TowerID<BonnieHero>());
            if (bonnieHero == null)
            {
                return;
            }

            switch (bonnieHero.towerModel.tier)
            {
                case < 16:
                    break;
                case < 20:
                    __instance.health = __instance.bloonModel.maxHealth = 10000;
                    break;
                case 20:
                    __instance.health = __instance.bloonModel.maxHealth = 20000;
                    break;
            }

            __instance.GetBloonBehavior<HealthPercentTrigger>().lowestHealth = __instance.bloonModel.maxHealth;
        }
    }

    [HarmonyPatch(typeof(InGame), nameof(InGame.RoundStart))]
    [HarmonyPostfix]
    private static void InGame_RoundStart()
    {
        var bonnieHero = InGame.instance.GetTowers().Find(tower => tower.towerModel.baseId == TowerID<BonnieHero>());
        if (bonnieHero != null && bonnieHero.towerModel.tier > 0)
        {
            InGame.instance.SpawnBloons(BloonID<BloonstoneCart>(), 3, 480);
        }
    }

    [HarmonyPatch(typeof(Ability), nameof(Ability.Activate))]
    [HarmonyPostfix]
    private static void Ability_Activate(Ability __instance)
    {
        var bonnieHero = InGame.instance.GetTowers()
            .Find(tower => tower.towerModel.baseId == TowerID<BonnieHero>());
        if (bonnieHero != null && __instance.abilityModel.name == "AbilityModel_MassDetonation")
        {
            var bloons = InGame.instance.GetBloons();
            if (bloons != null)
            {
                foreach (var bloon in bloons)
                {
                    if (bloon.bloonModel.baseId == BloonID<BloonstoneCart>())
                    {
                        var attackModel = bonnieHero.towerModel.GetAttackModel();
                        var dynamite = attackModel.weapons[0].projectile;
                        var explosion = dynamite.GetBehavior<CreateProjectileOnContactModel>().projectile;

                        var cartExplosionProjectile = InGame.instance.GetMainFactory()
                            .CreateEntityWithBehavior<
                                Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Projectile, ProjectileModel>(
                                explosion);

                        cartExplosionProjectile.Position.X = bloon.Position.X;
                        cartExplosionProjectile.Position.Y = bloon.Position.Y;
                        cartExplosionProjectile.Position.Z = bloon.Position.Z;

                        cartExplosionProjectile.owner = InGame.instance.GetUnityToSimulation().MyPlayerNumber;

                        bloon.Degrade(false, bonnieHero, false);
                    }
                }
            }
        }

        if (__instance.abilityModel.name == "AbilityModel_BEAST")
        {
            InGame.instance.SpawnBloons(ModContent.BloonID<BEAST>(), 1, 0);
        }
    }

    [HarmonyPatch(typeof(HealthPercentTrigger), nameof(HealthPercentTrigger.Trigger))]
    [HarmonyPostfix]
    public static void HealthPercentTrigger_Trigger(HealthPercentTrigger __instance)
    {
        if (__instance.bloon.bloonModel.baseId == BloonID<BEAST>())
        {
            var bonnieHero = InGame.instance.GetTowers()
                .Find(tower => tower.towerModel.baseId == TowerID<BonnieHero>());
            if (bonnieHero == null)
            {
                return;
            }

            var cashProjectile = Game.instance.model.GetTower("BananaFarm", 5).GetDescendant<WeaponModel>().projectile
                .Duplicate();
            cashProjectile.RemoveBehavior<AgeModel>();

            var worth = 25f;

            switch (bonnieHero.towerModel.tier)
            {
                case < 16:
                    worth = 25f;
                    break;
                case < 20:
                    worth = 50f;
                    break;
                case 20:
                    worth = 100f;
                    break;
            }

            CreateCashProjectile(bonnieHero, __instance.bloon.Position, worth);
        }
    }

    [HarmonyPatch(typeof(Bloon), nameof(Bloon.Process))]
    [HarmonyPostfix]
    private static void Bloon_Process(Bloon __instance)
    {
        if (__instance.bloonModel.baseId == BloonID<BloonstoneCart>() && __instance.GetUnityDisplayNode() != null)
        {
            __instance.Rotation += 225;
        }

        if (__instance.bloonModel.baseId == BloonID<BEAST>())
        {
            var bonnieHero = InGame.instance.GetTowers()
                .Find(tower => tower.towerModel.baseId == TowerID<BonnieHero>());
            if (bonnieHero == null)
            {
                return;
            }

            var collisionState =
                InGame.instance.bridge.Simulation.collisionChecker.GetInRange<Bloon>(__instance.Position.X,
                    __instance.Position.Y, 12);

            if (Mathf.Approximately(__instance.baseScale, 1))
            {
                __instance.baseScale = 0.8f;
            }

            if (__instance.GetUnityDisplayNode() != null)
            {
                __instance.Rotation += 90;
            }

            if (collisionState == null)
                return;

            while (collisionState.MoveNext())
            {
                var bloon = collisionState._Current_k__BackingField;
                if (bloon == null || bloon.bloonModel.baseId == BloonID<BEAST>())
                    continue;

                var stunDuration = 5;
                var moabStunDuration = 2;
                switch (bonnieHero.towerModel.tier)
                {
                    case < 16:
                        break;
                    case < 20:
                        stunDuration = 10;
                        moabStunDuration = 4;
                        break;
                    case 20:
                        stunDuration = 15;
                        moabStunDuration = 6;
                        break;
                }


                var slowMutator = new SlowModel.SlowMutator(0, "Stun:BEASTStun", "SniperStun", true, false, 0);

                if (!bloon.bloonModel.isMoab)
                {
                    bloon.AddMutator(slowMutator, stunDuration * 60);
                }
                else
                {
                    bloon.AddMutator(slowMutator, moabStunDuration * 60);
                }
            }
        }
    }
}
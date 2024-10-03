using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Enums;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Bloons;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2CppAssets.Scripts.Unity;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Simulation.Bloons;
using MelonLoader;
using BTD_Mod_Helper.Api;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Behaviors;
using UnityEngine;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.SMath;
using UnityEngine.UIElements;

namespace BonnieHeroMod
{
    //todo fix embrittlement bug

    [HarmonyPatch]
    public class BEAST : ModBloon
    {

        public override string BaseBloon => BloonType.Moab;
        public override string Name => "BEAST";
        public override void ModifyBaseBloonModel(BloonModel bloonModel)
        {
            var badImmunity = Game.instance.model.GetBloon("Bad").GetBehavior<BadImmunityModel>().Duplicate();

            bloonModel.SetDisplayGUID("6f2f238e52bdbe048b582029f4ea01b7");

            bloonModel.leakDamage = 0;
            bloonModel.speed = 90f;
            bloonModel.maxHealth = 4000;
            bloonModel.disallowCosmetics = true;
            bloonModel.dontShowInSandbox = true;
            //bloonModel.RemoveTag("Moab");
            //bloonModel.RemoveTag("Moabs");
            bloonModel.RemoveAllChildren();
            bloonModel.RemoveBehavior<DamageStateModel>();
            bloonModel.damageDisplayStates = bloonModel.damageDisplayStates.Empty();

            var hpPercTriggerModel = new HealthPercentTriggerModel("HealthPercentTriggerModel_BEAST", true, new float[] { 0.01f }, new string[] { "" }, false);

            bloonModel.AddBehavior(hpPercTriggerModel);
            bloonModel.AddBehavior(badImmunity);
        }

        [HarmonyPatch(typeof(HealthPercentTrigger), nameof(HealthPercentTrigger.Trigger))]
        [HarmonyPostfix]
        public static void BEAST_HPPostfix(HealthPercentTrigger __instance)
        {
            if (__instance.bloon.bloonModel.baseId == ModContent.BloonID<BEAST>())
            {
                var bonnieHero = InGame.instance.GetTowers().Find(tower => tower.towerModel.baseId == ModContent.TowerID<BonnieHero>());
                if (bonnieHero == null)
                {
                    MelonLogger.Error("bonnie is null");
                    return;
                }

                var cashProjectile = Game.instance.model.GetTower("BananaFarm", 5).GetDescendant<WeaponModel>().projectile.Duplicate();
                cashProjectile.RemoveBehavior<AgeModel>();

                var worth = 50f;

                switch (bonnieHero.towerModel.tier)
                {
                    case < 16:
                        worth = 50f;
                        break;
                    case < 20:
                        worth = 100f;
                        break;
                    case 20:
                        worth = 200f;
                        break;
                }

                cashProjectile.GetBehavior<CashModel>().minimum = worth;
                cashProjectile.GetBehavior<CashModel>().maximum = worth;

                var projectile = InGame.instance.GetMainFactory().CreateEntityWithBehavior<Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Projectile, ProjectileModel>(
                    cashProjectile);
                var arriveAtTarget = projectile.GetProjectileBehavior<ArriveAtTarget>();
                const uint dropRange = 20;
                arriveAtTarget.SetStartPosition(new Il2CppAssets.Scripts.Simulation.SMath.Vector3(__instance.bloon.Position.X,
                    __instance.bloon.Position.Y, 100));

                var targetPos = new Il2CppAssets.Scripts.Simulation.SMath.Vector3(__instance.bloon.Position.X + Random.Range(-dropRange, dropRange),
                    __instance.bloon.Position.Y + Random.Range(-dropRange, dropRange), 100);
                arriveAtTarget.targetPos = targetPos;

                projectile.Position.X = __instance.bloon.Position.X;
                projectile.Position.Y = __instance.bloon.Position.Y;
                projectile.Position.Z = 20;

                projectile.direction.X = 0;
                projectile.direction.Y = 0;
                projectile.direction.Z = 0;
                projectile.owner = InGame.instance.GetUnityToSimulation().MyPlayerNumber;
                projectile.target = new Target(targetPos);

                projectile.emittedFrom = new Il2CppAssets.Scripts.Simulation.SMath.Vector3(__instance.bloon.Position.X,
                    __instance.bloon.Position.Y, 100);

                projectile.EmittedBy = bonnieHero;
                projectile.lifespan = 999999;
            }
        }

        [HarmonyPatch(typeof(BloonManager), nameof(BloonManager.BloonSpawned))]
        [HarmonyPrefix]
        private static void Bloon_Spawn(Bloon bloon)
        {
            if (bloon.bloonModel.baseId == ModContent.BloonID<BEAST>())
            {
                var bonnieHero = InGame.instance.GetTowers().Find(tower => tower.towerModel.baseId == ModContent.TowerID<BonnieHero>());
                if (bonnieHero == null)
                {
                    MelonLogger.Error("bonnie is null");
                    return;
                }
                switch (bonnieHero.towerModel.tier)
                {
                    case < 16:
                        break;
                    case < 20:
                        bloon.health = bloon.bloonModel.maxHealth = 10000;
                        break;
                    case 20:
                        bloon.health = bloon.bloonModel.maxHealth = 20000;
                        break;
                }

                bloon.GetBloonBehavior<HealthPercentTrigger>().lowestHealth = bloon.bloonModel.maxHealth;
            }
        }

        [HarmonyPatch(typeof(Bloon), nameof(Bloon.Process))]
        [HarmonyPostfix]
        private static void BEASTStun(Bloon __instance)
        {
            if (__instance.bloonModel.baseId == ModContent.BloonID<BEAST>())
            {
                var bonnieHero = InGame.instance.GetTowers().Find(tower => tower.towerModel.baseId == ModContent.TowerID<BonnieHero>());
                if (bonnieHero == null)
                {
                    MelonLogger.Error("bonnie is null");
                    return;
                }

                var collisionState = InGame.instance.bridge.Simulation.collisionChecker.GetInRange<Bloon>(__instance.Position.X, __instance.Position.Y, 12);

                if (collisionState == null)
                    return;

                while (collisionState.MoveNext())
                {
                    var bloon = collisionState.Current;
                    if (bloon == null || bloon.bloonModel.baseId == BloonID<BEAST>())
                        continue;

                    var stunDuration = 5;
                    var moabStunDuration = 2;
                    switch (bonnieHero.towerModel.tier)
                    {
                        case 16:
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
}

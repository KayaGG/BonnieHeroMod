using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Enums;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Bloons;
using System;
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

namespace BonnieHeroMod
{
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
            bloonModel.maxHealth = 2000;
            bloonModel.disallowCosmetics = true;
            bloonModel.dontShowInSandbox = true;
            //bloonModel.RemoveTag("Moab");
            //bloonModel.RemoveTag("Moabs");
            bloonModel.RemoveAllChildren();
            bloonModel.RemoveBehavior<DamageStateModel>();
            bloonModel.damageDisplayStates = bloonModel.damageDisplayStates.Empty();

            bloonModel.AddBehavior(badImmunity);
        }

        [HarmonyPatch(typeof(Bloon), nameof(Bloon.Damage))]
        [HarmonyPostfix]
        public static void BEASTPostfix(Bloon __instance, double totalAmount, Il2CppAssets.Scripts.Simulation.Towers.Tower tower)
        {
            if (__instance.bloonModel.baseId == ModContent.BloonID<BEAST>())
            {
                MelonLogger.Msg("BEAST Damaged");
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
                    case 16:
                        bloon.health = 5000;
                        break;
                    case 20:
                        bloon.health = 10000;
                        break;
                }
            }
        }
    }
}

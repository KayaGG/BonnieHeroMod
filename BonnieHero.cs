using UnityEngine;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using MelonLoader;
using System.Linq;
using Il2CppAssets.Scripts.Utils;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using BTD_Mod_Helper.Api.Display;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Mods;
using UnityEngine.UI;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu.TowerSelectionMenuThemes;
using Il2CppAssets.Scripts.Unity.Bridge;
using static MelonLoader.MelonLogger;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using static BonnieHeroMod.BonnieHeroMod;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;

namespace BonnieHeroMod;

[HarmonyPatch]
public class BonnieHero : ModHero
{
    public override string BaseTower => TowerType.SpikeFactory;
    public override int Cost => 900;
    public override string DisplayName => "Bonnie";
    public override string Name => "BonnieHero";
    public override string Title => "Bloonstone Miner";
    public override string Description => "A hard working miner who summons carts of Bloonstones for extra cash. Her dynamite makes short work of any Bloon that passes by.";
    public override string Level1Description => "A hard working miner who summons carts of Bloonstones for extra cash. Her dynamite makes short work of any Bloon that passes by.";

    public override string NameStyle => TowerType.Gwendolin;
    public override string BackgroundStyle => TowerType.Gwendolin;
    public override string GlowStyle => TowerType.Gwendolin;

    public override float XpRatio => 1.0f;

    /// <param name="towerModel"></param>
    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
        var quincy = Game.instance.model.GetTowerWithName(TowerType.Quincy);
        var attackModel = towerModel.GetAttackModel();
        var projectile = attackModel.weapons[0].projectile;
        var caltropsBehavior = Game.instance.model.GetTowerFromId("NinjaMonkey-002").GetAttackModel(1).GetBehavior<RotateToTargetModel>().Duplicate();
        var explosion = Game.instance.model.GetTower("BombShooter").GetWeapon().projectile.GetBehavior<
            CreateProjectileOnContactModel>().Duplicate();

        towerModel.mods = quincy.mods;
        towerModel.ApplyDisplay<BonnieDisplay>();
        towerModel.GetBehavior<DisplayModel>().display = towerModel.display;
        towerModel.footprint = quincy.footprint.Duplicate();
        towerModel.radius = quincy.radius;
        towerModel.range = quincy.range;
        towerModel.doesntRotate = false;

        attackModel.range = quincy.range;
        attackModel.weapons[0].rate = 3f;
        attackModel.weapons[0].animation = 4;
        attackModel.AddBehavior(caltropsBehavior);

        projectile.GetDamageModel().damage = 0;
        projectile.GetDamageModel().immuneBloonProperties = Il2Cpp.BloonProperties.None;
        projectile.GetDamageModel().maxDamage = 0;
        projectile.pierce = 1.0f;
        projectile.maxPierce = 1.0f;
        projectile.GetBehavior<AgeModel>().lifespan = 10f;
        projectile.AddBehavior(explosion);

        explosion.projectile.radius = 30f;
        explosion.projectile.SetHitCamo(true);

        projectile.AddBehavior(Game.instance.model.GetTower("MortarMonkey").GetWeapon().projectile.GetBehavior<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExpireModel>().Duplicate());
        var effectOnExpire = projectile.GetBehavior<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExpireModel>();
        var effectOnExhaust = new CreateEffectOnExhaustedModel("CreateEffectOnExhaustedModel_", effectOnExpire.assetId,
            effectOnExpire.lifespan, effectOnExpire.fullscreen, effectOnExpire.randomRotation,
            effectOnExpire.effectModel);
        projectile.AddBehavior(effectOnExhaust);
    }

    [HarmonyPatch(typeof(RangeSupport.MutatorTower), nameof(RangeSupport.MutatorTower.Mutate))]
    internal static class RangeSupport_MutatorTower_Mutate
    {
        [HarmonyPrefix]
        private static bool Prefix(RangeSupport.MutatorTower __instance, Model model, ref bool __result)
        {
            
            if (__instance.id == "MinecartTier")
            {
                __result = true;
                return false;
            }

            return true;
        }
    }

    public class Levels
    {
        public class Level2 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Harmless minecarts appear on the natural bloon track which can be \"popped\" for cash. Upgrade them to make them more valuable, to a maximum of $300 per cart.";
            public override int Level => 2;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                var bankModel = Game.instance.model.GetTowerFromId("BananaFarm-040").GetBehavior<BankModel>().Duplicate();
                BankDepositsModel depoModel = new("BankDepositModel_", 0f, new(), 0);

                //depoModel.depositPercent = 20f;

                bankModel.autoCollect = false;
                bankModel.capacity = 999999f;
                bankModel.interest = 0f;
                towerModel.AddBehavior(bankModel);
                towerModel.AddBehavior(depoModel);

                //towerModel.towerSelectionMenuThemeId = "BananaFarmDeposit";
            }
        }

        public class Level3 : ModHeroLevel<BonnieHero>
        {
            public override string AbilityName => "Mass Detonation";

            public override string AbilityDescription => "Bonnie throws dynamite at every minecart at once, destroying them and any bloons around them.";

            public override string Description => "Mass Detonation - Bonnie throws dynamite at every minecart at once, destroying them and any bloons around them.";
            public override int Level => 3;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                //var abilityModel = new AbilityModel("MassDetonation", AbilityName, AbilityDescription, null, null, null, );
            }
        }

        public class Level4 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Longer fuses allow dynamite to sit on the track longer before exploding. Bonnie throws them more frequently.";
            public override int Level => 4;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                var attackModel = towerModel.GetAttackModel();
                var projectile = attackModel.weapons[0].projectile;

                attackModel.weapons[0].rate = 2f;

                projectile.GetBehavior<AgeModel>().lifespan = 20f;
            }
        }

        public class Level5 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Minecarts can now be upgraded to $800 per cart.";
            public override int Level => 5;
            public override void ApplyUpgrade(TowerModel towerModel)
            {   

            }
        }

        public class Level6 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Higher power explosives allow all dynamite explosions to deal more damage.";
            public override int Level => 6;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                var attackModel = towerModel.GetAttackModel();
                var projectile = attackModel.weapons[0].projectile;

                projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage = 2f;
            }
        }

        public class Level7 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Dynamite explosions have increased pierce and blast radius and can damage any bloon type.";
            public override int Level => 7;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                var attackModel = towerModel.GetAttackModel();
                var projectile = attackModel.weapons[0].projectile;

                projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.pierce = 30f;
                projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.radius = 28f;
                projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().immuneBloonProperties = Il2Cpp.BloonProperties.None;
            }
        }

        public class Level8 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Dynamite stacks have longer fuses and minecarts can be upgraded to $1,600 per cart.";
            public override int Level => 8;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                var attackModel = towerModel.GetAttackModel();
                var projectile = attackModel.weapons[0].projectile;


                projectile.GetBehavior<AgeModel>().lifespan = 40f;
            }
        }

        public class Level9 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Highly potent explosives allow all dynamite explosions to deal even more damage.";
            public override int Level => 9;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                var attackModel = towerModel.GetAttackModel();
                var projectile = attackModel.weapons[0].projectile;

                projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage = 4f;
            }
        }

        public class Level10 : ModHeroLevel<BonnieHero>
        {
            public override string AbilityName => "B.E.A.S.T";

            public override string AbilityDescription => "Calls forth the Bloonstone Excavation And Supply Truck which stuns bloons by running them over. Continually drops bloonstones as it takes damage from monkey attacks.";

            public override string Description => "B.E.A.S.T. - Calls forth the Bloonstone Excavation And Supply Truck which stuns bloons by running them over. Continually drops bloonstones as it takes damage from monkey attacks.";
            public override int Level => 10;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level11 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Minecarts can now be upgraded to $3,000 per cart.";
            public override int Level => 11;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level12 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Special formula dynamite deals 4x damage to MOAB-class bloons.";
            public override int Level => 12;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                var attackModel = towerModel.GetAttackModel();
                var projectile = attackModel.weapons[0].projectile;
                var moabMod = new DamageModifierForTagModel("DE_BonusMoabDamage", "Moabs", 4, 0, false, true);

                projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.AddBehavior(moabMod);
            }
        }

        public class Level13 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Mass Detonation cooldown is reduced and dynamite fuse time is increased.";
            public override int Level => 13;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                var attackModel = towerModel.GetAttackModel();
                var projectile = attackModel.weapons[0].projectile;

                projectile.GetBehavior<AgeModel>().lifespan = 80f;
            }
        }

        public class Level14 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Minecarts can now be upgraded to $5,000 per cart.";
            public override int Level => 14;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level15 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Ultra powerful explosives allow all dynamite explosions to deal even more damage.";
            public override int Level => 15;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                var attackModel = towerModel.GetAttackModel();
                var projectile = attackModel.weapons[0].projectile;

                projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage = 10f;
            }
        }

        public class Level16 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "The B.E.A.S.T. carries more valuable bloonstones and stuns bloons for longer.";
            public override int Level => 16;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level17 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Minecarts can now be upgraded to $8,000 per cart.";
            public override int Level => 17;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level18 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Increased payload gives all of Bonnie's dynamite attacks higher pierce and a larger blast radius.";
            public override int Level => 18;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                var attackModel = towerModel.GetAttackModel();
                var projectile = attackModel.weapons[0].projectile;

                projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.pierce = 100f;
                projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.radius = 38f;
            }
        }

        public class Level19 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Supercharged explosives allow all dynamite explosions to deal maximum damage.";
            public override int Level => 19;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                var attackModel = towerModel.GetAttackModel();
                var projectile = attackModel.weapons[0].projectile;

                projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage = 60f;
            }
        }

        public class Level20 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "The B.E.A.S.T. carries even more valuable bloonstones and stuns bloons for even longer. Minecarts can be upgraded to their maximum value of $12,000 per cart.";
            public override int Level => 20;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }
    }
}

class BonnieDisplay : ModDisplay
{
    public override string BaseDisplay => "7d6006081e3422941b90d122aefeb79e";
    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        node.RemoveBone("MonkeyJnt_Jetpack");
        node.RemoveBone("MonkeyRig:Propjectile_L");
        node.RemoveBone("MonkeyRig:Propjectile_R");
    }
}
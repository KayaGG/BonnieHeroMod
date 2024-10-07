using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using BTD_Mod_Helper.Api.Display;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models;

namespace BonnieHeroMod;

[HarmonyPatch]
public class BonnieHero : ModHero
{
    public override string BaseTower => TowerType.SpikeFactory;
    public override int Cost => 700;
    public override string DisplayName => "Bonnie";
    public override string Name => "BonnieHero";
    public override string Title => "Bloonstone Miner";
    public override string Description => "A hard working miner who summons carts of Bloonstones for extra cash. Her dynamite makes short work of any Bloon that passes by.";
    public override string Level1Description => "A hard working miner who summons carts of Bloonstones for extra cash. Her dynamite makes short work of any Bloon that passes by.";
    public override string Icon => "BonnieHero-Icon";
    public override string Portrait => "BonnieHero-Portrait";
    public override string Square => "BonnieHero-Square";
    public override string Button => "BonnieHero-Button";

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
        var explosion = Game.instance.model.GetTower("BombShooter").GetWeapon().projectile.GetBehavior<CreateProjectileOnContactModel>().Duplicate();
        var explosionSound = Game.instance.model.GetTower("BombShooter").GetWeapon().projectile.GetBehavior<CreateSoundOnProjectileCollisionModel>().Duplicate();



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
        projectile.RemoveBehavior<FadeProjectileModel>();
        projectile.AddBehavior(explosion);

        explosion.projectile.radius = 30f;
        explosion.projectile.SetHitCamo(true);
        projectile.AddBehavior(explosionSound);

        projectile.AddBehavior(Game.instance.model.GetTower("MortarMonkey").GetWeapon().projectile.GetBehavior<CreateEffectOnExpireModel>().Duplicate());
        projectile.display = new PrefabReference("4bce3e766a25dc74085e2427d1db6160");
        projectile.RemoveBehavior<SetSpriteFromPierceModel>();
        var effectOnExpire = projectile.GetBehavior<CreateEffectOnExpireModel>();
        var effectOnExhaust = new CreateEffectOnExhaustedModel("CreateEffectOnExhaustedModel_", effectOnExpire.assetId,
            effectOnExpire.lifespan, effectOnExpire.fullscreen, effectOnExpire.randomRotation,
            effectOnExpire.effectModel);
        explosion.projectile.AddBehavior(effectOnExhaust);
    }


    public class Levels
    {
        public class Level2 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Harmless minecarts appear on the natural bloon track which can be \"popped\" for cash. Upgrade them to make them more valuable, to a maximum of $300 per cart.";
            public override int Level => 2;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                /*var bankModel = Game.instance.model.GetTowerFromId("BananaFarm-040").GetBehavior<BankModel>().Duplicate();
                BankDepositsModel depoModel = new("BankDepositModel_", 0f, new(), 0);

                //depoModel.depositPercent = 20f;

                bankModel.autoCollect = false;
                bankModel.capacity = 999999f;
                bankModel.interest = 0f;
                towerModel.AddBehavior(bankModel);
                towerModel.AddBehavior(depoModel);*/

                //towerModel.towerSelectionMenuThemeId = "BananaFarmDeposit";
            }
        }

        public class Level3 : ModHeroLevel<BonnieHero>
        {
            public override string AbilityName => "Mass Detonation";

            public override string AbilityDescription => "Bonnie throws dynamite at every minecart at once, destroying them and any bloons around them.";

            public override string Portrait => "BonnieHeroL3-Portrait";

            public override string Description => "Mass Detonation - Bonnie throws dynamite at every minecart at once, destroying them and any bloons around them.";
            public override int Level => 3;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                //var abilityModel = new AbilityModel("MassDetonation", AbilityName, AbilityDescription, null, null, null, 30f, );
                //var attackModel = towerModel.GetAttackModel().Duplicate();

                /*var quincy = Game.instance.model.GetTowerWithName(TowerType.Quincy + " 3");
                var abilityModel = quincy.GetAbility().Duplicate();
                var subAbilityModel = Game.instance.model.GetTowerWithName("MonkeySub-040").GetAbility().Duplicate();
                var activateAttackModel = subAbilityModel.GetBehavior<ActivateAttackModel>().Duplicate();
                var turbo = abilityModel.GetBehavior<TurboModel>();

                abilityModel.name = "AbilityModel_MassDetonation";
                abilityModel.displayName = AbilityName;
                abilityModel.canActivateBetweenRounds = false;
                abilityModel.RemoveBehavior<CreateEffectOnAbilityModel>();
                abilityModel.RemoveBehavior<TurboModel>();
                abilityModel.icon = GetSpriteReference("MassDetonation");

                activateAttackModel.attacks.RemoveItem(activateAttackModel.attacks.First());
                activateAttackModel.cancelIfNoTargets = false;
                activateAttackModel.attacks.AddItem(attackModel);
                activateAttackModel.attacks[0].targetProvider = attackModel.targetProvider;
                activateAttackModel.attacks[0].range = attackModel.range;
                activateAttackModel.attacks[0].weapons = attackModel.weapons.Duplicate();
                
                abilityModel.AddBehavior(activateAttackModel);*/

                var abilityModel = Game.instance.model.GetTowerWithName("SuperMonkey-040").GetAbility().Duplicate();
                var activateAttackModel = abilityModel.GetBehavior<ActivateAttackModel>();
                var subAbilitySound = Game.instance.model.GetTowerWithName("MonkeySub-040").GetAbility().GetBehavior<CreateSoundOnAbilityModel>().sound;

                abilityModel.name = "AbilityModel_MassDetonation";
                abilityModel.displayName = AbilityName;
                abilityModel.canActivateBetweenRounds = false;
                abilityModel.icon = GetSpriteReference("MassDetonation");
                abilityModel.cooldown = 60f;
                abilityModel.Cooldown = 60f;
                abilityModel.RemoveBehavior<CreateEffectOnAbilityModel>();
                abilityModel.GetBehavior<CreateSoundOnAbilityModel>().sound = subAbilitySound;

                activateAttackModel.turnOffExisting = false;
                activateAttackModel.attacks[0] = towerModel.GetAttackModel().Duplicate();
                for (int i = 1; i <= 7; i++)
                {
                    activateAttackModel.attacks[0].AddWeapon(towerModel.GetAttackModel().weapons[0].Duplicate());
                }

                towerModel.AddBehavior(abilityModel);

                
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

                var abilityModel = towerModel.GetAbility();
                var activateAttackModel = abilityModel.GetBehavior<ActivateAttackModel>();

                for (int i = 0; i < activateAttackModel.attacks[0].weapons.Count; i++)
                {
                        activateAttackModel.attacks[0].SetWeapon(towerModel.GetAttackModel().weapons[0], i);
                }
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

                var abilityModel = towerModel.GetAbility();
                var activateAttackModel = abilityModel.GetBehavior<ActivateAttackModel>();

                for (int i = 0; i < activateAttackModel.attacks[0].weapons.Count; i++)
                {
                    activateAttackModel.attacks[0].SetWeapon(towerModel.GetAttackModel().weapons[0], i);
                }
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

                var abilityModel = towerModel.GetAbility();
                var activateAttackModel = abilityModel.GetBehavior<ActivateAttackModel>();

                for (int i = 0; i < activateAttackModel.attacks[0].weapons.Count; i++)
                {
                    activateAttackModel.attacks[0].SetWeapon(towerModel.GetAttackModel().weapons[0], i);
                }
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

                var abilityModel = towerModel.GetAbility();
                var activateAttackModel = abilityModel.GetBehavior<ActivateAttackModel>();

                for (int i = 0; i < activateAttackModel.attacks[0].weapons.Count; i++)
                {
                    activateAttackModel.attacks[0].SetWeapon(towerModel.GetAttackModel().weapons[0], i);
                }
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

                projectile.GetBehavior<AgeModel>().rounds = 10;

                var abilityModel = towerModel.GetAbility();
                var activateAttackModel = abilityModel.GetBehavior<ActivateAttackModel>();

                for (int i = 0; i < activateAttackModel.attacks[0].weapons.Count; i++)
                {
                    activateAttackModel.attacks[0].SetWeapon(towerModel.GetAttackModel().weapons[0], i);
                }
            }
        }

        public class Level10 : ModHeroLevel<BonnieHero>
        {
            public override string AbilityName => "B.E.A.S.T";

            public override string AbilityDescription => "Calls forth the Bloonstone Excavation And Supply Truck which stuns bloons by running them over. Continually drops bloonstones as it takes damage from monkey attacks.";

            public override string Portrait => "BonnieHeroL10-Portrait";

            public override string Description => "B.E.A.S.T. - Calls forth the Bloonstone Excavation And Supply Truck which stuns bloons by running them over. Continually drops bloonstones as it takes damage from monkey attacks.";
            public override int Level => 10;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                var quincy = Game.instance.model.GetTowerWithName(TowerType.Quincy + " 3");
                var homeland = Game.instance.model.GetTowerWithName("MonkeyVillage-050").GetAbility().GetBehavior<CreateSoundOnAbilityModel>();
                var abilityModel = quincy.GetAbility().Duplicate();


                abilityModel.name = "AbilityModel_BEAST";
                abilityModel.displayName = AbilityName;
                abilityModel.canActivateBetweenRounds = false;
                abilityModel.RemoveBehavior<TurboModel>();
                abilityModel.RemoveBehavior<CreateEffectOnAbilityModel>();
                abilityModel.RemoveBehavior<CreateSoundOnAbilityModel>();
                abilityModel.AddBehavior(homeland);
                abilityModel.icon = GetSpriteReference("BEAST");

                towerModel.AddBehavior(abilityModel);
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

                var abilityModel = towerModel.GetAbility();
                var activateAttackModel = abilityModel.GetBehavior<ActivateAttackModel>();

                for (int i = 0; i < activateAttackModel.attacks[0].weapons.Count; i++)
                {
                    activateAttackModel.attacks[0].SetWeapon(towerModel.GetAttackModel().weapons[0], i);
                }
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

                var abilityModel = towerModel.GetAbility();
                abilityModel.cooldown = 50f;
                abilityModel.Cooldown = 50f;
                var activateAttackModel = abilityModel.GetBehavior<ActivateAttackModel>();

                for (int i = 0; i < activateAttackModel.attacks[0].weapons.Count; i++)
                {
                    activateAttackModel.attacks[0].SetWeapon(towerModel.GetAttackModel().weapons[0], i);
                }
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

                var abilityModel = towerModel.GetAbility();
                var activateAttackModel = abilityModel.GetBehavior<ActivateAttackModel>();

                for (int i = 0; i < activateAttackModel.attacks[0].weapons.Count; i++)
                {
                    activateAttackModel.attacks[0].SetWeapon(towerModel.GetAttackModel().weapons[0], i);
                }
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

                var abilityModel = towerModel.GetAbility();
                var activateAttackModel = abilityModel.GetBehavior<ActivateAttackModel>();

                for (int i = 0; i < activateAttackModel.attacks[0].weapons.Count; i++)
                {
                    activateAttackModel.attacks[0].SetWeapon(towerModel.GetAttackModel().weapons[0], i);
                }
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

                var abilityModel = towerModel.GetAbility();
                var activateAttackModel = abilityModel.GetBehavior<ActivateAttackModel>();

                for (int i = 0; i < activateAttackModel.attacks[0].weapons.Count; i++)
                {
                    activateAttackModel.attacks[0].SetWeapon(towerModel.GetAttackModel().weapons[0], i);
                }
            }
        }

        public class Level20 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "The B.E.A.S.T. carries even more valuable bloonstones and stuns bloons for even longer. Minecarts can be upgraded to their maximum value of $12,000 per cart.";
            public override string Portrait => "BonnieHeroL20-Portrait";
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
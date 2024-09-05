using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity;

namespace BonnieHeroMod;

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
        var gwen = Game.instance.model.GetTowerWithName(TowerType.Gwendolin);
        var attackModel = towerModel.GetAttackModel();
        var projectile = attackModel.weapons[0].projectile;
        var explosion = Game.instance.model.GetTower("BombShooter").GetWeapon().projectile.GetBehavior<
            CreateProjectileOnContactModel>().Duplicate();


        towerModel.mods = quincy.mods;
        towerModel.display = gwen.display;
        towerModel.footprint = quincy.footprint.Duplicate();
        towerModel.radius = quincy.radius;
        towerModel.range = quincy.range;
        towerModel.doesntRotate = false;


        attackModel.range = quincy.range;
        attackModel.weapons[0].rate = 2.5f;

        projectile.GetDamageModel().damage = 1;
        projectile.pierce = 1.0f;
        projectile.maxPierce = 1.0f;
        projectile.AddBehavior(explosion);

        explosion.projectile.radius = 30f;

        projectile.AddBehavior(Game.instance.model.GetTower("MortarMonkey").GetWeapon().projectile.GetBehavior<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExpireModel>().Duplicate());
        var effectOnExpire = projectile.GetBehavior<Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExpireModel>();
        var effectOnExhaust = new CreateEffectOnExhaustedModel("CreateEffectOnExhaustedModel_", effectOnExpire.assetId,
            effectOnExpire.lifespan, effectOnExpire.fullscreen, effectOnExpire.randomRotation,
            effectOnExpire.effectModel);
        projectile.AddBehavior(effectOnExhaust);


    }

    public class Levels
    {
        public class Level2 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Harmless minecarts appear on the natural bloon track which can be \"popped\" for cash. Upgrade them to make them more valuable, to a maximum of $300 per cart.";
            public override int Level => 2;
            public override void ApplyUpgrade(TowerModel towerModel)
            {



                //towerModel.RemoveBehaviors<NecromancerZoneModel;
                /*var summon = Game.instance.model.GetTowerFromId("WizardMonkey-004").GetAttackModel(2).Duplicate();
            var agemodel = Game.instance.model.GetTowerFromId("SpikeFactory").GetAttackModel().weapons[0].projectile.GetBehavior<AgeModel>().Duplicate();

            summon.weapons[0].projectile.name = "AttackModel_Summon3_";
            summon.weapons[0].emission = new NecromancerEmissionModel("CartDeploy_", 1, 1, 1, 1, 1, 1, 0, null, null, null, 1, 1, 1, 1, 2);
            summon.weapons[0].rate = 6f;
            summon.weapons[0].projectile.GetBehavior<TravelAlongPathModel>().disableRotateWithPathDirection = false;
            summon.name = "AttackModel_Summon_";
            summon.weapons[0].projectile.GetDamageModel().damage = 1;
            summon.weapons[0].projectile.pierce = 999999f;
            summon.range = 999999;
            summon.weapons[0].projectile.GetBehavior<TravelAlongPathModel>().lifespanFrames = 0;
            summon.weapons[0].projectile.GetBehavior<TravelAlongPathModel>().lifespan = 999999f;


            agemodel.lifespanFrames = 0;
            agemodel.lifespan = 999999f;
            agemodel.rounds = 9999;

            summon.weapons[0].projectile.AddBehavior(agemodel);

            towerModel.AddBehavior(summon);*/
            }
            //public override void On
        }

        public class Level3 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Description";
            public override int Level => 3;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                    
            }
        }

        public class Level4 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Description";
            public override int Level => 4;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level5 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Description";
            public override int Level => 5;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level6 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Description";
            public override int Level => 6;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level7 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Description";
            public override int Level => 7;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level8 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Description";
            public override int Level => 8;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level9 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Description";
            public override int Level => 9;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level10 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Description";
            public override int Level => 10;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level11 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Description";
            public override int Level => 11;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level12 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Description";
            public override int Level => 12;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level13 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Description";
            public override int Level => 13;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level14 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Description";
            public override int Level => 14;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level15 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Description";
            public override int Level => 15;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level16 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Description";
            public override int Level => 16;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level17 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Description";
            public override int Level => 17;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level18 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Description";
            public override int Level => 18;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level19 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Description";
            public override int Level => 19;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }

        public class Level20 : ModHeroLevel<BonnieHero>
        {
            public override string Description => "Description";
            public override int Level => 20;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

            }
        }
    }
}
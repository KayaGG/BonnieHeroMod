using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Enums;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Unity;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Simulation.Bloons;
using MelonLoader;
using BTD_Mod_Helper.Api;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Behaviors;
using UnityEngine;
using Il2CppAssets.Scripts.Simulation.Towers;
using BloonType = BTD_Mod_Helper.Api.Enums.BloonType;

namespace BonnieHeroMod;

//todo fix embrittlement bug

public class BEAST : ModBloon
{
    public override string BaseBloon => BloonType.Moab;
    public override string Name => "BEAST";

    public override void ModifyBaseBloonModel(BloonModel bloonModel)
    {
        var badImmunity = Game.instance.model.GetBloon("Bad").GetBehavior<BadImmunityModel>().Duplicate();

        bloonModel.SetDisplayGUID("cf38e0e2f5778e742a65aa7d854e3b15");

        bloonModel.leakDamage = 0;
        bloonModel.speed = 90f;
        bloonModel.maxHealth = 4000;
        bloonModel.disallowCosmetics = true;
        bloonModel.dontShowInSandbox = true;

        bloonModel.RemoveTag("Moab");
        bloonModel.RemoveTag("Moabs");

        bloonModel.RemoveAllChildren();
        bloonModel.RemoveBehavior<DamageStateModel>();
        bloonModel.damageDisplayStates = bloonModel.damageDisplayStates.Empty();

        var hpPercTriggerModel = new HealthPercentTriggerModel("HealthPercentTriggerModel_BEAST", true,
            new float[] { 0.01f }, new string[] { "" }, false);

        bloonModel.AddBehavior(hpPercTriggerModel);
        bloonModel.AddBehavior(badImmunity);
    }
}
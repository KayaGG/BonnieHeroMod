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
    }
}
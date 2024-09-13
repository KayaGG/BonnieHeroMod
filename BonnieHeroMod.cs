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
using Il2CppAssets.Scripts.Simulation.Track.RoundManagers;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu.TowerSelectionMenuThemes;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;

[assembly: MelonInfo(typeof(BonnieHeroMod.BonnieHeroMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BonnieHeroMod;

[HarmonyPatch]
public class BonnieHeroMod : BloonsTD6Mod
{
    public override void OnTowerUpgraded(Tower tower, string upgradeName, TowerModel newBaseTowerModel)
    {
        base.OnTowerUpgraded(tower, upgradeName, newBaseTowerModel);
    }

    public override void OnRoundStart()
    {
        base.OnRoundStart();
    }
}

//Code by doombubbles
[HarmonyPatch(typeof(TSMThemeBananaFarm), nameof(TSMThemeBananaFarm.UpdateFromSimInfo))]
internal static class TSMThemeBananaFarm_UpdateFromSimInfo
{
    [HarmonyPrefix]
    internal static void Prefix(TowerToSimulation tower, ref int __state)
    {
        __state = tower.Def.tiers[1];
        tower.Def.tiers[1] = 4;
    }

    [HarmonyPostfix]
    internal static void Postfix(TSMThemeBananaFarm __instance, TowerToSimulation tower, ref int __state)
    {
        tower.Def.tiers[1] = __state;
    }
}
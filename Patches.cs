using HarmonyLib;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;

namespace BonnieHeroMod;

[HarmonyPatch]
public class Patches
{
    [HarmonyPatch(typeof(SupportRemoveFilterOutTag.MutatorTower), nameof(SupportRemoveFilterOutTag.MutatorTower.Mutate))]
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



}
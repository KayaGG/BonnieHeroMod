using System;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;

namespace BonnieHeroMod;

public struct BonnieData()
{
    public static int GetMaxTier(int tier)
    {
        return tier switch
        {
            < 5 => 10,
            < 8 => 15,
            < 11 => 20,
            < 14 => 25,
            < 17 => 30,
            < 20 => 35,
            >= 20 => 40
        };
    }


    public float CurrentTier { get; set; } = 0;
    public float SellAmount { get; set; } = 0;

    public static BonnieData Parse(string json)
    {
        return System.Text.Json.JsonSerializer.Deserialize<BonnieData>(json);
    }

    public string ToJson()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;

namespace BonnieHeroMod;

public struct BonnieData
{
    public BonnieData()
    {
    }

    public float MaxTier { get; set; } = 10;
    public float CurrentTier { get; set; } = 0;
    public float Bank { get; set; } = 0;

    public static BonnieData Parse(string json)
    {
        return System.Text.Json.JsonSerializer.Deserialize<BonnieData>(json);
    }

    public string ToJson()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}
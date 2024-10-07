using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;

namespace BonnieHeroMod;

public record BonnieData
{
    public float MaxTier { get; set; } = 10;
    public float CurrentTier { get; set; }
    public float Bank { get; set; }

    public static BonnieData Parse(string json)
    {
        return System.Text.Json.JsonSerializer.Deserialize<BonnieData>(json);
    }

    public string ToJson()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}
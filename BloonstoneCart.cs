using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Enums;
using Il2CppAssets.Scripts.Models.Bloons;

namespace BonnieHeroMod;

public class BloonstoneCart : ModBloon
{
    /// <inheritdoc />
    public override void ModifyBaseBloonModel(BloonModel bloonModel)
    {

    }

    /// <inheritdoc />
    public override string BaseBloon => BloonType.Ceramic;
}
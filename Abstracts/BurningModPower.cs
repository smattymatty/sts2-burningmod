#nullable enable
using BaseLib.Abstracts;

namespace BurningMod;

public abstract class BurningModPower : CustomPowerModel
{
    private const string PlaceholderPath = "res://BurningMod/images/powers/placeholder.png";

    public override string? CustomPackedIconPath => PlaceholderPath;
    public override string? CustomBigIconPath => PlaceholderPath;
    public override string? CustomBigBetaIconPath => PlaceholderPath;
}

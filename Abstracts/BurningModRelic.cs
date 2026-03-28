#nullable enable
using BaseLib.Abstracts;

namespace BurningMod;

public abstract class BurningModRelic : CustomRelicModel
{
    private const string PlaceholderPath = "res://BurningMod/images/relics/placeholder.png";

    public override string PackedIconPath => PlaceholderPath;
    protected override string BigIconPath => PlaceholderPath;
    protected override string PackedIconOutlinePath => PlaceholderPath;

    protected BurningModRelic(bool autoAdd = true) : base(autoAdd) { }
}

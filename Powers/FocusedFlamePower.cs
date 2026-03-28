#nullable enable
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Abstracts;

namespace BurningMod;

public class FocusedFlamePower : TemporaryFocusPower, ICustomPower
{
    public override AbstractModel OriginModel => ModelDb.Card<FocusedFlame>();

    public string? CustomPackedIconPath => "res://BurningMod/images/powers/focused_flame_power.png";
    public string? CustomBigIconPath => "res://BurningMod/images/powers/focused_flame_power.png";
    public string? CustomBigBetaIconPath => "res://BurningMod/images/powers/focused_flame_power.png";
}

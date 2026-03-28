#nullable enable
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Abstracts;

namespace BurningMod;

public class ScorchSelfFocusPower : TemporaryFocusPower, ICustomPower
{
    public override AbstractModel OriginModel => ModelDb.Card<ScorchSelf>();

    public string? CustomPackedIconPath => "res://BurningMod/images/powers/scorch_self_power.png";
    public string? CustomBigIconPath => "res://BurningMod/images/powers/scorch_self_power.png";
    public string? CustomBigBetaIconPath => "res://BurningMod/images/powers/scorch_self_power.png";
}

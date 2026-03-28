#nullable enable
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Abstracts;

namespace BurningMod;

public class FireBrandPower : BurningModPower
{
    public override string? CustomPackedIconPath => "res://BurningMod/images/powers/fire_brand_power.png";
    public override string? CustomBigIconPath => "res://BurningMod/images/powers/fire_brand_power.png";
    public override string? CustomBigBetaIconPath => "res://BurningMod/images/powers/fire_brand_power.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is not VulnerablePower) return;
        if (power.Owner == base.Owner) return;
        if (amount <= 0) return;

        Flash();
        await BurnApplyHelper.ApplyBurn(power.Owner, base.Amount, base.Owner, null);
    }
}

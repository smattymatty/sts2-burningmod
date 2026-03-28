using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Abstracts;

#nullable enable
namespace BurningMod;

public sealed class CausticFlamePower : BurningModPower
{
    public override string? CustomPackedIconPath => "res://BurningMod/images/powers/caustic_flame_power.png";
    public override string? CustomBigIconPath => "res://BurningMod/images/powers/caustic_flame_power.png";
    public override string? CustomBigBetaIconPath => "res://BurningMod/images/powers/caustic_flame_power.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is not BurnPower) return;
        if (amount <= 0m) return;
        if (applier != base.Owner) return;
        if (power.Owner == base.Owner) return; // don't trigger on self-burn
        Flash();
        await PowerCmd.Apply<PoisonPower>(power.Owner, base.Amount, base.Owner, cardSource);
    }
}

#nullable enable
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;

namespace BurningMod;

public class SmolderingRagePower : BurningModPower
{
    public override string? CustomPackedIconPath => "res://BurningMod/images/powers/smoldering_rage_power.png";
    public override string? CustomBigIconPath => "res://BurningMod/images/powers/smoldering_rage_power.png";
    public override string? CustomBigBetaIconPath => "res://BurningMod/images/powers/smoldering_rage_power.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is not BurnPower) return;
        if (power.Owner != base.Owner) return;
        if (amount <= 0) return;

        int strengthAmount = base.Owner.GetPowerAmount<StrengthPower>();
        if (strengthAmount <= 0) return;

        Flash();
        foreach (Creature enemy in base.Owner.CombatState!.GetOpponentsOf(base.Owner))
        {
            if (enemy.IsAlive)
                await BurnApplyHelper.ApplyBurn(enemy, strengthAmount, base.Owner, null);
        }
    }
}

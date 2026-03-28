#nullable enable
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;

namespace BurningMod;

public class FireDipPower : BurningModPower
{
    public override string? CustomPackedIconPath => "res://BurningMod/images/powers/fire_dip_power.png";
    public override string? CustomBigIconPath => "res://BurningMod/images/powers/fire_dip_power.png";
    public override string? CustomBigBetaIconPath => "res://BurningMod/images/powers/fire_dip_power.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer != base.Owner) return;
        if (props.HasFlag(ValueProp.Unpowered)) return;
        if (result.UnblockedDamage <= 0) return;
        Flash();
        await BurnApplyHelper.ApplyBurn(target, base.Amount, base.Owner, null);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == base.Owner.Side)
            await PowerCmd.Remove(this);
    }
}

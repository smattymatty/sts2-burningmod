#nullable enable
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;

namespace BurningMod;

public class KindlePower : BurningModPower
{
    public override string? CustomPackedIconPath => "res://BurningMod/images/powers/kindle_power.png";
    public override string? CustomBigIconPath => "res://BurningMod/images/powers/kindle_power.png";
    public override string? CustomBigBetaIconPath => "res://BurningMod/images/powers/kindle_power.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult _, ValueProp props, Creature? dealer, CardModel? __)
    {
        if (target != base.Owner) return;
        if (dealer == null) return;

        await BurnApplyHelper.ApplyBurn(dealer, base.Amount, base.Owner, null);
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (base.Owner.Side != side) return;
        await PowerCmd.Remove(this);
    }
}

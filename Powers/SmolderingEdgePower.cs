#nullable enable
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;

namespace BurningMod;

public sealed class SmolderingEdgePower : BurningModPower
{
    public override string? CustomPackedIconPath => "res://BurningMod/images/powers/smoldering_edge_power.png";
    public override string? CustomBigIconPath => "res://BurningMod/images/powers/smoldering_edge_power.png";
    public override string? CustomBigBetaIconPath => "res://BurningMod/images/powers/smoldering_edge_power.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _bonusDamageThisTurn = 0;

    public override int DisplayAmount => _bonusDamageThisTurn;

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side != base.Owner.Side) return Task.CompletedTask;
        _bonusDamageThisTurn = 0;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is not BurnPower) return Task.CompletedTask;
        if (amount <= 0m) return Task.CompletedTask;
        if (applier != base.Owner) return Task.CompletedTask;
        _bonusDamageThisTurn += base.Amount;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != base.Owner) return 0m;
        if (cardSource == null || !cardSource.Tags.Contains(CardTag.Shiv)) return 0m;
        return _bonusDamageThisTurn;
    }
}

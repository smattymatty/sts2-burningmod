using System.Threading.Tasks;
using Godot;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BurningMod;

public class BurnPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "res://BurningMod/images/powers/burn_power.png";
    public override string? CustomBigIconPath => "res://BurningMod/images/powers/burn_power.png";
    public override string? CustomBigBetaIconPath => "res://BurningMod/images/powers/burn_power.png";
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

    public bool EternalFlameActive { get; set; } = false;

    public int CalculateTotalDamageNextTurn()
    {
        return base.Amount;
    }


    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != base.Owner.Side) return;
        await CreatureCmd.Damage(choiceContext, base.Owner, base.Amount, ValueProp.Unpowered, null, null);
        if (base.Owner.IsAlive && !EternalFlameActive)
            await PowerCmd.Decrement(this);
    }
}

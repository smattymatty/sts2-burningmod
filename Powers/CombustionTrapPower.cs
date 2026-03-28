#nullable enable
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Abstracts;

namespace BurningMod;

public sealed class CombustionTrapPower : BurningModPower
{
    public override string? CustomPackedIconPath => "res://BurningMod/images/powers/combustion_trap_power.png";
    public override string? CustomBigIconPath => "res://BurningMod/images/powers/combustion_trap_power.png";
    public override string? CustomBigBetaIconPath => "res://BurningMod/images/powers/combustion_trap_power.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side != base.Owner.Side) return;
        Flash();
        await BurnApplyHelper.ApplyBurn(combatState.HittableEnemies, base.Amount, base.Owner, null);
        await PowerCmd.Remove(this);
    }
}

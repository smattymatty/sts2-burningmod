#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;

namespace BurningMod;

public class FlashpointPower : BurningModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://BurningMod/images/powers/flashpoint_power.png";
    public override string? CustomBigIconPath => "res://BurningMod/images/powers/flashpoint_power.png";
    public override string? CustomBigBetaIconPath => "res://BurningMod/images/powers/flashpoint_power.png";

    public override async Task AfterOrbEvoked(PlayerChoiceContext choiceContext, OrbModel orb, IEnumerable<Creature> targets)
    {
        if (base.Owner.CombatState == null) return;
        var enemies = base.Owner.CombatState.HittableEnemies;
        if (enemies.Count == 0) return;

        Flash();
        await PowerCmd.TickDownDuration(this);
        await CreatureCmd.Damage(choiceContext, enemies, 3m, ValueProp.Unpowered, base.Owner);
        await BurnApplyHelper.ApplyBurn(enemies, 3m, base.Owner, null);
    }
}

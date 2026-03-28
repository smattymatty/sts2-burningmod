#nullable enable
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using BaseLib.Abstracts;

namespace BurningMod;

public class HeatTransferPower : BurningModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://BurningMod/images/powers/heat_transfer_power.png";
    public override string? CustomBigIconPath => "res://BurningMod/images/powers/heat_transfer_power.png";
    public override string? CustomBigBetaIconPath => "res://BurningMod/images/powers/heat_transfer_power.png";

    public override async Task AfterOrbChanneled(PlayerChoiceContext choiceContext, Player player, OrbModel orb)
    {
        if (player != base.Owner.Player) return;
        var enemies = base.Owner.CombatState?.HittableEnemies;
        if (enemies == null || enemies.Count == 0) return;

        Flash();
        var target = base.Owner.Player.RunState.Rng.CombatTargets.NextItem(enemies)!;
        await BurnApplyHelper.ApplyBurn(target, base.Amount, base.Owner, null);
    }
}

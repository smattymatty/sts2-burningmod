#nullable enable
using System.Linq;
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

public sealed class ArsonistPower : BurningModPower
{
    public override string? CustomPackedIconPath => "res://BurningMod/images/powers/arsonist_power.png";
    public override string? CustomBigIconPath => "res://BurningMod/images/powers/arsonist_power.png";
    public override string? CustomBigBetaIconPath => "res://BurningMod/images/powers/arsonist_power.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == base.Owner.Side) return;
        Flash();
        foreach (var enemy in combatState.HittableEnemies.ToList())
        {
            if (!CombatManager.Instance.IsInProgress) return;
            if (!enemy.IsAlive) continue;
            int burnStacks = enemy.GetPowerAmount<BurnPower>();
            if (burnStacks > 0)
            {
                await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), enemy, burnStacks, ValueProp.Unblockable | ValueProp.Unpowered, base.Owner, null);
                if (!CombatManager.Instance.IsInProgress) return;
            }
        }
    }
}

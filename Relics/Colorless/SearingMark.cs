#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BurningMod;

[Pool(typeof(SharedRelicPool))]
public class SearingMark : BurningModRelic
{
    private bool _usedThisTurn = false;

    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override string PackedIconPath => "res://BurningMod/images/relics/searing_mark.png";
    protected override string BigIconPath => "res://BurningMod/images/relics/searing_mark.png";
    protected override string PackedIconOutlinePath => "res://BurningMod/images/relics/searing_mark.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>() };

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == base.Owner.Creature.Side)
            _usedThisTurn = false;
        return Task.CompletedTask;
    }

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (_usedThisTurn) return;
        if (applier != base.Owner.Creature) return;
        if (amount <= 0) return;
        if (power.Owner == base.Owner.Creature) return;
        if (power.Type != PowerType.Debuff) return;

        _usedThisTurn = true;
        Flash();
        await BurnApplyHelper.ApplyBurn(power.Owner, 2m, base.Owner.Creature, null);
    }
}

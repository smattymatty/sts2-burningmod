using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BurningMod;

[Pool(typeof(IroncladRelicPool))]
public class Firestarter : BurningModRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override string PackedIconPath => "res://BurningMod/images/relics/firestarter.png";
    protected override string BigIconPath => "res://BurningMod/images/relics/firestarter.png";
    protected override string PackedIconOutlinePath => "res://BurningMod/images/relics/firestarter.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>() };

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side != base.Owner.Creature.Side) return;
        if (combatState.RoundNumber > 2) return;

        Flash();
        await BurnApplyHelper.ApplyBurn(base.Owner.Creature, 1m, base.Owner.Creature, null);
        await BurnApplyHelper.ApplyBurn(combatState.HittableEnemies, 2m, base.Owner.Creature, null);
    }
}

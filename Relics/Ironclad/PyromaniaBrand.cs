using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
public class PyromaniaBrand : BurningModRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override string PackedIconPath => "res://BurningMod/images/relics/pyromania_brand.png";
    protected override string BigIconPath => "res://BurningMod/images/relics/pyromania_brand.png";
    protected override string PackedIconOutlinePath => "res://BurningMod/images/relics/pyromania_brand.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>() };

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != base.Owner.Creature) return;

        string entry = cardPlay.Card.Id.Entry;
        if (entry != "INFLAME" && entry != "BURNING_PACT" && entry != "INFERNAL_BLADE") return;

        Flash();
        await BurnApplyHelper.ApplyBurn(base.Owner.Creature, 1m, base.Owner.Creature, null);
        await BurnApplyHelper.ApplyBurn(base.Owner.Creature.CombatState.HittableEnemies, 2m, base.Owner.Creature, null);
    }
}

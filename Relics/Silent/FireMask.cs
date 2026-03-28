using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BurningMod;

[Pool(typeof(SilentRelicPool))]
public class FireMask : BurningModRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
    public override string PackedIconPath => "res://BurningMod/images/relics/fire_mask.png";
    protected override string BigIconPath => "res://BurningMod/images/relics/fire_mask.png";
    protected override string PackedIconOutlinePath => "res://BurningMod/images/relics/fire_mask.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>() };

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != base.Owner) return;
        if (!cardPlay.IsAutoPlay) return;
        if (base.Owner.Creature.CombatState == null) return;
        var enemies = base.Owner.Creature.CombatState.HittableEnemies;
        if (enemies.Count == 0) return;
        Flash();
        await BurnApplyHelper.ApplyBurn(enemies, 2m, base.Owner.Creature, null);
    }
}

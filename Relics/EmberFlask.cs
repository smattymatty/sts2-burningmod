#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BurningMod;

[Pool(typeof(SharedRelicPool))]
public class EmberFlask : BurningModRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override string PackedIconPath => "res://BurningMod/images/relics/ember_flask.png";
    protected override string BigIconPath => "res://BurningMod/images/relics/ember_flask.png";
    protected override string PackedIconOutlinePath => "res://BurningMod/images/relics/ember_flask.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>() };

    public override async Task AfterPotionUsed(PotionModel potion, Creature? target)
    {
        if (base.Owner.Creature.CombatState == null) return;

        Flash();
        await BurnApplyHelper.ApplyBurn(
            base.Owner.Creature.CombatState.HittableEnemies,
            3m,
            base.Owner.Creature,
            null);
    }
}

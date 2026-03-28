#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BurningMod;

[Pool(typeof(DefectRelicPool))]
public class ForgeCore : BurningModRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
    public override string PackedIconPath => "res://BurningMod/images/relics/forge_core.png";
    protected override string BigIconPath => "res://BurningMod/images/relics/forge_core.png";
    protected override string PackedIconOutlinePath => "res://BurningMod/images/relics/forge_core.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[]
        {
            HoverTipFactory.Static(StaticHoverTip.Channeling),
            HoverTipFactory.FromOrb<FireOrb>()
        };

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side != base.Owner.Creature.Side) return;
        if (combatState.RoundNumber != 1) return;

        Flash();
        await OrbCmd.Channel<FireOrb>(choiceContext, base.Owner);
    }
}

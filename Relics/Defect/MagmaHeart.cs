#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BurningMod;

[Pool(typeof(DefectRelicPool))]
public class MagmaHeart : BurningModRelic
{
    private bool _usedThisCombat = false;

    public override RelicRarity Rarity => RelicRarity.Rare;
    public override string PackedIconPath => "res://BurningMod/images/relics/magma_heart.png";
    protected override string BigIconPath => "res://BurningMod/images/relics/magma_heart.png";
    protected override string PackedIconOutlinePath => "res://BurningMod/images/relics/magma_heart.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[]
        {
            HoverTipFactory.FromOrb<FireOrb>(),
            HoverTipFactory.FromOrb<InfernoOrb>()
        };

    public override Task BeforeCombatStart()
    {
        _usedThisCombat = false;
        return Task.CompletedTask;
    }

    public override Task AfterOrbChanneled(PlayerChoiceContext choiceContext, Player player, OrbModel orb)
    {
        if (player != base.Owner) return Task.CompletedTask;
        if (_usedThisCombat) return Task.CompletedTask;
        if (orb is not FireOrb) return Task.CompletedTask;

        _usedThisCombat = true;
        Flash();
        OrbCmd.Replace(orb, ModelDb.Orb<InfernoOrb>().ToMutable(), player);
        return Task.CompletedTask;
    }
}

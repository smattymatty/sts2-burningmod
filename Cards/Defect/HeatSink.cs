#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BurningMod;

[Pool(typeof(DefectCardPool))]
public class HeatSink : BurningModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(5m, ValueProp.Move)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[]
        {
            HoverTipFactory.Static(StaticHoverTip.Channeling),
            HoverTipFactory.FromOrb<FireOrb>()
        };

    public HeatSink() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    public override string? CustomPortraitPath => "res://BurningMod/images/cards/heat_sink.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await OrbCmd.Channel<FireOrb>(choiceContext, base.Owner);

        int fireOrbCount = base.Owner.PlayerCombatState.OrbQueue.Orbs.Count(o => o is FireOrb or InfernoOrb);
        for (int i = 0; i < fireOrbCount; i++)
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, null);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(2m);
    }
}

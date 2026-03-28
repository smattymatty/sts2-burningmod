#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BurningMod;

[Pool(typeof(SilentCardPool))]
public class ChainedFlame : BurningModCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("WeakAmount", 2m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromPower<BurnPower>() };

    public ChainedFlame() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/chained_flame.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await PowerCmd.Apply<WeakPower>(cardPlay.Target, base.DynamicVars["WeakAmount"].BaseValue, base.Owner.Creature, this);
        int weakStacks = cardPlay.Target.GetPowerAmount<WeakPower>();
        int burnAmount = weakStacks;
        if (burnAmount > 0)
            await BurnApplyHelper.ApplyBurn(cardPlay.Target, burnAmount, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.RemoveKeyword(CardKeyword.Exhaust);
    }
}

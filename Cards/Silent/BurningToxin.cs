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
public class BurningToxin : BurningModCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("PoisonAmount", 3m),
        new DynamicVar("BurnAmount", 4m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>(), HoverTipFactory.FromPower<PoisonPower>() };

    public BurningToxin() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/burning_toxin.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        bool alreadyPoisoned = cardPlay.Target.GetPowerAmount<PoisonPower>() > 0;

        if (alreadyPoisoned)
        {
            await CardPileCmd.Draw(choiceContext, 1, base.Owner);
            await PlayerCmd.GainEnergy(1m, base.Owner);
        }

        await PowerCmd.Apply<PoisonPower>(cardPlay.Target, base.DynamicVars["PoisonAmount"].BaseValue, base.Owner.Creature, this);
        await BurnApplyHelper.ApplyBurn(cardPlay.Target, base.DynamicVars["BurnAmount"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["PoisonAmount"].UpgradeValueBy(2m);
        base.DynamicVars["BurnAmount"].UpgradeValueBy(2m);
    }
}

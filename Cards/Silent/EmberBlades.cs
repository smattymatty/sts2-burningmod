#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BurningMod;

[Pool(typeof(SilentCardPool))]
public class EmberBlades : BurningModCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Sly, CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("ShivCount", 2m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromCard<BurningShiv>(), HoverTipFactory.FromPower<BurnPower>() };

    public EmberBlades() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/ember_blades.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int count = (int)base.DynamicVars["ShivCount"].BaseValue;
        var cards = new List<CardModel>();
        for (int i = 0; i < count; i++)
            cards.Add(base.CombatState!.CreateCard<BurningShiv>(base.Owner));
        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, addedByPlayer: true);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["ShivCount"].UpgradeValueBy(1m);
    }
}

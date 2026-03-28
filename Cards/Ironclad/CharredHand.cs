#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
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

[Pool(typeof(IroncladCardPool))]
public class CharredHand : BurningModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("BurnAmount", 1m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromPower<BurnPower>(), HoverTipFactory.FromKeyword(CardKeyword.Exhaust) };

    public CharredHand() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/charred_hand.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Apply Burn and draw (upgraded: do it twice, then exhaust after)
        await BurnApplyHelper.ApplyBurn(base.Owner.Creature, base.DynamicVars["BurnAmount"].BaseValue, base.Owner.Creature, this);
        await CardPileCmd.Draw(choiceContext, 1, base.Owner);

        if (base.IsUpgraded)
        {
            await BurnApplyHelper.ApplyBurn(base.Owner.Creature, base.DynamicVars["BurnAmount"].BaseValue, base.Owner.Creature, this);
            await CardPileCmd.Draw(choiceContext, 1, base.Owner);
        }

        // Choose a card to exhaust
        CardModel? card = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1),
            context: choiceContext,
            player: base.Owner,
            filter: null,
            source: this)).FirstOrDefault();
        if (card != null)
            await CardCmd.Exhaust(choiceContext, card);

        if (base.Owner.Creature.GetPowerAmount<BurnPower>() >= 3)
        {
            await PlayerCmd.GainEnergy(1m, base.Owner);
            await CardCmd.Exhaust(choiceContext, cardPlay.Card);
        }
    }

    protected override void OnUpgrade() { }
}

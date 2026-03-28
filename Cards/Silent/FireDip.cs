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
public class FireDip : BurningModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("ShivCount", 1m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromCard<BurningShiv>(base.IsUpgraded), HoverTipFactory.FromPower<BurnPower>() };

    public FireDip() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/fire_dip.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int shivCount = (int)base.DynamicVars["ShivCount"].BaseValue;
        var shivs = new List<CardModel>();
        for (int i = 0; i < shivCount; i++)
            shivs.Add(base.CombatState!.CreateCard<BurningShiv>(base.Owner));
        await CardPileCmd.AddGeneratedCardsToCombat(shivs, PileType.Hand, addedByPlayer: true);
        await PowerCmd.Apply<FireDipPower>(base.Owner.Creature, 2m, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["ShivCount"].UpgradeValueBy(1m);
    }
}

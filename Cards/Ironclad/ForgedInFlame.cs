#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BurningMod;

[Pool(typeof(IroncladCardPool))]
public class ForgedInFlame : BurningModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Amount", 2m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromPower<BurnPower>(), HoverTipFactory.FromPower<PlatingPower>() };

    public ForgedInFlame() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/forged_in_flame.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ForgedInFlamePower>(base.Owner.Creature, base.DynamicVars["Amount"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Amount"].UpgradeValueBy(1m);
    }
}

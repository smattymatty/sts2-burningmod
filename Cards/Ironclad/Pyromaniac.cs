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
public class Pyromaniac : BurningModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("BaseStr", 1m),
        new DynamicVar("BaseDex", 0m),
        new DynamicVar("BurnStr", 2m),
        new DynamicVar("BurnDex", 1m)
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Exhaust };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>() };

    public Pyromaniac() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/pyromaniac.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool burning = base.Owner.Creature.HasPower<BurnPower>();
        decimal str = burning ? base.DynamicVars["BurnStr"].BaseValue : base.DynamicVars["BaseStr"].BaseValue;
        decimal dex = burning ? base.DynamicVars["BurnDex"].BaseValue : base.DynamicVars["BaseDex"].BaseValue;

        await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, str, base.Owner.Creature, this);
        if (dex > 0)
            await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, dex, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["BaseStr"].UpgradeValueBy(1m);
        base.DynamicVars["BurnStr"].UpgradeValueBy(1m);
        base.DynamicVars["BurnDex"].UpgradeValueBy(1m);
    }
}

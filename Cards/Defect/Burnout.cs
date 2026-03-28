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
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BurningMod;

[Pool(typeof(DefectCardPool))]
public class Burnout : BurningModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(8m, ValueProp.Move),
        new DynamicVar("BlockAmount", 8m),
        new DynamicVar("BurnAmount", 8m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[]
        {
            HoverTipFactory.FromPower<BurnPower>(),
            HoverTipFactory.FromCard<Burn>(base.IsUpgraded)
        };

    public override bool GainsBlock => true;

    public Burnout() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    public override string? CustomPortraitPath => "res://BurningMod/images/cards/burnout.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        await CreatureCmd.GainBlock(base.Owner.Creature, new BlockVar(base.DynamicVars["BlockAmount"].BaseValue, ValueProp.Move), null);

        await BurnApplyHelper.ApplyBurn(cardPlay.Target, base.DynamicVars["BurnAmount"].BaseValue, base.Owner.Creature, this);

        CardModel burn = base.CombatState!.CreateCard<Burn>(base.Owner);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(burn, PileType.Discard, addedByPlayer: true));
        await Cmd.Wait(0.5f);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
        base.DynamicVars["BlockAmount"].UpgradeValueBy(3m);
        base.DynamicVars["BurnAmount"].UpgradeValueBy(3m);
    }
}

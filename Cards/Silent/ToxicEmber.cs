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
public class ToxicEmber : BurningModCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("PoisonAmount", 4m),
        new DynamicVar("BurnAmount", 4m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>(), HoverTipFactory.FromPower<PoisonPower>() };

    public ToxicEmber() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/toxic_ember.png";

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card != this) return;
        if (base.Owner.Creature.CombatState == null) return;
        var enemies = base.Owner.Creature.CombatState.HittableEnemies;
        if (enemies.Count == 0) return;

        var poisonTarget = base.Owner.RunState.Rng.CombatTargets.NextItem(enemies)!;
        await PowerCmd.Apply<PoisonPower>(poisonTarget, 1m, base.Owner.Creature, this);

        var burnTarget = base.Owner.RunState.Rng.CombatTargets.NextItem(enemies)!;
        await BurnApplyHelper.ApplyBurn(burnTarget, 1m, base.Owner.Creature, this);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await PowerCmd.Apply<PoisonPower>(cardPlay.Target, base.DynamicVars["PoisonAmount"].BaseValue, base.Owner.Creature, this);
        await BurnApplyHelper.ApplyBurn(cardPlay.Target, base.DynamicVars["BurnAmount"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}

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

[Pool(typeof(IroncladCardPool))]
public class Ember : BurningModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("BurnAmount", 4m),
        new DynamicVar("BurnThreshold", 6m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>() };

    public Ember() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/ember.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await BurnApplyHelper.ApplyBurn(cardPlay.Target, base.DynamicVars["BurnAmount"].BaseValue, base.Owner.Creature, this);
        await BurnApplyHelper.ApplyBurn(base.Owner.Creature, 1m, base.Owner.Creature, this);

        if (cardPlay.Target.GetPowerAmount<BurnPower>() >= (int)base.DynamicVars["BurnThreshold"].BaseValue)
            await CardPileCmd.Draw(choiceContext, 1, base.Owner);
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card != this) return;
        if (base.Owner.Creature.CombatState == null) return;
        var enemies = base.Owner.Creature.CombatState.HittableEnemies;
        if (enemies.Count == 0) return;
        var target = base.Owner.RunState.Rng.CombatTargets.NextItem(enemies)!;
        await BurnApplyHelper.ApplyBurn(target, 1m, base.Owner.Creature, this);
        await BurnApplyHelper.ApplyBurn(base.Owner.Creature, 1m, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}

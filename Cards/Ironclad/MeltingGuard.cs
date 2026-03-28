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
public class MeltingGuard : BurningModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(3m, ValueProp.Move),
        new DynamicVar("BurnThreshold", 2m),
        new DynamicVar("ExhaustBurn", 3m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromPower<BurnPower>(), HoverTipFactory.FromKeyword(CardKeyword.Exhaust) };

    public MeltingGuard() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/melting_guard.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        int burnAmount = base.Owner.Creature.GetPowerAmount<BurnPower>();
        if (burnAmount >= (int)base.DynamicVars["BurnThreshold"].BaseValue)
            await CardCmd.Exhaust(choiceContext, this);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card != this) return;
        var combatState = base.Owner.Creature.CombatState!;

        foreach (var enemy in combatState.HittableEnemies)
            await BurnApplyHelper.ApplyBurn(enemy, base.DynamicVars["ExhaustBurn"].BaseValue, base.Owner.Creature, this);

        int totalBurn = base.Owner.Creature.GetPowerAmount<BurnPower>();
        foreach (var enemy in combatState.HittableEnemies)
            totalBurn += enemy.GetPowerAmount<BurnPower>();

        if (totalBurn > 0)
            await CreatureCmd.GainBlock(base.Owner.Creature, new BlockVar(totalBurn, ValueProp.Move), null);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(1m);
        base.DynamicVars["ExhaustBurn"].UpgradeValueBy(2m);
    }
}

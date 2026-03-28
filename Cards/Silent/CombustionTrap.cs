#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BurningMod;

[Pool(typeof(SilentCardPool))]
public class CombustionTrap : BurningModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(6m, ValueProp.Move),
        new DynamicVar("BurnAmount", 2m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>(), HoverTipFactory.FromPower<CombustionTrapPower>() };

    public CombustionTrap() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/combustion_trap.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        await BurnApplyHelper.ApplyBurn(base.CombatState!.HittableEnemies, 1m, base.Owner.Creature, this);
        await PowerCmd.Apply<CombustionTrapPower>(base.Owner.Creature, base.DynamicVars["BurnAmount"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(2m);
        base.DynamicVars["BurnAmount"].UpgradeValueBy(1m);
    }
}

#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BurningMod;

[Pool(typeof(DefectRelicPool))]
public class SlagProcessor : BurningModRelic
{
    private bool _usedThisTurn = false;

    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override string PackedIconPath => "res://BurningMod/images/relics/slag_processor.png";
    protected override string BigIconPath => "res://BurningMod/images/relics/slag_processor.png";
    protected override string PackedIconOutlinePath => "res://BurningMod/images/relics/slag_processor.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>() };

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == base.Owner.Creature.Side)
            _usedThisTurn = false;
        return Task.CompletedTask;
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner != base.Owner) return;
        if (_usedThisTurn) return;
        if (card.Type != CardType.Status) return;
        var enemies = base.Owner.Creature.CombatState?.HittableEnemies;
        if (enemies == null || enemies.Count == 0) return;

        _usedThisTurn = true;
        Flash();
        await CardCmd.Exhaust(choiceContext, card);
        await CardPileCmd.Draw(choiceContext, 1, base.Owner);
        var target = base.Owner.RunState.Rng.CombatTargets.NextItem(enemies)!;
        await BurnApplyHelper.ApplyBurn(target, 2m, base.Owner.Creature, null);
    }
}

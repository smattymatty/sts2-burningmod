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

[Pool(typeof(SilentRelicPool))]
public class ShivSmelter : BurningModRelic
{
    private int _turnsTriggered = 0;

    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override string PackedIconPath => "res://BurningMod/images/relics/shiv_smelter.png";
    protected override string BigIconPath => "res://BurningMod/images/relics/shiv_smelter.png";
    protected override string PackedIconOutlinePath => "res://BurningMod/images/relics/shiv_smelter.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromCard<BurningShiv>() };

    public override Task BeforeCombatStart()
    {
        _turnsTriggered = 0;
        return Task.CompletedTask;
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side != base.Owner.Creature.Side) return;
        if (_turnsTriggered >= 2) return;
        _turnsTriggered++;
        Flash();
        var cards = new List<CardModel>
        {
            combatState.CreateCard<BurningShiv>(base.Owner),
            combatState.CreateCard<BurningShiv>(base.Owner)
        };
        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, addedByPlayer: true);
    }
}

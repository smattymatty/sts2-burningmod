using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Abstracts;

namespace BurningMod;

public class TorchbearerPower : BurningModPower
{
    public override string? CustomPackedIconPath => "res://BurningMod/images/powers/torchbearer_power.png";
    public override string? CustomBigIconPath => "res://BurningMod/images/powers/torchbearer_power.png";
    public override string? CustomBigBetaIconPath => "res://BurningMod/images/powers/torchbearer_power.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player != base.Owner.Player) return;

        Flash();
        for (int i = 0; i < base.Amount; i++)
        {
            int pick = player.RunState.Rng.CombatCardGeneration.NextItem(new[] { 0, 1, 2 });
            CardModel card = pick switch
            {
                0 => combatState.CreateCard<TorchStrike>(player),
                1 => combatState.CreateCard<TorchSweep>(player),
                _ => combatState.CreateCard<TheFinalTorch>(player)
            };

            card.SetToFreeThisTurn();
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true);
        }
    }

}

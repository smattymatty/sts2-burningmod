#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BurningMod;

[Pool(typeof(IroncladCardPool))]
public class Torchbearer : BurningModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromCard<TorchStrike>(), HoverTipFactory.FromCard<TorchSweep>(), HoverTipFactory.FromCard<TheFinalTorch>() };

    public Torchbearer() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/torchbearer.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<TorchbearerPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);

        int pick = base.Owner.RunState.Rng.CombatCardGeneration.NextItem(new[] { 0, 1, 2 });
        CardModel card = pick switch
        {
            0 => base.CombatState!.CreateCard<TorchStrike>(base.Owner),
            1 => base.CombatState!.CreateCard<TorchSweep>(base.Owner),
            _ => base.CombatState!.CreateCard<TheFinalTorch>(base.Owner)
        };
        card.SetToFreeThisTurn();
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}

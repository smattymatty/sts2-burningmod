#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BurningMod;

[Pool(typeof(DefectCardPool))]
public class ScorchSelf : BurningModCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[]
        {
            HoverTipFactory.FromPower<ScorchSelfPower>(),
            HoverTipFactory.FromPower<BurnPower>(),
            HoverTipFactory.FromOrb<FireOrb>(),
            HoverTipFactory.FromCard<Burn>(false)
        };

    public ScorchSelf() : base(3, CardType.Power, CardRarity.Rare, TargetType.None) { }

    public override string? CustomPortraitPath => "res://BurningMod/images/cards/scorch_self.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        // Transform all cards in draw and discard piles into Burn status cards (hand stays)
        var draw = PileType.Draw.GetPile(base.Owner).Cards.ToList();
        var discard = PileType.Discard.GetPile(base.Owner).Cards.ToList();
        var toTransform = draw.Concat(discard)
            .Where(c => c.IsTransformable)
            .Select(c => new CardTransformation(c, base.CombatState!.CreateCard<Burn>(base.Owner)))
            .ToList();

        if (toTransform.Count > 0)
            await CardCmd.Transform(toTransform, null);

        await PowerCmd.Apply<ScorchSelfPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}

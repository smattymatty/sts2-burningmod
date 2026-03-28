#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BurningMod;

[Pool(typeof(DefectCardPool))]
public class InfernalOverload : BurningModCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[]
        {
            HoverTipFactory.FromPower<InfernalOverloadPower>(),
            HoverTipFactory.FromOrb<InfernoOrb>(),
            HoverTipFactory.FromOrb<FireOrb>()
        };

    public InfernalOverload() : base(3, CardType.Power, CardRarity.Rare, TargetType.None) { }

    public override string? CustomPortraitPath => "res://BurningMod/images/cards/inferno.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<InfernalOverloadPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);

        var fireOrbs = base.Owner.PlayerCombatState.OrbQueue.Orbs
            .Where(o => o is FireOrb).ToList();
        foreach (var orb in fireOrbs)
            OrbCmd.Replace(orb, ModelDb.Orb<InfernoOrb>().ToMutable(), base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}

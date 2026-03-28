#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BurningMod;

[Pool(typeof(DefectCardPool))]
public class HeatTransfer : BurningModCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[]
        {
            HoverTipFactory.FromPower<HeatTransferPower>(),
            HoverTipFactory.FromPower<BurnPower>()
        };

    public HeatTransfer() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None) { }

    public override string? CustomPortraitPath => "res://BurningMod/images/cards/heat_transfer.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        decimal amount = base.IsUpgraded ? 3m : 2m;
        await PowerCmd.Apply<HeatTransferPower>(base.Owner.Creature, amount, base.Owner.Creature, this);
    }

    protected override void OnUpgrade() { }
}

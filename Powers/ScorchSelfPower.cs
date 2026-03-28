#nullable enable
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Abstracts;

namespace BurningMod;

public class ScorchSelfPower : BurningModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://BurningMod/images/powers/scorch_self_power.png";
    public override string? CustomBigIconPath => "res://BurningMod/images/powers/scorch_self_power.png";
    public override string? CustomBigBetaIconPath => "res://BurningMod/images/powers/scorch_self_power.png";

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card is not Burn) return;
        if (card.Owner.Creature != base.Owner) return;

        Flash();
        await OrbCmd.Channel<FireOrb>(choiceContext, card.Owner);
        await PowerCmd.Apply<ScorchSelfFocusPower>(base.Owner, 1m, base.Owner, null);
    }
}

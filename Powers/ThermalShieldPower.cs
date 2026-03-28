#nullable enable
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;

namespace BurningMod;

public class ThermalShieldPower : BurningModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://BurningMod/images/powers/thermal_shield_power.png";
    public override string? CustomBigIconPath => "res://BurningMod/images/powers/thermal_shield_power.png";
    public override string? CustomBigBetaIconPath => "res://BurningMod/images/powers/thermal_shield_power.png";

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card is not Burn) return;
        if (card.Owner.Creature != base.Owner) return;

        Flash();
        await CreatureCmd.GainBlock(base.Owner, new BlockVar(base.Amount, ValueProp.Move), null);
    }
}

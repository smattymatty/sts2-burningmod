#nullable enable
using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;

namespace BurningMod;

public class ForgedInFlamePower : BurningModPower
{
    public override string? CustomPackedIconPath => "res://BurningMod/images/powers/forged_in_flame_power.png";
    public override string? CustomBigIconPath => "res://BurningMod/images/powers/forged_in_flame_power.png";
    public override string? CustomBigBetaIconPath => "res://BurningMod/images/powers/forged_in_flame_power.png";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (!(power is BurnPower)) return;
        if (power.Owner != base.Owner) return;
        if (amount <= 0) return;

        await PowerCmd.Apply<VigorPower>(base.Owner, base.Amount, base.Owner, null);
        await CreatureCmd.GainBlock(base.Owner, new BlockVar(base.Amount, ValueProp.Move), null);
        decimal plating = Math.Max(1m, Math.Floor(base.Amount / 2m));
        await PowerCmd.Apply<PlatingPower>(base.Owner, plating, base.Owner, cardSource);
    }
}

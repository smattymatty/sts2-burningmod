#nullable enable
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Abstracts;

namespace BurningMod;

public sealed class EternalFlamePower : BurningModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://BurningMod/images/powers/eternal_flame_power.png";
    public override string? CustomBigIconPath => "res://BurningMod/images/powers/eternal_flame_power.png";
    public override string? CustomBigBetaIconPath => "res://BurningMod/images/powers/eternal_flame_power.png";

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
#pragma warning disable CS8602
        foreach (var power in base.Owner.CombatState.Creatures
#pragma warning restore CS8602
            .SelectMany(c => c.Powers)
            .OfType<BurnPower>())
        {
            power.EternalFlameActive = true;
        }
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is BurnPower burnPower)
            burnPower.EternalFlameActive = true;
        return Task.CompletedTask;
    }
}

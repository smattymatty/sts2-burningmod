#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;

namespace BurningMod;

public class ResonantFieldPower : BurningModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://BurningMod/images/powers/resonant_field_power.png";
    public override string? CustomBigIconPath => "res://BurningMod/images/powers/resonant_field_power.png";
    public override string? CustomBigBetaIconPath => "res://BurningMod/images/powers/resonant_field_power.png";

    private readonly HashSet<Type> _seenPowers = new();

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        // Snapshot all current powers (including this one) so we don't re-trigger on them
        _seenPowers.Clear();
        foreach (var power in base.Owner.Powers)
            _seenPowers.Add(power.GetType());
        _seenPowers.Add(typeof(ResonantFieldPower));
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type != CardType.Power) return;
        if (cardPlay.Card.Owner.Creature != base.Owner) return;

        // Find any new powers not in our seen set
        var newPowers = base.Owner.Powers
            .Where(p => !_seenPowers.Contains(p.GetType()))
            .ToList();

        if (newPowers.Count == 0) return;

        // Mark them as seen before firing so we don't re-trigger
        foreach (var p in newPowers)
            _seenPowers.Add(p.GetType());

        Flash();
        foreach (var _ in newPowers)
        {
            await OrbCmd.Channel<FireOrb>(context, base.Owner.Player);
            var enemies = base.Owner.CombatState.HittableEnemies;
            if (enemies.Count > 0)
                await BurnApplyHelper.ApplyBurn(enemies, base.Amount, base.Owner, null);
        }
    }
}

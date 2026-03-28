#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BurningMod;

[Pool(typeof(DefectCardPool))]
public class KindlingCircuit : BurningModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(9m, ValueProp.Move)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>() };

    public KindlingCircuit() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    public override string? CustomPortraitPath => "res://BurningMod/images/cards/kindling_circuit.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // 1. Snapshot front orb type and Burn status before dealing damage
        var orbs = base.Owner.PlayerCombatState.OrbQueue.Orbs;
        OrbModel? frontOrb = orbs.Count > 0 ? orbs.First() : null;
        Type? orbType = frontOrb?.GetType();
        bool targetIsBurning = cardPlay.Target.HasPower<BurnPower>();

        // 2. Deal damage
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 3. Evoke front orb
        if (frontOrb != null)
            await OrbCmd.EvokeNext(choiceContext, base.Owner);

        // 4-5. If target was Burning, channel a fresh orb of the captured type
        if (orbType != null && targetIsBurning)
        {
            // Use OrbCmd.Channel<T> via reflection — can't call constructors on models
            MethodInfo channelGeneric = typeof(OrbCmd).GetMethod(nameof(OrbCmd.Channel), 1,
                new[] { typeof(PlayerChoiceContext), typeof(MegaCrit.Sts2.Core.Entities.Players.Player) })!;
            MethodInfo channelTyped = channelGeneric.MakeGenericMethod(orbType);
            await (Task)channelTyped.Invoke(null, new object[] { choiceContext, base.Owner })!;
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
    }
}

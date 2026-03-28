#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BurningMod;

[Pool(typeof(IroncladCardPool))]
public class ScorchedEarth : BurningModCard
{
    protected override IEnumerable<string> ExtraRunAssetPaths => NGroundFireVfx.AssetPaths;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("BurnAmount", 14m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>() };

    public ScorchedEarth() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/scorched_earth.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // Fire VFX on all enemies
        foreach (var enemy in base.CombatState!.HittableEnemies)
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(enemy));

        // Step 1: Apply Burn to ALL enemies
        foreach (var enemy in base.Owner.Creature.CombatState!.HittableEnemies)
        {
            await BurnApplyHelper.ApplyBurn(enemy, base.DynamicVars["BurnAmount"].BaseValue, base.Owner.Creature, this);
        }

        // Step 2: Deal damage equal to target's total Burn to target
        if (!cardPlay.Target.IsAlive) return;
        int targetBurn = cardPlay.Target.GetPowerAmount<BurnPower>();
        await DamageCmd.Attack(targetBurn)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["BurnAmount"].UpgradeValueBy(6m);
    }
}

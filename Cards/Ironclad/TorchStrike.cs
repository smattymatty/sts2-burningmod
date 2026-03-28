#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BurningMod;

[Pool(typeof(IroncladCardPool))]
public class TorchStrike : BurningModCard
{
    protected override IEnumerable<string> ExtraRunAssetPaths => NGroundFireVfx.AssetPaths;
    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Strike };
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(8m, ValueProp.Move),
        new DynamicVar("EnemyBurn", 3m),
        new DynamicVar("SelfBurn", 1m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>() };
    
    public TorchStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    public override string? CustomPortraitPath => "res://BurningMod/images/cards/torch_strike.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(cardPlay.Target));
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await BurnApplyHelper.ApplyBurn(cardPlay.Target, base.DynamicVars["EnemyBurn"].BaseValue, base.Owner.Creature, this);
        await BurnApplyHelper.ApplyBurn(base.Owner.Creature, base.DynamicVars["SelfBurn"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<TorchCounterPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
        Log.Debug($"[TorchStrike] TorchCounter is now {base.Owner.Creature.GetPowerAmount<TorchCounterPower>()}");
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
        base.DynamicVars["EnemyBurn"].UpgradeValueBy(1m);
        base.DynamicVars["SelfBurn"].UpgradeValueBy(1m);
    }
}
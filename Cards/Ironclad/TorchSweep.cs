#nullable enable
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
public class TorchSweep : BurningModCard
{
    protected override IEnumerable<string> ExtraRunAssetPaths => NGroundFireVfx.AssetPaths;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6m, ValueProp.Move),
        new DynamicVar("EnemyBurn", 3m),
        new DynamicVar("SelfBurn", 1m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>() };

    public TorchSweep() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/torch_sweep.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (var enemy in base.CombatState!.HittableEnemies)
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(enemy));

        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(base.CombatState!)
            .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);

        foreach (var enemy in base.Owner.Creature.CombatState!.HittableEnemies)
        {
            await BurnApplyHelper.ApplyBurn(enemy, base.DynamicVars["EnemyBurn"].BaseValue, base.Owner.Creature, this);
        }

        await BurnApplyHelper.ApplyBurn(base.Owner.Creature, base.DynamicVars["SelfBurn"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<TorchCounterPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
        Log.Debug($"[TorchSweep] TorchCounter is now {base.Owner.Creature.GetPowerAmount<TorchCounterPower>()}");
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
        base.DynamicVars["EnemyBurn"].UpgradeValueBy(2m);
        base.DynamicVars["SelfBurn"].UpgradeValueBy(1m);
    }
}

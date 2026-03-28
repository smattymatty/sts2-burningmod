#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
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
public class Immolation : BurningModCard
{
    protected override IEnumerable<string> ExtraRunAssetPaths => NGroundFireVfx.AssetPaths;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("SelfBurn", 2m),
        new DynamicVar("Multiplier", 3m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>() };

    public Immolation() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/immolation.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await BurnApplyHelper.ApplyBurn(base.Owner.Creature, base.DynamicVars["SelfBurn"].BaseValue, base.Owner.Creature, this);

        int burnAmount = base.Owner.Creature.GetPowerAmount<BurnPower>();

        decimal multiplier = base.DynamicVars["Multiplier"].BaseValue;
        decimal damage = burnAmount * multiplier;

        foreach (var enemy in base.CombatState!.HittableEnemies)
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(enemy));

        foreach (var enemy in base.Owner.Creature.CombatState!.GetOpponentsOf(base.Owner.Creature).ToList())
        {
            if (enemy.IsAlive)
            {
                await DamageCmd.Attack(damage)
                    .FromCard(this)
                    .Targeting(enemy)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .Execute(choiceContext);
                await BurnApplyHelper.ApplyBurn(enemy, burnAmount, base.Owner.Creature, this);
            }
        }

        await PowerCmd.Remove<BurnPower>(base.Owner.Creature);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["SelfBurn"].UpgradeValueBy(1m);
        base.DynamicVars["Multiplier"].UpgradeValueBy(1m);
    }
}

#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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

[Pool(typeof(SilentCardPool))]
public class DancingFlame : BurningModCard
{
    protected override IEnumerable<string> ExtraRunAssetPaths => NGroundFireVfx.AssetPaths;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust, CardKeyword.Sly };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(2m, ValueProp.Move),
        new DynamicVar("BurnAmount", 2m),
        new DynamicVar("HitCount", 3m),
        new BurnBonusHitsVar()
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>() };

    public DancingFlame() : base(2, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/dancing_flame.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal damage = base.DynamicVars.Damage.BaseValue;
        int hitCount = (int)base.DynamicVars["HitCount"].BaseValue;

        int burnApps = base.Owner.Creature.GetPowerAmount<AppliedThisTurnPower>();
        hitCount += burnApps;

        for (int i = 0; i < hitCount; i++)
        {
            var enemies = base.CombatState!.HittableEnemies;
            if (enemies.Count == 0) break;
            Creature target = base.Owner.RunState.Rng.CombatCardGeneration.NextItem(enemies)!;

            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(target));
            await DamageCmd.Attack(damage)
                .FromCard(this)
                .Targeting(target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            await BurnApplyHelper.ApplyBurn(target, base.DynamicVars["BurnAmount"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(1m);
        base.DynamicVars["BurnAmount"].UpgradeValueBy(1m);
    }
}

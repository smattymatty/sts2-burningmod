#nullable enable
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
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BurningMod;

[Pool(typeof(SilentCardPool))]
public class AshCloud : BurningModCard
{
    protected override IEnumerable<string> ExtraRunAssetPaths => NGroundFireVfx.AssetPaths;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("WeakAmount", 1m),
        new DynamicVar("BurnAmount", 2m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromPower<BurnPower>() };

    public AshCloud() : base(0, CardType.Skill, CardRarity.Common, TargetType.AllEnemies) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/ash_cloud.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (var enemy in base.CombatState!.HittableEnemies)
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(enemy));

        await PowerCmd.Apply<WeakPower>(base.CombatState.HittableEnemies, base.DynamicVars["WeakAmount"].BaseValue, base.Owner.Creature, this);
        await BurnApplyHelper.ApplyBurn(base.CombatState.HittableEnemies, base.DynamicVars["BurnAmount"].BaseValue, base.Owner.Creature, this);
        if (base.IsUpgraded)
        {
            await PowerCmd.Apply<WeakPower>(base.CombatState.HittableEnemies, base.DynamicVars["WeakAmount"].BaseValue, base.Owner.Creature, this);
            await BurnApplyHelper.ApplyBurn(base.CombatState.HittableEnemies, base.DynamicVars["BurnAmount"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() { }
}

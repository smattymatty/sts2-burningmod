using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BurningMod;

[Pool(typeof(SilentRelicPool))]
public class VenomCoil : BurningModRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    public override string PackedIconPath => "res://BurningMod/images/relics/venom_coil.png";
    protected override string BigIconPath => "res://BurningMod/images/relics/venom_coil.png";
    protected override string PackedIconOutlinePath => "res://BurningMod/images/relics/venom_coil.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromPower<PoisonPower>(), HoverTipFactory.FromPower<BurnPower>() };

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side != base.Owner.Creature.Side) return;
        if (combatState.RoundNumber != 1) return;
        Flash();
        await PowerCmd.Apply<PoisonPower>(combatState.HittableEnemies, 3m, base.Owner.Creature, null);
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented) return;
        if (creature.Side == base.Owner.Creature.Side) return;
        int poisonStacks = creature.GetPowerAmount<PoisonPower>();
        if (poisonStacks <= 0) return;
        if (base.Owner.Creature.CombatState == null) return;
        var enemies = base.Owner.Creature.CombatState.HittableEnemies;
        if (enemies.Count == 0) return;
        var target = base.Owner.RunState.Rng.CombatTargets.NextItem(enemies);
        Flash();
        await BurnApplyHelper.ApplyBurn(target, poisonStacks, base.Owner.Creature, null);
    }
}

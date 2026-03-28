using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BurningMod;

[Pool(typeof(IroncladRelicPool))]
public class PhoenixAsh : BurningModRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override string PackedIconPath => "res://BurningMod/images/relics/phoenix_ash.png";
    protected override string BigIconPath => "res://BurningMod/images/relics/phoenix_ash.png";
    protected override string PackedIconOutlinePath => "res://BurningMod/images/relics/phoenix_ash.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>() };

    private int _burnSnapshot;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        int burn = base.Owner.Creature.GetPowerAmount<BurnPower>();
        int enemies = base.Owner.Creature.CombatState.HittableEnemies.Count;
        Log.Warn($"[PhoenixAsh] AfterCardPlayed: card={cardPlay.Card.Id.Entry} isOurs={cardPlay.Card.Owner.Creature == base.Owner.Creature} enemies={enemies} burn={burn} snapshot={_burnSnapshot}");

        if (cardPlay.Card.Owner.Creature != base.Owner.Creature) return;
        if (enemies > 0) return;

        _burnSnapshot = burn;
        Log.Warn($"[PhoenixAsh] Snapshotted burn={_burnSnapshot}");
    }

    public override async Task AfterCombatVictory(CombatRoom _)
    {
        int currentBurn = base.Owner.Creature.GetPowerAmount<BurnPower>();
        Log.Warn($"[PhoenixAsh] AfterCombatVictory: IsDead={base.Owner.Creature.IsDead} snapshot={_burnSnapshot} currentBurn={currentBurn}");

        if (base.Owner.Creature.IsDead) return;
        if (_burnSnapshot <= 0) return;

        Log.Warn($"[PhoenixAsh] Healing for {_burnSnapshot}");
        Flash();
        await CreatureCmd.Heal(base.Owner.Creature, _burnSnapshot);
        _burnSnapshot = 0;
    }
}

#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using BaseLib.Abstracts;

namespace BurningMod;

public class ThawPower : BurningModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://BurningMod/images/powers/thaw_power.png";
    public override string? CustomBigIconPath => "res://BurningMod/images/powers/thaw_power.png";
    public override string? CustomBigBetaIconPath => "res://BurningMod/images/powers/thaw_power.png";

    private static readonly MethodInfo EvokeMethod = AccessTools.Method(
        typeof(OrbCmd), "Evoke",
        new[] { typeof(PlayerChoiceContext), typeof(Player), typeof(OrbModel), typeof(bool) });

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner.Player) return;

        var frostOrbs = player.PlayerCombatState.OrbQueue.Orbs
            .OfType<FrostOrb>()
            .ToList();

        if (frostOrbs.Count == 0)
        {
            await PowerCmd.Remove(this);
            return;
        }

        Flash();
        int count = frostOrbs.Count;

        foreach (var frost in frostOrbs)
        {
            await (Task)EvokeMethod.Invoke(null, new object[] { choiceContext, player, frost, true })!;
        }

        await PowerCmd.Remove(this);

        for (int i = 0; i < count; i++)
            await OrbCmd.Channel<FireOrb>(choiceContext, player);
    }
}

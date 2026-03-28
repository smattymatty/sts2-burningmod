#nullable enable
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Orbs;
using BaseLib.Abstracts;

namespace BurningMod;

public class InfernalOverloadPower : BurningModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://BurningMod/images/powers/inferno_power.png";
    public override string? CustomBigIconPath => "res://BurningMod/images/powers/inferno_power.png";
    public override string? CustomBigBetaIconPath => "res://BurningMod/images/powers/inferno_power.png";

    public override Task AfterOrbChanneled(PlayerChoiceContext choiceContext, Player player, OrbModel orb)
    {
        if (orb is not FireOrb) return Task.CompletedTask;
        if (player != base.Owner.Player) return Task.CompletedTask;

        Flash();
        OrbModel infernoOrb = ModelDb.Orb<InfernoOrb>().ToMutable();
        OrbCmd.Replace(orb, infernoOrb, player);
        return Task.CompletedTask;
    }
}

#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace BurningMod;

public static class BurnApplyHelper
{
    public static async Task ApplyBurn(IEnumerable<Creature> targets, decimal amount, Creature applier, CardModel? source)
    {
        await PowerCmd.Apply<BurnPower>(targets, amount, applier, source);
        if (applier.IsPlayer)
            await PowerCmd.Apply<AppliedThisTurnPower>(applier, 1m, applier, source);
    }

    public static async Task ApplyBurn(Creature target, decimal amount, Creature applier, CardModel? source)
    {
        await PowerCmd.Apply<BurnPower>(target, amount, applier, source);
        if (applier.IsPlayer)
            await PowerCmd.Apply<AppliedThisTurnPower>(applier, 1m, applier, source);
    }
}

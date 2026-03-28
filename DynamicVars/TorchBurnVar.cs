using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

#nullable enable
namespace BurningMod;

public class TorchBurnVar : DynamicVar
{
    public TorchBurnVar(decimal baseValue) : base("EnemyBurn", baseValue) { }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        int torchBonus = 0;
        if (card.Owner?.Creature != null)
        {
            int multiplier = card.IsUpgraded ? 3 : 2;
            torchBonus = (card.Owner.Creature.GetPowerAmount<TorchCounterPower>() + 1) * multiplier;
        }

        PreviewValue = BaseValue + torchBonus;
    }
}

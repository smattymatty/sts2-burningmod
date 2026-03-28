using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

#nullable enable
namespace BurningMod;

public class TorchDamageVar : DamageVar
{
    public TorchDamageVar(decimal damage, ValueProp props) : base(damage, props) { }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        // Calculate torch bonus (+1 for self since TheFinalTorch counts itself)
        int torchBonus = 0;
        if (card.Owner?.Creature != null)
        {
            int multiplier = card.IsUpgraded ? 3 : 2;
            torchBonus = (card.Owner.Creature.GetPowerAmount<TorchCounterPower>() + 1) * multiplier;
        }

        decimal effectiveBase = BaseValue + torchBonus;

        // Replicate DamageVar logic with the modified base
        decimal num = effectiveBase;
        var enchantment = card.Enchantment;
        if (enchantment != null)
        {
            num += enchantment.EnchantDamageAdditive(num, Props);
            num *= enchantment.EnchantDamageMultiplicative(num, Props);
            if (!card.IsEnchantmentPreview)
                EnchantedValue = num;
        }

        if (runGlobalHooks && card.Owner != null)
        {
            num = Hook.ModifyDamage(card.Owner.RunState, card.CombatState, target, card.Owner.Creature, effectiveBase, Props, card, ModifyDamageHookType.All, previewMode, out IEnumerable<AbstractModel> _);
        }

        PreviewValue = num;
    }
}

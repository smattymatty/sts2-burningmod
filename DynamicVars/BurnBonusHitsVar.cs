#nullable enable
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BurningMod;

public class BurnBonusHitsVar : DynamicVar
{
    private AbstractModel? _owner;

    public BurnBonusHitsVar() : base("BonusHits", 0m) { }

    public override void SetOwner(AbstractModel owner)
    {
        _owner = owner;
        base.SetOwner(owner);
    }

    private int GetLiveValue()
    {
        if (_owner is CardModel card && card.IsMutable && card.Owner?.Creature != null)
            return card.Owner.Creature.GetPowerAmount<AppliedThisTurnPower>();
        return 0;
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        PreviewValue = GetLiveValue();
    }

    protected override decimal GetBaseValueForIConvertible()
    {
        return GetLiveValue();
    }

    public override string ToString()
    {
        return GetLiveValue().ToString();
    }
}

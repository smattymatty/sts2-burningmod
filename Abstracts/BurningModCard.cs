#nullable enable
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Abstracts;

namespace BurningMod;

public abstract class BurningModCard : CustomCardModel
{
    private const string PlaceholderPath = "res://BurningMod/images/cards/placeholder.png";

    public override string PortraitPath => CustomPortraitPath ?? PlaceholderPath;
    public override string BetaPortraitPath => CustomPortraitPath ?? PlaceholderPath;
    public override string? CustomPortraitPath => null;

    protected BurningModCard(int energyCost, CardType type, CardRarity rarity, TargetType target,
        bool showInCardLibrary = true) : base(energyCost, type, rarity, target, showInCardLibrary) { }
}

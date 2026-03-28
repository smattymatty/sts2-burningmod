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
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BurningMod;

[Pool(typeof(IroncladCardPool))]
public class FireBrand : BurningModCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("BurnAmount", 2m),
        new DynamicVar("VulnAmount", 1m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>(), HoverTipFactory.FromPower<VulnerablePower>() };

    public FireBrand() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/fire_brand.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Apply power BEFORE Vulnerable so the passive triggers on its own on-play Vulnerable
        await PowerCmd.Apply<FireBrandPower>(base.Owner.Creature, base.DynamicVars["BurnAmount"].BaseValue, base.Owner.Creature, this);

        foreach (var enemy in base.Owner.Creature.CombatState!.HittableEnemies)
            await PowerCmd.Apply<VulnerablePower>(enemy, base.DynamicVars["VulnAmount"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["BurnAmount"].UpgradeValueBy(2m);
    }
}

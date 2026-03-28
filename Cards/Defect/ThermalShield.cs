#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BurningMod;

[Pool(typeof(DefectCardPool))]
public class ThermalShield : BurningModCard
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("BlockAmount", 4m),
        new DynamicVar("ShieldAmount", 4m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[]
        {
            HoverTipFactory.FromPower<ThermalShieldPower>(),
            HoverTipFactory.FromCard<Burn>(false)
        };

    public ThermalShield() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None) { }

    public override string? CustomPortraitPath => "res://BurningMod/images/cards/thermal_shield.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(base.Owner.Creature, new BlockVar(base.DynamicVars["BlockAmount"].BaseValue, ValueProp.Move), null);

        CardModel burn1 = base.CombatState!.CreateCard<Burn>(base.Owner);
        await CardPileCmd.AddGeneratedCardToCombat(burn1, PileType.Draw, addedByPlayer: true);

        CardModel burn2 = base.CombatState!.CreateCard<Burn>(base.Owner);
        await CardPileCmd.AddGeneratedCardToCombat(burn2, PileType.Discard, addedByPlayer: true);

        await PowerCmd.Apply<ThermalShieldPower>(base.Owner.Creature, base.DynamicVars["ShieldAmount"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["BlockAmount"].UpgradeValueBy(2m);
        base.DynamicVars["ShieldAmount"].UpgradeValueBy(1m);
    }
}

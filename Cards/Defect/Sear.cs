#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BurningMod;

[Pool(typeof(DefectCardPool))]
public class Sear : BurningModCard
{
    private decimal _extraBurnFromSearPlays;

    private decimal ExtraBurnFromSearPlays
    {
        get => _extraBurnFromSearPlays;
        set
        {
            AssertMutable();
            _extraBurnFromSearPlays = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(2m, ValueProp.Move),
        new DynamicVar("BurnAmount", 2m),
        new DynamicVar("Increase", 2m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>() };

    public Sear() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    public override string? CustomPortraitPath => "res://BurningMod/images/cards/sear.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        await BurnApplyHelper.ApplyBurn(cardPlay.Target, base.DynamicVars["BurnAmount"].BaseValue, base.Owner.Creature, this);

        // Buff all Sear copies (Claw pattern)
        decimal increase = base.DynamicVars["Increase"].BaseValue;
        foreach (var sear in base.Owner.PlayerCombatState.AllCards.OfType<Sear>())
        {
            sear.BuffFromSearPlay(increase);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(1m);
        base.DynamicVars["BurnAmount"].UpgradeValueBy(1m);
        base.DynamicVars["Increase"].UpgradeValueBy(1m);
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        base.DynamicVars["BurnAmount"].BaseValue += ExtraBurnFromSearPlays;
    }

    private void BuffFromSearPlay(decimal extraBurn)
    {
        base.DynamicVars["BurnAmount"].BaseValue += extraBurn;
        ExtraBurnFromSearPlays += extraBurn;
    }
}

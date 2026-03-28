using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

#nullable enable
namespace BurningMod;

[Pool(typeof(SilentCardPool))]
public class Cremation : BurningModCard
{
    protected override IEnumerable<string> ExtraRunAssetPaths => NGroundFireVfx.AssetPaths;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6m, ValueProp.Move),
        new DynamicVar("BurnAmount", 6m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>() };

    public Cremation() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/cremation.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        var discard = PileType.Discard.GetPile(base.Owner).Cards;
        if (discard.Count == 0) return;

        CardModel? chosen = (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            discard,
            base.Owner,
            new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1)
        )).FirstOrDefault();

        if (chosen == null) return;

        int energyCost = chosen.EnergyCost.GetAmountToSpend();
        await CardCmd.Exhaust(choiceContext, chosen);

        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(cardPlay.Target));

        int hits = 1 + energyCost;
        for (int i = 0; i < hits; i++)
        {
            if (!cardPlay.Target.IsAlive) break;
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
            await BurnApplyHelper.ApplyBurn(cardPlay.Target, base.DynamicVars["BurnAmount"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
        base.DynamicVars["BurnAmount"].UpgradeValueBy(2m);
    }
}

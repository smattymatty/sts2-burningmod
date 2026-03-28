#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BurningMod;

[Pool(typeof(IroncladCardPool))]
public class TheFinalTorch : BurningModCard
{
    protected override IEnumerable<string> ExtraRunAssetPaths => NGroundFireVfx.AssetPaths;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new TorchDamageVar(10m, ValueProp.Move),
        new TorchBurnVar(4m),
        new DynamicVar("SelfBurn", 1m)
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[] { HoverTipFactory.FromPower<BurnPower>(), HoverTipFactory.FromKeyword(CardKeyword.Exhaust), HoverTipFactory.FromCard<TorchStrike>(), HoverTipFactory.FromCard<TorchSweep>() };

    public TheFinalTorch() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/the_final_torch.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await PowerCmd.Apply<TorchCounterPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
        int torchCount = base.Owner.Creature.GetPowerAmount<TorchCounterPower>();
        int multiplier = base.IsUpgraded ? 3 : 2;
        int torchBonus = torchCount * multiplier;
        Log.Info($"[TheFinalTorch] torchCount={torchCount} multiplier={multiplier} baseDmg={base.DynamicVars.Damage.BaseValue} totalDmg={base.DynamicVars.Damage.BaseValue + torchBonus}");

        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(cardPlay.Target));
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue + torchBonus)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        await BurnApplyHelper.ApplyBurn(cardPlay.Target, base.DynamicVars["EnemyBurn"].BaseValue + torchBonus, base.Owner.Creature, this);
        await BurnApplyHelper.ApplyBurn(base.Owner.Creature, base.DynamicVars["SelfBurn"].BaseValue, base.Owner.Creature, this);

        // Exhaust all Torch cards across hand, draw, and discard piles
        var allPiles = new[] { PileType.Hand, PileType.Draw, PileType.Discard };
        foreach (var pileType in allPiles)
        {
            var torchCards = pileType.GetPile(base.Owner).Cards
                .Where(c => c is TorchStrike or TorchSweep)
                .ToList();
            foreach (var torch in torchCards)
                await CardCmd.Exhaust(choiceContext, torch);
        }
    }

    protected override void OnUpgrade() { }
}

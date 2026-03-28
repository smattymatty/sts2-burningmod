#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BurningMod;

[Pool(typeof(SilentCardPool))]
public class SmokeAndEmbers : BurningModCard
{
    protected override IEnumerable<string> ExtraRunAssetPaths => NGroundFireVfx.AssetPaths;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("BurnPerCard", 2m),
        new DynamicVar("WeakAmount", 2m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<BurnPower>() };

    public SmokeAndEmbers() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    public override string? CustomPortraitPath => "res://BurningMod/images/cards/smoke_and_embers.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        System.ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        var hand = PileType.Hand.GetPile(base.Owner).Cards.ToList();
        foreach (var card in hand)
            await CardCmd.Discard(choiceContext, card);
        int discarded = hand.Count;
        if (discarded > 0)
        {
            foreach (var enemy in base.Owner.Creature.CombatState!.HittableEnemies)
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(enemy));

            await BurnApplyHelper.ApplyBurn(base.Owner.Creature.CombatState!.HittableEnemies, base.DynamicVars["BurnPerCard"].BaseValue * discarded, base.Owner.Creature, this);
        }
        await PowerCmd.Apply<WeakPower>(cardPlay.Target, base.DynamicVars["WeakAmount"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["BurnPerCard"].UpgradeValueBy(1m);
        base.DynamicVars["WeakAmount"].UpgradeValueBy(1m);
    }
}

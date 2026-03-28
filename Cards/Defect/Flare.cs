#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BurningMod;

[Pool(typeof(DefectCardPool))]
public class Flare : BurningModCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[]
        {
            HoverTipFactory.Static(StaticHoverTip.Channeling),
            HoverTipFactory.FromOrb<FireOrb>(),
            HoverTipFactory.FromPower<BurnPower>()
        };

    public Flare() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    public override string? CustomPortraitPath => "res://BurningMod/images/cards/flare.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await OrbCmd.Channel<FireOrb>(choiceContext, base.Owner);

        int burningEnemies = base.CombatState!.HittableEnemies
            .Count(e => e.GetPowerAmount<BurnPower>() > 0);

        int drawCount = burningEnemies + (base.IsUpgraded ? 1 : 0);

        if (drawCount > 0)
            await CardPileCmd.Draw(choiceContext, drawCount, base.Owner);
    }

    protected override void OnUpgrade() { }
}

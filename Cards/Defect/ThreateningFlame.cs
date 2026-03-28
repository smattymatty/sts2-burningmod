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
public class ThreateningFlame : BurningModCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new IHoverTip[]
        {
            HoverTipFactory.Static(StaticHoverTip.Channeling),
            HoverTipFactory.FromOrb<FireOrb>()
        };

    public ThreateningFlame() : base(2, CardType.Skill, CardRarity.Rare, TargetType.None) { }

    public override string? CustomPortraitPath => "res://BurningMod/images/cards/threatening_flame.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int attackCount = PileType.Hand.GetPile(base.Owner).Cards
            .Count(c => c.Type == CardType.Attack);

        for (int i = 0; i < attackCount; i++)
            await OrbCmd.Channel<FireOrb>(choiceContext, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}

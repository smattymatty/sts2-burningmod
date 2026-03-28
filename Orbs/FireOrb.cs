#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Utils;

namespace BurningMod;

public class FireOrb : CustomOrbModel
{
    private static readonly Color LightningLayerColor = new(0.8f, 0.1f, 0.0f, 1.0f);
    private static readonly Color GlassLayerColor = new(0.95f, 0.75f, 0.2f, 1.0f);

    public override Color DarkenedColor => new Color("996510");
    public override string? CustomIconPath => "res://BurningMod/images/powers/burn_power.png";
    public override bool IncludeInRandomPool => true;

    public override decimal PassiveVal => ModifyOrbValue(2m);
    public override decimal EvokeVal => ModifyOrbValue(4m);

    public override Node2D? CreateCustomSprite()
    {
        var container = new Node2D();

        // Back layer: lightning orb (red crackling aura), 10% bigger
        string lightningPath = SceneHelper.GetScenePath("orbs/orb_visuals/lightning_orb");
        Node2D lightning = PreloadManager.Cache.GetScene(lightningPath)
            .Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
        new MegaSprite(lightning.GetNode("SpineSkeleton"))
            .GetAnimationState().SetAnimation("idle_loop");
        lightning.Modulate = LightningLayerColor;
        lightning.Scale = new Vector2(1.1f, 1.1f);
        container.AddChild(lightning);

        // Front layer: glass orb (orange)
        string glassPath = SceneHelper.GetScenePath("orbs/orb_visuals/glass_orb");
        Node2D glass = PreloadManager.Cache.GetScene(glassPath)
            .Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
        new MegaSprite(glass.GetNode("SpineSkeleton"))
            .GetAnimationState().SetAnimation("idle_loop");
        glass.Modulate = GlassLayerColor;
        container.AddChild(glass);

        return container;
    }

    public override async Task BeforeTurnEndOrbTrigger(PlayerChoiceContext choiceContext)
    {
        await Passive(choiceContext, null);
    }

    public override async Task Passive(PlayerChoiceContext choiceContext, Creature? target)
    {
        Trigger();
        var enemies = CombatState.GetOpponentsOf(Owner.Creature)
            .Where(e => e.IsHittable).ToList();
        if (enemies.Count == 0) return;

        Creature passiveTarget = target ?? Owner.RunState.Rng.CombatTargets.NextItem(enemies)!;

        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(passiveTarget));
        PlayPassiveSfx();
        await CreatureCmd.Damage(choiceContext, new[] { passiveTarget }, PassiveVal, ValueProp.Unpowered, Owner.Creature);
        await BurnApplyHelper.ApplyBurn(passiveTarget, PassiveVal, Owner.Creature, null);
    }

    public override async Task<IEnumerable<Creature>> Evoke(PlayerChoiceContext choiceContext)
    {
        var enemies = CombatState.GetOpponentsOf(Owner.Creature)
            .Where(e => e.IsHittable).ToList();
        if (enemies.Count == 0) return enemies;

        Creature target = Owner.RunState.Rng.CombatTargets.NextItem(enemies)!;
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(target));
        PlayEvokeSfx();
        await CreatureCmd.Damage(choiceContext, new[] { target }, EvokeVal, ValueProp.Unpowered, Owner.Creature);
        await BurnApplyHelper.ApplyBurn(target, EvokeVal, Owner.Creature, null);

        return enemies;
    }
}

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

public sealed class InfernoOrb : CustomOrbModel
{
    public override Color DarkenedColor => new Color("8B1A00");
    public override string? CustomIconPath => "res://BurningMod/images/powers/inferno_power.png";
    public override bool IncludeInRandomPool => false;

    public override decimal PassiveVal => ModifyOrbValue(4m);
    public override decimal EvokeVal => 0m;

    public override Node2D? CreateCustomSprite()
    {
        var container = new Node2D();

        // Back layer: large dark red lightning aura
        string lightningPath = SceneHelper.GetScenePath("orbs/orb_visuals/lightning_orb");
        Node2D lightning = PreloadManager.Cache.GetScene(lightningPath)
            .Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
        new MegaSprite(lightning.GetNode("SpineSkeleton"))
            .GetAnimationState().SetAnimation("idle_loop");
        lightning.Modulate = new Color(0.6f, 0.05f, 0.0f, 1.0f);
        lightning.Scale = new Vector2(1.3f, 1.3f);
        container.AddChild(lightning);

        // Mid layer: plasma orb (deep black)
        string plasmaPath = SceneHelper.GetScenePath("orbs/orb_visuals/plasma_orb");
        Node2D plasma = PreloadManager.Cache.GetScene(plasmaPath)
            .Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
        new MegaSprite(plasma.GetNode("SpineSkeleton"))
            .GetAnimationState().SetAnimation("idle_loop");
        plasma.Modulate = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        container.AddChild(plasma);

        // Front layer: glass orb (bright red core)
        string glassPath = SceneHelper.GetScenePath("orbs/orb_visuals/glass_orb");
        Node2D glass = PreloadManager.Cache.GetScene(glassPath)
            .Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
        new MegaSprite(glass.GetNode("SpineSkeleton"))
            .GetAnimationState().SetAnimation("idle_loop");
        glass.Modulate = new Color(1.0f, 0.15f, 0.0f, 1.0f);
        glass.Scale = new Vector2(0.8f, 0.8f);
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

        PlayPassiveSfx();
        foreach (var enemy in enemies)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(enemy));
            await CreatureCmd.Damage(choiceContext, new[] { enemy }, PassiveVal, ValueProp.Unpowered, Owner.Creature);
            await BurnApplyHelper.ApplyBurn(enemy, PassiveVal, Owner.Creature, null);
        }
    }

    public override async Task<IEnumerable<Creature>> Evoke(PlayerChoiceContext choiceContext)
    {
        var enemies = CombatState.GetOpponentsOf(Owner.Creature)
            .Where(e => e.IsHittable).ToList();
        if (enemies.Count == 0) return enemies;

        PlayEvokeSfx();
        foreach (var enemy in enemies)
        {
            decimal burnStacks = enemy.GetPowerAmount<BurnPower>();
            if (burnStacks <= 0) continue;
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(enemy));
            await CreatureCmd.Damage(choiceContext, new[] { enemy }, burnStacks,
                ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature);
        }

        return enemies;
    }
}

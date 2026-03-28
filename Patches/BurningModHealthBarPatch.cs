// HP bar Burn damage preview patch
// Original concept and implementation by Wuwo (wuwo.casa)
// Ported from WuwoBurningCompatBridge to native Harmony by smattymatty
// Segments: Arsonist (unblockable, bright orange) + Regular Burn (blockable, amber)

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace BurningMod;

[HarmonyPatch(typeof(NHealthBar), "RefreshForeground")]
public static class BurningModHealthBarPatch
{
    private static readonly ConditionalWeakTable<NHealthBar, BurnBarState> _states = new();

    // Amber/gold for regular blockable burn
    private static readonly Color BurnColor = new(0.90f, 0.65f, 0.10f, 1.0f);
    // Bright orange-red for unblockable Arsonist burn
    private static readonly Color ArsonistColor = new(0.90f, 0.50f, 0.10f, 1.0f);

    private class BurnBarState
    {
        public NinePatchRect BurnForeground = null!;
        public NinePatchRect ArsonistForeground = null!;
    }

    static void Postfix(NHealthBar __instance)
    {
        try
        {
            var creature = AccessTools.Field(typeof(NHealthBar), "_creature")
                .GetValue(__instance) as Creature;
            if (creature == null || creature.CurrentHp <= 0) return;

            var burnPower = creature.GetPower<BurnPower>();

            var hpForeground = AccessTools.Field(typeof(NHealthBar), "_hpForeground")
                .GetValue(__instance) as Control;
            var poisonForeground = AccessTools.Field(typeof(NHealthBar), "_poisonForeground")
                .GetValue(__instance) as NinePatchRect;
            var doomForeground = AccessTools.Field(typeof(NHealthBar), "_doomForeground")
                .GetValue(__instance) as NinePatchRect;

            if (hpForeground == null || poisonForeground == null || doomForeground == null) return;

            var state = _states.GetOrCreateValue(__instance);

            // Create burn foreground node if needed
            if (state.BurnForeground == null || !GodotObject.IsInstanceValid(state.BurnForeground))
            {
                var burnFg = (NinePatchRect)poisonForeground.Duplicate();
                burnFg.Visible = false;
                burnFg.SelfModulate = BurnColor;
                burnFg.Modulate = BurnColor;
                poisonForeground.GetParent().AddChild(burnFg);
                poisonForeground.GetParent().MoveChild(burnFg, doomForeground.GetIndex());
                state.BurnForeground = burnFg;
            }

            // Create arsonist foreground node if needed
            if (state.ArsonistForeground == null || !GodotObject.IsInstanceValid(state.ArsonistForeground))
            {
                var arsonistFg = (NinePatchRect)poisonForeground.Duplicate();
                arsonistFg.Visible = false;
                arsonistFg.SelfModulate = ArsonistColor;
                arsonistFg.Modulate = ArsonistColor;
                poisonForeground.GetParent().AddChild(arsonistFg);
                // Arsonist goes before burn (closer to current HP)
                poisonForeground.GetParent().MoveChild(arsonistFg, state.BurnForeground.GetIndex());
                state.ArsonistForeground = arsonistFg;
            }

            var burnForeground = state.BurnForeground;
            var arsonistForeground = state.ArsonistForeground;
            burnForeground.Visible = false;
            arsonistForeground.Visible = false;

            if (burnPower == null) return;

            // GetFgWidth method reference
            var getFgWidth = AccessTools.Method(typeof(NHealthBar), "GetFgWidth",
                new[] { typeof(int) });

            float maxFgWidth = (float)(AccessTools.Property(typeof(NHealthBar), "MaxFgWidth")
                .GetValue(__instance) ?? 0f);

            int burnStacks = burnPower.CalculateTotalDamageNextTurn();
            int currentBlock = creature.Block;
            int poisonDamage = creature.GetPower<PoisonPower>()?.CalculateTotalDamageNextTurn() ?? 0;
            bool hasIntangible = creature.HasPower<IntangiblePower>();

            // Hardened Shell caps total HP loss per turn — DisplayAmount is remaining capacity
            var shellPower = creature.GetPower<HardenedShellPower>();
            int shellRemaining = shellPower?.DisplayAmount ?? int.MaxValue;

            // Poison is unblockable — goes straight to HP, consumes shell first
            int cappedPoisonDamage = Math.Min(poisonDamage, shellRemaining);
            shellRemaining -= cappedPoisonDamage;
            int hpAfterPoison = Math.Max(0, creature.CurrentHp - cappedPoisonDamage);

            // Check if any player has Arsonist active
            var combatState = creature.CombatState;
            bool hasArsonist = combatState != null && combatState.Players.Any(p =>
                p.Creature.HasPower<ArsonistPower>());

            // Arsonist damage — unblockable, fires at start of enemy turn (single hit)
            // Intangible caps each damage instance to 1
            int rawArsonistDamage = hasArsonist ? burnStacks : 0;
            int arsonistDamage = hasIntangible ? Math.Min(rawArsonistDamage > 0 ? 1 : 0, hpAfterPoison)
                : Math.Min(rawArsonistDamage, hpAfterPoison);
            arsonistDamage = Math.Min(arsonistDamage, shellRemaining);
            shellRemaining -= arsonistDamage;
            int hpAfterArsonist = Math.Max(0, hpAfterPoison - arsonistDamage);

            // Regular burn damage — blockable, fires at end of turn (single hit)
            // Intangible caps the damage instance to 1 (after block)
            int rawBurnDamage = hasIntangible ? Math.Min(burnStacks > 0 ? 1 : 0, burnStacks)
                : burnStacks;
            int effectiveBurnDamage = Math.Max(0, rawBurnDamage - currentBlock);
            int burnDamage = Math.Min(effectiveBurnDamage, hpAfterArsonist);
            burnDamage = Math.Min(burnDamage, shellRemaining);

            float fullOffsetRight = hpForeground.OffsetRight;

            // --- Arsonist segment ---
            if (arsonistDamage > 0)
            {
                int hpAfterPoisonAndArsonist = Math.Max(0, hpAfterPoison - arsonistDamage);

                if (arsonistDamage >= hpAfterPoison)
                {
                    // Lethal arsonist
                    arsonistForeground.OffsetLeft = 0f;
                    arsonistForeground.OffsetRight = fullOffsetRight;
                    arsonistForeground.Visible = true;
                    hpForeground.Visible = false;
                }
                else
                {
                    float futureWidthAfterArsonist = (float)(getFgWidth.Invoke(__instance,
                        new object[] { hpAfterPoisonAndArsonist }) ?? 0f);
                    float futureOffsetRightAfterArsonist = futureWidthAfterArsonist - maxFgWidth;

                    hpForeground.OffsetRight = futureOffsetRightAfterArsonist;
                    hpForeground.Visible = true;

                    arsonistForeground.OffsetLeft = Math.Max(0f,
                        futureWidthAfterArsonist - arsonistForeground.PatchMarginLeft);
                    arsonistForeground.OffsetRight = fullOffsetRight;
                    arsonistForeground.Visible = true;

                    // Update fullOffsetRight for burn segment to sit after arsonist
                    fullOffsetRight = futureOffsetRightAfterArsonist;
                }
            }

            // --- Regular burn segment ---
            if (burnDamage > 0 && hpAfterArsonist > 0)
            {
                int hpAfterAll = Math.Max(0, hpAfterArsonist - burnDamage);

                if (burnDamage >= hpAfterArsonist)
                {
                    // Lethal burn
                    burnForeground.OffsetLeft = 0f;
                    burnForeground.OffsetRight = fullOffsetRight;
                    burnForeground.Visible = true;
                    hpForeground.Visible = false;
                }
                else
                {
                    float futureWidth = (float)(getFgWidth.Invoke(__instance,
                        new object[] { hpAfterAll }) ?? 0f);
                    float futureOffsetRight = futureWidth - maxFgWidth;

                    hpForeground.OffsetRight = futureOffsetRight;
                    hpForeground.Visible = true;

                    burnForeground.OffsetLeft = Math.Max(0f,
                        futureWidth - burnForeground.PatchMarginLeft);
                    burnForeground.OffsetRight = fullOffsetRight;
                    burnForeground.Visible = true;
                }
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"[BurningMod] HP bar patch error: {e.Message}");
        }
    }
}
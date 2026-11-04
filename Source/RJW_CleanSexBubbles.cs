using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RJW_CleanSexBubbles
{
    [StaticConstructorOnStartup]
    public static class ModInit
    {
        static ModInit()
        {
            var harmony = new Harmony("epitaph.rjwcleansexbubbles");
            harmony.PatchAll();
            Log.Message("[RJW Clean Sex Bubbles] Initialized successfully. Redundant second sentences muted.");
        }
    }

    [HarmonyPatch(typeof(rjw.Modules.Interactions.SexInteraction), nameof(rjw.Modules.Interactions.SexInteraction.AddToPlayLog))]
    public static class Patch_SexInteraction_AddToPlayLog
    {
        [HarmonyPrefix]
        public static bool Prefix(rjw.Modules.Interactions.SexInteraction __instance, Pawn initiator, Pawn recipient)
        {
            // Pass null for extraSentencePacks to silence the generic second sentence ('X had sex with Y' / 'X was fucked by Y')
            // while preserving the detailed first sentence ('Orally serviced X' / 'Was fucked vaginally by Y').
            PlayLogEntry_Interaction playLogEntry = new PlayLogEntry_Interaction(__instance.Def, initiator, recipient, null);
            Find.PlayLog.Add(playLogEntry);
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayLogEntry_Interaction), MethodType.Constructor, new Type[] { typeof(InteractionDef), typeof(Pawn), typeof(Pawn), typeof(List<RulePackDef>) })]
    public static class Patch_PlayLogEntry_Interaction_Ctor
    {
        [HarmonyPrefix]
        public static void Prefix(InteractionDef intDef, ref List<RulePackDef> extraSentencePacks)
        {
            if (intDef != null && extraSentencePacks != null && extraSentencePacks.Count > 0)
            {
                // Check if this interaction belongs to RJW sex interactions
                if (intDef.HasModExtension<rjw.SexInteractionExtension>() ||
                    intDef.defName.StartsWith("Sex_") ||
                    intDef.defName.StartsWith("Bestiality_") ||
                    intDef.defName.StartsWith("Rape_") ||
                    intDef.defName.StartsWith("Necro_") ||
                    intDef.defName.StartsWith("Masturbation_"))
                {
                    extraSentencePacks = null;
                }
            }
        }
    }
}

using Harmony;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;

namespace AdjustableColoredLights
{
    [StaticConstructorOnStartup]
    class Main
    {
        internal static Texture2D IconTexture;
        internal static Texture2D BlackTexture;

        public static bool IsColoredLightsResearched = false;

        static Main()
        {
            var harmony = HarmonyInstance.Create("com.AdjustableColoredLights.rimworld.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Log.Message("AdjustableColoredLights: Adding Harmony Postfix to CompGlower.CompGetGizmosExtra");
            Log.Message("AdjustableColoredLights: Adding Harmony Postfix to CompGlower.PostExposeData");

            IconTexture = ContentFinder<Texture2D>.Get("UI/changecolor", true);

            BlackTexture = new Texture2D(1, 1);
            BlackTexture.SetPixels(new Color[] { new Color(0.0823f, 0.098f, 0.1137f) });
            BlackTexture.Apply();
        }

        public static bool IsLight(Thing t)
        {
            string defName = t?.def.defName;
            return 
                defName != null && 
                Main.IsColoredLightsResearched &&
                (defName.Contains("Lamp") || defName.Contains("Light"));
        }
    }

    [HarmonyPatch(typeof(CompGlower), "CompGetGizmosExtra")]
    static class CompGlower_CompGetGizmosExtra
    {
        static void Postfix(CompGlower __instance, ref IEnumerable<Gizmo> __result)
        {
            if (Main.IsLight(__instance.parent))
            {
                List<Gizmo> l = new List<Gizmo>();
                if (__result != null)
                {
                    l.AddRange(__result);
                }

                l.Add(new Command_Action
                {
                    icon = Main.IconTexture,
                    defaultDesc = "AdjustableColoredLights.ChangeColorDesc".Translate(),
                    defaultLabel = "AdjustableColoredLights.ChangeColor".Translate(),
                    activateSound = SoundDef.Named("Click"),
                    action = delegate { Find.WindowStack.Add(new Dialog_FloatColorPicker(__instance)); },
                    groupKey = "AdjustableColoredLights".GetHashCode()
                });

                __result = l;
            }
        }
    }

    [HarmonyPatch(typeof(CompGlower), "PostExposeData")]
    static class CompGlower_PostExposeData
    {
        static void Postfix(CompGlower __instance)
        {
            if (Main.IsLight(__instance.parent))
            {
                ColorInt c = __instance.Props.glowColor;
                int r = c.r;
                int g = c.g;
                int b = c.b;
                Scribe_Values.Look<int>(ref r, "glowColor.r", 0, false);
                Scribe_Values.Look<int>(ref g, "glowColor.g", 0, false);
                Scribe_Values.Look<int>(ref b, "glowColor.b", 0, false);
                __instance.Props.glowColor = new ColorInt(r, g, b);
            }
        }
    }

    /*[HarmonyPatch(typeof(Game), "InitNewGame")]
    static class Patch_Game_InitNewGame
    {
        static void Postfix()
        {
            Main.IsColoredLightsResearched = false;
        }
    }*/

    [HarmonyPatch(typeof(ResearchManager), "ReapplyAllMods")]
    static class Patch_ResearchManager_ReapplyAllMods
    {
        static void Postfix()
        {
            foreach (ResearchProjectDef def in DefDatabase<ResearchProjectDef>.AllDefs)
            {
                if (def.defName.Equals("ColoredLights"))
                {
                    Main.IsColoredLightsResearched = def.IsFinished;
                    return;
                }
            }
        }
    }
}
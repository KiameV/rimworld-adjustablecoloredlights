using Harmony;
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

        static Main()
        {
            var harmony = HarmonyInstance.Create("com.AdjustableColoredLights.rimworld.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Log.Message("AdjustableColoredLights: Adding Harmony Postfix to CompGlower.CompGetGizmosExtra");

            IconTexture = ContentFinder<Texture2D>.Get("UI/changecolor", true);

            BlackTexture = new Texture2D(1, 1);
            BlackTexture.SetPixels(new Color[] { new Color(0.0823f, 0.098f, 0.1137f) });
            BlackTexture.Apply();
        }
    }

    [HarmonyPatch(typeof(CompGlower), "CompGetGizmosExtra")]
    static class CompGlower_CompGetGizmosExtra
    {
        static void Postfix(CompGlower __instance, ref IEnumerable<Gizmo> __result)
        {
            string defName = __instance.parent?.def.defName;
            if (defName != null &&
                (defName.Contains("Light") || defName.Contains("Lamp")))
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
                    groupKey = 887767542
                });

                Color rgb = __instance.Props.glowColor.ToColor;

                __result = l;
            }
        }
    }
}
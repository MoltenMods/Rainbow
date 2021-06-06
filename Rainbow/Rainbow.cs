using System.Linq;
using Assets.CoreScripts;
using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using Rainbow.Patches;
using Rainbow.Types;
using Reactor;

namespace Rainbow
{
    [BepInPlugin(Id, Name, Version)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public class RainbowPlugin : BasePlugin
    {
        public const string Id = "daemon.rainbow";
        public const string Name = "Rainbow";
        public const string Version = "0.1.0";

        public Harmony Harmony { get; } = new Harmony(Id);

        public override void Load()
        {
            RegisterCustomRpcAttribute.Register(this);
            RegisterInIl2CppAttribute.Register();
            
            ColorSelectionPatches.Initialize();
            
            Harmony.PatchAll();
        }
    }

    public static class Rainbow
    {
        public static void AddColors(CustomColor[] customColors)
        {
            var frontColors = Palette.PlayerColors.ToList();
            var backColors = Palette.ShadowColors.ToList();
            var colorNames = Palette.ColorNames.ToList();
            var shortColorNames = Palette.ShortColorNames.ToList();
            
            foreach (var customColor in customColors)
            {
                if (customColor.Hidden) continue;
                
                frontColors.Add(customColor.FrontColor);
                backColors.Add(customColor.BackColor);
                colorNames.Add(customColor.ColorName);
                shortColorNames.Add(customColor.ShortColorName);
            }
            
            Palette.PlayerColors = frontColors.ToArray();
            Palette.ShadowColors = backColors.ToArray();
            Palette.ColorNames = colorNames.ToArray();
            Palette.ShortColorNames = shortColorNames.ToArray();
            MedScanMinigame.ColorNames = Palette.ColorNames;
            Telemetry.ColorNames = Palette.ColorNames;
            
            ColorSelectionPatches.CustomColors.AddRange(customColors);
        }

        public static void AddColor(CustomColor customColor)
        {
            Palette.PlayerColors = Palette.PlayerColors.AddItem(customColor.FrontColor).ToArray();
            Palette.ShadowColors = Palette.ShadowColors.AddItem(customColor.BackColor).ToArray();
            Palette.ColorNames = Palette.ColorNames.AddItem(customColor.ColorName).ToArray();
            Palette.ShortColorNames = Palette.ShortColorNames.AddItem(customColor.ShortColorName).ToArray();
            MedScanMinigame.ColorNames = Palette.ColorNames;
            Telemetry.ColorNames = Palette.ColorNames;
            
            ColorSelectionPatches.CustomColors.Add(customColor);
        }
    }
    
    // ==============================
    // REMEMBER TO REMOVE THIS CODE
    // DO NOT KEEP THIS CODE
    // THIS IS NOT RAINBOW CODE
    // UNDER NO CIRCUMSTANCES SHOULD YOU SHIP THIS CODE
    // ==============================
    [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.GetPurchase))]
    public static class EveryPurchasePatch
    {
        public static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(PrivacyPolicyScreen), nameof(PrivacyPolicyScreen.Show))]
    public static class StopPrivacyPolicyScreenShowingEveryTimeForNoReasonDespiteMeAcceptingItOneMillionTimesPatch
    {
        // It seems that the game resets `SaveManager.AcceptedPrivacyPolicy` every time you launch it, so this code
        // ensures it's set to 1 (true) just before the game checks to see if it needs to show the popup.
        public static void Prefix()
        {
            SaveManager.AcceptedPrivacyPolicy = 1;
        }
    }
}
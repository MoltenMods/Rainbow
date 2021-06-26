using System.Linq;
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

            foreach (var customColor in customColors)
            {
                if (customColor.Hidden) continue;
                
                frontColors.Add(customColor.FrontColor);
                backColors.Add(customColor.BackColor);
                colorNames.Add(customColor.ColorName);
            }
            
            Palette.PlayerColors = frontColors.ToArray();
            Palette.ShadowColors = backColors.ToArray();
            Palette.ColorNames = colorNames.ToArray();
            // Telemetry.ColorNames = Palette.ColorNames;
            
            ColorSelectionPatches.CustomColors.AddRange(customColors);
        }

        public static void AddColor(CustomColor customColor)
        {
            Palette.PlayerColors = Palette.PlayerColors.AddItem(customColor.FrontColor).ToArray();
            Palette.ShadowColors = Palette.ShadowColors.AddItem(customColor.BackColor).ToArray();
            Palette.ColorNames = Palette.ColorNames.AddItem(customColor.ColorName).ToArray();
            // Telemetry.ColorNames = Palette.ColorNames;
            
            ColorSelectionPatches.CustomColors.Add(customColor);
        }
    }
}

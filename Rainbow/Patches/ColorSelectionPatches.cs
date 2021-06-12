using System;
using System.Collections.Generic;
using HarmonyLib;
using Rainbow.Extensions;
using Rainbow.MonoBehaviours;
using Rainbow.Types;
using Reactor.Extensions;
using UnityEngine;

namespace Rainbow.Patches
{
    internal static class ColorSelectionPatches
    {
        public static readonly int OriginalPaletteLength = Palette.PlayerColors.Length;
        public static readonly List<CustomColor> CustomColors = new List<CustomColor>();

        public static void Initialize()
        {
            var newColors = new CustomColor[]
            {
                // may have stolen some official colours before they were released
                new NormalColor(new Color32(115, 132, 148, Byte.MaxValue), 
                    new Color32(66, 82, 99, Byte.MaxValue), 
                    "Gray", "GRAY"),
                new NormalColor(new Color32(0, 128, 128, Byte.MaxValue), 
                    new Color32(0, 100, 100, Byte.MaxValue), 
                    "Teal", "TEAL"),
                new NormalColor(new Color32(236, 117, 120, Byte.MaxValue), 
                    new Color32(180, 67, 98, Byte.MaxValue), 
                    "Coral", "CORL"),
                new NormalColor(new Color32(99, 114, 24, Byte.MaxValue), 
                    new Color32(66, 91, 15, Byte.MaxValue), 
                    "Olive", "OLIV"),
                new NormalColor(new Color32(241, 195, 209, Byte.MaxValue), 
                    new Color32(225, 155, 177, Byte.MaxValue), 
                    "Rose", "ROSE"),
                new NormalColor(new Color32(176, 48, 96, Byte.MaxValue),
                    new Color32(112, 29, 60, Byte.MaxValue),
                    "Light Maroon", "LTMRN"),
                new NormalColor(new Color32(218, 165, 32, Byte.MaxValue), 
                    new Color32(156, 117, 22, Byte.MaxValue), 
                    "Gold", "GOLD"),
                new NormalColor(new Color32(168, Byte.MaxValue, 195, Byte.MaxValue), 
                    new Color32(123, 186, 143, Byte.MaxValue), 
                    "Mint", "MINT"),
                new NormalColor(new Color32(201, 146, 224, Byte.MaxValue), 
                    new Color32(156, 113, 173, Byte.MaxValue), 
                    "Lavender", "LVDR"),
                new NormalColor(new Color32(102, 35, 60, Byte.MaxValue),
                    new Color32(63, 9, 25, Byte.MaxValue),
                    "Maroon", "MARN"),
                new NormalColor(new Color32(144, 133, 116, Byte.MaxValue),
                    new Color32(82, 66, 59, Byte.MaxValue),
                    "Tan", "TAN"),
                new NormalColor(new Color32(254, 255, 188, Byte.MaxValue),
                    new Color32(204, 190, 138, Byte.MaxValue),
                    "Banana", "BNNA"),
                new CyclicColor(new[]
                {
                    // 0.99f is used instead of 1f, because HSV(0, a, b) is apparently the same as HSV(1, a, b), which
                    // is probably because it does a loopy loop. I spent a while trying to debug this.
                    new ColorStop(new HueColor(0f, 0.8f, 0.8f).ToRgbColor(), 
                        new HueColor(0f, 0.8f, 0.5f).ToRgbColor(), 
                        new HueShift(2.5f)),
                    new ColorStop(new HueColor(0.99f, 0.8f, 0.8f).ToRgbColor(),
                        new HueColor(0.99f, 0.8f, 0.5f).ToRgbColor())
                }, "Rainbow", "RNBW")/*,
                // not removing this code since I use it for testing (and I'm a data hoarder)
                new CyclicColor(new[]
                {
                    new ColorStop(new HueColor(0f, 0.8f, 0.5f).ToRgbColor(),
                        new HueColor(0f, 0.8f, 0.2f).ToRgbColor(),
                        new HueShift(1f)),
                    new ColorStop(new HueColor(0f, 0.8f, 1f).ToRgbColor(),
                        new HueColor(0f, 0.8f, 0.7f).ToRgbColor(),
                        new HueShift(1f))
                }, "Red Hot", "RDHT"),
                new CyclicColor(new []
                {
                    new ColorStop(new Color(1f, 0f, 0f, Byte.MaxValue),
                        new Color(1f, 0f, 0f, Byte.MaxValue), 5f),
                    new ColorStop(new Color(1f, 1f, 0f, Byte.MaxValue),
                        new Color(1f, 1f, 0f, Byte.MaxValue), 3f),
                    new ColorStop(new Color(0f, 1f, 0f, Byte.MaxValue),
                        new Color(0f, 1f, 0f, Byte.MaxValue), 5f)
                }, "Traffic Lights", "TRLT")*/
            };

            Rainbow.AddColors(newColors);
        }

        [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.OnEnable))]
        public static class PlayerTabDisplayPatch
        {
            public static void Postfix(PlayerTab __instance)
            {
                __instance.ColorTabPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
                __instance.XRange.min = 1.4f;
                __instance.XRange.max = 3.3f;
                
                foreach (var colorChip in __instance.ColorChips)
                {
                    colorChip.gameObject.Destroy();
                }
                __instance.ColorChips.Clear();

                for (var i = 0; i < Palette.PlayerColors.Length; i++)
                {
                    var isCustomColor = CustomColorExtensions.TryGetCustomColorById(i, out var customColor);
                    if (isCustomColor && customColor.Hidden) continue;
                    
                    var x = __instance.XRange.Lerp(i % 5 / 4f);
                    var y = __instance.YRange.Lerp(1f - i / 5 / 5.5f);
                    
                    var colorChip = UnityEngine.Object.Instantiate(__instance.ColorTabPrefab, __instance.transform);
                    colorChip.transform.localPosition = new Vector3(x, y, -1f);

                    var j = i;
                    colorChip.Button.OnClick.AddListener((Action) delegate
                    {
                        __instance.SelectColor(j);
                    });

                    if (isCustomColor && customColor.Type == CustomColorTypes.Cyclic)
                    {
                        colorChip.gameObject.SetCyclicColor(customColor as CyclicColor);
                    }
                    else
                    {
                        colorChip.gameObject.ClearCyclicColor();
                    }
                    
                    colorChip.Inner.color = Palette.PlayerColors[i];
                    __instance.ColorChips.Add(colorChip);
                }
            }
        }
    }
}

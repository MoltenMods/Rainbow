﻿using HarmonyLib;
using Rainbow.Extensions;
using Rainbow.Net;
using Rainbow.Types;
using Reactor.Networking;
using UnityEngine;
using Object = Il2CppSystem.Object;

namespace Rainbow.Patches
{
    internal class SetColorPatches
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckColor))]
        public static class CmdCheckColorPatch
        {
            public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] byte bodyColor)
            {
                var translator = TranslationController.Instance;

                var frontColor = Palette.PlayerColors[bodyColor];
                var backColor = Palette.ShadowColors[bodyColor];
                var colorName = translator.GetString(Palette.ColorNames[bodyColor], new Object[0]);

                if (AmongUsClient.Instance.AmClient)
                    __instance.SetCustomColor(frontColor, backColor, colorName);

                Rpc<SetColorRpc>.Instance.Send(new SetColorRpc.Data(frontColor, backColor, colorName));

                return false;
            }
        }

        [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.UpdateAvailableColors))]
        public static class AllColorsAvailablePatch
        {
            public static void Postfix(PlayerTab __instance)
            {
                PlayerControl.SetPlayerMaterialColors(PlayerControl.LocalPlayer.Data.ColorId, __instance.DemoImage);
                PlayerControl.SetPetImage(SaveManager.LastPet, PlayerControl.LocalPlayer.Data.ColorId,
                    __instance.PetImage);
                
                for (var i = 0; i < Palette.PlayerColors.Length; i++)
                {
                    __instance.AvailableColors.Add(i);
                }

                __instance.AvailableColors.Remove(PlayerControl.LocalPlayer.Data.ColorId);
            }
        }
        
        [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.SelectColor))]
        public static class SetHatColorPatch
        {
            // Not sure why this patch is needed. It basically just does what the normal code does, but somehow makes
            // the mod work properly
            public static bool Prefix(PlayerTab __instance, [HarmonyArgument(0)] int colorId)
            {
                __instance.UpdateAvailableColors();

                var color = (byte) colorId;

                SaveManager.BodyColor = color;
                if (PlayerControl.LocalPlayer) PlayerControl.LocalPlayer.CmdCheckColor(color);

                return false;
            }
            
            // Because the hat in the preview doesn't change color for some reason when selecting your color
            public static void Postfix(PlayerTab __instance, [HarmonyArgument(0)] int colorId)
            {
                __instance.HatImage.SetColor(colorId);
            }
        }
        
        // Probably the most important piece of code in this mod. It's so important, I even decided to write this
        // comment explaining how important it is. That's pretty important if you ask me.
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetPlayerMaterialColors), typeof(int), typeof(Renderer))]
        public static class SetMaterialColorsPatch
        {
            public static bool Prefix([HarmonyArgument(0)] int colorId, [HarmonyArgument(1)] Renderer rend)
            {
                if (CustomColorExtensions.TryGetCustomColorById(colorId, out var customColor) &&
                    customColor.Type == CustomColorTypes.Cyclic)
                {
                    rend.gameObject.SetCyclicVisorColor(customColor as CyclicColor);
                    return false;
                }

                rend.gameObject.ClearCyclicColor();
                return true;
            }
        }
    }
}
using Rainbow.Types;
using Reactor;
using UnityEngine;

namespace Rainbow.Extensions
{
    public static class PlayerControlExtensions
    {
        public static void SetCustomColor(this PlayerControl player, Color frontColor, Color backColor, string colorName)
        {
            Logger<RainbowPlugin>.Info(
                $"Setting player color for '{player.Data.PlayerName}': " +
                $"front color = {frontColor.ToString()}, back color = {backColor.ToString()}, " +
                $"name = {colorName}");

            player.myRend.gameObject.ClearCyclicColor();
            player.HatRenderer.FrontLayer.gameObject.ClearCyclicColor();
            player.HatRenderer.BackLayer.gameObject.ClearCyclicColor();
            if (player.CurrentPet) player.CurrentPet.gameObject.ClearCyclicColor();

            var colorIndex = Palette.PlayerColors.IndexOf(frontColor);
            if (colorIndex == -1)
            {
                Rainbow.AddColor(new NormalColor(frontColor, backColor, colorName, true));
            }
            
            player.SetColor(colorIndex);
        }
        
        public static bool TryGetCustomColor(this PlayerControl playerControl, out CustomColor customColor)
        {
            return CustomColorExtensions.TryGetCustomColorById(playerControl.Data.ColorId, out customColor);
        }
    }
}
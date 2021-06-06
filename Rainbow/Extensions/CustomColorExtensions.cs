using Rainbow.Patches;
using Rainbow.Types;
using Reactor;

namespace Rainbow.Extensions
{
    public static class CustomColorExtensions
    {
        public static bool TryGetCustomColorById(int colorId, out CustomColor customColor)
        {
            var originalPaletteLength = ColorSelectionPatches.OriginalPaletteLength;

            var isCustomColor = colorId >= originalPaletteLength;
            customColor = isCustomColor ? ColorSelectionPatches.CustomColors[colorId - originalPaletteLength] : null;

            return isCustomColor;
        }
    }
}
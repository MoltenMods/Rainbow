using Reactor;
using UnityEngine;

namespace Rainbow.Extensions
{
    public static class ColorExtensions
    {
        public static HueColor ToHueColor(this Color color)
        {
            Color.RGBToHSV(color, out var h, out var s, out var v);
            return new HueColor(h, s, v);
        }
    }

    public class HueColor
    {
        public float H { get; }
        public float S { get; }
        public float V { get; }

        public HueColor(float h, float s, float v)
        {
            H = h;
            S = s;
            V = v;
        }

        public Color ToRgbColor()
        {
            return Color.HSVToRGB(H, S, V);
        }

        public static HueColor Lerp(HueColor startValue, HueColor endValue, float interpolationValue)
        {
            var clampedInterpolationValue = Mathf.Clamp(interpolationValue, 0, 1);

            var h = Mathf.Lerp(startValue.H, endValue.H, clampedInterpolationValue);
            var s = Mathf.Lerp(startValue.S, endValue.S, clampedInterpolationValue);
            var v = Mathf.Lerp(startValue.V, endValue.V, clampedInterpolationValue);

            return new HueColor(h, s, v);
        }

        public override string ToString()
        {
            return $"HSV({H}, {S}, {V})";
        }
    }
}
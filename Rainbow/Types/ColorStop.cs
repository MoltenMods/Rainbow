using UnityEngine;

namespace Rainbow.Types
{
    public class ColorStop
    {
        public Color FrontColor { get; }
        public Color BackColor { get; }
        
        public Transition Transition { get; }
        public float Length { get; }

        public ColorStop(Color frontColor, Color backColor, Transition transition = null, float length = 0f)
        {
            FrontColor = frontColor;
            BackColor = backColor;

            Transition = transition;
            Length = length;
        }
        
        public ColorStop(Color frontColor, Color backColor, float length)
        {
            FrontColor = frontColor;
            BackColor = backColor;
            
            Length = length;
        }
    }

    public class RgbShift : Transition
    {
        public override TransitionTypes Type => TransitionTypes.RgbShift;
        
        public RgbShift(float length) : base(length) {}
    }

    public class HueShift : Transition
    {
        public override TransitionTypes Type => TransitionTypes.HueShift;
        
        public HueShift(float length) : base(length) {}
    }

    public abstract class Transition
    {
        public abstract TransitionTypes Type { get; }
        public float Length { get; }
        
        public Transition(float length)
        {
            Length = length;
        }
    }

    public enum TransitionTypes
    {
        RgbShift,
        HueShift
    }
}
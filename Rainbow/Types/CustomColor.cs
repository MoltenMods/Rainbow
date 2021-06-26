using System;
using System.Collections;
using Rainbow.Extensions;
using Reactor;
using UnityEngine;

namespace Rainbow.Types
{
    public class NormalColor : CustomColor
    {
        public override CustomColorTypes Type => CustomColorTypes.Normal;

        public NormalColor(Color frontColor, Color backColor, string colorName, bool hidden = false)
        {
            FrontColor = frontColor;
            BackColor = backColor;
            
            ColorName = CustomStringName.Register(colorName);

            Hidden = hidden;
        }
    }

    public class CyclicColor : CustomColor
    {
        public override CustomColorTypes Type => CustomColorTypes.Cyclic;
        
        public ColorStop[] ColorStops { get; }

        public CyclicColor(ColorStop[] colorStops, string colorName, bool hidden = false)
        {
            ColorStops = colorStops;

            ColorName = CustomStringName.Register(colorName);

            Hidden = hidden;

            Coroutines.Start(Cycle());
        }

        private IEnumerator Cycle()
        {
            var lastStopTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            for (var i = 0;; i++)
            {
                i %= ColorStops.Length;
                var colorStop = ColorStops[i];

                FrontColor = colorStop.FrontColor;
                BackColor = colorStop.BackColor;

                if (colorStop.Length != 0) yield return new WaitForSeconds(colorStop.Length);

                if (colorStop.Transition != null)
                {
                    var nextColorStop = ColorStops[(i + 1) % ColorStops.Length];

                    var transitionTime = colorStop.Transition.Length * 1000;
                    var difference = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - lastStopTime;
                    
                    switch (colorStop.Transition.Type)
                    {
                        case TransitionTypes.RgbShift:
                        {
                            while (difference < transitionTime)
                            {
                                var progress = difference / transitionTime;
                                FrontColor = Color.Lerp(colorStop.FrontColor, nextColorStop.FrontColor, progress);
                                BackColor = Color.Lerp(colorStop.BackColor, nextColorStop.BackColor, progress);

                                yield return new WaitForEndOfFrame();

                                difference = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - lastStopTime;
                            }

                            break;
                        }
                        case TransitionTypes.HueShift:
                        {
                            var frontStartColor = colorStop.FrontColor.ToHueColor();
                            var frontEndColor = nextColorStop.FrontColor.ToHueColor();
                            var backStartColor = colorStop.BackColor.ToHueColor();
                            var backEndColor = nextColorStop.BackColor.ToHueColor();

                            while (difference < transitionTime)
                            {
                                var progress = difference / transitionTime;
                                FrontColor = HueColor.Lerp(frontStartColor, frontEndColor, progress).ToRgbColor();
                                BackColor = HueColor.Lerp(backStartColor, backEndColor, progress).ToRgbColor();
                                
                                yield return new WaitForEndOfFrame();

                                difference = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - lastStopTime;
                            }

                            break;
                        }
                    }
                }
                
                lastStopTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
        }
    }
    
    public abstract class CustomColor
    {
        public abstract CustomColorTypes Type { get; }

        public Color FrontColor { get; protected set; }
        public Color BackColor { get; protected set; }
        
        public StringNames ColorName { get; protected set; }

        public bool Hidden { get; protected set; }
    }

    public enum CustomColorTypes
    {
        Normal,
        Cyclic
    }
}
using System;
using Rainbow.Types;
using Reactor;
using UnityEngine;

namespace Rainbow.MonoBehaviours
{
    [RegisterInIl2Cpp]
    public class DynamicVisorColor : MonoBehaviour
    {
        private static readonly int BodyColor = Shader.PropertyToID("_BodyColor");
        private static readonly int BackColor = Shader.PropertyToID("_BackColor");
        private static readonly int VisorColor = Shader.PropertyToID("_VisorColor");
        
        public SpriteRenderer SpriteRenderer { get; set; }

        public CyclicColor CyclicColor { get; set; }
        
        public DynamicVisorColor(IntPtr ptr) : base(ptr) {}

        private void Awake()
        {
            SpriteRenderer ??= gameObject.GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (SpriteRenderer is null || CyclicColor is null) return;

            SpriteRenderer.material.SetColor(BodyColor, CyclicColor.FrontColor);
            SpriteRenderer.material.SetColor(BackColor, CyclicColor.BackColor);
            SpriteRenderer.material.SetColor(VisorColor, Palette.VisorColor);
        }
    }
    
    [RegisterInIl2Cpp]
    public class DynamicColor : MonoBehaviour
    {
        public SpriteRenderer SpriteRenderer { get; set; }

        public CyclicColor CyclicColor { get; set; }
        
        public DynamicColor(IntPtr ptr) : base(ptr) {}

        private void Awake()
        {
            SpriteRenderer ??= gameObject.GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (SpriteRenderer is null || CyclicColor is null) return;

            SpriteRenderer.color = CyclicColor.FrontColor;
        }
    }
}
using System;
using System.Collections;
using UnityEngine;

namespace Utils
{
    public static class AnimationUtils
    {
        public static readonly Vector3 AlmostZero = Vector3.one * 0.001f;
        
        public static IEnumerator Animate(float duration, Action<float> animation, Func<float, float> easing = null)
        {
            var startTime = Time.unscaledTime;
            easing ??= static (t) => t;
            
            var t = 0f;
            animation(easing(0f));
            
            while (t < 1f)
            {
                t = Mathf.Clamp01((Time.unscaledTime - startTime) / duration);
                animation(easing(t));
                yield return null;
            }
        }
    }
}
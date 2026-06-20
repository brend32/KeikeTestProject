using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Interpolation functions
    /// </summary>
    public static class Easing
    {
        public static readonly Func<float, float> Linear = LinearFunction;
        public static readonly Func<float, float> Step = StepFunction;
        
        public static readonly Func<float, float> SineIn = SineInFunction;
        public static readonly Func<float, float> SineOut = SineOutFunction;
        public static readonly Func<float, float> SineInOut = SineInOutFunction;
        
        public static readonly Func<float, float> CubicIn = CubicInFunction;
        public static readonly Func<float, float> CubicOut = CubicOutFunction;
        public static readonly Func<float, float> CubicInOut = CubicInOutFunction;
        
        public static readonly Func<float, float> QuadIn = QuadInFunction;
        public static readonly Func<float, float> QuadOut = QuadOutFunction;
        public static readonly Func<float, float> QuadInOut = QuadInOutFunction;
        
        public static readonly Func<float, float> QuartIn = QuartInFunction;
        public static readonly Func<float, float> QuartOut = QuartOutFunction;
        public static readonly Func<float, float> QuartInOut = QuartInOutFunction;
        
        public static readonly Func<float, float> QuintIn = QuintInFunction;
        public static readonly Func<float, float> QuintOut = QuintOutFunction;
        public static readonly Func<float, float> QuintInOut = QuintInOutFunction;
        
        public static readonly Func<float, float> BounceOut25 = BounceOut25Function;
        public static readonly Func<float, float> BounceOut20 = BounceOut20Function;
        public static readonly Func<float, float> BounceOut15 = BounceOut15Function;
        
        public static readonly Func<float, float> OutBack = OutBackFunction;
        
        public static readonly Dictionary<string, Func<float, float>> Functions = new()
        {
            {nameof(Linear), Linear},
            {nameof(Step), Step},
            
            {nameof(SineIn), SineIn},
            {nameof(SineOut), SineOut},
            {nameof(SineInOut), SineInOut},
            
            {nameof(CubicIn), CubicIn},
            {nameof(CubicOut), CubicOut},
            {nameof(CubicInOut), CubicInOut},
            
            {nameof(QuadIn), QuadIn},
            {nameof(QuadOut), QuadOut},
            {nameof(QuadInOut), QuadInOut},
            
            {nameof(QuartIn), QuartIn},
            {nameof(QuartOut), QuartOut},
            {nameof(QuartInOut), QuartInOut},
            
            {nameof(QuintIn), QuintIn},
            {nameof(QuintOut), QuintOut},
            {nameof(QuintInOut), QuintInOut},
            
            {nameof(BounceOut25), BounceOut25},
            {nameof(BounceOut20), BounceOut20},
            {nameof(BounceOut15), BounceOut15},
            
            {nameof(OutBack), OutBack},
        };
        
        public static float LinearFunction(float t) => t;
        public static float StepFunction(float t) => t >= 1 ? 1 : 0;
        
        public static float SineInFunction(float t) => ToEaseIn(SineOutFunction)(t);
        public static float SineOutFunction(float t) => Mathf.Sin(t * Mathf.PI / 2f);
        public static float SineInOutFunction(float t) => ToEaseInOut(SineOutFunction)(t);
        
        public static float CubicInFunction(float t) => ToEaseIn(CubicOutFunction)(t);
        public static float CubicOutFunction(float t) => 1 + (--t) * t * t;
        public static float CubicInOutFunction(float t) => ToEaseInOut(CubicOutFunction)(t);
        
        public static float QuadInFunction(float t) => ToEaseIn(QuadOutFunction)(t);
        public static float QuadOutFunction(float t) => 1 - Mathf.Pow(1 - t, 2f);
        public static float QuadInOutFunction(float t) => ToEaseInOut(QuadOutFunction)(t);

        public static float QuartInFunction(float t) => ToEaseIn(QuartOutFunction)(t);
        public static float QuartOutFunction(float t) => 1 - Mathf.Pow(1 - t, 4f);
        public static float QuartInOutFunction(float t) => ToEaseInOut(QuartOutFunction)(t);

        public static float QuintInFunction(float t) => ToEaseIn(QuintOutFunction)(t);
        public static float QuintOutFunction(float t) => 1 + (--t) * t * t * t * t;
        public static float QuintInOutFunction(float t) => ToEaseInOut(QuintOutFunction)(t);

        public static float BounceOut25Function(float t) => 49f / 40f - Mathf.Pow(t - 7f / 10f, 2) * 2.5f;
        public static float BounceOut20Function(float t) => 9f / 8f - Mathf.Pow(t - 3f / 4f, 2) * 2;
        public static float BounceOut15Function(float t) => 25f / 24f - Mathf.Pow(t - 5f / 6f, 2) * 1.5f;

        public static float OutBackFunction(float t) {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;

            return 1 + c3 * Mathf.Pow(t - 1, 3f) + c1 * Mathf.Pow(t - 1, 2f);
        }

        public static Func<float, float> ToEaseIn(Func<float, float> easeOut)
        {
            return t => 1 - easeOut(1 - t);
        }

        public static Func<float, float> ToEaseInOut(Func<float, float> easeOut)
        {
            return t => t < 0.5 ? (1 - easeOut(1 - 2 * t)) / 2f : (1 + easeOut(2 * t - 1)) / 2f;
        }

        public static Func<float, float> GetEasing(string easing)
        {
            return Functions.GetValueOrDefault(easing, Linear);
        }

        public static Func<float, float> GetEasing(string easing, string defaultEasing)
        {
            if (easing.ToLower() == "default" || string.IsNullOrEmpty(easing))
            {
                return GetEasing(defaultEasing);
            }

            return GetEasing(easing);
        }

        /// <summary>
        /// Animation curve from 2 points
        /// </summary>
        /// <param name="p0">x1</param>
        /// <param name="p1">y1</param>
        /// <param name="p2">x2</param>
        /// <param name="p3">y2</param>
        /// <returns>Easing f(t)</returns>
        public static Func<float, float> CubicBezier(float p0, float p1, float p2, float p3)
        {
            // Source - https://stackoverflow.com/a/34457425
            p0 = Mathf.Clamp01(p0);
            p2 = Mathf.Clamp01(p2);

            var cx = 3 * p0;
            var bx = 3 * (p2 - p0) - cx;
            var ax = 1 - cx - bx;
            
            var cy = 3 * p1;
            var by = 3 * (p3 - p1) - cy;
            var ay = 1 - cy - by;

            static float Curve(float c, float b, float a, float t)
            {
                return ((a * t + b) * t + c) * t;
            }

            static float Derivative(float c, float b, float a, float t)
            {
                return (3 * a * t + 2 * b) * t + c;
            }

            float SolveX(float t)
            {
                float t2, x2;
                int i;
                const float epsilon = 0.02f;
                
                for (t2 = t, i = 0; i < 8; i++) {
                    x2 = Curve(cx, bx, ax, t2) - t;
                    if (Mathf.Abs(x2) < epsilon)
                        return t2;
                    
                    var d2 = Derivative(cx, bx, ax, t2);
                    if (Mathf.Abs(d2) < 1e-6)
                        break;
                    
                    t2 -= x2 / d2;
                }
                
                var t0 = 0f;
                var t1 = 1f;
                t2 = t;

                if (t2 < t0)
                    return t0;
                if (t2 > t1)
                    return t1;

                while (t0 < t1) {
                    x2 = Curve(cx, bx, ax, t2);
                    if (Mathf.Abs(x2 - t) < epsilon)
                        return t2;
                    
                    if (t > x2)
                    {
                        t0 = t2;
                    }
                    else
                    {
                        t1 = t2;
                    }

                    t2 = (t1 - t0) * 0.5f + t0;
                }

                return t2;
            }
            
            return (t) => Curve(cy, by, ay, SolveX(t));
        }
        
        public static Vector3 ExpDecay(Vector3 a, Vector3 b, float decay, float deltaTime)
        {
            return b + (a - b) * Mathf.Exp(-decay * deltaTime);
        }

        public static Vector2 ExpDecay(Vector2 a, Vector2 b, float decay, float deltaTime)
        {
            return b + (a - b) * Mathf.Exp(-decay * deltaTime);
        }
        
        public static float ExpDecay(float a, float b, float decay, float deltaTime)
        {
            return b + (a - b) * Mathf.Exp(-decay * deltaTime);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    // Easing functions for smooth animations
    public static class EasingFunctions
    {
        public static float Linear(float t) { return t; }
        public static float EaseInQuad(float t) { return t * t; }
        public static float EaseOutQuad(float t) { return 1 - (1 - t) * (1 - t); }
        public static float EaseInOutQuad(float t) { return t < 0.5f ? 2 * t * t : 1 - (float)Math.Pow(-2 * t + 2, 2) / 2; }
        public static float EaseInCubic(float t) { return t * t * t; }
        public static float EaseOutCubic(float t) { return 1 - (float)Math.Pow(1 - t, 3); }
        public static float EaseInOutCubic(float t) { return t < 0.5f ? 4 * t * t * t : 1 - (float)Math.Pow(-2 * t + 2, 3) / 2; }
        public static float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return 1 + c3 * (float)Math.Pow(t - 1, 3) + c1 * (float)Math.Pow(t - 1, 2);
        }
    }

    // Color interpolation helper
    public static class ColorInterpolator
    {
        public static Color Lerp(Color from, Color to, float t)
        {
            t = Math.Max(0, Math.Min(1, t));
            return Color.FromArgb(
                (int)Math.Round(from.A + (to.A - from.A) * t),
                (int)Math.Round(from.R + (to.R - from.R) * t),
                (int)Math.Round(from.G + (to.G - from.G) * t),
                (int)Math.Round(from.B + (to.B - from.B) * t));
        }
    }

    // Float interpolation
    public static class FloatInterpolator
    {
        public static float Lerp(float from, float to, float t)
        {
            return from + (to - from) * t;
        }

        public static int LerpInt(int from, int to, float t)
        {
            return (int)Math.Round(from + (to - from) * t);
        }

        public static Point LerpPoint(Point from, Point to, float t)
        {
            return new Point(LerpInt(from.X, to.X, t), LerpInt(from.Y, to.Y, t));
        }

        public static Rectangle LerpRect(Rectangle from, Rectangle to, float t)
        {
            return new Rectangle(
                LerpInt(from.X, to.X, t),
                LerpInt(from.Y, to.Y, t),
                LerpInt(from.Width, to.Width, t),
                LerpInt(from.Height, to.Height, t));
        }
    }

    // Generic animation object
    public class Animation
    {
        public float StartValue { get; set; }
        public float EndValue { get; set; }
        public float CurrentValue { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime StartTime { get; set; }
        public Func<float, float> Easing { get; set; }
        public Action<float> Update { get; set; }
        public Action Completed { get; set; }
        public bool IsCompleted { get; private set; }

        public Animation(float start, float end, int durationMs, Action<float> update, Action completed = null, Func<float, float> easing = null)
        {
            StartValue = start;
            EndValue = end;
            CurrentValue = start;
            Duration = TimeSpan.FromMilliseconds(durationMs);
            StartTime = DateTime.Now;
            Update = update;
            Completed = completed;
            Easing = easing ?? EasingFunctions.EaseInOutCubic;
        }

        public void Tick()
        {
            if (IsCompleted) return;

            double elapsed = (DateTime.Now - StartTime).TotalMilliseconds;
            float t = (float)Math.Min(1, elapsed / Duration.TotalMilliseconds);
            float easedT = Easing(t);
            CurrentValue = StartValue + (EndValue - StartValue) * easedT;
            Update?.Invoke(CurrentValue);

            if (t >= 1)
            {
                IsCompleted = true;
                Completed?.Invoke();
            }
        }
    }

    // Color animation object
    public class ColorAnimation
    {
        public Color StartColor { get; set; }
        public Color EndColor { get; set; }
        public Color CurrentColor { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime StartTime { get; set; }
        public Func<float, float> Easing { get; set; }
        public Action<Color> Update { get; set; }
        public Action Completed { get; set; }
        public bool IsCompleted { get; private set; }

        public ColorAnimation(Color start, Color end, int durationMs, Action<Color> update, Action completed = null, Func<float, float> easing = null)
        {
            StartColor = start;
            EndColor = end;
            CurrentColor = start;
            Duration = TimeSpan.FromMilliseconds(durationMs);
            StartTime = DateTime.Now;
            Update = update;
            Completed = completed;
            Easing = easing ?? EasingFunctions.EaseInOutCubic;
        }

        public void Tick()
        {
            if (IsCompleted) return;

            double elapsed = (DateTime.Now - StartTime).TotalMilliseconds;
            float t = (float)Math.Min(1, elapsed / Duration.TotalMilliseconds);
            float easedT = Easing(t);
            CurrentColor = ColorInterpolator.Lerp(StartColor, EndColor, easedT);
            Update?.Invoke(CurrentColor);

            if (t >= 1)
            {
                IsCompleted = true;
                Completed?.Invoke();
            }
        }
    }

    // Rectangle animation object
    public class RectangleAnimation
    {
        public Rectangle StartRect { get; set; }
        public Rectangle EndRect { get; set; }
        public Rectangle CurrentRect { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime StartTime { get; set; }
        public Func<float, float> Easing { get; set; }
        public Action<Rectangle> Update { get; set; }
        public Action Completed { get; set; }
        public bool IsCompleted { get; private set; }

        public RectangleAnimation(Rectangle start, Rectangle end, int durationMs, Action<Rectangle> update, Action completed = null, Func<float, float> easing = null)
        {
            StartRect = start;
            EndRect = end;
            CurrentRect = start;
            Duration = TimeSpan.FromMilliseconds(durationMs);
            StartTime = DateTime.Now;
            Update = update;
            Completed = completed;
            Easing = easing ?? EasingFunctions.EaseInOutCubic;
        }

        public void Tick()
        {
            if (IsCompleted) return;

            double elapsed = (DateTime.Now - StartTime).TotalMilliseconds;
            float t = (float)Math.Min(1, elapsed / Duration.TotalMilliseconds);
            float easedT = Easing(t);
            CurrentRect = FloatInterpolator.LerpRect(StartRect, EndRect, easedT);
            Update?.Invoke(CurrentRect);

            if (t >= 1)
            {
                IsCompleted = true;
                Completed?.Invoke();
            }
        }
    }

    // Central animation manager
    public static class AnimationManager
    {
        private static List<object> animations = new List<object>();
        private static Timer timer;
        private static object lockObj = new object();

        static AnimationManager()
        {
            timer = new Timer();
            timer.Interval = 16; // ~60 FPS
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            lock (lockObj)
            {
                for (int i = animations.Count - 1; i >= 0; i--)
                {
                    if (animations[i] is Animation anim)
                    {
                        anim.Tick();
                        if (anim.IsCompleted) animations.RemoveAt(i);
                    }
                    else if (animations[i] is ColorAnimation colorAnim)
                    {
                        colorAnim.Tick();
                        if (colorAnim.IsCompleted) animations.RemoveAt(i);
                    }
                    else if (animations[i] is RectangleAnimation rectAnim)
                    {
                        rectAnim.Tick();
                        if (rectAnim.IsCompleted) animations.RemoveAt(i);
                    }
                }
            }
        }

        public static void Start(float from, float to, int durationMs, Action<float> update, Action completed = null, Func<float, float> easing = null)
        {
            lock (lockObj)
            {
                animations.Add(new Animation(from, to, durationMs, update, completed, easing));
            }
        }

        public static void StartColor(Color from, Color to, int durationMs, Action<Color> update, Action completed = null, Func<float, float> easing = null)
        {
            lock (lockObj)
            {
                animations.Add(new ColorAnimation(from, to, durationMs, update, completed, easing));
            }
        }

        public static void StartRect(Rectangle from, Rectangle to, int durationMs, Action<Rectangle> update, Action completed = null, Func<float, float> easing = null)
        {
            lock (lockObj)
            {
                animations.Add(new RectangleAnimation(from, to, durationMs, update, completed, easing));
            }
        }
    }
}

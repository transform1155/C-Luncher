using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    // Modern color schemes for different version types
    public static class ColorSchemes
    {
        // Official Release - Blue theme
        public static class Release
        {
            public static Color Primary = Color.FromArgb(41, 128, 185);
            public static Color Secondary = Color.FromArgb(52, 152, 219);
            public static Color Accent = Color.FromArgb(46, 204, 113);
            public static Color Background = Color.FromArgb(236, 240, 241);
            public static Color Text = Color.FromArgb(44, 62, 80);
        }

        // Snapshot - Orange theme
        public static class Snapshot
        {
            public static Color Primary = Color.FromArgb(230, 126, 34);
            public static Color Secondary = Color.FromArgb(243, 156, 18);
            public static Color Accent = Color.FromArgb(255, 165, 0);
            public static Color Background = Color.FromArgb(253, 245, 230);
            public static Color Text = Color.FromArgb(44, 62, 80);
        }

        // April Fool's - Purple/Pink theme
        public static class AprilFool
        {
            public static Color Primary = Color.FromArgb(155, 89, 182);
            public static Color Secondary = Color.FromArgb(142, 68, 173);
            public static Color Accent = Color.FromArgb(232, 67, 147);
            public static Color Background = Color.FromArgb(245, 238, 248);
            public static Color Text = Color.FromArgb(44, 62, 80);
        }

        // Common colors
        public static Color Success = Color.FromArgb(46, 204, 113);
        public static Color Warning = Color.FromArgb(241, 196, 15);
        public static Color Error = Color.FromArgb(231, 76, 60);
        public static Color Info = Color.FromArgb(52, 152, 219);
        public static Color DarkBackground = Color.FromArgb(30, 30, 30);
        public static Color LightBackground = Color.FromArgb(245, 245, 245);
    }

    // Modern rounded button with smooth color animation and hover scale
    public class ModernButton : Button
    {
        private int borderRadius = 8;
        private Color borderColor = Color.Transparent;
        private int borderWidth = 0;
        private Color currentFillColor;
        private Color baseColor;
        private float scaleX = 1.0f;
        private float scaleY = 1.0f;
        private bool isHovered = false;
        private bool isPressed = false;

        [Category("Appearance")]
        public int BorderRadius
        {
            get { return borderRadius; }
            set { borderRadius = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public int BorderWidth
        {
            get { return borderWidth; }
            set { borderWidth = value; Invalidate(); }
        }

        public new Color BackColor
        {
            get { return baseColor; }
            set
            {
                baseColor = value;
                base.BackColor = value;
                if (currentFillColor.IsEmpty) currentFillColor = value;
            }
        }

        public ModernButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            base.BackColor = ColorSchemes.Release.Primary;
            baseColor = ColorSchemes.Release.Primary;
            currentFillColor = baseColor;
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            Cursor = Cursors.Hand;
            DoubleBuffered = true;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            AnimateToHover();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            AnimateToNormal();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            isPressed = true;
            AnimateToPressed();
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            isPressed = false;
            if (isHovered) AnimateToHover();
            else AnimateToNormal();
        }

        private void AnimateToHover()
        {
            Color targetColor = ControlPaint.Light(baseColor, 0.15f);
            AnimationManager.StartColor(
                currentFillColor,
                targetColor,
                180,
                (c) => { currentFillColor = c; Invalidate(); },
                null,
                EasingFunctions.EaseOutCubic);

            AnimationManager.Start(
                1.0f, 1.02f, 180,
                (v) => { scaleX = v; scaleY = v; Invalidate(); },
                null,
                EasingFunctions.EaseOutBack);
        }

        private void AnimateToNormal()
        {
            AnimationManager.StartColor(
                currentFillColor,
                baseColor,
                200,
                (c) => { currentFillColor = c; Invalidate(); },
                null,
                EasingFunctions.EaseOutCubic);

            AnimationManager.Start(
                scaleX, 1.0f, 200,
                (v) => { scaleX = v; scaleY = v; Invalidate(); },
                null,
                EasingFunctions.EaseOutCubic);
        }

        private void AnimateToPressed()
        {
            AnimationManager.StartColor(
                currentFillColor,
                ControlPaint.Dark(baseColor, 0.1f),
                80,
                (c) => { currentFillColor = c; Invalidate(); },
                null,
                EasingFunctions.EaseOutQuad);

            AnimationManager.Start(
                scaleX, 0.97f, 80,
                (v) => { scaleX = v; scaleY = v; Invalidate(); },
                null,
                EasingFunctions.EaseOutQuad);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Calculate scaled rectangle
            Rectangle drawRect = ClientRectangle;
            int cx = drawRect.Width / 2;
            int cy = drawRect.Height / 2;
            int w = (int)(drawRect.Width * scaleX);
            int h = (int)(drawRect.Height * scaleY);
            int x = cx - w / 2;
            int y = cy - h / 2;
            Rectangle scaledRect = new Rectangle(x + 1, y + 1, w - 2, h - 2);

            GraphicsPath path = GetRoundedRectPath(scaledRect, borderRadius);

            // Draw shadow (subtle)
            if (isHovered && !isPressed)
            {
                using (Pen shadowPen = new Pen(Color.FromArgb(20, 0, 0, 0), 2))
                {
                    Rectangle shadowRect = new Rectangle(scaledRect.X + 1, scaledRect.Y + 2, scaledRect.Width, scaledRect.Height);
                    GraphicsPath shadowPath = GetRoundedRectPath(shadowRect, borderRadius);
                    e.Graphics.DrawPath(shadowPen, shadowPath);
                }
            }

            // Draw gradient fill (top -> bottom lighten)
            Rectangle fillRect = scaledRect;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                fillRect,
                currentFillColor,
                ControlPaint.Light(currentFillColor, 0.08f),
                LinearGradientMode.Vertical))
            {
                Blend blend = new Blend();
                blend.Factors = new float[] { 0.0f, 0.3f, 0.5f, 0.7f, 1.0f };
                blend.Positions = new float[] { 0.0f, 0.25f, 0.5f, 0.75f, 1.0f };
                brush.Blend = blend;
                e.Graphics.FillPath(brush, path);
            }

            // Bottom highlight line
            using (Pen accentPen = new Pen(Color.FromArgb(30, 255, 255, 255)))
            {
                e.Graphics.DrawLine(accentPen,
                    fillRect.X + 4, fillRect.Y + fillRect.Height / 2,
                    fillRect.Right - 4, fillRect.Y + fillRect.Height / 2);
            }

            if (borderWidth > 0)
            {
                using (Pen pen = new Pen(borderColor, borderWidth))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }

            // Draw text with subtle shadow
            Rectangle textRect = new Rectangle(0, 0, Width, Height);
            TextRenderer.DrawText(e.Graphics, Text, Font,
                new Rectangle(textRect.X + 1, textRect.Y + 1, textRect.Width, textRect.Height),
                Color.FromArgb(30, 0, 0, 0),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);

            TextRenderer.DrawText(e.Graphics, Text, Font, textRect,
                ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int r = radius * 2;
            if (r > rect.Width) r = rect.Width;
            if (r > rect.Height) r = rect.Height;
            if (r <= 0) r = 2;
            path.AddArc(rect.X, rect.Y, r, r, 180, 90);
            path.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
            path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
            path.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    // Modern progress bar with animated gradient fill and shimmer effect
    public class ModernProgressBar : Control
    {
        private int targetValue = 0;
        private float displayValue = 0;
        private int maximum = 100;
        private Color progressColor = ColorSchemes.Release.Primary;
        private Color currentProgressColor;
        private Color backgroundColor = Color.FromArgb(230, 230, 230);
        private int borderRadius = 4;
        private bool showPercentage = true;
        private string customText = "";
        private float downloadSpeed = 0;
        private TimeSpan estimatedTime = TimeSpan.Zero;
        private float shimmerOffset = 0f;
        private Timer shimmerTimer;

        [Category("Behavior")]
        public int Value
        {
            get { return targetValue; }
            set
            {
                if (value < 0) value = 0;
                if (value > maximum) value = maximum;
                int oldVal = targetValue;
                targetValue = value;
                AnimationManager.Start(
                    (float)oldVal,
                    (float)value,
                    400,
                    (v) => { displayValue = v; Invalidate(); },
                    null,
                    EasingFunctions.EaseOutCubic);
            }
        }

        [Category("Behavior")]
        public int Maximum
        {
            get { return maximum; }
            set { maximum = value > 0 ? value : 1; Invalidate(); }
        }

        [Category("Appearance")]
        public Color ProgressColor
        {
            get { return progressColor; }
            set
            {
                Color oldColor = currentProgressColor.IsEmpty ? progressColor : currentProgressColor;
                progressColor = value;
                AnimationManager.StartColor(
                    oldColor, value, 300,
                    (c) => { currentProgressColor = c; Invalidate(); },
                    null,
                    EasingFunctions.EaseOutCubic);
            }
        }

        [Category("Appearance")]
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public int BorderRadius
        {
            get { return borderRadius; }
            set { borderRadius = value; Invalidate(); }
        }

        [Category("Appearance")]
        public bool ShowPercentage
        {
            get { return showPercentage; }
            set { showPercentage = value; Invalidate(); }
        }

        [Category("Data")]
        public string CustomText
        {
            get { return customText; }
            set { customText = value; Invalidate(); }
        }

        [Category("Data")]
        public float DownloadSpeed
        {
            get { return downloadSpeed; }
            set { downloadSpeed = value; Invalidate(); }
        }

        [Category("Data")]
        public TimeSpan EstimatedTime
        {
            get { return estimatedTime; }
            set { estimatedTime = value; Invalidate(); }
        }

        public ModernProgressBar()
        {
            Size = new Size(300, 30);
            Font = new Font("Segoe UI", 8F, FontStyle.Regular);
            DoubleBuffered = true;
            currentProgressColor = progressColor;

            // Shimmer animation
            shimmerTimer = new Timer();
            shimmerTimer.Interval = 30;
            shimmerTimer.Tick += (s, e) =>
            {
                shimmerOffset += 0.02f;
                if (shimmerOffset > 2f) shimmerOffset = 0f;
                if (displayValue > 0 && downloadSpeed > 0) Invalidate();
            };
            shimmerTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Background
            GraphicsPath bgPath = GetRoundedRectPath(ClientRectangle, borderRadius);
            using (SolidBrush bgBrush = new SolidBrush(backgroundColor))
            {
                e.Graphics.FillPath(bgBrush, bgPath);
            }

            // Progress
            if (displayValue > 0.5f)
            {
                int progressWidth = (int)((float)displayValue / maximum * Width);
                Rectangle progressRect = new Rectangle(0, 0, progressWidth, Height);
                GraphicsPath progressPath = GetRoundedRectPath(progressRect, borderRadius);

                // Animate moving gradient
                float animOffset = shimmerOffset;
                float gradStart = -0.3f + (animOffset % 1f);
                using (LinearGradientBrush progressBrush = new LinearGradientBrush(
                    progressRect,
                    currentProgressColor,
                    ControlPaint.Light(currentProgressColor, 0.25f),
                    0f))
                {
                    ColorBlend blend = new ColorBlend();
                    blend.Colors = new Color[]
                    {
                        currentProgressColor,
                        ControlPaint.Light(currentProgressColor, 0.05f),
                        ControlPaint.Light(currentProgressColor, 0.25f),
                        ControlPaint.Light(currentProgressColor, 0.05f),
                        currentProgressColor
                    };
                    blend.Positions = new float[] { 0.0f, 0.3f, 0.5f, 0.7f, 1.0f };
                    progressBrush.InterpolationColors = blend;
                    e.Graphics.FillPath(progressBrush, progressPath);
                }

                // Shimmer highlight - moving bright diagonal line
                if (downloadSpeed > 0 && displayValue < maximum)
                {
                    GraphicsState state = e.Graphics.Save();
                    e.Graphics.SetClip(progressPath);
                    using (LinearGradientBrush shimmerBrush = new LinearGradientBrush(
                        new Rectangle(-progressRect.Width, 0, progressRect.Width * 3, progressRect.Height),
                        Color.Transparent,
                        Color.FromArgb(120, 255, 255, 255),
                        LinearGradientMode.Horizontal))
                    {
                        ColorBlend shimmerBlend = new ColorBlend();
                        shimmerBlend.Colors = new Color[]
                        {
                            Color.Transparent,
                            Color.Transparent,
                            Color.FromArgb(80, 255, 255, 255),
                            Color.Transparent,
                            Color.Transparent
                        };
                        shimmerBlend.Positions = new float[] { 0.0f, 0.35f, 0.5f, 0.65f, 1.0f };
                        shimmerBrush.InterpolationColors = shimmerBlend;
                        shimmerBrush.TranslateTransform(shimmerOffset * progressRect.Width - progressRect.Width, 0);
                        e.Graphics.FillRectangle(shimmerBrush,
                            new Rectangle(0, 0, progressRect.Width, progressRect.Height));
                    }
                    e.Graphics.Restore(state);
                }
            }

            // Text
            string displayText = "";
            if (!string.IsNullOrEmpty(customText))
            {
                displayText = customText;
            }
            else if (showPercentage)
            {
                displayText = string.Format("{0}%", (int)((float)displayValue / maximum * 100));
            }

            if (!string.IsNullOrEmpty(displayText))
            {
                // Shadow
                TextRenderer.DrawText(e.Graphics, displayText, Font,
                    new Rectangle(1, 1, Width, Height),
                    Color.FromArgb(30, 0, 0, 0),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                // Main text
                TextRenderer.DrawText(e.Graphics, displayText, Font, ClientRectangle,
                    Color.FromArgb(50, 50, 50),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }

            // Speed and time info
            if (downloadSpeed > 0)
            {
                string speedText = FormatSpeed(downloadSpeed);
                string timeText = estimatedTime.TotalSeconds > 0 ?
                    string.Format("ETA: {0:mm\\:ss}", estimatedTime) : "";

                string infoText = string.Format("{0}  {1}", speedText, timeText).Trim();
                if (!string.IsNullOrEmpty(infoText))
                {
                    Rectangle infoRect = new Rectangle(5, Height - 12, Width - 10, 12);
                    using (Font smallFont = new Font("Segoe UI", 7F))
                    {
                        TextRenderer.DrawText(e.Graphics, infoText, smallFont, infoRect,
                            Color.FromArgb(100, 100, 100),
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.Top);
                    }
                }
            }
        }

        private string FormatSpeed(float bytesPerSecond)
        {
            if (bytesPerSecond < 1024)
                return string.Format("{0:F0} B/s", bytesPerSecond);
            else if (bytesPerSecond < 1024 * 1024)
                return string.Format("{0:F1} KB/s", bytesPerSecond / 1024);
            else
                return string.Format("{0:F2} MB/s", bytesPerSecond / (1024 * 1024));
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int r = radius * 2;
            if (r > rect.Width) r = rect.Width;
            if (r > rect.Height) r = rect.Height;
            if (r <= 0) r = 2;
            path.AddArc(rect.X, rect.Y, r, r, 180, 90);
            path.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
            path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
            path.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    // Version card control with selection and hover animations
    public class VersionCard : Panel
    {
        private string versionId = "";
        private string versionType = "release";
        private string versionDate = "";
        private bool isSelected = false;
        private Color cardColor;
        private Color currentBgColor;
        private float accentWidth = 4f;
        private bool isHovered = false;
        private int hoverOffset = 0;
        private Label lblTitle;
        private Label lblType;
        private Label lblDate;

        public event EventHandler CardSelected;

        [Category("Data")]
        public string VersionId
        {
            get { return versionId; }
            set { versionId = value; UpdateCard(); }
        }

        [Category("Data")]
        public string VersionType
        {
            get { return versionType; }
            set { versionType = value; UpdateColors(); }
        }

        [Category("Data")]
        public string VersionDate
        {
            get { return versionDate; }
            set { versionDate = value; UpdateCard(); }
        }

        [Category("Appearance")]
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                AnimateSelection();
            }
        }

        public VersionCard()
        {
            Size = new Size(200, 80);
            Cursor = Cursors.Hand;
            DoubleBuffered = true;
            currentBgColor = Color.White;
            BackColor = Color.Transparent;

            lblTitle = new Label()
            {
                AutoSize = true,
                Location = new Point(18, 12),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                BackColor = Color.Transparent
            };

            lblType = new Label()
            {
                AutoSize = true,
                Location = new Point(18, 34),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100),
                BackColor = Color.Transparent
            };

            lblDate = new Label()
            {
                AutoSize = true,
                Location = new Point(18, 52),
                Font = new Font("Segoe UI", 7F),
                ForeColor = Color.FromArgb(150, 150, 150),
                BackColor = Color.Transparent
            };

            Controls.AddRange(new Control[] { lblTitle, lblType, lblDate });
            UpdateColors();
        }

        private void UpdateColors()
        {
            switch (versionType.ToLower())
            {
                case "snapshot":
                    cardColor = ColorSchemes.Snapshot.Primary;
                    break;
                case "april_fool":
                case "april fool":
                    cardColor = ColorSchemes.AprilFool.Primary;
                    break;
                default:
                    cardColor = ColorSchemes.Release.Primary;
                    break;
            }

            // Update label type color
            if (lblType != null)
            {
                lblType.ForeColor = cardColor;
            }
            Invalidate();
        }

        private void UpdateCard()
        {
            lblTitle.Text = versionId;
            lblType.Text = " " + versionType.ToUpper() + " ";
            lblDate.Text = versionDate;
        }

        private void AnimateSelection()
        {
            Color targetBg = isSelected ?
                Color.FromArgb(245, 248, 252) :
                Color.White;

            // Add slight color tint for selected state
            if (isSelected)
            {
                targetBg = Color.FromArgb(
                    (255 + cardColor.R) / 2,
                    (255 + cardColor.G) / 2,
                    (255 + cardColor.B) / 2);
            }

            AnimationManager.StartColor(
                currentBgColor, targetBg, 350,
                (c) => { currentBgColor = c; Invalidate(); },
                null,
                EasingFunctions.EaseOutCubic);

            // Accent bar animate wider when selected
            float targetWidth = isSelected ? 6f : 4f;
            AnimationManager.Start(
                accentWidth, targetWidth, 300,
                (v) => { accentWidth = v; Invalidate(); },
                null,
                EasingFunctions.EaseOutBack);

            // Text slide animation
            int targetOffset = isHovered || isSelected ? 6 : 0;
            AnimationManager.Start(
                hoverOffset, targetOffset, 250,
                (v) =>
                {
                    hoverOffset = (int)v;
                    lblTitle.Location = new Point(18 + hoverOffset, 12);
                    lblType.Location = new Point(18 + hoverOffset, 34);
                    lblDate.Location = new Point(18 + hoverOffset, 52);
                    Invalidate();
                },
                null,
                EasingFunctions.EaseOutCubic);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;

            Color lightTint = Color.FromArgb(
                (cardColor.R + 255) / 2,
                (cardColor.G + 255) / 2,
                (cardColor.B + 255) / 2);

            Color targetColor = isSelected ?
                Color.FromArgb(
                    (cardColor.R + 245) / 2,
                    (cardColor.G + 248) / 2,
                    (cardColor.B + 252) / 2) :
                lightTint;

            AnimationManager.StartColor(
                currentBgColor, targetColor, 200,
                (c) => { currentBgColor = c; Invalidate(); },
                null,
                EasingFunctions.EaseOutCubic);

            int targetOffset = 6;
            AnimationManager.Start(
                hoverOffset, targetOffset, 200,
                (v) =>
                {
                    hoverOffset = (int)v;
                    lblTitle.Location = new Point(18 + hoverOffset, 12);
                    lblType.Location = new Point(18 + hoverOffset, 34);
                    lblDate.Location = new Point(18 + hoverOffset, 52);
                    Invalidate();
                },
                null,
                EasingFunctions.EaseOutCubic);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;

            Color targetColor = isSelected ?
                Color.FromArgb(
                    (cardColor.R + 255) / 2,
                    (cardColor.G + 255) / 2,
                    (cardColor.B + 255) / 2) :
                Color.White;

            AnimationManager.StartColor(
                currentBgColor, targetColor, 250,
                (c) => { currentBgColor = c; Invalidate(); },
                null,
                EasingFunctions.EaseOutCubic);

            AnimationManager.Start(
                hoverOffset, 0, 250,
                (v) =>
                {
                    hoverOffset = (int)v;
                    lblTitle.Location = new Point(18 + hoverOffset, 12);
                    lblType.Location = new Point(18 + hoverOffset, 34);
                    lblDate.Location = new Point(18 + hoverOffset, 52);
                    Invalidate();
                },
                null,
                EasingFunctions.EaseOutCubic);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Card background
            Rectangle cardRect = new Rectangle(0, 0, Width - 1, Height - 1);
            GraphicsPath path = GetRoundedRectPath(cardRect, 8);

            // Draw shadow when hovered
            if (isHovered)
            {
                using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(15, 0, 0, 0)))
                {
                    Rectangle shadowRect = new Rectangle(2, 3, Width - 3, Height - 3);
                    GraphicsPath shadowPath = GetRoundedRectPath(shadowRect, 8);
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }
            }

            // Main card fill
            using (SolidBrush bgBrush = new SolidBrush(currentBgColor))
            {
                e.Graphics.FillPath(bgBrush, path);
            }

            // Left accent bar (gradient)
            Rectangle accentRect = new Rectangle(0, 0, (int)accentWidth, Height);
            using (LinearGradientBrush accentBrush = new LinearGradientBrush(
                accentRect,
                cardColor,
                ControlPaint.Light(cardColor, 0.2f),
                LinearGradientMode.Horizontal))
            {
                e.Graphics.FillRectangle(accentBrush, accentRect);
            }

            // Border - animated appearance when selected
            if (isSelected)
            {
                using (Pen borderPen = new Pen(cardColor, 2))
                {
                    e.Graphics.DrawPath(borderPen, path);
                }
            }
            else
            {
                using (Pen borderPen = new Pen(Color.FromArgb(220, 220, 220), 1))
                {
                    e.Graphics.DrawPath(borderPen, path);
                }
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            CardSelected?.Invoke(this, EventArgs.Empty);
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int r = radius * 2;
            if (r > rect.Width) r = rect.Width;
            if (r > rect.Height) r = rect.Height;
            if (r <= 0) r = 2;
            path.AddArc(rect.X, rect.Y, r, r, 180, 90);
            path.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
            path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
            path.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    // Download state enum
    public enum DownloadState
    {
        Pending,
        Initializing,
        Downloading,
        Paused,
        Completed,
        Failed,
        Cancelled
    }

    // Download item with animated entry and smooth updates
    public class DownloadItem : Panel
    {
        private string fileName = "";
        private long totalBytes = 0;
        private long downloadedBytes = 0;
        private DownloadState state = DownloadState.Pending;
        private float speed = 0;
        private float entryOpacity = 0f;
        private int entryOffset = 20;

        private Label lblFileName;
        private Label lblStatus;
        private ModernProgressBar progressBar;
        private ModernButton btnPause;
        private ModernButton btnResume;
        private ModernButton btnCancel;
        private ModernButton btnRetry;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem mnuPauseResume;
        private ToolStripMenuItem mnuCancel;
        private ToolStripMenuItem mnuRetry;
        private ToolStripMenuItem mnuOpenLocation;
        private ToolStripMenuItem mnuCopyUrl;

        public event EventHandler PauseRequested;
        public event EventHandler ResumeRequested;
        public event EventHandler CancelRequested;
        public event EventHandler RetryRequested;
        public event EventHandler OpenLocationRequested;
        public event EventHandler CopyUrlRequested;

        private string url = "";

        [Category("Data")]
        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        [Category("Data")]
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; lblFileName.Text = value; }
        }

        [Category("Data")]
        public long TotalBytes
        {
            get { return totalBytes; }
            set { totalBytes = value; UpdateProgress(); }
        }

        [Category("Data")]
        public long DownloadedBytes
        {
            get { return downloadedBytes; }
            set { downloadedBytes = value; UpdateProgress(); }
        }

        [Category("Data")]
        public DownloadState State
        {
            get { return state; }
            set
            {
                state = value;
                UpdateStateUI();
            }
        }

        [Category("Data")]
        public float Speed
        {
            get { return speed; }
            set { speed = value; progressBar.DownloadSpeed = value; }
        }

        public DownloadItem()
        {
            Size = new Size(400, 80);
            BackColor = Color.Transparent;
            DoubleBuffered = true;

            // Create context menu
            contextMenu = new ContextMenuStrip();
            contextMenu.Items.Clear();

            mnuPauseResume = new ToolStripMenuItem("暂停", null, (s, e) => {
                if (State == DownloadState.Downloading) PauseRequested?.Invoke(this, EventArgs.Empty);
                else if (State == DownloadState.Paused) ResumeRequested?.Invoke(this, EventArgs.Empty);
            });
            mnuCancel = new ToolStripMenuItem("取消", null, (s, e) => CancelRequested?.Invoke(this, EventArgs.Empty));
            mnuRetry = new ToolStripMenuItem("重试", null, (s, e) => RetryRequested?.Invoke(this, EventArgs.Empty));
            mnuOpenLocation = new ToolStripMenuItem("打开文件位置", null, (s, e) => OpenLocationRequested?.Invoke(this, EventArgs.Empty));
            mnuCopyUrl = new ToolStripMenuItem("复制下载链接", null, (s, e) => CopyUrlRequested?.Invoke(this, EventArgs.Empty));

            contextMenu.Items.AddRange(new ToolStripItem[] {
                mnuPauseResume,
                mnuCancel,
                mnuRetry,
                new ToolStripSeparator(),
                mnuOpenLocation,
                mnuCopyUrl
            });

            ContextMenuStrip = contextMenu;

            lblFileName = new Label()
            {
                Location = new Point(15, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                BackColor = Color.Transparent
            };

            lblStatus = new Label()
            {
                Location = new Point(15, 55),
                AutoSize = true,
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(100, 100, 100),
                BackColor = Color.Transparent
            };

            progressBar = new ModernProgressBar()
            {
                Location = new Point(15, 28),
                Size = new Size(Width - 150, 20),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
            };

            btnPause = CreateMiniButton("⏸");
            btnResume = CreateMiniButton("▶");
            btnCancel = CreateMiniButton("✕");
            btnRetry = CreateMiniButton("↻");

            btnPause.BackColor = ColorSchemes.Warning;
            btnResume.BackColor = ColorSchemes.Success;
            btnCancel.BackColor = ColorSchemes.Error;
            btnRetry.BackColor = ColorSchemes.Info;

            btnPause.Click += (s, e) => PauseRequested?.Invoke(this, EventArgs.Empty);
            btnResume.Click += (s, e) => ResumeRequested?.Invoke(this, EventArgs.Empty);
            btnCancel.Click += (s, e) => CancelRequested?.Invoke(this, EventArgs.Empty);
            btnRetry.Click += (s, e) => RetryRequested?.Invoke(this, EventArgs.Empty);

            Controls.AddRange(new Control[] { lblFileName, lblStatus, progressBar, btnPause, btnCancel });

            // Entry animation
            AnimationManager.Start(
                0f, 1f, 500,
                (v) =>
                {
                    entryOpacity = v;
                    Invalidate();
                },
                null,
                EasingFunctions.EaseOutCubic);

            AnimationManager.Start(
                20f, 0f, 500,
                (v) =>
                {
                    entryOffset = (int)v;
                },
                null,
                EasingFunctions.EaseOutCubic);

            UpdateStateUI();
        }

        private ModernButton CreateMiniButton(string text)
        {
            return new ModernButton()
            {
                Text = text,
                Size = new Size(28, 28),
                Location = new Point(Width - 130, 24),
                Font = new Font("Segoe UI", 10F),
                BorderRadius = 14
            };
        }

        private void UpdateProgress()
        {
            if (totalBytes > 0)
            {
                progressBar.Maximum = 100;
                progressBar.Value = (int)((float)downloadedBytes / totalBytes * 100);
                lblStatus.Text = string.Format("{0} / {1}",
                    FormatBytes(downloadedBytes), FormatBytes(totalBytes));
            }
        }

        private void UpdateStateUI()
        {
            // Update context menu based on state
            mnuPauseResume.Text = (state == DownloadState.Downloading) ? "暂停" : "继续";
            mnuPauseResume.Enabled = (state == DownloadState.Downloading || state == DownloadState.Paused);
            mnuCancel.Enabled = (state == DownloadState.Downloading || state == DownloadState.Paused || state == DownloadState.Pending);
            mnuRetry.Enabled = (state == DownloadState.Failed || state == DownloadState.Cancelled);
            mnuOpenLocation.Enabled = (state == DownloadState.Completed);

            // Remove existing control buttons
            Controls.Remove(btnPause);
            Controls.Remove(btnResume);
            Controls.Remove(btnRetry);

            // Reposition buttons
            int rightX = Width - 130;
            btnPause.Location = new Point(rightX, 24);
            btnResume.Location = new Point(rightX, 24);
            btnRetry.Location = new Point(rightX, 24);
            btnCancel.Location = new Point(rightX + 34, 24);

            switch (state)
            {
                case DownloadState.Downloading:
                    Controls.Add(btnPause);
                    Controls.Add(btnCancel);
                    AnimationManager.StartColor(
                        progressBar.ProgressColor, ColorSchemes.Info, 200,
                        (c) => progressBar.ProgressColor = c,
                        null, EasingFunctions.EaseOutCubic);
                    break;
                case DownloadState.Paused:
                    Controls.Add(btnResume);
                    Controls.Add(btnCancel);
                    AnimationManager.StartColor(
                        progressBar.ProgressColor, ColorSchemes.Warning, 200,
                        (c) => progressBar.ProgressColor = c,
                        null, EasingFunctions.EaseOutCubic);
                    break;
                case DownloadState.Failed:
                    Controls.Add(btnRetry);
                    AnimationManager.StartColor(
                        progressBar.ProgressColor, ColorSchemes.Error, 200,
                        (c) => progressBar.ProgressColor = c,
                        null, EasingFunctions.EaseOutCubic);
                    break;
                case DownloadState.Completed:
                    AnimationManager.StartColor(
                        progressBar.ProgressColor, ColorSchemes.Success, 500,
                        (c) => progressBar.ProgressColor = c,
                        null, EasingFunctions.EaseOutCubic);
                    break;
                case DownloadState.Cancelled:
                    AnimationManager.StartColor(
                        progressBar.ProgressColor, Color.FromArgb(180, 180, 180), 200,
                        (c) => progressBar.ProgressColor = c,
                        null, EasingFunctions.EaseOutCubic);
                    break;
                default:
                    AnimationManager.StartColor(
                        progressBar.ProgressColor, Color.FromArgb(200, 200, 200), 200,
                        (c) => progressBar.ProgressColor = c,
                        null, EasingFunctions.EaseOutCubic);
                    break;
            }
        }

        private string FormatBytes(long bytes)
        {
            if (bytes < 1024) return bytes + " B";
            if (bytes < 1024 * 1024) return (bytes / 1024.0).ToString("F1") + " KB";
            return (bytes / (1024.0 * 1024.0)).ToString("F2") + " MB";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Card background with opacity for entry animation
            Color bgColor = Color.FromArgb((int)(entryOpacity * 255), 255, 255, 255);

            GraphicsPath path = GetRoundedRectPath(new Rectangle(2, 2, Width - 5, Height - 5), 8);
            using (SolidBrush brush = new SolidBrush(bgColor))
            {
                e.Graphics.FillPath(brush, path);
            }

            // Subtle border
            Color borderColor = Color.FromArgb((int)(entryOpacity * 220), 220, 220, 220);
            using (Pen pen = new Pen(borderColor, 1))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int r = radius * 2;
            if (r > rect.Width) r = rect.Width;
            if (r > rect.Height) r = rect.Height;
            if (r <= 0) r = 2;
            path.AddArc(rect.X, rect.Y, r, r, 180, 90);
            path.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
            path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
            path.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}

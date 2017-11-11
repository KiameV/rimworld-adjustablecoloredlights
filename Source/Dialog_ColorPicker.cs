using UnityEngine;
using Verse;

namespace AdjustableColoredLights
{
    class Dialog_FloatColorPicker : Window
    {
        private const float WIDTH = 140f;
        private const float HEIGHT = 190f;

        private static readonly Vector2 InitialPositionShift = new Vector2(4f, 0f);

        private readonly CompGlower CompGlower;

        protected override float Margin { get { return 0f; } }
        
        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(WIDTH, HEIGHT);
            }
        }

        public Dialog_FloatColorPicker(CompGlower compGlower)
        {
            this.CompGlower = compGlower;
            this.layer = WindowLayer.Super;
            this.closeOnClickedOutside = true;
            this.doWindowBackground = false;
            this.drawShadow = false;
        }

        protected override void SetInitialSizeAndPosition()
        {
            Vector2 vector = UI.MousePositionOnUIInverted + Dialog_FloatColorPicker.InitialPositionShift;
            if (vector.x + this.InitialSize.x > (float)UI.screenWidth)
            {
                vector.x = (float)UI.screenWidth - this.InitialSize.x;
            }
            if (vector.y + this.InitialSize.y - 50 > (float)UI.screenHeight)
            {
                vector.y = (float)UI.screenHeight - this.InitialSize.y - 50;
            }
            this.windowRect = new Rect(vector.x, vector.y, this.InitialSize.x, this.InitialSize.y);
        }

        public override void DoWindowContents(Rect rect)
        {
            Rect r = new Rect(0f, 0f, WIDTH, HEIGHT).ContractedBy(-5f);
            if (!r.Contains(Event.current.mousePosition))
            {
                float num = GenUI.DistFromRect(r, Event.current.mousePosition);
                if (num > 95f)
                {
                    this.Close(false);
                    return;
                }
            }

            GUI.DrawTexture(rect, Main.BlackTexture);

            Color rgb = this.CompGlower.Props.glowColor.ToColor;
            HSL hsl = new HSL();
            Color.RGBToHSV(rgb, out hsl.h, out hsl.s, out hsl.l);
            HSL originalHsl = new HSL(hsl);
            Color originalRGB = rgb;
            
            rgb.r = Widgets.HorizontalSlider(new Rect(5, 15, 125f, 20f), rgb.r, 0, 1, false, null, "AdjustableColoredLights.R".Translate(), ((int)(rgb.r * 255)).ToString());
            rgb.g = Widgets.HorizontalSlider(new Rect(5, 45, 125f, 20f), rgb.g, 0, 1, false, null, "AdjustableColoredLights.G".Translate(), ((int)(rgb.g * 255)).ToString());
            rgb.b = Widgets.HorizontalSlider(new Rect(5, 75, 125f, 20f), rgb.b, 0, 1, false, null, "AdjustableColoredLights.B".Translate(), ((int)(rgb.b * 255)).ToString());
            hsl.h = Widgets.HorizontalSlider(new Rect(5, 105, 125f, 20f), hsl.h, 0, 1, false, null, "AdjustableColoredLights.H".Translate(), ((int)(hsl.h * 255)).ToString());
            hsl.s = Widgets.HorizontalSlider(new Rect(5, 135, 125f, 20f), hsl.s, 0, 1, false, null, "AdjustableColoredLights.S".Translate(), ((int)(hsl.s * 255)).ToString());
            hsl.l = Widgets.HorizontalSlider(new Rect(5, 165, 125f, 20f), hsl.l, 0, 1, false, null, "AdjustableColoredLights.L".Translate(), ((int)(hsl.l * 255)).ToString());

            if (hsl != originalHsl)
            {
                rgb = Color.HSVToRGB(hsl.h, hsl.s, hsl.l);
            }

            if (rgb.r < 0.02f && rgb.g < 0.02f && rgb.b < 0.02f)
            {
                rgb = new Color(0.02f, 0.02f, 0.02f);
            }

            if (rgb != originalRGB)
            {
                this.CompGlower.Props.glowColor = new ColorInt(
                    (int)(rgb.r * 255), (int)(rgb.g * 255), (int)(rgb.b * 255), (int)(rgb.a * 255));

                this.CompGlower.parent?.Map?.glowGrid.MarkGlowGridDirty(this.CompGlower.parent.Position);
            }
        }
    }
}
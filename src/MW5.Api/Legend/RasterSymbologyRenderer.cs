﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MW5.Api.Concrete;
using MW5.Api.Enums;
using MW5.Api.Interfaces;

namespace MW5.Api.Legend
{
    /// <summary>
    /// Renders symbology preview in the expansion section below the layer.
    /// </summary>
    internal class RasterSymbologyRenderer : SymbologyRendererBase
    {
        public RasterSymbologyRenderer(LegendControl legend)
            : base(legend)
        {

        }

        /// <summary>
        /// Draws the raster symbology.
        /// </summary>
        public void Render(Graphics g, LegendLayer layer, Rectangle bounds, bool isSnapshot)
        {
            var top = GetSymbologyTop(bounds);

            var caption = layer.SymbologyCaption;

            DrawCategoriesCaption(g, bounds, ref top, caption, Font);

            DrawCategories(g, layer, bounds, ref top);
        }

        private void DrawCategories(Graphics g, LegendLayer layer, Rectangle bounds, ref int top)
        {
            var raster = layer.ImageSource as IRasterSource;
            if (raster == null)
            {
                return;
            }

            var left = bounds.Left + Constants.TextLeftPad;

            var r = new Rectangle(left, top + 1, Constants.IconWidth, Constants.CsItemHeight - 2);

            switch (raster.RenderingType)
            {
                case RasterRendering.SingleBand:
                    RenderColorScheme(g, layer, bounds, ref r, raster.GrayScaleColorScheme, true, true);
                    break;
                case RasterRendering.Rgb:
                    RenderColorScheme(g, layer, bounds, ref r, raster.RgbBandMapping, false);
                    break;
                case RasterRendering.ColorScheme:
                    RenderColorScheme(g, layer, bounds, ref r, raster.CustomColorScheme, true);
                    break;
                case RasterRendering.BuiltInColorTable:
                    RenderColorScheme(g, layer, bounds, ref r, raster.Bands[1].ColorTable, true, true);
                    break;
                case RasterRendering.Unknown:
                    RenderColorScheme(g, layer, bounds, ref r, null, true, true);
                    break;
            }

            top = r.Y;
        }

        /// <summary>
        /// Renders color scheme (rectangle and name for each break).
        /// </summary>
        private void RenderColorScheme(Graphics g, LegendLayer layer, Rectangle bounds, ref Rectangle r, RasterColorScheme scheme, bool gradients, bool horizontal = false)
        {
            var textRect = new Rectangle(
                r.Left + Constants.IconWidth + 3,
                r.Top,
                bounds.Width - Constants.TextRightPadNoIcon - Constants.CsTextLeftIndent,
                Constants.TextHeight);

            if (scheme == null)
            {
                return;
            }

            int count = 0;
            foreach (var item in scheme)
            {
                Brush brush;
                if (gradients)
                {
                    brush = new LinearGradientBrush(r, item.LowColor, item.HighColor, horizontal ? 0.0f : 90.0f);
                }
                else
                {
                    brush = new SolidBrush(item.LowColor);
                }

                g.FillRectangle(brush, r);
                g.DrawRectangle(Pens.Gray, r);

                layer.Elements.Add(new LayerElement(LayerElementType.RasterColorInterval, r, count));

                DrawText(g, item.ToString(), textRect, Font, Color.Black);

                r.Y += Constants.CsItemHeightAndPad();
                textRect.Y += Constants.CsItemHeightAndPad();

                count++;
            }
        }
    }
}

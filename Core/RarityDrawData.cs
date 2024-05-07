using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;

namespace PrettyRarities.Core
{
    /// <summary>
    /// We're provided with a DrawableTooltipLine when drawing on tooltips. But if we want to draw text in any other context, such as picked up item text, fishing, etc., we cannot use those.
    /// So, that's where this struct comes in. All the names and necessary info are stored here.
    /// </summary>
    public struct RarityDrawData
    {
        public string Text;
        public float X;
        public float Y;
        public DynamicSpriteFont Font;
        public float Rotation;
        public Vector2 Origin;
        public Vector2 BaseScale;

        public RarityDrawData(string text, float x, float y, DynamicSpriteFont font, float rotation, Vector2 origin, Vector2 baseScale) {
            Text = text;
            X = x;
            Y = y;
            Font = font;
            Rotation = rotation;
            Origin = origin;
            BaseScale = baseScale;
        }
    }
}

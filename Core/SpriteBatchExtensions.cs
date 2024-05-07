using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrettyRarities.Core
{
    public static class SpriteBatchExtensions
    {
        // [public static methods]

        public static void Begin(this SpriteBatch spriteBatch, SpriteBatchData spriteBatchData) {
            spriteBatch.Begin
            (
                spriteBatchData.SortMode, spriteBatchData.BlendState, spriteBatchData.SamplerState, spriteBatchData.DepthStencilState,
                spriteBatchData.RasterizerState, spriteBatchData.Effect, spriteBatchData.Matrix
            );
        }

        public static void End(this SpriteBatch spriteBatch, out SpriteBatchData spriteBatchInfo) {
            spriteBatchInfo = new SpriteBatchData(spriteBatch);
            spriteBatch.End();
        }
    }
}

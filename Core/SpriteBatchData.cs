using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrettyRarities.Core
{
    public struct SpriteBatchData
    {
        private static readonly Func<SpriteBatch, SpriteSortMode> sortModeFieldAccessor;
        private static readonly Func<SpriteBatch, BlendState> blendStateFieldAccessor;
        private static readonly Func<SpriteBatch, SamplerState> samplerStateFieldAccessor;
        private static readonly Func<SpriteBatch, DepthStencilState> depthStencilStateFieldAccessor;
        private static readonly Func<SpriteBatch, RasterizerState> rasterizerStateFieldAccessor;
        private static readonly Func<SpriteBatch, Effect> effectFieldAccessor;
        private static readonly Func<SpriteBatch, Matrix> matrixFieldAccessor;

        // [fields]

        public SpriteSortMode SortMode;
        public BlendState BlendState;
        public SamplerState SamplerState;
        public DepthStencilState DepthStencilState;
        public RasterizerState RasterizerState;
        public Effect Effect;
        public Matrix Matrix;

        // [constructors]

        static SpriteBatchData() {
            sortModeFieldAccessor = Utilities.GetFieldAccessor<SpriteBatch, SpriteSortMode>("sortMode");
            blendStateFieldAccessor = Utilities.GetFieldAccessor<SpriteBatch, BlendState>("blendState");
            samplerStateFieldAccessor = Utilities.GetFieldAccessor<SpriteBatch, SamplerState>("samplerState");
            depthStencilStateFieldAccessor = Utilities.GetFieldAccessor<SpriteBatch, DepthStencilState>("depthStencilState");
            rasterizerStateFieldAccessor = Utilities.GetFieldAccessor<SpriteBatch, RasterizerState>("rasterizerState");
            effectFieldAccessor = Utilities.GetFieldAccessor<SpriteBatch, Effect>("customEffect");
            matrixFieldAccessor = Utilities.GetFieldAccessor<SpriteBatch, Matrix>("transformMatrix");
        }

        public SpriteBatchData(SpriteBatch spriteBatch) {
            if (spriteBatch is null)
                throw new ArgumentNullException(nameof(spriteBatch));

            SortMode = sortModeFieldAccessor(spriteBatch);
            BlendState = blendStateFieldAccessor(spriteBatch);
            SamplerState = samplerStateFieldAccessor(spriteBatch);
            DepthStencilState = depthStencilStateFieldAccessor(spriteBatch);
            RasterizerState = rasterizerStateFieldAccessor(spriteBatch);
            Effect = effectFieldAccessor(spriteBatch);
            Matrix = matrixFieldAccessor(spriteBatch);
        }
    }
}

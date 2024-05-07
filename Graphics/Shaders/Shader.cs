using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using PrettyRarities.Core;

namespace PrettyRarities.Graphics.Shaders
{
    /// <summary>
	/// A wrapper class for <see cref="Effect"/> that is less restrictive than Terraria's <see cref="MiscShaderData"/>. Supports both pixel and vertex shaders.
	/// </summary>
	public class Shader
    {
        public Ref<Effect> Effect {
            get;
            internal set;
        }
        /// <summary>
        /// The <see cref="Effect"/> that the wrapper contains.
        /// </summary>
        public Effect WrappedEffect => Effect.Value;

        /// <summary>
        /// A wrapper class for <see cref="Effect"/> that is less restrictive than Terraria's <see cref="MiscShaderData"/>.
        /// </summary>
        public Shader(Ref<Effect> effect) => Effect = effect;

        /// <summary>
        /// Sets "mainColor" to the provided value, if it exists.
        /// </summary>
        /// <param name="color"></param>
        public Shader SetMainColor(Color color) {
            WrappedEffect.Parameters["mainColor"]?.SetValue(color.ToVector3());
            return this;
        }

        public Shader SetValue(string paramName, Vector2 value) {
            WrappedEffect.Parameters[paramName]?.SetValue(value);
            return this;
        }

        public Shader SetValue(string paramName, float value) {
            WrappedEffect.Parameters[paramName]?.SetValue(value);
            return this;
        }

        public Shader SetValue(string paramName, Vector3 value) {
            WrappedEffect.Parameters[paramName]?.SetValue(value);
            return this;
        }

        public Shader SetValue(string paramName, bool value) {
            WrappedEffect.Parameters[paramName]?.SetValue(value);
            return this;
        }

        public Shader SetValue(string paramName, Texture value) {
            WrappedEffect.Parameters[paramName]?.SetValue(value);
            return this;
        }

        /// <summary>
        /// Sets "secondaryColor" to the provided value, if it exists.
        /// </summary>
        /// <param name="color"></param>
        public Shader SetSecondaryColor(Color color) {
            WrappedEffect.Parameters["secondaryColor"]?.SetValue(color.ToVector3());
            return this;
        }

        /// <summary>
        /// Sets "bloomColor" to the provided value, if it exists.
        /// </summary>
        /// <param name="color"></param>
        public Shader SetBloomColor(Color color) {
            WrappedEffect.Parameters["bloomColor"]?.SetValue(color.ToVector3());
            return this;
        }

        /// <summary>
        /// Sets "frame" to the provided value, if it exists.
        /// </summary>
        /// <param name="rectangle"></param>
        public Shader SetFrame(Rectangle rectangle) {
            WrappedEffect.Parameters["frame"]?.SetValue(rectangle.ToVector4());
            return this;
        }

        /// <summary>
        /// Sets "opacity" to the provided value, if it exists.
        /// </summary>
        /// <param name="opacity"></param>
        public Shader SetOpacity(float opacity) {
            WrappedEffect.Parameters["opacity"]?.SetValue(opacity);
            return this;
        }

        /// <summary>
        /// Apply the shader.
        /// </summary>
        /// <param name="setCommonParams"> By default, it will set the "time" and "uWorldViewProjection" parameter if it exists.</param>
        /// <param name="pass">Specify a specific pass to use, if the shader has multiple.</param>
        public Shader Apply(bool setCommonParams = true, string pass = null) {
            // Apply commonly used parameters.
            if (setCommonParams)
                ApplyParams();

            WrappedEffect.CurrentTechnique.Passes[pass ?? ShaderRegistry.AutoloadPassName]?.Apply();
            return this;
        }

        /// <summary>
        /// This is automatically called when <see cref="Apply(bool, string)"/> is.
        /// Only manually call if you are passing the effect into <see cref="SpriteBatch.Begin()"/>.
        /// </summary>
        public void ApplyParams() {
            WrappedEffect.Parameters["time"]?.SetValue(Main.GlobalTimeWrappedHourly);
        }
    }
}

using Microsoft.CodeAnalysis.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PrettyRarities.Graphics;
using PrettyRarities.Graphics.Shaders;
using PrettyRarities.Particles;
using PrettyRarities.VanillaRarities;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI.Chat;
using static System.Net.Mime.MediaTypeNames;

namespace PrettyRarities.Core;

public static class RarityHelper
{
    public static PrettyRaritiesConfig ConfigInstance = ModContent.GetInstance<PrettyRaritiesConfig>();
    public static Texture2D GlowID0 => RarityTexturesRegistry.GetTexture("RarityGlow2");
    public static Texture2D GlowID1 => RarityTexturesRegistry.GetTexture("BloomGlow");
    public static Texture2D GlowID2 => RarityTexturesRegistry.GetTexture("FlareGlow");

    public static void DrawTooltipText(RarityDrawData line, Color innerColor = default) {
        string text = line.Text;
        Vector2 textPosition = new(line.X, line.Y);

        ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, text, textPosition, innerColor, line.Rotation, line.Origin, line.BaseScale);
    }

    /// <summary>
    /// When frame skip is off, which is usually the case when High FPS Support is enabled, this prevents animations from going blisteringly fast, especially particle updates.
    /// </summary>
    /// <returns>Whether to update or not.</returns>
    public static bool ShouldUpdate() {
        return true;
    }

    public static void DrawOuterTooltipText(RarityDrawData line, Color outerColor = default, bool outerPulse = false, float averagePulseDistance = 3f, float pulseSpeed = 5.5f, float pulseRange = 2.5f) {
        string text = line.Text;
        Vector2 textPosition = new(line.X, line.Y);

        // Controls outerPulse. If disabled, these two lines are unused
        float sin = (float)((averagePulseDistance + MathF.Sin(Main.GlobalTimeWrappedHourly * pulseSpeed)) / pulseRange);
        float sineOffset = MathHelper.Lerp(0.5f, 1f, sin);

        // Draw multiple copies of the same text to simulate outer border and pulse
        for (int i = 0; i < 12; i++) {
            Vector2 offset = (MathHelper.TwoPi * i / 12f).ToRotationVector2() * 2f;

            // Make it pulse if outerPulse is enabled
            offset *= outerPulse ? sineOffset : 1f;
            ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, text, (textPosition + offset).RotatedBy(MathHelper.TwoPi * (i / 12)), outerColor * 0.9f, line.Rotation, line.Origin, line.BaseScale);
        }
    }

    public static void DrawGlow(RarityDrawData line, Color glowColor = default, int glowTextureID = 0) {
        Texture2D glowTexture = GetGlowTexture(glowTextureID);
        string text = line.Text;
        Vector2 textSize = line.Font.MeasureString(text);
        Vector2 textCenter = textSize / 2f;
        Rectangle frame = new(0, 0, glowTexture.Width, glowTexture.Height);
        Vector2 glowPosition = new(line.X + textCenter.X, line.Y + textCenter.Y / 1.08f);
        Vector2 glowScale = GetGlowScale(glowTextureID, textSize);
        glowColor.A = 0;

        Main.spriteBatch.Draw(glowTexture, glowPosition, frame, glowColor, 0f, glowTexture.Size() / 2f, glowScale, SpriteEffects.None, 0f);
    }

    private static Vector2 GetGlowScale(int glowTextureID, Vector2 textSize) {
        return glowTextureID switch {
            0 => new(textSize.X * 0.12f, 0.7f),
            1 => new(textSize.X * 0.002f, 0.06f),
            2 => new(textSize.X * 0.006f, 0.20f),
            _ => new(textSize.X * 0.12f, 0.7f)
        };
    }

    private static Texture2D GetGlowTexture(int glowTextureID) {
        return glowTextureID switch {
            0 => GlowID0,
            1 => GlowID1,
            2 => GlowID2,
            _ => GlowID0
        };
    }

    public static void UpdateTooltipParticles(RarityDrawData line, ref List<Particle> particles)
    {
        Vector2 textSize = line.Font.MeasureString(line.Text);

        // Call the Update function on each particle
        for (int i = 0; i < particles.Count; i++)
            particles[i].Update();

        // Clean up particles that have existed long enough
        particles.RemoveAll((Particle s) => s.CurrentTime >= s.Lifetime);

        // Draw each particle
        foreach (Particle particle in particles)
            particle.Draw(Main.spriteBatch, new Vector2(line.X, line.Y) + textSize * 0.5f + particle.Position);
    }

    public static void RestartSpriteBatch(bool drawing, RarityDrawContext drawContext = RarityDrawContext.Tooltip) {
        if (drawing && drawContext == RarityDrawContext.PopupText) {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);
        } else {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null, Main.GameViewMatrix.ZoomMatrix);
        }

        if (drawing && drawContext == RarityDrawContext.Tooltip) {
            Main.spriteBatch.End(); //end and begin main.spritebatch to apply a shader
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
        } else {
            Main.spriteBatch.End(); //then end and begin again to make remaining tooltip lines draw in the default way
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
        }
    }

    public static float GetParticleSpawnRateMultiplier() {
        return ConfigInstance.ParticleSettings switch {
            ParticleSettings.Disabled => -1f,
            ParticleSettings.HeavilyReduced => 0.1f,
            ParticleSettings.Reduced => 0.5f,
            ParticleSettings.Normal => 1f,
            ParticleSettings.Increased => 2f,
            ParticleSettings.HeavilyIncreased => 4f,
            _ => 1f
        };
    }

    public static void SetRarityGradient(Vector2 linePos, float timeMult, float areaMult, Color uColor, bool isGlow, Vector3 hsl,
                                             float angle, float range, float glowFactor = 1.0f, bool glowSpin = false, float glowAngle = 0f,
                                             Vector2 fadeOutStrength = default, bool ignoreUIPosition = false, bool useNoise = false,
                                             float noiseSpeed = 1f, float noiseAngle = 0f, float noiseZoom = 1f,
                                             float minimumDarknessValue = 0f, float noiseDarknessMultiplier = 1f) {
        ShaderRegistry.GetShader("RarityGradient")
            .SetValue("uUIPosition", linePos)
            .SetValue("uTimeMultiplier", timeMult)
            .SetValue("uAreaMultiplier", areaMult)
            .SetValue("uColor", uColor.ToVector3())
            .SetValue("uGlow", isGlow)
            .SetValue("uGlowSpin", glowSpin)
            .SetValue("uGlowFactor", glowFactor)
            .SetValue("uBaseHSL", hsl)
            .SetValue("uAngle", angle)
            .SetValue("uGlowAngle", glowAngle)
            .SetValue("uRange", range)
            .SetValue("uFadeOutStrength", fadeOutStrength)
            .SetValue("uUseNoise", useNoise)
            .SetValue("uNoiseSpeed", noiseSpeed)
            .SetValue("uNoiseAngle", noiseAngle)
            .SetValue("uNoiseZoom", noiseZoom)
            .SetValue("uIgnoreUIPosition", ignoreUIPosition)
            .SetValue("uMinimumDarknessValue", minimumDarknessValue)
            .SetValue("uNoiseBrightnessMultiplier", noiseDarknessMultiplier)
            .Apply();
    }

    // mfw when variable overload
    public static void SetMainRarityGradient(Vector2 linePos, float timeMult, float areaMult, Color uColor, bool isGlow, Vector3 hsl,
                                             float angle, float range, float glowFactor = 1.0f, bool glowSpin = false, float glowAngle = 0f,
                                             Vector2 fadeOutStrength = default, bool ignoreUIPosition = false, bool useNoise = false,
                                             float noiseSpeed = 1f, float noiseAngle = 0f, float noiseZoom = 1f,
                                             float minimumDarknessValue = 0f, float noiseDarknessMultiplier = 1f) {
        ShaderRegistry.GetShader("RarityGradient2")
            .SetValue("uUIPosition", linePos)
            .SetValue("uTimeMultiplier", timeMult)
            .SetValue("uAreaMultiplier", areaMult)
            .SetValue("uColor", uColor.ToVector3())
            .SetValue("uGlow", isGlow)
            .SetValue("uGlowSpin", glowSpin)
            .SetValue("uGlowFactor", glowFactor)
            .SetValue("uBaseHSL", hsl)
            .SetValue("uAngle", angle)
            .SetValue("uGlowAngle", glowAngle)
            .SetValue("uRange", range)
            .SetValue("uFadeOutStrength", fadeOutStrength)
            .SetValue("uUseNoise", useNoise)
            .SetValue("uNoiseSpeed", noiseSpeed)
            .SetValue("uNoiseAngle", noiseAngle)
            .SetValue("uNoiseZoom", noiseZoom)
            .SetValue("uIgnoreUIPosition", ignoreUIPosition)
            .SetValue("uMinimumDarknessValue", minimumDarknessValue)
            .SetValue("uNoiseBrightnessMultiplier", noiseDarknessMultiplier)
            .Apply();
    }


    // Unused because it looks like shit
    public static void SetPulseShader(Vector2 linePos, Color mainColor, Color secondaryColor, Color uColor, float pulseAngle, float pulseSpeed, float pulseTimeMultiplier, bool isGlow, bool useNoise = false, float noiseAngle = 0f, float noiseSpeed = 1f, float noiseZoomMultiplier = 1f, bool glowSpin = false, float glowAngle = 0f, Vector2 fadeOutStrength = default) {
        ShaderRegistry.GetShader("PulseGradient")
            .SetValue("uUIPosition", linePos)
            .SetValue("mainColor", mainColor.ToVector3())
            .SetValue("secondaryColor", secondaryColor.ToVector3())
            .SetValue("uPulseAngle", pulseAngle)
            .SetValue("uNoiseAngle", noiseAngle)
            .SetValue("uPulseSpeed", pulseSpeed)
            .SetValue("uNoiseSpeed", noiseSpeed)
            .SetValue("uPulseTimeMultiplier", pulseTimeMultiplier)
            .SetValue("uNoiseZoom", noiseZoomMultiplier)
            .SetValue("uGlow", isGlow).SetValue("uGlowSpin", glowSpin)
            .SetValue("uGlowAngle", glowAngle)
            .SetValue("uFadeOutStrength", fadeOutStrength)
            .SetValue("uUseNoise", useNoise)
            .SetValue("uColor", uColor.ToVector3())
            .Apply();
    }
}
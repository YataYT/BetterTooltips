using Microsoft.Xna.Framework.Graphics;
using PrettyRarities.Core;
using PrettyRarities.Graphics.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using PrettyRarities.Particles;

namespace PrettyRarities.VanillaRarities;

public static class FieryRed
{
    internal static List<Particle> particleList = new();

    public static void DrawTooltipLine(RarityDrawData line, RarityDrawContext drawContext = RarityDrawContext.Tooltip) {
        // Common variables
        int darkness = 65;
        Color outerColor = new(darkness, darkness, darkness);
        float timeMult = 1.5f;
        float areaMult = 0.02f;
        float glowAngle = Main.GlobalTimeWrappedHourly;

        // Set up spriteBatch for drawing the gradient
        Main.spriteBatch.End(out SpriteBatchData spriteBatchData);
        Main.spriteBatch.Begin(spriteBatchData with { SortMode = SpriteSortMode.Immediate });

        // Draw the glow
        if (RarityHelper.ConfigInstance.EnableGlow && (drawContext == RarityDrawContext.Tooltip || drawContext == RarityDrawContext.MouseHover)) {
            ShaderRegistry.GetShader("FieryRedRarity").SetValue("uUIPosition", new Vector2(line.X, line.Y)).SetValue("uTimeMultiplier", timeMult)
                .SetValue("uAreaMultiplier", areaMult).SetValue("uColor", Color.White.ToVector3()).SetValue("uGlow", true)
                .SetValue("uGlowSpin", true).SetValue("uGlowAngle", glowAngle).SetValue("uFadeOutStrength", (Vector2)new(0.5f, 0.8f)).Apply();
            RarityHelper.DrawGlow(line, default, 1);
        }

        // Draw the outline
        ShaderRegistry.GetShader("FieryRedRarity").SetValue("uUIPosition", new Vector2(line.X, line.Y)).SetValue("uTimeMultiplier", timeMult)
            .SetValue("uAreaMultiplier", areaMult).SetValue("uColor", outerColor.ToVector3()).SetValue("uGlow", false)
            .SetValue("uGlowSpin", false).Apply();
        RarityHelper.DrawOuterTooltipText(line, outerColor, true, 6f, 3f, 6f);

        // Draw the text
        ShaderRegistry.GetShader("FieryRedRarity").SetValue("uUIPosition", new Vector2(line.X, line.Y)).SetValue("uTimeMultiplier", timeMult)
            .SetValue("uAreaMultiplier", areaMult).SetValue("uColor", Color.White.ToVector3()).SetValue("uGlow", false)
            .SetValue("uGlowSpin", false).Apply();
        RarityHelper.DrawTooltipText(line);

        // Reset the spriteBatch back to its defaults
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(spriteBatchData);

        // Spawn and update particles
        if (drawContext == RarityDrawContext.Tooltip && RarityHelper.ShouldUpdate()) {
            SpawnTooltipParticle(line);
            RarityHelper.UpdateTooltipParticles(line, ref particleList);
        }
    }

    public static void SpawnTooltipParticle(RarityDrawData line) {
        float spawnChance = 0.5f * RarityHelper.GetParticleSpawnRateMultiplier();

        if (Main.rand.NextFloat(0f, 1f) < spawnChance) {
            Vector2 textSize = line.Font.MeasureString(line.Text);
            int lifetime = 40;
            Vector2 position = new Vector2(Main.rand.NextFloat(-textSize.X * 0.45f, textSize.X * 0.45f), textSize.Y * 0.6f);
            Vector2 velocity = Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) * Main.rand.NextFloat(-1f, -0.6f);
            float maxScale = Main.rand.NextFloat(0.8f, 1.7f);
            float startingRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
            float rotationSpeed = Main.rand.NextFloat(-0.12f, 0.12f);
            Color particleColor = Color.Lerp(new Color(191, 69, 69), new Color(176, 176, 62), Main.rand.NextFloat());
            int fadeInTime = 10;
            int fadeOutTime = 10;
            particleList.Add(new Particle(ParticleType.FireParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor, fadeInTime, fadeOutTime, false));
        }
    }
}

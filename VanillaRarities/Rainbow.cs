using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PrettyRarities.Core;
using PrettyRarities.Graphics.Shaders;
using PrettyRarities.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace PrettyRarities.VanillaRarities;

public static class Rainbow
{
    internal static List<Particle> particleList = new();

    public static void DrawTooltipLine(RarityDrawData line, RarityDrawContext drawContext = RarityDrawContext.Tooltip) {
        // Common variables
        int darkness = 50;
        Color outerColor = new(darkness, darkness, darkness);
        float timeMult = 1.5f;
        float areaMult = 0.01f;

        // Set up spriteBatch for drawing the gradient
        Main.spriteBatch.End(out SpriteBatchData spriteBatchData);
        Main.spriteBatch.Begin(spriteBatchData with { SortMode = SpriteSortMode.Immediate });

        // Draw the glow
        if (RarityHelper.ConfigInstance.EnableGlow && (drawContext == RarityDrawContext.Tooltip || drawContext == RarityDrawContext.MouseHover)) {
            ShaderRegistry.GetShader("RainbowRarity").SetValue("uUIPosition", new Vector2(line.X, line.Y)).SetValue("uTimeMultiplier", timeMult)
                .SetValue("uAreaMultiplier", areaMult).SetValue("uColor", Color.White.ToVector3()).SetValue("uGlow", true)
                .SetValue("uFadeOutStrength", (Vector2)new(0.0f, 0.0f)).Apply();
            RarityHelper.DrawGlow(line, default, 0);
        }

        // Draw the outline
        ShaderRegistry.GetShader("RainbowRarity").SetValue("uUIPosition", new Vector2(line.X, line.Y)).SetValue("uTimeMultiplier", timeMult)
            .SetValue("uGlowSpin", false).SetValue("uAreaMultiplier", areaMult).SetValue("uColor", outerColor.ToVector3()).SetValue("uGlow", false).Apply();
        RarityHelper.DrawOuterTooltipText(line, outerColor, true, 6f, 3f, 6f);

        // Draw the text
        ShaderRegistry.GetShader("RainbowRarity").SetValue("uUIPosition", new Vector2(line.X, line.Y)).SetValue("uTimeMultiplier", timeMult)
            .SetValue("uGlowSpin", false).SetValue("uAreaMultiplier", areaMult).SetValue("uColor", Color.White.ToVector3()).SetValue("uGlow", false).Apply();
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
        float spawnChance = 0.2f * RarityHelper.GetParticleSpawnRateMultiplier();

        if (Main.rand.NextFloat(0f, 1f) < spawnChance) {
            Vector2 textSize = line.Font.MeasureString(line.Text);
            int lifetime = 60;
            Vector2 position = new Vector2(Main.rand.NextFloat(-(textSize.X / 2f) + 10f, (textSize.X / 2f) - 10f), textSize.Y * -0.7f);
            Vector2 velocity = (Vector2.UnitY * Main.rand.NextFloat(-0.6f, -0.7f)).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2 * 0.5f, MathHelper.PiOver2 * 0.5f) + MathHelper.Pi);
            ;
            float maxScale = Main.rand.NextFloat(0.7f, 1f);
            float startingRotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
            float rotationSpeed = Main.rand.NextFloat(-0.08f, 0.08f);
            Color particleColor = Main.hslToRgb(Main.rand.NextFloat(0, 1), 1f, 0.4f);
            int fadeInTime = 20;
            int fadeOutTime = 20;
            particleList.Add(new Particle(ParticleType.GlowyShardParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor, fadeInTime, fadeOutTime, true, frameOffset: Main.rand.Next(0, 3)));
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PrettyRarities.Core;
using PrettyRarities.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace PrettyRarities.VanillaRarities;

public static class Orange
{
    internal static List<Particle> particleList = new List<Particle>();

    public static void DrawTooltipLine(RarityDrawData line, RarityDrawContext drawContext = RarityDrawContext.Tooltip)
    {
        // Common variables
        int darkness = 30;
        Color outerColor = new(darkness, darkness, darkness);
        float timeMult = 4f;
        float areaMult = 0.01f;
        float range = 0.12f;
        Vector3 hsl = new Vector3(192f / 360f, 1.9f, 1.4f);

        // Set up spriteBatch for drawing the gradient
        Main.spriteBatch.End(out SpriteBatchData spriteBatchData);
        Main.spriteBatch.Begin(spriteBatchData with { SortMode = SpriteSortMode.Immediate });

        // Draw the glow
        if (RarityHelper.ConfigInstance.EnableGlow && (drawContext == RarityDrawContext.Tooltip || drawContext == RarityDrawContext.MouseHover)) {
            RarityHelper.SetMainRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, true, hsl, 0, range, 0.15f);
            RarityHelper.DrawGlow(line, Color.White);
        }

        // Draw the outline
        RarityHelper.SetMainRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, outerColor, false, hsl, 0, range);
        RarityHelper.DrawOuterTooltipText(line, outerColor, true, 6f, 3f, 6f);

        // Then draw the inner text
        RarityHelper.SetMainRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, false, hsl, 0, range);
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
        float spawnChance = 0.04f * RarityHelper.GetParticleSpawnRateMultiplier();

        if (Main.rand.NextFloat(0f, 1f) < spawnChance)
        {
            Vector2 textSize = line.Font.MeasureString(line.Text);
            int lifetime = 55;
            Vector2 position = new Vector2(Main.rand.NextFloat(-textSize.X * 0.45f, textSize.X * 0.45f), textSize.Y * 0.6f);
            Vector2 velocity = Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) * Main.rand.NextFloat(-0.6f, -0.4f);
            float maxScale = Main.rand.NextFloat(1f, 2.4f);
            float startingRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
            float rotationSpeed = Main.rand.NextFloat(-0.12f, 0.12f);
            Color particleColor = Color.Lerp(new Color(191, 69, 69), new Color(176, 176, 62), Main.rand.NextFloat());
            int fadeInTime = 10;
            int fadeOutTime = 10;
            particleList.Add(new Particle(ParticleType.FireParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor, fadeInTime, fadeOutTime, false));
        }
    }
}

using Microsoft.Xna.Framework;
using PrettyRarities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework.Graphics;

namespace PrettyRarities.VanillaRarities;

public static class Cyan
{
    internal static List<Particle> particleList = new List<Particle>();

    public static void DrawTooltipLine(RarityDrawData line, RarityDrawContext drawContext = RarityDrawContext.Tooltip) {
        // Common variables
        int darkness = 40;
        Color outerColor = new(darkness, darkness, darkness);
        float timeMult = -3f;
        float areaMult = 0.01f;
        float range = 0.2f;
        Vector3 hsl = new Vector3(193f / 360f, 1.9f, 0.4f);
        float angle = MathHelper.Pi * 0.15f;

        // Set up spriteBatch for drawing the gradient
        Main.spriteBatch.End(out SpriteBatchData spriteBatchData);
        Main.spriteBatch.Begin(spriteBatchData with { SortMode = SpriteSortMode.Immediate });

        // Draw the glow
        if (RarityHelper.ConfigInstance.EnableGlow && (drawContext == RarityDrawContext.Tooltip || drawContext == RarityDrawContext.MouseHover)) {
            RarityHelper.SetMainRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, true, hsl, angle, range, 0.5f);
            RarityHelper.DrawGlow(line, Color.White);
        }

        // Draw the outline
        RarityHelper.SetMainRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, outerColor, false, hsl, angle, range);
        RarityHelper.DrawOuterTooltipText(line, outerColor, true, 6f, 3f, 6f);

        // Then draw the inner text
        RarityHelper.SetMainRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, false, hsl, angle, range);
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
        float spawnChance = 0.25f * RarityHelper.GetParticleSpawnRateMultiplier();

        if (Main.rand.NextFloat(0f, 1f) < spawnChance) {
            Vector2 textSize = line.Font.MeasureString(line.Text);
            int lifetime = 85;
            Vector2 position = new Vector2(Main.rand.NextFloat(-textSize.X * 0.5f, textSize.X * 0.5f), textSize.Y * 0.6f);
            Vector2 velocity = Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) * Main.rand.NextFloat(-0.4f, -0.3f);
            float maxScale = Main.rand.NextFloat(0.4f, 0.8f);
            float startingRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
            float rotationSpeed = Main.rand.NextFloat(-0.12f, 0.12f);
            Color particleColor = Color.Lerp(new Color(5, 226, 255), new Color(5, 163, 255), Main.rand.NextFloat());
            int fadeInTime = 50;
            int fadeOutTime = 15;
            particleList.Add(new Particle(ParticleType.BloomParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor, fadeInTime, fadeOutTime, true, frameOffset: Main.rand.Next(2)));
        }
    }
}

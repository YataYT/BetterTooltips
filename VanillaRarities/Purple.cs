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

public static class Purple
{
    internal static List<Particle> particleList = new List<Particle>();

    public static void DrawTooltipLine(RarityDrawData line, RarityDrawContext drawContext = RarityDrawContext.Tooltip) {
        // Common variables
        int darkness = 30;
        Color outerColor = new(darkness, darkness, darkness);
        float timeMult = 5f;
        float areaMult = 0.02f;
        float range = 0.14f;
        Vector3 hsl = new Vector3(279f / 360f, 1.9f, 0.5f);
        float angle = MathHelper.Lerp(0, MathHelper.TwoPi, Main.GlobalTimeWrappedHourly * 0.02f % 1);

        // Set up spriteBatch for drawing the gradient
        Main.spriteBatch.End(out SpriteBatchData spriteBatchData);
        Main.spriteBatch.Begin(spriteBatchData with { SortMode = SpriteSortMode.Immediate });

        // Draw the glow
        if (RarityHelper.ConfigInstance.EnableGlow && (drawContext == RarityDrawContext.Tooltip || drawContext == RarityDrawContext.MouseHover)) {
            RarityHelper.SetMainRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, true, hsl, angle, range, 1.3f, true, Main.GlobalTimeWrappedHourly, new(0.5f, 0.8f));
            RarityHelper.DrawGlow(line, Color.White, 1);
        }

        // Draw the outline
        RarityHelper.SetRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, outerColor, false, hsl, angle, range);
        RarityHelper.DrawOuterTooltipText(line, outerColor, true, 6f, 3f, 6f);

        // Then draw the inner text
        RarityHelper.SetRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, false, hsl, angle, range);
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
        float spawnChance = 0.03f * RarityHelper.GetParticleSpawnRateMultiplier();
        if (Main.rand.NextFloat(0f, 1f) < spawnChance) {
            Vector2 textSize = line.Font.MeasureString(line.Text);
            int lifetime = 50;
            Vector2 position = Main.rand.NextVector2FromRectangle(new((int)(-textSize.X * 0.5f), (int)(-textSize.Y * 0.5f), (int)(textSize.X * 0.9f), (int)(textSize.Y * 0.9f)));
            Vector2 velocity = Vector2.Zero;
            float maxScale = Main.rand.NextFloat(0.05f, 0.1f);
            float startingRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
            float rotationSpeed = 0f;
            Color particleColor = new Color(180, 40, 255) * Main.rand.NextFloat(0.5f, 1.5f);
            int fadeInTime = 20;
            int fadeOutTime = 20;
            particleList.Add(new Particle(ParticleType.BurstParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale * 1.2f, lifetime, Color.White * 0.5f, fadeInTime, fadeOutTime, true));
            particleList.Add(new Particle(ParticleType.BurstParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor * 0.5f, fadeInTime, fadeOutTime, true));
        }

        float spawnChance2 = 0.2f * RarityHelper.GetParticleSpawnRateMultiplier();
        if (Main.rand.NextFloat(0f, 1f) < spawnChance2) {
            Vector2 textSize = line.Font.MeasureString(line.Text);
            int lifetime = 85;
            Vector2 position = new Vector2(Main.rand.NextFloat(-textSize.X * 0.5f, textSize.X * 0.5f), textSize.Y * 0.6f);
            Vector2 velocity = Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) * Main.rand.NextFloat(-0.4f, -0.3f);
            float maxScale = Main.rand.NextFloat(0.4f, 0.8f);
            float startingRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
            float rotationSpeed = Main.rand.NextFloat(-0.12f, 0.12f);
            Color particleColor = new Color(180, 40, 255) * Main.rand.NextFloat(0.5f, 1.5f) * 0.7f;
            int fadeInTime = 50;
            int fadeOutTime = 15;
            int randomFrame = Main.rand.Next(2);
            particleList.Add(new Particle(ParticleType.BloomParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale * 1.1f, lifetime, Color.White * 0.7f, fadeInTime, fadeOutTime, true, frameOffset: randomFrame));
            particleList.Add(new Particle(ParticleType.BloomParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor, fadeInTime, fadeOutTime, true, frameOffset: randomFrame));
        }
    }
}

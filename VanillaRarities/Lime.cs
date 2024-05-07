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

public static class Lime
{
    internal static List<Particle> particleList = new List<Particle>();

    public static void DrawTooltipLine(RarityDrawData line, RarityDrawContext drawContext = RarityDrawContext.Tooltip) {
        // Common variables
        int darkness = 40;
        Color outerColor = new(darkness, darkness, darkness);
        float timeMult = 4.5f;
        float areaMult = 0.01f;
        float range = 0.15f;
        Vector3 hsl = new Vector3(86f / 360f, 1.7f, 0.5f);
        float angle = 0;

        // Set up spriteBatch for drawing the gradient
        Main.spriteBatch.End(out SpriteBatchData spriteBatchData);
        Main.spriteBatch.Begin(spriteBatchData with { SortMode = SpriteSortMode.Immediate });

        // Draw the glow
        if (RarityHelper.ConfigInstance.EnableGlow && (drawContext == RarityDrawContext.Tooltip || drawContext == RarityDrawContext.MouseHover)) {
            RarityHelper.SetMainRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, true, hsl, angle, range, 0.35f);
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
        float spawnChance = 0.15f * RarityHelper.GetParticleSpawnRateMultiplier();

        if (Main.rand.NextFloat(0f, 1f) < spawnChance) {
            Vector2 textSize = line.Font.MeasureString(line.Text);
            int lifetime = 95;
            Vector2 position = new Vector2(Main.rand.NextFloat(-textSize.X * 0.55f, textSize.X * 0.55f), textSize.Y * 0.6f);
            Vector2 velocity = Vector2.UnitY * -0.3f;
            float maxScale = Main.rand.NextFloat(0.7f, 0.8f);
            float startingRotation = 0f;
            float rotationSpeed = 0f;
            Color particleColor = new Color(41, 153, 0) * Main.rand.NextFloat(0.6f, 1f);
            int fadeInTime = 10;
            int fadeOutTime = 10;
            particleList.Add(new Particle(ParticleType.CodeParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor, fadeInTime, fadeOutTime, true, frameOffset: Main.rand.Next(0, 11)));
        }
    }
}

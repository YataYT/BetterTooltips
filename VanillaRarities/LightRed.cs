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

public static class LightRed
{
    internal static List<Particle> particleList = new List<Particle>();

    public static void DrawTooltipLine(RarityDrawData line, RarityDrawContext drawContext = RarityDrawContext.Tooltip) {
        // Common variables
        int darkness = 45;
        Color outerColor = new(darkness, darkness, darkness);
        float timeMult = -4f;
        float areaMult = 0.1f;
        float range = 0.12f;
        Vector3 hsl = new Vector3(0f, 1.9f, 0.83f);
        float angle = MathHelper.PiOver2;

        // Set up spriteBatch for drawing the gradient
        Main.spriteBatch.End(out SpriteBatchData spriteBatchData);
        Main.spriteBatch.Begin(spriteBatchData with { SortMode = SpriteSortMode.Immediate });

        // Draw the glow
        if (RarityHelper.ConfigInstance.EnableGlow && (drawContext == RarityDrawContext.Tooltip || drawContext == RarityDrawContext.MouseHover)) {
            RarityHelper.SetMainRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, true, hsl, angle, range, 0.25f);
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
        float spawnChance = 0.08f * RarityHelper.GetParticleSpawnRateMultiplier();

        if (Main.rand.NextFloat(0f, 1f) < spawnChance) {
            Vector2 textSize = line.Font.MeasureString(line.Text);
            int lifetime = 90;
            Vector2 position = new Vector2(Main.rand.NextFloat(-textSize.X * 0.55f, textSize.X * 0.55f), textSize.Y * 0.6f);
            Vector2 velocity = Vector2.UnitY * Main.rand.NextFloat(-0.25f, -0.35f);
            float maxScale = Main.rand.NextFloat(0.6f, 1.6f);
            float startingRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
            float rotationSpeed = Main.rand.NextFloat(-0.12f, 0.12f);
            Color particleColor = Color.Lerp(new Color(191, 69, 69), new Color(176, 176, 62), Main.rand.NextFloat()) * 0.7f;
            int fadeInTime = 15;
            int fadeOutTime = 15;
            particleList.Add(new Particle(ParticleType.SquareParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor, fadeInTime, fadeOutTime, true));
        }
    }
}

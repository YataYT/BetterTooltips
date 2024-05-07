using Microsoft.Xna.Framework;
using PrettyRarities.Core;
using PrettyRarities.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework.Graphics;

namespace PrettyRarities.VanillaRarities;

public static class Green
{
    internal static List<Particle> particleList = new();

    public static void DrawTooltipLine(RarityDrawData line, RarityDrawContext drawContext = RarityDrawContext.Tooltip)
    {
        // Common variables
        int darkness = 45;
        Color outerColor = new(darkness, darkness, darkness);
        float timeMult = 3f;
        float areaMult = 0.01f;
        float range = 0.13f;
        Vector3 hsl = new Vector3(120f / 360f, 1.3f, 0.7f);

        // Set up spriteBatch for drawing the gradient
        Main.spriteBatch.End(out SpriteBatchData spriteBatchData);
        Main.spriteBatch.Begin(spriteBatchData with { SortMode = SpriteSortMode.Immediate });

        // Draw the outline
        RarityHelper.SetRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, outerColor, false, hsl, 0, range);
        RarityHelper.DrawOuterTooltipText(line);

        // Then draw the inner text
        RarityHelper.SetRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, false, hsl, 0, range);
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
        float spawnChance = 0.01f * RarityHelper.GetParticleSpawnRateMultiplier();

        if (Main.rand.NextFloat(0f, 1f) < spawnChance)
        {
            Vector2 textSize = line.Font.MeasureString(line.Text);
            int lifetime = 120;
            Vector2 position = Main.rand.NextVector2FromRectangle(new((int)(-textSize.X * 0.5f), (int)(-textSize.Y * 0.5f), (int)(textSize.X * 1f), (int)(textSize.Y * 0.8f)));
            Vector2 velocity = Vector2.Zero;
            float maxScale = Main.rand.NextFloat(0.4f, 1f);
            float startingRotation = Main.rand.NextFloat(0, MathHelper.PiOver2);
            float rotationSpeed = Main.rand.NextFloat(0.008f, 0.02f) * (Main.rand.NextBool() ? 1 : -1);
            Color particleColor = Color.Lerp(new Color(196, 255, 196), new Color(138, 255, 138), Main.rand.NextFloat());
            int fadeInTime = 15;
            int fadeOutTime = 15;
            Rectangle? baseFrame = new(0, 10, 10, 10);
            particleList.Add(new Particle(ParticleType.SparkleParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor, fadeInTime, fadeOutTime, true, textSize));
            particleList.Add(new Particle(ParticleType.SparkleParticle, position, startingRotation + MathHelper.PiOver4, r => velocity, r => rotationSpeed, maxScale * 0.65f, lifetime, particleColor, fadeInTime, fadeOutTime, true, textSize));
        }
    }
}

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
using PrettyRarities.Graphics.Shaders;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;

namespace PrettyRarities.VanillaRarities;

public static class Blue
{
    internal static List<Particle> particleList = new();

    public static void DrawTooltipLine(RarityDrawData line, RarityDrawContext drawContext = RarityDrawContext.Tooltip)
    {
        // Common variables
        int darkness = 50;
        Color outerColor = new(darkness, darkness, darkness);
        float timeMult = 1.5f;
        float areaMult = 0.01f;
        float range = 0.08f;
        Vector3 hsl = new Vector3(240f / 360f, 1f, 0.8f);

        // Set up spriteBatch for drawing the gradient
        Main.spriteBatch.End(out SpriteBatchData spriteBatchData);
        Main.spriteBatch.Begin(spriteBatchData with { SortMode = SpriteSortMode.Immediate });

        // Draw the outline
        RarityHelper.SetRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, outerColor, false, hsl, MathHelper.PiOver4, range);
        RarityHelper.DrawOuterTooltipText(line);

        // Then draw the inner text
        RarityHelper.SetRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, false, hsl, MathHelper.PiOver4, range);
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

    public static void SpawnTooltipParticle(RarityDrawData line)
    {
        float spawnChance = 0.005f * RarityHelper.GetParticleSpawnRateMultiplier();

        if (Main.rand.NextFloat(0f, 1f) < spawnChance)
        {
            Vector2 textSize = line.Font.MeasureString(line.Text);
            int lifetime = 120;
            Vector2 position = Main.rand.NextVector2FromRectangle(new((int)(-textSize.X * 0.5f), (int)(-textSize.Y * 0.5f), (int)(textSize.X * 0.8f), (int)(textSize.Y * 0.8f)));
            Vector2 velocity = Vector2.Zero;
            float maxScale = Main.rand.NextFloat(0.3f, 1f);
            float startingRotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
            float rotationSpeed = Main.rand.NextFloat(-0.01f, 0.01f);
            Color particleColor = Color.Lerp(Color.Lerp(new Color(89, 89, 255), new Color(140, 140, 255), Main.rand.NextFloat()), Color.White, 0.5f);
            int fadeInTime = 15;
            int fadeOutTime = 15;
            particleList.Add(new Particle(ParticleType.TwinkleParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor, fadeInTime, fadeOutTime, false));
        }
    }
}

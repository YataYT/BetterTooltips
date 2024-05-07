using Microsoft.CodeAnalysis.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PrettyRarities.Core;
using PrettyRarities.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace PrettyRarities.VanillaRarities;

public static class Amber
{
    internal static List<Particle> particleList = new();

    public static void DrawTooltipLine(RarityDrawData line, RarityDrawContext drawContext = RarityDrawContext.Tooltip)
    {
        // Common variables
        int darkness = 45;
        Color outerColor = new(darkness, darkness, darkness);
        float timeMult = 2f;
        float areaMult = 0.01f;
        float range = 0.24f;
        Vector3 hsl = new Vector3(41f / 360f, 1.9f, 0.6f);

        // Set up spriteBatch for drawing the gradient
        Main.spriteBatch.End(out SpriteBatchData spriteBatchData);
        Main.spriteBatch.Begin(spriteBatchData with { SortMode = SpriteSortMode.Immediate });

        // Draw the glow
        if (RarityHelper.ConfigInstance.EnableGlow && (drawContext == RarityDrawContext.Tooltip || drawContext == RarityDrawContext.MouseHover)) {
            RarityHelper.SetMainRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, true, hsl, 0, range, 0.4f);
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

    public static void SpawnTooltipParticle(RarityDrawData line)
    {
        float spawnChance = 0.03f * RarityHelper.GetParticleSpawnRateMultiplier();

        if (Main.rand.NextFloat(0f, 1f) < spawnChance)
        {
            Vector2 textSize = line.Font.MeasureString(line.Text);
            int lifetime = 130;
            Vector2 position = new Vector2(Main.rand.NextFloat(-(textSize.X / 2f) + 10f, (textSize.X / 2f) - 10f), textSize.Y / 1.8f);
            Vector2 velocity = (Vector2.UnitY * Main.rand.NextFloat(0.10f, 0.25f)).RotatedBy(Main.rand.NextFloat(MathHelper.Pi - (MathHelper.PiOver2 * 0.5f), MathHelper.Pi + (MathHelper.PiOver2 * 0.5f)));
            float maxScale = Main.rand.NextFloat(0.3f, 1f);
            float startingRotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
            float rotationSpeed = Main.rand.NextFloat(-0.08f, 0.08f);
            Color particleColor = Color.Lerp(new Color(255, 215, 128), new Color(89, 61, 0), Main.rand.NextFloat());
            int fadeInTime = 30;
            int fadeOutTime = 30;
            particleList.Add(new Particle(ParticleType.TwinkleParticle, position, startingRotation, v => velocity, r => rotationSpeed, maxScale, lifetime, particleColor, fadeInTime, fadeOutTime));
        }
    }
}
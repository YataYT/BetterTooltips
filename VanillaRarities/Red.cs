using Microsoft.Xna.Framework;
using PrettyRarities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework.Graphics;

namespace PrettyRarities.VanillaRarities;

public static class Red
{
    internal static List<Particle> particleList = new List<Particle>();

    public static void DrawTooltipLine(RarityDrawData line, RarityDrawContext drawContext = RarityDrawContext.Tooltip) {
        // Common variables
        int darkness = 30;
        Color outerColor = new(darkness, darkness, darkness);
        float timeMult = 3f;
        float areaMult = 0.03f;
        float range = 0.2f;
        Vector3 hsl = new Vector3(349f / 360f, 1.9f, 0.5f);
        float angle = 0;

        // Set up spriteBatch for drawing the gradient
        Main.spriteBatch.End(out SpriteBatchData spriteBatchData);
        Main.spriteBatch.Begin(spriteBatchData with { SortMode = SpriteSortMode.Immediate });

        // Draw the glow
        if (RarityHelper.ConfigInstance.EnableGlow && (drawContext == RarityDrawContext.Tooltip || drawContext == RarityDrawContext.MouseHover)) {
            RarityHelper.SetMainRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, true, hsl, angle, range, 0.4f, true, Main.GlobalTimeWrappedHourly * -1.2f, new Vector2(0.5f, 0.5f));
            RarityHelper.DrawGlow(line, Color.White);

            RarityHelper.DrawGlow(line, Color.White, 2);
        }

        // Draw the outline
        RarityHelper.SetRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, outerColor, false, hsl, angle, range);
        RarityHelper.DrawOuterTooltipText(line, outerColor, true, 8f, 3f, 6f);

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
        float spawnChance = 0.06f * RarityHelper.GetParticleSpawnRateMultiplier();

        if (Main.rand.NextFloat(0f, 1f) < spawnChance) {
            Vector2 textSize = line.Font.MeasureString(line.Text);
            int lifetime = 55;
            Vector2 position = Main.rand.NextVector2FromRectangle(new((int)(-textSize.X * 0.5f), (int)(-textSize.Y * 0.5f), (int)(textSize.X * 1f), (int)(textSize.Y * 0.8f)));
            Vector2 velocity = Vector2.Zero;
            float maxScale = Main.rand.NextFloat(0.5f, 1f);
            float startingRotation = Main.rand.NextFloat(MathHelper.PiOver2);
            float rotationSpeed = Main.rand.NextFloat(-0.06f, 0.06f);
            Color particleColor = Color.Lerp(new Color(255, 40, 40), new Color(255, 40, 72), Main.rand.NextFloat()) * Main.rand.NextFloat(0.5f, 1f);
            int fadeInTime = 15;
            int fadeOutTime = 15;
            particleList.Add(new Particle(ParticleType.TwinkleParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale * 1f, lifetime, Color.White * Main.rand.NextFloat(0.1f, 0.6f), fadeInTime, fadeOutTime, false));
            particleList.Add(new Particle(ParticleType.TwinkleParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor, fadeInTime, fadeOutTime, false));
        }
    }
}

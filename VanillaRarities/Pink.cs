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

public static class Pink
{
    internal static List<Particle> particleList = new List<Particle>();

    public static void DrawTooltipLine(RarityDrawData line, RarityDrawContext drawContext = RarityDrawContext.Tooltip) {
        // Common variables
        int darkness = 55;
        Color outerColor = new(darkness, darkness, darkness);
        float timeMult = 5f;
        float areaMult = 0.03f;
        float range = 0.1f;
        Vector3 hsl = new Vector3(300 / 360f, 1f, 0.7f);
        float angle = MathHelper.Pi / 0.55f;

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
        float spawnChance = 0.05f * RarityHelper.GetParticleSpawnRateMultiplier();

        if (Main.rand.NextFloat(0f, 1f) < spawnChance) {
            Vector2 textSize = line.Font.MeasureString(line.Text);
            int lifetime = (int)(textSize.X * 0.73f);
            Vector2 position = new Vector2(textSize.X * 0.6f, textSize.Y * Main.rand.NextFloat(-0.5f, 0.5f));
            Vector2 velocity = Vector2.UnitX * -1.55f;
            float maxScale = Main.rand.NextFloat(0.3f, 0.5f);
            float startingRotation = Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4) / 3f;
            float rotationSpeed = 0f;
            Color particleColor = new Color(255, 150, 255) * Main.rand.NextFloat(0.6f, 1.4f);
            int fadeInTime = 15;
            int fadeOutTime = 15;
            particleList.Add(new Particle(ParticleType.MusicParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor * 0.75f, fadeInTime, fadeOutTime, true, frameOffset: Main.rand.Next(1, 3)));
        }
    }
}

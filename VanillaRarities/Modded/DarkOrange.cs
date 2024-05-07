using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PrettyRarities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace PrettyRarities.VanillaRarities.Modded
{
    public static class DarkOrange
    {
        internal static List<Particle> particleList = new();

        public static void DrawTooltipLine(RarityDrawData line, RarityDrawContext drawContext = RarityDrawContext.Tooltip) {
            // Common variables
            int darkness = 40;
            Color outerColor = new(darkness, darkness, darkness);
            float timeMult = 2f;
            float areaMult = 0.02f;
            float range = 0.12f;
            Vector3 hsl = new Vector3(13f / 360f, 1.4f, 0.7f);
            float angle = MathHelper.Pi * 0.9f;
            float glowFactor = 0.9f;
            float noiseSpeed = 0.01f;
            float noiseAngle = MathHelper.PiOver2 * 1.5f;
            float noiseZoom = 0.008f;
            float minimumDarknessValue = 0.2f;
            float darknessMultiplier = 2f;

            // Set up spriteBatch for drawing the gradient
            Main.spriteBatch.End(out SpriteBatchData spriteBatchData);
            Main.spriteBatch.Begin(spriteBatchData with { SortMode = SpriteSortMode.Immediate });

            // Set the noise
            Main.instance.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("PrettyRarities/Assets/Textures/Noise/TechyNoise").Value;

            // Draw the glow
            if (RarityHelper.ConfigInstance.EnableGlow && (drawContext == RarityDrawContext.Tooltip || drawContext == RarityDrawContext.MouseHover)) {
                RarityHelper.SetMainRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, true, hsl, angle,
                    range, glowFactor, false, 0f, new Vector2(0.5f, 0.8f), true, true, noiseSpeed, noiseAngle, noiseZoom, minimumDarknessValue, darknessMultiplier);
                RarityHelper.DrawGlow(line, Color.White, 0);
            }

            // Draw the outline
            RarityHelper.SetRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, outerColor, false, hsl, angle, range, default, default, default, default, true, false, noiseSpeed, noiseAngle, noiseZoom, minimumDarknessValue, darknessMultiplier);
            RarityHelper.DrawOuterTooltipText(line, outerColor, true, 6f, 3f, 6f);

            // Then draw the inner text
            RarityHelper.SetRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, false, hsl, angle, range, default, default, default, default, true, false, noiseSpeed, noiseAngle, noiseZoom, minimumDarknessValue, darknessMultiplier);
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
                int lifetime = 95;
                Vector2 position = Main.rand.NextVector2FromRectangle(new((int)(-textSize.X * 0.5f), (int)(-textSize.Y * 0.5f), (int)(textSize.X * 1f), (int)(textSize.Y * 0.8f)));
                Vector2 velocity = Vector2.Zero;
                float maxScale = Main.rand.NextFloat(0.7f, 0.8f);
                float startingRotation = 0f;
                float rotationSpeed = 0f;
                Color particleColor = new Color(153, 71, 0) * Main.rand.NextFloat(0.6f, 1.4f);
                int fadeInTime = 10;
                int fadeOutTime = 10;
                particleList.Add(new Particle(ParticleType.CodeParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor, fadeInTime, fadeOutTime, true, frameOffset: Main.rand.Next(0, 11)));
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PrettyRarities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace PrettyRarities.VanillaRarities.Modded
{
    public static class Turquoise
    {
        internal static List<Particle> particleList = new();

        public static void DrawTooltipLine(RarityDrawData line, RarityDrawContext drawContext = RarityDrawContext.Tooltip) {
            // Common variables
            int darkness = 25;
            Color outerColor = new(darkness, darkness, darkness);
            float timeMult = 4f;
            float areaMult = 0.02f;
            float range = 0.32f;
            Vector3 hsl = new Vector3(167f / 360f, 2f, 0.6f);
            float angle = MathHelper.PiOver2 * -1.5f;
            float glowFactor = 1f + 0.4f * MathF.Cos(Main.GlobalTimeWrappedHourly * 1.5f);
            float noiseSpeed = 0.2f;
            float noiseAngle = MathHelper.PiOver2 * 1.5f;
            float noiseZoom = 0.004f;
            float minimumDarknessValue = 0f;
            float darknessMultiplier = 2f;

            // Set up spriteBatch for drawing the gradient
            Main.spriteBatch.End(out SpriteBatchData spriteBatchData);
            Main.spriteBatch.Begin(spriteBatchData with { SortMode = SpriteSortMode.Immediate });

            // Set the noise
            Main.instance.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("PrettyRarities/Assets/Textures/Noise/WaterNoise").Value;

            // Draw the glow
            if (RarityHelper.ConfigInstance.EnableGlow && (drawContext == RarityDrawContext.Tooltip || drawContext == RarityDrawContext.MouseHover)) {
                RarityHelper.SetMainRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, true, hsl, angle,
                    range, glowFactor, false, Main.GlobalTimeWrappedHourly * 0.5f, new Vector2(0.5f, 0.8f), false, true, noiseSpeed, noiseAngle, noiseZoom, minimumDarknessValue, darknessMultiplier);
                RarityHelper.DrawGlow(line, Color.White, 0);
            }

            // Draw the outline
            RarityHelper.SetMainRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, outerColor, false, hsl, angle, range, default, default, default, default, false, false);
            RarityHelper.DrawOuterTooltipText(line, outerColor, true, 6f, 3f, 6f);

            // Then draw the inner text
            RarityHelper.SetMainRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, false, hsl, angle, range, default, default, default, default, false, false);
            RarityHelper.DrawTooltipText(line);

            // Reset the spriteBatch back to its defaults

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
                int lifetime = 120;
                Vector2 position = Main.rand.NextVector2FromRectangle(new((int)(-textSize.X * 0.5f), (int)(-textSize.Y * 0.5f), (int)(textSize.X * 1f), (int)(textSize.Y * 0.8f)));
                Vector2 velocity = Vector2.Zero;
                float maxScale = Main.rand.NextFloat(0.4f, 0.7f);
                float startingRotation = Main.rand.NextFloat(0, MathHelper.PiOver2);
                float rotationSpeed = Main.rand.NextFloat(0.008f, 0.02f) * (Main.rand.NextBool() ? 1 : -1);
                Color particleColor = Color.Lerp(new Color(0, 255, 157), new Color(0, 255, 242), Main.rand.NextFloat()) * Main.rand.NextFloat(0.4f, 0.7f);                ;
                int fadeInTime = 15;
                int fadeOutTime = 15;
                Rectangle? baseFrame = new(0, 10, 10, 10);
                particleList.Add(new Particle(ParticleType.SparkleParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor, fadeInTime, fadeOutTime, true, textSize));
                particleList.Add(new Particle(ParticleType.SparkleParticle, position, startingRotation + MathHelper.PiOver4, r => velocity, r => rotationSpeed, maxScale * 0.65f, lifetime, particleColor, fadeInTime, fadeOutTime, true, textSize));
            }
        }
    }
}

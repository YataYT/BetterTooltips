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
    public static class PureGreen
    {
        internal static List<Particle> particleList = new();

        public static void DrawTooltipLine(RarityDrawData line, RarityDrawContext drawContext = RarityDrawContext.Tooltip) {
            // Common variables
            int darkness = 50;
            Color outerColor = new(darkness, darkness, darkness);
            float timeMult = 2.3f;
            float areaMult = 0.02f;
            float range = 0.24f;
            Vector3 hsl = new Vector3(120f / 360f, 2f, 0.6f);
            float angle = MathHelper.Pi * 0.9f;
            float glowFactor = 1.3f;
            float noiseSpeed = 0.15f;
            float noiseAngle = 0.1f * MathF.Cos(Main.GlobalTimeWrappedHourly * 0.5f);
            float noiseZoom = 0.005f;
            float minimumDarknessValue = 0.3f;
            float darknessMultiplier = 1.25f;

            // Set up spriteBatch for drawing the gradient
            Main.spriteBatch.End(out SpriteBatchData spriteBatchData);
            Main.spriteBatch.Begin(spriteBatchData with { SortMode = SpriteSortMode.Immediate });


            // Set the noise
            Main.instance.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("PrettyRarities/Assets/Textures/Noise/CultistRayMap").Value;

            // Draw the glow
            if (RarityHelper.ConfigInstance.EnableGlow && (drawContext == RarityDrawContext.Tooltip || drawContext == RarityDrawContext.MouseHover)) {
                RarityHelper.SetMainRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, true, hsl, angle,
                    range, glowFactor, false, Main.GlobalTimeWrappedHourly * 0.5f, new Vector2(0.5f, 0.8f), false, true, noiseSpeed, noiseAngle, noiseZoom, minimumDarknessValue, darknessMultiplier);
                RarityHelper.DrawGlow(line, Color.White, 0);
            }

            // Draw the outline
            RarityHelper.SetRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, outerColor, false, hsl, angle, range, default, default, default, default, false);
            RarityHelper.DrawOuterTooltipText(line, outerColor, true, 6f, 3f, 6f);

            // Then draw the inner text
            RarityHelper.SetRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, false, hsl, angle, range, default, default, default, default, false);
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
            float spawnChance = 0.09f * RarityHelper.GetParticleSpawnRateMultiplier();

            if (Main.rand.NextFloat(0f, 1f) < spawnChance) {
                Vector2 textSize = line.Font.MeasureString(line.Text);
                int lifetime = 85;
                Vector2 position = new Vector2(Main.rand.NextFloat(-textSize.X * 0.5f, textSize.X * 0.5f), textSize.Y * 0.6f);
                Vector2 velocity = Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) * Main.rand.NextFloat(-0.4f, -0.3f);
                float maxScale = Main.rand.NextFloat(0.5f, 0.9f);
                float startingRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                float rotationSpeed = Main.rand.NextFloat(-0.12f, 0.12f);
                Color particleColor = Color.Lerp(new Color(51, 255, 0), new Color(0, 255, 60), Main.rand.NextFloat()) * Main.rand.NextFloat(0.5f, 0.8f);
                int fadeInTime = 50;
                int fadeOutTime = 15;
                particleList.Add(new Particle(ParticleType.BloomParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor, fadeInTime, fadeOutTime, true, frameOffset: Main.rand.Next(2)));
            }
        }
    }
}

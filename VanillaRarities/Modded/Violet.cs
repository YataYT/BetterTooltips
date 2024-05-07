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
    public static class Violet
    {
        internal static List<Particle> particleList = new();

        public static void DrawTooltipLine(RarityDrawData line, RarityDrawContext drawContext = RarityDrawContext.Tooltip) {
            // Common variables
            int darkness = 30;
            Color outerColor = new(darkness, darkness, darkness);
            float timeMult = 2f;
            float areaMult = 0.005f;
            float range = 0.24f;
            Vector3 hsl = new Vector3(265f / 360f, 0.6f, 0.6f);
            float angle = MathHelper.Pi * 0.9f;
            float glowFactor = 3f;
            float noiseSpeed = 0.05f;
            float noiseAngle = MathHelper.PiOver2;
            float noiseZoom = 0.004f;
            float minimumDarknessValue = 0.15f;
            float darknessMultiplier = 2f;

            // Set up spriteBatch for drawing the gradient
            Main.spriteBatch.End(out SpriteBatchData spriteBatchData);
            Main.spriteBatch.Begin(spriteBatchData with { SortMode = SpriteSortMode.Immediate });

            // Set the noise
            Main.instance.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("PrettyRarities/Assets/Textures/Noise/VoronoiLoop").Value;

            // Draw the glow
            if (RarityHelper.ConfigInstance.EnableGlow && (drawContext == RarityDrawContext.Tooltip || drawContext == RarityDrawContext.MouseHover)) {
                RarityHelper.SetRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, true, hsl, angle,
                    range, glowFactor, false, Main.GlobalTimeWrappedHourly * 0.5f, new Vector2(0.4f, 0.7f), false, true, noiseSpeed, noiseAngle, noiseZoom, minimumDarknessValue, darknessMultiplier);
                RarityHelper.DrawGlow(line, Color.White, 0);
            }

            range = 0.18f;
            hsl = new Vector3(265f / 360f, 1f, 0.6f);
            timeMult = 2.5f;
            angle = MathHelper.Lerp(0, MathHelper.TwoPi, Main.GlobalTimeWrappedHourly * 0.03f % 1);

            // Draw the outline
            RarityHelper.SetRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, outerColor, false, hsl, angle, range, default, default, default, default, false);
            RarityHelper.DrawOuterTooltipText(line, outerColor, true, 8f, 3f, 6f);

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
            float spawnChance = 0.07f * RarityHelper.GetParticleSpawnRateMultiplier();

            if (Main.rand.NextFloat(0f, 1f) < spawnChance) {
                Vector2 textSize = line.Font.MeasureString(line.Text);
                int lifetime = 105;
                Vector2 position = new Vector2(Main.rand.NextFloat(-textSize.X * 0.45f, textSize.X * 0.45f), textSize.Y * 0.6f);
                Vector2 velocity = Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) * Main.rand.NextFloat(-0.35f, -0.25f);
                float maxScale = Main.rand.NextFloat(0.15f, 0.25f);
                float startingRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                float rotationSpeed = Main.rand.NextFloat(-0.09f, 0.09f);
                Color particleColor = Color.Lerp(new Color(96, 45, 199), new Color(132, 45, 199), Main.rand.NextFloat()) * Main.rand.NextFloat(0.7f, 1.8f);
                int fadeInTime = 30;
                int fadeOutTime = 30;
                particleList.Add(new Particle(ParticleType.StarParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor, fadeInTime, fadeOutTime, true));
            }
        }
    }
}

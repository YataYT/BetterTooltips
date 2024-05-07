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
    public static class DarkBlue
    {
        internal static List<Particle> particleList = new();

        public static void DrawTooltipLine(RarityDrawData line, RarityDrawContext drawContext = RarityDrawContext.Tooltip) {
            // Common variables
            int darkness = 30;
            Color outerColor = new(darkness, darkness, darkness);
            float timeMult = 3f;
            float areaMult = 0.02f;
            float range = 0.12f;
            Vector3 hsl = new Vector3(222f / 360f, 1.5f, 0.6f);
            float angle = MathHelper.Pi * 0.9f;
            float glowFactor = 0.8f + 0.5f * MathF.Cos(Main.GlobalTimeWrappedHourly * 4f);
            float noiseSpeed = 0.7f;
            float noiseAngle = MathHelper.PiOver2 * 1.5f;
            float noiseZoom = 0.005f;
            float minimumDarknessValue = 0.05f;
            float darknessMultiplier = 2f;

            // Set up spriteBatch for drawing the gradient
            Main.spriteBatch.End(out SpriteBatchData spriteBatchData);
            Main.spriteBatch.Begin(spriteBatchData with { SortMode = SpriteSortMode.Immediate });

            // Set the noise
            Main.instance.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("PrettyRarities/Assets/Textures/Noise/VoidGashes").Value;

            // Draw the glow
            if (RarityHelper.ConfigInstance.EnableGlow && (drawContext == RarityDrawContext.Tooltip || drawContext == RarityDrawContext.MouseHover)) {
                RarityHelper.SetRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, true, hsl, angle,
                    range, glowFactor, true, Main.GlobalTimeWrappedHourly * 0.5f, new Vector2(0.5f, 0.8f), false, true, noiseSpeed * -0.33f, noiseAngle, noiseZoom, minimumDarknessValue, darknessMultiplier);
                RarityHelper.DrawGlow(line);
                RarityHelper.DrawGlow(line, Color.White, 2);
            }

            // Reassign some values to not blend in with glow too much, improving readability
            hsl = new Vector3(222f / 360f, 1f, 0.7f);
            timeMult = -2f;
            areaMult = 0.04f;
            range = 0.12f;

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
            float spawnChance = 0.25f * RarityHelper.GetParticleSpawnRateMultiplier();

            if (Main.rand.NextFloat(0f, 1f) < spawnChance) {
                Vector2 textSize = line.Font.MeasureString(line.Text);
                int lifetime = 30;
                Vector2 position = new Vector2(Main.rand.NextFloat(-textSize.X * 0.45f, textSize.X * 0.45f), textSize.Y * 0.6f);
                Vector2 velocity = Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) * Main.rand.NextFloat(-1.3f, -1f);
                float maxScale = Main.rand.NextFloat(0.7f, 1f);
                float startingRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                float rotationSpeed = Main.rand.NextFloat(-0.03f, 0.03f);
                Color particleColor = Color.Lerp(new Color(43, 118, 222), new Color(43, 70, 222), Main.rand.NextFloat());
                int fadeInTime = 10;
                int fadeOutTime = 10;
                particleList.Add(new Particle(ParticleType.GlowyShardParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor, fadeInTime, fadeOutTime, false, frameOffset: Main.rand.Next(3)));
            }
        }
    }
}

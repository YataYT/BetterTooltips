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
    public static class HotPink
    {
        internal static List<Particle> particleList = new();

        public static void DrawTooltipLine(RarityDrawData line, RarityDrawContext drawContext = RarityDrawContext.Tooltip) {
            // Common variables
            int darkness = 40;
            Color outerColor = new(darkness, darkness, darkness);
            float timeMult = 1.5f;
            float areaMult = 0.02f;
            float range = 0.12f;
            Vector3 hsl = new Vector3(300f / 360f, 2f, 0.6f);
            float angle = MathHelper.Pi;
            float glowFactor = 0.7f;
            float noiseSpeed = 0.05f;
            float noiseAngle = MathHelper.PiOver2 * -1f;
            float noiseZoom = 0.004f;
            float minimumDarknessValue = 0f;
            float darknessMultiplier = 2f;

            // Set up spriteBatch for drawing the gradient
            Main.spriteBatch.End(out SpriteBatchData spriteBatchData);
            Main.spriteBatch.Begin(spriteBatchData with { SortMode = SpriteSortMode.Immediate });

            // Set the noise
            Main.instance.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("PrettyRarities/Assets/Textures/Noise/HarshNoise").Value;

            // Draw the glow
            if (RarityHelper.ConfigInstance.EnableGlow && (drawContext == RarityDrawContext.Tooltip || drawContext == RarityDrawContext.MouseHover)) {
                RarityHelper.SetRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, true, hsl, angle,
                    range, glowFactor, true, Main.GlobalTimeWrappedHourly * 0.5f, new Vector2(0.5f, 0.8f), true, true, noiseSpeed, noiseAngle, noiseZoom, minimumDarknessValue, darknessMultiplier);
                RarityHelper.DrawGlow(line, Color.White, 0);
                RarityHelper.DrawGlow(line, Color.White, 2);
            }

            // Set the noise
            Main.instance.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("PrettyRarities/Assets/Textures/Noise/BurnNoise").Value;
            range = 0.12f;
            hsl = new Vector3((300f + (MathF.Sin(Main.GlobalTimeWrappedHourly * 3) * 15)) / 360f, 1.2f, 0.5f);
            areaMult = 0.04f;
            timeMult = 4.5f;
            angle = MathHelper.Lerp(0, MathHelper.TwoPi, -Main.GlobalTimeWrappedHourly * 0.03f % 1);

            // Draw the outline
            RarityHelper.SetMainRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, outerColor, false, hsl, angle, range, default, default, default, default, true);
            RarityHelper.DrawOuterTooltipText(line, outerColor, true, 8f, 3f, 6f);

            // Then draw the inner text
            RarityHelper.SetMainRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, false, hsl, angle, range, default, default, default, default, true);
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
                int lifetime = 40;
                Vector2 position = Main.rand.NextVector2FromRectangle(new((int)(-textSize.X * 0.5f), (int)(-textSize.Y * 0.5f), (int)(textSize.X * 1f), (int)(textSize.Y * 0.8f)));
                Vector2 velocity = Vector2.Zero;
                float maxScale = Main.rand.NextFloat(0.3f, 0.5f);
                float startingRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                float rotationSpeed = 0f;
                Color particleColor = Color.Lerp(new Color(175, 0, 255), new Color(255, 0, 255), Main.rand.NextFloat()) * Main.rand.NextFloat(0.4f, 0.9f);
                int fadeInTime = 10;
                int fadeOutTime = 10;
                float colorDimmer = Main.rand.NextFloat(0.3f, 0.5f);
                particleList.Add(new Particle(ParticleType.SparkParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale * 1.2f, lifetime, Color.White * colorDimmer, fadeInTime, fadeOutTime, true));
                particleList.Add(new Particle(ParticleType.SparkParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor * colorDimmer, fadeInTime, fadeOutTime, true));
            }

            float spawnChance2 = 0.03f * RarityHelper.GetParticleSpawnRateMultiplier();

            if (Main.rand.NextFloat(0f, 1f) < spawnChance2) {
                Vector2 textSize = line.Font.MeasureString(line.Text);
                int lifetime = 45;
                Vector2 position = Main.rand.NextVector2FromRectangle(new((int)(-textSize.X * 0.5f), (int)(-textSize.Y * 0.5f), (int)(textSize.X * 1f), (int)(textSize.Y * 0.8f)));
                Vector2 velocity = Vector2.Zero;
                float maxScale = Main.rand.NextFloat(0.03f, 0.08f);
                float startingRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                float rotationSpeed = 0f;
                Color particleColor = Color.Lerp(new Color(255, 0, 255), new Color(255, 0, 175), Main.rand.NextFloat());
                int fadeInTime = 10;
                int fadeOutTime = 10;
                float colorDimmer = Main.rand.NextFloat(0.3f, 0.4f);
                particleList.Add(new Particle(ParticleType.BurstParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale * 1.2f, lifetime, Color.White * colorDimmer, fadeInTime, fadeOutTime, true));
                particleList.Add(new Particle(ParticleType.BurstParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor * colorDimmer, fadeInTime, fadeOutTime, true));
            }
        }
    }
}

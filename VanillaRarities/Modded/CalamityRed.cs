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
    public static class CalamityRed
    {
        internal static List<Particle> particleList = new();

        public static void DrawTooltipLine(RarityDrawData line, RarityDrawContext drawContext = RarityDrawContext.Tooltip) {
            // Common variables
            int darkness = 30;
            Color outerColor = new(darkness, darkness, darkness);
            float timeMult = 1.5f;
            float areaMult = 0.02f;
            float range = 0.24f;
            Vector3 hsl = new Vector3(0f, 2f, 0.6f);
            float angle = MathHelper.Pi * 0.9f;
            float glowFactor = 1.7f;
            float noiseSpeed = 0.25f;
            float noiseAngle = -MathHelper.PiOver4;
            float noiseZoom = 0.008f;
            float minimumDarknessValue = 0.25f;
            float darknessMultiplier = 2f;

            // Set up spriteBatch for drawing the gradient
            Main.spriteBatch.End(out SpriteBatchData spriteBatchData);
            Main.spriteBatch.Begin(spriteBatchData with { SortMode = SpriteSortMode.Immediate });

            // Set the noise
            Main.instance.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("PrettyRarities/Assets/Textures/Noise/SmokyNoise").Value;

            // Draw the glow
            if (RarityHelper.ConfigInstance.EnableGlow && (drawContext == RarityDrawContext.Tooltip || drawContext == RarityDrawContext.MouseHover)) {
                RarityHelper.SetRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, true, hsl, angle,
                    range, glowFactor, true, Main.GlobalTimeWrappedHourly * 0.5f, new Vector2(0.25f, 0.8f), false, true, noiseSpeed, noiseAngle, noiseZoom, minimumDarknessValue, darknessMultiplier);
                RarityHelper.DrawGlow(line, Color.White, 1);
            }

            // Set the noise
            Main.instance.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("PrettyRarities/Assets/Textures/Noise/LavaNoise").Value;
            minimumDarknessValue = 0f;
            darknessMultiplier = 2f;
            range = 0.2f;
            hsl = new Vector3(0f / 360f, 1.7f, 0.5f);
            areaMult = 0.03f;
            timeMult = 3f;
            angle = MathHelper.Lerp(0, MathHelper.TwoPi, -Main.GlobalTimeWrappedHourly * 0.015f % 1);
            noiseZoom = 0.025f;
            noiseAngle = MathHelper.Pi * 0.75f;
            noiseSpeed = 0.2f;

            // Draw the outline
            RarityHelper.SetRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, outerColor, false, hsl, angle, range, default, default, default, default, false, false, noiseSpeed, noiseAngle, noiseZoom, minimumDarknessValue, darknessMultiplier);
            RarityHelper.DrawOuterTooltipText(line, outerColor, true, 6f, 3f, 6f);

            // Then draw the inner text
            RarityHelper.SetRarityGradient(new Vector2(line.X, line.Y), timeMult, areaMult, Color.White, false, hsl, angle, range, default, default, default, default, false, true, noiseSpeed, noiseAngle, noiseZoom, minimumDarknessValue, darknessMultiplier);
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
            float spawnChance = 0.2f * RarityHelper.GetParticleSpawnRateMultiplier();

            if (Main.rand.NextFloat(0f, 1f) < spawnChance) {
                Vector2 textSize = line.Font.MeasureString(line.Text);
                int lifetime = 45;
                Vector2 position = new Vector2(Main.rand.NextFloat(-textSize.X * 0.45f, textSize.X * 0.45f), textSize.Y * 0.6f);
                Vector2 velocity = Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) * Main.rand.NextFloat(-1f, -0.75f);
                float maxScale = Main.rand.NextFloat(0.8f, 1.8f);
                float startingRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                float rotationSpeed = Main.rand.NextFloat(-0.12f, 0.12f);
                Color particleColor = Color.Lerp(new Color(156, 44, 44), new Color(156, 8, 8), Main.rand.NextFloat()) * Main.rand.NextFloat(1.2f, 1.5f);
                int fadeInTime = 8;
                int fadeOutTime = 8;
                particleList.Add(new Particle(ParticleType.FireParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor, fadeInTime, fadeOutTime, false));
            }

            float spawnChance2 = 0.1f * RarityHelper.GetParticleSpawnRateMultiplier();

            if (Main.rand.NextFloat(0f, 1f) < spawnChance2) {
                Vector2 textSize = line.Font.MeasureString(line.Text);
                int lifetime = 55;
                Vector2 position = new Vector2(Main.rand.NextFloat(-textSize.X * 0.45f, textSize.X * 0.45f), textSize.Y * 0.6f);
                Vector2 velocity = Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) * Main.rand.NextFloat(-0.7f, -0.5f);
                float maxScale = Main.rand.NextFloat(0.1f, 0.15f);
                float startingRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                float rotationSpeed = 0f;
                Color particleColor = Color.Lerp(new Color(156, 44, 44), new Color(156, 44, 44), Main.rand.NextFloat()) * Main.rand.NextFloat(0.3f, 0.6f);
                int fadeInTime = 10;
                int fadeOutTime = 10;
                particleList.Add(new Particle(ParticleType.FogParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor, fadeInTime, fadeOutTime, false));
            }
        }
    }
}

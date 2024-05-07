using Microsoft.Xna.Framework;
using PrettyRarities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace PrettyRarities.Particles
{
    /// <summary>
    /// This is not a real class. Rather than toss infinite variables, I'm just going to leave them here for other places to copy and put in their own file.
    /// </summary>
    public class ParticlePresets
    {
        private void FireParticle(DrawableTooltipLine line, ref List<Particle> particleList) {

        }

        private void WindParticle(DrawableTooltipLine line, ref List<Particle> particleList) {

        }

        private void TwinkleParticle(DrawableTooltipLine line, ref List<Particle> particleList) {
            Vector2 textSize = line.Font.MeasureString(line.Text);
            int lifetime = 120;
            Vector2 position = Main.rand.NextVector2FromRectangle(new((int)(-textSize.X * 0.5f), (int)(-textSize.Y * 0.5f), (int)(textSize.X * 0.8f), (int)(textSize.Y * 0.8f)));
            Vector2 velocity = Vector2.Zero;
            float maxScale = Main.rand.NextFloat(0.3f, 1f);
            float startingRotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
            float rotationSpeed = Main.rand.NextFloat(-0.01f, 0.01f);
            Color particleColor = Color.Lerp(Color.Lerp(new Color(89, 89, 255), new Color(140, 140, 255), Main.rand.NextFloat()), Color.White, 0.5f);
            int fadeInTime = 15;
            int fadeOutTime = 15;
            particleList.Add(new Particle(ParticleType.TwinkleParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor, fadeInTime, fadeOutTime, false));
        }

        private void SparkleParticle(DrawableTooltipLine line, ref List<Particle> particleList) {
            Vector2 textSize = line.Font.MeasureString(line.Text);
            int lifetime = 120;
            Vector2 position = Main.rand.NextVector2FromRectangle(new((int)(-textSize.X * 0.5f), (int)(-textSize.Y * 0.5f), (int)(textSize.X * 1f), (int)(textSize.Y * 0.8f)));
            Vector2 velocity = Vector2.Zero;
            float maxScale = Main.rand.NextFloat(0.4f, 1f);
            float startingRotation = Main.rand.NextFloat(0, MathHelper.PiOver2);
            float rotationSpeed = Main.rand.NextFloat(0.008f, 0.02f) * (Main.rand.NextBool() ? 1 : -1);
            Color particleColor = Color.Lerp(new Color(196, 255, 196), new Color(138, 255, 138), Main.rand.NextFloat());
            int fadeInTime = 15;
            int fadeOutTime = 15;
            Rectangle? baseFrame = new(0, 10, 10, 10);
            particleList.Add(new Particle(ParticleType.SparkleParticle, position, startingRotation, r => velocity, r => rotationSpeed, maxScale, lifetime, particleColor, fadeInTime, fadeOutTime, true, textSize));
            particleList.Add(new Particle(ParticleType.SparkleParticle, position, startingRotation + MathHelper.PiOver4, r => velocity, r => rotationSpeed, maxScale * 0.65f, lifetime, particleColor, fadeInTime, fadeOutTime, true, textSize));
        }
    }
}

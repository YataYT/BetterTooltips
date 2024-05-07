using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PrettyRarities.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace PrettyRarities.Core;

public class Particle
{
    public ParticleType Type;
    public Texture2D Texture;
    public Vector2 Position;
    public Func<Particle, Vector2> Velocity;
    public float Rotation;
    public Func<Particle, float> RotationalVelocity;
    public float MaxScale;
    public int Lifetime;
    public Color ParticleColor;
    public bool OpacityFade;
    public int FadeInTime;
    public int FadeOutTime;

    public int CurrentTime;
    public float Scale;
    public Color CurrentParticleColor;

    public float TimeLeft => Lifetime - CurrentTime;
    public Vector2 TextSize;

    // Exclusively used by Code Symbol particle (can be used for other particles with numerous sprites in a sprite sheet
    public int FrameOffset;

    public Particle(ParticleType particleType, Vector2 startPos, float startRotation, Func<Particle, Vector2> velocity, Func<Particle, float> rotationalVelocity, float scale, int lifetime, Color particleColor = default, int fadeInTime = 30, int fadeOutTime = 30, bool opacityFade = false, Vector2 textSize = default, Rectangle? baseFrame = null, bool singleFrame = true, Func<Particle, Rectangle?> customFrameDrawer = default, int frameOffset = 0) {
        Type = particleType;
        FrameOffset = frameOffset;  // Goes first since the very next line uses this
        Texture = GetTexture(particleType);
        Position = startPos;
        Velocity = velocity;
        Rotation = startRotation;
        RotationalVelocity = rotationalVelocity;
        MaxScale = scale;
        Lifetime = lifetime;
        ParticleColor = particleColor;
        FadeInTime = fadeInTime;
        FadeOutTime = fadeOutTime;
        OpacityFade = opacityFade;
        TextSize = textSize;
    }

    /// <summary>
    /// Updates the particle's properties. This is called automatically.
    /// </summary>
    public void Update()
    {
        // Add the resulting velocity to the position
        Position += Velocity(this);

        // Whether to fade out by changing opacity or by scale
        if (!OpacityFade)
            CurrentParticleColor = ParticleColor;
        else
            Scale = MaxScale;

        // Fade in
        if (CurrentTime <= FadeInTime)
        {
            if (OpacityFade)
                CurrentParticleColor = ParticleColor * MathHelper.Lerp(0f, 1f, (float)CurrentTime / FadeInTime);
            else
                Scale = MathHelper.Lerp(0f, MaxScale, (float)CurrentTime / FadeInTime);
        }

        // Fade out
        if (TimeLeft <= FadeOutTime)
        {
            if (OpacityFade)
                CurrentParticleColor = ParticleColor * MathHelper.Lerp(0f, 1f, (float)TimeLeft / FadeOutTime);
            else
                Scale = MathHelper.Lerp(0f, MaxScale, (float)TimeLeft / FadeOutTime);
        }

        // Add the resulting rotational velocity to the rotation
        Rotation += RotationalVelocity(this);

        // Increment the timer
        CurrentTime++;
    }

    /// <summary>
    /// Draws the particle. This is called automatically.
    /// </summary>
    /// <param name="spriteBatch">The spritebatch.</param>
    /// <param name="drawPosition">The position to draw the particle. This is separate from the particle's position.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 drawPosition) {
        Texture2D texture = Texture;

        // Exclusive to some particles
        if (Type == ParticleType.CodeParticle) {
            int offsetFrameOffset = (int)Math.Floor(CurrentTime / ((float)Lifetime / 4));
            texture = GetTexture(ParticleType.CodeParticle, offsetFrameOffset);
        } else if (Type == ParticleType.FogParticle) {
            int offsetFrameOffset = (int)Math.Floor(CurrentTime / ((float)Lifetime / 3));
            texture = GetTexture(ParticleType.FogParticle, offsetFrameOffset);
        } else if (Type == ParticleType.BurstParticle) {
            int offsetFrameOffset = (int)Math.Floor(CurrentTime / ((float)Lifetime / 5));
            texture = GetTexture(ParticleType.BurstParticle, offsetFrameOffset);
        } else if (Type == ParticleType.SymbolParticle) {
            int offsetFrameOffset = (int)Math.Floor(CurrentTime / ((float)Lifetime / 3));
            texture = GetTexture(ParticleType.SymbolParticle, offsetFrameOffset);
        } else if (Type == ParticleType.SparkParticle) {
            int offsetFrameOffset = (int)Math.Floor(CurrentTime / ((float)Lifetime / 5));
            texture = GetTexture(ParticleType.SparkParticle, offsetFrameOffset);
        }

        CurrentParticleColor.A = 0;
        spriteBatch.Draw(texture, drawPosition, null, CurrentParticleColor, Rotation, Texture.Size() / 2f, Scale, SpriteEffects.None, 0f);
    }

    /// <summary>
    /// Get the texture for the particle type.
    /// </summary>
    /// <param name="type">Type of particle you'd like to use.</param>
    /// <param name="offsetFrameOffset">Mainly used for code symbol particle, to change the texture while it's still alive.</param>
    /// <returns>The texture to be used.</returns>
    public Texture2D GetTexture(ParticleType type, int offsetFrameOffset = 0) {
        return type switch {
            ParticleType.FireParticle => RarityTexturesRegistry.GetTexture("FireParticle"),
            ParticleType.WindParticle => RarityTexturesRegistry.GetTexture("WindParticle"),
            ParticleType.TwinkleParticle => RarityTexturesRegistry.GetTexture("TwinkleParticle"),
            ParticleType.SparkleParticle => RarityTexturesRegistry.GetTexture("SparkleParticle"),
            ParticleType.CodeParticle => RarityTexturesRegistry.GetTexture($"CodeSymbolParticle_{(FrameOffset + offsetFrameOffset) % 11}"),
            ParticleType.MusicParticle => RarityTexturesRegistry.GetTexture($"MusicNoteParticle_{FrameOffset}"),
            ParticleType.SquareParticle => RarityTexturesRegistry.GetTexture("SquareParticle"),
            ParticleType.BurstParticle => RarityTexturesRegistry.GetTexture($"BurstParticle_{(FrameOffset + offsetFrameOffset) % 5}"),
            ParticleType.BloodParticle => RarityTexturesRegistry.GetTexture("BloodParticle"),
            ParticleType.BloomParticle => RarityTexturesRegistry.GetTexture($"BloomParticle_{FrameOffset}"),
            ParticleType.FogParticle => RarityTexturesRegistry.GetTexture($"FogParticle_{(FrameOffset + offsetFrameOffset) % 16}"),
            ParticleType.SymbolParticle => RarityTexturesRegistry.GetTexture($"SymbolParticle_{(FrameOffset + offsetFrameOffset) % 9}"),
            ParticleType.SparkParticle => RarityTexturesRegistry.GetTexture($"SparkParticle_{(FrameOffset + offsetFrameOffset) % 4}"),
            ParticleType.GlowyShardParticle => RarityTexturesRegistry.GetTexture($"GlowyShardParticle_{FrameOffset % 3}"),
            ParticleType.StarParticle => RarityTexturesRegistry.GetTexture($"StarParticle"),
            _ => RarityTexturesRegistry.GetTexture("TwinkleParticle"),
        };
    }
}

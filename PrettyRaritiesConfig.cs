using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace PrettyRarities
{
    public enum ParticleSettings {
        Disabled,
        HeavilyReduced,
        Reduced,
        Normal,
        Increased,
        HeavilyIncreased
    }

    public class PrettyRaritiesConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(ParticleSettings.Normal)]
        [DrawTicks]
        public ParticleSettings ParticleSettings;

        [DefaultValue(true)]
        public bool EnableGlow;
    }
}

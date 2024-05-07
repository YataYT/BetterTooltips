using Microsoft.Xna.Framework;
using PrettyRarities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace PrettyRarities.VanillaRarities;

public static class Gray
{
    public static void DrawTooltipLine(RarityDrawData line, RarityDrawContext drawContext = RarityDrawContext.Tooltip)
    {
        // Common variables
        int darkness = 25;
        Color outerColor = new(darkness, darkness, darkness);

        // Draw gray border
        RarityHelper.DrawOuterTooltipText(line, outerColor);

        // Draw text
        RarityHelper.DrawTooltipText(line, Color.Gray * 1.1f);
    }
}

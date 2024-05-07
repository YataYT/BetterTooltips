using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using PrettyRarities.Core;
using PrettyRarities.VanillaRarities;
using PrettyRarities.VanillaRarities.Modded;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using static Humanizer.In;

namespace PrettyRarities.GlobalItems;

public class GlobalItem_Rarities : GlobalItem
{
    public override void Load() {
        // Huge thanks to S. Pladison and their BetterExpertRarity mod for the IL edits and spriteBatch utilities <3
        IL_Main.MouseTextInner += CustomDrawMouseTextInner;
        IL_Main.DrawItemTextPopups += CustomDrawItemTextPopups;
    }

    private void CustomDrawItemTextPopups(ILContext il) {
        var c = new ILCursor(il);

        // Store the exit label
        ILLabel label = null;

        // Access the first return if statement to extract exit label
        if (!c.TryGotoNext(MoveType.Before,
            i => i.MatchLdloc(1),
            i => i.MatchLdfld(typeof(PopupText).GetField("active")),
            i => i.MatchBrfalse(out label)
            ))
            return;

        // Then go to where we'll implement the method
        if (!c.TryGotoNext(MoveType.After,
            i => i.MatchLdloc(1),
            i => i.MatchLdfld(out _),
            i => i.MatchLdarg0(),
            i => i.MatchDiv(),
            i => i.MatchStloc(5)
            ))
            return;

        // Call our function
        c.Emit(OpCodes.Ldloc, 1);
        c.Emit(OpCodes.Ldloc, 2);
        c.Emit(OpCodes.Ldloc, 4);
        c.EmitDelegate(DrawPopupText);
        c.Emit(OpCodes.Brfalse, label);
    }

    private void CustomDrawMouseTextInner(ILContext il) {
        var c = new ILCursor(il);

        // Land at the line where we want to actually draw the text ourselves
        if (!c.TryGotoNext(MoveType.After, i => i.MatchLdsfld(typeof(Main).GetField("mouseTextColor"))))
            return;

        if (!c.TryGotoNext(MoveType.After, i => i.MatchConvR4()))
            return;

        if (!c.TryGotoNext(MoveType.After, i => i.MatchLdcR4(255)))
            return;

        if (!c.TryGotoNext(MoveType.After, i => i.MatchDiv()))
            return;

        if (!c.TryGotoNext(MoveType.After, i => i.MatchStloc(10)))
            return;  

        var label = c.DefineLabel();

        // Draw the text
        c.Emit(OpCodes.Ldloc, 0);   // Mouse text contents
        c.Emit(OpCodes.Ldloc, 1);   // Stored rarity
        c.Emit(OpCodes.Ldloc, 2);   // Stored player difficulty(?)
        c.Emit(OpCodes.Ldloc, 7);  // Extracted X position value
        c.Emit(OpCodes.Ldloc, 8);  // Extracted Y position value
        c.EmitDelegate(DrawMouseText);
        c.Emit(OpCodes.Brtrue, label);
        c.Emit(OpCodes.Ret);
        c.MarkLabel(label);
    }

    private bool DrawMouseText(string text, int rarityID, int diff, int x, int y) {
        if (diff > 0)
            return true;

        // Set up data for drawing text, return false (aka don't run the rest of the code) if we have a supported rarity, otherwise return true (run the rest of the code)
        RarityDrawData data = new RarityDrawData(text, x, y, FontAssets.MouseText.Value, 0f, Vector2.Zero, Vector2.One);
        switch (rarityID) {
            case ItemRarityID.Master:
                FieryRed.DrawTooltipLine(data, RarityDrawContext.MouseHover);
                return false;
            case ItemRarityID.Expert:
                Rainbow.DrawTooltipLine(data, RarityDrawContext.MouseHover);
                return false;
            case ItemRarityID.Gray:
                Gray.DrawTooltipLine(data, RarityDrawContext.MouseHover);
                return false;
            case ItemRarityID.White:
                return true;    // White is skipped because it does nothing anyways, and apparently other mods use this method for other things, such as Calamity difficulty UI, which breaks color coding
            case ItemRarityID.Blue:
                Blue.DrawTooltipLine(data, RarityDrawContext.MouseHover);
                return false;
            case ItemRarityID.Green:
                Green.DrawTooltipLine(data, RarityDrawContext.MouseHover);
                return false;
            case ItemRarityID.Orange:
                Orange.DrawTooltipLine(data, RarityDrawContext.MouseHover);
                return false;
            case ItemRarityID.LightRed:
                LightRed.DrawTooltipLine(data, RarityDrawContext.MouseHover);
                return false;
            case ItemRarityID.Pink:
                Pink.DrawTooltipLine(data, RarityDrawContext.MouseHover);
                return false;
            case ItemRarityID.LightPurple:
                LightPurple.DrawTooltipLine(data, RarityDrawContext.MouseHover);
                return false;
            case ItemRarityID.Lime:
                Lime.DrawTooltipLine(data, RarityDrawContext.MouseHover);
                return false;
            case ItemRarityID.Yellow:
                Yellow.DrawTooltipLine(data, RarityDrawContext.MouseHover);
                return false;
            case ItemRarityID.Cyan:
                Cyan.DrawTooltipLine(data, RarityDrawContext.MouseHover);
                return false;
            case ItemRarityID.Red:
                Red.DrawTooltipLine(data, RarityDrawContext.MouseHover);
                return false;
            case ItemRarityID.Purple:
                Purple.DrawTooltipLine(data, RarityDrawContext.MouseHover);
                return false;
            case ItemRarityID.Quest:
                Amber.DrawTooltipLine(data, RarityDrawContext.MouseHover);
                return false;
        }

        // Now check for modded rarities
        bool calamityEnabled = ModLoader.TryGetMod("CalamityMod", out Mod calamityMod);

        if (calamityEnabled && calamityMod.TryFind("Turquoise", out ModRarity turquoise) && rarityID == turquoise.Type) {
            Turquoise.DrawTooltipLine(data, RarityDrawContext.MouseHover);
            return false;
        } else if (calamityEnabled && calamityMod.TryFind("PureGreen", out ModRarity pureGreen) && rarityID == pureGreen.Type) {
            PureGreen.DrawTooltipLine(data, RarityDrawContext.MouseHover);
            return false;
        } else if (calamityEnabled && calamityMod.TryFind("DarkBlue", out ModRarity darkBlue) && rarityID == darkBlue.Type) {
            DarkBlue.DrawTooltipLine(data, RarityDrawContext.MouseHover);
            return false;
        } else if (calamityEnabled && calamityMod.TryFind("Violet", out ModRarity violet) && rarityID == violet.Type) {
            Violet.DrawTooltipLine(data, RarityDrawContext.MouseHover);
            return false;
        } else if (calamityEnabled && calamityMod.TryFind("HotPink", out ModRarity hotPink) && rarityID == hotPink.Type) {
            HotPink.DrawTooltipLine(data, RarityDrawContext.MouseHover);
            return false;
        } else if (calamityEnabled && calamityMod.TryFind("CalamityRed", out ModRarity calamityRed) && rarityID == calamityRed.Type) {
            CalamityRed.DrawTooltipLine(data, RarityDrawContext.MouseHover);
            return false;
        } else if (calamityEnabled && calamityMod.TryFind("DarkOrange", out ModRarity darkOrange) && rarityID == darkOrange.Type) {
            DarkOrange.DrawTooltipLine(data, RarityDrawContext.MouseHover);
            return false;
        } else if (calamityEnabled && calamityMod.TryFind("Rainbow", out ModRarity rainbow) && rarityID == rainbow.Type) {
            Rainbow.DrawTooltipLine(data, RarityDrawContext.MouseHover);
            return false;
        }

        return true;
    } 

    private bool DrawPopupText(PopupText popupText, string text, Vector2 origin) {
        float screenPosY = popupText.position.Y - Main.screenPosition.Y;

        // Inverse Y position if gravity is reversed
        if (Main.LocalPlayer.gravDir == -1f)
            screenPosY = Main.screenHeight - screenPosY;

        // Ignore all non-item pop-up texts, such as fished blood moon enemies
        if (popupText.npcNetID != 0 || popupText.notActuallyAnItem)
            return true;

        // Also ignore all items that are coins, so they can have their color
        if (popupText.coinText)
            return true;

        // Process the data
        RarityDrawData data = new RarityDrawData(text, popupText.position.X - Main.screenPosition.X + origin.X, screenPosY + origin.Y,
            FontAssets.MouseText.Value, popupText.rotation, origin, Vector2.One * popupText.scale);

        // They get their own fields, so check them first
        if (popupText.master) {
            FieryRed.DrawTooltipLine(data, RarityDrawContext.PopupText);
            return false;
        } else if (popupText.expert) {
            Rainbow.DrawTooltipLine(data, RarityDrawContext.PopupText);
            return false;
        }

        switch (popupText.rarity) {
            case ItemRarityID.Gray:
                Gray.DrawTooltipLine(data, RarityDrawContext.PopupText);
                return false;
            case ItemRarityID.White:
                White.DrawTooltipLine(data, RarityDrawContext.PopupText);
                return false;
            case ItemRarityID.Blue:
                Blue.DrawTooltipLine(data, RarityDrawContext.PopupText);
                return false;
            case ItemRarityID.Green:
                Green.DrawTooltipLine(data, RarityDrawContext.PopupText);
                return false;
            case ItemRarityID.Orange:
                Orange.DrawTooltipLine(data, RarityDrawContext.PopupText);
                return false;
            case ItemRarityID.LightRed:
                LightRed.DrawTooltipLine(data, RarityDrawContext.PopupText);
                return false;
            case ItemRarityID.Pink:
                Pink.DrawTooltipLine(data, RarityDrawContext.PopupText);
                return false;
            case ItemRarityID.LightPurple:
                LightPurple.DrawTooltipLine(data, RarityDrawContext.PopupText);
                return false;
            case ItemRarityID.Lime:
                Lime.DrawTooltipLine(data, RarityDrawContext.PopupText);
                return false;
            case ItemRarityID.Yellow:
                Yellow.DrawTooltipLine(data, RarityDrawContext.PopupText);
                return false;
            case ItemRarityID.Cyan:
                Cyan.DrawTooltipLine(data, RarityDrawContext.PopupText);
                return false;
            case ItemRarityID.Red:
                Red.DrawTooltipLine(data, RarityDrawContext.PopupText);
                return false;
            case ItemRarityID.Purple:
                Purple.DrawTooltipLine(data, RarityDrawContext.PopupText);
                return false;
            case ItemRarityID.Quest:
                Amber.DrawTooltipLine(data, RarityDrawContext.PopupText);
                return false;
        }

        // Now check for modded rarities
        bool calamityEnabled = ModLoader.TryGetMod("CalamityMod", out Mod calamityMod);

        if (calamityEnabled && calamityMod.TryFind("Turquoise", out ModRarity turquoise) && popupText.rarity == turquoise.Type) {
            Turquoise.DrawTooltipLine(data, RarityDrawContext.PopupText);
            return false;
        } else if (calamityEnabled && calamityMod.TryFind("PureGreen", out ModRarity pureGreen) && popupText.rarity == pureGreen.Type) { 
            PureGreen.DrawTooltipLine(data, RarityDrawContext.PopupText);
            return false;
        } else if (calamityEnabled && calamityMod.TryFind("DarkBlue", out ModRarity darkBlue) && popupText.rarity == darkBlue.Type) {
            DarkBlue.DrawTooltipLine(data, RarityDrawContext.PopupText);
            return false;
        } else if (calamityEnabled && calamityMod.TryFind("Violet", out ModRarity violet) && popupText.rarity == violet.Type) {
            Violet.DrawTooltipLine(data, RarityDrawContext.PopupText);
            return false;
        } else if (calamityEnabled && calamityMod.TryFind("HotPink", out ModRarity hotPink) && popupText.rarity == hotPink.Type) {
            HotPink.DrawTooltipLine(data, RarityDrawContext.PopupText);
            return false;
        } else if (calamityEnabled && calamityMod.TryFind("CalamityRed", out ModRarity calamityRed) && popupText.rarity == calamityRed.Type) {
            CalamityRed.DrawTooltipLine(data, RarityDrawContext.PopupText);
            return false;
        } else if (calamityEnabled && calamityMod.TryFind("DarkOrange", out ModRarity darkOrange) && popupText.rarity == darkOrange.Type) {
            DarkOrange.DrawTooltipLine(data, RarityDrawContext.PopupText);
            return false;
        } else if (calamityEnabled && calamityMod.TryFind("Rainbow", out ModRarity rainbow) && popupText.rarity == rainbow.Type) {
            Rainbow.DrawTooltipLine(data, RarityDrawContext.PopupText);
            return false;
        }

        return true;
    }
    public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
    {
        // If another mod overrides this method, let them do their thing.
        if (LoaderUtils.HasOverride(item.ModItem, i => i.PreDrawTooltipLine)) {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
                return false;
        }

        if (item.rare == ItemRarityID.Expert || item.expert) {
            if (line.Mod == "Terraria" && line.Name == "ItemName") {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                Rainbow.DrawTooltipLine(data);
                return false;
            }
        }

        if (item.rare == ItemRarityID.Master) {
            if (line.Mod == "Terraria" && line.Name == "ItemName") {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                FieryRed.DrawTooltipLine(data);
                return false;
            }
        }

        if (item.rare == ItemRarityID.Quest)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                Amber.DrawTooltipLine(data);
                return false;
            }
        }

        if (item.rare == ItemRarityID.Gray)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName") {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                Gray.DrawTooltipLine(data);
                return false;
            }
        }

        if (item.rare == ItemRarityID.White)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                White.DrawTooltipLine(data);
                return false;
            }
        }

        if (item.rare == ItemRarityID.Blue)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                Blue.DrawTooltipLine(data);
                return false;
            }
        }

        if (item.rare == ItemRarityID.Green)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                Green.DrawTooltipLine(data);
                return false;
            }
        }

        if (item.rare == ItemRarityID.Orange)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                Orange.DrawTooltipLine(data);
                return false;
            }
        }

        if (item.rare == ItemRarityID.LightRed)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                LightRed.DrawTooltipLine(data);
                return false;
            }
        }

        if (item.rare == ItemRarityID.Pink)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                Pink.DrawTooltipLine(data);
                return false;
            }
        }

        if (item.rare == ItemRarityID.Purple)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                Purple.DrawTooltipLine(data);
                return false;
            }
        }

        if (item.rare == ItemRarityID.Lime)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                Lime.DrawTooltipLine(data);
                return false;
            }
        }

        if (item.rare == ItemRarityID.Yellow)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName") {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);     
                Yellow.DrawTooltipLine(data);
                return false;
            }
        }

        if (item.rare == ItemRarityID.Cyan)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName") {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                Cyan.DrawTooltipLine(data);
                return false;
            }
        }

        if (item.rare == ItemRarityID.Red)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName") {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                Red.DrawTooltipLine(data);
                return false;
            }
        }

        if (item.rare == ItemRarityID.LightPurple)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName") {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                LightPurple.DrawTooltipLine(data);
                return false;
            }
        }

        // Now check for modded rarities
        if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod) && calamityMod.TryFind("Turquoise", out ModRarity turquoise) && item.rare == turquoise.Type) {
            if (line.Mod == "Terraria" && line.Name == "ItemName") {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                Turquoise.DrawTooltipLine(data);
                return false;
            }
        }

        if (ModLoader.TryGetMod("CalamityMod", out calamityMod) && calamityMod.TryFind("PureGreen", out ModRarity pureGreen) && item.rare == pureGreen.Type) {
            if (line.Mod == "Terraria" && line.Name == "ItemName") {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                PureGreen.DrawTooltipLine(data);
                return false;
            }
        }

        if (ModLoader.TryGetMod("CalamityMod", out calamityMod) && calamityMod.TryFind("DarkBlue", out ModRarity darkBlue) && item.rare == darkBlue.Type) {
            if (line.Mod == "Terraria" && line.Name == "ItemName") {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                DarkBlue.DrawTooltipLine(data);
                return false;
            }
        }

        if (ModLoader.TryGetMod("CalamityMod", out calamityMod) && calamityMod.TryFind("Violet", out ModRarity violet) && item.rare == violet.Type) {
            if (line.Mod == "Terraria" && line.Name == "ItemName") {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                Violet.DrawTooltipLine(data);
                return false;
            }
        }

        if (ModLoader.TryGetMod("CalamityMod", out calamityMod) && calamityMod.TryFind("HotPink", out ModRarity hotPink) && item.rare == hotPink.Type) {
            if (line.Mod == "Terraria" && line.Name == "ItemName") {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                HotPink.DrawTooltipLine(data);
                return false;
            }
        }

        if (ModLoader.TryGetMod("CalamityMod", out calamityMod) && calamityMod.TryFind("CalamityRed", out ModRarity calamityRed) && item.rare == calamityRed.Type) {
            if (line.Mod == "Terraria" && line.Name == "ItemName") {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                CalamityRed.DrawTooltipLine(data);
                return false;
            }
        }

        if (ModLoader.TryGetMod("CalamityMod", out calamityMod) && calamityMod.TryFind("DarkOrange", out ModRarity darkOrange) && item.rare == darkOrange.Type) {
            if (line.Mod == "Terraria" && line.Name == "ItemName") {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                DarkOrange.DrawTooltipLine(data);
                return false;
            }
        }

        if (ModLoader.TryGetMod("CalamityMod", out calamityMod) && calamityMod.TryFind("Rainbow", out ModRarity rainbow) && item.rare == rainbow.Type)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                RarityDrawData data = new RarityDrawData(line.Text, line.X, line.Y, line.Font, line.Rotation, line.Origin, line.BaseScale);
                Rainbow.DrawTooltipLine(data);
                return false;
            }
        }

        return true;
    }
}

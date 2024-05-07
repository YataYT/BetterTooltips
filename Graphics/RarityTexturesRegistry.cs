using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PrettyRarities.Graphics
{
    public class RarityTexturesRegistry : ModSystem
    {
        internal static Dictionary<string, Texture2D> TextureLookupTable;

        public static bool HasLoaded { get; private set; }

        public override void Load() {
            HasLoaded = false;

            if (Main.netMode == NetmodeID.Server)
                return;

            TextureLookupTable = new();

            foreach (string path in Mod.GetFileNames().Where(p => p.Contains("Assets/Textures"))) {
                string name = Path.GetFileNameWithoutExtension(path);
                string formattedPath = Path.Combine(Path.GetDirectoryName(path), name).Replace(@"\", @"/");

                Texture2D texture = ModContent.Request<Texture2D>($"PrettyRarities/{formattedPath}", AssetRequestMode.ImmediateLoad).Value;

                if (!TextureLookupTable.ContainsKey(name))
                    TextureLookupTable.Add(name, texture);
                else
                    Mod.Logger.Warn($"RarityTexturesRegistry loading error: A texture with name {name} has already been registered!");
            }

            HasLoaded = true;
        }

        public override void Unload() {
            Main.QueueMainThreadAction(() => {
                if (Main.netMode == NetmodeID.Server)
                    return;

                TextureLookupTable = null;
            });
        }

        /// <summary>
        /// Returns the texture from the lookup table with the same name. Returns null if it can't find anything.
        /// </summary>
        /// <param name="textureFileName">The file name of the texture.</param>
        /// <returns></returns>
        public static Texture2D GetTexture(string textureFileName) {
            if (Main.netMode == NetmodeID.Server)
                return null;

            if (TextureLookupTable.TryGetValue(textureFileName, out var texture))
                return texture;

            return null;
        }
    }
}

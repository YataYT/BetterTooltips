using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Shaders;
using Terraria;
using Terraria.ModLoader;
using Terraria.Social.WeGame;
using ReLogic.Content;
using Terraria.ID;
using System.IO;

namespace PrettyRarities.Graphics.Shaders
{
    public class ShaderRegistry : ModSystem
    {
        public const string AutoloadPassName = "AutoloadPass";

        internal static Dictionary<string, Shader> ShaderLookupTable { get; set; } = null;

        public static bool HasLoaded {  get; private set; }

        public override void Load() {
            HasLoaded = false;

            if (Main.netMode == NetmodeID.Server)
                return;

            ShaderLookupTable = new();

            // Loop through every file in the "Assets/Effects/Shaders" folder.
            foreach (string path in Mod.GetFileNames().Where(p => p.Contains("Assets/Shaders"))) {
                string name = Path.GetFileNameWithoutExtension(path);
                if (name.Contains("Compiler"))
                    continue;

                string formattedPath = Path.Combine(Path.GetDirectoryName(path), name).Replace(@"\", @"/");
                if (formattedPath.Contains("Compiler"))
                    continue;

                Effect effect = Mod.Assets.Request<Effect>(formattedPath, AssetRequestMode.ImmediateLoad).Value;

                if (!ShaderLookupTable.ContainsKey(name))
                    ShaderLookupTable.Add(name, new Shader(new(effect)));
                else
                    Mod.Logger.Warn($"ShaderManager loading error: A shader with name {name} has already been registered!");
            }

            HasLoaded = true;
        }

        public override void Unload() {
            Main.QueueMainThreadAction(() => {
                if (Main.netMode == NetmodeID.Server)
                    return;

                ShaderLookupTable = null;
            });
        }

        public static Shader GetShader(string shaderFileName) {
            if (Main.netMode is NetmodeID.Server)
                return null;

            if (ShaderLookupTable.TryGetValue(shaderFileName, out var shader))
                return shader;

            return null;
        }
    }
}

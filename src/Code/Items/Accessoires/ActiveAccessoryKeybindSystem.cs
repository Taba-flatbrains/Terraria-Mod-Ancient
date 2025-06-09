using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Ancient.src.Code.Items.Accessoires
{
    internal class ActiveAccessoryKeybindSystem : ModSystem
    {
        public static ModKeybind ActiveAccessoryKeybind { get; private set; }

        public override void Load()
        {
            // Registers a new keybind
            // We localize keybinds by adding a Mods.{ModName}.Keybind.{KeybindName} entry to our localization files. The actual text displayed to English users is in en-US.hjson
            ActiveAccessoryKeybind = KeybindLoader.RegisterKeybind(Mod, "Active Accessory", "P");
        }

        // Please see ExampleMod.cs' Unload() method for a detailed explanation of the unloading process.
        public override void Unload()
        {
            // Not required if your AssemblyLoadContext is unloading properly, but nulling out static fields can help you figure out what's keeping it loaded.
            ActiveAccessoryKeybind = null;
        }
    }
}

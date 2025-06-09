using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;

namespace Ancient.src.Code.Tiles.OceanTemple
{
    internal class OceanTempleCrystal : ModTile // texture could be better
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
            
            DustType = DustID.GemSapphire;

            AddMapEntry(new Color(100, 200, 255));

            // No item drops yet
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.0f;
            g = 0.25f;
            b = 0.5f;
        }
    }
}

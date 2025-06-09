using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace Ancient.src.Code.Walls
{
    internal class OceanTempleWall : ModWall
    {
        public override void SetStaticDefaults()
        {

            Main.wallHouse[Type] = false;

            DustType = DustID.GemSapphire;

            AddMapEntry(new Color(0, 128, 255));

            RegisterItemDrop(ItemID.GlassWall); 
        }
    }
}

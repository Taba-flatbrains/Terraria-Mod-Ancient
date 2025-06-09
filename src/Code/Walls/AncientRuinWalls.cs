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
    internal class AncientRuinWall : ModWall
    {
        public override void SetStaticDefaults()
        {

            Main.wallHouse[Type] = false;

            DustType = DustID.WoodFurniture;

            AddMapEntry(new Color(102, 51, 3));

            RegisterItemDrop(ItemID.WoodWall);
        }
    }

    internal class AncientRuinWall2 : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;

            DustType = DustID.WoodFurniture;

            AddMapEntry(new Color(102, 51, 3));

            RegisterItemDrop(ItemID.WoodWall);
        }
    }
}

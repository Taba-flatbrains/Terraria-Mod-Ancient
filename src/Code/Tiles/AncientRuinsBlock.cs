using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Tiles
{
    internal class AncientRuinsBlock : ModTile
    {
        public override void SetStaticDefaults()
        {
            DustType = DustID.WoodFurniture;
            Main.tileSolid[Type] = true;

            AddMapEntry(new Color(153, 76, 3));
            
            RegisterItemDrop(ItemID.Wood);
        }
    }
}

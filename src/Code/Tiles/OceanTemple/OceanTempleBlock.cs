using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Ionic.Zip;
using Ancient.src.Common.Systems;

namespace Ancient.src.Code.Tiles.OceanTemple
{
    internal class OceanTempleBlock : ModTile
    {
        public override void SetStaticDefaults()
        {
            DustType = DustID.WoodFurniture;
            Main.tileSolid[Type] = true;
            MinPick = 200;
            

            AddMapEntry(new Color(102, 178, 255));

            RegisterItemDrop(ItemID.Wood);
        }
        public override bool CanExplode(int i, int j)
        {
            return false;
        }
        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return Condition.DownedMoonLord.Predicate.Invoke();
        }
    }
}

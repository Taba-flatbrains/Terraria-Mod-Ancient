using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Ancient.src.Code.Walls.AstralBiome
{
    internal class AstralNodeBackgroundWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;

            DustType = DustID.GemSapphire;

            AddMapEntry(new Color(122, 210, 230));
        }
    }
}

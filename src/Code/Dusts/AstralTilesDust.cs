using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Dusts
{
    internal class AstralTilesDust : ModDust
    {
        public override void SetStaticDefaults()
        {
            UpdateType = DustID.Dirt;
        }
    }
}

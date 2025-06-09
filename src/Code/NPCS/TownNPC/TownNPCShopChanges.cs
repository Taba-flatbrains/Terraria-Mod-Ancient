using Ancient.src.Code.Items.Materials;
using Ancient.src.Common.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Ancient.src.Code.NPCS.TownNPC
{
    internal class TownNPCShopChanges : GlobalNPC
    {
        public override void SetupTravelShop(int[] shop, ref int nextSlot) // change items of traveling merchant shop
        {
            if (NPC.downedEmpressOfLight || NPC.downedMoonlord || DownedBossSystem.DownedDarknessEmbrace) 
            {
                shop[nextSlot] = ModContent.ItemType<MarvelScale>();
                nextSlot++;
            }
        }
    }
}

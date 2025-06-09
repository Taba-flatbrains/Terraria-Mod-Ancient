using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Buffs
{
    internal class NoTeleportationDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
        }
    }

    public class NoTeleportationDebuffItems : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (!player.HasBuff<NoTeleportationDebuff>())
            {
                return base.CanUseItem(item, player);
            }

            if (
                item.type == ItemID.RodofDiscord ||
                item.type == ItemID.RodOfHarmony
                
            )
            {
                return false;
            }

            return base.CanUseItem(item, player);
        }
    }
}

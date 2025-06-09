using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Common.VanillaPatches
{
    internal class ItemChanges : GlobalItem
    {
        public override void SetDefaults(Item entity)
        {
            if (entity.type == ItemID.Zenith)
            {
                entity.damage = 170;
            }

            if (entity.type == ItemID.LastPrism)
            {
                entity.damage = 95;
            }
        }
    }
}

using Ancient.src.Code.Items.Scrolls;
using Ancient.src.Code.Items.Usables.Totems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Common.Loot
{
    internal class FishingCrateLoot : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ItemID.JungleFishingCrate || item.type == ItemID.JungleFishingCrateHard)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<HealingTotem>(), 7, 1, 1));
            }
            else if (item.type == ItemID.FrozenCrate || item.type == ItemID.FrozenCrateHard)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<EyeOfSkadiScroll>(), 7, 1, 1));
            }
            else if (item.type == ItemID.LavaCrate || item.type == ItemID.LavaCrateHard)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SharangaScroll>(), 7, 1, 1));
            }
        }
    }
}

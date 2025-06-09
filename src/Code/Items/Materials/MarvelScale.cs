using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Items.Materials
{
    internal class MarvelScale : ModItem
    {
        public override void SetDefaults()
        {
            Item.material = true;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(gold: 30);
            Item.rare = ItemRarityID.Cyan;
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 59;
            Item.width = 20;
            Item.height = 20;
        }
    }
}

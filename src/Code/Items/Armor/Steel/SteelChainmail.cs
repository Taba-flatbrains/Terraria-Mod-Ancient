using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Ancient.src.Code.Items.Materials.Bars;

namespace Ancient.src.Code.Items.Armor.Steel
{

    [AutoloadEquip(EquipType.Body)]
    internal class SteelChainmail : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(silver: 40); // How many coins the item is worth
            Item.rare = ItemRarityID.Green; // The rarity of the item
            Item.defense = 4; // The amount of defense the item will give when equipped
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<SteelBar>(25)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}

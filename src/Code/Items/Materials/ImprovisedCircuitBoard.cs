using Ancient.src.Code.Items.Materials.Bars;
using Ancient.src.Code.Tiles;
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
    internal class ImprovisedCircuitBoard : ModItem
    {
        public override void SetDefaults()
        {
            Item.material = true;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(0,0,10);
            Item.rare = ItemRarityID.LightPurple;
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 59;
            Item.width = 20;
            Item.height = 20;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<SolderingTinBar>(), 1).AddIngredient(ItemID.Nanites, 1).AddIngredient(ItemID.Wire, 2)
                .AddTile(ModContent.TileType<AdvancedMechanicsWorkbenchTile>()).Register();
        }
    }
}

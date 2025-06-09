using Ancient.src.Code.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Ancient.src.Code.Items.Materials.Bars;

namespace Ancient.src.Code.Items.Accessoires
{
    internal class IronCore : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.defense = 5;
            Item.value = Item.sellPrice(0, 0, 60);
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.IronBar, 10).AddIngredient(ModContent.ItemType<SteelBar>(), 10).AddTile(ModContent.TileType<AncientAltarTile>())
                .AddOnCraftCallback(AltarCraftingCallbacks.GetOnCraftCallback(Type))
                .AddCondition(AltarCraftingConditions.IronCore).Register();
        }
    }
}

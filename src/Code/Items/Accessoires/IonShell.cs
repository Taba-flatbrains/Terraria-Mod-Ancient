using Ancient.src.Code.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Items.Accessoires
{
    internal class IonShell : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.defense = 1;
            Item.value = Item.sellPrice(0, 2);
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Obsidian, 60).AddIngredient(ItemID.DemoniteBar, 30).AddIngredient(ItemID.Amethyst, 10)
                .AddTile(ModContent.TileType<AncientAltarTile>())
                .AddOnCraftCallback(AltarCraftingCallbacks.GetOnCraftCallback(Type))
                .AddCondition(AltarCraftingConditions.IonShell).Register();
        }
    }
}

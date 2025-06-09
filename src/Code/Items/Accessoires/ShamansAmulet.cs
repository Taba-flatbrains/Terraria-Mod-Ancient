using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Ancient.src.Code.Items.Usables.Totems;

namespace Ancient.src.Code.Items.Accessoires
{
    internal class ShamansAmulet : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.value = Item.sellPrice(0, 4);
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ShamansArmuletPlayer>().active = true;
            player.maxTurrets += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Vine, 5)
                .AddRecipeGroup(RecipeGroupID.Wood, 2)
                .AddRecipeGroup("Ancient:gem", 6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    internal class ShamansArmuletPlayer : ModPlayer
    {
        public bool active = false;
        public override void ResetEffects()
        {
            active = false;
        }
        public static bool HasItem(Player player)
        {
            return player.GetModPlayer<ShamansArmuletPlayer>().active;
        }
    }
}

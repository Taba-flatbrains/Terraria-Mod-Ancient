using Ancient.src.Code.Items.Materials;
using Ancient.src.Code.Tiles.Decorative;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Common.Systems
{
    internal class Recipes : ModSystem
    {
        public override void AddRecipes()
        {
            // Poison Vanilla Recipes
            Recipe.Create(ItemID.PoisonedKnife, 100).AddIngredient(ItemID.ThrowingKnife, 100).AddIngredient(ModContent.ItemType<Poison>()).Register();
            Recipe.Create(ItemID.DartTrap).AddIngredient(ItemID.StoneBlock, 15).AddIngredient(ModContent.ItemType<Poison>(), 1)
                .AddIngredient(ItemID.WoodenArrow, 20).AddIngredient(ItemID.CopperBar, 5).AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ItemID.VialofVenom).AddIngredient(ItemID.BottledWater).AddIngredient(ModContent.ItemType<Poison>(), 5).Register();
            Recipe.Create(ItemID.FlaskofPoison).AddIngredient(ItemID.BottledWater).AddIngredient(ModContent.ItemType<Poison>()).AddTile(TileID.ImbuingStation).Register();

            // Cog from Metal Frames + Gold
            Recipe.Create(ItemID.CogWall, 4).AddIngredient(ModContent.ItemType<MetalFrame>()).AddIngredient(ItemID.GoldBar).AddTile(TileID.WorkBenches).Register();
        }
    }
}

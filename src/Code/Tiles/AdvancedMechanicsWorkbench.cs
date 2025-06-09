using Ancient.src.Code.Tiles.Decorative;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Ancient.src.Code.Tiles
{
    internal class AdvancedMechanicsWorkbench : ModItem // crafting station for bullets and rockets
    {
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Cog, 10).AddIngredient(ItemID.Wood, 20).AddIngredient(ModContent.ItemType<MetalFrame>(), 10).AddIngredient(ItemID.Nanites, 10)
                .AddTile(TileID.HeavyWorkBench).Register();
            CreateRecipe().AddIngredient(ItemID.Cog, 15).AddIngredient(ItemID.Wood, 20).AddIngredient(ModContent.ItemType<MetalFrame>(), 10).AddIngredient(ItemID.Nanites, 20)
                .AddTile(TileID.Anvils).Register();
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AdvancedMechanicsWorkbenchTile>());
            Item.width = 48;
            Item.height = 48;
            Item.value = Item.sellPrice(0, 0, 80, 0);
            Item.rare = ItemRarityID.LightPurple;
        }
    }

    internal class AdvancedMechanicsWorkbenchTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;

            DustType = DustID.Stone;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(70, 70, 70));

            RegisterItemDrop(ModContent.ItemType<AdvancedMechanicsWorkbench>());
            AnimationFrameHeight = 54;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 12) // spend 5 ticks on every animation frame
            {
                frameCounter = 0;
                frame++;
                if (frame > 1) // 2 total animation frames
                {
                    frame = 0;
                }
            }
        }
    }
}

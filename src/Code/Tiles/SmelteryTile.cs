using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ObjectData;

namespace Ancient.src.Code.Tiles
{
    internal class SmelteryTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;

            DustType = DustID.Stone;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(100, 80, 80));

            RegisterItemDrop(ModContent.ItemType<Smeltery>());
            AnimationFrameHeight = 54;
        }


        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 4) // spend 5 ticks on every animation frame
            {
                frameCounter = 0;
                frame++;
                if (frame > 4) // 5 total animation frames
                {
                    frame = 0;
                }
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.83f;
            g = 0.31f;
            b = 0.12f;
        }
    }
        

    internal class Smeltery : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SmelteryTile>());
            Item.width = 32;
            Item.height = 32;
            Item.value = Item.sellPrice(0, 0, 20, 0);
        }

        public override void SetStaticDefaults()
        {

        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.ClayBlock, 30).AddRecipeGroup("IronBar", 5).AddIngredient(ItemID.Torch, 10).AddIngredient(ItemID.StoneBlock, 20).AddTile(TileID.HeavyWorkBench).Register();
            CreateRecipe().AddIngredient(ItemID.ClayBlock, 100).AddRecipeGroup("IronBar", 6).AddIngredient(ItemID.Torch, 15).AddIngredient(ItemID.StoneBlock, 20).AddTile(TileID.WorkBenches).Register();
        }
    }
}

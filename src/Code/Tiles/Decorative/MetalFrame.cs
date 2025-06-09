using Ancient.src.Code.Dusts;
using Ancient.src.Code.Items.Materials.Bars;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Tiles.Decorative
{
    internal class MetalFrame : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MetalFrameTile>());
            Item.width = 16;
            Item.height = 16;
            Item.value = Item.buyPrice(0, 12, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.material = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).AddIngredient(ItemID.LeadBar, 1).AddIngredient(ModContent.ItemType<SteelBar>(), 1).AddTile(TileID.HeavyWorkBench).Register();
            CreateRecipe(2).AddIngredient(ItemID.LeadBar, 1).AddIngredient(ModContent.ItemType<SteelBar>(), 2).AddTile(TileID.Anvils).Register();
        }
    }

    internal class MetalFrameTile : ModTile
    {

        public override void SetStaticDefaults()
        {
            DustType = DustID.Ash;
            Main.tileSolid[Type] = true;
            AddMapEntry(new Color(50,50,50));
            MineResist = 2;
            RegisterItemDrop(ModContent.ItemType<MetalFrame>());
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }
    }
}

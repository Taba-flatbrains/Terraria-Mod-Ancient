using Ancient.src.Code.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Walls
{
    internal class LeatherWallWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;

            DustType = DustID.BorealWood_Small;

            AddMapEntry(new Color(164, 116, 33));

            RegisterItemDrop(ModContent.ItemType<LeatherWall>());
        }
    }

    internal class LeatherWall : ModItem
    {
        public override void SetDefaults() 
        {
            Item.maxStack = 9999;
            Item.DefaultToPlaceableWall(ModContent.WallType<LeatherWallWall>());
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 59;
            Item.width = 20;
            Item.height = 20;
            Item.value = 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe(4).AddIngredient(ItemID.Leather, 1).AddTile(TileID.WorkBenches).Register();
        }
    }
}

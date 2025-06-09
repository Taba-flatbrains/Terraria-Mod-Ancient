using Ancient.src.Code.Tiles;
using Ionic.Zip;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Ancient.src.Code.Items.Materials.Bars
{
    internal abstract class Bar : ModItem
    {
        public override void SetDefaults()
        {
            Item.material = true;
            Item.maxStack = 9999;
            Item.value = 250;
            Item.rare = ItemRarityID.White;
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 59;
            Item.width = 20;
            Item.height = 20;
        }
    }

    public abstract class BarTile : ModTile
    {
        public abstract Color MapColor
        {
            get;
        }

        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 1100;
            Main.tileSolid[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            AddMapEntry(MapColor, Language.GetText("MapObject.MetalBar")); // localized text for "Metal Bar"
        }
    }



    internal class BronzeBar : Bar
    {

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CopperBar, 5).AddIngredient(ItemID.TinBar, 1).AddTile(ModContent.TileType<SmelteryTile>()).Register();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<BronzeBarTile>());
            Item.value = Item.sellPrice(0, 0, 2, 0);
        }
    }

    internal class BronzeBarTile : BarTile
    {
        public override Color MapColor
        {
            get
            {
                return Color.Brown;
            }
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            RegisterItemDrop(ModContent.ItemType<BronzeBar>());
        }
    }


    // no use
    /*internal class ElectrumBar : Bar
    {
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.GoldBar, 2).AddIngredient(ItemID.SilverBar, 1).AddTile(ModContent.TileType<SmelteryTile>()).Register();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<ElectrumBarTile>());
            Item.value = Item.sellPrice(0, 0, 11, 0);
        }
    }

    internal class ElectrumBarTile : BarTile
    {
        public override Color MapColor
        {
            get
            {
                return Color.Yellow;
            }
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            RegisterItemDrop(ModContent.ItemType<ElectrumBar>());
        }
    }*/
    internal class SteelBar : Bar
    {
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.IronBar, 1).AddTile(ModContent.TileType<SmelteryTile>()).Register();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<SteelBarTile>());
            Item.value = Item.sellPrice(0, 0, 4, 0);
        }
    }


    internal class SteelBarTile : BarTile
    {
        public override Color MapColor
        {
            get
            {
                return Color.Gray;
            }
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            RegisterItemDrop(ModContent.ItemType<SteelBar>());
        }
    }



    internal class SolderingTinBar : Bar
    {
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.TinBar, 2).AddIngredient(ItemID.LeadBar, 1).AddTile(ModContent.TileType<SmelteryTile>()).Register();
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SolderingTinBarTile>());
            Item.value = Item.sellPrice(0, 0, 3, 0);
        }
    }


    internal class SolderingTinBarTile : BarTile
    {
        public override Color MapColor
        {
            get
            {
                return Color.Gray;
            }
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            RegisterItemDrop(ModContent.ItemType<SolderingTinBar>());
        }
    }



    internal class DarkSteelBarTile : BarTile
    {
        public override Color MapColor
        {
            get
            {
                return Color.Black;
            }
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            RegisterItemDrop(ModContent.ItemType<DarkSteelBar>());
        }
    }

    internal class DarkSteelBar : Bar
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.DefaultToPlaceableTile(ModContent.TileType<DarkSteelBarTile>());
        }
    }
}

using Ancient.src.Code.Items.Materials.AstralBiome;
using Ancient.src.Code.Items.Materials.Bars;
using Ancient.src.Code.Tiles;
using Ancient.src.Code.Tiles.AstralBiome;
using Ancient.src.Common.Structures;
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
    internal class GogglesOfTrueSight : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.defense = 5;
            Item.value = Item.sellPrice(0, 5, 0);
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Red;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Lens, 2).AddIngredient(ModContent.ItemType<ArcaneShard>(), 25).AddTile(TileID.MythrilAnvil).Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AstralBiomeShaderPlayer>().AstralBiomeShaderIntensityMultiplier *= 0.65f;
        }
    }
}

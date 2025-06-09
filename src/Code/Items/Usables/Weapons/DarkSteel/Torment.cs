using Ancient.src.Code.Buffs;
using Ancient.src.Code.Items.Materials.Bars;
using Ancient.src.Code.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria;

namespace Ancient.src.Code.Items.Usables.Weapons.DarkSteel
{
    internal class Torment : ModItem
    {

        public override void SetDefaults()
        {
            // This method quickly sets the whip's properties.
            // Mouse over to see its parameters.
            Item.DefaultToWhip(ModContent.ProjectileType<TormentProjectile>(), 180, 4, 10, 30);
            Item.value = Item.sellPrice(0, 20);
            Item.rare = ItemRarityID.Purple;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<DarkSteelBar>(), 12).AddTile(TileID.MythrilAnvil).Register();
        }

        // Makes the whip receive melee prefixes
        public override bool MeleePrefix()
        {
            return true;
        }
    }
}

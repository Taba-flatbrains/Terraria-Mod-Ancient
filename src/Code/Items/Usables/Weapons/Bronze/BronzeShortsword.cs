using Ancient.src.Code.Items.Materials.Bars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Ancient.src.Code.Projectiles.ShortSword;

namespace Ancient.src.Code.Items.Usables.Weapons.Bronze
{
    internal class BronzeShortSword : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 7;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 11;
            Item.useAnimation = 11;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.knockBack = 4f;
            Item.value = Item.buyPrice(silver: 16); // Buy this item for one gold - change gold to any coin and change the value to any number <= 100
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;

            Item.noUseGraphic = true; // The sword is actually a "projectile", so the item should not be visible when used
            Item.noMelee = true; // The projectile will do the damage and not the item

            Item.shoot = ModContent.ProjectileType<BronzeShortSwordProjectile>(); // The projectile is what makes a shortsword work
            Item.shootSpeed = 2.1f; // This value bleeds into the behavior of the projectile as velocity, keep that in mind when tweaking values
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BronzeBar>(8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}

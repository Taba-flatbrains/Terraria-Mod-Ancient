using Ancient.src.Code.Items.Materials.AstralBiome;
using Ancient.src.Code.Projectiles.Ammo;
using Ancient.src.Code.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Items.Ammo
{
    internal class AphoticBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.damage = 18; // The damage for projectiles isn't actually 12, it actually is the damage combined with the projectile and the item together.
            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
            Item.knockBack = 3f;
            Item.value = 6;
            Item.rare = ItemRarityID.Cyan;
            Item.shoot = ModContent.ProjectileType<AphoticBulletProjectile>(); // The projectile that weapons fire when using this item as ammunition.
            Item.shootSpeed = 6f; // The speed of the projectile. This value equivalent to Silver Bullet since ExampleBullet's Projectile.extraUpdates is 1.
            Item.ammo = AmmoID.Bullet; // The ammo class this ammo belongs to.
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe(100)
                .AddIngredient(ItemID.MusketBall, 70)
                .AddIngredient<SoulOfTwilight>()
                .AddTile<AdvancedMechanicsWorkbenchTile>()
                .Register();
        }
    }
}

using Ancient.src.Code.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Items.Usables.Weapons
{
    internal class Sharanga : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30; // The width of item hitbox
            Item.height = 30; // The height of item hitbox

            Item.autoReuse = false;  // Whether or not you can hold click to automatically use it again.
            Item.damage = 36; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.DamageType = DamageClass.Ranged; // What type of damage does this item affect?
            Item.knockBack = 2f; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            Item.noMelee = true; // So the item's animation doesn't do damage.
            Item.rare = ItemRarityID.Orange; // The color that the item's name will be in-game.
            Item.shootSpeed = 11f; // The speed of the projectile (measured in pixels per frame.)
            Item.useAnimation = 25; // The length of the item's use animation in ticks (60 ticks == 1 second.)
            Item.useTime = 25; // The item's use time in ticks (60 ticks == 1 second.)
            Item.UseSound = SoundID.Item5; // The sound that this item plays when used.
            Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, shoot, etc.)
            Item.value = Item.sellPrice(0, 2, 60); // The value of the weapon in copper coins

            // Custom ammo and shooting homing projectiles
            Item.shoot = ProjectileID.CursedArrow;
            Item.useAmmo = AmmoID.Arrow;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.MoltenFury, 2).AddIngredient(ItemID.HellstoneBar, 10)
                .AddTile(ModContent.TileType<AncientAltarTile>())
                .AddOnCraftCallback(AltarCraftingCallbacks.GetOnCraftCallback(Type))
                .AddCondition(AltarCraftingConditions.Sharanga).Register(); 
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ProjectileID.CursedArrow, damage, knockback, player.whoAmI);
            return false;
        }
    }
}

using Ancient.src.Code.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Ancient.src.Code.Projectiles.Elf.Drops;

namespace Ancient.src.Code.Items.Usables.Weapons
{
    internal class Silverstream : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30; // The width of item hitbox
            Item.height = 30; // The height of item hitbox

            Item.autoReuse = true;  // Whether or not you can hold click to automatically use it again.
            Item.damage = 140; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.DamageType = DamageClass.Ranged; // What type of damage does this item affect?
            Item.knockBack = 2f; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            Item.noMelee = true; // So the item's animation doesn't do damage.
            Item.rare = ItemRarityID.Red; // The color that the item's name will be in-game.
            Item.shootSpeed = 25f; // The speed of the projectile (measured in pixels per frame.)
            Item.useAnimation = 12; // The length of the item's use animation in ticks (60 ticks == 1 second.)
            Item.useTime = 12; // The item's use time in ticks (60 ticks == 1 second.)
            Item.UseSound = SoundID.Item5; // The sound that this item plays when used.
            Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, shoot, etc.)
            Item.value = Item.sellPrice(0, 25); // The value of the weapon in copper coins

            // Custom ammo and shooting homing projectiles
            Item.shoot = 10;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SilverstreamArrow>(), damage, knockback, player.whoAmI, 0, 0);
            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (new Random().Next(3) != 0)
            {
                return false;
            } else
            {
                return true;
            }
        }
    }
}

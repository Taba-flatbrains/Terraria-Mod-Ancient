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
using Ancient.src.Code.Items.Materials.Bars;
using Microsoft.Xna.Framework.Graphics;
using Ancient.src.Code.Projectiles;

namespace Ancient.src.Code.Items.Usables.Weapons.DarkSteel
{
    internal class Deadalus : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30; // The width of item hitbox
            Item.height = 30; // The height of item hitbox

            Item.autoReuse = true;  // Whether or not you can hold click to automatically use it again.
            Item.damage = 400; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.DamageType = DamageClass.Ranged; // What type of damage does this item affect?
            Item.knockBack = 5f; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            Item.noMelee = true; // So the item's animation doesn't do damage.
            Item.rare = ItemRarityID.Red; // The color that the item's name will be in-game.
            Item.shootSpeed = 100f; // The speed of the projectile (measured in pixels per frame.)
            Item.useAnimation = 45; // The length of the item's use animation in ticks (60 ticks == 1 second.)
            Item.useTime = 45; // The item's use time in ticks (60 ticks == 1 second.)
            Item.UseSound = SoundID.Item5; // The sound that this item plays when used.
            Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, shoot, etc.)
            Item.value = Item.sellPrice(0, 15, 0); // The value of the weapon in copper coins
            Item.crit = 25;

            // Custom ammo and shooting homing projectiles
            Item.useAmmo = AmmoID.Arrow;
            Item.shoot = ModContent.ProjectileType<DeadalusArrow>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<DarkSteelBar>(), 12).AddTile(TileID.MythrilAnvil).Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<DeadalusArrow>(), damage, knockback, player.whoAmI);
            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Ancient/src/Code/Items/Usables/Weapons/DarkSteel/DeadalusInventory").Value;
            frame = new Rectangle(0, 0, texture.Width, texture.Height);
            scale *= 1.7f;
            spriteBatch.Draw(texture,
                new Rectangle((int)position.X - (int)(frame.Width * scale * 0.5), (int)position.Y - (int)(frame.Height * scale * 0.5), (int)(frame.Width * scale), (int)(frame.Height * scale)),
                drawColor);
            return false;
        }
    }
}

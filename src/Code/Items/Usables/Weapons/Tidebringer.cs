using Ancient.src.Code.Projectiles.Desolator;
using Ancient.src.Code.Projectiles.HeroSword;
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
using Ancient.src.Code.Projectiles;
using Ancient.src.Code.Items.Materials.AstralBiome;
using Ancient.src.Code.Items.Materials;

namespace Ancient.src.Code.Items.Usables.Weapons
{
    internal class Tidebringer : ModItem
    {
        public override void AddRecipes() // only placeholder should be removed when admiral is added to pirate army which will also drop this item
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<MarvelScale>(), 10).AddIngredient(ItemID.GoldBar, 15)
                .AddTile(TileID.MythrilAnvil).Register();
        }
        public override void SetDefaults()
        {
            Item.damage = 250;
            Item.DamageType = DamageClass.Melee;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.5f;
            Item.value = Item.sellPrice(gold: 25);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<TidebringerProjectile>();
            Item.shootSpeed = 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center, velocity, ModContent.ProjectileType<TidebringerProjectile>(), Item.damage, 
                Item.knockBack * 0.4f, Owner: Main.myPlayer, 1);
            

            return false;
        }
    }
}

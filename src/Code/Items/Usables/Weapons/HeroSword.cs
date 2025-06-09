using Ancient.src.Code.Items.Materials.Bars;
using Ancient.src.Code.Projectiles.Desolator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System.Diagnostics.Metrics;
using Ancient.src.Code.Projectiles.HeroSword;

namespace Ancient.src.Code.Items.Usables.Weapons
{
    internal class HeroSword : ModItem
    {
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.BrokenHeroSword, 2).AddIngredient(ItemID.ChlorophyteBar, 10).AddTile(TileID.MythrilAnvil).Register();
        }

        public override void SetDefaults()
        {
            Item.damage = 125;
            Item.DamageType = DamageClass.Melee;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.5f;
            Item.value = Item.sellPrice(gold: 15);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        private int counter = 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            counter++;
            if (counter % 3 == 0)
            {
                Projectile.NewProjectile(source, player.Center + new Vector2(player.direction * -100, 0),
                    velocity, ModContent.ProjectileType<HeroSwordSoldier>(), Item.damage, Item.knockBack * 0.4f, Owner: Main.myPlayer);
            }

            return false;
        }

        public override void UseItemFrame(Player player)
        {
            if (new Random().Next(3) != 0) { return; }
            float offset = 0;
            if (player.direction == -1) { offset = MathF.PI; }
            Vector2 TipPosition = ((Vector2)player.HandPosition) + new Vector2(100, 0).RotatedBy(-player.direction * (player.itemTime * Math.PI / 25) + offset);
            Dust.NewDust(TipPosition, 3, 3, DustID.Terra);
        }
    }
}

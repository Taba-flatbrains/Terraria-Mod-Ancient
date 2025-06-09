using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Ancient.src.Code.Tiles;
using Ancient.src.Code.Items.Materials;
using Ancient.src.Code.Items.Materials.AstralBiome;
using Ancient.src.Code.Projectiles.OceanTemple;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Ancient.src.Code.Items.Usables.Weapons.OceanTemple
{
    internal class MarinasTome : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 105;
            Item.crit = 10;
            Item.knockBack = 5;

            Item.mana = 100;
            Item.width = 28;
            Item.height = 32;
            Item.UseSound = SoundID.Item43;
            Item.channel = true;
            Item.DamageType = DamageClass.Magic;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.shoot = ModContent.ProjectileType<WaterPulse>();
            Item.shootSpeed = 16;
            Item.noMelee = true;


            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 30);
        }

        
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.SpellTome, 1).AddIngredient(ModContent.ItemType<MarvelScale>(), 10).AddIngredient(ModContent.ItemType<SoulOfTwilight>(), 15)
                .AddTile(TileID.Bookcases).Register();
        }

        private int cd = 0;

        public override void UpdateInventory(Player player)
        {
            cd--;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            cd = 200;
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            return cd < 0;
        }
    }
}

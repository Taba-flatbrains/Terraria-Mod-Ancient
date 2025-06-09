using Ancient.src.Code.Items.Materials.Bars;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Ancient.src.Code.Projectiles.Desolator;
using System.Diagnostics.Metrics;
using Terraria.DataStructures;
using System;
using System.Collections.Generic;
using Ancient.src.Code.Buffs;

namespace Ancient.src.Code.Items.Usables.Weapons.DarkSteel
{
    internal class Desolator : ModItem
    {
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<DarkSteelBar>(), 12).AddTile(TileID.MythrilAnvil).Register();
        }

        public override void SetDefaults()
        {
            Item.damage = 220;
            Item.DamageType = DamageClass.Melee;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.5f;
            Item.value = Item.sellPrice(gold: 15); 
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item1;
            Item.crit = 25;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<DesolatorCircularSwingProjectile>();
            Item.shootSpeed = 2;
        }

        private int couter = 0;
        private List<NPC> HitNPCs = new();

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            couter++;
            if (player.whoAmI == Main.myPlayer && Main.netMode != NetmodeID.Server && couter % 2 == 0)
            {
                HitNPCs.RemoveAll(npc => !npc.HasBuff<Desolated>() || !npc.active);
                for (int i = 0; i < HitNPCs.Count; i++)
                {
                    NPC npc = HitNPCs[i];

                    Projectile.NewProjectile(source, npc.Center + new Vector2(player.direction * 10, 0),
                        velocity, ModContent.ProjectileType<DesolatorCircularSwingProjectile>(), Item.damage, Item.knockBack, Owner: Main.myPlayer);
                }
                Projectile.NewProjectile(source, player.Center + new Vector2(player.direction * 10, 0),
                    velocity, ModContent.ProjectileType<DesolatorCircularSwingProjectile>(), Item.damage, Item.knockBack, Owner: Main.myPlayer);
            }
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            HitNPCs.Add(target);
            target.AddBuff(ModContent.BuffType<Desolated>(), 60 * 15);
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage += 0.2f; // + 20% crit damage
        }
    }
}

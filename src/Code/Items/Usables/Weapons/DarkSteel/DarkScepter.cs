using Ancient.src.Code.Items.Materials.Bars;
using Ancient.src.Code.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Ancient.src.Code.Dusts;
using Terraria.Audio;

namespace Ancient.src.Code.Items.Usables.Weapons.DarkSteel
{
    internal class DarkScepter : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.mana = 14;
            Item.autoReuse = true;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 300;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.Item15;
            // Item.UseSound =

            Item.useStyle = ItemUseStyleID.HoldUp;
            //Item.shoot = ModContent.ProjectileType<DeadalusArrow>();
            //Item.shootSpeed = 10;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<DarkSteelBar>(), 12).AddTile(TileID.MythrilAnvil).Register();
        }

        public override bool? UseItem(Player player)
        {
            player.GetModPlayer<DarkScepterPlayer>().holding = true;
            player.GetModPlayer<DarkScepterPlayer>().damage = Item.damage;
            player.GetModPlayer<DarkScepterPlayer>().knockback = Item.knockBack;
            return false;
        }
    }

    public class DarkScepterPlayer : ModPlayer
    {
        public float timeHolding = 0;
        public bool holding = false;
        public int damage = 0;
        public float knockback = 0;

        public static readonly int MaxCharge = 240;
        public static readonly float ProjectileVelocity = 14;

        public override void UpdateEquips()
        {
            if (Main.netMode == NetmodeID.Server) {
                return;
            }

            Vector2 animationCenter = Player.Center + new Vector2(Player.direction * 20, -20);

            if (holding && timeHolding < MaxCharge)
            {
                holding = false;
                timeHolding += 1 * Player.GetAttackSpeed<MagicDamageClass>();

                //charging
                Lighting.AddLight(animationCenter, new Vector3(0.015f, 0, 0)*timeHolding+ new Vector3(0.3f, 0.1f, 0.1f));
                for (int i = 0; i < 6; i++)
                {
                    float rotation = ((float)i / 4 * MathHelper.TwoPi) + ((float)timeHolding / MaxCharge * MathHelper.TwoPi * 2);
                    for (int i2 = 0; i2 < 3; i2++)
                    {
                        Dust.NewDust(animationCenter + (new Vector2(MathF.Cos(rotation), MathF.Sin(rotation)) * timeHolding), 10, 10, ModContent.DustType<DarkSteelWeaponsDust2>());
                    }
                }
                   

            } else
            {
                if (timeHolding != 0)
                {
                    //shooting
                    int shots = (int)timeHolding / 5;
                    for (int i = 0; i < shots; i++)
                    {
                        float rotation = i * MathF.PI * 2 / shots;
                        SoundEngine.PlaySound(SoundID.Item20, animationCenter);
                        Projectile.NewProjectile(
                            Player.GetSource_FromAI(),
                            Player.position,
                            new Vector2(MathF.Sin(rotation), MathF.Cos(rotation)) * ProjectileVelocity,
                            ModContent.ProjectileType<DarkScepterProjectile>(),
                            damage,
                            knockback,
                            Main.myPlayer
                        );
                    }

                    timeHolding = 0;
                }
            }
        }
    }
}

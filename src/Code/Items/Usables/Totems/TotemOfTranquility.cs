using Ancient.src.Code.Buffs;
using Ancient.src.Code.Dusts;
using Ancient.src.Code.Items.Accessoires;
using Ancient.src.Code.Items.Materials;
using Ancient.src.Code.Projectiles.Totem;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.CodeDom;

namespace Ancient.src.Code.Items.Usables.Totems
{
    internal class TotemOfTranquility : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller
        }

        public override void SetDefaults()
        {
            Item.material = true;

            Item.mana = 10; // mana cost
            Item.width = 34;
            Item.height = 34;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.HoldUp; // how the player's arm moves when using the item
            Item.value = Item.sellPrice(gold: 4);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item44; // What sound should play when using the item

            // These below are needed for a minion weapon
            Item.noMelee = true; // this item doesn't do any melee damage
            Item.DamageType = DamageClass.Summon; // Makes the damage register as summon. If your item does not have any damage type, it becomes true damage (which means that damage scalars will not affect it). Be sure to have a damage type

            Item.shoot = ModContent.ProjectileType<TotemOfTranquilityProjectile>(); // This item creates the minion projectile
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (ShamansArmuletPlayer.HasItem(player))
            {
                Item.damage = 35;
            }
            else
            {
                Item.damage = 0;
            }
        }

        public override void ModifyWeaponKnockback(Player player, ref StatModifier knockback)
        {
            if (ShamansArmuletPlayer.HasItem(player))
            {
                Item.knockBack = 4;
            }
            else
            {
                Item.knockBack = 0;
            }
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position
            position = Main.MouseWorld;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            // Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 8)
                .AddIngredient(ItemID.Wood, 20)
                .AddIngredient(ItemID.SoulofMight, 1)
                .AddIngredient(ItemID.SoulofFright, 1)
                .AddIngredient(ItemID.SoulofSight, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    internal class TotemOfTranquilityProjectile : ModProjectile
    {
        Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 42;
            Projectile.tileCollide = false;

            // These below are needed for a minion weapon
            Projectile.friendly = false; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.sentry = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Owner.UpdateMaxTurrets();
        }

        private int ticks = 0;
        private int ProjectileCountMode = 1;

        private int ActiveDist => ShamansArmuletPlayer.HasItem(Owner) ? 16 * 15 : 16 * 10;

        public override void AI()
        {
            if (ticks % 8 == 0)
            {
                Projectile.frame += 1 * ProjectileCountMode;
                if (Projectile.frame == 9)
                {
                    ProjectileCountMode = -1;
                }
                if (Projectile.frame == 0)
                {
                    ProjectileCountMode = 1;
                }
            }


            ticks++;
            visualOffsetY += new Vector2(0, MathF.Sin(ticks / 60f) / 6f);

            Lighting.AddLight(Projectile.Center + visualOffsetY + new Vector2(0, 30), new Color(0.68f, 0.64f, 0.12f).ToVector3() * 1.5f);
            if (ticks % 3 == 0)
            {
                Dust.NewDust(Projectile.position + visualOffsetY + new Vector2(-3, 47), 28, 10, ModContent.DustType<TotemHoveringDust>(), SpeedY: -2, newColor: new Color(0.68f, 0.64f, 0.12f));
            }

            if (ticks % 5 == 0)
            {
                Dust.NewDust(Projectile.Center + Vector2.UnitX.RotatedBy(MathHelper.Pi / 100 * ticks) * ActiveDist, 1, 1, DustID.GoldFlame);
                Dust.NewDust(Projectile.Center + Vector2.UnitX.RotatedBy(MathHelper.Pi / 100 * ticks + MathHelper.Pi) * ActiveDist, 1, 1, DustID.GoldFlame);
            }


            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active)
                {
                    if (projectile.penetrate != -1 && projectile.hostile) //  
                    {
                        if (Vector2.Distance(Projectile.Center, projectile.Center) < ActiveDist)
                        {
                            projectile.GetGlobalProjectile<TotemOfTranquilityGlobalProjectile>().active += 1;
                        }
                    }
                }
            }
        }

        private Vector2 visualOffsetY = new Vector2(0, -5);
        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Color color = Lighting.GetColor(Projectile.Center.ToTileCoordinates());

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + visualOffsetY, new Rectangle(0, Projectile.height*Projectile.frame, Projectile.width, Projectile.height)
                , color, -Projectile.rotation, new Vector2(11, 21),
                    Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        private bool SearchForTargets(out float distanceFromTarget, out Vector2 targetCenter)
        {
            // Starting search distance
            distanceFromTarget = 500f;
            targetCenter = Projectile.position;
            bool foundTarget = false;

            // This code is required either way, used for finding a target
            for (int i = 0; i < Main.maxNPCs; i++)
            {

                NPC npc = Main.npc[i];

                if (!npc.CanBeChasedBy()) { continue; }

                float between = Vector2.Distance(npc.Center, Projectile.Center);
                bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                bool inRange = between < distanceFromTarget;
                bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright

                if (((closest && inRange) || !foundTarget) && lineOfSight)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                }

            }
            if (targetCenter == Projectile.position)
            {
                return false;
            }
            return true;
        }
    }


    public class TotemOfTranquilityGlobalProjectile : GlobalProjectile
    {
        public int active = 0;
        public bool slowed = false;
        private int ticks = 0;
        public override bool InstancePerEntity => true;

        private const float ProjectileSlow = 0.6f;

        public override void PostAI(Projectile projectile)
        {
            ticks++;
            if (ticks % (projectile.extraUpdates + 1) != 0) { return; }

            if (active >= 1)
            {
                if (!slowed) {
                    projectile.velocity *= ProjectileSlow;
                }
                slowed = true;
            } else
            {
                if (slowed)
                {
                    projectile.velocity /= ProjectileSlow;
                }
                slowed = false;
            }
            active = 0;
        }
    }
}

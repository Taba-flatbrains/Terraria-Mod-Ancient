using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Ancient.src.Code.Dusts;
using Terraria.DataStructures;

namespace Ancient.src.Code.Projectiles.OceanTemple
{
    internal class WaterPulse : ModProjectile // Marinas Tome Projectile
    {
        private const float MaxVelocity = 5f;
        private const float HitBoxRadius = 10;

        public override void SetDefaults()
        {
            Projectile.width = 20; // Hitbox width of projectile
            Projectile.height = 20; // Hitbox height of projectile
            Projectile.friendly = true; // Projectile hits enemies
            Projectile.timeLeft = 170; // Time it takes for projectile to expire
            Projectile.penetrate = -1; // Projectile pierces infinitely
            Projectile.tileCollide = false; // Projectile does not collide with tiles
            Projectile.usesLocalNPCImmunity = true; // Uses local immunity frames
            Projectile.localNPCHitCooldown = 20; // We set this to -1 to make sure the projectile doesn't hit twice
            Projectile.DamageType = DamageClass.Magic; // Projectile is a melee projectile
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }

        private int ticks = 0;
        public override void AI()
        {
            ticks++;
            Player player = Main.player[Projectile.owner];

            Vector2 dust_direction = Vector2.UnitX.RotatedByRandom(Math.PI * 2) * 6;
            Dust.NewDust(Projectile.Center - new Vector2(5, 5), 10, 10, ModContent.DustType<WaterPulseDust>(), dust_direction.X, dust_direction.Y);


            if (Projectile.owner == Main.myPlayer) {
                if (Vector2.Distance(Main.MouseWorld, Projectile.Center) > 8)
                {
                    Vector2 unitVectorTowardsMouse = Projectile.Center.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero);
                    Vector2 unitVectorFromPlayer = -Projectile.Center.DirectionTo(player.Center).SafeNormalize(Vector2.Zero);
                    Projectile.velocity = unitVectorTowardsMouse * MaxVelocity + Main.player[Projectile.owner].velocity + unitVectorFromPlayer * 2;
                } else
                {
                    Projectile.velocity = Vector2.Zero;
                }

                // spawn fragments
                if (ticks % 5 == 0)
                {
                    Vector2 direction = Projectile.Center - player.MountedCenter;
                    direction.Normalize();
                    direction *= 16;
                    direction = direction.RotatedByRandom(0.5f);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.MountedCenter, direction, ModContent.ProjectileType<WaterPulseFragments>(), 
                        Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.whoAmI);        
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
            List<Vector2> list = new List<Vector2>();
            Vector2 beam_direction = Projectile.Center - Main.player[Projectile.owner].MountedCenter;
            beam_direction.Normalize();
            beam_direction *= 2 * HitBoxRadius;
            for (int i = 0; i < Vector2.Distance(Main.player[Projectile.owner].MountedCenter, Projectile.Center)/(2 * HitBoxRadius); i++)
            {
                list.Add(Main.player[Projectile.owner].MountedCenter + (i * beam_direction));
            }
            list.Add(Projectile.Center);
            DrawLine(list);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Main.player[Projectile.owner].MountedCenter;
            Vector2 end = Projectile.Center;

            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, HitBoxRadius, ref collisionPoint);
        }

        private void DrawLine(List<Vector2> list)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = new Rectangle(0,0,20,20);
            Vector2 origin = new Vector2(10, 1);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (i == 1)
                {
                    frame.Y += 20;
                }
                if (i == list.Count - 2)
                {
                    frame.Y += 20;
                }

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.White);
                color = new Color(new Vector4(color.ToVector3(), 0.3f));
                Vector2 scale = new Vector2(1, 1);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);
                //Dust.NewDust(pos - new Vector2(10,10), 20, 20, ModContent.DustType<WaterPulseDust>());

                pos += diff;
            }
        }
    }

    internal class WaterPulseFragments : ModProjectile
    {
        private const float MaxVelocity = 13f;
        public override void SetDefaults()
        {
            Projectile.width = 12; // Hitbox width of projectile
            Projectile.height = 12; // Hitbox height of projectile
            Projectile.friendly = true; // Projectile hits enemies
            Projectile.timeLeft = 90; // Time it takes for projectile to expire
            Projectile.penetrate = -1; // Projectile pierces infinitely
            Projectile.tileCollide = false; // Projectile does not collide with tiles
            Projectile.usesLocalNPCImmunity = true; // Uses local immunity frames
            Projectile.localNPCHitCooldown = 12; // We set this to -1 to make sure the projectile doesn't hit twice
            Projectile.DamageType = DamageClass.Magic; // Projectile is a melee projectile
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

        private int ticks = 0;

        private Vector2 PulsePosition => Main.projectile[(int)Projectile.ai[0]].Center;
        private bool StoppedTurning = false;
        private Vector2 offset = Vector2.Zero;
        private int length = 4;

        public override void OnSpawn(IEntitySource source)
        {
            offset = Projectile.velocity * 2;
            length = new Random().Next(1, 4);
        }

        public override void AI()
        {
            ticks++;
            Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.3f, 0.7f));

            if (Projectile.owner == Main.myPlayer)
            {
                Player player = Main.player[Projectile.owner];
                Vector2 unitVectorTowardsMouse = Projectile.Center.DirectionTo(PulsePosition+offset).SafeNormalize(Vector2.Zero);

                if (ticks > 3 && ticks < 35 && Vector2.Distance(Projectile.Center, PulsePosition + offset) > 33 && !StoppedTurning)
                {
                    Projectile.velocity = unitVectorTowardsMouse * MaxVelocity + Main.player[Projectile.owner].velocity;
                } else if (ticks > 5)
                {
                    StoppedTurning = true;
                }

                if (Vector2.Distance(Projectile.Center, player.Center) > Vector2.Distance(player.Center, PulsePosition + offset) + 20)
                {
                    Projectile.timeLeft = 0;
                    Projectile.active = false;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (ticks % 7 == 0)
            {
                Dust.NewDust(Projectile.position, 10, 10, ModContent.DustType<WaterPulseDust>());
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            for (int k = 0; k < Math.Min(Projectile.oldPos.Length, length); k++)
            {
                Vector2 pos = Projectile.oldPos[k];
                Color color = Lighting.GetColor(pos.ToTileCoordinates().X, pos.ToTileCoordinates().Y);
                color = new Color(new Vector4(color.ToVector3(), 0.8f - (0.2f * k)));
                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, null, color, Projectile.rotation, Vector2.Zero, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}

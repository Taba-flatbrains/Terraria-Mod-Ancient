using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Ancient.src.Common.Util;

namespace Ancient.src.Code.Projectiles.Boss.YrimirsSoul
{
    internal class HagethornRay : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 60; // The width of projectile hitbox
            Projectile.height = 60; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 60 * 15; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.alpha = 30;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.position, origin, 5f * Projectile.scale, ref collisionPoint);
        }

        private int ticks = 0;

        Vector2 origin = Vector2.Zero;
        Vector2 direction = Vector2.Zero;
        public override void AI()
        {
            if (ticks == 0)
            {
                origin = Projectile.position;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Pi;
                direction = Projectile.velocity;
                direction.Normalize();
            }
            ticks += 1;
            if (ticks > 90)
            {
                Projectile.velocity = Vector2.Zero;
            }
            Vector2 shift = new Vector2(new Random().NextSingle() - 0.5f, new Random().NextSingle() - 0.5f) * 0.5f;
            Projectile.position += shift;
            origin += shift;
        }

        public override bool PreDraw(ref Color lightColor) // trail
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new Vector2(30, 30);
            float talpha = Projectile.alpha / 255f;


            Color color = Lighting.GetColor(origin.ToTileCoordinates().X, origin.ToTileCoordinates().Y);
            color = new Color(new Vector4(color.ToVector3(), talpha));
            Main.EntitySpriteDraw(texture, origin - Main.screenPosition, new Rectangle(0, 0, 60, 60), color, Projectile.rotation + MathHelper.Pi, drawOrigin, 
                1, SpriteEffects.None, 0); // start point

            for (int k = 0; k < (Vector2.Distance(Projectile.position, origin) - 40) / 85f; k++)
            {
                float addRot = 0;
                if (new Random().NextSingle() < 0.1f)
                {
                    addRot = MathHelper.Pi;
                }

                Vector2 drawPos = (k + 0.5f) * direction * 85 + origin;
                color = Lighting.GetColor(drawPos.ToTileCoordinates().X, drawPos.ToTileCoordinates().Y);
                color = new Color(new Vector4(color.ToVector3(), talpha));
                if (new Random().Next(20) == 0) { Dust.NewDust(drawPos, 10, 10, DustID.Frost); }
                Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, new Rectangle(0, 60, 60, 60), color, Projectile.rotation + addRot, drawOrigin, 1, SpriteEffects.None, 0);
            }

            color = Lighting.GetColor(Projectile.position.ToTileCoordinates().X, Projectile.position.ToTileCoordinates().Y);
            color = new Color(new Vector4(color.ToVector3(), talpha));
            Main.EntitySpriteDraw(texture, Projectile.position - Main.screenPosition, new Rectangle(0, 0, 60, 60), color, Projectile.rotation + MathHelper.Pi, drawOrigin,
                1, SpriteEffects.None, 0); // end point

            return false;
        }
    }
}

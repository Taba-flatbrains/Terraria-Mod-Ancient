using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Projectiles
{
    internal class GlisteningWraith : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24; // The width of projectile hitbox
            Projectile.height = 24; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Default; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.alpha = 100;
            Projectile.scale = 0.9f;
        }

        private const int TrailCacheLength = 4;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = TrailCacheLength; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }


        private NPC target => Main.npc[(int)Projectile.ai[0]];

        public override void AI()
        {
            if (target.active == false) { Projectile.active = false; return; };

            Vector2 delta_vel = target.Center - Projectile.Center;
            delta_vel.Normalize();
            Projectile.velocity += delta_vel * 0.25f;
            Projectile.velocity *= 0.99f;

            Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.2f, 0.4f));

            if (new Random().Next(5) == 0)
                Dust.NewDust(Projectile.position, 22, 22, DustID.BlueCrystalShard);
        }

        public override bool PreDraw(ref Color lightColor) // trail
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new Vector2(12, 12);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Lighting.GetColor(Projectile.Center.ToTileCoordinates().X, Projectile.Center.ToTileCoordinates().Y);
                color = new Color(new Vector4(color.ToVector3(), Projectile.alpha));
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle(0, 24, 24, 24), color, Projectile.rotation + new Random().Next(-10, 11), drawOrigin, Projectile.scale - (0.1f * k), SpriteEffects.None, 0);
            }

            return true;
        }
    }
}

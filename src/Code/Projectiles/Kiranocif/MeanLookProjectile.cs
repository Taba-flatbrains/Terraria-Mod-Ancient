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

namespace Ancient.src.Code.Projectiles.Kiranocif
{
    internal class MeanLookProjectile : ModProjectile
    {
        private Player target => Main.player[(int)Projectile.ai[0]];

        public override void SetDefaults()
        {
            Projectile.width = 20; // The width of projectile hitbox
            Projectile.height = 20; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.penetrate = 2; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 240; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

        public override void AI()
        {
            Vector2 direction = target.Center - Projectile.position;
            direction.Normalize();
            Projectile.velocity += direction * 0.3f;
            if (Projectile.velocity.Length() > 16f)
            {
                Projectile.velocity *= 0.95f;
            }

            Projectile.rotation += 0.04f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (target.HasBuff(BuffID.Stoned)) { return; }
            target.AddBuff(BuffID.Stoned, 140);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new Vector2(10, 10);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Lighting.GetColor(Projectile.Center.ToTileCoordinates().X, Projectile.Center.ToTileCoordinates().Y);
                color = new Color(new Vector4(color.ToVector3(), Projectile.alpha));
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle(0, 20, 20, 20), color, Projectile.rotation + new Random().Next(-10, 11), drawOrigin, Projectile.scale - (0.1f * k), SpriteEffects.None, 0);
            }

            return false;
        }
    }
}

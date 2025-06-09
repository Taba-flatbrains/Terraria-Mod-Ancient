using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Ancient.src.Code.Buffs;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace Ancient.src.Code.Projectiles.Boss.YrimirsSoul
{
    internal class SearingDroplet : ModProjectile
    {
        private const int TrailCacheLength = 5;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = TrailCacheLength; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 16; // The width of projectile hitbox
            Projectile.height = 16; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.penetrate = 1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 60 * 7; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.alpha = 200;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new Vector2(8, 8);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                int n = 1; // the drawn frame
                if (k == 0)
                {
                    n = 0;
                }
                if (k > Projectile.oldPos.Length / 2)
                {
                    n = 2;
                }
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Lighting.GetColor(Projectile.Center.ToTileCoordinates().X, Projectile.Center.ToTileCoordinates().Y);
                color = new Color(new Vector4(color.ToVector3(), 0.8f));
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle(0, 16 * n, 16, 16), color, Projectile.rotation + new Random().Next(-10, 11), drawOrigin, Projectile.scale - (0.05f * k), SpriteEffects.None, 0);
            }
            return true;
        }

        public Player target => Main.player[(int)Projectile.ai[0]];
        public override void AI()
        {
            if (target == null) { return; }
            if (!target.active || target.dead) { return; }

            Vector2 delta_vel = target.Center - Projectile.Center;
            delta_vel.Normalize();
            Projectile.velocity += delta_vel * 0.25f;
            Projectile.velocity *= 0.99f;

            Lighting.AddLight(Projectile.Center, new Vector3(0.5f, 0.5f, 0.5f));
            if (Projectile.timeLeft % 5 == 0)
            {
                Dust.NewDust(Projectile.position, 16, 16, DustID.InfernoFork);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<MoltenBuff>(), 60 * 10);
        }
    }
}

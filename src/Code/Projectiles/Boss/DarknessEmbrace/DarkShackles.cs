using Ancient.src.Code.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Light;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Projectiles.Boss.DarknessEmbrace
{
    internal class DarkShackles : ModProjectile
    {
        private static int TrailCacheLength = 20;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = TrailCacheLength; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 12; // The width of projectile hitbox
            Projectile.height = 12; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Magic; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 180; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            // Main.projFrames[Type] = 1;

            for (int k = 0; k < TrailCacheLength; k++)
            {
                offsets[k] = Vector2.Zero;
            }
        }

        public Vector2 StartPos = new();
        public override void OnSpawn(IEntitySource source)
        {
            StartPos = Projectile.position;
            Vector2 offset = new();
            for (int k = 0; k < ProjectileID.Sets.TrailCacheLength[Projectile.type]; k++)
            {
                offset += new Vector2(new Random().Next(-1, 2), new Random().Next(-1, 2));
                offsets[k] = offset;
            }
        }
        private Vector2[] offsets = new Vector2[TrailCacheLength];
        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            Vector2 offset = new();
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                offset += new Vector2((new Random().NextSingle()-0.5f)*0.3f, (new Random().NextSingle() - 0.5f) * 0.3f);
                offsets[k] += offset;
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Lighting.GetColor(Projectile.Center.ToTileCoordinates().X, Projectile.Center.ToTileCoordinates().Y);
                Main.EntitySpriteDraw(texture, drawPos + offsets[k], null, color, Projectile.rotation + new Random().Next(-10, 11), drawOrigin, Projectile.scale, SpriteEffects.None, 0);
                
            }

            return false;
        }

        public override void AI()
        {
            Vector2 drawOrigin = new Vector2(Projectile.width * 0.5f, Projectile.height * 0.5f);
            int k = new Random().Next(TrailCacheLength);
            Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
            Dust.NewDust(drawPos + offsets[k], 1, 1, DustID.Corruption);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<DarkShacklesBuff>(), 180);
        }
    }
}

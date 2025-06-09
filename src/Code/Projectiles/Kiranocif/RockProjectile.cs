using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Ancient.src.Code.Dusts;

namespace Ancient.src.Code.Projectiles.Kiranocif
{
    internal class RockProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 36; // The width of projectile hitbox
            Projectile.height = 36; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.penetrate = 2; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = new Random().Next(4);
            Projectile.rotation = new Random().NextSingle() * MathF.PI;
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Projectile.rotation += 0.02f * MathF.Sin(Projectile.whoAmI);
                if (Projectile.timeLeft % 50 == 0)
                {
                    Dust.NewDust(Projectile.Center + new Vector2(40, 0).RotatedByRandom(MathHelper.TwoPi) + new Vector2(-5 , -5), 5, 5, ModContent.DustType<RockDust>());
                }
            }
        }
    }
}

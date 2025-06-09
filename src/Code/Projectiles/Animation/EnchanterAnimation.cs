using System;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace Ancient.src.Code.Projectiles.Animation
{
    internal class EnchanterAnimation : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 0; // The width of projectile hitbox
            Projectile.height = 0; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 25; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
        }

        public override void AI()
        {
            if (Main.netMode == NetmodeID.Server) { return; } // Animation only

            if (new Random().Next(4) == 0)
            {
                Dust.NewDust(Projectile.position - new Vector2(10, 10), 20, 20, DustID.GemSapphire);
            }
        }
    }
}

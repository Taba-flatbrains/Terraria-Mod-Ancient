using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Ancient.src.Code.Projectiles.Totem
{
    internal class FishProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32; // The width of projectile hitbox
            Projectile.height = 32; // The height of projectile hitbox
            //Projectile.aiStyle = ProjAIStyleID.; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Summon; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 6000; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.scale = 0.9f;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = new Random().Next(3);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + (MathHelper.PiOver4 * Projectile.spriteDirection);
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;
            }
            Projectile.velocity.Y += 0.05f;

            if (Projectile.velocity.X < 0)
            {
                Projectile.spriteDirection = -1;
            }
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Ancient.src.Code.Projectiles.Elf
{
    internal class DarkElfRangerBowProj : ModProjectile
    {
        private NPC Owner => Main.npc[(int)Projectile.ai[0]];

        public override void SetDefaults()
        {
            Projectile.width = 18; // The width of projectile hitbox
            Projectile.height = 36; // The height of projectile hitbox
            //Projectile.aiStyle = ProjAIStyleID.; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 25; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
        }

        public override void OnSpawn(IEntitySource source)
        {
            //Projectile.spriteDirection = Owner.spriteDirection * -1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity = Vector2.Zero;
            Projectile.Center = Owner.Center + new Vector2(Owner.spriteDirection * 10 * (0.5f + MathF.Abs(MathF.Cos(Projectile.rotation))), -1 + 6 * MathF.Sin(Projectile.rotation));
        }

        public override void AI()
        {
            Projectile.position += Owner.velocity;
        }
    }
}

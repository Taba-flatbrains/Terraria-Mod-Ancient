using Ancient.src.Code.Buffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Projectiles.Elf
{
    // meteor projectile from starcaller
    internal class Dschotrom : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 40; // The width of projectile hitbox
            Projectile.height = 40; // The height of projectile hitbox
            //Projectile.aiStyle = ProjAIStyleID.; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = new Random().NextSingle() * MathF.PI;
        }

        public override void AI()
        {
            Dust.NewDust(Projectile.position, 40, 40, DustID.Torch);
            Projectile.velocity.Y += 0.2f;
            Projectile.velocity.Y *= 0.97f;
            Projectile.rotation += 0.08f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<MoreGravityBuff>(), 5 * 60);
        }
    }
}

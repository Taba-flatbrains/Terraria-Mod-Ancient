using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Ancient.src.Code.Projectiles.Elf
{
    // High Elf Sorcerer Attack
    internal class ArcaneFlames : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 18; // The width of projectile hitbox
            Projectile.height = 18; // The height of projectile hitbox
            //Projectile.aiStyle = ProjAIStyleID.; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Magic; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 60 * 7; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.alpha = 122;
        }

        public override void PostDraw(Color lightColor)
        {
            Dust.NewDust(Projectile.position, 18, 18, DustID.Torch);
        }

        public Player target;
        public override void AI()
        {
            if (target == null) 
            {
                target = GetNearestPlayer(Projectile);
                if (target == null)
                {
                    Projectile.active = false;
                    return;
                }
            }

            if (target == null) { return; }
            if (!target.active || target.dead) 
            {
                target = GetNearestPlayer(Projectile);
            }
            if (target == null) { return; }
            if (!target.active || target.dead) { return; }

            Vector2 delta_vel = target.Center - Projectile.Center;
            delta_vel.Normalize();
            Projectile.velocity += delta_vel * 0.15f;
            Projectile.velocity *= 0.99f;

            Lighting.AddLight(Projectile.Center, new Vector3(0.5f, 0.5f, 0.5f));
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 60 * 5);
        }

        private Player GetNearestPlayer(Projectile npc)  // made by chat gpt
        {
            Player nearestPlayer = null;
            float shortestDistance = float.MaxValue;

            // Iterate through players and find the nearest one
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];

                // Skip non-active players
                if (player.active && !player.dead)
                {
                    // Calculate the distance between the NPC and the player
                    float distance = Vector2.Distance(npc.Center, player.Center);

                    // Check if the current player is closer than the previous nearest player
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        nearestPlayer = player;
                    }
                }
            }
            return nearestPlayer;
        }
    }
}

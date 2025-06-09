using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Projectiles.Boss.DarknessEmbrace
{
    internal class VoidBolts : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32; // The width of projectile hitbox
            Projectile.height = 32; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Magic; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.alpha = 50;
            Main.projFrames[Type] = 4;
        }


        private Player TargetedPlayer;
        public static int MaxVelocity = 5;
        private int tick = 0;
        private bool ChangedDirection = false;

        private int frameCounter = 0;
        private int frame = 0;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.netUpdate = true;
            TargetedPlayer = GetNearestPlayer(Projectile);
        }
        public override void AI()
        {
            Projectile.rotation += 2;
            Lighting.AddLight(Projectile.Center, new Vector3(0.4f, 0.05f, 0.5f));
            tick++;
            if (tick > 200 && !ChangedDirection)
            {
                ChangedDirection = true;
                Vector2 direcion = TargetedPlayer.Center-Projectile.Center;
                direcion.Normalize();
                Projectile.velocity = direcion * MaxVelocity;
            }

            frameCounter++;
            if (frameCounter > 5) 
            {
                frameCounter = 0;
                frame++;
                if (frame >= Main.projFrames[Type])
                {
                    frame = 0;
                }
                Projectile.frame = frame;
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            TargetedPlayer = GetNearestPlayer(Projectile);
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
                if (player.active)
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

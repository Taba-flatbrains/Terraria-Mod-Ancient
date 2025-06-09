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
    internal class GhastlyDagger : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 26; // The width of projectile hitbox
            Projectile.height = 10; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Ranged; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            // Main.projFrames[Type] = 1;
        }

        private int ticks = 0;
        private NPC Boss;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.netUpdate = true;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].type == ModContent.NPCType<NPCS.Boss.DarknessEmbrace.DarknessEmbrace>())
                {
                    Boss = Main.npc[i];
                    return;
                }
            }
            Projectile.active = false; // if boss is downed and this projectile is still alive
        }
        public override void AI()
        {
            ticks++;
            if (ticks > 150)
            {
                Vector2 direction = Boss.Center - Projectile.position;
                if (direction.Length() < 30)
                {
                    Projectile.active = false;
                }
                direction.Normalize();
                Projectile.velocity = direction * 15;
                Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) - MathHelper.Pi;
            }
            else
            {
                Projectile.rotation += 0.3f;
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].type == ModContent.NPCType<NPCS.Boss.DarknessEmbrace.DarknessEmbrace>())
                {
                    Boss = Main.npc[i];
                    return;
                }
            }
            Projectile.active = false; // if boss is downed and this projectile is still alive
        }
    }
}

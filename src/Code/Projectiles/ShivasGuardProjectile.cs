using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Ancient.src.Code.Items.Accessoires;

namespace Ancient.src.Code.Projectiles
{
    internal class ShivasGuardProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18; // The width of projectile hitbox
            Projectile.height = 18; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Magic; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 210; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.alpha = 122;
        }

        public Vector2 rotation_center;

        public override void OnSpawn(IEntitySource source)
        {
            rotation_center = Projectile.position;
        }

        public int projectile_number = 0;
        public float dist_from_center = 20;
        public int tick = 0;

        public static int TicksInCircle = 79; 
        public override void AI()
        {
            Lighting.AddLight(rotation_center, new Vector3(0, 0.05f, 0.5f));
            Lighting.AddLight(Projectile.Center, new Vector3(0, 0.05f, 0.5f));
            tick++;
            dist_from_center += 1;
            float e_tick = projectile_number * TicksInCircle / ShivasGuardPlayer.projectile_count / 2 + tick;
            float rotation = e_tick / TicksInCircle * 180;
            Projectile.rotation = rotation;
            Projectile.position = rotation_center + (new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * dist_from_center);
            if (new Random().Next(10)==0)
            {
                Dust.NewDust(Projectile.position, 18, 18, DustID.Ice);
                Projectile.alpha += 3;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, 5);
        }
    }
}

using Ancient.src.Common.Events;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Projectiles.Elf
{
    // Attack of High Elf Priest
    internal class PurifyingLight : ModProjectile
    {
        private const int ShouldHeal = 1000;
        public override void SetDefaults()
        {
            Projectile.width = 0; // The width of projectile hitbox
            Projectile.height = 0; // The height of projectile hitbox
            //Projectile.aiStyle = ProjAIStyleID.; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Magic; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 1; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Vector2.Distance(targetHitbox.Center.ToVector2(), Projectile.Center) < 3 * 16;
        }

        public override void AI()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) { return; }
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (ElfInvasion.Elves.Contains(Main.npc[i].type) && Vector2.Distance(Main.npc[i].Center, Projectile.Center) < 3 * 16)
                {
                    int AmountHealed = 0;
                    int MaxHeal = Main.npc[i].lifeMax - Main.npc[i].life;
                    int RealShouldHeal = ShouldHeal;
                    if (Main.expertMode) { RealShouldHeal *= 2; }
                    if (Main.masterMode) { RealShouldHeal *= 2; }
                    AmountHealed = Math.Min(RealShouldHeal, MaxHeal);
                    if (AmountHealed > 0)
                    {
                        Main.npc[i].life += AmountHealed; 
                        Main.npc[i].netUpdate = true;
                        Main.npc[i].HealEffect(AmountHealed);
                    }
                }
            }
        }
    }
}

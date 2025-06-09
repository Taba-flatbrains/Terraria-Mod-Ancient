using Ancient.src.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Ancient.src.Code.Projectiles.Elf;

namespace Ancient.src.Code.NPCS.Invasion.Elf
{
    internal class DarkElfRanger : ModNPC
    {
        public override void OnKill()
        {
            ElfInvasion.currentKillCount += 1;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 21;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                // Influences how the NPC looks in the Bestiary
                Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 28;
            NPC.height = 42;
            NPC.damage = 70;
            NPC.defense = 30;
            NPC.lifeMax = 10000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.PlayerKilled;
            NPC.value = 3000f;

            NPC.aiStyle = NPCAIStyleID.Fighter; // custom ai
                                                //Banner = Item.NPCtoBanner(NPCID.Zombie); // <- to be replaced with own banner
                                                //BannerItem = Item.BannerToItem(Banner); 
            NPC.knockBackResist = 0.3f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (ElfInvasion.Ongoing)
            {
                return 1;
            }
            return 0;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Shhh"),
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5 && !Attacking) // Adjust the frame speed
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
                if (NPC.frame.Y >= 14 * frameHeight)
                {
                    NPC.frame.Y = frameHeight;
                }
            }
            if (Attacking)
            {
                NPC.frame.Y = 14 * frameHeight;
            }
        }


        private bool Attacking = false;
        private int ticks = 0;
        public override bool PreAI()
        {
            ticks += 1;

            if (!Attacking) // not attacking
            {
                if (ticks >= 60 * 5)
                {
                    Attacking = true;
                    NPC.TargetClosest();
                    ticks = 0;
                }
                return true;
            }

            NPC.velocity.X *= 0.9f;
            if ((double)NPC.velocity.X > -0.1 && (double)NPC.velocity.X < 0.1)
                NPC.velocity.X = 0f;
            // attacking
            if (ticks >= 30)
            {
                Attacking = false;
                ticks = 0;
            }
            Attack();
            return false;
        }

        private void Attack()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) { return; }

            if (NPC.target == 255) { return; }

            if (Vector2.Distance(NPC.position, Main.player[NPC.target].position) > 16 * 100) { return; }

            if (ticks == 5)  // take out bow
            {
                Vector2 proj_v = Main.player[NPC.target].Center - NPC.Center;
                proj_v.Normalize();
                proj_v.RotatedBy(0.5 * NPC.direction);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, proj_v * 8, ModContent.ProjectileType<DarkElfRangerBowProj>(), 0, 0, ai0: NPC.whoAmI);
            }
            if (ticks == 15)  // shoot
            {
                Vector2 proj_v = Main.player[NPC.target].Center - NPC.Center;
                proj_v.Normalize();
                proj_v.RotatedByRandom(0.1f);
                proj_v.RotatedBy(0.5 * NPC.direction);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, proj_v * 13, ProjectileID.WoodenArrowHostile, 80, 2f);
            }
        }

        public override void AI()
        {
            if (ElfInvasion.Ongoing)
            {
                NPC.timeLeft = 2;
                NPC.despawnEncouraged = false;
            }
        }

        public override void PostAI()
        {
            if (ElfInvasion.Ongoing)
            {
                NPC.timeLeft = 2;
                NPC.despawnEncouraged = false;
            }
        }
    }
}

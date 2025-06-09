using Ancient.src.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Ancient.src.Code.Walls;
using Ancient.src.Common.Structures;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ancient.src.Code.Projectiles.Elf;
using Terraria.Audio;

namespace Ancient.src.Code.NPCS.Invasion.Elf
{
    internal class HighElfSorcerer : ModNPC
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
				new FlavorTextBestiaryInfoElement("Old and wise. His talent is unmatched."),
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
                if (ticks < 5)
                {
                    NPC.frame.Y = 14 * frameHeight;
                } else if (ticks < 10) 
                {
                    NPC.frame.Y = 15 * frameHeight;
                }
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
            if (ticks == 10)
            {
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
            }
            if ((ticks == 10 || ticks == 20) && NPC.target != 255 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (new Random().Next(10) == 0)
                {
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + 2 * 16, (int)NPC.position.Y, ModContent.NPCType<FlaringWyvern>());
                } else
                {
                    Vector2 proj_v = Main.player[NPC.target].Center - NPC.Center;
                    proj_v.Normalize();
                    proj_v.RotatedByRandom(0.3f);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, proj_v * 8, ModContent.ProjectileType<ArcaneFlames>(), 70, 2f);
                }
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

using Ancient.src.Code.Projectiles.Elf;
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
using Ancient.src.Code.Dusts;
using Terraria.Audio;

namespace Ancient.src.Code.NPCS.Invasion.Elf
{
    internal class DarkElfWarlock : ModNPC
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
            NPC.damage = 50;
            NPC.defense = 40;
            NPC.lifeMax = 12000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath5;
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
			new FlavorTextBestiaryInfoElement("Nobody knows what lies underneath the cloak."),
        });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frameCounter++;
            if (NPC.frameCounter >= 10 && !Attacking) // Adjust the frame speed
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
                if (NPC.frame.Y >= 6 * frameHeight)
                {
                    NPC.frame.Y = 0;
                }
            }
            if (Attacking)
            {
                if (ticks < 5)
                {
                    NPC.frame.Y = 14 * frameHeight;
                }
                else if (ticks < 10)
                {
                    NPC.frame.Y = 15 * frameHeight;
                }
                else if (ticks < 15)
                {
                    NPC.frame.Y = 16 * frameHeight;
                }
                else if (ticks < 20)
                {
                    NPC.frame.Y = 17 * frameHeight;
                }
                else if (ticks < 60)
                {
                    NPC.frame.Y = 16 * frameHeight;
                }
                if (ticks < 75)
                {
                    NPC.frame.Y = 15 * frameHeight;
                }
                else if (ticks < 85)
                {
                    NPC.frame.Y = 14 * frameHeight;
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
                    AttackCounter += 1;
                }
                NPC.velocity.X *= 0.9f;
                return true;
            }

            NPC.velocity.X *= 0.85f;
            if ((double)NPC.velocity.X > -0.1 && (double)NPC.velocity.X < 0.1)
                NPC.velocity.X = 0f;
            // attacking
            if (ticks >= 90)
            {
                Attacking = false;
                ticks = 0;
            }
            Attack();
            return false;
        }

        private int AttackCounter = 1;
        private void Attack()
        {
            if (ticks > 30 && ticks < 60 && ticks % 6 == 0)
            {
                Dust.NewDust(NPC.Center + new Vector2(7, -2), 3, 3, DustID.CorruptTorch, newColor: new Color(135, 248, 20));
                Dust.NewDust(NPC.Center + new Vector2(-7, -2), 3, 3, DustID.CorruptTorch,  newColor: new Color(135, 248, 20));
            }
            if (ticks == 35) { SoundEngine.PlaySound(SoundID.Item8, NPC.Center); }
            if (Main.netMode == NetmodeID.MultiplayerClient) { return; }
            if ((AttackCounter + NPC.whoAmI) % 2 == 0) // crippling energy
            {
                if ((ticks == 30 || ticks == 40 || ticks == 50 || ticks == 60) && NPC.target != 255)
                {
                    Vector2 proj_v = Main.player[NPC.target].Center - NPC.Center;
                    proj_v.Normalize();
                    proj_v.RotatedByRandom(0.5f);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, proj_v * 8, ModContent.ProjectileType<CripplingEnergy>(), 70, 2f);
                }
            }
            else // summon
            {
                if (ticks == 50)
                {
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + 2 * 16, (int)NPC.position.Y, ModContent.NPCType<WickedImp>());
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X - 2 * 16, (int)NPC.position.Y, ModContent.NPCType<WickedImp>());
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

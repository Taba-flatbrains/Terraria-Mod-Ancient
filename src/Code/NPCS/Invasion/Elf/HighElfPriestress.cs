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
using Terraria.Audio;

namespace Ancient.src.Code.NPCS.Invasion.Elf
{
    internal class HighElfPriestress : ModNPC
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
				new FlavorTextBestiaryInfoElement("Holy cow"),
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
                }
                else if (ticks < 10)
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
                    AttackCounter += 1;
                    ticks = 0;

                    if (AttackCounter % 2 == 0) // try to target other elf
                    {
                        for (int i = 0; i < Main.npc.Length; i++)
                        {
                            if (ElfInvasion.Elves.Contains(Main.npc[i].type) && i != NPC.whoAmI && Vector2.Distance(Main.npc[i].Center, NPC.Center) < 750 
                                && Main.npc[i].life < Main.npc[i].lifeMax)
                            {
                                AttackLocation = Main.npc[i].Center;
                                return true;
                            }
                        }
                    }

                    // target player
                    if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) < 500)
                    {
                        AttackLocation = Main.player[NPC.target].Center;
                    } else
                    {
                        Attacking = false;
                    }
                }
                return true;
            }

            NPC.velocity.X *= 0.9f;
            if ((double)NPC.velocity.X > -0.1 && (double)NPC.velocity.X < 0.1)
                NPC.velocity.X = 0f;
            // attacking
            if (ticks >= 35)
            {
                Attacking = false;
                ticks = 0;
            }
            Attack();
            return false;
        }

        private int AttackCounter = 0;
        private Vector2 AttackLocation = Vector2.Zero;
        private void Attack()
        {
            if (AttackLocation.X > NPC.Center.X)
            {
                NPC.direction = 1;
            } else
            {
                NPC.direction = -1;
            }

            if (ticks>10) // animation
            {
                Lighting.AddLight(AttackLocation, new Vector3(1, 0.95f, 0.5f) * ticks / 35);
                float rotation = MathF.PI * 2 / 25 * ticks;
                Dust.NewDust(AttackLocation + new Vector2(60, 0).RotatedBy(rotation), 3, 3, DustID.GoldFlame);
                Dust.NewDust(AttackLocation + new Vector2(60, 0).RotatedBy(rotation + (MathF.PI / 12)), 3, 3, DustID.GoldFlame);
            }
            if (ticks == 30) // strike
            {
                SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal, AttackLocation);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), AttackLocation, Vector2.Zero, ModContent.ProjectileType<PurifyingLight>(), 60, 2f);
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

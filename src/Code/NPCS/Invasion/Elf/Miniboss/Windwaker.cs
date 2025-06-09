using Ancient.src.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using Ancient.src.Code.Dusts.ElfInvasion;
using Terraria.Audio;
using Ancient.src.Code.Projectiles.Elf;

namespace Ancient.src.Code.NPCS.Invasion.Elf.Miniboss
{
    internal class Windwaker : ModNPC
    {
        public override void OnKill()
        {
            ElfInvasion.currentKillCount += 15;
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
            NPC.damage = 100;
            NPC.defense = 50;
            NPC.lifeMax = 100000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.PlayerKilled;
            NPC.value = 3000f;

            // NPC.aiStyle = NPCAIStyleID.Fighter; // custom ai
                                                //Banner = Item.NPCtoBanner(NPCID.Zombie); // <- to be replaced with own banner
                                                //BannerItem = Item.BannerToItem(Banner); 
            NPC.knockBackResist = 0.0f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (ElfInvasion.Ongoing)
            {
                return 0.2f;
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
				new FlavorTextBestiaryInfoElement("One of the mightiest elves. She destroys her foes with wind and magic."),
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frameCounter++;
            if (NPC.frameCounter >= 4) // Adjust the frame speed
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
                if (NPC.frame.Y >= 14 * frameHeight)
                {
                    NPC.frame.Y = 0;
                }
            }
            if (ticks % (60 * AttackCooldown) < 5)
            {
                NPC.frame.Y = 15 * frameHeight;
            }
            if (ticks % (60 * AttackCooldown) < 15)
            {
                NPC.frame.Y = 16 * frameHeight;
            }
            if (ticks % (60 * AttackCooldown) < 20)
            {
                NPC.frame.Y = 15 * frameHeight;
            }
        }

        private int AttackMode = 0; // 0: Pick up another Elf, 1: Launch tornado, 2:
        private int TravelMode = 1; // 0: Walking, 1: Hovering, 2: Flying

        private NPC TargetedElf = null;

        private float Acceleration = 0.3f;
        private float FrictionX = 0.9f;
        private float FrictionY = 1f;
        private const float AttackCooldown = 5; // in seconds

        private int ticks = 0;
        private int ticks2 = 0; // Used to determine hovering duration

        private int hoveringMode = 0; // 0: Off, 1: Up, 2: On
        public override void AI()
        {
            if (ticks % (4 * 60) == 0) { NPC.TargetClosest(); }

            // Select Travel Option
            if ((float)NPC.life / (float)NPC.lifeMax > 0.7f)
            {
                TravelMode = 0;
            } else
            {
                TravelMode = 1;
            }

            // Travel
            Vector2 TravelLocation = Vector2.Zero;
            if (Main.player[NPC.target].active)
            {
                TravelLocation = Main.player[NPC.target].position;
            } else
            {
                if (NPC.despawnEncouraged)
                {
                    if (NPC.Center.X > Main.maxTilesX / 2 * 16)
                    {
                        TravelLocation = new Vector2(Main.maxTilesX, ((float)Main.worldSurface + 50) * 16);
                    } else
                    {
                        TravelLocation = new Vector2(0, ((float)Main.worldSurface + 50) * 16);
                    }
                } else
                {
                    TravelLocation = new Vector2(Main.maxTilesX / 2 * 16, ((float)Main.worldSurface + 50) * 16);
                }
            }
            if (ticks % 20 == 0) // Can only change direction every 20 ticks
            {
                if (TravelLocation.X > NPC.Center.X)
                {
                    NPC.direction = 1;
                }
                else
                {
                    NPC.direction = -1;
                }
            }

            if (TravelMode == 0)
            {
                NPC.noGravity = false;
                if (ticks % (60 * AttackCooldown) > 20) // in attack animation, so does not walk
                {
                    NPC.velocity += new Vector2(NPC.direction * Acceleration, 0);
                }
                NPC.velocity.X *= FrictionX;
                NPC.velocity.Y *= FrictionY;
                if (NPCUtils.ShouldJump(NPC))
                {
                    NPC.velocity.Y -= 8;
                }
            } 
            else if (TravelMode == 1)
            {
                if (hoveringMode == 0 || Vector2.Distance(NPC.Center, Main.player[NPC.target].Center) > 16 * 10) // chases player while walking or when far away
                {
                    NPC.velocity += new Vector2(NPC.direction * Acceleration, 0);
                }
                NPC.velocity.X *= FrictionX;
                NPC.velocity.Y *= FrictionY;
                
                if (hoveringMode == 0)
                {
                    NPC.noGravity = false;
                    if (NPCUtils.ShouldJump(NPC) || (MathF.Abs(Main.player[NPC.target].Center.X - NPC.Center.X) < 16 * 7 && Main.player[NPC.target].Center.Y < NPC.Center.Y)) // hovers when trying to jump or if player is close in x direction but above npc
                    {
                        hoveringMode = 1;
                        ticks2 = 0;
                    }
                }
                if (hoveringMode == 1)
                {
                    NPC.velocity.Y = MathF.Min(2, (Main.player[NPC.target].Center.Y - NPC.Center.Y) / 32);
                    NPC.noGravity = true;
                    if (ticks2 > 60 * 4) { hoveringMode = 2; ticks2 = 0; }
                }
                if (hoveringMode == 2)
                {
                    NPC.noGravity = true;
                    if (Main.player[NPC.target].Center.Y - 3 * 16 > NPC.Center.Y || ticks2 > 60 * 5)
                    {
                        hoveringMode = 0;
                        ticks2 = 0;
                    }
                }
            } 
            else if (TravelMode == 2) // currently unused
            {
                NPC.noGravity = true;
            }

            // Choose attack
            if (ticks % (60 * AttackCooldown) == 0) 
            {
                AttackMode = (AttackMode + 1) % 2;
            }

            // Attacking
            if (AttackMode == 0) // boost elf
            {
                if (ticks % (60 * AttackCooldown) == 0) // targetting
                {
                    TargetedElf = null;
                    for (int i = 0; i < Main.npc.Length; i++)
                    {
                        if (ElfInvasion.Elves.Contains(Main.npc[i].type) && i != NPC.whoAmI && Vector2.Distance(Main.npc[i].Center, NPC.Center) < 750)
                        {
                            TargetedElf = Main.npc[i];

                            break;
                        }
                    }
                }
                if (TargetedElf != null)
                {
                    if (ticks % (60 * AttackCooldown) == 10)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal, NPC.Center);
                        TargetedElf.noGravity = true;
                        TargetedElf.GetGlobalNPC<WindwakerBuffNPC>().active = true;
                    }

                    if (ticks % (60 * AttackCooldown) > 20) // in attack animation
                    {
                        if (TargetedElf.Center.X > NPC.Center.X)
                        {
                            NPC.direction = 1;
                        }
                        else
                        {
                            NPC.direction = -1;
                        }
                    }
                }
            }
            else if (AttackMode == 1) // launch tornados
            {
                if (ticks % (60 * AttackCooldown) == 0) // targetting
                {
                    NPC.TargetClosest();
                }
                if (NPC.target == 255) { return; }

                if (Vector2.Distance(NPC.position, Main.player[NPC.target].position) > 16 * 100) { return; }

                if (ticks % (60 * AttackCooldown) == 10)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), 
                        NPC.Center, new Vector2(10 * NPC.direction, 0),
                        ProjectileID.SandnadoHostile, 70, 0.5f);
                }
            }


            // Dusts
            float DustChance = 0.5f;
            if (hoveringMode != 0 || TravelMode == 2)
            {
                DustChance *= 2;
            }
            if (new Random().NextSingle() < DustChance)
                Dust.NewDust(NPC.Center + new Vector2(0, 3), 0, 0, ModContent.DustType<WindwakerDust>(), SpeedX: NPC.velocity.X, SpeedY: NPC.velocity.Y);

            ticks++;
            ticks2++;
        }
    }

    public class WindwakerBuffNPC : GlobalNPC
    {
        public bool active = false;
        public override bool InstancePerEntity => true;

        public override void PostAI(NPC npc)
        {
            if (active)
            {
                // Dusts
                float DustChance = 0.5f;
                if (new Random().NextSingle() < DustChance)
                    Dust.NewDust(npc.Center + new Vector2(0, 3), 0, 0, ModContent.DustType<WindwakerDust>(), SpeedX: npc.velocity.X, SpeedY: npc.velocity.Y);

                npc.velocity.Y *= 0.9f;
                if (Main.player[npc.target].Center.Y > npc.Center.Y)
                {
                    npc.velocity.Y += 0.15f;
                } else
                {
                    npc.velocity.Y -= 0.15f;
                }
            }
        }
    }
}

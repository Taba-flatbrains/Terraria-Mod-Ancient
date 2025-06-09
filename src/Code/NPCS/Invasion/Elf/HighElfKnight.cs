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
    internal class HighElfKnight : ModNPC
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
            NPC.damage = 100;
            NPC.defense = 90;
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
				new FlavorTextBestiaryInfoElement("Very skilled knight, learned it the old fashioned way from the greatest masters in sword fighting."),
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5) // Adjust the frame speed
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
                if (NPC.frame.Y >= 14 * frameHeight)
                {
                    NPC.frame.Y = frameHeight;
                }
            }
            if (AttackAnimation)
            {
                NPC.frame.Y = 15 * frameHeight;
            }
        }


        private bool Attacking = false;
        private bool AttackAnimation = false;
        private int ticks = 0;
        public override bool PreAI()
        {
            ticks += 1;

            if (!Attacking) // not attacking
            {
                if (ticks >= 60)
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
                AttackAnimation = false;
                ticks = 0;
            }
            Attack();
            return true;  // always walking
        }

        private void Attack()
        {
            if (NPC.target != 255 && Vector2.Distance(NPC.position, Main.player[NPC.target].position) < 200 && ticks == 1)
            {
                AttackAnimation = true;
                SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<HighElfGreatsword>(), 100, 4f, ai0: NPC.whoAmI);
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

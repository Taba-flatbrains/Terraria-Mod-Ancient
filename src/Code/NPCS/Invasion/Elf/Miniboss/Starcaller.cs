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
using Ancient.src.Code.Projectiles.Elf;
using Microsoft.Xna.Framework;

namespace Ancient.src.Code.NPCS.Invasion.Elf.Miniboss
{
    internal class Starcaller : ModNPC
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
            NPC.value = 30000f;

            NPC.aiStyle = NPCAIStyleID.Fighter; // custom ai
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
				new FlavorTextBestiaryInfoElement("He uses powerful celestrial magic to stomp his opponents into the ground. "),
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
                    NPC.frame.Y = frameHeight;
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

        private const float AttackCooldown = 5; // in seconds

        private bool Attacking = false;
        private int ticks = 0;
        public override bool PreAI()
        {
            ticks += 1;

            if (!Attacking) // not attacking
            {
                if (ticks % (60 * AttackCooldown) == 0)
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

        private int AttackType = 0; // 0: Dschotrom (Metorites), 1: Celestrial Shover (Ars Margica Star Rain)
        private void Attack()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) { return; }

            if (NPC.target == 255) { return; }

            if (Vector2.Distance(NPC.position, Main.player[NPC.target].position) > 16 * 100) { return; }

            if (ticks == 0)
            {
                AttackType += 1;
                AttackType %= 2;
            }

            if (ticks > 10 && ticks < 15 && AttackType == 0)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), Main.player[NPC.target].position + new Vector2(new Random().Next(16 * -5, 16 * 5), -16 * 50), new Vector2(0, 4).RotatedByRandom(0.5f),
                    ModContent.ProjectileType<Dschotrom>(), 100, 0.5f);
            }

            if (ticks == 10 && AttackType == 1)
            {
                for (int i = 0; i < 6; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), Main.player[NPC.target].position + new Vector2(16 * (i - 3) * 4, -16 * 50), new Vector2(0, 15),
                    ModContent.ProjectileType<CelestrialShower>(), 90, 0.2f);
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

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
    internal class DarkElfAssasin : ModNPC
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
            NPC.defense = 50;
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
				new FlavorTextBestiaryInfoElement("Specialized at murdering not so innocent players."),
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
            if (ticks >= 40)
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

            if (Vector2.Distance(NPC.position, Main.player[NPC.target].position) > 16 * 40) 
            { 
                Attacking = false;
                return;
            }

            if (ticks == 1) // teleports onto player and jumps at them
            {
                if (FoundSpaceForAttack(out Vector2 newPosition))
                {
                    NPC.position = newPosition;
                    SoundEngine.PlaySound(SoundID.Item8, position: NPC.Center);
                    Vector2 direction = Main.player[NPC.target].position + new Vector2(0, -30) - NPC.position;
                    direction.Normalize();
                    NPC.velocity = direction * 25;
                } else
                {
                    Attacking = false;
                    return;
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

        private bool FoundSpaceForAttack(out Vector2 position)
        {
            for (int i = 0; i < 10; i++)
            {
                position = Main.player[NPC.target].position - new Vector2(NPC.direction * 16 * (13 + i), 10 + i);
                if (!Main.tile[position.ToTileCoordinates()].HasUnactuatedTile)
                {
                    return true;
                }
            }
            position = Vector2.Zero;
            return false;
        }
    }
}

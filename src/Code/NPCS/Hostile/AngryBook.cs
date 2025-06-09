using Ancient.src.Code.Projectiles;
using Ancient.src.Code.Walls;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Ancient.src.Code.NPCS.Hostile
{
    internal class AngryBook : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 3;

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
            NPC.height = 28;
            NPC.damage = 20;
            NPC.defense = 0;
            NPC.lifeMax = 50;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath7;
            NPC.value = 550f;
            NPC.aiStyle = -1; // custom ai
                              //Banner = Item.NPCtoBanner(NPCID.Zombie); // <- to be replaced with own banner
                              //BannerItem = Item.BannerToItem(Banner); 
            NPC.knockBackResist = 0.2f;
            NPC.noGravity = true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY + 1].WallType == ModContent.WallType<AncientRuinWall>() // only spawns inside the ruins
                || Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY + 1].WallType == ModContent.WallType<AncientRuinWall2>())
            {
                return SpawnCondition.Cavern.Chance*100;
            }

            return 0f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("A book fighting to keep its secrets."),
            });
        }

        public int Mode = 0; // 0: slowly wandering in direction player, 1: walking left, 2: walking right, 3: oscilating left-right and flying up, 4: slowly losing height
        public int TicksTillModeChange = 0;
        public int AttackCooldown = 100;
        public override void AI()
        {
            Player nearestPlayer = GetNearestPlayer(NPC);
            if (nearestPlayer.Center.X > NPC.Center.X)
            {
                NPC.direction = 1;
                NPC.spriteDirection = 1;
            } else
            {
                NPC.direction = -1;
                NPC.spriteDirection = -1;
            }
            if (TicksTillModeChange == 0)
            {
                Mode = new Random().Next(0, 5);
                TicksTillModeChange = 160;
            }
            switch (Mode) 
            {
                case 0:
                    {
                        NPC.velocity = (nearestPlayer.Center + new Vector2(0, -3*16) - NPC.Center) / 150;  // trying to hover over player
                        break;
                    }
                case 1:
                    {
                        NPC.velocity = new Vector2(0.3f, 0);
                        break;
                    }
                case 2:
                    {
                        NPC.velocity = new Vector2(-0.3f, 0);
                        break;
                    }
                case 3:
                    {
                        NPC.velocity = new Vector2((float)Math.Sin(Main.time/60)/3, 0.2f);
                        break;
                    }
                case 4:
                    {
                        NPC.velocity = new Vector2((float)Math.Sin(Main.time / 30) / 10, -0.3f);
                        break;
                    }
            }
            TicksTillModeChange--;

            if (Main.netMode != NetmodeID.MultiplayerClient && (nearestPlayer.Center.Distance(NPC.Center)) < 400)
            {
                if (AttackCooldown == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (nearestPlayer.Center - NPC.Center) / 30, ModContent.ProjectileType<PaperProjectile>(), NPC.damage, 0.2f, Main.myPlayer);
                    SoundEngine.PlaySound(SoundID.NPCHit5, position: NPC.position);
                    AttackCooldown = 200;
                }
                AttackCooldown--;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5) // Adjust the frame speed
            {
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 2 * frameHeight) // Adjust the number of frames in your animation
                    NPC.frame.Y = 0;
                NPC.frameCounter = 0;
            }
        }

        private Player GetNearestPlayer(NPC npc)  // made by chat gpt
        {
            Player nearestPlayer = null;
            float shortestDistance = float.MaxValue;

            // Iterate through players and find the nearest one
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];

                // Skip non-active players
                if (player.active)
                {
                    // Calculate the distance between the NPC and the player
                    float distance = Vector2.Distance(npc.Center, player.Center);

                    // Check if the current player is closer than the previous nearest player
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        nearestPlayer = player;
                    }
                }
            }

            return nearestPlayer;
        }
    }
}

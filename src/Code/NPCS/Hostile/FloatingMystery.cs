using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Ancient.src.Code.Walls;
using Terraria.ModLoader.Utilities;
using Ancient.src.Common.Structures;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Ancient.src.Code.NPCS.Hostile
{
    internal class FloatingMystery : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 10;

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
            NPC.damage = 30;
            NPC.defense = 2;
            NPC.lifeMax = 200;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath7;
            NPC.value = 250f;
            NPC.aiStyle = -1; // custom ai
                              //Banner = Item.NPCtoBanner(NPCID.Zombie); // <- to be replaced with own banner
                              //BannerItem = Item.BannerToItem(Banner); 
            NPC.knockBackResist = 0.1f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Nothing is known about this thing."),
            });
        }

        public static int VisualTypes = 2;
        public static List<int> VisualTypesFrameEndList = new List<int>()
        { 
            0, // -1 type not used
            5, // first type (Visual Type = 0)
            10
        };
        public static List<int> VisualTypesAlphaList = new List<int>()
        {
            0, // first type (Visual Type = 0)
            170
        };
        public int VisualType = 0;
        public override void OnSpawn(IEntitySource source)
        {
            VisualType = new Random().Next(0, VisualTypes);
            NPC.alpha = VisualTypesAlphaList[VisualType];
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5) // Adjust the frame speed
            {
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > (VisualTypesFrameEndList[VisualType+1]-1) * frameHeight) // Adjust the number of frames in your animation
                    NPC.frame.Y = frameHeight * VisualTypesFrameEndList[VisualType];
                NPC.frameCounter = 0;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<AncientRuinsBiome>())
            {
                return SpawnCondition.Cavern.Chance * 9;
            }
            return 0f;
        }

        public Vector2 direction = new Vector2(0, 0);
        public static float MaxVelocity = 0.3f;
        public int ticksTillDirectionChange = 0;
        private int counter = 0;
        public override void AI()
        {
            Player nearestPlayer = GetNearestPlayer(NPC);
            NPC.rotation += 0.05f;
            NPC.velocity = direction * new Vector2((float)Math.Sin(ticksTillDirectionChange/10)+2f, (float)Math.Cos(ticksTillDirectionChange / 10)+2f);
            NPC.velocity /= 2;
            Lighting.AddLight(NPC.position, new Vector3(0.25f, 0.25f, 0.3f));
            if (ticksTillDirectionChange == 0)
            {
                ticksTillDirectionChange = 300;
                if (counter%5==0)
                {
                    direction = new Vector2(MathF.Sin(nearestPlayer.position.X-NPC.position.X+counter), MathF.Sin(nearestPlayer.position.Y - NPC.position.Y));
                } else
                {
                    direction = nearestPlayer.Center - NPC.Center;
                }
                direction.Normalize();
                direction *= MaxVelocity;
            }
            ticksTillDirectionChange--;
            counter++;
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

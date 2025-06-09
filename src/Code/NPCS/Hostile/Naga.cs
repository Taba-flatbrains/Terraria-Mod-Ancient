using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Ancient.src.Code.Walls;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using Ancient.src.Common.Structures;
using Terraria.Graphics.Light;
using Terraria.Audio;
using Ancient.src.Code.Projectiles;

namespace Ancient.src.Code.NPCS.Hostile
{
    internal class Naga : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 9;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                // Influences how the NPC looks in the Bestiary
                Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 62;
            NPC.damage = 150; 
            NPC.defense = 30;
            NPC.lifeMax = 30000;
            NPC.HitSound = SoundID.NPCHit7;
            NPC.DeathSound = SoundID.NPCDeath5;
            NPC.value = 3000f;
            NPC.aiStyle = -1; // custom ai
                              //Banner = Item.NPCtoBanner(NPCID.Zombie); // <- to be replaced with own banner
                              //BannerItem = Item.BannerToItem(Banner); 
            NPC.knockBackResist = 0.1f;
            NPC.buffImmune = NPCID.Sets.ImmuneToRegularBuffs;

            NPC.npcSlots = 2f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY + 1].WallType == ModContent.WallType<OceanTempleWall>() ||
                spawnInfo.Player.InModBiome<OceanTempleBiome>()) // only spawns inside the ruins or when player is inside the biome
            {
                return (SpawnCondition.Ocean.Chance + SpawnCondition.Underground.Chance + SpawnCondition.Overworld.Chance) * 5;
            }
            return 0f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("A majestic oceanbound species, very strong in both magical and physical combat."),
            });
        }


        public int Mode = 0; // 0 = walking, 1 = shooting magic bolt, 3 = teleporting behind player, 2 || 4 = charging at player
        public int ModeTicks = 1;
        public Player target => Main.player[NPC.target];


        private float Acceleration = 0.25f;
        private float FrictionX = 0.9f;
        private float FrictionY = 1f;

        public override void AI()
        {
            ModeTicks--;
            if (ModeTicks <= 0)
            {
                Mode = (Mode + 1) % 5;
                ModeTicks = 60 * 3;
                if (Mode == 0)
                {
                    NPC.direction *= -1;
                } else
                {
                    NPC.TargetClosest();
                }
            }

            if (Mode == 0 || NPC.despawnEncouraged)
            {
                NPC.velocity += new Vector2(NPC.direction * Acceleration, 0);
                if (NPCUtils.ShouldJump(NPC))
                {
                    NPC.velocity.Y -= 7;
                }
                if (NPC.collideX && ModeTicks % 5 == 0) { NPC.direction *= -1; }
            }
            else if (Mode == 1)
            {
                if (ModeTicks == 60 * 3) {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.direction * 5, -5), (target.Center - NPC.Center) / 40, ModContent.ProjectileType<NagaMagicProjectile>(), 120, 0.1f);
                    ModeTicks = 30;
                }
            }
            else if (Mode == 3)
            {
                if (ModeTicks == 60 * 3) { 

                    int IOffset = 0;
                    if (NPC.direction == 1) { IOffset = 8; }
                    for (int i = 0; i < 16; i++) // iterate until good location is found
                    {
                        Vector2 offset = new Vector2(7 * 16, 0).RotatedBy(MathF.PI / 8 * (i + IOffset));
                        Vector2 NewPosition = target.Center + offset;
                        if (!NPCUtils.InWall(NewPosition, NPC.width, NPC.height))
                        {
                            SoundEngine.PlaySound(SoundID.Item8, position: NPC.Center);
                            NPC.position = NewPosition;
                        }
                    }
                    ModeTicks = 30;
                }
            }
            else if (Mode == 4 || Mode == 2)
            {
                NPC.velocity += new Vector2(NPC.direction * Acceleration * 1.3f, 0);
                if (NPCUtils.ShouldJump(NPC))
                {
                    NPC.velocity.Y -= 7;
                }
            }
            

            NPC.velocity.X *= FrictionX;
            NPC.velocity.Y *= FrictionY;

            NPC.spriteDirection = NPC.direction;

            // despawning
            if (!OceanTempleBiome.StaticNearBiome(target))
            {
                NPC.despawnEncouraged = true; // Naga can only walk outside of temple
            }
        }


        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5) // Adjust the frame speed
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
                if (NPC.frame.Y >= 8 * frameHeight)
                {
                    NPC.frame.Y = 0;
                }
            }
        }
    }
}

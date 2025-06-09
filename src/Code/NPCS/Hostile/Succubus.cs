using Ancient.src.Code.Walls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using rail;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Ancient.src.Code.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace Ancient.src.Code.NPCS.Hostile
{
    internal class Succubus : ModNPC
    {
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
            NPC.width = 40;
            NPC.height = 56;
            NPC.damage = 80;
            NPC.defense = 50;
            NPC.lifeMax = 40000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.PlayerKilled;
            NPC.value = 12000f;
            NPC.aiStyle = -1; // custom ai
                              //Banner = Item.NPCtoBanner(NPCID.Zombie); // <- to be replaced with own banner
                              //BannerItem = Item.BannerToItem(Banner); 
            NPC.knockBackResist = 0.05f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneUnderworldHeight && NPC.downedMoonlord)
            {
                return SpawnCondition.Underworld.Chance * 0.7f;
            }
            return 0f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("A beatiful demon trying to deceive you. Don't fall for it."),
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
                if (NPC.frame.Y >= 14 * frameHeight && travelingMode == 0)
                {
                    NPC.frame.Y = frameHeight;
                }
                else if (NPC.frame.Y >= 17 * frameHeight && travelingMode == 1)
                {
                    NPC.frame.Y = 14 * frameHeight;
                }
                else if (NPC.frame.Y < 13 * frameHeight && travelingMode == 1)
                {
                    NPC.frame.Y = 14 * frameHeight;
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (AttackType == 2)
            {
                Vector2 origin;
                Vector2 offset;
                SpriteEffects Flipped = SpriteEffects.None;
                origin = new Vector2(0, 16);
                offset = new Vector2(11, 8);
                if (NPC.direction == -1)
                {
                    offset = new Vector2(-11, 8);
                    //origin = new Vector2(14, 16);
                    Flipped = SpriteEffects.FlipHorizontally;
                }
                Main.instance.LoadItem(ItemID.DemonScythe);
                spriteBatch.Draw(TextureAssets.Item[ItemID.DemonScythe].Value, NPC.Center - screenPos + offset + new Vector2(-10,0), default, 
                    Lighting.GetColor(NPC.Center.ToTileCoordinates()).MultiplyRGB(new Color(0.95f, 0.9f, 0.9f)),
                    0.0f, origin, 0.8f, Flipped, 0f);
            }
        }

        private int ticks = 0;
        private int AttackCooldown = 0;

        private int AttackType = 0; // 0: Blink behind player, 1: Demon Scythe, 2: ahri charm
        private int AttackTime = 0;

        private int travelingMode = 0; // 0: Walking, 1: Flying

        private float Acceleration = 0.13f;
        private float FrictionX = 0.85f;
        private float FrictionY = 1f;
        

        public override void AI()
        {
            if (ticks % (4 * 60) == 0) { NPC.TargetClosest(); }

            if (ticks % (10 * 60) == 0) { travelingMode = (travelingMode + 1) % 2; }

            ticks++;

            // Travel
            Vector2 TravelLocation = Vector2.Zero;
            if (Main.player[NPC.target].active)
            {
                TravelLocation = Main.player[NPC.target].position;
            }
            else
            {
                TravelLocation = new Vector2(Main.maxTilesX / 2 * 16, ((float)Main.worldSurface + 50) * 16);
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

                if (NPC.despawnEncouraged)
                {
                    NPC.direction *= -1;
                }
            }


            if (travelingMode == 0) // walking
            {
                NPC.noGravity = false;
                NPC.velocity += new Vector2(NPC.direction * Acceleration, 0);
                NPC.velocity.X *= FrictionX;
                NPC.velocity.Y *= FrictionY;
                if (NPCUtils.ShouldJump(NPC))
                {
                    NPC.velocity.Y -= 7;
                }
            }
            else if (travelingMode == 1) // flying
            {
                NPC.noGravity = true;
                Vector2 direction = TravelLocation - NPC.Center;
                direction.Normalize();
                direction *= Acceleration;
                NPC.velocity += direction;
                NPC.velocity += new Vector2(NPC.direction * Acceleration / 3, 0);
                NPC.velocity.X *= 0.94f;
                NPC.velocity.Y *= 0.91f;
            }

            // Attacking
            if (AttackType == 0)
            {
                if (AttackTime == 50)
                {
                    int IOffset = 0;
                    if (NPC.direction == 1) { IOffset = 8; }
                    for (int i = 0; i < 16; i++) // iterate until good location is found
                    {
                        Vector2 offset = new Vector2(7 * 16, 0).RotatedBy(MathF.PI / 8 * (i + IOffset));
                        Vector2 NewPosition = TravelLocation + offset;
                        if (!NPCUtils.InWall(NewPosition, NPC.width, NPC.height))
                        {
                            SoundEngine.PlaySound(SoundID.Item8, position: NPC.Center);
                            NPC.position = NewPosition;
                        }
                    }
                }

                if (AttackTime > 120)
                {
                    AttackTime = 0;
                    AttackType = 1;
                }
            }
            else if (AttackType == 1)
            {
                if (AttackTime == 1)
                {
                    SoundEngine.PlaySound(SoundID.Item1, NPC.position);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
                        direction.Normalize();
                        direction *= 6;
                        direction = direction.RotatedByRandom(0.1);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction, ModContent.ProjectileType<DevilishKiss>(), 90, 0.2f);
                    }
                }

                if (AttackTime > 180)
                {
                    AttackTime = 0;
                    AttackType = 2;
                }
            }
            else if (AttackType == 2)
            {
                if (AttackTime % 20 == 1)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
                        direction.Normalize();
                        direction *= 4.5f;
                        direction = direction.RotatedByRandom(0.2);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction, ProjectileID.DemonSickle, 90, 0.2f);
                    }
                }

                if (AttackTime > 60)
                {
                    AttackTime = 0;
                    AttackType = 0;
                }
            }
            AttackTime += 1;
        }
    }
}

using Ancient.src.Code.Projectiles;
using Ancient.src.Code.Walls;
using Ancient.src.Common.Structures;
using System;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.ModLoader.Config;
using Ancient.src.Code.Projectiles.Kiranocif;
using Terraria.Graphics.Light;

namespace Ancient.src.Code.NPCS.Hostile
{
    internal class Kiranocif : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 7;

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
            NPC.height = 75;
            NPC.damage = 150;
            NPC.defense = 70;
            NPC.lifeMax = 250000;
            NPC.HitSound = SoundID.NPCHit7;
            NPC.DeathSound = SoundID.NPCDeath5;
            NPC.value = 150000f;
            NPC.aiStyle = -1; // custom ai
                              //Banner = Item.NPCtoBanner(NPCID.Zombie); // <- to be replaced with own banner
                              //BannerItem = Item.BannerToItem(Banner); 
            NPC.knockBackResist = 0f;
            NPC.buffImmune = NPCID.Sets.ImmuneToRegularBuffs;

            NPC.npcSlots = 2f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (NPC.downedMoonlord && !NPC.AnyNPCs(Type)) // post moonlord and only 1 at a time
            {
                return SpawnCondition.SandstormEvent.Chance * 0.6f;
            }
            return 0f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Classic bkc tar"),
            });
        }

        private int ticks = 0;

        private int mode = 0; // 0:  , 1:
        private int ticksUntilChange = 0;


        private float Acceleration = 0.2f;
        private float FrictionX = 0.8f;
        private float FrictionY = 1f;

        private Vector2 RockSlidePointer = Vector2.Zero;
        public override void AI()
        {
            ticks++;


            // targetting code
            if (ticks % (4 * 60) == 0) { NPC.TargetClosest(); }
            Player target = Main.player[NPC.target];

            // Despawn Code
            bool allPlayersDead = true;
            foreach (Player player in Main.player)
            {
                if (player.active && !player.dead)
                {
                    allPlayersDead = false;
                    break;
                }
            }
            if (allPlayersDead)
            {
                if (!NPC.despawnEncouraged) {
                    NPC.EncourageDespawn(10 * 60);
                }
            }
            if (NPC.despawnEncouraged)
            {
                if (NPC.timeLeft <= 0)
                {
                    NPC.active = false;
                }
                if (Vector2.Distance(NPC.Center, target.Center) < 60 * 16 && target.active && !target.dead) { 
                    NPC.DiscourageDespawn(60000);
                }
            }


            // Travel
            Vector2 TravelLocation = Vector2.Zero;
            if (target.active)
            {
                TravelLocation = target.position;
            }
            else
            {
                if (NPC.despawnEncouraged)
                {
                    if (NPC.Center.X > Main.maxTilesX / 2 * 16)
                    {
                        TravelLocation = new Vector2(Main.maxTilesX, ((float)Main.worldSurface + 50) * 16);
                    }
                    else
                    {
                        TravelLocation = new Vector2(0, ((float)Main.worldSurface + 50) * 16);
                    }
                }
                else
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
            if (true) // Should Move
            {
                NPC.velocity += new Vector2(NPC.direction * Acceleration, 0);
                NPC.velocity.X *= FrictionX;
                NPC.velocity.Y *= FrictionY;

                if (NPCUtils.ShouldJump(NPC))
                {
                    NPC.velocity.Y -= 7;
                }
            }

            NPC.spriteDirection = NPC.direction;



            //Attacking
            ticksUntilChange--;
            if (ticksUntilChange <= 0) // Change Mode 0: rock slide, 1: rock shield, 2: komet (Rock Tomb), 3: mean look
            {
                mode = (mode + 1) % 5;
                switch (mode)
                {
                    case 0:
                        ticksUntilChange = 90;
                        RockSlidePointer = new Vector2(0, 16 * -60) + NPC.Center;
                        break;
                    case 1:
                        ticksUntilChange = 160;

                        for (int i = 0; i < 5; i++) {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity,
                                        ModContent.ProjectileType<RockShield>(), 90, 4, -1, i, target.whoAmI, NPC.whoAmI);
                            }
                        }

                        break;
                    case 2:
                        ticksUntilChange = 120;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 offset;
                            Vector2 velocity;

                            offset = new Vector2(16 * 50 * NPC.direction, -16 * 32);
                            velocity = -offset;
                            velocity.Normalize();
                            velocity *= 12;

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), target.Center + offset, velocity,
                                        ModContent.ProjectileType<RockTombProjectile>(), 60, 20, -1);
                        }
                        break;
                    case 3:
                        ticksUntilChange = 200;
                        break;
                    case 4:
                        ticksUntilChange = 1;
                        break;
                }
            }

            switch (mode)
            {
                case 0:
                    if (ticksUntilChange > 25)
                    {
                        RockSlidePointer.Y = NPC.Center.Y + 16 * -60;
                        if (target.position.X > RockSlidePointer.X)
                        {
                            RockSlidePointer.X += 15;
                        } else
                        {
                            RockSlidePointer.X += -15;
                        }

                        bool RocksShouldFall = false;
                        float smallestY = RockSlidePointer.Y + 16 * 140; // ignores players way below kiranocif
                        for (int i = 0; i < Main.maxPlayers; i++) {
                            Player player = Main.player[i];
                            if (player.active && !player.dead)
                            {
                                if (MathF.Abs(player.Center.X - RockSlidePointer.X) < 16 * 5 && player.Center.Y > RockSlidePointer.Y) // player below and near the pointer
                                {
                                    RocksShouldFall = true;
                                    smallestY = MathF.Min(player.Center.Y, RockSlidePointer.Y); // select highest player as target
                                }
                            }
                        }

                        if (RocksShouldFall && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (ticks % 4 == 0)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(RockSlidePointer.X, smallestY - (32 * 16)), new Vector2(0, 12), 
                                    ModContent.ProjectileType<RockProjectile>(), 70, 4);
                            }
                        }
                    }
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    if (ticksUntilChange == 1)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(10 * NPC.direction, -22), new Vector2(0, -3),
                                        ModContent.ProjectileType<MeanLookProjectile>(), 40, 0, -1, target.whoAmI);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(10 * NPC.direction, -22), new Vector2(0, 3),
                                        ModContent.ProjectileType<MeanLookProjectile>(), 40, 0, -1, target.whoAmI);
                        }
                    }
                    break;
                case 4:
                    break;
            }
        }

        private const int BodyFrame = 0;
        private const int HeadFrame = 1;
        private const int FrontLegFrame = 2;
        private const int BackLegFrame = 3;
        private const int BackArmFrame = 4;
        private const int FrontArmFrame = 5;
        private const int TailFrame = 6;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;

            Vector2 bodyCenter = NPC.Center + new Vector2(0, NPC.gfxOffY) - Main.screenPosition; // screen position already included, should use bodycenter for all future calculations

            Vector2 hipPos = bodyCenter + new Vector2(-3 * NPC.spriteDirection, 16);
            Vector2 tailPos = bodyCenter + new Vector2(-9 * NPC.spriteDirection, 12);
            Vector2 shoulderPos = bodyCenter + new Vector2(2 * NPC.spriteDirection, -6);
            Vector2 headPos = bodyCenter + new Vector2(4 * NPC.spriteDirection, -11);

            Vector2 FrontLegMovementOffset = new Vector2(MathF.Sin((ticks / 15f)) * 3.5f, Math.Min(MathF.Sin((ticks / 15f) + MathHelper.PiOver2 + +MathHelper.Pi) * 2, 0));
            Vector2 BackLegMovementOffset = new Vector2(MathF.Sin((ticks / 15f) + MathHelper.Pi) * 3.5f, Math.Min(MathF.Sin((ticks / 15f) + MathHelper.PiOver2) * 2, 0));

            FrontLegMovementOffset.X *= NPC.spriteDirection;
            BackLegMovementOffset.X *= NPC.spriteDirection;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }


            float ArmRotation = MathF.Sin(ticks / 120f) * 0.1f;

            spriteBatch.Draw(texture, tailPos, new Rectangle(0, TailFrame * 70, 70, 70)
                , drawColor, MathF.Sin(ticks/60f)*0.1f, new Vector2(40, 36), Vector2.One, spriteEffects, 0); // Tail
            spriteBatch.Draw(texture, hipPos + BackLegMovementOffset, new Rectangle(0, BackLegFrame * 70, 70, 70)
                , drawColor, 0, new Vector2(35, 28), Vector2.One, spriteEffects, 0); // Back Leg
            spriteBatch.Draw(texture, shoulderPos, new Rectangle(0, BackArmFrame * 70, 70, 70)
                , drawColor, 0.2f - ArmRotation, new Vector2(35, 35), Vector2.One, spriteEffects, 0); // Back Armm
            spriteBatch.Draw(texture, bodyCenter, new Rectangle(0, BodyFrame * 70, 70, 70)
                , drawColor, 0, new Vector2(35,35), Vector2.One, spriteEffects, 0); // Body
            spriteBatch.Draw(texture, hipPos + FrontLegMovementOffset, new Rectangle(0, FrontLegFrame * 70, 70, 70)
                , drawColor, 0, new Vector2(35, 28), Vector2.One, spriteEffects, 0); // Front Leg
            spriteBatch.Draw(texture, shoulderPos, new Rectangle(0, FrontArmFrame * 70, 70, 70)
                , drawColor, ArmRotation, new Vector2(35, 35), Vector2.One, spriteEffects, 0); // Front Arm
            spriteBatch.Draw(texture, headPos, new Rectangle(0, HeadFrame * 70, 70, 70)
                , drawColor, 0, new Vector2(35, 45), Vector2.One, spriteEffects, 0); // Head


            // Eye glowing animation
            if (mode == 3)
            {
                Vector2 eyePos = headPos + new Vector2(6 * NPC.direction, -13);

                texture = ModContent.Request<Texture2D>("Ancient/src/Code/NPCS/Hostile/KiranocifEyeSpark").Value;

                Color color;
                color = Color.White;
                color = new Color(new Vector4(color.ToVector3(), 0.0f));
                spriteBatch.Draw(texture, eyePos, new Rectangle(0, 0, 20, 20)
                , color, ticks * 0.05f, new Vector2(10, 10), Vector2.One * (1f - (ticksUntilChange * 0.003f)), SpriteEffects.None, 0); // Center

                Lighting.AddLight(eyePos + screenPos, Color.LightBlue.ToVector3() * (0.8f - (ticksUntilChange * 0.003f)));
            }

            return false;
        }
    }
}

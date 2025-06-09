using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using System.Drawing.Printing;
using Ancient.src.Code.Projectiles.Boss.YrimirsSoul;
using Ancient.src.Common.Systems;
using ReLogic.Content;
using Terraria.DataStructures;
using Terraria.GameContent.UI.BigProgressBar;
using Ancient.src.Common.Util;
using Ancient.src.Code.Dusts;

namespace Ancient.src.Code.NPCS.Boss.YrimirsSoul
{
    [AutoloadBossHead] 
    internal class YrimirsSoul : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 20;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                // Influences how the NPC looks in the Bestiary
                Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            // Automatically group with other bosses
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 48;
            NPC.height = 100;
            NPC.damage = 100;
            NPC.defense = 80;
            NPC.lifeMax = 350000;
            NPC.HitSound = SoundID.NPCHit7;
            NPC.DeathSound = SoundID.NPCDeath5;
            NPC.value = 300000f;
            NPC.aiStyle = -1; // custom ai
                              //Banner = Item.NPCtoBanner(NPCID.Zombie); // <- to be replaced with own banner
                              //BannerItem = Item.BannerToItem(Banner); 
            NPC.knockBackResist = 0f;
            NPC.buffImmune = NPCID.Sets.ImmuneToRegularBuffs;


            NPC.boss = true;
            NPC.npcSlots = 6f;
            // Custom boss bar
            NPC.BossBar = ModContent.GetInstance<YrimirsSoulBossBar>(); 

            // The following code assigns a music track to the boss in a simple way.
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot("Ancient/src/Assets/Music/Darkness Embrace"); // todo: write music :(
            }

            NPC.npcSlots = 6f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("May the legend live on forever"),
            });
        }

        public override void OnKill()
        {
            // This sets downedMinionBoss to true, and if it was false before, it initiates a lantern night
            NPC.SetEventFlagCleared(ref DownedBossSystem.DownedYrimirsSoul, -1);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses; // use the boss immunity cooldown counter, to prevent ignoring boss attacks by taking damage from other sources
            return true;
        }

        private int ticks = 0;

        private int MovementMode = 0; // -1: no movement with friction, 0: no movement / movement guided by current attack, 1: walking towards target, 2: run in air 

        private float Acceleration = 1f;
        private float FrictionX = 0.75f;
        private float FrictionY = 1f;



        public int AttackMode = -1; // 0: none+spawn sword  sword (run), 1: sword stab, 2: sword slash, 3: summon infernal spirits (sword into sky), 4: none, 5: crosschop

        public int AttackTimer = 0;

        private int InfernalSpiritsSummoningCount = 0;

        public int Attack2Duration = 25;

        private Vector2 OriginPos = Vector2.Zero;
        private int VerticalMoveCooldown = 0;
        private int VerticalMoveMode = 0; // used for jumping / falling in air: 0: no vertical movement, 1: jumping, 2: falling
        public override void AI()
        {
            ticks++;
            AttackTimer--;

            if (ticks % (4 * 60) == 0) { NPC.TargetClosest(); }
            Player target = Main.player[NPC.target];

            if (AttackTimer <= 0) 
            {
                AttackMode += 1;
                if (AttackMode >= 6)
                {
                    AttackMode = -1;
                }

                switch (AttackMode) // On Attack Selected
                {
                    case 0:
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BlazingEmber>(), 100, 4f, -1, NPC.whoAmI);
                        }
                        if (NPC.life > NPC.lifeMax / 2f)
                        {
                            AttackTimer = 100;
                            MovementMode = 1;
                        }
                        else // when <50% hp run in air and for longer time
                        {
                            AttackTimer = 200;
                            MovementMode = 2;
                        }
                        break;

                    case 1:
                        AttackTimer = 40;
                        MovementMode = 0;

                        if (NPC.target != 255)
                        {
                            Vector2 BlinkLocation = target.Center + new Vector2(-200 * NPC.spriteDirection, -100);
                            if (SpaceForBlink(BlinkLocation))
                            {
                                NPC.Center = BlinkLocation;
                            }
                        }

                        NPC.velocity = target.Center - NPC.Center;// dash towards player
                        NPC.velocity.Normalize();
                        NPC.velocity *= 5;
                        break;
                        

                    case 2:
                        AttackTimer = Attack2Duration;
                        MovementMode = -1;
                        if (NPC.life < NPC.lifeMax / 2f)
                        {
                            NPC.noGravity = true;
                        }
                        break;

                    case 3:
                        AttackTimer = 300;
                        MovementMode = -1;
                        InfernalSpiritsSummoningCount = 3; 
                        if (NPC.life * 1f / NPC.lifeMax < 0.4f)
                        {
                            InfernalSpiritsSummoningCount = 5;
                        }
                        break;
                    case 4:
                        if (NPC.life > NPC.lifeMax / 2f)
                        {
                            AttackTimer = 200;
                            MovementMode = 1;
                        }
                        else // when <50% hp run in air and for longer time
                        {
                            AttackTimer = 300;
                            MovementMode = 2;
                        }
                        break;
                    case 5: // crosschop (3x?), when low health go invis and go dark
                        AttackTimer = 390;
                        MovementMode = 0;
                        OriginPos = NPC.Center;
                        NPC.noTileCollide = true;
                        NPC.noGravity = true;

                        break;
                }
            }

            switch (AttackMode) // Every Tick
            {
                case 0:
                    break;

                case 1:
                    NPC.velocity.Y *= 0.5f;
                    break;

                case 2:
                    break;

                case 3:
                    if (ticks % 5 == 0)
                    {
                        for (int i = 0; i < InfernalSpiritsSummoningCount; i++)
                        {
                            Vector2 pos = NPC.Center + new Vector2(16 * 35, 0).RotatedBy(i * MathHelper.TwoPi / InfernalSpiritsSummoningCount);
                            Dust.NewDust(pos, 30, 30, DustID.InfernoFork); // ok but not perfect
                        }
                    }


                    if (AttackTimer == 1 && Main.netMode != NetmodeID.MultiplayerClient) // Summon Spirits
                    {
                        for (int i = 0; i < InfernalSpiritsSummoningCount; i++) {
                            Vector2 pos = NPC.Center + new Vector2(16 * 35, 0).RotatedBy(i * MathHelper.TwoPi / InfernalSpiritsSummoningCount);
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)pos.X - 2 * 16, (int)pos.Y, ModContent.NPCType<InfernalSpirit>());
                        }
                    }
                    break;

                case 5:
                    if (AttackTimer % 90 == 0)
                    {
                        NPC.direction *= -1;
                        Vector2 BlinkLocation = target.Center + new Vector2(-1000 * NPC.direction, -600);
                        NPC.Center = BlinkLocation;
                    }
                    if (AttackTimer % 90 == 85)
                    {
                        Vector2 dashLocation = target.Center + (20 * target.velocity);
                        NPC.velocity = dashLocation - NPC.Center;// dash towards player
                        NPC.velocity.Normalize();
                        NPC.velocity *= 17;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), handPos + NPC.Center - 10 * NPC.velocity, NPC.velocity, ModContent.ProjectileType<HagethornRay>(), 60, 0);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), handPos + NPC.Center + 190 * NPC.velocity, -NPC.velocity, ModContent.ProjectileType<HagethornRay>(), 60, 0);

                            if (NPC.life < NPC.lifeMax / 2f)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), handPos + NPC.Center + 90 * NPC.velocity - 100 * NPC.velocity.RotatedBy(MathHelper.PiOver2), NPC.velocity.RotatedBy(MathHelper.PiOver2), ModContent.ProjectileType<HagethornRay>(), 60, 0);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), handPos + NPC.Center + 90 * NPC.velocity + 100 * NPC.velocity.RotatedBy(MathHelper.PiOver2), -NPC.velocity.RotatedBy(MathHelper.PiOver2), ModContent.ProjectileType<HagethornRay>(), 60, 0);
                            }
                        }
                    }

                    if (AttackTimer == 1)
                    {
                        NPC.Center = OriginPos; // return to initial position
                        NPC.noTileCollide = false;
                        NPC.noGravity = false;
                        if (!SpaceForBlink(OriginPos))
                        {
                            // todo: destroy blocks blocking teleport
                        }
                    }
                    break;
            }


            // Movement
            switch (MovementMode)
            {
                case -1:
                    Walking = false;
                    NPC.velocity.X *= FrictionX;
                    NPC.velocity.Y *= FrictionY;
                    break;
                case 0:
                    Walking = false;
                    NPC.velocity.Y *= FrictionY;
                    break;

                case 1:
                    // Travel
                    Walking = true;
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
                    break; 


                case 2: // walk in air
                    Walking = true;
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

                        switch(VerticalMoveMode)
                        {
                            case 0: // None
                                NPC.noGravity = true;
                                break;
                            case 1: // jumping
                                NPC.noGravity = false;
                                if (NPC.velocity.Y > 1f && (NPC.Center.Y - NPC.height / 2) % 16 > 11)
                                {
                                    NPC.velocity.Y = 0;
                                    NPC.noGravity = true;
                                    VerticalMoveMode = 0;
                                    NPC.position.Y += 16 - ((NPC.Center.Y - NPC.height / 2) % 16); // Changing position to match block grid
                                }
                                break;
                            case 2: // falling
                                NPC.noGravity = false;
                                if (NPC.velocity.Y > 6f && (NPC.Center.Y - NPC.height / 2) % 16 > 11)
                                {
                                    NPC.velocity.Y = 0;
                                    NPC.noGravity = true;
                                    VerticalMoveMode = 0;
                                    NPC.position.Y += 16 - ((NPC.Center.Y - NPC.height / 2) % 16); // Changing position to match block grid
                                }
                                break;
                        }

                        if (VerticalMoveCooldown <= 0)
                        {
                            if (TravelLocation.Y - NPC.Center.Y - NPC.height > 40) // should fall 
                            {
                                VerticalMoveMode = 2;
                                VerticalMoveCooldown = 60;
                            } else if (TravelLocation.Y - NPC.Center.Y + NPC.height < 40) { // should jump
                                NPC.velocity.Y -= 7;
                                VerticalMoveMode = 1;
                                VerticalMoveCooldown = 60;
                            }
                        }
                    }
                    VerticalMoveCooldown--;

                    // create falling block animation where he is walking, line up created blocks with real blocks
                    if (VerticalMoveMode == 0)
                    {
                        Point pos = (NPC.Center + new Vector2(0, NPC.height / 2)).ToTileCoordinates();
                        Dust.NewDust(pos.ToWorldCoordinates(), 0, 0, ModContent.DustType<YrimirFallingBlockDust>());
                    }

                    break;
            }
            NPC.spriteDirection = NPC.direction;

            // despawn code
            if (NPC.despawnEncouraged && NPC.timeLeft == 0)
            {
                NPC.active = false;
            }
            bool shoulddespawn = true;
            foreach (Player player in Main.player)
            {
                if (player.active && !player.dead)
                {
                    shoulddespawn = false;
                    break;
                }
            }
            if (shoulddespawn)
            {
                NPC.active = false;
            }
        }

        public Vector2 handPos = Vector2.Zero; // used in calcing sword pos

        private const int HeadFrame = 0;
        private int BodyFrame = 1; // 1-3
        private const int BackLegSkirtFrame = 4;
        private const int FrontLegSkirtFrame = 5;
        private const int BackLegFrame = 6;
        private const int LowerBackLegFrame = 7;
        private const int FrontLegFrame = 8;
        private const int LowerFrontLegFrame = 9;
        private const int BackArmFrame = 10;
        private const int BackForearmFrame = 11;
        private int FrontArmFrame = 12; // 12-13
        private const int FrontForearmFrame = 14;


        private float WalkAnimationProgress = 0;
        private bool Walking = true;
        private float WalkAnimationSpeed = 0.09f;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {



            // select animated frames
            FrontArmFrame = 12 + ((ticks / 30) % 2);
            BodyFrame = 1 + ((ticks / 10) % 3);


            // rotation in joints
            // Arms
            float BackShoulderRotation = 0.5f;
            float BackElbowRotation = 0f;

            float FrontShoulderRotation = 0.5f;
            float FrontElbowRotation = 0f;

            // Legs
            float FrontHipRotation = 0f;
            float FrontKneeRotation = 0f;

            float BackHipRotation = 0f;
            float BackKneeRotation = 0f;

            // Skirt - not sure if i want it to move
            float SkirtRotation = 0f;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }


            // logic

            // todo: when starting to walk from 0 velocity a animation is needed
            Vector2 skirtPosOffset = Vector2.Zero;
            if (Walking)
            {
                WalkAnimationProgress += WalkAnimationSpeed;

                FrontHipRotation = MathF.Sin(WalkAnimationProgress) * 0.8f + 0.2f;
                FrontKneeRotation = FrontHipRotation + MathF.Sin(WalkAnimationProgress - 2) - 1;

                BackHipRotation = MathF.Sin(WalkAnimationProgress + MathHelper.Pi) * 0.8f + 0.2f;
                BackKneeRotation = BackHipRotation + MathF.Sin(WalkAnimationProgress - 2 + MathHelper.Pi) - 1;
                skirtPosOffset.Y += MathF.Sin(WalkAnimationProgress * 2 - 0.6f) * 0.5f + 0.5f;
            }

            switch (AttackMode)
            {
                case 1:
                    // Arm Movement
                    FrontShoulderRotation = 0.7f - (AttackTimer * 1f / Attack2Duration);
                    FrontElbowRotation = 0.65f - (AttackTimer * 0.4f / Attack2Duration);
                    BackShoulderRotation = 0.65f - (AttackTimer * 1f / Attack2Duration);
                    BackElbowRotation = -0.6f - (AttackTimer * 0.4f / Attack2Duration);

                    // Dash Forwards
                    FrontHipRotation = 0.6f;
                    BackHipRotation = -0.5f;
                    FrontKneeRotation = 0.1f;
                    BackKneeRotation = -1.2f;
                    break;
                case 2:
                    FrontShoulderRotation = 0.1f + (AttackTimer * 0.3f / Attack2Duration); 
                    FrontElbowRotation = -0.5f + (AttackTimer * 0.65f / Attack2Duration);
                    BackShoulderRotation = 0.05f + (AttackTimer * 0.3f / Attack2Duration); 
                    BackElbowRotation = -0.45f + (AttackTimer * 0.65f / Attack2Duration);
                    break;
                case 3:
                    FrontShoulderRotation = -0.4f + MathUtils.LogisticFunction(300 - AttackTimer, 1.6f, 90);
                    FrontElbowRotation = -0.2f + MathUtils.LogisticFunction(300 - AttackTimer, 1.8f, 90);
                    BackShoulderRotation = -0.6f + MathUtils.LogisticFunction(300 - AttackTimer, 1.8f, 90);
                    BackElbowRotation = -0.18f + MathUtils.LogisticFunction(300 - AttackTimer, 1.9f, 90);
                    break;
                case 5:
                    FrontElbowRotation = -MathUtils.AngleBetween(new Vector2(0, -1), NPC.velocity) + 2.1f;
                    FrontShoulderRotation = FrontElbowRotation + 0.1f;
                    BackShoulderRotation = -1;

                    FrontHipRotation = 0.3f;
                    BackHipRotation = -0.5f;
                    FrontKneeRotation = -0.1f;
                    BackKneeRotation = -1.2f;
                    break;
            }



            if (spriteEffects == SpriteEffects.None) 
            {
                FrontHipRotation *= -1;
                BackHipRotation *= -1;
                FrontKneeRotation *= -1;
                BackKneeRotation *= -1;
                FrontShoulderRotation *= -1;
                FrontElbowRotation *= -1;
                BackShoulderRotation *= -1;
                BackElbowRotation *= -1;
            }

            // logic end



            Texture2D texture = TextureAssets.Npc[Type].Value;

            Vector2 bodyCenter = NPC.Center + new Vector2(0, NPC.gfxOffY) - Main.screenPosition;

            Vector2 hipPos = bodyCenter + new Vector2(-1 * NPC.spriteDirection, 15);
            Vector2 headPos = bodyCenter + new Vector2(1 * NPC.spriteDirection, -24);
            Vector2 shoulderPos = bodyCenter + new Vector2(-8 * NPC.spriteDirection, -12);
            Vector2 skirtPos = hipPos + new Vector2(2 * NPC.spriteDirection, 5) - skirtPosOffset;


            Vector2 backForearmPos = shoulderPos + new Vector2(2 * NPC.spriteDirection, 0) + new Vector2(6 * NPC.spriteDirection, 11).RotatedBy(BackShoulderRotation);
            Vector2 frontForearmPos = shoulderPos + new Vector2(6 * NPC.spriteDirection, 11).RotatedBy(FrontShoulderRotation);

            Vector2 backKneePos = hipPos + new Vector2(2 * NPC.spriteDirection, 0) + new Vector2(0, 17).RotatedBy(BackHipRotation);
            Vector2 frontKneePos = hipPos + new Vector2(0, 17).RotatedBy(FrontHipRotation);

            spriteBatch.Draw(texture, skirtPos + new Vector2(-2 * NPC.spriteDirection, -2), new Rectangle(0, BackLegSkirtFrame * 70, 70, 70)
               , drawColor, SkirtRotation, new Vector2(35, 35), Vector2.One, spriteEffects, 0); // Back Skirt

            spriteBatch.Draw(texture, backKneePos, new Rectangle(0, LowerBackLegFrame * 70, 70, 70)
                , drawColor, BackKneeRotation, new Vector2(35, 35), Vector2.One, spriteEffects, 0); // Back Calve
            spriteBatch.Draw(texture, hipPos + new Vector2(2 * NPC.spriteDirection, 0), new Rectangle(0, BackLegFrame * 70, 70, 70)
                , drawColor, BackHipRotation, new Vector2(35, 35), Vector2.One, spriteEffects, 0); // Back Thigh

            spriteBatch.Draw(texture, backForearmPos, new Rectangle(0, BackForearmFrame * 70, 70, 70)
                , drawColor, BackElbowRotation, new Vector2(35, 35), Vector2.One, spriteEffects, 0); // Back Forearm
            spriteBatch.Draw(texture, shoulderPos + new Vector2(2 * NPC.spriteDirection, 0), new Rectangle(0, BackArmFrame * 70, 70, 70)
                , drawColor, BackShoulderRotation, new Vector2(35, 35), Vector2.One, spriteEffects, 0); // Backarm

            spriteBatch.Draw(texture, bodyCenter, new Rectangle(0, BodyFrame * 70, 70, 70)
                , drawColor, 0, new Vector2(35, 35), Vector2.One, spriteEffects, 0); // Body
            spriteBatch.Draw(texture, headPos, new Rectangle(0, HeadFrame * 70, 70, 70)
                , drawColor, 0, new Vector2(35, 35), Vector2.One, spriteEffects, 0); // Head

            spriteBatch.Draw(texture, frontForearmPos, new Rectangle(0, FrontForearmFrame * 70, 70, 70)
                , drawColor, FrontElbowRotation, new Vector2(35, 35), Vector2.One, spriteEffects, 0); // Front Forearm
            spriteBatch.Draw(texture, shoulderPos, new Rectangle(0, FrontArmFrame * 70, 70, 70)
                , drawColor, FrontShoulderRotation, new Vector2(35, 35), Vector2.One, spriteEffects, 0); // Front Backarm
            handPos = frontForearmPos + new Vector2(14 * NPC.spriteDirection, 4).RotatedBy(FrontElbowRotation) - bodyCenter + new Vector2(0, NPC.gfxOffY);

            spriteBatch.Draw(texture, frontKneePos, new Rectangle(0, LowerFrontLegFrame * 70, 70, 70)
                , drawColor, FrontKneeRotation, new Vector2(35, 35), Vector2.One, spriteEffects, 0); // Front Calve
            spriteBatch.Draw(texture, hipPos, new Rectangle(0, FrontLegFrame * 70, 70, 70)
                , drawColor, FrontHipRotation, new Vector2(35, 35), Vector2.One, spriteEffects, 0); // Front Thigh

            spriteBatch.Draw(texture, skirtPos, new Rectangle(0, FrontLegSkirtFrame * 70, 70, 70)
               , drawColor, SkirtRotation, new Vector2(35, 35), Vector2.One, spriteEffects, 0); // Front Skirt

            return false;
        }


        private bool SpaceForBlink(Vector2 BlinkCenter)
        {
            Point TopLeftCorner = (BlinkCenter - new Vector2(NPC.width / 2f, NPC.height / 2f)).ToTileCoordinates();
            int width = (int)MathF.Ceiling(((BlinkCenter.X % 16) + NPC.width) / 16f);
            int height = (int)MathF.Ceiling(((BlinkCenter.Y % 16) + NPC.height) / 16f);

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++)
                {
                    Point pos = new Point(x, y) + TopLeftCorner;
                    if (Main.tile[pos].HasTile && Main.tile[pos].HasUnactuatedTile) {
                        return false;
                    }
                }
            }
            return true;
        }
    }


    public class YrimirsSoulBossBar : ModBossBar
    {
        private int bossHeadIndex = -1;

        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
        {
            // Display the previously assigned head index
            if (bossHeadIndex != -1)
            {
                return TextureAssets.NpcHeadBoss[bossHeadIndex];
            }
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams)
        {
            // Make the bar shake the less health the NPC has
            float lifePercent = drawParams.Life / drawParams.LifeMax;
            float shakeIntensity = Utils.Clamp(1f - lifePercent - 0.2f, 0f, 1f);
            drawParams.BarCenter.Y -= 20f;
            drawParams.BarCenter += Main.rand.NextVector2Circular(0.5f, 0.5f) * shakeIntensity * 15f;

            //drawParams.IconColor = Color.Red;

            return true;
        }

        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)
        {
            // Here the game wants to know if to draw the boss bar or not. Return false whenever the conditions don't apply.
            // If there is no possibility of returning false (or null) the bar will get drawn at times when it shouldn't, so write defensive code!

            NPC npc = Main.npc[info.npcIndexToAimAt];
            if (!npc.active)
                return false;

            // We assign bossHeadIndex here because we need to use it in GetIconTexture
            bossHeadIndex = npc.GetBossHeadTextureIndex();

            life = npc.life;
            lifeMax = npc.lifeMax;

            return true;
        }
    }
}

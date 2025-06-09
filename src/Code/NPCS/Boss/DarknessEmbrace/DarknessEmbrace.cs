using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Ancient.src.Code.Items.Materials.Bars;
using Ancient.src.Code.Items.Usables.Consumables;
using Ancient.src.Code.Tiles.Boss.Trophies;
using Ancient.src.Code.Tiles.Boss.Relics;
using Ancient.src.Common.Systems;
using Terraria.GameContent.UI.BigProgressBar;
using Ancient.src.Code.Projectiles.Boss.DarknessEmbrace;
using Ancient.src.Code.Projectiles;
using Microsoft.CodeAnalysis;
using Terraria.Audio;
using System.Security.Cryptography.X509Certificates;
using Terraria.Graphics.Effects;
using System.IO;
using Terraria.Graphics.Shaders;

namespace Ancient.src.Code.NPCS.Boss.DarknessEmbrace
{
    [AutoloadBossHead]
    internal class DarknessEmbrace : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5;

            // Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            // Automatically group with other bosses
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            // Specify the debuffs it is immune to. Most NPCs are immune to Confused.
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            // This boss also becomes immune to OnFire and all buffs that inherit OnFire immunity during the second half of the fight. See the ApplySecondStageBuffImmunities method.

            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.width = 70;
            NPC.height = 140;
            NPC.damage = 80;
            NPC.defense = 60;
            NPC.lifeMax = 95000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 15);
            // NPC.SpawnWithHigherTime(30); // no idea what this does
            NPC.boss = true;
            NPC.npcSlots = 6f; // Take up open spawn slots, preventing random NPCs from spawning during the fight

            // Default buff immunities should be set in SetStaticDefaults through the NPCID.Sets.ImmuneTo{X} arrays.
            // To dynamically adjust immunities of an active NPC, NPC.buffImmune[] can be changed in AI: NPC.buffImmune[BuffID.OnFire] = true;
            // This approach, however, will not preserve buff immunities. To preserve buff immunities, use the NPC.BecomeImmuneTo and NPC.ClearImmuneToBuffs methods instead, as shown in the ApplySecondStageBuffImmunities method below.

            // Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
            NPC.aiStyle = -1;

            // Custom boss bar
            NPC.BossBar = ModContent.GetInstance<DarknessEmbraceBossBar>();

            // The following code assigns a music track to the boss in a simple way.
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot("Ancient/src/Assets/Music/Darkness Embrace");
            }

            for (int i = 0; i < AttackPatternLength; i++)
            {
                AttackPattern[i] = 0;
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Sets the description of this NPC that is listed in the bestiary
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
                new FlavorTextBestiaryInfoElement("A chaotic amalgamation of all things considered evil held together by a metallic substance.")
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

            // Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<DarknessEmbraceBossBag>()));

            // Trophies are spawned with 1/10 chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DarknessEmbraceTrophy>(), 10));

            // ItemDropRule.MasterModeCommonDrop for the relic
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<DarknessEmbraceRelic>()));

            // ItemDropRule.MasterModeDropOnAllPlayers for the pet
            //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MinionBossPetItem>(), 4));

            // All our drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

            // Notice we use notExpertRule.OnSuccess instead of npcLoot.Add so it only applies in normal mode
            // Boss masks are spawned with 1/7 chance
            //notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MinionBossMask>(), 7)); // dont feel like making a mask :/

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<DarkSteelBar>(), minimumDropped: 16, maximumDropped: 24));

            // Finally add the leading rule
            npcLoot.Add(notExpertRule);
        }

        public override void OnSpawn(IEntitySource source)
        {
            InitAttackPattern();
        }

        public override void OnKill()
        {
            // This sets downedMinionBoss to true, and if it was false before, it initiates a lantern night
            NPC.SetEventFlagCleared(ref DownedBossSystem.DownedDarknessEmbrace, -1);
        }


        private void Despawn()
        {
            NPC.active = false;
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

        private bool AnimationDirection = true;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 4) // Adjust the frame speed
            {
                if (NPC.frame.Y > 3 * frameHeight) // Adjust the number of frames in your animation
                    AnimationDirection = false;
                if (NPC.frame.Y == 0) // Adjust the number of frames in your animation
                    AnimationDirection = true;
                if (AnimationDirection)
                {
                    NPC.frame.Y += frameHeight;
                } else
                {
                    NPC.frame.Y -= frameHeight;
                }
                NPC.frameCounter = 0;
            }
        }

        private static int AttackPatternLength = 32;
        private byte[] AttackPattern = new byte[AttackPatternLength];
        private static int NAttackModes = 5;

        public void InitAttackPattern()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.netUpdate = true;
                for (int i = 2; i < AttackPatternLength; i++)
                {
                    AttackPattern[i] = (byte)new Random().Next(0, NAttackModes);
                }
            }
        }


        public int AttackMode = 0; 
        private bool AttackOver = true;
        public float AttackTimer = 0;
        public Vector2 targetPos;
        private int AttackModePointer = 0;
        private int UtilCounter = 0;
        public override void AI()
        {
            int projectile_dmg = NPC.damage / 2;
            if (Main.expertMode)
            {
                projectile_dmg = (int)(NPC.damage / 2.5);
            }
            if (Main.masterMode)
            {
                projectile_dmg = (int)(NPC.damage / 3.3);
            }

            Player targeted_player = GetNearestPlayer(NPC);
            float cooldown_reduction_multiplier = 1f;
            if (Main.expertMode)
            {
                cooldown_reduction_multiplier += 0.5f;
            }
            if (Main.masterMode)
            {
                cooldown_reduction_multiplier += 0.3f;
            }
            if (!targeted_player.ZoneUnderworldHeight)
            {
                cooldown_reduction_multiplier += 4;
            }
            
            AttackTimer += (10 - (NPC.life * 10 / NPC.lifeMax)) * cooldown_reduction_multiplier / 10;

            if (AttackTimer > 0)
            {
                AttackOver = true;

                switch (AttackMode)
                {
                    case 0:
                        {
                            break;
                        }
                    case 1:
                        {
                            break;
                        }
                    case 2:
                        {
                            NPC.position = targetPos;
                            NPC.alpha = 0;
                            Lighting.AddLight(NPC.position, new Vector3(1, 0.5f, 2));
                            SoundEngine.PlaySound(SoundID.Item8, position: NPC.Center);
                            break;
                        }
                    case 3:
                        {
                            break;
                        }
                    case 4:
                        {
                            NPC.alpha = 0;
                            break;
                        }
                }
            }
            if (AttackOver)
            {
                AttackMode = AttackPattern[AttackModePointer];
                AttackModePointer = (AttackModePointer + 1) % AttackPattern.Length;
                AttackOver = false;
                switch (AttackMode)
                {
                    case 0:
                        {
                            AttackTimer = -90;
                            if (Main.netMode == NetmodeID.MultiplayerClient) { break; }
                            for (int i = 0; i < 4 + (AttackModePointer % 3); i++)
                            {
                                int rotation = i * 90;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2((float)Math.Cos(rotation) * 100,
                                    (float)Math.Sin(rotation) * 100), new Vector2((i % 5) * 3 - 7, (i % 5) * 3 - 7), ModContent.ProjectileType<VoidBolts>(), projectile_dmg, 0.2f);
                            }
                            break;
                        }
                    case 1:
                        {
                            AttackTimer = -200;
                            if (Main.netMode == NetmodeID.MultiplayerClient) { break; }
                            Vector2 projectile_velocity = targeted_player.Center - NPC.Center;
                            projectile_velocity.Normalize();
                            projectile_velocity *= 10;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projectile_velocity, ModContent.ProjectileType<DarkShackles>(), projectile_dmg, 0);
                            
                            break;
                        }
                    case 2:
                        {
                            AttackTimer -= 150;
                            targetPos = targeted_player.Center + new Vector2(-20, -170);
                            if (Main.netMode == NetmodeID.MultiplayerClient) { break; }
                            int projectile_count = 5;
                            for (int i = 0; i < projectile_count; i++)
                            {
                                float rotation = i * (float)Math.PI * 2 / projectile_count;
                                Vector2 offset = new Vector2(MathF.Cos(rotation), MathF.Sin(rotation));
                                offset *= 80;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset, Vector2.Zero, ModContent.ProjectileType<GhastlyDagger>(), projectile_dmg, 0.2f);
                                
                            }
                            break;
                        }
                    case 3:
                        {
                            AttackTimer = -100;
                            if (Main.netMode == NetmodeID.MultiplayerClient) { break; }
                            int projectile_count = 20;
                            for (int i = 0; i < projectile_count; i++)
                            {
                                float rotation = i * (float)Math.PI * 2 / projectile_count;
                                Vector2 offset = new Vector2(MathF.Cos(rotation), MathF.Sin(rotation));
                                offset *= 900;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset, Vector2.Zero, ModContent.ProjectileType<GhastlyDagger>(), projectile_dmg, 0.2f);
                                
                            }

                            break;
                        }
                    case 4:
                        {
                            if (Main.netMode == NetmodeID.MultiplayerClient) { break; }
                            AttackTimer = -100;
                            NPC.alpha = 100;

                            break;
                        }
                }
            }

            switch (AttackMode)
            {
                case 0:
                    {
                        
                        break;
                    }
                case 1:
                    {
                        
                        break;
                    }
                case 2:
                    {
                        NPC.alpha += 2;

                        break;
                    }
                case 3:
                    {

                        break;
                    }
                case 4:
                    {
                        if ((int)MathF.Round(AttackTimer%30)==0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.position += new Vector2(400 * MathF.Sin(UtilCounter/10), 0);
                            UtilCounter++;

                            Lighting.AddLight(NPC.position, new Vector3(1, 0.5f, 2));
                            SoundEngine.PlaySound(SoundID.Item8, position: NPC.Center);

                            NPC.netUpdate = true;
                        }
                        break;
                    }
            }

            AttackTimer++;

            // Check if all players are dead
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
                Despawn();
            }
        }


        // values to be transfered: AttackMode
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AttackPattern);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AttackPattern = reader.ReadBytes(32);
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

    public class DarknessEmbraceBossBar : ModBossBar
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

    public class DarknessEmbraceShaderData : ScreenShaderData
    {
        private int DarknessEmbraceIndex;

        public DarknessEmbraceShaderData(Asset<Effect> shader, string passName) : base(shader, passName)
        {
        }

        private void UpdateDarknessEmbraceIndex()
        {
            if (DarknessEmbraceIndex >= 0 && Main.npc[DarknessEmbraceIndex].active && Main.npc[DarknessEmbraceIndex].type == ModContent.NPCType<DarknessEmbrace>())
            {
                return;
            }
            DarknessEmbraceIndex = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<DarknessEmbrace>())
                {
                    DarknessEmbraceIndex = i;
                    break;
                }
            }
        }

        public override void Apply()
        {
            UpdateDarknessEmbraceIndex();
            if (DarknessEmbraceIndex == -1)
            {
                UseIntensity(0);
                base.Apply();
                return;
            }
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            UseTargetPosition(Main.npc[DarknessEmbraceIndex].Center - Main.screenPosition + zero);
            UseIntensity(1);
            base.Apply();
        }
    }

    public class DarknessEmbracePlayer : ModPlayer
    {
        public override void OnEnterWorld()
        {
            Terraria.Graphics.Effects.Filters.Scene.Activate("DarknessEmbrace");
        }
        // Terraria.Graphics.Effects.Filters.Scene["DarknessEmbrace"].Deactivate();
    }
}

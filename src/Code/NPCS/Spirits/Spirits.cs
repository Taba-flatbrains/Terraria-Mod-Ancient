using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader.Utilities;
using Ancient.src.Common.Structures;
using System.Diagnostics.Metrics;
using Microsoft.Xna.Framework;
using Ancient.src.Code.Items.Materials;
using Terraria.GameContent.ItemDropRules;
using Ancient.src.Code.Items.Materials.AstralBiome;
using Terraria.Audio;

namespace Ancient.src.Code.NPCS.Spirits
{
    internal abstract class SpiritBase : ModNPC
    {
        public virtual bool friendly => false;
        public virtual float spawnRateMultiplier => 1;

        public virtual Vector3 LightColor => new Vector3(0.1f, 0, 0.5f);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                // Influences how the NPC looks in the Bestiary
                Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.friendly = friendly;

            NPC.width = 30;
            NPC.height = 30;
            NPC.damage = 200;
            NPC.defense = 50;
            NPC.lifeMax = 20000;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath7;
            NPC.value = 5000f;
            NPC.knockBackResist = 0.5f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;

            AIType = NPCID.Pixie;
            NPC.aiStyle = NPCAIStyleID.HoveringFighter;
            AnimationType = NPCID.Pixie; // Use vanilla zombie's type when executing animation code. Important to also match Main.npcFrameCount[NPC.type] in SetStaticDefaults.
                                         //Banner = Item.NPCtoBanner(NPCID.Zombie); // <- to be replaced with own banner
                                         //BannerItem = Item.BannerToItem(Banner);

            SpawnModBiomes = new int[] { ModContent.GetInstance<AstralBiome>().Type };
            NPC.alpha = 100;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Formless energy from the Astral realm."),
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<AstralBiome>())
            {
                return (SpawnCondition.Cavern.Chance + SpawnCondition.Overworld.Chance + SpawnCondition.Underground.Chance) * 0.7f * spawnRateMultiplier;  
            }
            return 0f;
        }

        private int ai_ticks = 0;
        private static readonly int TicksTillBehaviorSwap = 60 * 13;
        public override bool PreAI()
        {
            Lighting.AddLight(NPC.position, LightColor);
            ai_ticks++;

            if (ai_ticks == 60 * 30)
            {
                NPC.EncourageDespawn(60 * 20);
            }
            if (ai_ticks == 60 * 60 && friendly)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath7, NPC.position);
                NPC.active = false;
            }

            if (ai_ticks % (2 * TicksTillBehaviorSwap) < TicksTillBehaviorSwap || (friendly && Vector2.Distance(NPC.position, GetNearestPlayer(NPC).position) < 300))
            {
                IdleAI();
                return false;
            }
            return true;
        }

        public Vector2 direction = new Vector2(0, 0);
        public static float MaxVelocity = 0.6f;
        public int ticksTillDirectionChange = 0;
        private int counter = 0;
        private void IdleAI()
        {
            Player nearestPlayer = GetNearestPlayer(NPC);
            NPC.rotation += 0.05f;
            NPC.velocity = direction * new Vector2((float)Math.Sin(ticksTillDirectionChange / 10) + 2f, (float)Math.Cos(ticksTillDirectionChange / 10) + 2f);
            NPC.velocity /= 2;
            if (ticksTillDirectionChange == 0)
            {
                ticksTillDirectionChange = 300;
                if (counter % 5 == 0)
                {
                    direction = new Vector2(MathF.Sin(nearestPlayer.position.X - NPC.position.X + counter), MathF.Sin(nearestPlayer.position.Y - NPC.position.Y));
                }
                else
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

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulOfTwilight>(), 2, 1, 1));
        }
    }

    internal class VodooSpirit : SpiritBase
    {
        public override float spawnRateMultiplier => 1f;
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Venom, 60 * 5);
        }
    }

    internal class MischievousSpirit : SpiritBase // gets revealed by goggles of true sight
    {
        public override float spawnRateMultiplier => 0.3f;
        public override Vector3 LightColor => Vector3.Zero;

        public override bool PreAI()
        {
            NPC.alpha = (int)(205 * Main.LocalPlayer.GetModPlayer<AstralBiomeShaderPlayer>().AstralBiomeShaderIntensityMultiplier)+50;
            Lighting.AddLight(NPC.position, new Vector3(1.5f, 1.5f, 0)*(1-Main.LocalPlayer.GetModPlayer<AstralBiomeShaderPlayer>().AstralBiomeShaderIntensityMultiplier));

            return base.PreAI();
        }
    }

    internal class FriendlySpirit : SpiritBase
    {
        public override bool friendly => true;
        public override float spawnRateMultiplier => 0.5f;

        public override Vector3 LightColor => new(0.65f, 1.2f, 0.65f);
    }
}

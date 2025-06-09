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
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace Ancient.src.Code.NPCS.Hostile
{
    internal class ExplosiveRat : ModNPC
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
            NPC.width = 10;
            NPC.height = 10;
            NPC.damage = 60;
            NPC.defense = 0;
            NPC.lifeMax = 40;
            NPC.HitSound = SoundID.NPCHit27;
            NPC.DeathSound = SoundID.NPCDeath48;
            NPC.value = 250f;
            NPC.knockBackResist = 0.2f;
            NPC.aiStyle = NPCAIStyleID.Herpling; // Fighter AI, important to choose the aiStyle that matches the NPCID that we want to mimic

            AIType = NPCID.Herpling; // Use vanilla zombie's type when executing AI code. (This also means it will try to despawn during daytime)
            AnimationType = NPCID.Bunny; // Use vanilla zombie's type when executing animation code. Important to also match Main.npcFrameCount[NPC.type] in SetStaticDefaults.
                                          //Banner = Item.NPCtoBanner(NPCID.Zombie); // <- to be replaced with own banner
                                          //BannerItem = Item.BannerToItem(Banner);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("cat food"),
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneJungle)
            {
                return (SpawnCondition.SurfaceJungle.Chance + SpawnCondition.UndergroundJungle.Chance) * 1.2f;  // pretty annoying has to spawn rather rarely
            }
            return 0f;
        }

        public override void DrawEffects(ref Color drawColor)
        {
            if (new Random().Next(8)==0)
            {
                Dust.NewDust(new Vector2(NPC.position.X + 7 * NPC.direction, NPC.position.Y - 5), 1, 1, DustID.Torch);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.npc[hurtInfo.DamageSource.SourceNPCIndex].ModNPC is ExplosiveRat)
            {
                NPC rat = Main.npc[hurtInfo.DamageSource.SourceNPCIndex];
                rat.active = false;
                for (int k = 0; k < 40; k++)
                {
                    Dust.NewDust(rat.position,
                        1,1, DustID.InfernoFork
                    );
                }
                
                if (!Main.expertMode)
                {
                    return;
                }
                SoundEngine.PlaySound(SoundID.Item62, position: rat.position);
                int explosion_radius = 5;
                for (int i = 0; i < explosion_radius*2-1; i++)
                {
                    for (int j = 0; j < explosion_radius * 2 - 1; j++)
                    {
                        bool a = false;
                        if (Microsoft.Xna.Framework.Vector2.Distance(new Microsoft.Xna.Framework.Vector2(explosion_radius, explosion_radius), new Microsoft.Xna.Framework.Vector2(i, j)) < explosion_radius
                             && TileLoader.CanExplode(i, j) && TileLoader.CanKillTile(i, j, Main.tile[i, j].TileType, ref a))
                        {
                            WorldGen.KillTile((int)rat.position.X / 16 + i - explosion_radius, (int)rat.position.Y / 16 + j - explosion_radius);  // kills tile in center of explosion
                        }
                    }
                }
            }
        }
    }
}

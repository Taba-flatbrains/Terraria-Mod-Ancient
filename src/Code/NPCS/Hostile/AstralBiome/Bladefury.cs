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

namespace Ancient.src.Code.NPCS.Hostile.AstralBiome
{
    internal class Bladefury : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5;

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
            NPC.height = 40;
            NPC.damage = 200;
            NPC.defense = 40;
            NPC.lifeMax = 20000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.value = 4000f;
            NPC.aiStyle = NPCAIStyleID.DungeonSpirit; // custom ai
                                                //Banner = Item.NPCtoBanner(NPCID.Zombie); // <- to be replaced with own banner
                                                //BannerItem = Item.BannerToItem(Banner); 
            NPC.knockBackResist = 0.05f;
            NPC.noGravity = true;

            SpawnModBiomes = new int[] { ModContent.GetInstance<Common.Structures.AstralBiome>().Type };
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<Common.Structures.AstralBiome>())
            {
                return (SpawnCondition.Cavern.Chance + SpawnCondition.Overworld.Chance + SpawnCondition.Underground.Chance) * 0.5f;
            }
            return 0f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Fragments of a long lost blade. They turn you to a pile of flesh in a matter of seconds."),
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 5) 
            {
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= 5 * frameHeight) // Adjust the number of frames in your animation
                    NPC.frame.Y = 0;
                NPC.frameCounter = 0;
            }
        }

        private int ticks = 0;
        public override void PostAI()
        {
            ticks++;
            NPC.rotation = ticks / 4f;
        }
    }
}

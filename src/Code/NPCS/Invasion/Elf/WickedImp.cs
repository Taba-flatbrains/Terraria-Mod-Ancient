using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Ancient.src.Common.Events;

namespace Ancient.src.Code.NPCS.Invasion.Elf
{
    internal class WickedImp : ModNPC
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
            NPC.width = 28;
            NPC.height = 42;
            NPC.damage = 70;
            NPC.defense = 20;
            NPC.lifeMax = 3000;
            NPC.value = 0f;

            NPC.aiStyle = NPCAIStyleID.Fighter; // custom ai
                                                //Banner = Item.NPCtoBanner(NPCID.Zombie); // <- to be replaced with own banner
                                                //BannerItem = Item.BannerToItem(Banner);
            AIType = NPCID.Fritz;

            NPC.HitSound = SoundID.NPCHit7;
            NPC.DeathSound = SoundID.NPCDeath5;
            NPC.knockBackResist = 0.3f;

            NPC.npcSlots = 0.2f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
			// Sets the spawning conditions of this NPC that is listed in the bestiary.
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

			// Sets the description of this NPC that is listed in the bestiary.
			new FlavorTextBestiaryInfoElement("He is indeed a sussy baka."),
        });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction * -1;
            NPC.frameCounter++;
            if (NPC.frameCounter >= 4) // Adjust the frame speed
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
                if (NPC.frame.Y >= 5 * frameHeight)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override void AI()
        {
            if (ElfInvasion.Ongoing)
            {
                NPC.timeLeft = 2;
                NPC.despawnEncouraged = false;
                NPC.TargetClosest();
            }
        }

        public override void PostAI()
        {
            if (ElfInvasion.Ongoing)
            {
                NPC.timeLeft = 2;
                NPC.despawnEncouraged = false;
            }
        }
    }
}

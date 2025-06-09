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
using Microsoft.Xna.Framework;

namespace Ancient.src.Code.NPCS.Hostile.Tentacles
{
    internal abstract class TentacleBase : ModNPC
    {
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
            NPC.width = 20;
            NPC.height = 80;
            NPC.damage = 50;
            NPC.defense = 20;
            NPC.lifeMax = 400;
            NPC.HitSound = SoundID.NPCHit19;
            NPC.DeathSound = SoundID.NPCDeath22;
            NPC.value = 350f;
            NPC.aiStyle = -1; // custom ai
                              //Banner = Item.NPCtoBanner(NPCID.Zombie); // <- to be replaced with own banner
                              //BannerItem = Item.BannerToItem(Banner); 
            NPC.knockBackResist = 0;
        }

        public override void AI()
        {
            Visuals();
            if (new Random().Next(5)==0) 
            {
                if (NPC.type==ModContent.NPCType<BloodyTentacle>())
                {
                    Dust.NewDust(NPC.position, 1, 1, TileID.CrimsonPlants, newColor: new Color(110, 20, 20));
                }
                if (NPC.type == ModContent.NPCType<RottenTentacle>())
                {
                    Dust.NewDust(NPC.position, 1, 1, TileID.CrimsonPlants, newColor: new Color(100, 10, 120));
                }
            }
        }

        private void Visuals()
        {
            // Some visuals here
            Lighting.AddLight(NPC.Center, Color.Purple.ToVector3() * 0.2f);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 20) // Adjust the frame speed
            {
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 3 * frameHeight) // Adjust the number of frames in your animation
                    NPC.frame.Y = 0;
                NPC.frameCounter = 0;
            }
        }
    }

    internal class RottenTentacle : TentacleBase
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Disgusting Tentacle emerging from the ground."),
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCorrupt)
            {
                return SpawnCondition.Corruption.Chance * 1f;
            }

            return 0f;
        }
    }

    internal class BloodyTentacle : TentacleBase
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Disgusting Tentacle emerging from the ground."),
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCrimson)
            {
                return SpawnCondition.Crimson.Chance * 1f;
            }
            return 0f;
        }
    }
}

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
using Ancient.src.Common;
using Ancient.src.Code.Tiles.AstralBiome;
using Microsoft.Xna.Framework;
using System.Net.Mail;

namespace Ancient.src.Code.NPCS.Hostile.AstralBiome
{
    internal class CrystalBeast : ModNPC
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
            NPC.width = 68;
            NPC.height = 54;
            NPC.damage = 100;
            NPC.defense = 80;
            NPC.lifeMax = 40000;
            NPC.HitSound = SoundID.DD2_WitherBeastHurt;
            NPC.DeathSound = SoundID.DD2_WitherBeastDeath;
            NPC.value = 4000f;
            NPC.aiStyle = NPCAIStyleID.Fighter; // custom ai
                                                //Banner = Item.NPCtoBanner(NPCID.Zombie); // <- to be replaced with own banner
                                                //BannerItem = Item.BannerToItem(Banner); 
            NPC.knockBackResist = 0.05f;
            AnimationType = NPCID.DD2WitherBeastT2;

            SpawnModBiomes = new int[] { ModContent.GetInstance<Common.Structures.AstralBiome>().Type };
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<Common.Structures.AstralBiome>())
            {
                return (SpawnCondition.Cavern.Chance + SpawnCondition.Overworld.Chance + SpawnCondition.Underground.Chance) * 0.3f;
            }
            return 0f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Powerful beast with sharp crystals on its back. Fighting it should be avoided. "),
            });
        }

        private static readonly int[] DestroyableBlocks = new int[] 
        {
            TileID.Stone,
            TileID.Dirt,
            ModContent.TileType<AstralStone>(),
            ModContent.TileType<AstralStone2>(),
            ModContent.TileType<AstralStone3>(),
            ModContent.TileType < AstralDirt >(),
            TileID.WoodBlock,
            TileID.Mud,
            TileID.Sand,
            TileID.Sandstone,
            TileID.Ebonstone,
            TileID.Ebonsand,
            TileID.Crimsand,
            TileID.Crimstone
        };
        private static readonly Point[] BlockDestroyOffsetsY = new Point[]
        {
            Point.Zero,
            new Point(0, -1),
            new Point(0, 1),
        };
        private static readonly Point[] BlockDestroyOffsetX = new Point[]
        {
            Point.Zero,
            new Point(1, 0),
            new Point(-1, 0)
        };
        public override bool PreAI()
        {
            // destroy blocks in front of it
            Point TileCoordinateCenter = NPC.Center.ToTileCoordinates();
            TileCoordinateCenter += new Point(2 * NPC.direction, 0);

            foreach (Point DestroyOffsetY in BlockDestroyOffsetsY)
            {
                foreach (Point DestroyOffsetX in BlockDestroyOffsetX)
                {
                    Point DestroyOffset = DestroyOffsetY + DestroyOffsetX;
                    if (DestroyableBlocks.Contains(Main.tile[TileCoordinateCenter + DestroyOffset].TileType))
                    {
                        WorldGen.KillTile(TileCoordinateCenter.X + DestroyOffset.X, TileCoordinateCenter.Y + DestroyOffset.Y);
                    }
                }
            }

            return true;
        }
    }
}

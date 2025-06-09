using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;
using Ancient.src.Code.Walls;
using Ancient.src.Code.Tiles.OceanTemple;
using Ancient.src.Code.Tiles;
using Microsoft.CodeAnalysis;
using Ancient.src.Code.Items.Materials.Bars;
using Ancient.src.Code.Items.Usables.Weapons.OceanTemple;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ancient.src.Code.Buffs;
using Ancient.src.Code.Items.ReforgeStones;
using Ancient.src.Code.Items.Materials;

namespace Ancient.src.Common.Structures
{
    internal class OceanTemple : ModSystem
    {
        // useful recources: https://hackmd.io/@tModLoader/HJUiVKXzu
        // https://forums.terraria.org/index.php?threads/modding-tutorial-world-generation.47601/
        // https://github.com/tModLoader/tModLoader/wiki/Vanilla-World-Generation-Steps
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            // 4. We use FindIndex to locate the index of the vanilla world generation task called "Underground". This ensures our code runs at the correct step.
            int GenIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Create Ocean Caves"));
            if (GenIndex != -1)
            {
                // 5. We register our world generation pass by passing in a name and the method that will execute our world generation code.	
                tasks.Insert(GenIndex + 1, new PassLegacy("Create Ocean Temple", WorldGenOceanTemple));
            }
        }

        public static int DistFromWorldBorder = 150;
        private void WorldGenOceanTemple(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating Ocean Temple";
            
            bool left = WorldGen.genRand.NextBool();
            Point origin;
            if (left)
            {
                origin = new(Main.maxTilesX-DistFromWorldBorder, (int)Main.worldSurface);
            } else
            {
                origin = new(DistFromWorldBorder, (int)Main.worldSurface);
            }
            PlaceOceanTemple(origin);
        }

        public static readonly int TunnelingIterations = 30;

        public static readonly int BaseStorieWidth = 26;
        public static readonly int BaseStorieHeight = 18;
        public static readonly int WallStrength = 4;
        public static readonly int SizeIncrement = 3;
        public static readonly int StorieCount = 8;
        private void PlaceOceanTemple(Point origin)
        {
            bool foundSurface = WorldUtils.Find(origin, Searches.Chain(new Searches.Up(1000),
                        new Conditions.IsSolid().AreaOr(1, 50).Not()), out Point location); // find surface
            if (!foundSurface) { return; }
            location += new Point(0, 49);

            for (int i = 0; i < TunnelingIterations; i++) // cut entry
            {
                WorldUtils.Gen(location, new Shapes.Rectangle(WorldGen.genRand.Next(5, 10), WorldGen.genRand.Next(5, 10)), new Actions.Clear());
                location += new Point(WorldGen.genRand.Next(-3, 4), WorldGen.genRand.Next(-2 , 7)); // tries to wander underground
            }

            for (int i = 0;i < StorieCount;i++)
            {
                // creating rooms
                int TotalStorieWidth = BaseStorieWidth + (i * SizeIncrement * 2);
                WorldUtils.Gen(location, new Shapes.Rectangle(TotalStorieWidth, BaseStorieHeight), new Actions.Clear());
                WorldUtils.Gen(location, new Shapes.Rectangle(TotalStorieWidth, BaseStorieHeight), new Actions.PlaceTile((ushort)ModContent.TileType<OceanTempleBlock>()));
                WorldUtils.Gen(location + new Point(WallStrength, WallStrength), new Shapes.Rectangle(TotalStorieWidth - (WallStrength * 2), BaseStorieHeight - (WallStrength * 2)), new Actions.Clear());
                WorldUtils.Gen(location + new Point(WallStrength, WallStrength), new Shapes.Rectangle(TotalStorieWidth - (WallStrength*2), BaseStorieHeight - (WallStrength*2)), new Actions.PlaceWall((ushort)ModContent.WallType<OceanTempleWall>()));

                // generating holes (on top of storie)
                int holePosition = WorldGen.genRand.Next(3) * (BaseStorieWidth + (i * SizeIncrement * 2) - 8 - SizeIncrement * 2 - WallStrength) / 2 + WallStrength + SizeIncrement; // 0: left, 1: mid, 2:right
                WorldUtils.Gen(location + new Point(holePosition, 0), new Shapes.Rectangle(4, WallStrength), new Actions.Clear());
                WorldUtils.Gen(location + new Point(holePosition, 0), new Shapes.Rectangle(4, 1), new Actions.PlaceTile(TileID.Platforms));
                WorldUtils.Gen(location + new Point(holePosition, 0), new Shapes.Rectangle(4, WallStrength), new Actions.PlaceWall((ushort)ModContent.WallType<OceanTempleWall>()));

                // place two ocean temple crystals on each side
                WorldGen.PlaceTile(location.X + 3 + WallStrength, location.Y + 3 + WallStrength, ModContent.TileType<OceanTempleCrystal>());
                WorldGen.PlaceTile(location.X - 3 + TotalStorieWidth - WallStrength, location.Y + 3 + WallStrength, ModContent.TileType<OceanTempleCrystal>());

                if (i > StorieCount - 4)
                {
                    PlaceOceanTempleChest(location.X + WorldGen.genRand.Next(WallStrength, TotalStorieWidth - 3 - WallStrength), location.Y + BaseStorieHeight - 1 - WallStrength);
                }

                location += new Point(-SizeIncrement, BaseStorieHeight-WallStrength);
            }
        }

        private void PlaceOceanTempleChest(int x, int y)
        {
            List<int> loot = new();
            List<int> lootStackSize = new();


            // rare loot
            List<int> specialLootList = new() 
            {
                ModContent.ItemType<Butterfly>(),
                ItemID.Shellphone,
                ModContent.ItemType<AncientClub>()
            };

            if (!WorldGen.genRand.NextBool(3))
            {
                loot.Add(specialLootList[WorldGen.genRand.Next(specialLootList.Count)]);
                lootStackSize.Add(1);
            }

            // fish
            List<int> fishList = new()
            {
                ItemID.ArmoredCavefish,
                ItemID.AtlanticCod,
                ItemID.Bass,
                ItemID.ChaosFish,
                ItemID.CrimsonTigerfish,
                ItemID.Damselfish,
                ItemID.DoubleCod,
                ItemID.Ebonkoi,
                ItemID.FlarefinKoi,
                ItemID.Flounder,
                ItemID.FrostMinnow,
                ItemID.Hemopiranha,
                ItemID.Honeyfin,
                ItemID.NeonTetra,
                ItemID.Obsidifish,
                ItemID.PrincessFish,
                ItemID.Prismite,
                ItemID.RedSnapper,
                ItemID.RockLobster,
                ItemID.Salmon,
                ItemID.Shrimp,
                ItemID.SpecularFish,
                ItemID.Trout,
                ItemID.Tuna,
                ItemID.VariegatedLardfish
            };
            for (int i = 0; i < 2; i++)
            {
                if (!WorldGen.genRand.NextBool(3))
                {
                    loot.Add(fishList[WorldGen.genRand.Next(0, fishList.Count)]);
                    lootStackSize.Add(WorldGen.genRand.Next(1, 3));
                }
            }

            if (WorldGen.genRand.NextBool(2))
            {
                loot.Add(ModContent.ItemType<MarvelScale>());
                lootStackSize.Add(WorldGen.genRand.Next(3, 5));
            }

            // common loot
            if (WorldGen.genRand.NextBool(2)) // healing
            {
                loot.Add(ItemID.SuperHealingPotion);
                lootStackSize.Add(WorldGen.genRand.Next(3, 5));
            }
            if (WorldGen.genRand.NextBool(4)) // torches
            {
                loot.Add(ItemID.Torch);
                lootStackSize.Add(WorldGen.genRand.Next(10, 20));
            }
            if (WorldGen.genRand.NextBool(2)) // money
            {
                loot.Add(ItemID.GoldCoin);
                lootStackSize.Add(WorldGen.genRand.Next(2, 8));
            }
            if (WorldGen.genRand.NextBool(3)) // dynamite
            {
                loot.Add(ItemID.Dynamite);
                lootStackSize.Add(WorldGen.genRand.Next(2, 8));
            }
            if (WorldGen.genRand.NextBool(3)) // bars
            {
                if (WorldGen.genRand.NextBool(2))
                {
                    loot.Add(ItemID.LunarBar);
                } else
                {
                    loot.Add(ModContent.ItemType<DarkSteelBar>());
                }
                lootStackSize.Add(WorldGen.genRand.Next(3, 5));
            }

            List<int> ammoList = new() // arrows / bullets
            {
                    3567,
                    ItemID.ChlorophyteBullet,
                    ItemID.HighVelocityBullet,
                    ItemID.NanoBullet,
                    3568,
                    ItemID.ChlorophyteArrow,
                    ItemID.HolyArrow,
                    ItemID.RocketIII
            };
            if (WorldGen.genRand.NextBool(2)) 
            {
                loot.Add(ammoList[WorldGen.genRand.Next(0, ammoList.Count)]);
                lootStackSize.Add(WorldGen.genRand.Next(40, 120));
            }

            if (WorldGen.genRand.NextBool(3)) // bait
            {
                loot.Add(ItemID.MasterBait);
                lootStackSize.Add(WorldGen.genRand.Next(1, 3));
            }


            int chestIndex = WorldGen.PlaceChest(x, y, style: 17);
            if (chestIndex == -1)
            {
                return;
            }

            Chest oceanTempleChest = Main.chest[chestIndex];

            for (int itemIndex = 0; itemIndex < loot.Count; itemIndex++)
            {
                oceanTempleChest.item[itemIndex].SetDefaults(loot[itemIndex]);
                oceanTempleChest.item[itemIndex].stack = lootStackSize[itemIndex];
            }
        }
    }

    public class OceanTempleBlockCount : ModSystem
    {
        public int BlockCount;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            BlockCount = tileCounts[ModContent.TileType<OceanTempleBlock>()];
        }
    }

    public class OceanTempleBiome : ModBiome // i dont think this structure deserves its own background
    {
        public override bool IsBiomeActive(Player player)
        {
            return StaticIsBiomeActive(player);
        }

        public static bool StaticIsBiomeActive(Player player)
        {
            return ModContent.GetInstance<OceanTempleBlockCount>().BlockCount >= 60 && Main.tile[(player.position / 16).ToPoint16()].WallType == ModContent.WallType<OceanTempleWall>();
        }

        public static bool StaticNearBiome(Player player)
        {
            return ModContent.GetInstance<OceanTempleBlockCount>().BlockCount >= 60;
        }
    }

    public class OceanTempleEffectsPlayer : ModPlayer
    {
        public override void PreUpdateBuffs()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            if (OceanTempleBiome.StaticNearBiome(Player))
            {
                Player.AddBuff(ModContent.BuffType<NoTeleportationDebuff>(), 2);
            }
        }
    }
}

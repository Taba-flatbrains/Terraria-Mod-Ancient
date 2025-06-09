using Ancient.src.Code.Items.ReforgeStones;
using Ancient.src.Code.Items.Scrolls;
using Ancient.src.Code.Tiles;
using Ancient.src.Code.Walls;
using Ancient.src.Common.Structures.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.WorldBuilding;

namespace Ancient.src.Common.Structures
{
    // useful recources: https://hackmd.io/@tModLoader/HJUiVKXzu
    // https://forums.terraria.org/index.php?threads/modding-tutorial-world-generation.47601/
    // https://github.com/tModLoader/tModLoader/wiki/Vanilla-World-Generation-Steps

    internal class AncientRuins : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            // 4. We use FindIndex to locate the index of the vanilla world generation task called "Underground". This ensures our code runs at the correct step.
            int GenIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Buried Chests"));
            if (GenIndex != -1)
            {
                // 5. We register our world generation pass by passing in a name and the method that will execute our world generation code.	
                tasks.Insert(GenIndex + 1, new PassLegacy("Ancient Ruins", WorldGenAncientRuins));
            }
        }

        private void WorldGenAncientRuins(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating Structures .. Ancient Ruins";
            for (int k=0; k<30; k++)
            {
                for (int tries = 0; tries < 100; tries++)
                {
                    Point origin = new(WorldGen.genRand.Next(100, Main.maxTilesX-100), WorldGen.genRand.Next((int)Main.rockLayer, Main.UnderworldLayer));
                    Point location;
                    bool foundSuitableLocation = WorldUtils.Find(origin, Searches.Chain(new Searches.Rectangle(50, 50), 
                        new Conditions.IsSolid().AreaOr(20, 20).Not(), new WorldGenConditionHasShimmer().AreaOr(100, 100).Not(),
                        new WorldGenConditionHasDungeonBricks().AreaOr(50,50).Not()), out location);
                    if (!foundSuitableLocation)
                    {
                        continue;
                    }



                    if (PlaceAncientRuin(location))
                    {
                        break;
                    }
                }
            }
        }

        static readonly int MinStorieHeight = 8;
        static readonly int MinStorieWidth = 22;
        

        public bool PlaceAncientRuin(Point location)
        {
            Dictionary<ushort, int> tileDictionary = new Dictionary<ushort, int>();
            WorldUtils.Gen(new Point(location.X - 25, location.Y - 25), new Shapes.Rectangle(50, 50), new Actions.TileScanner(TileID.Dirt, TileID.Stone, TileID.Sand).Output(tileDictionary));
            if (tileDictionary[TileID.Dirt] + tileDictionary[TileID.Stone] + tileDictionary[TileID.Sand] < 1250)
                return false; // If not, return false, which will cause the calling method to attempt a different origin

            
            for (int building = 0; building < 3; building++)
            {
                int storieXOffset;
                int oldStorieXOffset = 0;
                int oldStorieWidth = 0;
                int storieYOffset = 0;
                for (int storie = 0; storie < 5; storie++)
                {
                    int storieWidth = WorldGen.genRand.Next(0, 12 - (building * 3));
                    storieXOffset = WorldGen.genRand.Next(0, 12);
                    int storieHeight = WorldGen.genRand.Next(0, 2);
                    bool storiesHasHoles = WorldGen.genRand.NextBool(2);
                    for (int y = 0; y < storieHeight; y++) // stops a multitile from being partly destroyed
                    {
                        WorldGen.KillTile(location.X+storieXOffset, location.Y+storieYOffset+y);
                    }
                    WorldUtils.Gen(location + new Point(storieXOffset, storieYOffset), new Shapes.Rectangle(MinStorieWidth + storieWidth, MinStorieHeight + storieHeight), new Actions.SetTile((ushort)ModContent.TileType<AncientRuinsBlock>()));
                    WorldUtils.Gen(location + new Point(storieXOffset + 1, storieYOffset + 1), new Shapes.Rectangle(MinStorieWidth + storieWidth - 2, MinStorieHeight + storieHeight - 2), new Actions.Clear());


                    List<ushort> wallTypes = new List<ushort>()
                    {
                        (ushort)ModContent.WallType<AncientRuinWall>(),
                        (ushort)ModContent.WallType<AncientRuinWall2>(),
                        WallID.Wood,
                        WallID.Wood
                    };
                    int wall = WorldGen.genRand.Next(wallTypes.Count-1);
                    ushort wallType = wallTypes[wall];
                    if (!WorldGen.genRand.NextBool(4))
                    {
                        WorldUtils.Gen(location + new Point(storieXOffset + 1, storieYOffset + 1), new Shapes.Rectangle(MinStorieWidth + storieWidth - 2, MinStorieHeight + storieHeight - 2), new Actions.PlaceWall(wallType));
                    } else
                    {
                        for (int chunk = 0; chunk < 5; chunk++)
                        {
                            WorldUtils.Gen(location + new Point(storieXOffset + 1 + WorldGen.genRand.Next(16), storieYOffset + 1 + WorldGen.genRand.Next(4)), new Shapes.Rectangle(MinStorieWidth + storieWidth - 2 - 15, MinStorieHeight + storieHeight - 2 - 3), new Actions.PlaceWall(wallType));
                            if (chunk == 0)
                            {
                                WorldUtils.Gen(location + new Point(storieXOffset + 1 + WorldGen.genRand.Next(16), storieYOffset + 1 + WorldGen.genRand.Next(4)), new Shapes.Rectangle(MinStorieWidth + storieWidth - 2 - 15, MinStorieHeight + storieHeight - 2 - 3), new Actions.PlaceTile(TileID.Cobweb));
                            }
                        }
                    }


                    if (storiesHasHoles)
                    {
                        for (int y = 0; y < storieHeight; y++) // random holes add style
                        {
                            for (int hole = 0; hole < 4; hole++)
                            {
                                WorldGen.KillTile(location.X + storieXOffset + WorldGen.genRand.Next(storieWidth + MinStorieWidth), location.Y + storieYOffset + y);
                                // WorldUtils.Gen(location + new Point(storieXOffset + WorldGen.genRand.Next(storieWidth + MinStorieWidth), storieYOffset + y), new Shapes.Rectangle(2,1), new Actions.ClearWall()); // does not work for some reason
                            }
                        }
                    }

                    if (WorldGen.genRand.NextBool(3)) // gen door 
                    {
                        int DoorSide = WorldGen.genRand.Next(2);
                        if (DoorSide==0||DoorSide==2) // left 
                        {
                            WorldUtils.Gen(location + new Point(storieXOffset, storieYOffset + MinStorieHeight - 4 + storieHeight), new Shapes.Rectangle(1, 3), new Actions.ClearTile());
                            WorldGen.PlaceTile(location.X + storieXOffset, location.Y + storieYOffset + MinStorieHeight - 4 + storieHeight, TileID.ClosedDoor);
                        }
                        if (DoorSide == 1 || DoorSide == 2) // right
                        {
                            WorldUtils.Gen(location + new Point(storieXOffset + storieWidth + MinStorieWidth - 1, storieYOffset + MinStorieHeight - 4 + storieHeight), new Shapes.Rectangle(1, 3), new Actions.ClearTile());
                            WorldGen.PlaceTile(location.X + storieXOffset + storieWidth + MinStorieWidth - 1, location.Y + storieYOffset + MinStorieHeight - 4 + storieHeight, TileID.ClosedDoor);
                        }
                    }
                    int HoleOffset = WorldGen.genRand.Next(Math.Min(storieWidth, oldStorieWidth) + MinStorieWidth - 4);
                    int HoleBonusWidth = WorldGen.genRand.Next(2);
                    WorldUtils.Gen(location + new Point(Math.Max(storieXOffset, oldStorieXOffset) + 1 + HoleOffset, storieYOffset), new Shapes.Rectangle(2 + HoleBonusWidth, 1), new Actions.Clear());
                    if (storie!=0) // dont fill hole with wall if top of building
                    {
                        WorldUtils.Gen(location + new Point(Math.Max(storieXOffset, oldStorieXOffset) + 1 + HoleOffset, storieYOffset), new Shapes.Rectangle(2 + HoleBonusWidth, 1), new Actions.PlaceWall((ushort)ModContent.WallType<AncientRuinWall>()));
                    }
                    if (!WorldGen.genRand.NextBool(4))  // 1 in 4 Chance that the hole does not get filled with platforms
                    {
                        WorldUtils.Gen(location + new Point(Math.Max(storieXOffset, oldStorieXOffset) + 1 + HoleOffset, storieYOffset), new Shapes.Rectangle(2 + HoleBonusWidth, 1), new Actions.PlaceTile(TileID.Platforms));
                    }

                    if (WorldGen.genRand.NextBool(8)) // unlikely that a torch will spawn
                    {
                        WorldGen.PlaceTile(location.X + storieXOffset + 1 + WorldGen.genRand.Next(storieWidth + MinStorieWidth - 2),
                            location.Y + storieYOffset + storieHeight + WorldGen.genRand.Next(2) + 1, TileID.Torches);
                    }

                    List<int> FurnitureTypes = new List<int>
                    {
                        TileID.WorkBenches,
                        TileID.Anvils,
                        TileID.Furnaces,
                        ModContent.TileType<SmelteryTile>(),
                        TileID.Kegs,
                        TileID.Statues,
                        TileID.Statues,
                        TileID.Statues,
                        TileID.Statues,
                        TileID.Statues,
                        TileID.Statues,
                        TileID.Bookcases
                    };
                    Dictionary<int, int> FurnitureStyles = new Dictionary<int, int>()
                    {
                        {TileID.WorkBenches, 1 },
                        {TileID.Anvils, 2 },
                        {TileID.Furnaces, 1 },
                        {ModContent.TileType<SmelteryTile>(), 1},
                        {TileID.Kegs, 1},
                        {TileID.Statues, 74},  // not sure if that is the real number of statues
                        {TileID.Bookcases, 1}
                    };
                    int furniture = WorldGen.genRand.Next(FurnitureTypes.Count - 1);
                    WorldGen.PlaceTile(location.X + storieXOffset + 1 + WorldGen.genRand.Next(storieWidth + MinStorieWidth - 2 - TileObjectData.GetTileData(FurnitureTypes[furniture], 0).Width),
                        location.Y + storieYOffset + storieHeight + MinStorieHeight - 2, FurnitureTypes[furniture], style: WorldGen.genRand.Next(FurnitureStyles[FurnitureTypes[furniture]]));

                    if (WorldGen.genRand.NextBool(6)) // unlikely that a chest will spawn
                    {
                        if (WorldGen.genRand.NextBool(2)) // contain unknown scroll
                        {
                            WorldGen.AddBuriedChest(location + new Point(storieXOffset + 4 + WorldGen.genRand.Next(storieWidth + MinStorieWidth - 5), storieYOffset - 1), contain: ModContent.ItemType<UnknownScroll>());  // position is bottom right corner of chest unlike with any other method
                        }
                        else
                        {
                            if (WorldGen.genRand.NextBool(3)) // contain ancient club
                            {
                                WorldGen.AddBuriedChest(location + new Point(storieXOffset + 4 + WorldGen.genRand.Next(storieWidth + MinStorieWidth - 5), storieYOffset - 1 + storieHeight + MinStorieHeight));
                            } else
                            {
                                WorldGen.AddBuriedChest(location + new Point(storieXOffset + 4 + WorldGen.genRand.Next(storieWidth + MinStorieWidth - 5), storieYOffset - 1), contain: ModContent.ItemType<AncientClub>());  // position is bottom right corner of chest unlike with any other method
                            }
                        }
                    }
                    if (WorldGen.genRand.NextBool(4)) // place painting
                    {
                        PaintingEntry painting = WorldGen.RandHousePicture();
                        WorldGen.PlaceTile(location.X + storieXOffset + 4 + WorldGen.genRand.Next(storieWidth + MinStorieWidth - 5), location.Y + storieYOffset - 5, painting.tileType, style: painting.style);
                    }

                    storieYOffset += storieHeight + MinStorieHeight - 1;
                    oldStorieXOffset = storieXOffset;
                    oldStorieWidth = storieWidth;
                }
                location.X += 18;
            }
            return true;
        }
    }

    public class AncientRuinsBlockCount : ModSystem
    {
        public int BlockCount;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            BlockCount = tileCounts[ModContent.TileType<AncientRuinsBlock>()];
        }
    }

    public class AncientRuinsBiome : ModBiome // i dont think this structure deserves its own background and music
    {
        public override bool IsBiomeActive(Player player)
        {
            return ModContent.GetInstance<AncientRuinsBlockCount>().BlockCount >= 40;
        }
    }
}

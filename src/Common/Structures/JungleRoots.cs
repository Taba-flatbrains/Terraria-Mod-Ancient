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
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Ancient.src.Common.Structures
{
    internal class JungleRoots : ModSystem
    {
        // useful recources: https://hackmd.io/@tModLoader/HJUiVKXzu
        // https://forums.terraria.org/index.php?threads/modding-tutorial-world-generation.47601/
        // https://github.com/tModLoader/tModLoader/wiki/Vanilla-World-Generation-Steps
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            // 4. We use FindIndex to locate the index of the vanilla world generation task called "Underground". This ensures our code runs at the correct step.
            int GenIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Trees"));
            if (GenIndex != -1)
            {
                // 5. We register our world generation pass by passing in a name and the method that will execute our world generation code.	
                tasks.Insert(GenIndex + 1, new PassLegacy("Jungle Roots", WorldGenJungleRoots));
            }
        }

        private void WorldGenJungleRoots(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating Jungle Roots";
            for (int k = 0; k < 6; k++)
            {
                for (int tries = 0; tries < 100; tries++) // lazy approach to find jungle
                {
                    Point origin = new(WorldGen.genRand.Next(100, Main.maxTilesX - 100), (int)Main.worldSurface);
                    Dictionary<ushort, int> tileDictionary = new Dictionary<ushort, int>();
                    WorldUtils.Gen(new Point(origin.X - 25, origin.Y - 25), new Shapes.Rectangle(50, 50), new Actions.TileScanner(TileID.Mud).Output(tileDictionary));
                    
                    bool foundSuitableLocation = tileDictionary[TileID.Mud] > 1400; // mud is indicative of the jungle biome
                    
                    if (!foundSuitableLocation)
                    {
                        continue;
                    }

                    if (PlaceJungleRoot(origin))
                    {
                        break;
                    }
                }
            }
        }

        private bool PlaceJungleRoot(Point origin)
        {
            bool foundSurface = WorldUtils.Find(origin, Searches.Chain(new Searches.Up(1000),
                        new Conditions.IsSolid().AreaOr(1, 50).Not()), out Point location); // find surface
            if (!foundSurface) { return false; }
            location += new Point(0, 49);

            for (int i = 0; i < 20; i++)
            {
                WorldUtils.Gen(location, new Shapes.Rectangle(2, 2), new Actions.Clear());
                WorldUtils.Gen(location, new Shapes.Rectangle(2, 2), new Actions.PlaceTile(TileID.LivingMahogany));
                location += new Point(WorldGen.genRand.Next(3) - 1, WorldGen.genRand.Next(4) - 1); // tries to wander underground
            }
            return true;
        }
    }
}

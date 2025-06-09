using Ancient.src.Code.Tiles.AstralBiome;
using Ancient.src.Code.Walls.AstralBiome;
using Ancient.src.Common.Structures.Util;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.Generation;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace Ancient.src.Common.Structures
{
    internal class AstralBiomeGeneration : ModSystem
    {
        public static Point AstralBiomeSpot = Point.Zero;
        public static Vector2 PrimordialPearlPosition = new Vector2(-100,-100);


        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            tasks.Add(new PassLegacy("MarkFutureAstralBiomeSpots", MarkFutureAstralBiomeSpots));
        }

        private void MarkFutureAstralBiomeSpots(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Finding Astral Biome Spot";
            for (int tries = 0; tries < 100; tries++) // lazy approach to find evil biome
            {
                Point origin = new(WorldGen.genRand.Next(Main.maxTilesX / 2 + 200, Main.maxTilesX - 100), (int)Main.worldSurface);
                if (WorldGen.genRand.NextBool(2))
                {
                    origin = new(WorldGen.genRand.Next(100, Main.maxTilesX / 2 - 200), (int)Main.worldSurface);
                }

                Dictionary<ushort, int> tileDictionary = new Dictionary<ushort, int>();
                WorldUtils.Gen(new Point(origin.X - 25, origin.Y - 25), new Shapes.Rectangle(50, 50), new Actions.TileScanner(TileID.Ebonstone, TileID.Crimstone).Output(tileDictionary));

                bool foundSuitableLocation = tileDictionary[TileID.Ebonstone] + tileDictionary[TileID.Crimstone] > 500;

                if (!foundSuitableLocation)
                {
                    continue;
                }

                PlaceFutureAstralBiomeSpot(origin);
                break;
            }
        }

        private void PlaceFutureAstralBiomeSpot(Point origin)
        {
            for (int trys = 0; trys < 10; trys++) // find surface
            {
                origin.X += (int)((trys % 2 - 0.5f) * 10);
                bool foundSurface = WorldUtils.Find(origin, Searches.Chain(new Searches.Up(1000),
                            new Conditions.IsSolid().AreaOr(1, 50).Not()), out origin); // find surface
                if (foundSurface)
                {
                    origin += new Point(0, 49);
                    break;
                }
            }

            AstralBiomeSpot = origin;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Add("AstralBiomeSpot", AstralBiomeSpot);
            tag.Add("PrimordialPearlPos", PrimordialPearlPosition);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.ContainsKey("AstralBiomeSpot")) 
            {
                AstralBiomeSpot = tag.Get<Point>("AstralBiomeSpot");
                PrimordialPearlPosition = tag.Get<Vector2>("PrimordialPearlPos");
            } else
            {
                throw new Exception("Astral Biome Spot does not exist");
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.WriteVector2(PrimordialPearlPosition);
        }

        public override void NetReceive(BinaryReader reader)
        {
            PrimordialPearlPosition = reader.ReadVector2();
            Main.NewText(PrimordialPearlPosition);
        }

        private static readonly int StandardBiomeWidth = 400;
        public static void GenAstralBiome()
        {
            if (AstralBiomeSpot == Point.Zero)
            {
                return;
            }

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText("The aura grows unstable...", Color.LightGreen);
            } else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(Terraria.Localization.NetworkText.FromLiteral("The aura grows unstable..."), Color.LightGreen);
            }


            Point location = AstralBiomeSpot;
            Point surface = location;

            int ActualBiomeWidth = StandardBiomeWidth * Main.maxTilesX / 4200; // small worlds are 4200 tiles broad, biome should get larger as world is getting larger
            Rectangle workingArea = new(location.X-(ActualBiomeWidth/2), (int)(Main.worldSurface*0.35), ActualBiomeWidth, Main.maxTilesY - 250 - ((int)(Main.worldSurface * 0.35)));
            Point BiomePosition = workingArea.Location;
            Point BiomeCenter = workingArea.Center;
            Shapes.Rectangle BiomeShape = new Shapes.Rectangle(workingArea.Width, workingArea.Height);

            // remove chests in working area
            /*
            foreach (Chest chest in Main.chest)
            {
                if (workingArea.Contains(chest.x, chest.y))
                {
                    WorldGen.KillTile(chest.x, chest.y);
                }
            }
            */

            WorldUtils.Gen(BiomePosition, BiomeShape, new ModWorldGenActions.SwapTiles([TileID.Ebonstone, TileID.Crimstone, TileID.Stone, TileID.IceBlock, TileID.Sandstone], (ushort)ModContent.TileType<AstralStone>()));
            WorldUtils.Gen(BiomePosition, BiomeShape, new ModWorldGenActions.SwapTiles(
                [TileID.Dirt, TileID.Mud, TileID.ClayBlock, TileID.Sand, TileID.Sand, TileID.CrimsonHardenedSand, TileID.CrimsonSandstone, TileID.CorruptHardenedSand,
                TileID.CorruptSandstone, TileID.HallowHardenedSand, TileID.HallowSandstone, TileID.Grass, TileID.CorruptGrass, TileID.CrimsonGrass, TileID.HallowedGrass,
                TileID.CorruptJungleGrass, TileID.CrimsonJungleGrass, TileID.MushroomGrass, TileID.Crimsand, TileID.Ebonsand, TileID.Pearlsand, TileID.SnowBlock], 
                (ushort)ModContent.TileType<AstralDirt>()));
            WorldUtils.Gen(BiomePosition, BiomeShape, new Actions.RemoveWall());


            // Clear Trees
            WorldUtils.Gen(BiomePosition, BiomeShape, new ModWorldGenActions.ClearSpecificTile(TileID.Trees));
            WorldUtils.Gen(BiomePosition, BiomeShape, new ModWorldGenActions.ClearSpecificTile(TileID.Cactus));


            // Above ground spikes
            for (int i = 0; i < 10 * ActualBiomeWidth / StandardBiomeWidth; i++)
            {
                PlaceOvergroundSpike(new Point(WorldGen.genRand.Next(BiomePosition.X + 20, BiomePosition.X + workingArea.Width - 20), (int)Main.worldSurface + WorldGen.genRand.Next(20, 90)));
            }


            // Aura Tower
            PlaceAuraTower(surface);


            // Astral Nodes
            int CountAstralNodesX = (int)Math.Floor((double)workingArea.Width / AstralNodesGap);
            int CountAstralNodesY = (int)Math.Floor((double)workingArea.Height / AstralNodesGap);
            for (int i = 0; i < CountAstralNodesX; i++)
            {
                for (int j = 0; j < CountAstralNodesY; j++)
                {
                    Point AstralNodeLocation = BiomePosition + new Point(i * AstralNodesGap, j * AstralNodesGap);
                    AstralNodeLocation += new Point(WorldGen.genRand.Next(0, AstralNodesGap/2), WorldGen.genRand.Next(-2, AstralNodesGap / 2));
                    PlaceAstralNode(AstralNodeLocation);
                }
            }
            
            // Sync to all Clients
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendTileSquare(-1, BiomePosition.X, BiomePosition.Y, workingArea.Width, workingArea.Height);
            }

            AstralBiomeSpot = Point.Zero;
        }

        private static readonly int AuraTowerBaseDepth = 60;
        private static readonly int AuraTowerBaseGap = 10;
        private static readonly int AuraTowerBaseWidth = 30;
        private static readonly int AuraTowerWallThickness = 5;

        private static readonly int AuraTowerEntranceHeight = 10;
        private static readonly int AuraTowerMidHeight = 50;

        private static readonly int AuraTowerTopRadius = 22;


        private static void PlaceAuraTower(Point origin)
        {
            //hole
            WorldUtils.Gen(origin + new Point(AuraTowerBaseWidth / 2, -AuraTowerMidHeight), new Shapes.Circle(AuraTowerBaseGap + AuraTowerBaseWidth, 
                AuraTowerBaseDepth + AuraTowerMidHeight), new Actions.Clear());

            //tower base
            WorldUtils.Gen(origin, new Shapes.Rectangle(AuraTowerBaseWidth, AuraTowerBaseDepth), new Actions.Clear());
            WorldUtils.Gen(origin, new Shapes.Rectangle(AuraTowerBaseWidth, AuraTowerBaseDepth), new Actions.PlaceTile(TileID.RainbowBrick));
            // WorldUtils.Gen(origin+new Point(AuraTowerWallThickness, 0), new Shapes.Rectangle(AuraTowerBaseDepth-AuraTowerWallThickness, AuraTowerBaseWidth-AuraTowerWallThickness), new Actions.ClearTile());
            WorldUtils.Gen(origin+new Point(1, 0), new Shapes.Rectangle(AuraTowerBaseWidth - 2, AuraTowerBaseDepth - 1), new Actions.PlaceWall(WallID.RainbowBrick));

            //MidTower
            WorldUtils.Gen(origin-new Point(0, AuraTowerMidHeight), new Shapes.Rectangle(AuraTowerBaseWidth, AuraTowerMidHeight-AuraTowerEntranceHeight), new Actions.PlaceTile(TileID.RainbowBrick));
            WorldUtils.Gen(origin - new Point(-1, AuraTowerMidHeight), new Shapes.Rectangle(AuraTowerBaseWidth-2, AuraTowerMidHeight), new Actions.PlaceWall(WallID.RainbowBrick));

            //Top Tower
            origin += new Point(AuraTowerBaseWidth / 2, -AuraTowerMidHeight);
            WorldUtils.Gen(origin + new Point(0, -AuraTowerTopRadius + 5), new Shapes.Circle(AuraTowerTopRadius), new Actions.PlaceTile(TileID.RainbowBrick));
            WorldUtils.Gen(origin + new Point(0, -AuraTowerTopRadius + 5), new Shapes.Circle(AuraTowerTopRadius-AuraTowerWallThickness), new Actions.ClearTile());
            WorldUtils.Gen(origin + new Point(0, -AuraTowerTopRadius + 5), new Shapes.Circle(AuraTowerTopRadius - 1), new Actions.PlaceWall(WallID.RainbowBrick));

            // Path
            WorldUtils.Gen(origin - new Point(-AuraTowerWallThickness, AuraTowerMidHeight+AuraTowerWallThickness)- new Point(AuraTowerBaseWidth / 2, -AuraTowerMidHeight), new Shapes.Rectangle(AuraTowerBaseWidth - (2 * AuraTowerWallThickness), AuraTowerMidHeight - AuraTowerEntranceHeight + AuraTowerWallThickness), new Actions.ClearTile());

            //Top Tower Design
            origin -= new Point(AuraTowerBaseWidth / 2 - AuraTowerWallThickness, 0);
            WorldUtils.Gen(origin, new Shapes.Rectangle(AuraTowerBaseWidth - (2 * AuraTowerWallThickness), 1), new Actions.PlaceTile(TileID.Platforms, 30));
            WorldUtils.Gen(origin + new Point((AuraTowerBaseWidth - (2 * AuraTowerWallThickness))/2, 1), new Shapes.Rectangle(1, AuraTowerMidHeight-AuraTowerEntranceHeight+2), new Actions.PlaceTile(TileID.Chain));

            origin += new Point(AuraTowerBaseWidth / 2 - AuraTowerWallThickness, -AuraTowerTopRadius + 5); //Center origin
            //WorldGen.Place3x3(origin.X - 1, origin.Y - 1, (ushort)ModContent.TileType<PrimordialOrb>());
            WorldGen.PlaceTile(origin.X, origin.Y, ModContent.TileType<PrimordialOrb>(), forced: true);
            PrimordialPearlPosition = origin.ToWorldCoordinates();
        }

        private static readonly int SpikeTopMaxXDiff = 45;
        private static readonly int SpikeMinHeight = 70;
        private static readonly int SpikeMaxHeight = 160;
        private static readonly int SpikeThickness = 10;
        private static void PlaceOvergroundSpike(Point origin)
        {
            Point SpikeBase = origin;
            Point SpikeTop = SpikeBase + new Point(WorldGen.genRand.Next(-SpikeTopMaxXDiff, 1 + SpikeTopMaxXDiff), -WorldGen.genRand.Next(SpikeMinHeight, SpikeMaxHeight));

            int workingAreaXRadius = Math.Max(SpikeThickness, Math.Abs(SpikeTop.X - SpikeBase.X));
            Rectangle workingArea = new(SpikeBase.X - workingAreaXRadius, SpikeTop.Y, workingAreaXRadius * 2, SpikeBase.Y - SpikeTop.Y);
            Point SpikeGenPos = new(workingArea.X, workingArea.Y);

            for (int y = 0; y < workingArea.Height; y++) 
            {
                int LayerCenterXOffset = (int)((SpikeBase.X-SpikeTop.X)*((float)y/workingArea.Height));
                int LayerRadiusX = 1 + (int)((float)SpikeThickness/workingArea.Height*y);
                for (int x = -LayerRadiusX; x < LayerRadiusX; x++)
                {
                    Point PosBlockToBePlaced = SpikeTop + new Point(x+LayerCenterXOffset, y);
                    WorldGen.KillTile(PosBlockToBePlaced.X, PosBlockToBePlaced.Y);
                    WorldGen.PlaceTile(PosBlockToBePlaced.X, PosBlockToBePlaced.Y, ModContent.TileType<AstralStone3>(), forced: true, mute: true);
                }
            }
        }


        private static readonly int AstralNodesGap = 70;
        private static readonly int AstralNodesCircleBumpIterations = 9;
        private static readonly int AstralNodeBumpRadius = 3;
        private static readonly int AstralNodeRadius = 8;
        private static readonly int AstralNodeCrystalOuterRadius = 7;
        private static readonly int AstralNodeCrystalInnerRadius = 5;
        private static void PlaceAstralNode(Point origin)
        {
            Dictionary<ushort, int> tileDictionary = new Dictionary<ushort, int>();
            WorldUtils.Gen(new Point(origin.X - 13, origin.Y - 13), new Shapes.Rectangle(26, 26), new Actions.TileScanner((ushort)ModContent.TileType<AstralStone>(), (ushort)ModContent.TileType<AstralDirt>()).Output(tileDictionary));
            bool foundSuitableLocation = tileDictionary[(ushort)ModContent.TileType<AstralStone>()] + tileDictionary[(ushort)ModContent.TileType<AstralDirt>()] > 150; // check if node is really in astral biome
            if (!foundSuitableLocation) 
            {
                return; 
            }
            
            for (int i = 0; i < AstralNodesCircleBumpIterations; i++)
            {
                float rotation = i * MathF.PI * 2 / AstralNodesCircleBumpIterations;
                Point location = origin + new Point((int)(MathF.Sin(rotation) * AstralNodeRadius), (int)(MathF.Cos(rotation) * AstralNodeRadius));
                Point offset = new(WorldGen.genRand.Next(-1, 2), WorldGen.genRand.Next(-1, 2));
                PlaceCircleKillMultitiles(location+offset, AstralNodeBumpRadius, ModContent.TileType<AstralStone2>());
            }
            PlaceCircleKillMultitiles(origin, AstralNodeCrystalOuterRadius, ModContent.TileType<SmallArcaneCrystal>());
            WorldUtils.Gen(origin, new Shapes.Circle(AstralNodeCrystalInnerRadius), new Actions.ClearTile());
            WorldUtils.Gen(origin, new Shapes.Circle(AstralNodeCrystalOuterRadius+1), new Actions.PlaceWall((ushort)ModContent.WallType<AstralNodeBackgroundWall>()));
        }

        private static void PlaceCircleKillMultitiles(Point center, int radius, int type)
        {
            for (int i = 0; i < radius*2+1; i++)
            {
                for (int j = 0; j < radius*2+1; j++)
                {
                    if (new Vector2(i - radius, j-radius).Length() > radius)
                    {
                        continue;
                    }
                    WorldGen.KillTile(i + center.X - radius, j + center.Y - radius);
                    WorldGen.PlaceTile(i + center.X - radius, j + center.Y - radius, type, forced: true, mute: true);
                }
            }
        }
    }

    public class AstralBiomeGenOnMoodlordKill : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            //AstralBiomeGeneration.GenAstralBiome();
            if (npc.type == NPCID.MoonLordCore && Condition.NotDownedMoonLord.Predicate.Invoke() && Main.netMode != NetmodeID.MultiplayerClient)
            {
                AstralBiomeGeneration.GenAstralBiome();
            }
        }
    }

    public class AstralBiome : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<AstralBiomeSurfaceBackgroundStyle>();
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<ExampleUndergroundBackgroundStyle>();

        public override string BestiaryIcon => "src/Assets/Textures/AstralBiomeBestiaryIcon";
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override string MapBackground => BackgroundPath; // Re-uses Bestiary Background for Map Background

        public override bool IsBiomeActive(Player player)
        {
            return ModContent.GetInstance<AstralBiomeBlockCount>().BlockCount >= 100;
        }
    }

    public class ExampleUndergroundBackgroundStyle : ModUndergroundBackgroundStyle
    {
        public override void FillTextureArray(int[] textureSlots)
        {
            textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "src/Assets/Textures/Backgrounds/AstralBiomeUnderground0");
            textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "src/Assets/Textures/Backgrounds/AstralBiomeUnderground1");
            textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "src/Assets/Textures/Backgrounds/AstralBiomeUnderground2");
            textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "src/Assets/Textures/Backgrounds/AstralBiomeUnderground3");
        }
    }

    public class AstralBiomeSurfaceBackgroundStyle : ModSurfaceBackgroundStyle
    {
        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot)
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                    {
                        fades[i] = 1f;
                    }
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                    {
                        fades[i] = 0f;
                    }
                }
            }
        }

        public override int ChooseFarTexture()
        {
            return BackgroundTextureLoader.GetBackgroundSlot(Mod, "src/Assets/Textures/Backgrounds/AstralBiomeSurfaceFar");
        }

        private static int SurfaceFrameCounter;
        private static int SurfaceFrame;
        public override int ChooseMiddleTexture()
        {
            if (++SurfaceFrameCounter > 12)
            {
                SurfaceFrame = (SurfaceFrame + 1) % 4;
                SurfaceFrameCounter = 0;
            }
            switch (SurfaceFrame)
            {
                case 0:
                    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "src/Assets/Textures/Backgrounds/AstralBiomeSurfaceMid0");
                case 1:
                    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "src/Assets/Textures/Backgrounds/AstralBiomeSurfaceMid1");
                case 2:
                    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "src/Assets/Textures/Backgrounds/AstralBiomeSurfaceMid2");
                case 3:
                    return BackgroundTextureLoader.GetBackgroundSlot(Mod, "src/Assets/Textures/Backgrounds/AstralBiomeSurfaceMid3"); // You can use the full path version of GetBackgroundSlot too
                default:
                    return -1;
            }
        }

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {
            return BackgroundTextureLoader.GetBackgroundSlot(Mod, "src/Assets/Textures/Backgrounds/AstralBiomeSurfaceClose");
        }
    }

    public class AstralBiomeShaderData : ScreenShaderData
    {

        public AstralBiomeShaderData(Asset<Effect> shader, string passName) : base(shader, passName)
        {
        }

        public override void Apply()
        {
            //Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            //UseTargetPosition(Main.npc[AstralBiomeIndex].Center - Main.screenPosition + zero);

            UseIntensity(MathF.Min((float)ModContent.GetInstance<AstralBiomeBlockCount>().BlockCount/500, 1)*Main.LocalPlayer.GetModPlayer<AstralBiomeShaderPlayer>().AstralBiomeShaderIntensityMultiplier);
            base.Apply();
        }
    }

    public class AstralBiomePlayer : ModPlayer
    {
        public override void OnEnterWorld()
        {
            Terraria.Graphics.Effects.Filters.Scene.Activate("AstralBiome");
        }
    }

    public class AstralBiomeShaderPlayer : ModPlayer
    {
        public float AstralBiomeShaderIntensityMultiplier = 1;
        public override void ResetEffects()
        {
            AstralBiomeShaderIntensityMultiplier = 1;
        }
    }
}


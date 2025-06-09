using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Ancient.src.Code.NPCS
{
    internal class NPCUtils
    {
        public static bool ShouldJump(NPC npc, bool ShouldStepOverBlock = true)
        {
            if (npc.gfxOffY > 0)
            {
                if (npc.GetGlobalNPC<NPCUtilsNPC>().OnSlope)
                {
                    npc.gfxOffY -= MathF.Abs(npc.velocity.X);
                    if (MathF.Abs(npc.velocity.X) < 0.05f)
                    {
                        npc.gfxOffY -= 2;
                    }
                } else
                {
                    npc.gfxOffY -= MathF.Abs(npc.velocity.X) * 3;
                }
            } else
            {
                npc.GetGlobalNPC<NPCUtilsNPC>().OnSlope = false;
            }

            if (!OnGround(npc)) { return false; }

            Point NewPositionBot = (npc.Center + npc.velocity * 5 + new Vector2((npc.width / 2) * npc.direction, npc.height / 2 - 9)).ToTileCoordinates();
            Point BlockTouchingFeet = (npc.Center + npc.velocity + new Vector2((npc.width / 2 + 1) * npc.direction, npc.height / 2 - 7)).ToTileCoordinates();
            int TileCoordinateWidth = (int)MathF.Ceiling(npc.width / 16f);
            int TileCoordinateHeight = (int)MathF.Ceiling(npc.height / 16f);

            // check for wall, if only lowest block no need for jumping, can just step over
            for (int i = 1; i < TileCoordinateHeight; i++)
            {
                if (Main.tileSolid[Main.tile[NewPositionBot + new Point(0, -i)].TileType] && Main.tile[NewPositionBot + new Point(0, -i)].HasUnactuatedTile)
                {
                    return true;
                }
            }

            // step over block
            if (Main.tileSolid[Main.tile[BlockTouchingFeet].TileType] && Main.tile[BlockTouchingFeet].HasUnactuatedTile)
            {
                npc.velocity.Y -= 1f;
                if (Main.tile[BlockTouchingFeet].Slope == Terraria.ID.SlopeType.SlopeUpRight && npc.direction == -1 ||
                    Main.tile[BlockTouchingFeet].Slope == Terraria.ID.SlopeType.SlopeUpLeft && npc.direction == 1)
                {
                    npc.position.Y -= 8;
                    npc.gfxOffY += 8;
                    if (npc.position.Y % 16 != 8)
                    {
                        npc.position.Y -= 8;
                        npc.gfxOffY += 8;
                    }
                    npc.GetGlobalNPC<NPCUtilsNPC>().OnSlope = true;
                } 
                else if (Main.tile[BlockTouchingFeet].IsHalfBlock)
                {
                    npc.position.Y -= 8;
                    npc.gfxOffY += 8;
                    if (npc.position.Y % 16 == 8)
                    {
                        npc.position.Y -= 8;
                        npc.gfxOffY += 8;
                    }
                }  
                else
                {
                    npc.position.Y -= 8;
                    npc.gfxOffY += 8;
                    if (npc.position.Y % 16 != 8)
                    {
                        npc.position.Y -= 8;
                        npc.gfxOffY += 8;
                    }
                }
            }

            // check for hole 
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < TileCoordinateWidth; i++)
                {
                    if ((Main.tileSolid[Main.tile[NewPositionBot + new Point(i * npc.direction, 2-j)].TileType] ||
                        Main.tileSolidTop[Main.tile[NewPositionBot + new Point(i * npc.direction, 2-j)].TileType]) && Main.tile[NewPositionBot + new Point(i * npc.direction, 2-j)].HasUnactuatedTile)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool OnGround(NPC npc)
        {
            if (npc.velocity.Y < -0.1f) { return false; }
            Point Left = (npc.Center + new Vector2(-(npc.width - 1) / 2, npc.height / 2)).ToTileCoordinates() + new Point(0, 1);
            int TileCoordinateWidth = (int)MathF.Ceiling(npc.width / 16f);
            for (int i = 0; i < TileCoordinateWidth; i++)
            {
                if ((Main.tileSolid[Main.tile[Left + new Point(i, 0)].TileType] || Main.tileSolidTop[Main.tile[Left + new Point(i, 0)].TileType]) && Main.tile[Left + new Point(i, 0)].HasUnactuatedTile)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool InWall(Vector2 position, int width, int height) // top left corner position, world coordinates width and height
        {
            Point origin = position.ToTileCoordinates();
            int Width = (int)MathF.Ceiling(((float)width) / 16f) + 1;
            int Height = (int)MathF.Ceiling(((float)height) / 16f) + 1;

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Point tile_position = origin + new Point(i,j);
                    Tile tile = Main.tile[tile_position];
                    if (Main.tileSolid[tile.TileType] && tile.HasUnactuatedTile)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public class NPCUtilsNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool OnSlope = false;
    }
}

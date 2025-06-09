using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace Ancient.src.Common.Structures.Util
{
    public class WorldGenConditionHasShimmer : GenCondition
    {
        protected override bool CheckValidity(int x, int y)
        {
            if (GenBase._tiles[x, y].CheckingLiquid)
            {
                return GenBase._tiles[x, y].LiquidType == LiquidID.Shimmer;
            }

            return false;
        }
    }

    public class WorldGenConditionHasDungeonBricks : GenCondition
    {
        protected override bool CheckValidity(int x, int y)
        {
            if (GenBase._tiles[x, y].HasTile)
            {
                return GenBase._tiles[x, y].TileType == TileID.BlueDungeonBrick || GenBase._tiles[x, y].TileType == TileID.GreenDungeonBrick ||
                    GenBase._tiles[x, y].TileType == TileID.PinkDungeonBrick;
            }

            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.WorldBuilding;
using Terraria;
using Microsoft.Xna.Framework;

namespace Ancient.src.Common.Structures.Util
{
    public static class ModWorldGenActions
    {
        public class SwapTiles : GenAction
        {
            private ushort[] _type;
            private ushort _new_type;

            public SwapTiles(ushort[] remove_type, ushort place_type)
            {
                _type = remove_type;
                _new_type = place_type;
            }

            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                Tile tile = GenBase._tiles[x, y];
                if (tile.HasTile)
                {
                    if (_type.Contains(tile.TileType))
                    {
                        tile.ResetToType(_new_type);
                        return UnitApply(origin, x, y, args);
                    }
                }
                return Fail();
            }
        }

        public class ClearSpecificTile : GenAction
        {
            private ushort _type;

            public ClearSpecificTile(ushort remove_type)
            {
                _type = remove_type;
            }

            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                Tile tile = GenBase._tiles[x, y];
                if (tile.HasTile)
                {
                    if (tile.TileType == _type)
                    {
                        tile.ClearTile();
                    }
                }
                return Fail();
            }
        }
    }
}

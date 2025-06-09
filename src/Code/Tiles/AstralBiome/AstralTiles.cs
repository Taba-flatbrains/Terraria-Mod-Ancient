using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Ancient.src.Code.Tiles.OceanTemple;
using Ancient.src.Code.Dusts;

namespace Ancient.src.Code.Tiles.AstralBiome
{
    internal abstract class AstralTileBase : ModTile
    {
        public virtual int Drop => ItemID.DirtBlock;
        public virtual Color mapColor => new Color(87, 129, 193);
        public virtual int minPick => 200;

        public override void SetStaticDefaults()
        {
            // DustType = DustID.WoodFurniture;
            Main.tileSolid[Type] = true;
            AddMapEntry(mapColor);
            MinPick = minPick;
            MineResist = 2;
            DustType = ModContent.DustType<AstralTilesDust>();

            RegisterItemDrop(Drop);
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }
    }

    internal class AstralDirt : AstralTileBase
    {
        public override int minPick => 0;
        public override Color mapColor => new Color(63, 134, 202);
    }

    internal class AstralStone : AstralTileBase
    {
        public override int Drop => ItemID.StoneBlock;
        public override int minPick => 0;
        public override Color mapColor => new Color(41, 42, 122);
    }

    internal class AstralStone2 : AstralTileBase
    {
        public override int Drop => ItemID.StoneBlock;
        public override Color mapColor => new Color(92, 45, 136);
    }

    internal class AstralStone3 : AstralTileBase
    {
        public override int Drop => ItemID.StoneBlock;
        public override Color mapColor => new Color(47, 15, 42);
    }

    public class AstralBiomeBlockCount : ModSystem
    {
        public int BlockCount;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            BlockCount = tileCounts[ModContent.TileType<AstralDirt>()] + tileCounts[ModContent.TileType<AstralStone>()] + tileCounts[ModContent.TileType<AstralStone2>()];
        }
    }
}

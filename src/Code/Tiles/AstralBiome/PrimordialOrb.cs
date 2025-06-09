using Ancient.src.Code.Items.Materials.AstralBiome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;

namespace Ancient.src.Code.Tiles.AstralBiome
{
    internal class PrimordialOrb : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.AnchorBottom = Terraria.DataStructures.AnchorData.Empty;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(0,0,0));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.2f;
            g = 0.2f;
            b = 0.3f;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return false;
        }

        public override bool IsTileDangerous(int i, int j, Player player)
        {
            return true;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            noBreak = true;
            return false;
        }

        public override void RandomUpdate(int i, int j)
        {
            Dust.NewDust(new Point(i, j).ToWorldCoordinates(), 1, 1, DustID.MoonBoulder);
            Lighting.AddLight(new Point(i, j).ToWorldCoordinates(), new Vector3(1, 1, 1));
            foreach (Player player in Main.player)
            {
                if (Vector2.Distance(player.Center, new Point(i, j).ToWorldCoordinates()) < 16 * 15)
                {
                    Vector2 direction = new Point(i, j).ToWorldCoordinates() - player.Center;
                    direction.Normalize();
                    player.velocity += direction * 5;
                }
            }
        }
    }
}

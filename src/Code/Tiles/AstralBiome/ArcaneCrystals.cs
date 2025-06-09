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
using Ancient.src.Code.Items.Materials.AstralBiome;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Ancient.src.Code.Tiles.AstralBiome
{
    internal class ArcaneCrystal : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;

            DustType = DustID.GemSapphire;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(122, 210, 230));

            RegisterItemDrop(ModContent.ItemType<ArcaneShard>());
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.2f;
            g = 0.2f;
            b = 0.3f;
        }
    }

    internal class SmallArcaneCrystal : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;

            DustType = DustID.GemSapphire;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.addTile(Type);
            MineResist = 6;
            MinPick = 200;

            AddMapEntry(new Color(122, 210, 230));

            RegisterItemDrop(ModContent.ItemType<ArcaneShard>());
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.4f;
            g = 0.4f;
            b = 0.6f;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D texture = TextureAssets.Tile[Type].Value;
            HasSolidSurroundingTiles(i, j, out int direction);

            float rotation = MathHelper.PiOver2 * direction;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

            Vector2 rotation_offset = Vector2.Zero;
            switch (direction)
            {
                case 1:
                    {
                        rotation_offset = new Vector2(16, 0);
                        break;
                    }
                case 2:
                    {
                        rotation_offset = new Vector2(16, 16);
                        break;
                    }
                case 3:
                    {
                        rotation_offset = new Vector2(0, 16);
                        break;
                    }
            }

            spriteBatch.Draw(texture, new Point(i, j).ToWorldCoordinates()-Main.screenPosition+zero-new Vector2(8, 8)+rotation_offset, 
                default, Lighting.GetColor(new Point(i, j)), rotation, 
                Vector2.One, 1, SpriteEffects.None, 0f);

            return false;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            noBreak = HasSolidSurroundingTiles(i, j);
            return true;
        }

        private bool HasSolidSurroundingTiles(int i, int j)
        {
            return HasSolidSurroundingTiles(i, j, out _);
        }
        private bool HasSolidSurroundingTiles(int i, int j, out int Direction) // Direction: 0 -> bottom, 1 -> left, 2 -> top, 3 -> right
        {
            if (Main.tile[i, j + 1].HasUnactuatedTile && Main.tile[i, j + 1].TileType != ModContent.TileType<SmallArcaneCrystal>())
            {
                Direction = 0;
                return true;
            }
            if (Main.tile[i-1, j].HasUnactuatedTile && Main.tile[i - 1, j].TileType != ModContent.TileType<SmallArcaneCrystal>())
            {
                Direction = 1;
                return true;
            }
            if (Main.tile[i, j - 1].HasUnactuatedTile && Main.tile[i, j - 1].TileType != ModContent.TileType<SmallArcaneCrystal>())
            {
                Direction = 2;
                return true;
            }
            if (Main.tile[i + 1, j].HasUnactuatedTile && Main.tile[i + 1, j].TileType != ModContent.TileType<SmallArcaneCrystal>())
            {
                Direction = 3;
                return true;
            }
            Direction = -1;
            return false;
        }
    }
}

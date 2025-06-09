using Ancient.src.Code.Items;
using Ancient.src.Code.Items.Materials.Bars;
using Ancient.src.Code.Projectiles.Trap;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Tiles.Traps
{
    internal class LaserTrap :  ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(silver: 25); // How many coins the item is worth
            Item.rare = ItemRarityID.Green; // The rarity of the item

            Item.DefaultToPlaceableTile(ModContent.TileType<LaserTrapTile>());
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Wire, 5).AddIngredient<SolderingTinBar>(3).AddIngredient<SteelBar>(10).AddRecipeGroup(RecipeGroups.GemRecipeGroup, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }


    internal class LaserTrapTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.IsAMechanism[Type] = true;
            TileID.Sets.IgnoresNearbyHalfbricksWhenDrawn[Type] = true;
            TileID.Sets.DontDrawTileSliced[Type] = true;

            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileFrameImportant[Type] = true;

            DustType = DustID.Stone;
            AddMapEntry(new Color(90, 90, 90));
        }

        public override bool IsTileDangerous(int i, int j, Player player) => true;

        public override void PlaceInWorld(int i, int j, Item item) // set style
        {
            Tile tile = Main.tile[i, j];
            if (Main.LocalPlayer.direction == -1)
            {
                tile.TileFrameY += 18;
            }
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
            }
        }

        public override bool Slope(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int nextFrameY = (tile.TileFrameY / 18 + 1) % 6;
            tile.TileFrameY = (short)(nextFrameY * 18);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
            }
            return false;
        }

        public override void HitWire(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int style = tile.TileFrameY / 18;
            Vector2 direction = new Vector2(1, 0);

            if (style == 0)
            {
                direction = new Vector2(1, 0);
            }
            else if (style == 1)
            {
                direction = new Vector2(-1, 0);
            }
            else if (style == 2 || style == 3)
            {
                direction = new Vector2(0, -1);
            }
            else if (style == 4 || style == 5)
            {
                direction = new Vector2(0, 1);
            }

            Vector2 spawnPos = new Point(i, j).ToWorldCoordinates() + (direction * 8);

            if (Wiring.CheckMech(i, j, 30))
            {
                Projectile.NewProjectile(Wiring.GetProjectileSource(i, j), spawnPos, direction * 25f, ModContent.ProjectileType<Laserbeam>(), 40, 2f, Main.myPlayer);
                SoundEngine.PlaySound(SoundID.Item12);
            }
        }
    }
}

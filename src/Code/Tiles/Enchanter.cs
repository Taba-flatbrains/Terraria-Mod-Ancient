using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Ancient.src.Code.Items.Scrolls;
using Terraria.ModLoader.IO;
using Ancient.src.Code.Items.ReforgeStones;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using static Ancient.Ancient;
using Ancient.src.Code.Items.Materials.AstralBiome;
using Ancient.src.Code.Projectiles.Elf;
using Ancient.src.Code.Projectiles;
using Ancient.src.Code.Projectiles.Animation;
using Terraria.Audio;

namespace Ancient.src.Code.Tiles
{
    internal abstract class Enchanter : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.value = Item.buyPrice(0, 50, 0, 0);
            Item.rare = ItemRarityID.Cyan;
        }
    }

    internal class ElvenEnchanter : Enchanter
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ElvenEnchanterTile>());
        }
    }

    internal class LunarEnchanter : Enchanter
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LunarEnchanterTile>());
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.StoneBlock, 30).AddIngredient(ModContent.ItemType<ArcaneShard>(), 30).AddTile(TileID.WorkBenches).Register();
        }
    }

    internal abstract class EnchanterTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;

            DustType = DustID.Stone;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<EnchanterTileEntity>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(100, 80, 80));

            RegisterItemDrop(ModContent.ItemType<AncientAltar>());
            AnimationFrameHeight = 54;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 15) // spend 5 ticks on every animation frame
            {
                frameCounter = 0;
                frame++;
                if (frame > 1) // 2 total animation frames
                {
                    frame = 0;
                }
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            b = 0.1f;
            g = 0.02f;
            r = 0.01f;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            base.KillMultiTile(i, j, frameX, frameY);
            ModContent.GetInstance<EnchanterTileEntity>().Kill(i, j);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
            ModContent.GetInstance<EnchanterTileEntity>().Kill(i, j);
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;

            player.cursorItemIconID = player.inventory[player.selectedItem].type;
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Main.mouseRightRelease = false;

            if (TileUtils.TryGetTileEntityAs(i, j, out EnchanterTileEntity entity))
            {
                if ((player.inventory[player.selectedItem].CanHavePrefixes() || player.inventory[player.selectedItem].IsAir) && player.selectedItem != 58) // Swap Item with Item in the Enchanter
                {
                    if (entity.ReforgedItem == null) { entity.ReforgedItem = new Item(); }
                    Item PlayerItem = player.inventory[player.selectedItem].Clone();
                    player.inventory[player.selectedItem] = entity.ReforgedItem.Clone();
                    entity.ReforgedItem = PlayerItem;
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        player.inventory[player.selectedItem].NetStateChanged();

                        ModPacket packet = Mod.GetPacket();
                        packet.Write((byte)MessageType.InteractWithTileEntity);
                        packet.Write((int)entity.Type);
                        packet.Write((int)entity.Position.X);
                        packet.Write((int)entity.Position.Y);
                        ItemIO.Send(entity.ReforgedItem, packet, writeStack: true);
                        packet.Send();
                    }
                } 
                else if (ReforgeStoneUtils.ReforgeStoneTypes.Contains(player.inventory[player.selectedItem].type)) // apply reforge stone to item
                {
                    int PrefixTryToApply = ((ReforgeStoneBase)player.inventory[player.selectedItem].ModItem).PrefixID;
                    if (entity.ReforgedItem != null)
                    {
                        if (entity.ReforgedItem.CanApplyPrefix(PrefixTryToApply) &&
                            entity.ReforgedItem.prefix != PrefixTryToApply)
                        {
                            entity.ReforgedItem.Prefix(((ReforgeStoneBase)player.inventory[player.selectedItem].ModItem).PrefixID);
                            player.inventory[player.selectedItem].stack -= 1;

                            if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                ModPacket packet = Mod.GetPacket();
                                packet.Write((byte)MessageType.InteractWithTileEntity);
                                packet.Write((int)entity.Type);
                                packet.Write((int)entity.Position.X);
                                packet.Write((int)entity.Position.Y);
                                ItemIO.Send(entity.ReforgedItem, packet, writeStack: true);
                                packet.Send();
                            }

                            // Animation
                            Projectile.NewProjectile(new EntitySource_TileInteraction((Entity)player, i, j, context: "Enchanter"), entity.Position.ToWorldCoordinates() + new Vector2(16, 4),
                                new Vector2(0,-0.3f), ModContent.ProjectileType<EnchanterAnimation>(), 0, 0);

                            SoundEngine.PlaySound(SoundID.Item29, (entity.Position + new Point16(1, 1)).ToWorldCoordinates());
                        }
                    }
                }
            }
            return false;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (TileUtils.TryGetTileEntityAs(i, j, out EnchanterTileEntity entity) && TileUtils.GetTopLeftTileInMultitile(i, j) == new Point16(i - 2, j))
            {
                if (entity.ReforgedItem != null)
                {
                    Vector2 playerPos = Main.LocalPlayer.position;
                    spriteBatch.Draw(TextureAssets.Item[entity.ReforgedItem.type].Value, new Vector2(
                        i * 16 - playerPos.X + (Main.screenWidth / 2) + 158,
                        (float)(j * 16 - playerPos.Y + (Main.screenHeight / 2) + 155 + Math.Sin(Main.time / 60) * 5) - Main.LocalPlayer.gfxOffY), new Color(255, 255, 255));
                }

                if (Type == ModContent.TileType<ElvenEnchanterTile>() && new Random().Next(30) == 0)
                {
                    Dust.NewDust(new Point(i - 1, j).ToWorldCoordinates() + new Vector2(-4, 3), 8, 8, DustID.ShimmerSpark);
                }
            }
            return true;
        }
    }

    internal class ElvenEnchanterTile : EnchanterTile { }
    internal class LunarEnchanterTile : EnchanterTile { }

    internal class EnchanterTileEntity : ModTileEntity
    {
        public Item ReforgedItem = new Item();

        public override void OnKill()
        {
            if (ReforgedItem == null)
            {
                return;
            }
            if (ReforgedItem.IsAir) { return; }
            Item.NewItem(ReforgedItem.GetSource_DropAsItem(), Position.ToWorldCoordinates() + new Vector2(16, 16), new Vector2(16, 16), ReforgedItem);
        }
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && (tile.TileType == ModContent.TileType<ElvenEnchanterTile>() || tile.TileType == ModContent.TileType<LunarEnchanterTile>());
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            Point16 tileOrigin = new Point16(0, 0);

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                // Sync the entire multitile's area.  Modify "width" and "height" to the size of your multitile in tiles
                int width = 3;
                int height = 3;
                NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

                // Sync the placement of the tile entity with other clients
                // The "type" parameter refers to the tile type which placed the tile entity, so "Type" (the type of the tile entity) needs to be used here instead
                NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j, number3: Type);
                return -1;
            }

            // ModTileEntity.Place() handles checking if the entity can be placed, then places it for you
            // Set "tileOrigin" to the same value you set TileObjectData.newTile.Origin to in the ModTile
            int placedEntity = Place(i - tileOrigin.X, j - tileOrigin.Y);
            return placedEntity;
        }

        public override void OnNetPlace()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("ReforgedItem", ReforgedItem);
        }

        public override void LoadData(TagCompound tag)
        {
            ReforgedItem = tag.Get<Item>("ReforgedItem");
        }

        public override void NetSend(BinaryWriter writer) // I need to sync the item but I have no Idea how to do it
        {
            ItemIO.Send(ReforgedItem, writer, writeStack: true);
        }

        public override void NetReceive(BinaryReader reader)
        {
            ReforgedItem = ItemIO.Receive(reader, readStack: true);
        }
    }
}

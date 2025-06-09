using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.Localization;
using Ancient.src.Common.UI;
using Terraria.ModLoader.IO;
using Ancient.src.Code.Items.Scrolls;
using Microsoft.Xna.Framework.Graphics;
using Ancient.src.Code.Items.Accessoires;
using Ancient.src.Code.Items.Usables.Weapons;
using Terraria.ModLoader.Exceptions;
using Ancient.src.Code.Items.Usables.Consumables;
using System.IO;
using Terraria.Social.WeGame;
using static Ancient.Ancient;
using Stubble.Core.Classes;

namespace Ancient.src.Code.Tiles
{
    internal class AncientAltar : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AncientAltarTile>());
            Item.width = 32;
            Item.height = 32;
            Item.value = Item.buyPrice(0, 12, 50, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override void SetStaticDefaults()
        {

        }
    }

    internal class AncientAltarTile : ModTile
    {
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            if (player.inventory[player.selectedItem].ModItem as Scroll == null) { return; }
            if (((Scroll)player.inventory[player.selectedItem].ModItem).ritualResultItemID == null) { return; }

            
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;

            player.cursorItemIconID = ((Scroll)player.inventory[player.selectedItem].ModItem).ritualResultItemID;
        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;

            DustType = DustID.Stone;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<AncientAltarTileEntity>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0,0);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(100, 80, 80));

            RegisterItemDrop(ModContent.ItemType<AncientAltar>());
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.7f;
            g = 0.3f;
            b = 0.6f;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<AncientAltarTileEntity>().Kill(i, j);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            ModContent.GetInstance<AncientAltarTileEntity>().Kill(i, j);
        }

        public override bool RightClick(int i, int j)
        {

            Player player = Main.LocalPlayer;
            Main.mouseRightRelease = false;

            if (TileUtils.TryGetTileEntityAs(i, j, out AncientAltarTileEntity entity))
            {
                if (ScrollConstants.GetScrolls().Contains(player.HeldItem.type))
                {
                    entity.ScrollItem = (Scroll)ModContent.GetModItem(player.HeldItem.type);
                    if (NetmodeID.SinglePlayer != Main.netMode)
                    {
                        ModPacket packet = Mod.GetPacket();
                        packet.Write((byte)MessageType.InteractWithTileEntity);
                        packet.Write((int)entity.Type);
                        packet.Write((int)entity.Position.X);
                        packet.Write((int)entity.Position.Y);
                        ItemIO.Send(player.HeldItem, packet, writeStack: true);
                        packet.Send();
                        //NetMessage.SendTileSquare(-1, i, j, 1);
                    }

                    return true;
                }
            }
            return false;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (TileUtils.TryGetTileEntityAs(i, j, out AncientAltarTileEntity entity) && TileUtils.GetTopLeftTileInMultitile(i, j) == new Point16(i - 2, j))
            {
                if (entity.ScrollItem != null)
                {
                    if (entity.ejectingItem == -1)
                    {
                        Vector2 playerPos = Main.LocalPlayer.position;
                        spriteBatch.Draw(entity.ScrollItem.ritualResultItemTexture, new Vector2(
                            i * 16 - playerPos.X + (Main.screenWidth / 2) + 158,
                            (float)(j * 16 - playerPos.Y + (Main.screenHeight / 2) + 140 + Math.Sin(Main.time / 60) * 5) - Main.LocalPlayer.gfxOffY), new Color(255, 255, 255));
                        // further animation needed, looks a bit shitty
                    } 
                    else if (entity.ejectingItem == 0)
                    {
                        entity.ScrollItem = null;
                        Vector2 playerPos = Main.LocalPlayer.position;
                        for (int k = 0; k < 40; k++)
                        {
                            Dust.NewDust(new Vector2(
                                (i - 2f) * 16, j * 16 + entity.ejectingItem - 100),
                                50, 50, DustID.InfernoFork
                            );
                        }
                        entity.ejectingItem--;
                    } 
                    else if (entity.ejectingItem > 0)
                    {
                        entity.ejectingItem--;
                        Vector2 playerPos = Main.LocalPlayer.position;
                        spriteBatch.Draw(entity.ScrollItem.ritualResultItemTexture, new Vector2(
                            i * 16 - playerPos.X + (Main.screenWidth / 2) + 158,
                            j * 16 - playerPos.Y + (Main.screenHeight / 2) + 140 - 60 + entity.ejectingItem - Main.LocalPlayer.gfxOffY) 
                            , new Color(255-60+entity.ejectingItem, 255-180+(entity.ejectingItem*3), 255 - 120+(entity.ejectingItem * 2)));
                        Dust.NewDust(new Vector2(
                            (i-2f) * 16, j * 16 + entity.ejectingItem - 100),
                            50, 50, DustID.Water_Corruption
                        );
                    }
                }
            }
            return true;
        }
    }

    internal class AncientAltarTileEntity : ModTileEntity
    {
        public int ejectingItem = -1;
        public Scroll ScrollItem = (Scroll)ModContent.GetModItem(ModContent.ItemType<NoneScroll>());
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<AncientAltarTile>();
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
            int placedEntity = Place(i-tileOrigin.X, j-tileOrigin.Y);
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
            tag.Add("ScrollItem", ScrollItem.Type);
        }

        public override void LoadData(TagCompound tag)
        {
            try
            {
                ScrollItem = (Scroll)ModContent.GetModItem(tag.Get<int>("ScrollItem"));
            } catch
            {
                ScrollItem = (Scroll)ModContent.GetModItem(ModContent.ItemType<NoneScroll>());
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(ejectingItem);
            writer.Write(ScrollItem.Type);
        }

        public override void NetReceive(BinaryReader reader)
        {
            ejectingItem = reader.ReadInt32();
            ScrollItem = (Scroll)ModContent.GetModItem(reader.ReadInt32());
        }
    }

    public static class AltarCraftingConditions
    {
        public static Func<bool> AltarWithRecipeNearby(int ResultItemID)
        {
            return () => TileEntity.ByPosition.Values.Any(entity =>
            {
                if (entity.type == ModContent.TileEntityType<AncientAltarTileEntity>())
                {
                    if (((AncientAltarTileEntity)entity).ScrollItem != null)
                    {
                        return ((AncientAltarTileEntity)entity).ScrollItem.ritualResultItemID == ResultItemID &&
                        Vector2.Distance(new Vector2(entity.Position.X * 16, entity.Position.Y * 16), Main.LocalPlayer.position) < 100;  // needs to be in 100 pixel proximity
                    }
                }
                return false;
            });
        }

        public static readonly Condition EyeOfSkadi = new Condition("Mods.Ancient.Conditions.EyeOfSkadiSelected", AltarWithRecipeNearby(ModContent.ItemType<EyeOfSkadi>()));
        public static readonly Condition IronCore = new Condition("Mods.Ancient.Conditions.IronCoreSelected", AltarWithRecipeNearby(ModContent.ItemType<IronCore>()));
        public static readonly Condition Sharanga = new Condition("Mods.Ancient.Conditions.SharangaSelected", AltarWithRecipeNearby(ModContent.ItemType<Sharanga>()));
        public static readonly Condition IonShell = new Condition("Mods.Ancient.Conditions.IonShellSelected", AltarWithRecipeNearby(ModContent.ItemType<IonShell>()));
        public static readonly Condition LivingwoodBarrier = new Condition("Mods.Ancient.Conditions.LivingwoodBarrierSelected", AltarWithRecipeNearby(ModContent.ItemType<LivingwoodBarrier>()));
        public static readonly Condition ChaosPendant = new Condition("Mods.Ancient.Conditions.ChaosPendantSelected", AltarWithRecipeNearby(ModContent.ItemType<ChaosPendant>()));
        public static readonly Condition Mekansm = new Condition("Mods.Ancient.Conditions.MekansmSelected", AltarWithRecipeNearby(ModContent.ItemType<Mekansm>()));
        public static readonly Condition SuspiciousLookingCrystal = new Condition("Mods.Ancient.Conditions.SuspiciousLookingCrystalSelected", AltarWithRecipeNearby(ModContent.ItemType<SuspiciousLookingCrystal>()));
        public static readonly Condition BloodStone = new Condition("Mods.Ancient.Conditions.BloodStoneSelected", AltarWithRecipeNearby(ModContent.ItemType<BloodStone>()));
        public static readonly Condition Bloodlust = new Condition("Mods.Ancient.Conditions.BloodlustSelected", AltarWithRecipeNearby(ModContent.ItemType<Bloodlust>()));
    }

    public static class AltarCraftingCallbacks
    {
        public static Recipe.OnCraftCallback GetOnCraftCallback(int ResultItemID)
        {
            return (Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack) => 
            {
                IEnumerable<TileEntity> altarsWithCraftedItem = TileEntity.ByPosition.Values.Where(entity =>
                {
                    if (entity.type == ModContent.TileEntityType<AncientAltarTileEntity>())
                    {
                        if (((AncientAltarTileEntity)entity).ScrollItem != null)
                        {
                            return ((AncientAltarTileEntity)entity).ScrollItem.ritualResultItemID == ResultItemID &&
                            Vector2.Distance(new Vector2(entity.Position.X * 16, entity.Position.Y * 16), Main.LocalPlayer.position) < 100;
                        }
                    }
                    return false;
                });
                AncientAltarTileEntity AltarUsed = (AncientAltarTileEntity)altarsWithCraftedItem.ToList()[0];
                AltarUsed.ejectingItem = 60;
                NetMessage.SendData(MessageID.TileEntitySharing, number: AltarUsed.ID, number2: AltarUsed.Position.X, number3: AltarUsed.Position.Y);
            };
        }
    }
}
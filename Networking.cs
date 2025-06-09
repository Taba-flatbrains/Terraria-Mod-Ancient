using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader.IO;
using Ancient.src.Code.Tiles;
using Terraria.ModLoader;
using Ancient.src.Code.Items.Scrolls;
using Terraria.Localization;

namespace Ancient
{
    partial class Ancient
    {
        internal enum MessageType : byte
        {
            InteractWithTileEntity
            // int:Tile Entity Type
            // int:Tile Entity X
            // int:TIle Entity Y
            // Item:Item interacted with
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MessageType msgType = (MessageType)reader.ReadByte();

            switch (msgType)
            {
                case MessageType.InteractWithTileEntity:
                    int EntityType = reader.ReadInt32();
                    int EntityX = reader.ReadInt32();
                    int EntityY = reader.ReadInt32();
                    Item UsedItem = ItemIO.Receive(reader, true);
                    if (ModContent.TileEntityType<AncientAltarTileEntity>() == EntityType)
                    {
                        if (TileUtils.TryGetTileEntityAs(EntityX, EntityY, out AncientAltarTileEntity entity))
                        {
                            entity.ScrollItem = (Scroll)ModContent.GetModItem(UsedItem.type);
                            NetMessage.SendData(MessageID.TileEntitySharing, number: entity.ID, number2: entity.Position.X, number3: entity.Position.Y);
                        }
                    } else if (ModContent.TileEntityType<EnchanterTileEntity>() == EntityType)
                    {
                        if (TileUtils.TryGetTileEntityAs(EntityX, EntityY, out EnchanterTileEntity entity))
                        {
                            entity.ReforgedItem = UsedItem;
                            NetMessage.SendData(MessageID.TileEntitySharing, number: entity.ID, number2: entity.Position.X, number3: entity.Position.Y);
                        }
                    }
                            
                    

                    break;
                default:
                    Logger.WarnFormat("ExampleMod: Unknown Message type: {0}", msgType);
                    break;
            }
        }
    }
}

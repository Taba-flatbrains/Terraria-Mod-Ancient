using Ancient.src.Code.Items.Scrolls;
using Ancient.src.Code.Items.Usables.Totems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Common.Systems
{
    internal class ChestItemWorld : ModSystem
    {
        public override void PostWorldGen()
        {
            base.PostWorldGen();
            int[] itemsToPlaceInFrozenChests = { ModContent.ItemType<EyeOfSkadiScroll>() };
            int[] itemsToPlaceInShadowChests = { ModContent.ItemType<SharangaScroll>() };
            int[] itemsToPlaceInJungleChests = { ModContent.ItemType<HealingTotem>() };

            int itemsPlaced = 0; int itemsToPlaceInFrozenChestsChoice = 0; int itemsToPlaceInShadowChestsChoice = 0; int itemsToPlaceInJungleChestsChoice = 0;// temp variables

            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest == null)
                {
                    continue;
                }
                Tile chestTile = Main.tile[chest.x, chest.y];

                if (chestTile.TileType == TileID.Containers && chestTile.TileFrameX == 11 * 36) // 11 ist die internal tile id von frozen chests -1
                {
                    if (WorldGen.genRand.NextBool(4)) // 25% chance
                        continue;
                    for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {
                            // Place the item
                            chest.item[inventoryIndex].SetDefaults(itemsToPlaceInFrozenChests[itemsToPlaceInFrozenChestsChoice]);
                            // Decide on the next item that will be placed.
                            itemsToPlaceInFrozenChestsChoice = (itemsToPlaceInFrozenChestsChoice + 1) % itemsToPlaceInFrozenChests.Length;
                            // Alternate approach: Random instead of cyclical: chest.item[inventoryIndex].SetDefaults(WorldGen.genRand.Next(itemsToPlaceInFrozenChests));
                            itemsPlaced++;
                            break;
                        }
                    }
                }

                if (chestTile.TileType == TileID.Containers && chestTile.TileFrameX == 4 * 36) 
                {
                    if (WorldGen.genRand.NextBool(4)) // 25% chance
                        continue;
                    for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {
                            // Place the item
                            chest.item[inventoryIndex].SetDefaults(itemsToPlaceInShadowChests[itemsToPlaceInShadowChestsChoice]);
                            // Decide on the next item that will be placed.
                            itemsToPlaceInShadowChestsChoice = (itemsToPlaceInShadowChestsChoice + 1) % itemsToPlaceInShadowChests.Length;
                            // Alternate approach: Random instead of cyclical: chest.item[inventoryIndex].SetDefaults(WorldGen.genRand.Next(itemsToPlaceInFrozenChests));
                            itemsPlaced++;
                            break;
                        }
                    }
                }

                if (chestTile.TileType == TileID.Containers && chestTile.TileFrameX == 10 * 36) 
                {
                    if (WorldGen.genRand.NextBool(4)) // 25% chance
                        continue;
                    for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {
                            // Place the item
                            chest.item[inventoryIndex].SetDefaults(itemsToPlaceInJungleChests[itemsToPlaceInJungleChestsChoice]);
                            // Decide on the next item that will be placed.
                            itemsToPlaceInJungleChestsChoice = (itemsToPlaceInJungleChestsChoice + 1) % itemsToPlaceInJungleChests.Length;
                            // Alternate approach: Random instead of cyclical: chest.item[inventoryIndex].SetDefaults(WorldGen.genRand.Next(itemsToPlaceInFrozenChests));
                            itemsPlaced++;
                            break;
                        }
                    }
                }
            }
        }
    }
}

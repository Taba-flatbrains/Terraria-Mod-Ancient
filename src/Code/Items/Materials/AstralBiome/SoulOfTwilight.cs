using Ancient.src.Code.Tiles.AstralBiome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Ancient.src.Code.Items.Materials.AstralBiome
{
    internal class SoulOfTwilight : ModItem
    {
        public override void SetDefaults()
        {
            Item.material = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 10000;
            Item.rare = ItemRarityID.Cyan;
            Item.width = 20;
            Item.height = 20;
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 59;

            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation

            ItemID.Sets.ItemIconPulse[Item.type] = true; // The item pulses while in the player's inventory
            ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity

            Item.ResearchUnlockCount = 25; // Configure the amount of this item that's needed to research it in Journey mode.
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, new Vector3(0.1f, 0.1f, 0.5f) * Main.essScale); // Makes this item glow when thrown out of inventory.
        }
    }
}

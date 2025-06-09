using Ancient.src.Code.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Ancient.src.Code.Items.Materials;
using Ancient.src.Code.Buffs;
using Microsoft.Xna.Framework;

namespace Ancient.src.Code.Items.Accessoires
{
    internal class ElectronicAssistant : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.value = Item.sellPrice(0, 3);
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Lime;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.MartianConduitPlating, 30)
                .AddIngredient(ModContent.ItemType<ImprovisedCircuitBoard>(), 10)
                .AddTile(ModContent.TileType<AdvancedMechanicsWorkbenchTile>()).Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active) { continue; }

                if (Vector2.Distance(npc.Center, player.Center) < 16 * 30 && !npc.friendly)
                {
                    npc.AddBuff(ModContent.BuffType<DetectedBuff>(), 60 * 5);
                }
            }
        }
    }
}

using Ancient.src.Code.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Items.Accessoires
{
    internal class EyeOfSkadi : ModItem
    {
        public override void SetDefaults() 
        {
            Item.accessory = true;
            Item.defense = 2;
            Item.value = Item.sellPrice(0, 1);
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.IceBlock, 30).AddIngredient(ItemID.Sapphire, 10).AddTile(ModContent.TileType<AncientAltarTile>())
                .AddOnCraftCallback(AltarCraftingCallbacks.GetOnCraftCallback(Type))
                .AddCondition(AltarCraftingConditions.EyeOfSkadi).Register(); 
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FrostburnAfflictingPlayer>().eyeOfSkadiActive = true;
        }
    }

    public class FrostburnAfflictingPlayer : ModPlayer
    {
        public bool eyeOfSkadiActive = false;  
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (eyeOfSkadiActive && !target.friendly) 
            {
                target.AddBuff(BuffID.Frostburn, 60 * 5);
                target.AddBuff(BuffID.Frozen, 60 * 1);
            }
        }

        public override void ResetEffects()
        {
            eyeOfSkadiActive = false;
        }
    }
}

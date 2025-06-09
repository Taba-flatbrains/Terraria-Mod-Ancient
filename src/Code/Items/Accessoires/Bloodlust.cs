using Ancient.src.Code.Items.Materials.Bars;
using Ancient.src.Code.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Ancient.src.Code.Items.Accessoires
{
    internal class Bloodlust : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.value = Item.sellPrice(0, 5);
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Pink;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.SoulofFright, 15)
                .AddTile(ModContent.TileType<AncientAltarTile>())
                .AddOnCraftCallback(AltarCraftingCallbacks.GetOnCraftCallback(Type))
                .AddCondition(AltarCraftingConditions.Bloodlust).Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<BloodlustPlayer>().active = true;
        }
    }

    public class BloodlustPlayer : ModPlayer
    {
        public bool active = false;

        public override void ResetEffects()
        {
            active = false;
        }

        public override void UpdateEquips()
        {
            if (!active)
            {
                return;
            }

            float multiplier = 2 - ((float)Player.statLife / (float)Player.statLifeMax);
            Player.moveSpeed += 0.1f * multiplier;
            Player.GetAttackSpeed(DamageClass.Magic) += 0.05f * multiplier;
            Player.GetAttackSpeed(DamageClass.Ranged) += 0.05f * multiplier;
            Player.GetAttackSpeed(DamageClass.Melee) += 0.05f * multiplier;
        }
    }
}

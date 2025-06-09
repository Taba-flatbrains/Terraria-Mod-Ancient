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
    internal class BloodStone : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.value = Item.sellPrice(0, 6);
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Yellow;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.SpectreBar, 10).AddIngredient(ItemID.Ruby, 30).AddIngredient(ItemID.LifeCrystal, 2).AddIngredient(ItemID.ManaCrystal, 2)
                .AddTile(ModContent.TileType<AncientAltarTile>())
                .AddOnCraftCallback(AltarCraftingCallbacks.GetOnCraftCallback(Type))
                .AddCondition(AltarCraftingConditions.EyeOfSkadi).Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<BloodStonePlayer>().active = true;
        }
    }

    public class BloodStonePlayer : ModPlayer
    {
        public bool active = false;

        public override void ResetEffects()
        {
            active = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.SpawnedFromStatue || NPCID.TargetDummy == target.type) // can not lifesteal from statue spawned enemies or target dummies
            {
                return;
            }

            if (Player.HasBuff(BuffID.Bleeding) || Player.HasBuff(BuffID.MoonLeech)) // No heal during antiheal effects
            {
                return;
            }

            if (hit.DamageType.CountsAsClass(DamageClass.Magic) && active)
            {
                int EmptyMana = Player.statManaMax2 - Player.statMana;
                int OriginalManaRegened = Math.Min(20, damageDone / 10);
                int OverflowHealthRegen = Math.Max(0, OriginalManaRegened - EmptyMana);
                if (OverflowHealthRegen > 0)
                {
                    Player.Heal(OverflowHealthRegen / 3);
                }
                Player.statMana += OriginalManaRegened - OverflowHealthRegen;
            }
        }
    }
}

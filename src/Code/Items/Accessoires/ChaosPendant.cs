using Ancient.src.Code.Items.Materials.Bars;
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
    // inspired from chaos knight's chaos strike
    internal class ChaosPendant : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.value = Item.sellPrice(0, 20);
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Purple;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<DarkSteelBar>(), 12).AddTile(ModContent.TileType<AncientAltarTile>())
                .AddOnCraftCallback(AltarCraftingCallbacks.GetOnCraftCallback(Type))
                .AddCondition(AltarCraftingConditions.ChaosPendant).Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ChaosPendantHoldingPlayer>().HasChaosPendant = true;
        }
    }

    public class ChaosPendantHoldingPlayer : ModPlayer
    {
        public bool HasChaosPendant = false;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (HasChaosPendant)
            {
                modifiers.CritDamage += 0.2f; // + 20% crit damage
            }
        }

        private int cooldown = 10;
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

            if (HasChaosPendant && hit.Crit && cooldown <= 0)
            {
                cooldown = 15;
                Player.Heal(Math.Min((int)Math.Floor(hit.Damage*0.025)+2, 15)); // heal by 10% of the damage done when critting, capped 20
            }
        }

        public override void ResetEffects()
        {
            HasChaosPendant = false;
            cooldown--;
        }
    }
}

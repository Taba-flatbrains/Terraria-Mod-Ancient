using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Ancient.src.Code.Tiles;
using Ancient.src.Code.Dusts;
using Terraria.Audio;

namespace Ancient.src.Code.Items.Accessoires
{
    [AutoloadEquip(EquipType.Shield)]
    internal class LivingwoodBarrier : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.value = Item.sellPrice(0, 1, 80);
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddRecipeGroup(RecipeGroupID.Wood, 100).AddIngredient(ItemID.Emerald, 20).AddTile(ModContent.TileType<AncientAltarTile>())
                .AddOnCraftCallback(AltarCraftingCallbacks.GetOnCraftCallback(Type))
                .AddCondition(AltarCraftingConditions.LivingwoodBarrier).Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<LivingwoodBarrierShieldedPlayer>().ShieldLayers = Math.Min(player.GetModPlayer<LivingwoodBarrierShieldedPlayer>().ShieldLayers+1,
                LivingwoodBarrierShieldedPlayer.MaxShieldLayers);
            Item.defense = (int)Math.Round((decimal)player.GetModPlayer<LivingwoodBarrierShieldedPlayer>().ShieldLayers / LivingwoodBarrierShieldedPlayer.MaxShieldLayers
                * LivingwoodBarrierShieldedPlayer.MaxDefense);
        }
    }

    public class LivingwoodBarrierShieldedPlayer : ModPlayer
    {
        public int ShieldLayers = 0;  // ticks charged
        public static int MaxShieldLayers = 60 * 30;
        public static int MaxDefense = 20;
        public static int ShieldLayersLostOnHit = 60 * 10;

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (ShieldLayers > MaxShieldLayers/3*2+1)
            {
                AnimateOnHit();
            }
            ShieldLayers = Math.Max(ShieldLayers-ShieldLayersLostOnHit, 0);
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            ShieldLayers = Math.Max(ShieldLayers - ShieldLayersLostOnHit, 0);
        }

        private void AnimateOnHit()
        {
            SoundEngine.PlaySound(SoundID.Item70, position: Player.Center);
            for (int i = 0; i<30;i++)
            {
                Dust.NewDust(Player.Center, 1, 1, ModContent.DustType<LivingwoodBarrierDust>());
            }
        }
    }
}

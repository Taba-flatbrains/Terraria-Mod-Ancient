using Microsoft.Xna.Framework;
using System;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Items.Armor.Setless
{
    [AutoloadEquip(EquipType.Legs)]
    internal class BootsOfTravel : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.buyPrice(platinum: 1, gold: 50); // How many coins the item is worth
            Item.rare = ItemRarityID.Red; // The rarity of the item
            Item.defense = 11; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<BootsOfTravelPlayer>().active = true;
        }
    }

    public class BootsOfTravelPlayer : ModPlayer
    {
        public bool active = false;
        private int timer = 0;
        public override void ResetEffects()
        {
            active = false;
        }

        public override void UpdateEquips()
        {
            if (active)
            {
                Player.moveSpeed += 1.5f;
                Player.GetAttackSpeed(DamageClass.Generic) += 0.20f;
            }
        }

        public override void PostUpdateRunSpeeds()
        {
            if (MathF.Abs(Player.velocity.Y) > 0.5f) { return; }

            for (int i = 0; i < MathF.Abs(Player.velocity.X) * 2; i++)
            {
                if (new Random().Next(60) == 0)
                {
                    Dust.NewDust(Player.Center + new Vector2(-2, 20), 4, 4, DustID.Flare, SpeedX: -Player.velocity.X * 0.3f, SpeedY: -0.7f);
                    Dust.NewDust(Player.Center + new Vector2(-2, 20), 4, 4, DustID.Torch, SpeedX: -Player.velocity.X * 0.3f, SpeedY: -0.7f);
                }
            }
        }
    }
}

using Ancient.src.Code.Items.Accessoires;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Ancient.src.Code.Buffs
{
    internal class TotemManaRegenerationBuff : ModBuff
    {
    }

    internal class TotemManaRegenerationBuffPlayer : ModPlayer
    {
        private int ticks = 0;
        public override void PreUpdate()
        {
            if (Player.HasBuff<TotemManaRegenerationBuff>())
            {
                ticks++;
                if (ticks % 12 == 0 && Player.statMana < Player.statManaMax2 && Player.manaRegenDelay <= 15)
                {
                    Player.statMana += 1;
                }
            }

            charge *= 0.995f;
            if (charge < 1) {
                charge = 1;
            }
        }

        // charge manathirst totem
        float charge = 1;
        public override void OnConsumeMana(Item item, int manaConsumed)
        {
            if (ShamansArmuletPlayer.HasItem(Player))
            {
                charge += manaConsumed * 0.006f;
            }
        }

        public static float GetCharge(Player player)
        {
            return player.GetModPlayer<TotemManaRegenerationBuffPlayer>().charge;
        }
    }
}

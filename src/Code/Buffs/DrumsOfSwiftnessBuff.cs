using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Ancient.src.Code.Buffs
{
    internal class DrumsOfSwiftnessBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<DrumsOfSwiftnessBuffPlayer>().active = true;
        }
    }

    public class DrumsOfSwiftnessBuffPlayer : ModPlayer
    {
        public bool active = false;
        public override void ResetEffects()
        {
            active = false;
        }

        public override void UpdateEquips()
        {
            if (active)
            {
                Player.moveSpeed += 0.15f;
                Player.GetAttackSpeed(DamageClass.Generic) += 0.15f;
            }
        }
    }
}

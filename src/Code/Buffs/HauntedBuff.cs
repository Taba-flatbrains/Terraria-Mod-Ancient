using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Ancient.src.Code.Buffs
{
    internal class HauntedBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<HauntedBuffPlayer>().active = true;
        }
    }

    internal class HauntedBuffPlayer : ModPlayer
    {
        public bool active = false;
        public int value = 0;

        public override void ResetEffects()
        {
            active = false;
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (active)
            {
                // Apply a flat bonus to every hit
                // idk how it works
            }
        }
    }

    internal class HauntedBuffNPC : GlobalNPC
    {
        public int value = 0;
        public override bool InstancePerEntity => true;

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // Only player attacks should benefit from this buff, hence the NPC and trap checks.
            if (projectile.npcProj || projectile.trap)
                return;

            if (npc.HasBuff<HauntedBuff>())
            {
                // Apply a flat bonus to every hit
                modifiers.FlatBonusDamage += value;
            }
        }

        public override void AI(NPC npc)
        {
            if (!npc.HasBuff<HauntedBuff>())
            {
                value = 0;
            }

            if (value > 30)
            {
                value--;
            }
        }
    }
}

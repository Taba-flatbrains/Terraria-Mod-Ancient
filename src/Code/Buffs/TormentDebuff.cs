using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Ancient.src.Code.Dusts;

namespace Ancient.src.Code.Buffs
{
    public class TormentDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // This allows the debuff to be inflicted on NPCs that would otherwise be immune to all debuffs.
            // Other mods may check it for different purposes.
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }

    public class TormentDebuffNPC : GlobalNPC
    {
        public override void AI(NPC npc)
        {
            if (npc.HasBuff<TormentDebuff>())
            {
                for (int i = 0; i < 1 + (npc.width * npc.height/2000); i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<DarkSteelWeaponsDust2>());
                }
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // Only player attacks should benefit from this buff, hence the NPC and trap checks.
            if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated)
                return;


            // SummonTagDamageMultiplier scales down tag damage for some specific minion and sentry projectiles for balance purposes.
            var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
            if (npc.HasBuff<TormentDebuff>())
            {
                // Apply a flat bonus to every hit
                modifiers.CritDamage += 0.2f;
                if (new Random().NextSingle() < 0.1f)
                {
                    modifiers.SetCrit();
                }
            }
        }
    }
}

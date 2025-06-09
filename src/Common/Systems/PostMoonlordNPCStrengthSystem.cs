using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Ancient.src.Common.Systems
{
    internal class PostMoonlordNPCStrengthSystem : GlobalNPC
    {
        public override void SetDefaults(NPC entity)  
        {
            if (NPC.downedMoonlord)  // does not work for some slimes for some reason
            {
                if ((!entity.boss) && (!entity.CountsAsACritter) && (!entity.isLikeATownNPC) && (!entity.townNPC))
                {
                    entity.lifeMax *= 2;
                    entity.damage = (int)(entity.damage * 1.5f);
                    entity.defense = (int)(entity.defense * 1.5f);
                    entity.value *= (int)(entity.value * 1.5f);
                }
            }
        }
    }
}

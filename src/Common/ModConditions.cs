using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Ancient.src.Common
{
    internal class ModConditions
    {
        public static readonly Condition LowHP = new Condition("Mods.Ancient.Conditions.LowHP", () => (float)Main.LocalPlayer.statLife / (float)Main.LocalPlayer.statLifeMax < 0.2f);
    }
}

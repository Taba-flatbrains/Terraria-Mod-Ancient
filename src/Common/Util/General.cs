using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Ancient.src.Common.Util
{
    internal static class GeneralUtil
    {
        public static int ProjectileDamageMultiplier()
        {
            if (Main.masterMode) { return 3; }
            if (Main.expertMode) { return 2; }
            return 1;
        }
    }
}

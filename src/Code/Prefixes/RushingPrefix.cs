using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Ancient.src.Code.Prefixes
{
    internal class RushingPrefix : ModPrefix
    {
        public override PrefixCategory Category => PrefixCategory.Accessory;

        public override bool CanRoll(Item item)
        {
            return true;
        }

        public override float RollChance(Item item)
        {
            return 0;
        }

        // I think this is only relevant for weapon reforges
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {

        }

        // Modify the cost of items with this modifier with this function.
        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1.6f;
        }

        // idk
        public override void Apply(Item item)
        {

        }

        public override void ApplyAccessoryEffects(Player player)
        {
            player.moveSpeed += 0.05f;
        }

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            yield return new TooltipLine(Mod, "CritChance", "+5% movement speed")
            {
                IsModifier = true, // Sets the color to the positive modifier color.
            };
        }
    }
}

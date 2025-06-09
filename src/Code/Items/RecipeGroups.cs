using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Ancient.src.Code.Items
{
    internal class RecipeGroups : ModSystem
    {
        public static RecipeGroup GemRecipeGroup;
        public override void AddRecipeGroups()
        {
            // Create a recipe group and store it
            // Language.GetTextValue("LegacyMisc.37") is the word "Any" in English, and the corresponding word in other languages
            GemRecipeGroup = new RecipeGroup(() => string.Join(" ", Language.GetTextValue("LegacyMisc.37"), "gem"),
                ItemID.Diamond, ItemID.Ruby, ItemID.Topaz, ItemID.Sapphire, ItemID.Amethyst, ItemID.Emerald);

            // To avoid name collisions, when a modded items is the iconic or 1st item in a recipe group, name the recipe group: ModName:ItemName
            RecipeGroup.RegisterGroup("Ancient:gem", GemRecipeGroup);
        }
    }
}

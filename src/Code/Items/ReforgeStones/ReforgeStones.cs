using Ancient.src.Code.Items.Materials.AstralBiome;
using Ancient.src.Code.Prefixes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Items.ReforgeStones
{
    public class ReforgeStoneUtils
    {
        public static List<int> ReforgeStoneTypes = new List<int>();
    }

    internal abstract class ReforgeStoneBase : ModItem
    {
        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.value = 250;
            Item.rare = ItemRarityID.Cyan;
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 59;
            Item.width = 20;
            Item.height = 20;
            ReforgeStoneUtils.ReforgeStoneTypes.Add(Item.type);
        }

        public abstract int PrefixID { get; }
    }

    internal class ElvenReforgeStone : ReforgeStoneBase // obtainable by elf town npc
    {
        public override int PrefixID => ModContent.PrefixType<BlessedPrefix>();
    }

    internal class CollosalReforgeStone : ReforgeStoneBase // obtainable by killing kiranocif
    {
        public override int PrefixID => ModContent.PrefixType<CollosalPrefix>();
    }

    internal class ShimmeringReforgeStone : ReforgeStoneBase // obtainable by killing elf wizard
    {
        public override int PrefixID => ModContent.PrefixType<WisePrefix>();
    }

    internal class BloodstainedReforgeStone : ReforgeStoneBase // todo: make obtainable by killing some monstrosity in dungeon
    {
        public override int PrefixID => ModContent.PrefixType<BloodstainedPrefix>();
    }

    internal class LevitatingReforgeStone : ReforgeStoneBase // Craftbar
    {
        public override int PrefixID => ModContent.PrefixType<RushingPrefix>();

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.StoneBlock, 10).AddIngredient(ModContent.ItemType<ArcaneShard>(), 10).AddTile(TileID.MythrilAnvil).Register();
        }
    }

    internal class UnholyReforgeStone : ReforgeStoneBase // Demon in Underworld
    {
        public override int PrefixID => ModContent.PrefixType<UnholyPrefix>();
    }

    internal class StrategistsStone : ReforgeStoneBase // Pirate Invasion Post Big3
    {
        public override int PrefixID => ModContent.PrefixType<StrategicPrefix>();
    }

    internal class AncientClub : ReforgeStoneBase // Ocean Temple & Ancient Ruin
    {
        public override int PrefixID => ModContent.PrefixType<ForcefulPrefix>();
    }
}

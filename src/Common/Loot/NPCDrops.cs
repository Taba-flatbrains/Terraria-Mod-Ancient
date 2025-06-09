using Ancient.src.Code.Items.Armor.Setless;
using Ancient.src.Code.Items.Materials;
using Ancient.src.Code.Items.ReforgeStones;
using Ancient.src.Code.Items.Scrolls;
using Ancient.src.Code.Items.Usables.Weapons;
using Ancient.src.Code.NPCS.Hostile;
using Ancient.src.Code.NPCS.Hostile.Tentacles;
using Ancient.src.Code.NPCS.Invasion.Elf;
using Ancient.src.Code.NPCS.Invasion.Elf.Miniboss;
using Ancient.src.Code.NPCS.Spirits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;



namespace Ancient.src.Common.Loot
{
    internal class NPCDrops : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == ModContent.NPCType<Bull>())
            {
                npcLoot.Add(ItemDropRule.Common(ItemID.Leather, 1, 2, 4));
            }
            if (npc.type == ModContent.NPCType<RottenTentacle>())
            {
                npcLoot.Add(ItemDropRule.Common(ItemID.RottenChunk, 1, 1, 1));
            }
            if (npc.type == ModContent.NPCType<BloodyTentacle>())
            {
                npcLoot.Add(ItemDropRule.Common(ItemID.Vertebrae, 1, 1, 1));
            }
            if (npc.type == ModContent.NPCType<AngryBook>())
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<UnknownScroll>(), 2, 1, 1));
            }
            if (npc.type == ModContent.NPCType<VodooSpirit>())
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<UnknownScroll>(), 100, 1, 1));
            }
            if (npc.type == ModContent.NPCType<PoisonDartFrog>())
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Poison>(), 1, 1, 2));
            }
            if (npc.type == ModContent.NPCType<HighElfSorcerer>())
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShimmeringReforgeStone>(), 15, 1, 1));
            }
            if (npc.type == ModContent.NPCType<Succubus>())
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<UnholyReforgeStone>(), 5, 1, 1));
            }
            if (npc.type == ModContent.NPCType<Windwaker>())
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BootsOfTravel>(), 10, 1, 1));
            }
            if (npc.type == ModContent.NPCType<Naga>())
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MarvelScale>(), 2, 1, 2));
            }
            if (npc.type == ModContent.NPCType<DarkElfRanger>())
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Silverstream>(), 10, 1, 1));
            }
            if (npc.type == ModContent.NPCType<Kiranocif>())
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CollosalReforgeStone>(), 1, 1, 1));
            }
            // todo: add unchained beast bloodstained reforge stone drop
        }
    }
}

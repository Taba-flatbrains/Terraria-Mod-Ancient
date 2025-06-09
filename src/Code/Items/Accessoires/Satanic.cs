using Ancient.src.Code.Items.Materials.Bars;
using Ancient.src.Code.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Ancient.src.Code.Buffs;

namespace Ancient.src.Code.Items.Accessoires
{
    internal class Satanic : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.value = Item.sellPrice(0, 20);
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Expert;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SatanicPlayer>().active = true;
            player.GetModPlayer<SatanicPlayer>().cooldown--;
        }


        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int line = 0; line < tooltips.Count; line++)
            {
                if (tooltips[line].Text.Contains('{'))
                {
                    tooltips[line].Text = string.Format(tooltips[line].Text, ActiveAccessoryKeybindSystem.ActiveAccessoryKeybind.GetAssignedKeys()[0]);
                }
            }
        }
    }

    public class SatanicPlayer : ModPlayer
    {
        public bool active = false;
        public int cooldown = 0;
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Player.CCed) { return; }
            if (ActiveAccessoryKeybindSystem.ActiveAccessoryKeybind.JustPressed && cooldown < 1 && Main.myPlayer == Player.whoAmI && active)
            {
                cooldown = 120 * 60;

                Player.AddBuff(ModContent.BuffType<SatanicBuff>(), 60 * 10);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (active)
            {
                modifiers.ArmorPenetration += 10;
            }
        }

        public override void ResetEffects()
        {
            active = false;
        }
    }

}
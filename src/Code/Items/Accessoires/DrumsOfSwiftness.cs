using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Ancient.src.Code.Projectiles;
using Terraria.Audio;
using Terraria.GameInput;
using Ancient.src.Code.Buffs;
using Microsoft.Xna.Framework;

namespace Ancient.src.Code.Items.Accessoires
{
    internal class DrumsOfSwiftness : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.value = Item.sellPrice(0, 3);
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Pink;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Leather, 10).AddIngredient(ItemID.SoulofSight, 15).AddIngredient(ItemID.Wood, 30).AddTile(TileID.WorkBenches).Register();
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

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<DrumsOfSwiftnessPlayer>().active = true;
            player.GetModPlayer<DrumsOfSwiftnessPlayer>().cooldown--;
        }
    }

    public class DrumsOfSwiftnessPlayer : ModPlayer
    {
        public bool active = false;
        public int cooldown = 0;
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Player.CCed) { return; }
            if (ActiveAccessoryKeybindSystem.ActiveAccessoryKeybind.JustPressed && cooldown < 1 && Main.myPlayer == Player.whoAmI && active)
            {
                cooldown = 40 * 60;
                foreach (var player in Main.player)
                {
                    if (Vector2.Distance(Player.Center, player.Center) < 16 * 40)
                    {
                        player.AddBuff(ModContent.BuffType<DrumsOfSwiftnessBuff>(), 60 * 10, quiet: false);
                    }
                }
            }
        }

        public override void ResetEffects()
        {
            active = false;
        }
    }
}

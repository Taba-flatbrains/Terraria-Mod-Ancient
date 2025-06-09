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
using Microsoft.Xna.Framework;
using Ancient.src.Code.Items.Materials.Bars;
using Ancient.src.Code.Tiles;

namespace Ancient.src.Code.Items.Accessoires
{
    internal class Mekansm : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.value = Item.sellPrice(0, 2);
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<BronzeBar>(), 15).AddIngredient(ItemID.Diamond, 10).AddTile(ModContent.TileType<AncientAltarTile>())
                .AddOnCraftCallback(AltarCraftingCallbacks.GetOnCraftCallback(Type))
                .AddCondition(AltarCraftingConditions.Mekansm).Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MekansmPlayer>().active = true;
            player.GetModPlayer<MekansmPlayer>().cooldown--;
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

    public class MekansmPlayer : ModPlayer
    {
        public bool active = false;
        public int cooldown = 0;
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Player.CCed) { return; }
            if (ActiveAccessoryKeybindSystem.ActiveAccessoryKeybind.JustPressed && cooldown < 1 && Main.myPlayer == Player.whoAmI && active)
            {
                cooldown = 60 * 60;

                SoundStyle sound = SoundID.DD2_DarkMageHealImpact;
                sound.Volume *= 1.7f;
                SoundEngine.PlaySound(sound, Player.Center);

                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];

                    // Skip non-active players
                    if (player.active)
                    {
                        // Calculate the distance between the NPC and the player
                        float distance = Vector2.Distance(Player.Center, player.Center);

                        // Check if the current player is closer than the previous nearest player
                        if (distance < 1000)
                        {
                            player.Heal(100);
                        }
                    }
                }
            }
        }

        public override void UpdateLifeRegen()
        {
            if (active)
            {
                Player.lifeRegen += (int)(Player.lifeRegen * 1.2f);
            }
        }

        public override void ResetEffects()
        {
            active = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

using Terraria;
using Steamworks;
using Terraria.GameInput;
using Microsoft.Xna.Framework;
using Ancient.src.Code.Projectiles;
using Terraria.Audio;

namespace Ancient.src.Code.Items.Accessoires
{
    internal class ShivasGuard : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.defense = 4;
            Item.value = Item.sellPrice(0, 4);
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Pink;
            Item.damage = 46;
            Item.DamageType = DamageClass.Magic;
        }

        public override void AddRecipes()
        { 
            CreateRecipe().AddIngredient(ItemID.FrostCore, 1).AddIngredient(ItemID.SoulofNight, 15).AddTile(TileID.MythrilAnvil).Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ShivasGuardPlayer>().active = true;
            player.GetModPlayer<ShivasGuardPlayer>().cooldown--;
            player.GetModPlayer<ShivasGuardPlayer>().damage = Item.damage;
        }

        public override bool MagicPrefix()
        {
            return false;
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

    public class ShivasGuardPlayer : ModPlayer
    {
        public int damage = 20;
        public bool active = false;
        public int cooldown = 0;
        public static int projectile_count = 30;
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Player.CCed) { return; }
            if (ActiveAccessoryKeybindSystem.ActiveAccessoryKeybind.JustPressed && cooldown < 1 && Main.myPlayer == Player.whoAmI && active)
            {
                cooldown = 10*60;
                SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, Player.Center);
                for (int projectile = 0; projectile < projectile_count; projectile++)
                {
                    int projectile_index = Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, new Vector2(), ModContent.ProjectileType<ShivasGuardProjectile>(), damage, 0, Main.myPlayer);
                    ((ShivasGuardProjectile)Main.projectile[projectile_index].ModProjectile).projectile_number = projectile;
                }
            }
        }

        public override void ResetEffects()
        {
            active = false;
        }
    }
}

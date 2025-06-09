using Ancient.src.Code.Buffs;
using Ancient.src.Code.Items.Materials;
using Ancient.src.Code.Items.Materials.AstralBiome;
using Ancient.src.Code.Items.Materials.Bars;
using Ancient.src.Code.Projectiles;
using Ancient.src.Code.Tiles;
using Ancient.src.Code.Tiles.AstralBiome;
using Ancient.src.Common.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Items.Accessoires
{
    internal class PrismarinePearl : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.value = Item.buyPrice(1, 20);
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Red;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<MarvelScale>(), 6).AddIngredient(ModContent.ItemType<SoulOfTwilight>(), 5).AddTile(TileID.MythrilAnvil).Register();
            CreateRecipe().AddIngredient(ModContent.ItemType<MarvelScale>(), 6).AddIngredient(ModContent.ItemType<SoulOfTwilight>(), 5).AddTile(ModContent.TileType<PrimordialOrb>()).Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<PrismarinePearlPlayer>().active = true;
            player.GetModPlayer<PrismarinePearlPlayer>().cooldown--;
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

    public class PrismarinePearlPlayer : ModPlayer
    {
        public bool active = false;
        public int cooldown = 0;
        public bool shielded = false;
        public int ticks = 0;

        private const int duration = 3;
        private const int cd = 60;

        private int DamageAccumulated = 0;
        private int TargetNPC = -1;

        public override void ResetEffects()
        {
            active = false;
            ticks -= 1;
            if (ticks < 0)
            {
                shielded = false;
            }
        }

        private void Enable()
        {
            if (!active || cooldown > 1) { return; }
            cooldown = cd * 60;

            SoundStyle sound = SoundID.Item8;
            SoundEngine.PlaySound(sound, Player.Center);

            ticks = 60 * duration;
            shielded = true;
            DamageAccumulated = 0;

            Player.AddBuff(ModContent.BuffType<PrismarinePearlBuff>(), duration * 60, quiet: false);
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Player.CCed) { return; }
            if (ActiveAccessoryKeybindSystem.ActiveAccessoryKeybind.JustPressed && cooldown < 1 && Main.myPlayer == Player.whoAmI && active)
            {
                Enable();
            }
        }

        public override void PostUpdateEquips()
        {
            if (ticks == 1 && DamageAccumulated >= 2 && TargetNPC != -1)
            {
                SoundEngine.PlaySound(SoundID.Item14, Player.Center);

                Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, new Vector2(4, 4), ModContent.ProjectileType<GlisteningWraith>(), DamageAccumulated/2, 0.5f, Main.myPlayer, TargetNPC);
                Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, new Vector2(-4, -4), ModContent.ProjectileType<GlisteningWraith>(), DamageAccumulated / 2, 0.5f, Main.myPlayer, TargetNPC);

                DamageAccumulated = 0;
                TargetNPC = -1;
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (!active) { return; }
            if (modifiers.GetDamage(npc.damage, Player.statDefense, 0.5f) > Player.statLife && cooldown < 1) // too lazy to correctly assign def effectiveness in different difficulties
            {
                Enable();
            }

            if (shielded)
            {
                SoundEngine.PlaySound(SoundID.NPCHit43, Player.Center);
                DamageAccumulated += (int)(modifiers.GetDamage(npc.damage, Player.statDefense, 0.5f) * 0.9f);
                modifiers.FinalDamage *= 0.1f;
                TargetNPC = npc.whoAmI;
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (!active) { return; }
            if (modifiers.GetDamage(proj.damage * GeneralUtil.ProjectileDamageMultiplier(), Player.statDefense, 0.5f) * GeneralUtil.ProjectileDamageMultiplier() > Player.statLife && cooldown < 1)
            {
                Enable();
            }

            if (shielded)
            {
                SoundEngine.PlaySound(SoundID.NPCHit43, Player.Center);
                DamageAccumulated += (int)(modifiers.GetDamage(proj.damage * GeneralUtil.ProjectileDamageMultiplier(), Player.statDefense, 0.5f) * 0.9f);
                modifiers.FinalDamage *= 0.1f;

                float Distance = 16 * 50;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    var npc = Main.npc[i];
                    if (npc.active && Vector2.Distance(npc.Center, Player.Center) < Distance)
                    {
                        Distance = Vector2.Distance(npc.Center, Player.Center);
                        TargetNPC = npc.whoAmI;
                    }
                }
            }
        }
    }
}

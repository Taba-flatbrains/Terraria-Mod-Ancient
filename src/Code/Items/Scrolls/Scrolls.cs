using Ancient.src.Code.Items.Accessoires;
using Ancient.src.Code.Items.Usables.Consumables;
using Ancient.src.Code.Items.Usables.Weapons;
using Ancient.src.Code.NPCS.Boss.DarknessEmbrace;
using Ancient.src.Code.NPCS.Boss.YrimirsSoul;
using Ancient.src.Code.Tiles;
using Ancient.src.Common.Structures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Items.Scrolls
{
    internal class ScrollConstants
    {
        public static int[] GetScrolls() 
        {
            int[] scrolls = { ModContent.ItemType<SharangaScroll>(), ModContent.ItemType<EyeOfSkadiScroll>(), 
                ModContent.ItemType<IronCoreScroll>(), ModContent.ItemType<IonShellScroll>(), ModContent.ItemType<LivingwoodBarrierScroll>(),
            ModContent.ItemType<ChaosPendantScroll>(), ModContent.ItemType<SuspiciousLookingCrystalScroll>(), ModContent.ItemType<MekansmScroll>(), ModContent.ItemType<BloodStoneScroll>(),
            ModContent.ItemType<BloodlustScroll>()};
            return scrolls;
        }    
    }

    internal abstract class Scroll : ModItem
    {
        public override void SetDefaults()
        {
            Item.material = true;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(0, 1);
            Item.width = 20;
            Item.height = 20;
            
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 60;
        }
        public override void AddRecipes()  // debugging only, scrolls should not be craftable
        {
            //CreateRecipe().AddIngredient(ItemID.DirtBlock, 2).Register();
        }
        public abstract Texture2D ritualResultItemTexture { get; }
        public abstract int ritualResultItemID { get; }
    }

    internal class SharangaScroll : Scroll
    {
        public override void SetDefaults() 
        {
            base.SetDefaults();
        }
        public override void SetStaticDefaults() 
        { 
            base.SetStaticDefaults();
        }
        public override Texture2D ritualResultItemTexture => ModContent.Request<Texture2D>("Ancient/src/Code/Items/Usables/Weapons/Sharanga").Value;

        public override int ritualResultItemID => ModContent.ItemType<Sharanga>();
    }

    internal class NoneScroll : Scroll
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override Texture2D ritualResultItemTexture => ModContent.Request<Texture2D>("Ancient/src/Code/Items/Scrolls/NoneScroll").Value; // only placeholder should not be able to be used

        public override int ritualResultItemID => Type;  // only placeholder should not be able to be used
    }

    internal class EyeOfSkadiScroll : Scroll
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override Texture2D ritualResultItemTexture => ModContent.Request<Texture2D>("Ancient/src/Code/Items/Accessoires/EyeOfSkadi").Value;
        public override int ritualResultItemID => ModContent.ItemType<EyeOfSkadi>();
    }

    internal class IronCoreScroll : Scroll
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override Texture2D ritualResultItemTexture => ModContent.Request<Texture2D>("Ancient/src/Code/Items/Accessoires/IronCore").Value;
        public override int ritualResultItemID => ModContent.ItemType<IronCore>();
    }

    internal class IonShellScroll : Scroll
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.buyPrice(0, 5);
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override Texture2D ritualResultItemTexture => ModContent.Request<Texture2D>("Ancient/src/Code/Items/Accessoires/IonShell").Value;
        public override int ritualResultItemID => ModContent.ItemType<IonShell>();
    }

    internal class LivingwoodBarrierScroll : Scroll
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.buyPrice(0, 5);
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override Texture2D ritualResultItemTexture => ModContent.Request<Texture2D>("Ancient/src/Code/Items/Accessoires/LivingwoodBarrier").Value;
        public override int ritualResultItemID => ModContent.ItemType<LivingwoodBarrier>();
    }

    internal class ChaosPendantScroll : Scroll
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.buyPrice(0, 50);
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override Texture2D ritualResultItemTexture => ModContent.Request<Texture2D>("Ancient/src/Code/Items/Accessoires/ChaosPendant").Value;
        public override int ritualResultItemID => ModContent.ItemType<ChaosPendant>();
    }

    internal class MekansmScroll : Scroll
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.buyPrice(0, 4);
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override Texture2D ritualResultItemTexture => ModContent.Request<Texture2D>("Ancient/src/Code/Items/Accessoires/Mekansm").Value;
        public override int ritualResultItemID => ModContent.ItemType<Mekansm>();
    }

    
    internal class SuspiciousLookingCrystalScroll : Scroll
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.buyPrice(0, 10);
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override Texture2D ritualResultItemTexture => ModContent.Request<Texture2D>("Ancient/src/Code/Items/Usables/Consumables/SuspiciousLookingCrystal").Value;
        public override int ritualResultItemID => ModContent.ItemType<SuspiciousLookingCrystal>();
    }

    internal class BloodStoneScroll : Scroll
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.buyPrice(0, 10);
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override Texture2D ritualResultItemTexture => ModContent.Request<Texture2D>("Ancient/src/Code/Items/Accessoires/BloodStone").Value;
        public override int ritualResultItemID => ModContent.ItemType<BloodStone>();
    }

    internal class BloodlustScroll : Scroll
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.buyPrice(0, 10);
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override Texture2D ritualResultItemTexture => ModContent.Request<Texture2D>("Ancient/src/Code/Items/Accessoires/Bloodlust").Value;
        public override int ritualResultItemID => ModContent.ItemType<Bloodlust>();
    }

    internal class UnknownScroll : ModItem // Not a real scroll
    {
        public override void SetDefaults()
        {
            Item.maxStack = 1;
            Item.value = Item.buyPrice(0, 2);
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Red;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (NPC.downedMoonlord)
            {
                if (Vector2.Distance(AstralBiomeGeneration.PrimordialPearlPosition, Main.LocalPlayer.Center) < 16 * 20)
                {
                    tooltips.Add(new TooltipLine(Mod, "UnknownScroll", "Summons Yrimirs Soul"));
                } else
                {
                    tooltips.Add(new TooltipLine(Mod, "UnknownScroll", "The orb is calling"));
                }
            } else
            {
                tooltips.Add(new TooltipLine(Mod, "UnknownScroll", "You can not read it"));
            }
        }
        
        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<YrimirsSoul>()) && Vector2.Distance(AstralBiomeGeneration.PrimordialPearlPosition, Main.LocalPlayer.Center) < 16 * 20;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                // If the player using the item is the client
                // (explicitly excluded serverside here)
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                int type = ModContent.NPCType<YrimirsSoul>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // If the player is not in multiplayer, spawn directly
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                }
                else
                {
                    // If the player is in multiplayer, request a spawn
                    // This will only work if NPCID.Sets.MPAllowedEnemies[type] is true, which we set in MinionBossBody
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                }
            }

            return true;
        }
    }
}

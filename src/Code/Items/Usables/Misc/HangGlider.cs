using Ancient.src.Code.Items.Materials;
using Ancient.src.Code.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Items.Usables.Misc
{
    internal abstract class HangGliderBase : ModItem
    {
        public abstract string InventoryTextureLocation { get; }
        public virtual float AirResistance => 0.1f;
        public virtual float Acceleration => 0.27f;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.holdStyle = ItemHoldStyleID.HoldUp;
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(ModContent.Request<Texture2D>(InventoryTextureLocation).Value,
                new Rectangle((int)position.X - (int)(frame.Width * scale * 0.5), (int)position.Y - (int)(frame.Height * scale * 0.5), (int)(frame.Width * scale), (int)(frame.Height * scale)),
                drawColor);
            return false;
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X = player.Center.X - (51 * player.direction);
            player.itemLocation.Y -= -20;
            player.itemRotation = 0.1f * player.direction + (new Random().NextSingle() * Math.Min(0.01f * player.velocity.Y * Convert.ToInt32(player.velocity.Y > 0), 0.02f));
            
        }

        public override void UpdateInventory(Player player)
        {
            if (player.inventory[player.selectedItem].type == Type )
            {
                player.GetModPlayer<HangGliderPlayer>().active = true;
                player.GetModPlayer<HangGliderPlayer>().Acceleration = Acceleration;
                player.GetModPlayer<HangGliderPlayer>().AirResistance = AirResistance;
            }
        }
    }

    internal class HangGliderPlayer : ModPlayer
    {
        public bool active = false;

        public float Acceleration = 0;

        public float AirResistance = 0;

        public override void ResetEffects()
        {
            active = false;
        }

        public override void PreUpdateMovement()
        {
            if (Player.velocity.Y > 0.5f && active)
            {
                Player.velocity.X += Player.direction * Player.velocity.Y * Acceleration - (Player.velocity.X * AirResistance);
                Player.velocity.Y /= 1.15f;
            }
        }
    }


    internal class HangGlider : HangGliderBase
    {
        public override string InventoryTextureLocation
        {
            get
            {
                return "Ancient/src/Code/Items/Usables/Misc/HangGliderInInventory";
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Wood, 5).AddIngredient(ItemID.Leather, 6).AddTile(TileID.WorkBenches).Register();
        }
    }

    internal class HarpyGlider : HangGliderBase
    {
        public override string InventoryTextureLocation
        {
            get
            {
                return "Ancient/src/Code/Items/Usables/Misc/HarpyGliderInInventory";
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Wood, 5).AddIngredient(ItemID.Feather, 10).AddIngredient(ItemID.Silk, 5).AddTile(TileID.WorkBenches).Register();
        }

        public override float AirResistance => 0.11f;
        public override float Acceleration => 0.48f;
    }
}

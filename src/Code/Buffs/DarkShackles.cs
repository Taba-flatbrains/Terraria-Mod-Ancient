using Ancient.src.Code.Items.Accessoires;
using Ancient.src.Code.NPCS.Boss.DarknessEmbrace;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Ancient.src.Code.Buffs
{
    public class DarkShacklesBuff : ModBuff
    {

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<DarkShacklesDebuffPlayer>().active = true;
        }
    }

    public class DarkShacklesDebuffPlayer : ModPlayer
    {
        public bool active = false;
        public override void ResetEffects()
        {
            active = false;
        }
        public override void PreUpdateMovement()
        {
            if (active)
            {
                Player.velocity *= 0.4f;
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (active)
            {
                Texture2D texture = ModContent.Request<Texture2D>("Ancient/src/Code/Buffs/DarkShacklesBuffAnimation").Value;
                Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                //Vector2 drawPos = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X + (Main.screenWidth / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + (Main.screenHeight / 2)));

                Vector2 drawOrigin = new Vector2(texture.Width * -0.5f, texture.Height * -0.5f);
                Vector2 drawPos = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2) + drawOrigin;
                Main.spriteBatch.Draw(texture, drawPos, sourceRectangle, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }   
        }

        public override void OnEnterWorld()
        {
            Terraria.Graphics.Effects.Filters.Scene.Activate("Shackled");
        }
    }

    public class ShackledShaderData : ScreenShaderData
    {

        public ShackledShaderData(Asset<Effect> shader, string passName) : base(shader, passName)
        {
        }


        public override void Apply()
        {
            if (Main.LocalPlayer.buffType.Contains(ModContent.BuffType<DarkShacklesBuff>()))
            {

                UseIntensity(1);
            } else
            {
                UseIntensity(0);
            }


            base.Apply();
        }
    }
}

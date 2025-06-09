using Ancient.src.Code.Items.Scrolls;
using Ancient.src.Code.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Ancient.src.Common.UI
{
    /*
    // gave up on this idea
    internal class AncientAltarUI : UIState
    {
        public static bool visible;
        public static AncientAltarTileEntity ancientAltarTileEntity;
        private int UIHeight = 600;
        private int UIWidth = 600;

        private UIElement area;
        public static CustomItemSlot scrollslot;
        private UIImage backgroundTexture;

        public override void OnInitialize()
        {
            visible = false;
            ancientAltarTileEntity = null;
            area = new UIElement();
            // centered
            area.Left.Set(-UIWidth / 2, 0.5f);
            area.Top.Set(-UIHeight / 2, 0.5f);
            area.Width.Set(UIWidth, 0f);
            area.Height.Set(UIHeight, 0f);

            backgroundTexture = new UIImage(ModContent.Request<Texture2D>("Ancient/src/Common/UI/AncientAltarUI"));
            backgroundTexture.Left.Set(0, 0f);
            backgroundTexture.Top.Set(0, 0f);
            backgroundTexture.Width.Set(UIWidth, 0f);
            backgroundTexture.Height.Set(UIHeight, 0f);

            scrollslot = new CustomItemSlot();
            scrollslot.Left.Set(-20, 0.5f);
            scrollslot.Top.Set(-25, 0.5f);

            
            area.Append(backgroundTexture);
            area.Append(scrollslot);
            Append(area);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!visible)
                return;
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // block 3x3 big, 1.5x1.5 offset needed because block starts at upper left edge
            if (ancientAltarTileEntity == null) { return; }
            if (Math.Sqrt(Math.Pow(ancientAltarTileEntity.Position.X + 1.5 - (Main.LocalPlayer.position.X / 16), 2) + Math.Pow(ancientAltarTileEntity.Position.Y + 1.5 - (Main.LocalPlayer.position.Y / 16), 2)) > 6)
            {
                visible = false; // close UI when walking away
            }
            
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                
            }
        }
    }

    [Autoload(Side = ModSide.Client)]
    internal class AncientAltarUISystem : ModSystem
    {
        private UserInterface AncientAltarUIUI;
        internal AncientAltarUI AncientAltarUI;

        public override void Load()
        {
            AncientAltarUI = new();
            AncientAltarUIUI = new();
            AncientAltarUIUI.SetState(AncientAltarUI);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            AncientAltarUIUI?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            layers.Insert(5, new LegacyGameInterfaceLayer("Ancient: Altar UI", delegate { AncientAltarUIUI.Draw(Main.spriteBatch, new GameTime()); return true; }, InterfaceScaleType.UI));
        }
    }
    */
}

using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Ancient.src.Code.NPCS.Boss.DarknessEmbrace;
using Ancient.src.Code.Buffs;
using Ancient.src.Common.Structures;

namespace Ancient
{
	partial class Ancient : Mod
	{
        public override void Load()
        {

            // All of this loading needs to be client-side.

            if (Main.netMode != NetmodeID.Server)
            {
                // First, you load in your shader file.
                // You'll have to do this regardless of what kind of shader it is,
                // and you'll have to do it for every shader file.
                // This example assumes you have both armour and screen shaders.

                //Ref<Effect> dyeRef = new Ref<Effect>(GetEffect("Effects/MyDyes"));
                //Ref<Effect> specialRef = new Ref<Effect>(GetEffect("Effects/MySpecials"));
                //Ref<Effect> filterRef = new Ref<Effect>(GetEffect("Effects/MyFilters"));

                // To add a dye, simply add this for every dye you want to add.
                // "PassName" should correspond to the name of your pass within the *technique*,
                // so if you get an error here, make sure you've spelled it right across your effect file.

                //GameShaders.Armor.BindShader(ItemType<MyDyeItem>(), new ArmorShaderData(dyeRef, "PassName"));

                // If your dye takes specific parameters such as colour, you can append them after binding the shader.
                // IntelliSense should be able to help you out here.  

                //GameShaders.Armor.BindShader(ItemType<MyColourDyeItem>(), new ArmorShaderData(dyeRef, "ColourPass")).UseColor(1.5f, 0.15f, 0f);
                //GameShaders.Armor.BindShader(ItemType<MyNoiseDyeItem>(), new ArmorShaderData(dyeRef, "NoisePass")).UseImage("Images/Misc/noise"); // Uses the default Terraria noise map.

                // To bind a miscellaneous, non-filter effect, use this.
                // If you're actually using this, you probably already know what you're doing anyway.

                //GameShaders.Misc["EffectName"] = new MiscShaderData(specialref, "PassName");

                // To bind a screen shader, use this.
                // EffectPriority should be set to whatever you think is reasonable.  
                
                Filters.Scene["DarknessEmbrace"] = new Filter(new DarknessEmbraceShaderData(Assets.Request<Effect>("src/Assets/Effects/DarknessEmbrace", AssetRequestMode.ImmediateLoad), "BackgroundShaderFunction"), EffectPriority.VeryHigh);
                Filters.Scene["Shackled"] = new Filter(new ShackledShaderData(Assets.Request<Effect>("src/Assets/Effects/Shackled", AssetRequestMode.ImmediateLoad), "BackgroundShaderFunction"), EffectPriority.VeryHigh);
                Filters.Scene["AstralBiome"] = new Filter(new AstralBiomeShaderData(Assets.Request<Effect>("src/Assets/Effects/AstralBiomeShader", AssetRequestMode.ImmediateLoad), "BackgroundShaderFunction"), EffectPriority.Medium);
            }
        }
    }
}
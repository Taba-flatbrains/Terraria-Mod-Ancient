using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Ancient.src.Code.Projectiles.Flails
{
    internal class HeavyFlailProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true; // This ensures that the projectile is synced when other players join the world.
            Projectile.width = 22; // The width of your projectile
            Projectile.height = 22; // The height of your projectile
            Projectile.friendly = true; // Deals damage to enemies
            Projectile.penetrate = -1; // Infinite pierce
            Projectile.DamageType = DamageClass.Melee; // Deals melee damage
            Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true; // Used for hit cooldown changes in the ai hook
            Projectile.localNPCHitCooldown = 10; // This facilitates custom hit cooldown logic

            // Here we reuse the flail projectile aistyle and set the aitype to the Sunfury. These lines will get our projectile to behave exactly like Sunfury would. This only affects the AI code, you'll need to adapt other code for the other behaviors you wish to use.
            Projectile.aiStyle = ProjAIStyleID.Flail;
            AIType = ProjectileID.Mace;

            // These help center the projectile as it rotates since its hitbox and scale doesn't match the actual texture size
            DrawOffsetX = -6;
            DrawOriginOffsetY = -6;
        }

        // All of the following methods are additional behaviors of Sunfury that are not automatically inherited by ExampleFlailProjectile through the use of Projectile.aiStyle and AIType. You'll need to find corresponding code in the decompiled source code if you wish to clone a different vanilla projectile as a starting point.

        // Draw the projectile in full brightness, ignoring lighting conditions.
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        // In PreDrawExtras, we trick the game into thinking the projectile is actually a Sunfury projectile. After PreDrawExtras, the Terraria code will draw the chain. Drawing the chain ourselves is quite complicated, ExampleAdvancedFlailProjectile has an example of that. Then, in PreDraw, we restore the Projectile.type back to normal so we don't break anything.  
        public override bool PreDrawExtras()
        {
            Projectile.type = ProjectileID.Mace;
            return base.PreDrawExtras();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.type = ModContent.ProjectileType<HeavyFlailProjectile>();

            // This code handles the after images.
            if (Projectile.ai[0] == 1f)
            {
                Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
                Vector2 drawPosition = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
                Vector2 drawOrigin = new Vector2(projectileTexture.Width, projectileTexture.Height) / 2f;
                Color drawColor = Projectile.GetAlpha(lightColor);
                drawColor.A = 127;
                drawColor *= 0.5f;
                int launchTimer = (int)Projectile.ai[1];
                if (launchTimer > 5)
                {
                    launchTimer = 5;
                }

                SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                for (float transparency = 1f; transparency >= 0f; transparency -= 0.125f)
                {
                    float opacity = 1f - transparency;
                    Vector2 drawAdjustment = Projectile.velocity * -launchTimer * transparency;
                    Main.EntitySpriteDraw(projectileTexture, drawPosition + drawAdjustment, null, drawColor * opacity, Projectile.rotation, drawOrigin, Projectile.scale * 1.15f * MathHelper.Lerp(0.5f, 1f, opacity), spriteEffects, 0);
                }
            }

            return base.PreDraw(ref lightColor);
        }
    }
}

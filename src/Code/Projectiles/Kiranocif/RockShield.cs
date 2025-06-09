using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Ancient.src.Code.Projectiles.Kiranocif
{
    internal class RockShield : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 36; // The width of projectile hitbox
            Projectile.height = 36; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.penetrate = 2; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }

        private float RotationOffset = 0;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = new Random().Next(2);
            RotationOffset = new Random().NextSingle() - 0.5f;
            RotationOffset *= 0.2f;
        }

        public override void AI()
        {
            if (Projectile.timeLeft > launchTime)
            {
                Projectile.Center = cif.Center;
            }
            if (Projectile.timeLeft == launchTime)
            {
                Projectile.velocity = target.Center - Projectile.Center;
                Projectile.velocity.Normalize();
                Projectile.velocity *= 14;

                Projectile.rotation = Projectile.velocity.ToRotation() + RotationOffset;
            }
            if (Projectile.timeLeft < launchTime)
            {

            }
        }
        private float launchTime => 320 - 20 * n;
        public float n => Projectile.ai[0];
        public Player target => Main.player[(int)Projectile.ai[1]];

        private NPC cif => Main.npc[(int)Projectile.ai[2]];
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > launchTime)
            {
                Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

                float RotationDisplacement = n / 5 * MathHelper.TwoPi;
                RotationDisplacement += 0.05f * Projectile.timeLeft;
                Vector2 drawPos = Projectile.Center - Main.screenPosition;
                drawPos += new Vector2(0, Projectile.timeLeft / 5).RotatedBy(RotationDisplacement);

                Color color = Lighting.GetColor((drawPos + Main.screenPosition).ToTileCoordinates());

                Main.EntitySpriteDraw(texture, drawPos, new Rectangle(0, 50 * Projectile.frame, 50, 50), color, RotationDisplacement + MathHelper.PiOver4,
                    new Vector2(25, 25), 1f, SpriteEffects.None, 0);

                return false;
            } else
            {
                return true;
            }

        }
    }
}

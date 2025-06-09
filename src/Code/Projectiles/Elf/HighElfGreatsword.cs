using Ancient.src.Code.Projectiles.OceanTemple;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Projectiles.Elf
{
    internal class HighElfGreatsword : ModProjectile
    {
        private NPC Owner => Main.npc[(int)Projectile.ai[0]];
        private ref float Timer => ref Projectile.ai[2];
        private ref float InitialAngle => ref Projectile.ai[1];
        private float Progress = 0;

        private const float SWINGRANGE = 0.5f * (float)Math.PI; // The angle a swing attack covers (300 deg)
        private const float FIRSTHALFSWING = 0.6f; // How much of the swing happens before it reaches the target angle (in relation to swingRange)

        public override void SetDefaults()
        {
            Projectile.width = 54; // Hitbox width of projectile
            Projectile.height = 50; // Hitbox height of projectile
            Projectile.friendly = false; // Projectile hits enemies
            Projectile.hostile = true;
            Projectile.timeLeft = 10000; // Time it takes for projectile to expire
            Projectile.penetrate = -1; // Projectile pierces infinitely
            Projectile.tileCollide = false; // Projectile does not collide with tiles
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.spriteDirection = Owner.spriteDirection;

            float targetAngle = (Main.player[Owner.target].position - Owner.position).ToRotation();

            if (Projectile.spriteDirection == 1)
            {
                // However, we limit the rangle of possible directions so it does not look too ridiculous
                targetAngle = MathHelper.Clamp(targetAngle, (float)-Math.PI * 1 / 3, (float)Math.PI * 1 / 6);
            }
            else
            {
                if (targetAngle < 0)
                {
                    targetAngle += 2 * (float)Math.PI; // This makes the range continuous for easier operations
                }

                targetAngle = MathHelper.Clamp(targetAngle, (float)Math.PI * 5 / 6, (float)Math.PI * 4 / 3);
            }

            InitialAngle = targetAngle - FIRSTHALFSWING * SWINGRANGE * Projectile.spriteDirection; // Otherwise, we calculate the angle
            Projectile.position += new Vector2(Projectile.spriteDirection * 6, 1);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            // Projectile.spriteDirection for this projectile is derived from the mouse position of the owner in OnSpawn, as such it needs to be synced. spriteDirection is not one of the fields automatically synced over the network. All Projectile.ai slots are used already, so we will sync it manually. 
            writer.Write((sbyte)Projectile.spriteDirection);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadSByte();
        }

        public override void AI()
        {
            ExecuteStrike();
            SetSwordPosition();
            Timer += 1;
        }

        public void SetSwordPosition()
        {
            Projectile.rotation = InitialAngle + Projectile.spriteDirection * Progress; // Set projectile rotation

            Projectile.scale = 1; // Slightly scale up the projectile and also take into account melee size modifiers
            Projectile.position += Owner.velocity;
        }

        private const int execTime = 20;
        private void ExecuteStrike()
        {
            Owner.direction = Projectile.spriteDirection;
            Progress = MathHelper.SmoothStep(0, SWINGRANGE, Timer / execTime);

            if (Timer >= execTime)
            {
                Projectile.active = false;
            }
        }

        public override bool PreDraw(ref Microsoft.Xna.Framework.Color lightColor)
        {
            // Calculate origin of sword (hilt) based on orientation and offset sword rotation (as sword is angled in its sprite)
            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            if (Projectile.spriteDirection > 0)
            {
                origin = new Vector2(0, Projectile.height);
                rotationOffset = MathHelper.ToRadians(45f);
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(Projectile.width, Projectile.height);
                rotationOffset = MathHelper.ToRadians(135f);
                effects = SpriteEffects.FlipHorizontally;
            }

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);

            // Since we are doing a custom draw, prevent it from normally drawing
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Owner.Center;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
        }
    }
}

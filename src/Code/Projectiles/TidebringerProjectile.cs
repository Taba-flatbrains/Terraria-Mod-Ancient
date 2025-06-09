using Ancient.src.Common.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Ancient.src.Code.Projectiles
{
    internal class TidebringerProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 1; // The width of projectile hitbox
            Projectile.height = 1; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Melee; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 45; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Projectile does not collide with tiles

            Projectile.usesLocalNPCImmunity = true; // Uses local immunity frames
            Projectile.localNPCHitCooldown = 20; // We set this to -1 to make sure the projectile doesn't hit twice
        }

        private int ticks = 0;

        private int SpreadVelocity = 11;
        private const int Width = 25;
        private float CleaveAngle = 0.6f;

        private Vector2 direction = Vector2.One;
        private int size = 0;
        private float OpacityMultiplier = 0.5f;
        public override void OnSpawn(IEntitySource source)
        {
            direction = Projectile.velocity;
            direction.Normalize();
            Projectile.velocity = Vector2.Zero;

            if (Projectile.ai[0]==0)
            {
                SpreadVelocity = 15;
                CleaveAngle = 1.4f;
                Projectile.timeLeft = 17;
                OpacityMultiplier = 0.2f;
            }
        }
        public override void AI()
        {
            ticks++;
            size += SpreadVelocity;
        }

        private const float BeamMultiplier = 0.3f;
        private const int BeamLayerDistance = 6;
        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Vector2 origin = new Vector2(10, 10);
            SpriteEffects effects = SpriteEffects.None;

            int BeamCount = (int)(size * CleaveAngle * BeamMultiplier);
            float BeamSpacing = CleaveAngle / BeamCount * 2;
            for (int i = 0; i < 4; i++)
            {
                Vector2 InitialBeamDirection = direction * (size + (i * BeamLayerDistance));
                InitialBeamDirection = InitialBeamDirection.RotatedBy(-CleaveAngle);

                float ScaleMultiplier = 0.6f + i * 0.1f;

                for (int j = 0; j < BeamCount; j++)
                {
                    Vector2 pos = InitialBeamDirection.RotatedBy(j* BeamSpacing) + Projectile.position; 

                    Color color = Lighting.GetColor(pos.ToTileCoordinates());
                    color = new Color(new Vector4(color.ToVector3(), (i * 0.2f + 0.2f) * OpacityMultiplier));

                    Main.EntitySpriteDraw(texture, pos - Main.screenPosition, null, color, Projectile.rotation, origin, Projectile.scale * ScaleMultiplier, effects, 0);
                }
            }
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 1.5f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.myPlayer == Projectile.owner && Projectile.ai[0] == 1)
            {
                Vector2 pos = target.Hitbox.ClosestPointInRect(Projectile.position);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, pos - Projectile.position, ModContent.ProjectileType<TidebringerProjectile>(), Projectile.damage,
                Projectile.knockBack, Owner: Projectile.owner, 0);
                hit.SourceDamage = 2 * hit.SourceDamage;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int HitSize = size + 4 * BeamLayerDistance - Width;
            Vector2 ProjectileToTarget = targetHitbox.ClosestPointInRect(Projectile.position) - Projectile.position;
            if (targetHitbox.Distance(Projectile.position) > HitSize - Width && targetHitbox.Distance(Projectile.position) < HitSize + Width &&
                MathF.Abs(MathUtils.AngleBetween(direction, ProjectileToTarget)) < CleaveAngle)
            {
                return true;
            }
            return false;
        }
    }
}

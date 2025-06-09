using Ancient.src.Code.Buffs;
using Ancient.src.Common.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Projectiles.Boss.YrimirsSoul
{
    internal class BlazingEmber : ModProjectile
    {
        NPCS.Boss.YrimirsSoul.YrimirsSoul Owner => (NPCS.Boss.YrimirsSoul.YrimirsSoul)Main.npc[(int)Projectile.ai[0]].ModNPC;

        Vector2 Offset = StandardHoldingOffset;


        private Vector2 TipPosition()
        {
            float EffectiveRotation = MathHelper.Pi - Projectile.rotation;
            if (Projectile.spriteDirection == -1)
            {
                EffectiveRotation = MathHelper.Pi + Projectile.rotation;
            }
            return Projectile.Center + new Vector2(-50.4f * Projectile.spriteDirection, 64.8f).RotatedBy(EffectiveRotation);
        }

        public override void SetDefaults()
        {
            Projectile.width = 80; // The width of projectile hitbox
            Projectile.height = 80; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 3600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;
        }

        private static Vector2 StandardHoldingOffset = new Vector2(0, 0);
        public override void AI()
        {
            if (!Owner.NPC.active || Owner.AttackMode < 0) // todo: add disappearing animation, dont instantly set active = false
            {
                Projectile.active = false;
                return;
            }
            Projectile.spriteDirection = Owner.NPC.spriteDirection;
            Projectile.Center = Owner.NPC.Center + Offset;

            switch (Owner.AttackMode)
            {
                case 0:
                    Offset = Owner.handPos;
                    break;
                case 1: // stab
                    Offset = Owner.handPos;
                    Projectile.rotation = -1f - (Owner.AttackTimer * 0.3f / 100);
                    if (Owner.AttackTimer < 30 && Projectile.timeLeft % 4 == 0)
                    {
                        Dust.NewDust(TipPosition(), 1, 1, DustID.Pixie);
                    }
                    break;
                case 2: // swing
                    Offset = Owner.handPos;
                    Projectile.rotation = -1.5f + (Owner.AttackTimer * 2.2f / Owner.Attack2Duration);
                    break;
                case 3: // Sword Hover
                    Offset = Owner.handPos - new Vector2(0, MathUtils.LogisticFunction(300 - Owner.AttackTimer, 35f, 130f, 0.04f));
                    Projectile.rotation = 0.25f - (Owner.AttackTimer * 0.2f / 100);
                    if (Owner.AttackTimer < 250 && Projectile.timeLeft % 4 == 0)
                    {
                        Dust.NewDust(TipPosition(), 1, 1, DustID.Frost); 
                    }
                    break;
                case 4:
                    Offset = Owner.handPos;
                    Projectile.rotation = 0;
                    break;
                case 5:
                    Offset = Owner.handPos;
                    Projectile.rotation = -MathUtils.AngleBetween(new Vector2(0, -1), Owner.NPC.velocity) + 1.05f;
                    break;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f; // funktioniert noch nicht so brauch noch sprite direction und so
            Vector2 lineEnd = TipPosition();
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, lineEnd, 5f * Projectile.scale, ref collisionPoint);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            SpriteEffects spriteEffects = SpriteEffects.None;
            Vector2 drawOrigin = new Vector2(13, 67);
            int rot_multiplier = -1;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
                drawOrigin = new Vector2(67, 67);
                rot_multiplier = 1;
            }

            Color color = Lighting.GetColor(Projectile.Center.ToTileCoordinates().X, Projectile.Center.ToTileCoordinates().Y);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation * rot_multiplier, drawOrigin, Projectile.scale, spriteEffects, 0);

            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<MoltenBuff>(), 600);
        }
    }
}

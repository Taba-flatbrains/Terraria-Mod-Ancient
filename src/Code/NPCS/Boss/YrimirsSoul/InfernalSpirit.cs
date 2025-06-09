using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Ancient.src.Common.Structures;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Ancient.src.Code.NPCS.Invasion.Elf;
using Terraria.GameContent;
using Terraria.DataStructures;
using Ancient.src.Common.Util;
using Terraria.Achievements;
using Ancient.src.Code.Buffs;
using Ancient.src.Code.Projectiles.Boss.YrimirsSoul;

namespace Ancient.src.Code.NPCS.Boss.YrimirsSoul
{
    internal class InfernalSpirit : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 30;
            NPC.damage = 90;
            NPC.defense = 50;
            NPC.lifeMax = 20000;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath7;
            NPC.value = 5000f;
            NPC.knockBackResist = 0.5f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;

            AIType = NPCID.Pixie;
            NPC.aiStyle = NPCAIStyleID.HoveringFighter;
            NPC.alpha = 100;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Blazing"),
            });
        }

        private int ticks = -200;
        private Vector2 BlinkTarget = Vector2.Zero;
        private Vector2 BlinkLocation = Vector2.Zero;

        private Player target => Main.player[NPC.target];

        private int Variety = 0;
        private int ShotsFired = 0;
        public override bool PreAI()
        {
            if (ticks == -200)
            {
                Variety = new Random().Next(4) + 1;
            }
            ticks++;

            // Choose blink target
            if (ticks % (60 * 15) == 0 && target.active && !target.dead)
            {
                BlinkTarget = target.Center + 20 * target.velocity;
                BlinkLocation = NPC.Center;
            }
            // Execute Blink
            if ((ticks % (60 * 15) == 120 || (BlinkLocation.Distance(BlinkTarget) < 16 && ticks % (60 * 15) < 120)) && target.active 
                && !target.dead && BlinkLocation.Distance(Vector2.Zero) > 1)
            {
                if (ShotsFired % 3 == 0) // dont blink + satellites turn into projectiles
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), BlinkLocation, Vector2.One.RotatedBy(MathHelper.TwoPi / 5f * i), ModContent.ProjectileType<SearingDroplet>(),
                            50, 2, -1, NPC.target);
                    }
                } else
                {
                    NPC.Center = BlinkLocation;
                }
                ticks = 120;
                ShotsFired++;
            }
            // Shift Blink Location further to Blink target
            else if (ticks % (60 * 15) < 120)
            {
                Vector2 direction = BlinkTarget - BlinkLocation;
                direction = direction.SafeNormalize(Vector2.Zero) * (MathF.Sqrt(direction.Length()) + 10) * 0.4f;
                BlinkLocation += direction;
            }
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;

            Vector2 origin = new Vector2(16, 16);
            Vector2 pos = NPC.Center - Main.screenPosition;
            SpriteEffects spriteEffects = SpriteEffects.None;

            float opacityBodySatellites = 0.5f;
            float opacityBlinkSatellites = 1 / 256f;
            if (ticks % (60 * 15) < 120)
            {
                opacityBlinkSatellites = 0.2f + ((ticks % (60 * 15)) / 240);
                opacityBodySatellites = 0.5f - ((ticks % (60 * 15)) / 300);
            }
            Vector2 BlinkSatellitesOffset = BlinkLocation - NPC.Center;

            Vector3[] SatellitesPos = new Vector3[5];
            for (int i = 0; i < SatellitesPos.Length; i++)
            {
                SatellitesPos[i] = new Vector3(pos, 0) + MathUtils.RotateVector(new Vector3(37, 0, 0), MathUtils.RotateVector(new Vector3(0, 1, 0), new Vector3(1, 0, 0), MathF.Sin(ticks / 60f) * 0.6f),
                    (((float)i) / SatellitesPos.Length + (ticks * 0.02f)) * MathHelper.TwoPi);
            }


            // Draw body Satellitles behind body
            for (int i = 0; i < SatellitesPos.Length; i++)
            {
                if (SatellitesPos[i].Z < 0) { continue; }
                spriteBatch.Draw(texture, new Vector2(SatellitesPos[i].X, SatellitesPos[i].Y), new Rectangle(0, 32 * Variety, 32, 32)
                , new Color(new Vector4(drawColor.ToVector3(), opacityBodySatellites)), 0.02f * ticks, origin, Vector2.One, spriteEffects, 0); // Behind Body
            }

            // Draw blink Satellitles behind body
            if (opacityBlinkSatellites != 1 / 256f) { 
                for (int i = 0; i < SatellitesPos.Length; i++)
                {
                    if (SatellitesPos[i].Z < 0) { continue; }
                    spriteBatch.Draw(texture, new Vector2(SatellitesPos[i].X, SatellitesPos[i].Y) + BlinkSatellitesOffset, new Rectangle(0, 32 * Variety, 32, 32)
                    , new Color(new Vector4(drawColor.ToVector3(), opacityBlinkSatellites)), 0.02f * ticks, origin, Vector2.One, spriteEffects, 0); // Behind Body
                }
            }


            spriteBatch.Draw(texture, pos, new Rectangle(0, 0, 32, 32)
                , new Color(new Vector4(drawColor.ToVector3(), 0.5f)), 0.02f * ticks, origin, Vector2.One, spriteEffects, 0); // Body


            // Draw Satellitles in front of body
            for (int i = 0; i < SatellitesPos.Length; i++)
            {
                if (SatellitesPos[i].Z > 0) { continue; }
                spriteBatch.Draw(texture, new Vector2(SatellitesPos[i].X, SatellitesPos[i].Y), new Rectangle(0, 32 * Variety, 32, 32)
                , new Color(new Vector4(drawColor.ToVector3(), opacityBodySatellites)), 0.02f * ticks, origin, Vector2.One, spriteEffects, 0); // Behind Body
            }

            // Draw blink Satellitles in front of body
            if (opacityBlinkSatellites != 1 / 256f)
            {
                for (int i = 0; i < SatellitesPos.Length; i++)
                {
                    if (SatellitesPos[i].Z > 0) { continue; }
                    spriteBatch.Draw(texture, new Vector2(SatellitesPos[i].X, SatellitesPos[i].Y) + BlinkSatellitesOffset, new Rectangle(0, 32 * Variety, 32, 32)
                    , new Color(new Vector4(drawColor.ToVector3(), opacityBlinkSatellites)), 0.02f * ticks, origin, Vector2.One, spriteEffects, 0); // Behind Body
                }
            }

            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<MoltenBuff>(), 600);
        }
    }
}

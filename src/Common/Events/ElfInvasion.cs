using Ancient.src.Code.Buffs;
using Ancient.src.Code.NPCS.Invasion.Elf;
using Ancient.src.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.Utilities;
using Terraria.WorldBuilding;

namespace Ancient.src.Common.Events
{
    internal class ElfInvasion : ModSystem
    {
        public static List<int> Elves = new List<int>()
        {
            ModContent.NPCType<HighElfPriestress>(),
            ModContent.NPCType<HighElfKnight>(),
            ModContent.NPCType<HighElfSorcerer>(),
            ModContent.NPCType<DarkElfAssasin>(),
            ModContent.NPCType<DarkElfRanger>(),
            ModContent.NPCType<DarkElfWarlock>(),
        };
        /*
         * Important Lines: 
         * 
         * Für Server bei Invasion
         * NetMessage.SendData(MessageID.InvasionProgressReport, -1, -1, (NetworkText)null, Main.invasionProgress, (float)Main.invasionProgressMax, 3f, 1f, 0, 0, 0);
         * 
         * Für Server oder Singleplayer
         * Main.ReportInvasionProgress(currentKillCount, requiredKillCount, 3, currentWave);
         * 
         * 
         * 
         * 
         */

        public static bool Downed = false;

        public static bool Ongoing = false;
        public static int Progress = 0;
        public static float currentKillCount = 0;
        public static int requiredKillCount = 100;

        public override void Load()
        {

        }

        public override void Unload()
        {

        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Add("Ongoing", Ongoing);
            tag.Add("Progress", Progress);
            tag.Add("Downed", Downed);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.ContainsKey("Ongoing"))
            {
                Ongoing = tag.Get<bool>("Ongoing");
                Progress = tag.Get<int>("Progress");
                Downed = tag.Get<bool>("Downed");

                if (Ongoing)
                {
                    currentKillCount = Progress;
                    requiredKillCount = 100;
                }
            }
            else
            {
                throw new Exception("Elf Invasion Load World Data Error");
            }
        }

        public override void PostUpdateInvasions()
        {
            Update();
        }

        private static bool Daytime = true;
        public override void PostUpdateWorld()
        {
            // empress of light or moonlord or darkness embrace defeated
            if (!NPC.downedEmpressOfLight && !NPC.downedMoonlord && !DownedBossSystem.DownedDarknessEmbrace) { return; }
            // 1 in 7 chance every morning, after it has been beaten every 14 days
            if (Main.dayTime)
            {
                if (!Daytime)
                {
                    if ((new Random().Next(0, 7) == 0 && (!Downed)) || (new Random().Next(0, 50) == 0 && Downed))
                    {
                        StartInvasion();
                    }
                    Daytime = true;
                }
            } else
            {
                Daytime = false;
            }
        }

        public static void StartInvasion()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            if (!Main.CanStartInvasion())
                return;

            NPC.waveKills = 0.0f;
            NPC.waveNumber = 1;
            Ongoing = true;
            Main.invasionType = -1; // idk
            Main.invasionSize = 100;

            int num = 0;
            for (int i = 0; i < 255; i++)
            {
                /*
                if (player[i].active && player[i].statLifeMax >= 200)
                */
                if (Main.player[i].active && Main.player[i].ConsumedLifeCrystals >= 5)
                    num++;
            }
            requiredKillCount = 80 + 40 * num;

            WorldGen.BroadcastText(NetworkText.FromLiteral("The elves are entering your dimension!"), new Vector4(130, 210, 255, 255));
            NetMessage.SendData(MessageID.WorldData, -1, -1, (NetworkText)null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);

           if (Main.netMode != NetmodeID.MultiplayerClient)
                Main.ReportInvasionProgress(0, 1, 0, 0);
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.InvasionProgressReport, -1, -1, (NetworkText)null, 0, 1f, 0f, 0f, 0, 0, 0);
        }

        private static void StopInvasion()
        {
            Downed = true;
            Ongoing = false;
            NPC.waveKills = 0.0f;
            NPC.waveNumber = 0;
            Main.invasionType = 0;
            NetMessage.SendData(MessageID.WorldData, -1, -1, (NetworkText)null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            WorldGen.BroadcastText(NetworkText.FromLiteral("The elves have been defeated!"), new Vector4(130, 210, 255, 255));
        }

        private static void ReportEventProgress()
        {
            Main.ReportInvasionProgress((int)currentKillCount, requiredKillCount, 0, 0);
        }

        private static void SyncInvasionProgress(int toWho)
        {
            NetMessage.SendData(MessageID.InvasionProgressReport, toWho, -1, (NetworkText)null, (int)currentKillCount, (float)requiredKillCount, 3f, 0, 0, 0, 0);
        }

        private static void Update()
        {
            if (!Ongoing)
            {
                return;
            }
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Progress = (int)(currentKillCount * 100 / requiredKillCount);

            if (Progress >= 100)
            {
                StopInvasion();
            }

            ReportEventProgress();
            SyncInvasionProgress(-1);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            Main.invasionProgressAlpha = 0;
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            DrawElfInvasionProgress(spriteBatch);
        }

        private static float invasionProgressAlpha = 0;
        private static void DrawElfInvasionProgress(SpriteBatch spriteBatch)
        {

            if (Main.invasionProgressDisplayLeft > 0)
                invasionProgressAlpha += 0.05f;
            else
                invasionProgressAlpha -= 0.05f;

            if (invasionProgressAlpha < 0f)
                invasionProgressAlpha = 0f;

            if (invasionProgressAlpha > 1f)
                invasionProgressAlpha = 1f;

            if (invasionProgressAlpha <= 0f)
                return;

            float num = 0.5f + invasionProgressAlpha * 0.5f;
            Texture2D value = ModContent.Request<Texture2D>("Ancient/src/Common/Events/ElfInvasionIcon").Value;
            string text = "Elf Invasion";
            Microsoft.Xna.Framework.Color c = new Microsoft.Xna.Framework.Color(88, 0, 160) * 0.5f;
            Main.invasionProgressAlpha = 1;


            int num7 = (int)(200f * num);
            int num8 = (int)(45f * num);
            Vector2 vector3 = new Vector2(Main.screenWidth - 120, Main.screenHeight - 40);
            Utils.DrawInvBG(R: new Microsoft.Xna.Framework.Rectangle((int)vector3.X - num7 / 2, (int)vector3.Y - num8 / 2, num7, num8), sb: spriteBatch, c: new Microsoft.Xna.Framework.Color(63, 65, 151, 255) * 0.785f);
            string text3 = "";
            text3 = ((Main.invasionProgressMax != 0) ? ((int)((float)Main.invasionProgress * 100f / (float)Main.invasionProgressMax) + "%") : Main.invasionProgress.ToString());
            text3 = Language.GetTextValue("Game.WaveCleared", text3);
            Texture2D value3 = TextureAssets.ColorBar.Value;
            _ = TextureAssets.ColorBlip.Value;
            if (Main.invasionProgressMax != 0)
            {
                spriteBatch.Draw(value3, vector3, null, Microsoft.Xna.Framework.Color.White * Main.invasionProgressAlpha, 0f, new Vector2(value3.Width / 2, 0f), num, SpriteEffects.None, 0f);
                float num9 = MathHelper.Clamp((float)Main.invasionProgress / (float)Main.invasionProgressMax, 0f, 1f);
                Vector2 vector4 = FontAssets.MouseText.Value.MeasureString(text3);
                float num10 = num;
                if (vector4.Y > 22f)
                    num10 *= 22f / vector4.Y;

                float num11 = 169f * num;
                float num12 = 8f * num;
                Vector2 vector5 = vector3 + Vector2.UnitY * num12 + Vector2.UnitX * 1f;
                Utils.DrawBorderString(spriteBatch, text3, vector5 + new Vector2(0f, -4f), Microsoft.Xna.Framework.Color.White * Main.invasionProgressAlpha, num10, 0.5f, 1f);
                vector5 += Vector2.UnitX * (num9 - 0.5f) * num11;
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, vector5, new Microsoft.Xna.Framework.Rectangle(0, 0, 1, 1), new Microsoft.Xna.Framework.Color(255, 241, 51) * Main.invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(num11 * num9, num12), SpriteEffects.None, 0f);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, vector5, new Microsoft.Xna.Framework.Rectangle(0, 0, 1, 1), new Microsoft.Xna.Framework.Color(255, 165, 0, 127) * Main.invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(2f, num12), SpriteEffects.None, 0f);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, vector5, new Microsoft.Xna.Framework.Rectangle(0, 0, 1, 1), Microsoft.Xna.Framework.Color.Black * Main.invasionProgressAlpha, 0f, new Vector2(0f, 0.5f), new Vector2(num11 * (1f - num9), num12), SpriteEffects.None, 0f);
            }
            

            Vector2 vector6 = FontAssets.MouseText.Value.MeasureString(text);
            float num13 = 120f;
            if (vector6.X > 200f)
                num13 += vector6.X - 200f;

            Microsoft.Xna.Framework.Rectangle r3 = Utils.CenteredRectangle(new Vector2((float)Main.screenWidth - num13, Main.screenHeight - 80), (vector6 + new Vector2(value.Width + 12, 6f)) * num);
            Utils.DrawInvBG(spriteBatch, r3, c);
            spriteBatch.Draw(value, r3.Left() + Vector2.UnitX * num * 8f, null, Microsoft.Xna.Framework.Color.White * Main.invasionProgressAlpha, 0f, new Vector2(0f, value.Height / 2), num * 0.8f, SpriteEffects.None, 0f);
            Utils.DrawBorderString(spriteBatch, text, r3.Right() + Vector2.UnitX * num * -22f, Microsoft.Xna.Framework.Color.White * Main.invasionProgressAlpha, num * 0.9f, 1f, 0.4f);
        }
    }

    class ElfInvasionSpawnRateMultiplierGlobalNPC : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns) // hopefully ignores town spawn rate multiplier
        {
            if (ElfInvasion.Ongoing)
            {
                spawnRate = 120;
                maxSpawns = 18;
            }
        }
    }

    public class ElfInvasionPlayer : ModPlayer
    {
        public override void PreUpdateBuffs()
        {
            if (ElfInvasion.Ongoing)
            {
                if (GroundDistance() > 35)
                {
                    Player.AddBuff(ModContent.BuffType<MoreGravityBuff>(), 2);
                }
            }
        }

        private int GroundDistance()
        {
            Point origin = Player.Center.ToTileCoordinates();
            origin.X -= 10;
            int r = 0;
            for (int j=0; j < 5; j++)
            {
                int temp = _GroundDistance(origin);
                if (temp > r)
                {
                    r = temp;
                }
                origin.X += 5;
            }
            return r;
        }

        private int _GroundDistance(Point origin)
        {
            int c = 0;
            for (int i = 0; i < Main.maxTilesY - origin.Y; i++)
            {
                c++;
                if (Main.tile[origin + new Point(0, i)].HasUnactuatedTile && Main.tile[origin + new Point(0, i)].HasTile && Main.tileSolid[Main.tile[origin + new Point(0, i)].TileType])
                {
                    break;
                }
            }
            return c;
        }
    }
}

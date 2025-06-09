using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Ancient.src.Code.NPCS.Invasion.Elf
{
    internal class FlaringWyvern : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 3;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                // Influences how the NPC looks in the Bestiary
                Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 40;
            NPC.damage = 100;
            NPC.defense = 50;
            NPC.lifeMax = 8000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath5;
            NPC.value = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;

                                                //Banner = Item.NPCtoBanner(NPCID.Zombie); // <- to be replaced with own banner
                                                //BannerItem = Item.BannerToItem(Banner); 
            NPC.knockBackResist = 0f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
			// Sets the spawning conditions of this NPC that is listed in the bestiary.
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

			// Sets the description of this NPC that is listed in the bestiary.
			new FlavorTextBestiaryInfoElement("A smaller but more dangerous variety of a wyvern. Wizards like to keep them as a pet."),
        });
        }

        public override void FindFrame(int frameHeight)
        {
            
        }

        public override void OnSpawn(IEntitySource source)
        {
            NPC.TargetClosest();
        }

        private List<Vector2> LastPosition = new List<Vector2>()
        {
        };
        private List<float> LastRotation = new List<float> { };

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (LastRotation.Count < 96) { return true; }
            int c = 0;
            int j = 95;
            Texture2D FlaringWyvernTexture = TextureAssets.Npc[ModContent.NPCType<FlaringWyvern>()].Value;
            for (int i = 95; i > 0; i--)
            {
                if (c > 2) { break; }
                if (Vector2.Distance(LastPosition[i], LastPosition[j]) < 24)
                {
                    continue;
                }
                j = i;
                c++;

                float rotation = LastRotation[i];


                if (c == 1 || c == 2) // middle body
                {
                    spriteBatch.Draw(FlaringWyvernTexture, LastPosition[i] - Main.screenPosition, new Rectangle(0, 40, 40, 40), Lighting.GetColor(LastPosition[i].ToTileCoordinates()), rotation,
                        new Vector2(20, 20), 1, SpriteEffects.None, 0);
                }
                if (c == 3) // tail
                {
                    Dust.NewDust(LastPosition[i], 4, 4, DustID.Torch);
                    spriteBatch.Draw(FlaringWyvernTexture, LastPosition[i] - Main.screenPosition, new Rectangle(0, 80, 40, 40), Lighting.GetColor(LastPosition[i].ToTileCoordinates()), rotation,
                        new Vector2(20, 20), 1, SpriteEffects.None, 0);
                }
            }

            return true;
        }

        public override void AI()
        {
            if (NPC.target == 255) 
            {
                NPC.active = false;
                return; 
            }
            LastPosition.Add(NPC.Center); // newest npc position at index 2
            if (LastPosition.Count > 96) { LastPosition.RemoveAt(0); }

            LastRotation.Add(NPC.rotation); // newest npc rotaiton at index 2
            if (LastRotation.Count > 96) { LastRotation.RemoveAt(0); }

            Vector2 delta_vel = Main.player[NPC.target].Center - NPC.Center;
            delta_vel.Normalize();
            NPC.velocity += delta_vel * 0.15f;
            NPC.velocity *= 0.985f;
            NPC.rotation = NPC.velocity.AngleTo(Vector2.UnitX);

            Lighting.AddLight(NPC.Center, new Vector3(0.5f, 0.5f, 0.5f));
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace MultiworldPylon.Content
{
    public class MultiWorld1 : Subworld
    {
        public override int Width => 6400;
        public override int Height => 1800;

        public override bool ShouldSave => true;
        public override bool NoPlayerSaving => false;
        public override bool NormalUpdates => true;
        public override List<GenPass> Tasks => new List<GenPass>()
        {
            new StandardWorldGenPassSetup(index),
            new StandardWorldGenPass(index)
        };
        public static float logoRotation;
        public static float logoRotationDirection = -1f;
        public static float logoRotationSpeed = 1f;
        public static float logoScale = 1f;
        public static float logoScaleDirection = 1f;
        public static float logoScaleSpeed = 1f;
        public override void OnLoad()
        {
            Main.dayTime = true;
            Main.time = 27000;
        }
        public override void DrawMenu(GameTime gameTime)
        {
            Main.screenPosition.X += 4;
            if (Main.screenPosition.X > 2.1474835E+09f)
                Main.screenPosition.X = 0f;

            if (Main.screenPosition.X < -2.1474835E+09f)
                Main.screenPosition.X = 0f;


            Main.cloudBGAlpha = 1;
            Main.spriteBatch.Draw(TextureAssets.Background[0].Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            Main.ColorOfTheSkies = Color.White;
            Overlays.Scene.Update(gameTime);
            Overlays.Scene.Draw(Main.spriteBatch, RenderLayers.Sky);
            MultiworldPylon.DrawBG.Invoke(Main.instance, []);

            Star.UpdateStars();
            Cloud.UpdateClouds();

            Main.background = 0;

            if (WorldGen.remixWorldGen) {
			    logoRotation += logoRotationSpeed * 4E-05f;
			    if (logoRotation < 3.04f) {
				    logoRotation += logoRotationSpeed * 0.0016f;
				    if (logoRotationSpeed < 0f)
					    logoRotationSpeed = 0f;
			    }

			    if (logoRotation > 3.22f)
				    logoRotationDirection = -1f;
			    else if (logoRotation < 3.06f)
				    logoRotationDirection = 1f;

			    if (logoRotationSpeed < 20f && logoRotationDirection == 1f)
				    logoRotationSpeed += 1f;
			    else if (logoRotationSpeed > -20f && logoRotationDirection == -1f)
				    logoRotationSpeed -= 1f;

			    logoScale += logoScaleSpeed * 9E-05f;
			    if (logoScale > 1f)
				    logoScaleDirection = -1f;
			    else if (logoScale < 0.9f)
				    logoScaleDirection = 1f;

			    if (logoScaleSpeed < 50f && logoScaleDirection == 1f)
				    logoScaleSpeed += 1f;
			    else if (logoScaleSpeed > -50f && logoScaleDirection == -1f)
				    logoScaleSpeed -= 1f;
		    }
		    else if (WorldGen.drunkWorldGen) {
			    logoRotation += logoRotationSpeed * 4E-06f;
			    if (logoRotationSpeed > 0f)
				    logoRotationSpeed += 1500f;
			    else
				    logoRotationSpeed -= 1500f;

			    logoScale -= 0.05f;
			    if (logoScale < 0f)
				    logoScale = 0f;
		    }
		    else {
			    if (logoRotation > 0.09f) {
				    logoRotation += logoRotationSpeed * 0.0016f;
				    if (logoRotationSpeed > 0f)
					    logoRotationSpeed = 0f;
			    }

			    logoRotation += logoRotationSpeed * 4E-06f;
			    if (logoRotation > 0.08f)
				    logoRotationDirection = -1f;
			    else if (logoRotation < -0.08f)
				    logoRotationDirection = 1f;

			    if (logoRotationSpeed < 20f && logoRotationDirection == 1f)
				    logoRotationSpeed += 1f;
			    else if (logoRotationSpeed > -20f && logoRotationDirection == -1f)
				    logoRotationSpeed -= 1f;

			    logoScale += logoScaleSpeed * 9E-06f;
			    if (logoScale > 1.35f)
				    logoScaleDirection = -1f;
			    else if (logoScale < 1f)
				    logoScaleDirection = 1f;

			    if (logoScaleSpeed < 50f && logoScaleDirection == 1f)
				    logoScaleSpeed += 1f;
			    else if (logoScaleSpeed > -50f && logoScaleDirection == -1f)
				    logoScaleSpeed -= 1f;
		    }
            
            MultiworldPylon.UpdateAndDrawModMenu.Invoke(null, [Main.spriteBatch, gameTime, Color.White, logoRotation, logoScale]);
            if (MenuLoader.CurrentMenu.DisplayName == "Terraria 1.3.5.3")
            {
                Main.spriteBatch.Draw(TextureAssets.Logo3.Value, new Vector2(Main.screenWidth / 2, 100f), new Rectangle(0, 0, TextureAssets.Logo.Width(), TextureAssets.Logo.Height()), Color.White, logoRotation, new Vector2(TextureAssets.Logo.Width() / 2, TextureAssets.Logo.Height() / 2), logoScale, SpriteEffects.None, 0f);
            }
            else if (MenuLoader.CurrentMenu.DisplayName == "Journey's End")
            {
                Main.spriteBatch.Draw(TextureAssets.Logo.Value, new Vector2(Main.screenWidth / 2, 100f), new Rectangle(0, 0, TextureAssets.Logo.Width(), TextureAssets.Logo.Height()), Color.White, logoRotation, new Vector2(TextureAssets.Logo.Width() / 2, TextureAssets.Logo.Height() / 2), logoScale, SpriteEffects.None, 0f);
            }
            if (WorldGenerator.CurrentGenerationProgress != null)
            {
                WorldGen.crimson = (index > 3 && index < 8) ? index > 5 : Main.ActiveWorldFileData.HasCrimson;
                MultiworldPylon.worldLoadState.Update(gameTime);
                MultiworldPylon.worldLoadState.Draw(Main.spriteBatch);
            }
            else base.DrawMenu(gameTime);
            Utils.DrawBorderStringBig(Main.spriteBatch, Language.GetTextValue("Mods.MultiworldPylon.UI.LoadingText"), new Vector2(8, Main.screenHeight - 8), Color.White, 1, 0, 1);
        }
        public virtual int index => 1;
        public override void OnEnter()
        {
            Main.drunkWorld = false;
            Main.getGoodWorld = false;
            Main.notTheBeesWorld = false;
            Main.tenthAnniversaryWorld = false;
            Main.dontStarveWorld = false;
            Main.remixWorld = false;
            Main.noTrapsWorld = false;
            Main.zenithWorld = false;
            Main.starGame = false;

            switch (index)
            {
                case 8:
                    Main.zenithWorld = true;
                    break;
                case 9:
                    Main.drunkWorld = true;
                    break;
                case 10:
                    Main.getGoodWorld = true;
                    break;
                case 11:
                    Main.notTheBeesWorld = true;
                    break;
                case 12:
                    Main.tenthAnniversaryWorld = true;
                    break;
                case 13:
                    Main.dontStarveWorld = true;
                    break;
                case 14:
                    Main.remixWorld = true;
                    break;
                case 15:
                    Main.noTrapsWorld = true;
                    break;
            }
        }
        public override void OnExit()
        {
            var data = Main.ActiveWorldFileData;
            Main.drunkWorld = data.DrunkWorld;
            Main.getGoodWorld = data.ForTheWorthy;
            Main.notTheBeesWorld = data.NotTheBees;
            Main.tenthAnniversaryWorld = data.Anniversary;
            Main.dontStarveWorld = data.DontStarve;
            Main.remixWorld = data.RemixWorld;
            Main.noTrapsWorld = data.NoTrapsWorld;
            Main.zenithWorld = data.DrunkWorld && data.RemixWorld;
            Main.starGame = false;
        }
    }
    public class MultiWorld2 : MultiWorld1
    {
        public override int index => 2;
    }
    public class MultiWorld3 : MultiWorld1
    {
        public override int index => 3;
    }
    public class MultiWorld4 : MultiWorld1
    {
        public override int index => 4;
    }
    public class MultiWorld5 : MultiWorld1
    {
        public override int index => 5;
    }
    public class MultiWorld6 : MultiWorld1
    {
        public override int index => 6;
    }

    public class MultiWorld7 : MultiWorld1
    { public override int index => 7;
    }
    public class MultiWorld8 : MultiWorld1
    {
        public override int index => 8;
    }
    public class MultiWorld9 : MultiWorld1
    {
        public override int index => 9;
    }
    public class MultiWorld10 : MultiWorld1
    {
        public override int index => 10;
    }
    public class MultiWorld11 : MultiWorld1
    {
        public override int index => 11;
    }
    public class MultiWorld12 : MultiWorld1
    {
        public override int index => 12;
    }
    public class MultiWorld13 : MultiWorld1
    {
        public override int index => 13;
    }
    public class MultiWorld14 : MultiWorld1
    {
        public override int index => 14;
    }
    public class MultiWorld15 : MultiWorld1
    {
        public override int index => 15;
    }
    public class StandardWorldGenPassSetup : GenPass
    {
        public int index;
        public StandardWorldGenPassSetup(int i) : base("Standard World Setup", 1) { index = i; }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            WorldGen.everythingWorldGen = false;
            WorldGen.drunkWorldGen = false;
            WorldGen.drunkWorldGenText = false;
            WorldGen.drunkWorldGenText = false;
            WorldGen.getGoodWorldGen = false;
            WorldGen.notTheBees = false;
            WorldGen.tenthAnniversaryWorldGen = false;
            WorldGen.dontStarveWorldGen = false;
            WorldGen.remixWorldGen = false;
            WorldGen.noTrapsWorldGen = false;

            GenVars.structures = new StructureMap();
            GenVars.desertHiveHigh = Main.maxTilesY;
            GenVars.desertHiveLow = 0;
            GenVars.desertHiveLeft = Main.maxTilesX;
            GenVars.desertHiveRight = 0;
            GenVars.worldSurfaceLow = 0.0;
            GenVars.worldSurface = 0.0;
            GenVars.worldSurfaceHigh = 0.0;
            GenVars.rockLayerLow = 0.0;
            GenVars.rockLayer = 0.0;
            GenVars.rockLayerHigh = 0.0;
            GenVars.copper = 7;
            GenVars.iron = 6;
            GenVars.silver = 9;
            GenVars.gold = 8;
            GenVars.dungeonSide = 0;
            GenVars.jungleHut = (ushort)WorldGen.genRand.Next(5);
            GenVars.shellStartXLeft = 0;
            GenVars.shellStartYLeft = 0;
            GenVars.shellStartXRight = 0;
            GenVars.shellStartYRight = 0;
            GenVars.PyrX = null;
            GenVars.PyrY = null;
            GenVars.numPyr = 0;
            GenVars.jungleMinX = -1;
            GenVars.jungleMaxX = -1;
            GenVars.snowMinX = new int[Main.maxTilesY];
            GenVars.snowMaxX = new int[Main.maxTilesY];
            GenVars.snowTop = 0;
            GenVars.snowBottom = 0;
            GenVars.skyLakes = 1;
            if (Main.maxTilesX > 8000)
                GenVars.skyLakes++;

            if (Main.maxTilesX > 6000)
                GenVars.skyLakes++;

            GenVars.beachBordersWidth = 275;
            GenVars.beachSandRandomCenter = GenVars.beachBordersWidth + 5 + 40;
            GenVars.beachSandRandomWidthRange = 20;
            GenVars.beachSandDungeonExtraWidth = 40;
            GenVars.beachSandJungleExtraWidth = 20;
            GenVars.oceanWaterStartRandomMin = 220;
            GenVars.oceanWaterStartRandomMax = GenVars.oceanWaterStartRandomMin + 40;
            GenVars.oceanWaterForcedJungleLength = 275;
            GenVars.leftBeachEnd = 0;
            GenVars.rightBeachStart = 0;
            GenVars.evilBiomeBeachAvoidance = GenVars.beachSandRandomCenter + 60;
            GenVars.evilBiomeAvoidanceMidFixer = 50;
            GenVars.lakesBeachAvoidance = GenVars.beachSandRandomCenter + 20;
            GenVars.smallHolesBeachAvoidance = GenVars.beachSandRandomCenter + 20;
            GenVars.surfaceCavesBeachAvoidance = GenVars.beachSandRandomCenter + 20;
            GenVars.surfaceCavesBeachAvoidance2 = GenVars.beachSandRandomCenter + 20;
            GenVars.jungleOriginX = 0;
            GenVars.snowOriginLeft = 0;
            GenVars.snowOriginRight = 0;
            GenVars.logX = -1;
            GenVars.logY = -1;
            GenVars.dungeonLocation = 0;

            MultiworldPylon.worldFileSeed.SetValue(Main.ActiveWorldFileData, (index > 2 && index < 4 || index == 5 || index >= 7) ? Main.rand.Next() : Main.ActiveWorldFileData.Seed);

            switch (index)
            {
                case 4:
                case 5:
                    WorldGen.crimson = false;
                    break;
                case 6:
                case 7:
                    WorldGen.crimson = true;
                    break;
                case 8:
                    WorldGen.everythingWorldGen = true;
                    break;
                case 9:
                    WorldGen.drunkWorldGen = true;
                    WorldGen.drunkWorldGenText = true;
                    break;
                case 10:
                    WorldGen.getGoodWorldGen = true;
                    break;
                case 11:
                    WorldGen.notTheBees = true;
                    break;
                case 12:
                    WorldGen.tenthAnniversaryWorldGen = true;
                    break;
                case 13:
                    WorldGen.dontStarveWorldGen = true;
                    break;
                case 14:
                    WorldGen.remixWorldGen = true;
                    break;
                case 15:
                    WorldGen.noTrapsWorldGen = true;
                    break;
            }
        }
    }
    public class StandardWorldGenPass : GenPass
    {
        public int index;
        public StandardWorldGenPass(int i) : base("Standard World", 99) { index = i; }
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            var passes = new List<GenPass>();
            var passesFromMods = new List<GenPass>();
            var cfg = MultiworldPylon.instance.GetCfg();
            foreach (var item in WorldGen.VanillaGenPasses.Values)
            {
                if (index < 8)
                {
                    if (item.Name == "Floating Islands" && !cfg.GenerateFloatingIslands) continue;
                    if (item.Name == "Temple" && !cfg.GenerateDungeon) continue;
                    if (item.Name == "Dungeon" && !cfg.GenerateDungeon) continue;
                    if (item.Name == "Pyramids" && !cfg.GeneratePyramids) continue;
                    if (item.Name == "Jungle Temple" && !cfg.GenerateJungleTemple) continue;
                    if (item.Name == "Hives" && !cfg.GenerateHives) continue;
                    if (item.Name == "Altars" && !cfg.GenerateAltars) continue;
                }
                passesFromMods.Add(item);
                if ((index == 2 || index == 3) && item.Name == "Corruption") continue;
                passes.Add(item);
                progress.TotalWeight += item.Weight;
            }
            var prog = new GenerationProgress();
            prog.TotalWeight = progress.TotalWeight - 100;

            SystemLoader.ModifyWorldGenTasks(passesFromMods, ref prog.TotalWeight);

            foreach (var pass in passesFromMods)
            {
                if (!passes.Contains(pass))
                {
                    bool flag = index < 8 ? cfg.DN_AllowFullModdedWorldgen : cfg.DN_AllowFullModdedWorldgenPostMoonlord;
                    foreach (var item in MultiworldPylon.RegisteredModWorldGens)
                    {
                        if (item.Item1 == pass.Name && item.Item2.Contains(index))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                        passes.Add(pass);
                }
            }
            var remove = new List<GenPass>();
            foreach (var pass in passes)
            {
                if (!passesFromMods.Contains(pass))
                {
                    bool flag = index < 8 ? cfg.DN_AllowFullModdedWorldgen : cfg.DN_AllowFullModdedWorldgenPostMoonlord;
                    foreach (var item in MultiworldPylon.RegisteredModWorldGens)
                    {
                        if (item.Item1 == pass.Name && item.Item2.Contains(index))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                        remove.Add(pass);
                }
            }
            prog.TotalWeight = 100;
            foreach (var pass in passes)
            {
                if (!remove.Contains(pass))
                    prog.TotalWeight += pass.Weight;
            }

            GenVars.configuration = WorldGenConfiguration.FromEmbeddedPath("Terraria.GameContent.WorldBuilding.Configuration.json");

            foreach (var item in passes)
            {
                if (remove.Contains(item))
                    continue;
                progress.Start(item.Weight);
                try {
                    WorldGen._genRand = new(Main.ActiveWorldFileData.Seed);
                    Main._rand = new(Main.ActiveWorldFileData.Seed);
                    item.Apply(progress, GenVars.configuration.GetPassConfiguration(item.Name));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                progress.End();
            }
        }
    }
}

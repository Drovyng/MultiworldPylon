using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MultiworldPylon.Content;
using ReLogic.Content;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;
using Terraria.WorldBuilding;

namespace MultiworldPylon
{
	public class MultiworldPylon : Mod
	{
		public static MultiworldPylon instance;
		public static UserInterface customUI;
		public static MultiworldPylonUI customUIState;
		public static Asset<Texture2D> pylonRotating;
		public static UIWorldLoad worldLoadState;
		public static FieldInfo worldFileSeed;
		public static MethodInfo UpdateAndDrawModMenu;
		public static MethodInfo DrawBG;
        public static Asset<Texture2D> IconBase;
		public static List<(string, List<int>)> RegisteredModWorldGens;
		public static List<(string, Asset<Texture2D>, LocalizedText, System.Func<bool>)> RegisteredModSubworlds;
        public override void Load()
        {
            instance = this;
            RegisteredModWorldGens = new();
            RegisteredModSubworlds = new();
            IconBase = Assets.Request<Texture2D>("Content/IconBase");
        }
        public override object Call(params object[] args)
        {
            switch (args[0])
            {
                case 0:
                    if (args.Length < 3) return false;
                    var worlds = new List<int>();
                    for (int i = 2; i < args.Length; i++)
                    {
                        worlds.Add((int)args[i]);
                    }
                    RegisteredModWorldGens.Add(((string)args[1], worlds));
                    return true;
                case 1:
                    var func = () => true;
                    if (args.Length < 5 || args[1] == null) return false;
                    try
                    {
                        RegisteredModSubworlds.Add((
                            (string)args[1],
                            (Asset<Texture2D>)args[2] ?? IconBase,
                            (LocalizedText)args[3] ?? LocalizedText.Empty,
                            (System.Func<bool>)args[4] ?? func
                        ));
                    }
                    catch (System.Exception)
                    {
                        return false;
                    }
                    return true;
            }
            return false;
        }
        public MultiworldPylonConfig GetCfg()
        {
            var i = ModContent.GetInstance<MultiworldPylonConfig>();
            if (i == null) i = new();
            return i;
        }
        public override void PostSetupContent()
        {
            if (!Main.dedServ)
            {
                customUI = new();
                customUIState = new();
                worldLoadState = new();
                customUI.SetState(customUIState);
                pylonRotating = Assets.Request<Texture2D>("Content/Rotating");
            }
            worldFileSeed = typeof(WorldFileData).GetField("_seed", BindingFlags.Instance | BindingFlags.NonPublic);
            UpdateAndDrawModMenu = typeof(MenuLoader).GetMethod("UpdateAndDrawModMenu", BindingFlags.Static | BindingFlags.NonPublic);
            DrawBG = typeof(Main).GetMethod("DrawBG", BindingFlags.Instance | BindingFlags.NonPublic);
        }
        public static void CountdownNew()
        {
            var cfg = instance.GetCfg();
            if (NPC.downedMoonlord ? !cfg.CountdownInstantEndGame : !cfg.CountdownInstant)
                PylonTimeout = (NPC.downedMoonlord ? cfg.CountdownTimerEndGame : cfg.CountdownTimer) * 60;
        }
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            if (whoAmI != 256)
            {
                if (reader.ReadByte() > 0)
                {
                    CountdownNew();
                    var pack2 = GetPacket(4);
                    pack2.Write(PylonTimeout);
                    pack2.Send();
                    return;
                }
                var pack = GetPacket(4);
                pack.Write(PylonTimeout);
                pack.Send(whoAmI);
                return;
            }
            PylonTimeout = reader.ReadInt32();
            PylonTimeoutWait = false;
        }
        public static int PylonTimeout = -1;
        public static bool PylonTimeoutWait = true;
    }
    [BackgroundColor(32, 49, 36, 216)]
    public class MultiworldPylonConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        [Header("Stats")]
        [BackgroundColor(192, 129, 64, 192)]
        [DefaultValue(false)]
        public bool CountdownInstant;
        [BackgroundColor(192, 129, 64, 192)]
        [DefaultValue(300)]
        [Range(30, 600)]
        [Slider]
        public int CountdownTimer;
        [BackgroundColor(192, 129, 64, 192)]
        [DefaultValue(false)]
        public bool CountdownInstantEndGame;
        [BackgroundColor(192, 129, 64, 192)]
        [DefaultValue(150)]
        [Range(30, 600)]
        [Slider]
        public int CountdownTimerEndGame;
        [Header("PreMoonlordWorlds")]
        [BackgroundColor(54, 192, 64, 192)]
        [DefaultValue(false)]
        public bool GenerateDungeon;
        [BackgroundColor(54, 192, 64, 192)]
        [DefaultValue(false)]
        public bool GenerateJungleTemple;
        [BackgroundColor(54, 192, 64, 192)]
        [DefaultValue(false)]
        public bool GenerateHives;
        [BackgroundColor(54, 192, 64, 192)]
        [DefaultValue(false)]
        public bool GeneratePyramids;
        [BackgroundColor(54, 192, 64, 192)]
        [DefaultValue(false)]
        public bool GenerateAltars;
        [BackgroundColor(54, 192, 64, 192)]
        [DefaultValue(false)]
        public bool GenerateFloatingIslands;
        [Header("DoNot")]
        [BackgroundColor(192, 64, 64, 192)]
        [DefaultValue(false)]
        public bool DN_AllowFullModdedWorldgen;
        [BackgroundColor(192, 64, 64, 192)]
        [DefaultValue(false)]
        public bool DN_AllowFullModdedWorldgenPostMoonlord;
        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message)
        {
            return false;
        }
    }
    public class MultiworldPylonSystem : ModSystem
    {
        public static bool activated;
        public override void UpdateUI(GameTime gameTime)
        {
            if (activated)
            {
                MultiworldPylon.customUI.Update(gameTime);
                if (Main.LocalPlayer.controlInv) activated = false;
            }
        }
        public override void ClearWorld()
        {
            activated = false;
            if (Main.netMode == 1)
            {
                MultiworldPylon.PylonTimeout = -1;
                var i = MultiworldPylon.instance.GetPacket(1);
                i.Write((byte)0);
                i.Send(255);
            }
        }
        public override void SetupContent()
        {
            if (Main.netMode != 1) MultiworldPylon.PylonTimeout = 0;
        }
        public override void OnWorldUnload()
        {
            activated = false;
            if (Main.netMode == 1) MultiworldPylon.PylonTimeout = -1;
        }
        public bool DrawUI()
        {
            if (activated) MultiworldPylon.customUI.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
            return true;
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            var i = layers.FindIndex((e) => e.Name == "Vanilla: Mouse Text");
            if (i > 0)
            {
                layers.Insert(i, new LegacyGameInterfaceLayer("MultiworldPylon: UI", DrawUI, InterfaceScaleType.UI));
            }
        }
        public override void PreSaveAndQuit()
        {
            MultiworldPylon.PylonTimeout = -1;
        }
        public override void PreUpdateWorld()
        {
            if (Main.netMode == 0 && MultiworldPylon.PylonTimeout == -1) MultiworldPylon.PylonTimeout++;

            if (MultiworldPylon.PylonTimeout > 0)
            {
                MultiworldPylon.PylonTimeout--;
                if (Main.netMode == 2 && MultiworldPylon.PylonTimeout % 20 == 0)
                {
                    var lol = MultiworldPylon.instance.GetPacket(4);
                    lol.Write(MultiworldPylon.PylonTimeout);
                    lol.Send();
                }
            }
        }
    }
}

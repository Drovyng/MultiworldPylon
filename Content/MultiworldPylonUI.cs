using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace MultiworldPylon.Content
{
    public class MultiworldPylonUI : UIState
    {
        public class UIWorldButton : UIElement
        {
            public int index;
            public bool locked = false;
            public UIWorldButton(int index) : base()
            {
                Width.Pixels = CellSizeIn;
                Height.Pixels = CellSizeIn;
                this.index = index;
            }
            public override void OnInitialize()
            {
                Append(GetIconElement(index));
                if (index > 2 && index < 4 || index == 5 || index == 7)
                    Append(new UIImage(Main.Assets.Request<Texture2D>("Images/UI/WorldCreation/IconEvilRandom"))
                    {
                        HAlign = 1,
                        VAlign = 1,
                        ImageScale = 0.8f,
                        IgnoresMouseInteraction = true
                    });
                var pan = new UIPanel() { Width = { Percent = 1 }, Height = { Percent = 1 }, BackgroundColor = Color.Transparent };
                Append(pan);
                if ((((index != 0 && SubworldSystem.Current != null) || (index > 7 && index < 16 && !NPC.downedMoonlord)) && !NPC.downedMoonlord) || (index > 15 && !MultiworldPylon.RegisteredModSubworlds[index - 16].Item4()))
                {
                    UICommon.WithFadedMouseOver(pan, new Color(0, 0, 0, 0.35f), new Color(0, 0, 0, 0.2f), Color.Red, default);
                    Append(new UIImage(Main.Assets.Request<Texture2D>("Images/UI/Workshop/PublicityPrivate"))
                    {
                        HAlign = 0.5f,
                        VAlign = 0.5f,
                        ImageScale = 0.8f,
                        Color = new Color(1, 1, 1, 0.25f),
                        IgnoresMouseInteraction = true
                    });
                    locked = true;
                }
                else
                    UICommon.WithFadedMouseOver(pan, new Color(1, 1, 1, 0), new Color(1, 1, 1, 0), index == 0 ? Color.Lime : default);
            }
            protected Asset<Texture2D> GetIcon(int index)
            {
                if (index > 15) return MultiworldPylon.RegisteredModSubworlds[index - 16].Item2;
                if (index < 4)
                    return MultiworldPylon.IconBase;

                if (index == 9)
                    return Main.Assets.Request<Texture2D>("Images/UI/IconCorruptionCrimson");

                if (index == 10)
                    return GetSeedIcon("FTW");

                if (index == 11)
                    return GetSeedIcon("NotTheBees");

                if (index == 12)
                    return GetSeedIcon("Anniversary");

                if (index == 13)
                    return GetSeedIcon("DontStarve");

                if (index == 14)
                    return GetSeedIcon("Remix");

                if (index == 15)
                    return GetSeedIcon("Traps");

                return Main.Assets.Request<Texture2D>("Images/UI/Icon" + (index < 6 ? "Corruption" : "Crimson"));
            }

            protected UIElement GetIconElement(int index)
            {
                if (index == 8)
                {
                    Asset<Texture2D> asset = Main.Assets.Request<Texture2D>("Images/UI/IconEverythingAnimated");
                    UIImageFramed uIImageFramed = new UIImageFramed(asset, asset.Frame(7, 16));
                    uIImageFramed.OnUpdate += UpdateGlitchAnimation;
                    uIImageFramed.IgnoresMouseInteraction = true;
                    return uIImageFramed;
                }

                return new UIImage(GetIcon(index))
                {
                    IgnoresMouseInteraction = true
                };
            }
            protected int _glitchFrameCounter;
            protected int _glitchFrame;
            protected int _glitchVariation;

            private void UpdateGlitchAnimation(UIElement affectedElement)
            {
                _ = _glitchFrame;
                int minValue = 3;
                int num = 3;
                if (_glitchFrame == 0)
                {
                    minValue = 15;
                    num = 120;
                }

                if (++_glitchFrameCounter >= Main.rand.Next(minValue, num + 1))
                {
                    _glitchFrameCounter = 0;
                    _glitchFrame = (_glitchFrame + 1) % 16;
                    if ((_glitchFrame == 4 || _glitchFrame == 8 || _glitchFrame == 12) && Main.rand.Next(3) == 0)
                        _glitchVariation = Main.rand.Next(7);
                }

                (affectedElement as UIImageFramed).SetFrame(7, 16, _glitchVariation, _glitchFrame, 0, 0);
            }

            private Asset<Texture2D> GetSeedIcon(string seed) => Main.Assets.Request<Texture2D>("Images/UI/Icon" + (Main.ActiveWorldFileData.HasCorruption ? "Corruption" : "Crimson") + seed);

            public override void LeftClick(UIMouseEvent evt)
            {
                if (!locked)
                {
                    MultiworldPylon.PylonTimeoutWait = Main.netMode == 1 && index > 0;
                    MultiworldPylon.customUIState.WaitIndex = index;
                    if (Main.netMode == 1 && SubworldSystem.Current == null)
                    {
                        var i = MultiworldPylon.instance.GetPacket(1);
                        i.Write((byte)0);
                        i.Send(255);
                    }
                }
            }
            public override void Draw(SpriteBatch spriteBatch)
            {
                base.Draw(spriteBatch);
                if (IsMouseHovering)
                {
                    var tooltip = Language.GetTextValue("Mods.MultiworldPylon.UI.Home");
                    switch (index)
                    {
                        case 0:
                            break;
                        case 1:
                            tooltip = Language.GetTextValue("Mods.MultiworldPylon.UI.Normal"); break;
                        case 2:
                            tooltip = Language.GetTextValue("Mods.MultiworldPylon.UI.Purity"); break;
                        case 3:
                            tooltip = Language.GetTextValue("Mods.MultiworldPylon.UI.PurityRNG"); break;
                        case 4:
                        case 5:
                            tooltip = Language.GetTextValue("Mods.MultiworldPylon.UI.Corruption"); break;
                        case 6:
                        case 7:
                            tooltip = Language.GetTextValue("Mods.MultiworldPylon.UI.Crimson"); break;
                        case 8:
                            tooltip = "getfixedboy"; break;
                        case 9:
                            tooltip = "Drunk World"; break;
                        case 10:
                            tooltip = "fortheworthy"; break;
                        case 11:
                            tooltip = "notthebees"; break;
                        case 12:
                            tooltip = "celebrationmk10"; break;
                        case 13:
                            tooltip = "constant"; break;
                        case 14:
                            tooltip = "dontdigup"; break;
                        case 15:
                            tooltip = "notraps"; break;
                        default:
                            tooltip = MultiworldPylon.RegisteredModSubworlds[index - 16].Item3.ToString();
                            break;
                    }
                    if (index == 5 || index == 7)
                        tooltip += " " + Language.GetTextValue("Mods.MultiworldPylon.UI.RNG");
                    if (locked)
                    {
                        tooltip += " [" + Language.GetTextValue("Mods.MultiworldPylon.UI.Locked") + "]";
                    }
                    UICommon.TooltipMouseText(tooltip);
                }
            }
        }
        public const float CellSize = 66;
        public const float CellSizeIn = CellSize - 6;
        public const float PanelWidth = CellSize * 8 + 18;
        public static float PanelHeight = CellSize * 2 + 18 + 70;

        public bool holding;
        public UIPanel panel;
        public UIText countdown;
        public List<UIElement> buttons;
        public Vector2 grabPos = Vector2.Zero;
        public Vector2 panelPos = Vector2.Zero;

        public override void OnInitialize()
        {
            panel = new UIPanel()
            {
                Width = { Pixels = PanelWidth },
                Height = { Pixels = PanelHeight + (MultiworldPylon.RegisteredModSubworlds.Count + 7) / 8 * CellSize }
            };
            Append(panel);
            buttons = new();
        }
        public override void OnDeactivate()
        {
            WaitIndex = -1;
        }
        public override void OnActivate()
        {
            WaitIndex = -1;
            if (Main.netMode == 1)
            {
                var j = MultiworldPylon.instance.GetPacket(1);
                j.Write((byte)0);
                j.Send(255);
            }
            panel.RemoveAllChildren();
            panel.Append(new UIText(Language.GetTextValue("Mods.MultiworldPylon.UI.SelectWorld"), 0.8f, true)
            {
                HAlign = 0.5f,
                Top = { Pixels = 8 }
            });
            buttons.Clear();
            panel.Left.Pixels = Main.screenWidth / Main.UIScale * 0.5f - panel.Width.Pixels * 0.5f;
            panel.Top.Pixels = Main.screenHeight / Main.UIScale * 0.5f - panel.Height.Pixels * 0.5f;
            for (int i = 0; i < 16; i++)
            {
                var btn = new UIWorldButton(i);
                btn.Left.Pixels = buttons.Count % 8 * CellSize;
                btn.Top.Pixels = buttons.Count / 8 * CellSize + 50;
                buttons.Add(btn);
                panel.Append(btn);
            }
            var count = MultiworldPylon.RegisteredModSubworlds.Count;
            for (int i = 0; i < count; i++)
            {
                var btn = new UIWorldButton(16 + i);
                btn.Left.Pixels = buttons.Count % 8 * CellSize;
                btn.Top.Pixels = buttons.Count / 8 * CellSize + CellSize * 2 + 50;
                buttons.Add(btn);
                panel.Append(btn);
            }
            countdown = new UIText("Charge: 0%", 0.4f, true)
            {
                HAlign = 0.5f,
                VAlign = 1
            };
            panel.Append(countdown);
        }
        public int WaitIndex = -1;
        public void JoinWorld()
        {
            if (WaitIndex == -1 || MultiworldPylon.PylonTimeoutWait || (MultiworldPylon.PylonTimeout > 0 && WaitIndex > 0))
            {
                if (!MultiworldPylon.PylonTimeoutWait) WaitIndex = -1;
                return;
            }
            if (WaitIndex > 0) {
                if (Main.netMode == 1)
                {
                    var i = MultiworldPylon.instance.GetPacket(1);
                    i.Write((byte)1);
                    i.Send(255);
                }
                else MultiworldPylon.CountdownNew();
            }
            switch (WaitIndex)
            {
                case 1:
                    SubworldSystem.Enter<MultiWorld1>(); break;
                case 2:
                    SubworldSystem.Enter<MultiWorld2>(); break;
                case 3:
                    SubworldSystem.Enter<MultiWorld3>(); break;
                case 4:
                    SubworldSystem.Enter<MultiWorld4>(); break;
                case 5:
                    SubworldSystem.Enter<MultiWorld5>(); break;
                case 6:
                    SubworldSystem.Enter<MultiWorld6>(); break;
                case 7:
                    SubworldSystem.Enter<MultiWorld7>(); break;
                case 8:
                    SubworldSystem.Enter<MultiWorld8>(); break;
                case 9:
                    SubworldSystem.Enter<MultiWorld9>(); break;
                case 10:
                    SubworldSystem.Enter<MultiWorld10>(); break;
                case 11:
                    SubworldSystem.Enter<MultiWorld11>(); break;
                case 12:
                    SubworldSystem.Enter<MultiWorld12>(); break;
                case 13:
                    SubworldSystem.Enter<MultiWorld13>(); break;
                case 14:
                    SubworldSystem.Enter<MultiWorld14>(); break;
                case 15:
                    SubworldSystem.Enter<MultiWorld15>(); break;
                default:
                    if (WaitIndex > 15)
                    {
                        SubworldSystem.Enter(MultiworldPylon.RegisteredModSubworlds[WaitIndex - 16].Item1);
                        break;
                    }
                    SubworldSystem.Exit();
                    break;
            }
            WaitIndex = 0;
        }
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            if (evt.Target == panel && panel.IsMouseHovering)
            {
                holding = true;
                grabPos = MultiworldPylon.customUI.MousePosition;
                panelPos.X = panel.Left.Pixels;
                panelPos.Y = panel.Top.Pixels;
                Main.LocalPlayer.mouseInterface = true;
                Main.mouseLeft = false;
                return;
            }
        }
        public override void Update(GameTime gameTime)
        {
            Main.LocalPlayer.mouseInterface = Main.LocalPlayer.mouseInterface || holding || panel.IsMouseHovering;

            if (MultiworldPylon.PylonTimeout != -1)
            {
                var cfg = MultiworldPylon.instance.GetCfg();
                if (NPC.downedMoonlord ? !cfg.CountdownInstantEndGame : !cfg.CountdownInstant)
                {
                    var perc = 1000 - MultiworldPylon.PylonTimeout * 1000 / 60 / (NPC.downedMoonlord ? cfg.CountdownTimerEndGame : cfg.CountdownTimer);
                    countdown.SetText(Language.GetTextValue("Mods.MultiworldPylon.UI.ChargeText") + " " + (perc / 10f) + (perc % 10 == 0 ? ".0" : "") + "%");
                }
                else
                    countdown.SetText(Language.GetTextValue("Mods.MultiworldPylon.UI.ChargeText") + " 100%");
            }
            else
                countdown.SetText(Language.GetTextValue("Mods.MultiworldPylon.UI.ChargeText") + " NAN");

            JoinWorld();

            if (holding)
            {
                Main.LocalPlayer.mouseInterface = true;
                var offPos = MultiworldPylon.customUI.MousePosition - grabPos;
                panel.Left.Pixels = panelPos.X + offPos.X;
                panel.Top.Pixels = panelPos.Y + offPos.Y;
            }

            if (panel.Left.Pixels < 0) panel.Left.Pixels = 0;
            else if (panel.Left.Pixels + panel.Width.Pixels > Main.screenWidth) panel.Left.Pixels = Main.screenWidth - panel.Width.Pixels;

            if (panel.Top.Pixels < 0) panel.Top.Pixels = 0;
            else if (panel.Top.Pixels + panel.Height.Pixels > Main.screenHeight) panel.Top.Pixels = Main.screenHeight - panel.Height.Pixels;

            base.Update(gameTime);
        }
        public override void LeftMouseUp(UIMouseEvent evt)
        {
            if (holding)
            {
                panelPos.X = panel.Left.Pixels;
                panelPos.Y = panel.Top.Pixels;
            }
            holding = false;
            Main.LocalPlayer.mouseInterface = true;
        }
    }
}

using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MultiworldPylon.Content
{
    public class MultiworldPylonTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = true;
            Main.tileWaterDeath[Type] = false;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileCut[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.Origin = new Point16(1, 4);
            TileObjectData.newTile.CoordinateHeights = new int[5] {
                16,
                16,
                16,
                16,
                16
            };
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(150, 200, 175));
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return true;
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.cursorItemIconID = ModContent.ItemType<MultiworldPylonItem>();
            player.cursorItemIconText = "";
            player.cursorItemIconEnabled = true;
        }
        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (drawData.tileFrameX % TileObjectData.GetTileData(drawData.tileCache).CoordinateFullWidth == 0 && drawData.tileFrameY == 0)
            {
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
            }
        }
        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            DefaultDrawPylonCrystal(spriteBatch, i, j, new Vector2(0, -12f), Color.White * 0.1f, Color.White, 4);
        }
        public override bool RightClick(int i, int j)
        {
            MultiworldPylonSystem.activated = !MultiworldPylonSystem.activated;
            SoundEngine.PlaySound(MultiworldPylonSystem.activated ? SoundID.MenuOpen : SoundID.MenuClose);

            if (MultiworldPylonSystem.activated)
                MultiworldPylon.customUIState.Activate();
            else
                MultiworldPylon.customUIState.Deactivate();

            return true;
        }
        public void DefaultDrawPylonCrystal(SpriteBatch spriteBatch, int i, int j, Vector2 crystalOffset, Color pylonShadowColor, Color dustColor, int dustChanceDenominator)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null);
            // Gets offscreen vector for different lighting modes
            Vector2 offscreenVector = new Vector2(Main.offScreenRange);
            if (Main.drawToScreen)
            {
                offscreenVector = Vector2.Zero;
            }
            // Double check that the tile exists
            Point point = new Point(i, j);
            Tile tile = Main.tile[point.X, point.Y];
            if (tile == null || !tile.HasTile)
            {
                return;
            }

            TileObjectData tileData = TileObjectData.GetTileData(tile);

            var texture = MultiworldPylon.pylonRotating;

            // Frame our modded crystal sheet accordingly for proper drawing

            Rectangle crystalFrame = new(0, 0, 28, 36);
            Rectangle smartCursorGlowFrame = new(30, 0, 28, 36);

            // I have no idea what is happening here; but it fixes the frame bleed issue. All I know is that the vertical sinusoidal motion has something to with it.
            // If anyone else has a clue as to why, please do tell. - MutantWafflez
            //         crystalFrame.Height -= 1;
            //         smartCursorGlowFrame.Height -= 1;

            // Calculate positional variables for actually drawing the crystal
            Vector2 origin = crystalFrame.Size() / 2f;
            Vector2 tileOrigin = new Vector2(tileData.CoordinateFullWidth / 2f, tileData.CoordinateFullHeight / 2f);
            Vector2 crystalPosition = point.ToWorldCoordinates(tileOrigin.X - 2f, tileOrigin.Y) + crystalOffset;

            // Calculate additional drawing positions with a sine wave movement
            float sinusoidalOffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * (Math.PI * 2) / 5);
            Vector2 drawingPosition = crystalPosition + offscreenVector + new Vector2(0f, sinusoidalOffset * 4f - 6);

            // Do dust drawing
            if (!Main.gamePaused && Main.instance.IsActive && (!Lighting.UpdateEveryFrame || Main.rand.NextBool(4)) && Main.rand.NextBool(dustChanceDenominator))
            {
                Rectangle dustBox = Utils.CenteredRectangle(crystalPosition, crystalFrame.Size() * 1.5f);
                int numForDust = Dust.NewDust(dustBox.TopLeft(), dustBox.Width, dustBox.Height, DustID.TintableDustLighted, 0f, 0f, 254, dustColor, 0.5f);
                Dust obj = Main.dust[numForDust];
                obj.velocity *= 0.1f;
                Main.dust[numForDust].velocity.Y -= 0.2f;
            }
            
            // Get color value and draw the the crystal
            var color = Color.Lerp(Lighting.GetColor(point.X, point.Y), Color.White, 0.8f);
            spriteBatch.Draw(texture.Value, drawingPosition - Main.screenPosition, crystalFrame, color * 0.8f, 0f, origin, 1.5f, SpriteEffects.None, 0f);
            
            float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * ((float)Math.PI * 2f) / 1f) * 0.2f + 0.8f;
            Color shadowColor = pylonShadowColor * scale;
            for (float shadowPos = 0f; shadowPos < 1f; shadowPos += 1f / 6f)
            {
                spriteBatch.Draw(texture.Value, drawingPosition - Main.screenPosition + ((float)Math.PI * 2f * shadowPos).ToRotationVector2() * (6f + sinusoidalOffset * 2f), crystalFrame, shadowColor, 0f, origin, 1.5f, SpriteEffects.None, 0f);
            }

            // Interpret smart cursor outline color & draw it
            int selectionLevel = 0;
            if (Main.InSmartCursorHighlightArea(point.X, point.Y, out bool actuallySelected))
            {
                selectionLevel = 1;
                if (actuallySelected)
                {
                    selectionLevel = 2;
                }
            }

            if (selectionLevel == 0)
            {
                return;
            }

            int averageBrightness = (color.R + color.G + color.B) / 3;

            if (averageBrightness <= 10)
            {
                return;
            }

            Color selectionGlowColor = Colors.GetSelectionGlowColor(selectionLevel == 2, averageBrightness);
            spriteBatch.Draw(texture.Value, drawingPosition - Main.screenPosition, smartCursorGlowFrame, selectionGlowColor, 0f, origin, 1.5f, SpriteEffects.None, 0f);
        }

    }
}

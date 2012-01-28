using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using libdiablo3;
using libdiablo3.Api;

namespace TreasureMap
{
    public partial class frmTreasureMap : Form
    {
        private bool running;
        private Thread updateThread;
        private Diablo3Api d3Api;
        private World world;

        private Bitmap stageBitmap;
        private Bitmap previewBitmap;
        private readonly Pen masterScenePen = new Pen(Color.Black, 1.0f);
        private readonly Pen subScenePen = new Pen(Color.DarkGray, 1.0f);
        private readonly Brush unwalkableBrush = Brushes.Red;
        private readonly Pen unwalkablePen = new Pen(Color.Red, 1.0f);
        private readonly Brush walkableBrush = Brushes.Blue;
        private readonly Pen walkablePen = new Pen(Color.Blue, 1.0f);

        public frmTreasureMap()
        {
            InitializeComponent();

            // FIXME: Check for elevated privileges and relaunch if we don't have them
            this.DoubleBuffered = true;

            running = true;
            d3Api = new Diablo3Api();
            d3Api.OnLogMessage += LogMessageCallback;

            updateThread = new Thread(new ThreadStart(UpdateLoop));
            updateThread.Start();
        }

        private void Log(LogLevel level, string msg)
        {
            msg = String.Format("[{0}]: {1}", level.ToString().ToUpper(), msg);
            // FIXME:
        }

        private void UpdateMap()
        {
            stageBitmap = Draw();
            previewBitmap = ResizeImage(stageBitmap, picPreview.Width, picPreview.Height);
            UpdateControls();
        }

        private delegate void UpdateControlsCallback();
        private void UpdateControls()
        {
            try
            {
                if (picMap.InvokeRequired)
                {
                    this.Invoke(new UpdateControlsCallback(UpdateControls));
                    return;
                }

                picMap.Image = stageBitmap;
                picPreview.Image = previewBitmap;
            }
            catch { }
        }

        private Bitmap Draw()
        {
            // Find the world bounding box
            // FIXME: This should use scenes, not actors
            AABB worldBox = new AABB(
                new Vector3f(Single.MaxValue, Single.MaxValue, 0f),
                new Vector3f(Single.MinValue, Single.MinValue, 0f));
            foreach (Actor actor in world.AllActors)
            {
                AABB aabb = actor.BoundingBox;
                if (aabb.Min.X == 0f && aabb.Min.Y == 0f)
                    continue;

                if (aabb.Min.X < worldBox.Min.X) worldBox.Min.X = aabb.Min.X;
                if (aabb.Min.Y < worldBox.Min.Y) worldBox.Min.Y = aabb.Min.Y;
                if (aabb.Max.X > worldBox.Max.X) worldBox.Max.X = aabb.Max.X;
                if (aabb.Max.Y > worldBox.Max.Y) worldBox.Max.Y = aabb.Max.Y;
            }

            int width = Math.Max((int)worldBox.Max.X + 1, 256);
            int height = Math.Max((int)worldBox.Max.Y + 1, 256);

            // Create the bitmap
            var bitmap = new Bitmap(width, height, PixelFormat.Format16bppRgb555);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.Default;

                graphics.FillRectangle(Brushes.LightGray, 0, 0, bitmap.Width, bitmap.Height);

                this.DrawShapes(graphics);

                //if (this.PrintSceneLabels)
                //    this.DrawLabels(graphics);

                graphics.Save();
            }

            return bitmap;
        }

        private void DrawShapes(Graphics graphics)
        {
            foreach (Scene scene in world.Scenes)
                DrawScene(graphics, scene);

            foreach (Gizmo gizmo in world.Gizmos)
            {
                AABB aabb = gizmo.BoundingBox;
                if (aabb.Min.X == 0f && aabb.Min.Y == 0f) continue;
                switch (gizmo.GizmoType)
                {
                    case GizmoGroup.Door:
                    case GizmoGroup.Portal:
                    case GizmoGroup.TownPortal:
                    case GizmoGroup.Waypoint:
                    case GizmoGroup.HearthPortal:
                        DrawActor(gizmo, graphics, Brushes.Green, 7);
                        break;
                    case GizmoGroup.Barricade:
                        DrawActor(gizmo, graphics, Brushes.Brown, 7);
                        break;
                    case GizmoGroup.Destructible:
                        break;
                    case GizmoGroup.DestructibleLootContainer:
                    case GizmoGroup.LootContainer:
                    case GizmoGroup.QuestLoot:
                    case GizmoGroup.Healthwell:
                        DrawActor(gizmo, graphics, Brushes.DarkViolet, 7);
                        break;
                    case GizmoGroup.PlayerSharedStash:
                    case GizmoGroup.CheckPoint:
                    case GizmoGroup.BossPortal:
                    case GizmoGroup.DungeonStonePortal:
                    case GizmoGroup.Savepoint:
                        DrawActor(gizmo, graphics, Brushes.Blue, 7);
                        break;
                }
            }
            foreach (Hero hero in world.Heros)
            {
                AABB aabb = hero.BoundingBox;
                if (aabb.Min.X == 0f && aabb.Min.Y == 0f) continue;
                DrawActor(hero, graphics, Brushes.White, 10);
            }
            foreach (Item item in world.Items)
            {
                AABB aabb = item.BoundingBox;
                if (aabb.Min.X == 0f && aabb.Min.Y == 0f) continue;
                if (item.Type == ActorName.GoldCoin || item.Type == ActorName.GoldCoins)
                    DrawActor(item, graphics, Brushes.Gold, 7);
                else
                    DrawActor(item, graphics, Brushes.CornflowerBlue, 7);
            }
            foreach (Monster monster in world.Monsters)
            {
                AABB aabb = monster.BoundingBox;
                if (aabb.Min.X == 0f && aabb.Min.Y == 0f) continue;
                DrawActor(monster, graphics, Brushes.Red, 7);
            }
            foreach (NPC npc in world.NPCs)
            {
                AABB aabb = npc.BoundingBox;
                if (aabb.Min.X == 0f && aabb.Min.Y == 0f) continue;
                DrawActor(npc, graphics, Brushes.Orange, 7);
            }
        }

        private void DrawScene(Graphics graphics, Scene scene)
        {
            AABB aabb = scene.BoundingBox;
            var rect = new Rectangle((int)aabb.Min.X, (int)aabb.Min.Y, (int)aabb.Width, (int)aabb.Height);
            graphics.DrawRectangle(/*scene.Parent == null ? masterScenePen : subScenePen*/ masterScenePen, rect);

            DrawCells(graphics, scene);
        }

        private void DrawCells(Graphics graphics, Scene scene)
        {
            foreach (NavCell cell in scene.NavCells)
            {
                if ((cell.Flags & NavCellFlags.AllowWalk) != NavCellFlags.AllowWalk)
                    continue;

                AABB aabb = cell.BoundingBox;
                aabb.Min += scene.BoundingBox.Min;
                aabb.Max += scene.BoundingBox.Min;
                var rect = new Rectangle(new Point((int)aabb.Min.X, (int)aabb.Min.Y), new Size((int)aabb.Width, (int)aabb.Height));
                graphics.DrawRectangle(walkablePen, rect);
                //graphics.FillRectangle(walkableBrush, rect);
            }
        }

        private void DrawActor(Actor actor, Graphics graphics, Brush brush, int radius)
        {
            AABB aabb = actor.BoundingBox;
            var rect = new Rectangle((int)aabb.Min.X, (int)aabb.Min.Y, (int)aabb.Width + radius, (int)aabb.Height + radius);
            graphics.FillEllipse(brush, rect);
        }

        private void UpdateLoop()
        {
            if (!d3Api.IsDiabloRunning())
            {
                Log(LogLevel.Info, "Waiting for Diablo III process...");

                while (!d3Api.IsDiabloRunning())
                {
                    if (!running) return;
                    Thread.Sleep(500);
                }
            }
            Log(LogLevel.Info, "Found Diablo III process");

            try
            {
                if (!d3Api.Init())
                {
                    Log(LogLevel.Info, "Waiting for Diablo III game...");

                    while (!d3Api.Init())
                    {
                        if (!running) return;
                        Thread.Sleep(500);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, "Failed to initialize the D3 API: " + ex.Message);
                return;
            }
            
            world = d3Api.InitWorld();
            Log(LogLevel.Info, "Initialized D3 world");

            // Main loop
            while (running)
            {
                Thread.Sleep(100);
                d3Api.UpdateWorld(world);
                UpdateMap();
            }

            Log(LogLevel.Info, "Shutting down...");
            d3Api.Shutdown();
        }

        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            //a holder for the result
            var result = new Bitmap(width, height);

            //use a graphics object to draw the resized image into the bitmap
            using (Graphics graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                //draw the image into the target bitmap
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            //return the resulting bitmap
            return result;
        }

        private void LogMessageCallback(LogLevel level, string message, Exception ex)
        {
            if (ex != null)
                message += Environment.NewLine + ex.ToString();
            Log(level, message);
        }

        private void frmTreasureMap_FormClosing(object sender, FormClosingEventArgs e)
        {
            running = false;
        }
    }
}

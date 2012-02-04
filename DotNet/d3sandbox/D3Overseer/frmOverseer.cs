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

namespace D3Overseer
{
    public partial class frmOverseer : Form
    {
        private delegate void LogCallback(LogLevel level, string msg);
        private delegate void UpdateControlsCallback();

        private bool running;
        private Thread updateThread;
        private Diablo3Api d3Api;
        private World world;

        private int kills;
        private int prevGold;
        private int prevXP;
        private RateTracker goldRate = new RateTracker(TimeSpan.FromHours(1.0));
        private RateTracker xpRate = new RateTracker(TimeSpan.FromHours(1.0));

        #region Map Members

        private Bitmap stageBitmap;
        private Bitmap previewBitmap;
        private readonly Pen masterScenePen = new Pen(Color.DarkGray, 1.0f);
        private readonly Pen subScenePen = new Pen(Color.LightGray, 1.0f);
        private readonly Brush unwalkableBrush = Brushes.Red;
        private readonly Pen unwalkablePen = new Pen(Color.Red, 1.0f);
        private readonly Brush walkableBrush = Brushes.Blue;
        private readonly Pen walkablePen = new Pen(Color.Blue, 1.0f);

        #endregion Map Members

        private readonly Pen emptyInventoryPen = new Pen(Color.DarkGray, 1.0f);
        private readonly Pen fullInventoryPen = new Pen(Color.Black, 1.0f);

        public frmOverseer()
        {
            InitializeComponent();

            // FIXME: Check for elevated privileges and relaunch if we don't have them

            this.DoubleBuffered = true;

            lblVersion.Text = String.Format("Overseer {0} (#{1}) - {2}",
                Utils.GetBuildVersion(), 1, Utils.GetBuildDate().ToString("MMMM d, yyyy"));

            running = true;
            d3Api = new Diablo3Api();
            d3Api.OnLogMessage += LogMessageCallback;

            updateThread = new Thread(new ThreadStart(UpdateLoop));
            updateThread.Start();
        }

        private void Log(LogLevel level, string msg)
        {
            try
            {
                if (txtLog.InvokeRequired)
                {
                    txtLog.Invoke(new LogCallback(Log), new object[] { level, msg });
                    return;
                }

                msg = String.Format("[{0}]: {1}", level.ToString().ToUpper(), msg);
                txtLog.AppendText(msg + Environment.NewLine);
            }
            catch { }
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

                if (world.Me != null)
                {
                    // Track gold changes over time
                    if (world.Me.Gold != prevGold)
                    {
                        if (prevGold != 0)
                        {
                            int goldDiff = world.Me.Gold - prevGold;
                            goldRate.AddValue(goldDiff);
                        }
                        prevGold = world.Me.Gold;
                    }

                    // Track XP changes over time
                    int totalXP = world.Me.TotalXP;
                    if (totalXP != prevXP)
                    {
                        if (prevXP != 0)
                        {
                            int xpDiff = totalXP - prevXP;
                            xpRate.AddValue(xpDiff);
                        }
                        prevXP = totalXP;
                    }
                }

                UpdateControls();
            }

            Log(LogLevel.Info, "Shutting down...");
            d3Api.Shutdown();
        }

        private void UpdateControls()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new UpdateControlsCallback(UpdateControls));
                    return;
                }

                if (world.Scenes == null && world.Me == null)
                    lblAttached.Text = "No";
                else if (world.Me == null)
                    lblAttached.Text = "Out of Game";
                else
                    lblAttached.Text = "Yes (#" + d3Api.ProcessID + ")";

                if (world.Me != null)
                {
                    lblKills.Text = kills.ToString();

                    lblGold.Text = world.Me.Gold.ToString();
                    lblGoldRate.Text = goldRate.PerHour.ToString("0.0");

                    // Health
                    progHealth.ForeColor = Color.Red;
                    progHealth.Maximum = world.Me.Health.Max;
                    progHealth.Value = world.Me.Health.Value;

                    // Resource1
                    switch (world.Me.PrimaryResourceType)
                    {
                        case ResourceType.Fury:
                            progRes1.ForeColor = Color.OrangeRed;
                            lblRes1.Text = "Fury";
                            break;
                        case ResourceType.Hatred:
                            progRes1.ForeColor = Color.OrangeRed;
                            lblRes1.Text = "Hatred";
                            break;
                        //case ResourceType.Focus:
                        //case ResourceType.Mana:
                        //case ResourceType.ArcanePower:
                    }
                    progRes1.Maximum = world.Me.PrimaryResource.Max;
                    progRes1.Value = world.Me.PrimaryResource.Value;

                    // Resource2
                    if (world.Me.SecondaryResourceType != ResourceType.None)
                    {
                        progRes2.ForeColor = Color.Blue;
                        lblRes2.Text = "Discipline";

                        progRes2.Maximum = world.Me.SecondaryResource.Max;
                        progRes2.Value = world.Me.SecondaryResource.Value;

                        progRes2.Visible = true;
                        lblRes2.Visible = true;
                    }
                    else
                    {
                        progRes2.Visible = false;
                        lblRes2.Visible = false;
                    }

                    // Level/Experience
                    lblLevel.Text = world.Me.Level.ToString();
                    lblXP.Text = String.Format("{0} / {1}", world.Me.XP, world.Me.XPNextLevel);
                    lblXPRate.Text = xpRate.PerHour.ToString("0.0");
                    progExp.Maximum = world.Me.XPNextLevel;
                    progExp.Value = world.Me.XP;

                    DrawInventory();
                }

                if (world.Scenes != null)
                {
                    UpdateMap();
                }
            }
            catch { }
        }

        #region Map Rendering

        private void UpdateMap()
        {
            stageBitmap = Draw();
            previewBitmap = ResizeImage(stageBitmap, picPreview.Width, picPreview.Height);

            picMap.Image = stageBitmap;
            picPreview.Image = previewBitmap;
        }

        private Bitmap Draw()
        {
            // Find the world bounding box
            AABB worldBox = new AABB(
                new Vector3f(Single.MaxValue, Single.MaxValue, 0f),
                new Vector3f(Single.MinValue, Single.MinValue, 0f));

            foreach (Scene scene in world.Scenes)
            {
                AABB aabb = scene.BoundingBox;
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
            if (world.Scenes == null)
                return;

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
            if (world.Me != null)
                DrawActor(world.Me, graphics, Brushes.White, 10);
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

        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            //a holder for the result
            var result = new Bitmap(width, height);

            //use a graphics object to draw the resized image into the bitmap
            using (Graphics graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.Bilinear;
                graphics.PixelOffsetMode = PixelOffsetMode.None;
                graphics.SmoothingMode = SmoothingMode.None;

                //draw the image into the target bitmap
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            //return the resulting bitmap
            return result;
        }

        #endregion Map Rendering

        #region Inventory Rendering

        private void DrawInventory()
        {
            int rows = world.Me.Backpack.Slots.GetLength(0);
            int columns = world.Me.Backpack.Slots.GetLength(1);

            int width = columns * 32 + 1;
            int height = rows * 32 + 1;

            // Create the bitmap
            var bitmap = new Bitmap(width, height, PixelFormat.Format16bppRgb555);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

                graphics.FillRectangle(Brushes.LightGray, 0, 0, bitmap.Width, bitmap.Height);

                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < columns; x++)
                    {
                        var rect = new Rectangle(x * 32, y * 32, 32, 32);
                        graphics.DrawRectangle(emptyInventoryPen, rect);
                    }
                }

                foreach (Item item in world.Me.Backpack.GetItems())
                {
                    var rect = new Rectangle(item.InventoryX * 32, item.InventoryY * 32, 32, item.Is1x2 ? 64 : 32);
                    var color = IntToColor(item.ItemType.Hash); //colorBrushes[(uint)item.ItemType.BaseType.Hash % colorBrushes.Length];
                    var brush = new SolidBrush(color);
                    graphics.FillRectangle(brush, rect);
                    graphics.DrawRectangle(fullInventoryPen, rect);
                }

                graphics.Save();
            }

            picBackpack.Image = bitmap;
        }

        private static Color IntToColor(int value)
        {
            int r = (value & 0xFF);
            int g = ((value >> 8) & 0xFF);
            int b = ((value >> 16) & 0xFF);

            return Color.FromArgb(r, g, b);
        }

        #endregion Inventory Rendering

        private void LogMessageCallback(LogLevel level, string message, Exception ex)
        {
            if (ex != null)
                message += Environment.NewLine + ex.ToString();
            Log(level, message);
        }

        private void frmOverseer_FormClosing(object sender, FormClosingEventArgs e)
        {
            running = false;
        }
    }
}

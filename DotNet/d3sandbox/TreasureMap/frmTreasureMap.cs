using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        private Graphics map;

        public frmTreasureMap()
        {
            InitializeComponent();

            // FIXME: Check for elevated privileges and relaunch if we don't have them
            map = picMap.CreateGraphics();
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

            float worldWidth = worldBox.Max.X - worldBox.Min.X;
            float worldHeight = worldBox.Max.Y - worldBox.Min.Y;

            int width = picMap.Width;
            int height = picMap.Height;

            Pen actorPen = new Pen(Color.Blue);
            Pen npcPen = new Pen(Color.Purple);
            Pen hostilePen = new Pen(Color.Red);
            Pen itemPen = new Pen(Color.Green);
            Pen goldPen = new Pen(Color.Gold);
            Pen heroPen = new Pen(Color.Fuchsia);

            try
            {
                map.Clear(Color.White);
                foreach (Actor actor in world.AllActors)
                {
                    AABB aabb = actor.BoundingBox;
                    if (aabb.Min.X == 0f && aabb.Min.Y == 0f)
                        continue;

                    int x1 = (int)(((aabb.Min.X - worldBox.Min.X) / worldWidth) * width);
                    int y1 = (int)(((aabb.Min.Y - worldBox.Min.Y) / worldHeight) * height);
                    int x2 = (int)(((aabb.Max.X - worldBox.Min.X) / worldWidth) * width);
                    int y2 = (int)(((aabb.Max.Y - worldBox.Min.Y) / worldHeight) * height);

                    int w = Math.Max(x2 - x1, 1);
                    int h = Math.Max(y2 - y1, 1);

                    Pen pen = actorPen;

                    if (actor.Team == TeamType.Hostile)
                        pen = hostilePen;
                    else if (actor.Category == ActorCategory.Item)
                        pen = itemPen;
                    else if (actor.Type == ActorName.WitchDoctor_Female)
                    {
                        pen = heroPen;
                        w *= 5;
                        h *= 5;
                    }
                    else if (actor.Team == TeamType.Ally || actor.Team == TeamType.Team)
                        pen = npcPen;

                    if (actor.Type == ActorName.GoldCoin ||
                        actor.Type == ActorName.GoldCoin_flippy ||
                        actor.Type == ActorName.GoldCoins ||
                        actor.Type == ActorName.GoldLarge ||
                        actor.Type == ActorName.GoldMedium ||
                        actor.Type == ActorName.GoldSmall ||
                        actor.Type == ActorName.PlacedGold)
                    {
                        pen = goldPen;
                    }

                    if (pen == actorPen)
                        continue;

                    map.DrawRectangle(pen, x1, y1, w, h);
                }
            }
            catch
            {
                // Handle the case of the form closing while this thread is still running
            }
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

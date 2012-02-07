using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Magic;
using libdiablo3.Process;

namespace libdiablo3.Api
{
    public class Diablo3Api : IDisposable
    {
        public const string PROCESS_NAME = "Diablo III";

        public event LogHandler OnLogMessage;

        public int ProcessID { get { return d3.ProcessId; } }
        public AI.AI AI { get { return ai; } }

        private BlackMagic d3;
        private MemoryReader memReader;
        private Injector injector;
        private AI.AI ai;
        private bool computedHash;

        public Diablo3Api()
        {
            Log.OnLogMessage += LogMessageCallback;

            d3 = new BlackMagic();
            d3.SetDebugPrivileges = false;
            memReader = new MemoryReader(d3);

            ai = new AI.AI(this);
        }

        public bool IsDiabloRunning()
        {
            return SProcess.GetProcessFromProcessName(PROCESS_NAME) != 0;
        }

        #region Init/Shutdown

        public bool Init()
        {
            // Open the Diablo III process
            int processID = SProcess.GetProcessFromProcessName(PROCESS_NAME);
            if (processID == 0)
            {
                Log.Error("Diablo III process not found");
                return false;
            }

            // Attempt to open the D3 process with read/write permission
            if (!d3.IsProcessOpen && !d3.Open(processID))
            {
                d3 = null;
                Log.Error("Failed to open Diablo III process " + processID);
                return false;
            }

            if (!computedHash)
            {
                // Compute the MD5 hash of the D3 executable to make sure we're working with the right version
                byte[] md5Bytes;
                using (FileStream exeStream = File.OpenRead(d3.GetModuleFilePath()))
                    md5Bytes = new MD5CryptoServiceProvider().ComputeHash(exeStream);
                string md5Hash = ProcessUtils.BytesToHexString(md5Bytes);
                if (md5Hash != Offsets.MD5_CLIENT)
                    throw new MemoryReadException("MD5 checksum failed: " + md5Hash, 0);

                computedHash = true;
            }

            // Initialize the memory reader, including important global pointer addresses
            if (!memReader.UpdatePointers())
                return false;

            // Install our detour into the DirectX9 EndScene() method
            try
            {
                injector = new Injector(d3, memReader.EndSceneAddr);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to inject custom code", ex);
                return false;
            }

            return true;
        }

        public void Shutdown()
        {
            try
            {
                if (injector != null)
                    injector.Dispose();
                injector = null;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to dispose the injector", ex);
            }

            try
            {
                if (d3 != null)
                    d3.Close();
                d3 = null;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to close the D3 process", ex);
            }
        }

        public void Dispose()
        {
            Shutdown();
        }

        #endregion Init/Shutdown

        public World InitWorld()
        {
            if (memReader == null)
            {
                Log.Error("InitWorld() called before Init()");
                return null;
            }

            World world = new World(memReader, injector);
            world.Update();
            return world;
        }

        public void UpdateWorld(World world)
        {
            if (memReader == null)
            {
                Log.Error("UpdateWorld() called before Init()");
                return;
            }

            // Update all of the pointers in case there was a login/logout
            if (!memReader.UpdatePointers())
            {
                Log.Warn("Failed to update pointers");
                return;
            }

            // Update the current world and everything in it
            world.Update();
        }

        private void LogMessageCallback(LogLevel level, string message, Exception ex)
        {
            LogHandler handler = OnLogMessage;
            if (handler != null)
                handler(level, message, ex);
        }
    }
}

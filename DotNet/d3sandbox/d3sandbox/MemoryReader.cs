using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Magic;

namespace d3sandbox
{
    /// <summary>
    /// Provides methods for reading from Diablo III memory
    /// </summary>
    public class MemoryReader
    {
        #region Win32 API Imports

        private const uint TH32CS_SNAPTHREAD = 0x00000004;
        private const uint THREAD_QUERY_INFORMATION = 0x40;

        [DllImport("kernel32.dll")]
        public static extern bool Thread32First(IntPtr hSnapshot, ref THREADENTRY32 lppe);
        [DllImport("kernel32.dll")]
        public static extern bool Thread32Next(IntPtr hSnapshot, ref THREADENTRY32 lppe);
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("ntdll.dll")]
        public static extern uint NtQueryInformationThread(IntPtr handle, uint infclass, ref THREAD_BASIC_INFORMATION info, uint length, IntPtr bytesread);
        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);
        [DllImport("kernel32.dll")]
        private static extern Int32 CloseHandle(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential)]
        public struct THREADENTRY32
        {
            public UInt32 dwSize;
            public UInt32 cntUsage;
            public UInt32 th32ThreadID;
            public UInt32 th32OwnerProcessID;
            public Int32 tpBasePri;
            public Int32 tpDeltaPri;
            public UInt32 dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct THREAD_BASIC_INFORMATION
        {
            public bool ExitStatus;
            public IntPtr TebBaseAddress;
            public uint processid;
            public uint threadid;
            public uint AffinityMask;
            public uint Priority;
            public uint BasePriority;
        }

        #endregion Win32 API Imports

        public uint EndSceneAddr;

        private BlackMagic d3;
        private uint pObjMgr;
        private uint pRActors;
        private uint pScenes;

        private uint pTLSObjMgr;
        private uint pACDs;
        private uint pAttributes;
        private uint pD3DDevice;

        private static Dictionary<uint, HeroClass> gbidHeroClasses;

        static MemoryReader()
        {
            // Initialize the GBID->HeroClass lookup table
            gbidHeroClasses = new Dictionary<uint, HeroClass>();
            gbidHeroClasses.Add(Utils.HashItemName("Barbarian"), HeroClass.Barbarian);
            gbidHeroClasses.Add(Utils.HashItemName("DemonHunter"), HeroClass.DemonHunter);
            gbidHeroClasses.Add(Utils.HashItemName("Monk"), HeroClass.Monk);
            gbidHeroClasses.Add(Utils.HashItemName("WitchDoctor"), HeroClass.WitchDoctor);
            gbidHeroClasses.Add(Utils.HashItemName("Wizard"), HeroClass.Wizard);
        }

        public MemoryReader(BlackMagic d3)
        {
            this.d3 = d3;

            // Fetch the global pointer to the object manager
            pObjMgr = d3.ReadUInt(Offsets.OBJMANAGER);

            // Fetch the pointer to the thread-local object manager
            pTLSObjMgr = GetTLSPointer();

            pRActors = GetRActorContainer();
            pScenes = GetScenesContainer();
            pACDs = GetACDContainer();
            pAttributes = GetAttributeContainer();
            pD3DDevice = GetDirect3DDevice();
            EndSceneAddr = d3.ReadUInt(d3.ReadUInt(pD3DDevice) + Offsets.D3D_DEVICE_ENDSCENE_OFFSET);
        }

        /// <summary>
        /// Fetch the current player
        /// </summary>
        public Player GetPlayer()
        {
            uint id = GetPlayerRActorID();
            if (id == Offsets.INVALID)
                return null;

            uint actorPtr = IDToPtr(pRActors, Offsets.SIZEOF_RACTOR, id);
            if (actorPtr == Offsets.INVALID)
                throw new InvalidHostException("Failed to resolve player actorID " + id + " to a pointer");
            RActor actor = new RActor(d3, actorPtr, d3.ReadBytes(actorPtr, Offsets.SIZEOF_RACTOR));

            uint acdPtr = IDToPtr(pACDs, Offsets.SIZEOF_ACD, actor.AcdID);
            if (acdPtr == Offsets.INVALID)
                throw new InvalidHostException("Failed to resolve player acdID " + actor.AcdID + " to a pointer");
            ActorCommonData acd = new ActorCommonData(acdPtr, d3.ReadBytes(acdPtr, Offsets.SIZEOF_ACD));
            acd.AttributesPtr = IDToPtr(pAttributes, 384, acd.AttributesID);

            Player player = new Player(actor, acd);
            player.Attributes = GetEntityAttributes(player);
            return player;
        }

        /// <summary>
        /// Fetch all scene entities
        /// </summary>
        public List<Entity> GetEntities()
        {
            // Grab the size of the RActors array
            int actorArraySize = d3.ReadInt(pRActors + 268);

            // Grab the first actor
            uint pRActor = d3.ReadUInt(d3.ReadUInt(pRActors + 328));

            // Loop through the array and grab all valid actor objects
            List<Entity> entities = new List<Entity>(actorArraySize);
            for (uint i = 0; i < actorArraySize; i++)
            {
                RActor actor = GetRActor(pRActor + i * Offsets.SIZEOF_RACTOR);
                if (actor != null)
                {
                    uint acdPtr = IDToPtr(pACDs, Offsets.SIZEOF_ACD, actor.AcdID);
                    if (acdPtr != Offsets.INVALID)
                    {
                        ActorCommonData acd = new ActorCommonData(acdPtr, d3.ReadBytes(acdPtr, Offsets.SIZEOF_ACD));
                        acd.AttributesPtr = IDToPtr(pAttributes, Offsets.SIZEOF_ATTRIBUTE, acd.AttributesID);

                        Entity entity = new Entity(actor, acd);
                        entity.Attributes = GetEntityAttributes(entity);

                        entities.Add(entity);
                    }
                }
            }

            return entities;
        }

        public List<Scene> GetScenes()
        {
            // Grab the size of the Scenes array
            int sceneArraySize = d3.ReadInt(pScenes + Offsets.ARRAY_SIZE_OFFSET);

            // Grab the first scene
            uint pScene = d3.ReadUInt(pScenes + Offsets.ARRAY_OFFSET);

            // Loop through the array and grab all valid scene objects
            List<Scene> scenes = new List<Scene>(sceneArraySize);
            for (uint i = 0; i < sceneArraySize; i++)
            {
                Scene scene = GetScene(pScene + i * Offsets.SIZEOF_SCENE);
                if (scene != null)
                    scenes.Add(scene);
            }

            return scenes;
        }

        public GameAttributeValue? GetAttribute(Entity entity, GameAttribute attrib)
        {
            uint ptr;

            uint attribID = 0xFFFFF000 | (uint)attrib.ID;

            /*ptr = d3.ReadUInt(d3.ReadUInt(attributes + 56) + 4 * (d3.ReadUInt(attributes + 200) & (attribID ^ (attribID >> 16))));
            if (ptr != 0)
            {
                while (d3.ReadUInt(ptr + 4) != attribID)
                {
                    ptr = d3.ReadUInt(ptr);
                    if (ptr == 0)
                        goto Lookup2;
                }
                //FIXME:
                //sub_846AE0(actorPtr, _attribID);
                throw new NotImplementedException();
            }
        Lookup2:*/
            uint v0 = d3.ReadUInt(entity._acd.AttributesPtr + 16);
            ptr = d3.ReadUInt(d3.ReadUInt(v0 + 8) + 4 * (d3.ReadUInt(v0 + 1048) & (attribID ^ (attribID >> 16))));
            if (ptr != 0)
            {
                while (d3.ReadUInt(ptr + 4) != attribID)
                {
                    ptr = d3.ReadUInt(ptr);
                    if (ptr == 0)
                        return null;
                }

                return ReadAttribute(ptr, attrib);
            }

            return null;
        }

        /// <summary>
        /// Fetch all attributes associated with a given entity
        /// </summary>
        /// <param name="entity">Entity to fetch attributes for</param>
        public Dictionary<GameAttribute, GameAttributeValue> GetEntityAttributes(Entity entity)
        {
            Dictionary<GameAttribute, GameAttributeValue> attributes = new Dictionary<GameAttribute, GameAttributeValue>();

            for (int i = 0; i < GameAttribute.Attributes.Length; i++)
            {
                GameAttribute attribute = GameAttribute.Attributes[i];
                GameAttributeValue? value = GetAttribute(entity, attribute);
                if (value.HasValue)
                    attributes[attribute] = value.Value;
            }

            return attributes;
        }

        private GameAttributeValue ReadAttribute(uint ptr, GameAttribute attrib)
        {
            if (attrib.IsInteger)
                return new GameAttributeValue(d3.ReadInt(ptr + 8));
            else
                return new GameAttributeValue(d3.ReadFloat(ptr + 8));
        }

        /// <summary>
        /// Fetch the RActor ID of the current player
        /// </summary>
        private uint GetPlayerRActorID()
        {
            uint v0 = d3.ReadUInt(d3.ReadUInt(pObjMgr + 2352));
            if (v0 == Offsets.INVALID)
                return Offsets.INVALID;

            uint v1 = d3.ReadUInt(pObjMgr + 1916 + 168);
            if (v1 == 0)
                return Offsets.INVALID;

            uint v2 = 30520 * v0 + v1 + 88;
            if (v2 == 0)
                return Offsets.INVALID;

            return d3.ReadUInt(v2 + 8);
        }

        /// <summary>
        /// Fetches the global pointer to the RActors container
        /// </summary>
        private uint GetRActorContainer()
        {
            uint pActors = d3.ReadUInt(pObjMgr + 0x8AC);
            if (d3.ReadASCIIString(pActors, 7) == "RActors")
                return pActors;
            return Offsets.INVALID;
        }

        /// <summary>
        /// Fetches the global pointer to the scenes container
        /// </summary>
        private uint GetScenesContainer()
        {
            uint pScenes = d3.ReadUInt(pObjMgr + 0x8F0);
            if (d3.ReadASCIIString(pScenes, 7) == "Scenes")
                return pScenes;
            return Offsets.INVALID;
        }

        /// <summary>
        /// Fetch 
        /// </summary>
        /// <returns></returns>
        private uint GetTLSPointer()
        {
            uint tlsPtr = Offsets.INVALID;

            IntPtr snapHandle = CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD, 0);
            if (snapHandle != null)
            {
                THREADENTRY32 info = new THREADENTRY32();
                info.dwSize = (uint)Marshal.SizeOf(typeof(THREADENTRY32));
                bool moreThreads = true;
                if (Thread32First(snapHandle, ref info))
                {
                    while (moreThreads)
                    {
                        if (info.th32OwnerProcessID == d3.ProcessId)
                        {
                            IntPtr threadHandle = OpenThread(THREAD_QUERY_INFORMATION, false, info.th32ThreadID);
                            if (threadHandle != null)
                            {
                                THREAD_BASIC_INFORMATION tbi = new THREAD_BASIC_INFORMATION();
                                if (NtQueryInformationThread(threadHandle, 0, ref tbi,
                                    (uint)Marshal.SizeOf(typeof(THREAD_BASIC_INFORMATION)), IntPtr.Zero) == 0)
                                {
                                    uint tlsOffset = (uint)tbi.TebBaseAddress.ToInt32() + Offsets.TLS_OFFSET;
                                    uint tlsIndex = d3.ReadUInt(Offsets.TLS_INDEX);
                                    tlsPtr = d3.ReadUInt(d3.ReadUInt(d3.ReadUInt(tlsOffset + (tlsIndex * 4))));
                                    
                                    CloseHandle(threadHandle);
                                    break;
                                }
                            }
                        }

                        info.dwSize = (uint)Marshal.SizeOf(typeof(THREADENTRY32));
                        moreThreads = Thread32Next(snapHandle, ref info);
                    }
                }

                CloseHandle(snapHandle);
            }

            return tlsPtr;
        }

        /// <summary>
        /// Fetch the pointer to the thread-local Actor Common Data container
        /// </summary>
        private uint GetACDContainer()
        {
            return d3.ReadUInt(d3.ReadUInt(pTLSObjMgr + 0xD4));
        }

        /// <summary>
        /// Fetch the pointer to the thread-local attribute container
        /// </summary>
        private uint GetAttributeContainer()
        {
            return d3.ReadUInt(d3.ReadUInt(pTLSObjMgr + 0xC8) + 0x70);
        }

        private uint GetDirect3DDevice()
        {
            return d3.ReadUInt(d3.ReadUInt(Offsets.D3DMANAGER_PTR) + Offsets.D3DMANAGER_DEVICE_OFFSET);
        }

        /// <summary>
        /// Read an RActor object
        /// </summary>
        /// <param name="ptr">Pointer to the RActor to read</param>
        private RActor GetRActor(uint ptr)
        {
            if (d3.ReadUInt(ptr) != Offsets.INVALID)
                return new RActor(d3, ptr, d3.ReadBytes(ptr, 1068));
            return null;
        }

        /// <summary>
        /// Read a scene object
        /// </summary>
        /// <param name="ptr">Pointer to the scene to read</param>
        private Scene GetScene(uint ptr)
        {
            if (d3.ReadUInt(ptr) != Offsets.INVALID)
                return new Scene(d3, ptr, d3.ReadBytes(ptr, Offsets.SIZEOF_SCENE));
            return null;
        }

        /// <summary>
        /// Converts an object manager ID to a pointer
        /// </summary>
        /// <param name="container">Object manager</param>
        /// <param name="objSize">Size of the stored objects</param>
        /// <param name="id">Object ID</param>
        /// <returns>A pointer to the requested object</returns>
        private uint IDToPtr(uint container, uint objSize, uint id)
        {
            uint shortID = id & 0xFFFF;

            if (shortID >= d3.ReadUInt(container + 256))
                return Offsets.INVALID;

            uint v0 = d3.ReadUInt(container + 328);
            int v1 = d3.ReadInt(container + 396);

            uint ptr = d3.ReadUInt(v0 + 4 * (shortID >> v1)) + objSize * (uint)(shortID & ((1 << v1) - 1));
            if (d3.ReadUInt(ptr) == id)
                return ptr;

            return Offsets.INVALID;
        }

        /// <summary>
        /// Performs a reverse lookup from a hashed game balance ID to the 
        /// original string, as an enum of hero classes
        /// </summary>
        /// <param name="gbid">Hero game balance ID</param>
        public static HeroClass GBIDToClass(uint gbid)
        {
            HeroClass heroClass;
            if (gbidHeroClasses.TryGetValue(gbid, out heroClass))
                return heroClass;
            return HeroClass.Unknown;
        }
    }
}

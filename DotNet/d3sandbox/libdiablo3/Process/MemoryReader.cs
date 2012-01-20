using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Magic;

namespace libdiablo3.Process
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

        public BlackMagic D3;
        public uint pObjMgr;
        public uint pRActors;
        public uint pScenes;
        public uint pUI;
        public uint pPlayer;

        public uint pTLSObjMgr;
        public uint pACDs;
        public uint pAttributes;
        public uint pD3DDevice;

        public MemoryReader(BlackMagic d3)
        {
            this.D3 = d3;
        }

        public bool IsGameRunning()
        {
            uint objMgr = D3.ReadUInt(Offsets.OBJMGR);
            uint actors = D3.ReadUInt(objMgr + Offsets.OBJMGR_ACTORS_OFFSET);
            return actors != 0;
        }

        public bool Init()
        {
            try
            {
                // Fetch the global pointer to the object manager
                pObjMgr = D3.ReadUInt(Offsets.OBJMGR);

                pRActors = GetActorContainer();
                if (pRActors == Offsets.INVALID)
                    return false;

                pScenes = GetScenesContainer();
                pUI = GetUIContainer();
                pPlayer = GetPlayerPtr();

                // Fetch the pointer to the thread-local object manager
                pTLSObjMgr = GetTLSPointer();

                pACDs = GetACDContainer();
                pAttributes = GetAttributeContainer();
                pD3DDevice = GetDirect3DDevice();

                EndSceneAddr = D3.ReadUInt(D3.ReadUInt(pD3DDevice) + Offsets.D3D_DEVICE_ENDSCENE_OFFSET);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Fetch the current player
        /// </summary>
        public D3Actor GetPlayer()
        {
            // Fetch the current player actorID
            uint actorID = D3.ReadUInt(pPlayer + Offsets.PLAYER_ACTORID_OFFSET);
            if (actorID == Offsets.INVALID)
                return null;

            // Resolve actorID to a pointer
            uint actorPtr = IDToPtr(pRActors, Offsets.SIZEOF_RACTOR, actorID);
            if (actorPtr == Offsets.INVALID)
                return null;

            return new D3Actor(this, actorPtr, D3.ReadBytes(actorPtr, Offsets.SIZEOF_RACTOR));
        }

        public uint[] GetActiveSkills()
        {
            byte[] data = D3.ReadBytes(pPlayer + Offsets.PLAYER_ACTIVESKILLS_OFFSET,
                Offsets.ACTIVE_SKILL_COUNT * Offsets.SIZEOF_ACTIVESKILL);

            uint[] skills = new uint[Offsets.ACTIVE_SKILL_COUNT];
            for (int i = 0; i < skills.Length; i++)
                skills[i] = BitConverter.ToUInt32(data, i * Offsets.SIZEOF_ACTIVESKILL);
            return skills;
        }

        /// <summary>
        /// Fetch all scene entities
        /// </summary>
        public List<D3Actor> GetActors()
        {
            // Grab the size of the RActors array
            int actorArraySize = D3.ReadInt(pRActors + Offsets.ARRAY_SIZE_OFFSET);

            // Grab the first actor
            uint pRActor = D3.ReadUInt(D3.ReadUInt(pRActors + Offsets.ARRAY_START_PTR_OFFSET));

            // Loop through the array and grab all valid actor objects
            List<D3Actor> actors = new List<D3Actor>(actorArraySize);
            for (uint i = 0; i < actorArraySize; i++)
            {
                D3Actor actor = GetActor(pRActor + i * Offsets.SIZEOF_RACTOR);
                if (actor != null)
                    actors.Add(actor);
            }

            return actors;
        }

        public List<D3Scene> GetScenes()
        {
            // Grab the size of the Scenes array
            int sceneArraySize = D3.ReadInt(pScenes + Offsets.ARRAY_SIZE_OFFSET);

            // Grab the first scene
            uint pScene = D3.ReadUInt(pScenes + Offsets.ARRAY_START_PTR_OFFSET);

            // Loop through the array and grab all valid scene objects
            List<D3Scene> scenes = new List<D3Scene>(sceneArraySize);
            for (uint i = 0; i < sceneArraySize; i++)
            {
                D3Scene scene = GetScene(pScene + i * Offsets.SIZEOF_SCENE);
                if (scene != null)
                    scenes.Add(scene);
            }

            return scenes;
        }

        public D3AttributeValue? GetAttribute(D3Actor actor, D3Attribute attrib)
        {
            if (actor.Acd == null)
                return null;

            uint ptr;
            uint attribID = Offsets.ATTRIBUTE_MASK | (uint)attrib.ID;
            uint v0 = D3.ReadUInt(actor.Acd.AttributesPtr + 16);

            ptr = D3.ReadUInt(D3.ReadUInt(v0 + 8) + 4 * (D3.ReadUInt(v0 + Offsets.ARRAY_SLOTCOUNT_OFFSET) & (attribID ^ (attribID >> 16))));
            if (ptr != 0)
            {
                while (D3.ReadUInt(ptr + 4) != attribID)
                {
                    ptr = D3.ReadUInt(ptr);
                    if (ptr == 0)
                        return null;
                }

                return ReadAttribute(ptr, attrib);
            }

            return null;
        }

        /// <summary>
        /// Fetch all attributes associated with a given actor
        /// </summary>
        /// <param name="actor">Actor to fetch attributes for</param>
        public Dictionary<D3Attribute, D3AttributeValue> GetActorAttributes(D3Actor actor)
        {
            Dictionary<D3Attribute, D3AttributeValue> attributes = new Dictionary<D3Attribute, D3AttributeValue>();
            if (actor.Acd == null)
                return attributes;

            uint v0 = D3.ReadUInt(actor.Acd.AttributesPtr + 16);
            uint capacity = D3.ReadUInt(v0 + Offsets.ARRAY_SLOTCOUNT_OFFSET);

            uint basePtr;
            uint ptr;
            for (uint i = 0; i < capacity; i++)
            {
                basePtr = D3.ReadUInt(D3.ReadUInt(v0 + 8) + 4 * i);
                if (basePtr != 0 && basePtr != Offsets.INVALID)
                {
                    ptr = basePtr;
                    while (ptr != 0)
                    {
                        uint attribID = D3.ReadUInt(ptr + 4);
                        if (attribID != Offsets.INVALID && (attribID & Offsets.ATTRIBUTE_MASK) == Offsets.ATTRIBUTE_MASK)
                        {
                            D3Attribute attrib = D3Attribute.AttributesMap[(int)(attribID & 0xFFF)];
                            attributes.Add(attrib, ReadAttribute(ptr, attrib));
                        }

                        ptr = D3.ReadUInt(ptr);
                    }
                }
            }

            return attributes;
        }

        public uint GetUIPtr(string uiHandle)
        {
            uint id = ProcessUtils.HashLowerCase(uiHandle);
            uint index = id & 0x7FF;
            uint uiPtrArray = D3.ReadUInt(D3.ReadUInt(pUI) + 8);
            uint lastAddr = D3.ReadUInt(uiPtrArray + (index * 4));

            while (true)
            {
                if (lastAddr == 0)
                    return Offsets.INVALID;

                uint nextAddr = D3.ReadUInt(D3.ReadUInt(lastAddr + Offsets.UI_1_OFFSET) + Offsets.UI_2_OFFSET);
                if (nextAddr == id)
                    return D3.ReadUInt(lastAddr + Offsets.UI_3_OFFSET);

                lastAddr = D3.ReadUInt(lastAddr);
            }
        }

        public string GetTextboxValue(string uiHandle)
        {
            uint ptr = GetUIPtr(uiHandle);
            if (ptr == Offsets.INVALID)
                return null;

            int length = D3.ReadInt(ptr + Offsets.UI_TEXTBOX_LENGTH);
            return D3.ReadASCIIString(D3.ReadUInt(ptr + Offsets.UI_TEXTBOX_STR), length);
        }

        public D3AttributeValue ReadAttribute(uint ptr, D3Attribute attrib)
        {
            if (attrib.IsInteger)
                return new D3AttributeValue(D3.ReadInt(ptr + 8));
            else
                return new D3AttributeValue(D3.ReadFloat(ptr + 8));
        }

        public uint GetPlayerPtr()
        {
            uint count = D3.ReadUInt(D3.ReadUInt(pObjMgr + Offsets.OBJMGR_PLAYER_OFFSET));
            if (count == Offsets.INVALID)
                return Offsets.INVALID;

            uint v1 = D3.ReadUInt(pObjMgr + Offsets.PLAYER_OFFSET1 + Offsets.PLAYER_OFFSET2);
            if (v1 == 0)
                return Offsets.INVALID;

            uint v2 = Offsets.SIZEOF_PLAYER * count + v1 + Offsets.PLAYER_OFFSET3;
            if (v2 == 0)
                return Offsets.INVALID;

            return v2;
        }

        /// <summary>
        /// Fetches the global pointer to the RActors container
        /// </summary>
        public uint GetActorContainer()
        {
            uint pActors = D3.ReadUInt(pObjMgr + Offsets.OBJMGR_ACTORS_OFFSET);
            if (pActors == 0)
                return Offsets.INVALID;

            if (D3.ReadASCIIString(pActors, 7) == "RActors")
                return pActors;
            return Offsets.INVALID;
        }

        /// <summary>
        /// Fetches the global pointer to the scenes container
        /// </summary>
        public uint GetScenesContainer()
        {
            uint pScenes = D3.ReadUInt(pObjMgr + Offsets.OBJMGR_SCENES_OFFSET);
            if (pScenes == 0)
                return Offsets.INVALID;

            if (D3.ReadASCIIString(pScenes, 6) == "Scenes")
                return pScenes;
            return Offsets.INVALID;
        }

        /// <summary>
        /// Fetches the global pointer to the UI object container
        /// </summary>
        public uint GetUIContainer()
        {
            uint pUI = D3.ReadUInt(pObjMgr + Offsets.OBJMGR_UI_OFFSET);
            return pUI;
        }

        /// <summary>
        /// Fetch 
        /// </summary>
        /// <returns></returns>
        public uint GetTLSPointer()
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
                        if (info.th32OwnerProcessID == D3.ProcessId)
                        {
                            IntPtr threadHandle = OpenThread(THREAD_QUERY_INFORMATION, false, info.th32ThreadID);
                            if (threadHandle != null)
                            {
                                THREAD_BASIC_INFORMATION tbi = new THREAD_BASIC_INFORMATION();
                                if (NtQueryInformationThread(threadHandle, 0, ref tbi,
                                    (uint)Marshal.SizeOf(typeof(THREAD_BASIC_INFORMATION)), IntPtr.Zero) == 0)
                                {
                                    uint tlsOffset = (uint)tbi.TebBaseAddress.ToInt32() + Offsets.TLS_OFFSET;
                                    uint tlsIndex = D3.ReadUInt(Offsets.TLS_INDEX);
                                    tlsPtr = D3.ReadUInt(D3.ReadUInt(D3.ReadUInt(tlsOffset + (tlsIndex * 4))));
                                    
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
        /// Fetch the pointer to the thread-local attribute container
        /// </summary>
        public uint GetAttributeContainer()
        {
            return D3.ReadUInt(D3.ReadUInt(pTLSObjMgr + Offsets.TLS_ATTRIBUTES_OFFSET1) +
                Offsets.TLS_ATTRIBUTES_OFFSET2);
        }

        /// <summary>
        /// Fetch the pointer to the thread-local Actor Common Data container
        /// </summary>
        public uint GetACDContainer()
        {
            return D3.ReadUInt(D3.ReadUInt(pTLSObjMgr + Offsets.TLS_ACD_OFFSET));
        }

        public uint GetDirect3DDevice()
        {
            return D3.ReadUInt(D3.ReadUInt(Offsets.D3DMANAGER_PTR) + Offsets.D3DMANAGER_DEVICE_OFFSET);
        }

        /// <summary>
        /// Read an RActor object
        /// </summary>
        /// <param name="ptr">Pointer to the RActor to read</param>
        public D3Actor GetActor(uint ptr)
        {
            if (D3.ReadUInt(ptr) != Offsets.INVALID)
                return new D3Actor(this, ptr, D3.ReadBytes(ptr, Offsets.SIZEOF_RACTOR));
            return null;
        }

        /// <summary>
        /// Read a scene object
        /// </summary>
        /// <param name="ptr">Pointer to the scene to read</param>
        public D3Scene GetScene(uint ptr)
        {
            if (D3.ReadUInt(ptr) != Offsets.INVALID)
                return new D3Scene(this, ptr, D3.ReadBytes(ptr, Offsets.SIZEOF_SCENE));
            return null;
        }

        /// <summary>
        /// Converts an object manager ID to a pointer
        /// </summary>
        /// <param name="container">Object manager</param>
        /// <param name="objSize">Size of the stored objects</param>
        /// <param name="id">Object ID</param>
        /// <returns>A pointer to the requested object</returns>
        public uint IDToPtr(uint container, uint objSize, uint id)
        {
            uint shortID = id & 0xFFFF;

            if (shortID >= D3.ReadUInt(container + Offsets.ARRAY_CAPACITY_OFFSET))
                return Offsets.INVALID;

            uint v0 = D3.ReadUInt(container + Offsets.ARRAY_START_PTR_OFFSET);
            int v1 = D3.ReadInt(container + Offsets.ARRAY_HASHING_OFFSET);

            uint ptr = D3.ReadUInt(v0 + 4 * (shortID >> v1)) + objSize * (uint)(shortID & ((1 << v1) - 1));
            if (D3.ReadUInt(ptr) == id)
                return ptr;

            return Offsets.INVALID;
        }
    }
}

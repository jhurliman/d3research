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

        internal IntPtr smallBuffer;
        internal IntPtr attribBuffer;
        internal IntPtr acdBuffer;

        public MemoryReader(BlackMagic d3)
        {
            this.D3 = d3;

            smallBuffer = Marshal.AllocHGlobal(4);
            attribBuffer = Marshal.AllocHGlobal(12);
            acdBuffer = Marshal.AllocHGlobal(Offsets.SIZEOF_ACD);
        }

        ~MemoryReader()
        {
            Marshal.FreeHGlobal(smallBuffer);
            Marshal.FreeHGlobal(attribBuffer);
            Marshal.FreeHGlobal(acdBuffer);
        }

        public bool IsGameRunning()
        {
            uint objMgr = ReadUInt(Offsets.OBJMGR);
            uint actors = ReadUInt(objMgr + Offsets.OBJMGR_ACTORS_OFFSET);
            return actors != 0;
        }

        public bool UpdatePointers()
        {
            try
            {
                // Fetch the global pointer to the object manager
                pObjMgr = ReadUInt(Offsets.OBJMGR);

                pRActors = GetActorContainer();
                if (pRActors == Offsets.INVALID)
                    return false;

                pScenes = GetScenesContainer();
                pUI = GetUIContainer();
                pPlayer = GetPlayerPtr();

                // Fetch the pointer to the thread-local object manager, if we
                // haven't yet
                if (pTLSObjMgr == 0)
                    pTLSObjMgr = GetTLSPointer();

                pACDs = GetACDContainer();
                pAttributes = GetAttributeContainer();
                pD3DDevice = GetDirect3DDevice();

                EndSceneAddr = ReadUInt(ReadUInt(pD3DDevice) + Offsets.D3D_DEVICE_ENDSCENE_OFFSET);
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
            uint actorID = ReadUInt(pPlayer + Offsets.PLAYER_ACTORID_OFFSET);
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
        public Dictionary<int, D3Actor> GetActors()
        {
            // Grab the size of the RActors array
            int arraySize = ReadInt(pRActors + Offsets.ARRAY_SIZE_OFFSET);
            if (arraySize == 0)
                return new Dictionary<int,D3Actor>(0);

            uint v0 = ReadUInt(pRActors + Offsets.ARRAY_START_PTR_OFFSET);
            int v1 = ReadInt(pRActors + Offsets.ARRAY_HASHING_OFFSET);

            Dictionary<int, D3Actor> actors = new Dictionary<int,D3Actor>(arraySize);
            for (uint i = 0; i < arraySize; i++)
            {
                uint ptr = Offsets.INVALID;
                while (true)
                {
                    ptr = ReadUInt(v0 + 4 * (i >> v1)) + Offsets.SIZEOF_RACTOR * (uint)(i & ((1 << v1) - 1));
                    if (ReadUInt(ptr) != Offsets.INVALID)
                        break;
                }

                D3Actor actor = GetActor(ptr);
                if (actor != null)
                    actors.Add(actor.ActorID, actor);
            }

            return actors;
        }

        public Dictionary<int, D3ActorCommonData> GetACDs()
        {
            // Grab the size of the ActorCommonData array
            int arraySize = ReadInt(pACDs + Offsets.ARRAY_SIZE_OFFSET);
            if (arraySize == 0)
                return new Dictionary<int, D3ActorCommonData>(0);

            uint v0 = ReadUInt(pACDs + Offsets.ARRAY_START_PTR_OFFSET);
            int v1 = ReadInt(pACDs + Offsets.ARRAY_HASHING_OFFSET);

            Dictionary<int, D3ActorCommonData> acds = new Dictionary<int, D3ActorCommonData>(arraySize);
            for (uint i = 0; i < arraySize; i++)
            {
                uint ptr = Offsets.INVALID;
                while (true)
                {
                    ptr = ReadUInt(v0 + 4 * (i >> v1)) + Offsets.SIZEOF_ACD * (uint)(i & ((1 << v1) - 1));
                    if (ReadUInt(ptr) != Offsets.INVALID)
                        break;
                }

                D3ActorCommonData acd = GetACD(ptr);
                if (acd != null)
                    acds.Add(acd.AcdID, acd);
            }

            return acds;
        }

        public List<D3Scene> GetScenes()
        {
            // Grab the size of the Scenes array
            int sceneArraySize = ReadInt(pScenes + Offsets.ARRAY_SIZE_OFFSET);

            // Grab the first scene
            uint pScene = ReadUInt(pScenes + Offsets.ARRAY_START_PTR_OFFSET);

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

        public D3AttributeValue? GetAttribute(uint attributesPtr, D3Attribute attrib)
        {
            uint ptr;
            uint attribID = Offsets.ATTRIBUTE_MASK | (uint)attrib.ID;
            uint v0 = ReadUInt(attributesPtr + 16);

            ptr = ReadUInt(ReadUInt(v0 + 8) + 4 * (ReadUInt(v0 + Offsets.ATTRIB_SLOTCOUNT_OFFSET) & (attribID ^ (attribID >> 16))));
            if (ptr != 0)
            {
                while (ReadUInt(ptr + 4) != attribID)
                {
                    ptr = ReadUInt(ptr);
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
        /// <param name="attributesPtr">Pointer to an attributes container</param>
        public D3AttributeMap GetAttributes(uint attributesPtr)
        {
            D3AttributeMap attributes = new D3AttributeMap();
            
            uint v0 = ReadUInt(attributesPtr + 16);
            uint p0 = ReadUInt(v0 + 8);
            uint capacity = ReadUInt(v0 + Offsets.ATTRIB_SLOTCOUNT_OFFSET);

            uint basePtr;
            uint ptr;
            byte[] data = D3.ReadBytes(p0, 4 * (int)capacity);

            unsafe
            {
                fixed (byte* bytePtr = data)
                {
                    uint* ptrs = (uint*)bytePtr;

                    for (int i = 0; i < capacity; i++)
                    {
                        basePtr = *(ptrs + i);
                        if (basePtr != 0 && basePtr != Offsets.INVALID)
                        {
                            ptr = basePtr;
                            while (ptr != 0)
                            {
                                byte[] attribData = D3.ReadBytes(ptr, 12, attribBuffer);
                                ptr = BitConverter.ToUInt32(attribData, 0);
                                uint attribID = BitConverter.ToUInt32(attribData, 4);

                                if (attribID != Offsets.INVALID)
                                {
                                    D3Attribute attrib = D3Attribute.AttributesMap[(int)(attribID & 0xFFF)];
                                    if (attrib.IsInteger)
                                        attributes[attribID] = new D3AttributeValue(BitConverter.ToInt32(attribData, 8));
                                    else
                                        attributes[attribID] = new D3AttributeValue(BitConverter.ToSingle(attribData, 8));
                                }
                            }
                        }
                    }
                }
            }

            return attributes;
        }

        public uint GetUIPtr(string uiHandle)
        {
            uint id = ProcessUtils.HashLowerCase(uiHandle);
            uint index = id & 0x7FF;
            uint uiPtrArray = ReadUInt(ReadUInt(pUI) + 8);
            uint lastAddr = ReadUInt(uiPtrArray + (index * 4));

            while (true)
            {
                if (lastAddr == 0)
                    return Offsets.INVALID;

                uint nextAddr = ReadUInt(ReadUInt(lastAddr + Offsets.UI_1_OFFSET) + Offsets.UI_2_OFFSET);
                if (nextAddr == id)
                    return ReadUInt(lastAddr + Offsets.UI_3_OFFSET);

                lastAddr = ReadUInt(lastAddr);
            }
        }

        public string GetTextboxValue(string uiHandle)
        {
            uint ptr = GetUIPtr(uiHandle);
            if (ptr == Offsets.INVALID)
                return null;

            int length = ReadInt(ptr + Offsets.UI_TEXTBOX_LENGTH);
            return D3.ReadASCIIString(ReadUInt(ptr + Offsets.UI_TEXTBOX_STR), length);
        }

        public D3AttributeValue ReadAttribute(uint ptr, D3Attribute attrib)
        {
            if (attrib.IsInteger)
                return new D3AttributeValue(ReadInt(ptr + 8));
            else
                return new D3AttributeValue(ReadFloat(ptr + 8));
        }

        public uint GetPlayerPtr()
        {
            uint count = ReadUInt(ReadUInt(pObjMgr + Offsets.OBJMGR_PLAYER_OFFSET));
            if (count == Offsets.INVALID)
                return Offsets.INVALID;

            uint v1 = ReadUInt(pObjMgr + Offsets.PLAYER_OFFSET1 + Offsets.PLAYER_OFFSET2);
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
            uint pActors = ReadUInt(pObjMgr + Offsets.OBJMGR_ACTORS_OFFSET);
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
            uint pScenes = ReadUInt(pObjMgr + Offsets.OBJMGR_SCENES_OFFSET);
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
            uint pUI = ReadUInt(pObjMgr + Offsets.OBJMGR_UI_OFFSET);
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
                                    uint tlsIndex = ReadUInt(Offsets.TLS_INDEX);
                                    tlsPtr = ReadUInt(ReadUInt(ReadUInt(tlsOffset + (tlsIndex * 4))));
                                    
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
            return ReadUInt(ReadUInt(pTLSObjMgr + Offsets.TLS_ATTRIBUTES_OFFSET1) +
                Offsets.TLS_ATTRIBUTES_OFFSET2);
        }

        /// <summary>
        /// Fetch the pointer to the thread-local Actor Common Data container
        /// </summary>
        public uint GetACDContainer()
        {
            return ReadUInt(ReadUInt(pTLSObjMgr + Offsets.TLS_ACD_OFFSET));
        }

        public uint GetDirect3DDevice()
        {
            return ReadUInt(ReadUInt(Offsets.D3DMANAGER_PTR) + Offsets.D3DMANAGER_DEVICE_OFFSET);
        }

        /// <summary>
        /// Read an RActor object
        /// </summary>
        /// <param name="ptr">Pointer to the RActor to read</param>
        public D3Actor GetActor(uint ptr)
        {
            uint id = ReadUInt(ptr);
            if (id != Offsets.INVALID && id != 0)
                return new D3Actor(this, ptr, D3.ReadBytes(ptr, Offsets.SIZEOF_RACTOR));
            return null;
        }

        /// <summary>
        /// Read an ActorCommonData object
        /// </summary>
        /// <param name="ptr">Pointer to the ActorCommonData to read</param>
        public D3ActorCommonData GetACD(uint ptr)
        {
            uint id = ReadUInt(ptr);
            if (id != Offsets.INVALID && id != 0)
                return new D3ActorCommonData(this, ptr, D3.ReadBytes(ptr, Offsets.SIZEOF_ACD, acdBuffer));
            return null;
        }

        /// <summary>
        /// Read a scene object
        /// </summary>
        /// <param name="ptr">Pointer to the scene to read</param>
        public D3Scene GetScene(uint ptr)
        {
            uint id = ReadUInt(ptr);
            if (id != Offsets.INVALID && id != 0)
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

            if (shortID >= ReadUInt(container + Offsets.ARRAY_CAPACITY_OFFSET))
                return Offsets.INVALID;

            uint v0 = ReadUInt(container + Offsets.ARRAY_START_PTR_OFFSET);
            int v1 = ReadInt(container + Offsets.ARRAY_HASHING_OFFSET);

            uint ptr = ReadUInt(v0 + 4 * (shortID >> v1)) + objSize * (uint)(shortID & ((1 << v1) - 1));
            if (ReadUInt(ptr) == id)
                return ptr;

            return Offsets.INVALID;
        }

        public uint IndexToPtr(uint container, uint objSize, uint index)
        {
            uint v0 = ReadUInt(pACDs + Offsets.ARRAY_START_PTR_OFFSET);
            int v1 = ReadInt(pACDs + Offsets.ARRAY_HASHING_OFFSET);

            uint ptr = Offsets.INVALID;
            while (true)
            {
                ptr = ReadUInt(v0 + 4 * (index >> v1)) + Offsets.SIZEOF_ACD * (uint)(index & ((1 << v1) - 1));
                if (ReadUInt(ptr) != Offsets.INVALID)
                    break;
            }

            return ptr;
        }

        public uint ReadUInt(uint dwAddress)
        {
            return D3.ReadUInt(dwAddress, false, smallBuffer);
        }

        public int ReadInt(uint dwAddress)
        {
            return D3.ReadInt(dwAddress, false, smallBuffer);
        }

        public float ReadFloat(uint dwAddress)
        {
            return D3.ReadFloat(dwAddress, false, smallBuffer);
        }
    }
}

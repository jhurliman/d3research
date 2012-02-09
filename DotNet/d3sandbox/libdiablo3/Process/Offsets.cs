using System;

namespace libdiablo3.Process
{
    public static class Offsets
    {
        public const int VERSION = 8392;
        public const string MD5_CLIENT = "2E9FB55374285126E08AC85AB3F77D42";

        public const uint INVALID = 0xFFFFFFFF;

        public const uint GOLD_GBID = 0x07869277;

        public const uint ARRAY_CAPACITY_OFFSET = 0x100;
        public const uint ARRAY_SIZE_OFFSET = 0x10C;
        public const uint ARRAY_START_PTR_OFFSET = 0x148;
        public const uint ARRAY_HASHING_OFFSET = 0x18C;

        public const uint ATTRIB_SLOTCOUNT_OFFSET = 0x418;
        public const uint ATTRIB_COUNT_OFFSET = 0x41C;

        public const int SIZEOF_RACTOR = 1068;
        public const int SIZEOF_ACD = 720;
        public const int SIZEOF_SCENE = 680;
        public const int SIZEOF_ATTRIBUTE = 384;
        public const int SIZEOF_MOVEMENTINFO = 116;
        public const int SIZEOF_ACTIVESKILL = 8;
        public const int SIZEOF_PLAYER = 30520;

        public const int ACTIVE_SKILL_COUNT = 9;

        public const uint ATTRIBUTE_MASK = 0xFFFFF000;

        public const uint PLAYER_ACTORID_OFFSET = 8;
        public const uint PLAYER_ACTIVESKILLS_OFFSET = 0xBC;

        public const uint PLAYER_OFFSET1 = 0x77C;
        public const uint PLAYER_OFFSET2 = 0xA8;
        public const uint PLAYER_OFFSET3 = 0x58;

        public const uint METHOD_USEPOWER = 0x009426A0;

        /// <summary>Global pointer to the object manager, an object pointing
        /// to many important containers</summary>
        /// <remarks>"mov eax, dword_*" instruction near the beginning of usePower()</remarks>
        public const uint OBJMGR = 0x0143BE24;
        public const uint OBJMGR_ACTORS_OFFSET = 0x8B0;
        public const uint OBJMGR_SCENES_OFFSET = 0x8F4;
        public const uint OBJMGR_UI_OFFSET = 0x924;
        public const uint OBJMGR_PLAYER_OFFSET = 0x934;

        /// <summary>Global variable holding the thread-local storage index</summary>
        /// <remarks>"mov eax, dwTlsIndex" instruction at the top of getNumACDs()</remarks>
        public const uint TLS_INDEX = 0x01425F50;
        public const uint TLS_OFFSET = 0xE10;
        public const uint TLS_ATTRIBUTES_OFFSET1 = 0xC8;
        public const uint TLS_ATTRIBUTES_OFFSET2 = 0x70;
        public const uint TLS_ACD_OFFSET = 0xD4;

        public const uint UI_1_OFFSET = 0x20C;
        public const uint UI_2_OFFSET = 0x20;
        public const uint UI_3_OFFSET = 0x20C;
        public const uint UI_TEXTBOX_STR = 0xA28;
        public const uint UI_TEXTBOX_LENGTH = 0xA38;

        /// <summary>Global pointer to the Window object, which holds Direct3D
        /// object pointers and other important information</summary>
        /// <remarks>"mov dword_*, edi" instruction near the end of disposeGraphics()</remarks>
        public const uint D3DMANAGER_PTR = 0x014E1678;
        public const uint D3DMANAGER_ADAPTER_OFFSET = 0x4A4;
        public const uint D3DMANAGER_DEVICE_OFFSET = 0x4A8;
        public const uint D3D_DEVICE_ENDSCENE_OFFSET = 0xA8;
    }
}

using System;

namespace libdiablo3.Process
{
    public static class Offsets
    {
        public const int VERSION = 8101;
        public const string MD5_CLIENT = "CB3BFA433C8DCC0A6B966BE3A9035BF8";

        public const uint INVALID = 0xFFFFFFFF;

        public const uint ARRAY_CAPACITY_OFFSET = 0x100;
        public const uint ARRAY_SIZE_OFFSET = 0x10C;
        public const uint ARRAY_START_PTR_OFFSET = 0x148;
        public const uint ARRAY_HASHING_OFFSET = 0x18C;
        public const uint ARRAY_SLOTCOUNT_OFFSET = 0x418;

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

        public const uint METHOD_MESSAGEBOX = 0x00E34430;
        public const uint METHOD_USEPOWER = 0x00941640;

        public const uint OBJMGR = 0x140593C;
        public const uint OBJMGR_ACTORS_OFFSET = 0x8AC;
        public const uint OBJMGR_SCENES_OFFSET = 0x8F0;
        public const uint OBJMGR_UI_OFFSET = 0x920;
        public const uint OBJMGR_PLAYER_OFFSET = 0x930;

        public const uint TLS_INDEX = 0x013EFD70;
        public const uint TLS_OFFSET = 0xE10;
        public const uint TLS_ATTRIBUTES_OFFSET1 = 0xC8;
        public const uint TLS_ATTRIBUTES_OFFSET2 = 0x70;
        public const uint TLS_ACD_OFFSET = 0xD4;

        public const uint UI_1_OFFSET = 0x20C;
        public const uint UI_2_OFFSET = 0x20;
        public const uint UI_3_OFFSET = 0x20C;
        public const uint UI_TEXTBOX_STR = 0xA28;
        public const uint UI_TEXTBOX_LENGTH = 0xA38;

        public const uint D3DMANAGER_PTR = 0x014AA8F8;
        public const uint D3DMANAGER_ADAPTER_OFFSET = 0x4A4;
        public const uint D3DMANAGER_DEVICE_OFFSET = 0x4A8;
        public const uint D3D_DEVICE_ENDSCENE_OFFSET = 0xA8;
    }
}

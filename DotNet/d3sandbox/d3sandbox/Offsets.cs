using System;

namespace d3sandbox
{
    public static class Offsets
    {
        public const uint INVALID = 0xFFFFFFFF;

        public const uint ARRAY_SIZE_OFFSET = 0x10C;
        public const uint ARRAY_OFFSET = 0x148;

        public const int SIZEOF_RACTOR = 1068;
        public const int SIZEOF_ACD = 720;
        public const int SIZEOF_SCENE = 680;
        public const int SIZEOF_ATTRIBUTE = 384;

        public const uint OBJMANAGER = 0x140593C;

        public const uint TLS_INDEX = 0x013EFD70;
        public const uint TLS_OFFSET = 0xE10;

        public const uint METHOD_MESSAGEBOX = 0x00E34430;

        public const uint D3DMANAGER_PTR = 0x014AA8F8;
        public const uint D3DMANAGER_ADAPTER_OFFSET = 0x4A4;
        public const uint D3DMANAGER_DEVICE_OFFSET = 0x4A8;
        public const uint D3D_DEVICE_ENDSCENE_OFFSET = 0xA8;
    }
}

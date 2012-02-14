using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdiablo3.Process
{
    public enum UIVTableOffset : int
    {
        None = -1,
        Label = 0x0,
        Panel = 0x2eab8,
        Button = 0x335f8,
        ListBox = 0x462b0,
        HotkeyBind = 0x47440,
        TabButton = 0x33b70,
        ItemButton = 0x43ff8,
        ScrollBar = 0x45918,
        ScrollBarThumb = 0x47990,
        CheckBox = 0x460d8,
        ComboBox = 0x2ec80,
        RadioButtonGroup = 0x33c08,
        EditBox = 0x30290,
        Hotkey = 0x476f0,
        Transition = 0x320a0,
        Model = 0x46960,
        List = 0x46178,
        ImageList = 0x472f0,
        DragPanel = 0x30568,
        StackPanel = 0x46560,
        Blinker = 0x464d8,
        ProgressBar = 0x46438,
        Tooltip = 0x2f78,
        ConsoleOutput = 0x41758,
        TitleBar = 0x45ca0,
        DrawHook = 0x478e8,
        Video = 0x472b0,
        ChatMessageList = 0x47948,
        ChatEditBox = 0x30320,
        ConsoleEditBox = 0x303b0,
        Tutorial = 0x45da0,
        Timer = 0x474d8,
        ItemTag = 0x33690,
        ListBoxButton = 0x18508,
        SkillIcon = 0x16ed0,
        ListBoxFolder = 0x16f78,
        FloatBubble = 0x18658,
        DialogTree = 0x17000,
        Map = 0x33e8,
        DialogText = 0x16950,
    }

    public class D3UIObject
    {
        public readonly uint Pointer;
        public readonly uint VTable;
        public readonly int ID;
        public readonly string Name;
        public readonly int ParentID;
        public readonly bool Visible;

        public D3UIObject(MemoryReader memReader, uint ptr, byte[] data)
        {
            Pointer = ptr;
            VTable = BitConverter.ToUInt32(data, 0);
            Visible = BitConverter.ToBoolean(data, 0x20);
            ID = BitConverter.ToInt32(data, 0x28);
            Name = ProcessUtils.AsciiBytesToString(data, 0x30, 0x200);
            ParentID = BitConverter.ToInt32(data, 0x230);
            //ParentName = ProcessUtils.AsciiBytesToString(data, 0x238, 0x200);
        }

        public override string ToString()
        {
            return String.Format("0x{0} - {1}", VTable.ToString("X"), Name);
        }
    }
}

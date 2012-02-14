using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libdiablo3.Process;

namespace libdiablo3.Api
{
    public enum UIType : uint
    {
        None = 0,
        Blinker,
        Button,
        ChatEditBox,
        ChatMessageList,
        CheckBox,
        ComboBox,
        ConsoleEditBox,
        ConsoleOutput,
        DialogText,
        DialogTree,
        DragPanel,
        DrawHook,
        EditBox,
        FloatBubble,
        Globe,
        Hotkey,
        HotkeyBind,
        ImageList,
        ItemButton,
        ItemTag,
        Label,
        List,
        ListBox,
        ListBoxButton,
        ListBoxFolder,
        Map,
        Model,
        Panel,
        ProgressBar,
        RadioButtonGroup,
        Recipe,
        ScrollBar,
        ScrollBarThumb,
        SkillIcon,
        StackPanel,
        TabButton,
        Timer,
        TitleBar,
        Tooltip,
        Transition,
        Tutorial,
        Video,
    }

    public class UIObject
    {
        private readonly MemoryReader memReader;

        internal bool visible;
        internal UIObject parent;
        internal List<UIObject> children;

        public readonly uint Pointer;
        public readonly int ID;
        public readonly string Name;
        public readonly UIType Type;

        public UIObject Parent { get { return parent; } }
        public List<UIObject> Children { get { return children; } }

        public bool Visible
        {
            get
            {
                if (!this.visible)
                    return false;

                UIObject parent = this.parent;
                while (parent != null)
                {
                    if (!parent.visible)
                        return false;
                    parent = parent.parent;
                }

                return true;
            }
        }

        public UIObject(MemoryReader memReader, uint ptr, int id, string name, uint vtable, bool visible)
        {
            this.memReader = memReader;
            Pointer = ptr;
            ID = id;
            Name = name;
            this.visible = visible;

            if (!Offsets.UI_VTABLES.TryGetValue(vtable, out Type))
                Type = (UIType)vtable;

            children = new List<UIObject>();
        }

        public string GetText()
        {
            return memReader.GetUITextValue(this.Name);
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", Type, Name);
        }
    }
}

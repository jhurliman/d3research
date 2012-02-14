using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Magic;
using Fasm;

namespace libdiablo3.Process
{
    public class Injector : IDisposable
    {
        private BlackMagic d3;
        private uint oEndScene;
        private byte[] origEndSceneBytes = new byte[] { 0x8B, 0xFF, 0x55, 0x8B, 0xEC };
        private Dictionary<string, Tuple<uint, int>> allocatedMemory = new Dictionary<string, Tuple<uint, int>>();
        private Random rng = new Random();

        public Injector(BlackMagic d3, uint oEndScene)
        {
            this.d3 = d3;
            this.oEndScene = oEndScene;

            InstallHook();
        }

        public void Dispose()
        {
            RemoveHook();
            foreach (KeyValuePair<string, Tuple<uint, int>> kvp in allocatedMemory)
                d3.FreeMemory(kvp.Value.Item1);
        }

        public void UsePower(uint actorPtr, uint acdPtr, D3PowerInfo power)
        {
            uint flagAddress = GetAddress("UsePower_Flag");

            d3.WriteObject(GetAddress("UsePower_PowerInfo"), power, typeof(D3PowerInfo));
            d3.WriteUInt(GetAddress("UsePower_ActorPtr"), actorPtr);
            d3.WriteUInt(GetAddress("UsePower_AcdPtr"), acdPtr);
            d3.WriteInt(flagAddress, 1);

            while (d3.ReadInt(flagAddress) == 1)
                Thread.Sleep(1);
        }

        public void PressButton(uint buttonPtr)
        {
            uint flagAddress = GetAddress("PressButton_Flag");

            d3.WriteUInt(GetAddress("PressButton_Ptr"), buttonPtr);
            d3.WriteInt(flagAddress, 1);

            while (d3.ReadInt(flagAddress) == 1)
                Thread.Sleep(1);
        }

        private void InstallHook()
        {
            #region Inject New Method

            ManagedFasm fasm = new ManagedFasm(d3.ProcessHandle);
            fasm.SetMemorySize(0x4096); // Allocate 16KB of memory

            string[] endSceneHookASM = new string[]
            {
                // Original EndScene instructions
                "mov edi, edi",
                "push ebp",
                "mov ebp, esp",

                // Save the state of the FPU and general purpose registers
                "pushfd",
                "pushad",

                "call " + InstallUsePower(),
                "call " + InstallPressButton(),

                // Restore FPU and general purpose registers
                "@out:",
                "popad",
                "popfd",
                // Return to right after where we detoured
                "jmp " + (oEndScene + 5),
            };

            foreach (string line in RandomizeAsm(endSceneHookASM))
                fasm.AddLine(line);

            byte[] compiled = fasm.Assemble();
            uint hookCode = AllocateMemory("EndSceneHook", compiled.Length);
            fasm.Inject(hookCode);
            fasm.Clear();

            #endregion Inject New Method

            #region Detour EndScene

            origEndSceneBytes = d3.ReadBytes(oEndScene, 5);

            fasm.AddLine("jmp 0x{0:X08}", hookCode);
            fasm.Inject(oEndScene);
            fasm.Clear();

            #endregion Detour EndScene
        }

        #region Injected Methods

        private uint InstallUsePower()
        {
            ManagedFasm fasm = new ManagedFasm(d3.ProcessHandle);
            fasm.SetMemorySize(0x4096); // Allocate 16KB of memory

            AllocateMemory("UsePower_PowerInfo", 40);
            AllocateMemory("UsePower_ActorPtr", 4);
            AllocateMemory("UsePower_AcdPtr", 4);
            AllocateMemory("UsePower_Flag", 4);

            string[] asm = new string[]
            {
                "mov edx, [" + GetAddress("UsePower_Flag") + "]",
                "cmp edx, 1",
                "jne @out",

                "push " + GetAddress("UsePower_AcdPtr"),
                "push 1",
                "push 1",
                "mov esi, " + GetAddress("UsePower_PowerInfo"),
                "mov eax, [" + GetAddress("UsePower_ActorPtr") + "]",
                "call " + Offsets.METHOD_USEPOWER,
                "add esp, 12",

                "@reset:",
                "mov ebp, " + GetAddress("UsePower_Flag"),
                "mov edx, 0",
                "mov [ebp], edx",

                "@out:",
                "retn"
            };

            foreach (string line in RandomizeAsm(asm))
                fasm.AddLine(line);

            byte[] compiled = fasm.Assemble();
            uint address = AllocateMemory("UsePower", compiled.Length);
            fasm.Inject(address);
            fasm.Clear();

            return address;
        }

        private uint InstallPressButton()
        {
            ManagedFasm fasm = new ManagedFasm(d3.ProcessHandle);
            fasm.SetMemorySize(0x4096); // Allocate 16KB of memory

            AllocateMemory("PressButton_Ptr", 4);
            AllocateMemory("PressButton_Flag", 4);

            string[] asm = new string[]
            {
                "mov edx, [" + GetAddress("PressButton_Flag") + "]",
                "cmp edx, 1",
                "jne @out",

                "push 25h",
                "mov ecx, [" + GetAddress("PressButton_Ptr") + "]",
                "call " + Offsets.METHOD_PRESSBUTTON,

                "@reset:",
                "mov ebp, " + GetAddress("PressButton_Flag"),
                "mov edx, 0",
                "mov [ebp], edx",

                "@out:",
                "retn"
            };

            foreach (string line in RandomizeAsm(asm))
                fasm.AddLine(line);

            byte[] compiled = fasm.Assemble();
            uint address = AllocateMemory("PressButton", compiled.Length);
            fasm.Inject(address);
            fasm.Clear();

            return address;
        }

        #endregion Injected Methods

        private uint AllocateMemory(string name, int size)
        {
            uint address = d3.AllocateMemory(size);
            allocatedMemory[name] = new Tuple<uint, int>(address, size);
            return address;
        }

        private uint GetAddress(string name)
        {
            return allocatedMemory[name].Item1;
        }

        private void RemoveHook()
        {
            d3.WriteBytes(oEndScene, origEndSceneBytes);
        }

        private bool IsHooked()
        {
            return d3.ReadByte(oEndScene) == 0xE9;
        }

        #region Obfuscation

        private string[] NOOP_CODE = new string[]
        {
            "mov eax, eax",
            "mov ecx, ecx",
            "mov ebp, ebp",
            "mov edx, edx",
            "mov ebx, ebx",
            "mov esp, esp",
            "mov esi, esi",
            "mov edi, edi",
            "nop",
            "push ebp|pop ebp",
            "push eax|pop eax",
            "push ecx|pop ecx",
            "push edx|pop edx",
            "push ebx|pop ebx",
            "push esp|pop esp",
            "push edi|pop edi",
            "xchg eax, eax",
            "xchg ebp, ebp",
            "xchg ecx, ecx",
            "xchg edx, edx",
            "xchg ebx, ebx",
            "xchg esp, esp",
            "xchg edi, edi",
            "xchg eax, ebp|xchg ebp, eax",
            "xchg ecx, ebp|xchg ebp, ecx",
            "xchg eax, edx|xchg edx, eax",
            "xchg eax, ebx|xchg ebx, eax",
            "xchg eax, edi|xchg edi, eax",
            "xchg edi, edx|xchg edx, edi",
            "xchg ecx, ebx|xchg ebx, ecx",
            "xchg ebp, edi|xchg edi, ebp",
        };

        private string[] RandomizeAsm(IEnumerable<string> asm)
        {
            List<string> output = new List<string>();

            foreach (string line in asm)
            {
                for (int i = 0; i < rng.Next(3, 6); i++)
                {
                    string noopLine = NOOP_CODE[rng.Next(0, NOOP_CODE.Length)];
                    if (noopLine.Contains("|"))
                    {
                        foreach (string noopLineSection in noopLine.Split('|'))
                        {
                            if (noopLineSection.Length > 0)
                                output.Add(noopLineSection);
                        }
                    }
                    else
                    {
                        output.Add(noopLine);
                    }
                }

                output.Add(line);
            }

            return output.ToArray();
        }

        #endregion Obfuscation
    }
}

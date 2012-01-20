using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Magic;
using Fasm;

namespace d3sandbox
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

        public void UsePower(Player player, PowerInfo power)
        {
            uint flagAddress = GetAddress("UsePower_Flag");

            d3.WriteObject(GetAddress("UsePower_PowerInfo"), power, typeof(PowerInfo));
            d3.WriteUInt(GetAddress("UsePower_ActorPtr"), player._rActor.Pointer);
            d3.WriteUInt(GetAddress("UsePower_AcdPtr"), player._acd.Pointer);
            d3.WriteInt(flagAddress, 1);

            while (d3.ReadInt(flagAddress) == 1)
                Thread.Sleep(1);
        }

        private void InstallHook()
        {
            ManagedFasm fasm = new ManagedFasm(d3.ProcessHandle);
            fasm.SetMemorySize(0x4096); // Allocate 16KB of memory

            #region Inject New Method

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

                // Call an internal D3 function that shows a message box
                /*"push wszCaption",
                "push wszText",
                "call 0x00E34430",
                "add esp, 8",*/

                // Restore FPU and general purpose registers
                "@out:",
                "popad",
                "popfd",
                // Return to right after where we detoured
                "jmp " + (oEndScene + 5),

                // Data storage
                //"wszText du 'World',0",
                //"wszCaption du 'Hello',0"
            };

            foreach (string line in RandomizeAsm(endSceneHookASM))
                fasm.AddLine(line);

            byte[] compiled = fasm.Assemble();
            uint hookCode = AllocateMemory("EndSceneHook", compiled.Length);
            fasm.Inject(hookCode);

            #endregion Inject New Method

            #region Detour EndScene

            origEndSceneBytes = d3.ReadBytes(oEndScene, 5);

            fasm.Clear();
            fasm.AddLine("jmp 0x{0:X08}", hookCode);
            fasm.Inject(oEndScene);

            #endregion Detour EndScene
        }

        #region Injected Methods

        private uint InstallUsePower()
        {
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
                "call 0x941640",
                "add esp, 12",

                "@reset:",
                "mov ebp, " + GetAddress("UsePower_Flag"),
                "mov edx, 0",
                "mov [ebp], edx",

                "@out:",
                "retn"
            };

            ManagedFasm fasm = new ManagedFasm(d3.ProcessHandle);
            fasm.SetMemorySize(0x4096);

            foreach (string line in RandomizeAsm(asm))
                fasm.AddLine(line);

            byte[] compiled = fasm.Assemble();
            uint address = AllocateMemory("UsePower", compiled.Length);
            fasm.Inject(address);

            return address;
        }

        #endregion Injected Methods

        private uint AllocateMemory(string name, int size)
        {
            uint address = d3.AllocateMemory(size);
            allocatedMemory[name] = new Tuple<uint,int>(address, size);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace PDP1
{
    class Cpu
    {
        private const int IO_PAPER_TAPE_READER = 1;
        private const int IO_PAPER_TAPE_PUNCH = 2;
        private const int IO_TYPEWRITER = 4;

        private const int PHASE_FETCH_INST = 0;
        private const int PHASE_FETCH = 1;
        private const int PHASE_FETCH_EXT = 2;
        private const int PHASE_EXEC = 3;
        private const int PHASE_STORE = 4;
        private const int PHASE_STORE_EXT = 5;
        private const int PHASE_JUMP = 6;
        private const int PHASE_BREAK_1 = 10;
        private const int PHASE_BREAK_2 = 11;
        private const int PHASE_BREAK_3 = 12;
        private const int PHASE_FETCH_DEFER = 13;
        private const int PHASE_STORE_DEFER = 14;
        private const int PHASE_HISPEED_INITIAL = 15;
        private const int PHASE_HISPEED_FINAL = 16;

        UInt32 ac;
        UInt32 pc;
        UInt32 epc;
        UInt32 io;
        UInt32 dInst;
        UInt32 addressExtSwitches;
        UInt32 addressSwitches;
        UInt32 memoryBuffer;
        UInt32 memoryAddress;
        UInt32 memoryMask;
        UInt32[] memory;
        Boolean stopped;
        Boolean[] senseSwitches;
        Boolean debug;
        String debugLog;
        Boolean overflow;
        Boolean extend;
        Boolean extendSwitch;
        Boolean singleStep;
        Boolean singleInst;
        Boolean[] flags;
        UInt32 testWord;
        Boolean power;
        Boolean cycleFF;
        Boolean deferFF;
        Boolean hsCycleFF;
        Boolean brkCtr1FF;
        Boolean brkCtr2FF;
        Boolean readInFF;
        Boolean seqBreakFF;
        Boolean ioHaltFF;
        Boolean ioCommandsFF;
        Boolean ioSyncFF;
        PaperTapeReader paperTapeReader;
        PaperTapePunch paperTapePunch;
        TypeWriter terminal;
        SequenceBreak sequenceBreak;
        HighSpeedControl highSpeedControl;
        LinePrinter linePrinter;
        Type51TapeControl type51TapeControl;
        Type52TapeControl type52Unit0;
        Type52TapeControl type52Unit1;
        Type52TapeControl type52Unit2;
        UInt32 readInCommand;
        int readInPhase;
        int phase;
        int nextPhase;
        Boolean hsRequested;
        IoDevice hsDevice;
        public long Cycles { get; private set; }
        UInt32 inst;
        public String LinePrinterOutput { get; set; }
        public long Late { get; private set; }
        Boolean sequenceBreak120;
        private int breakWaiting;
        private int breakStarted;
        private int breakActive;
        private int interrupt;
        private int ioValue;
        private IoDevice highSpeedRequestingDevice;
        private List<IoDevice>[] devices;
        private Boolean terminateFlag;

        public Cpu()
        {
            int i;
            ac = 0;
            pc = 0;
            io = 0;
            epc = 0;
            Late = 0;
            highSpeedRequestingDevice = null;
            memory = new UInt32[65536];
            senseSwitches = new Boolean[7];
            flags = new Boolean[7];
            for (i = 0; i < 7; i++) senseSwitches[i] = false;
            for (i = 0; i < 7; i++) flags[i] = false;
            debug = false;
            power = false;
            LinePrinterOutput = "";
            extendSwitch = false;
            testWord = 0;
            addressExtSwitches = 0;
            addressSwitches = 0;
            paperTapeReader = new PaperTapeReader(this);
            paperTapePunch = new PaperTapePunch(this);
            terminal = new TypeWriter(this);
            sequenceBreak = new SequenceBreak(this);
            highSpeedControl = new HighSpeedControl(this);
            linePrinter = new LinePrinter(this);
            type51TapeControl = new Type51TapeControl(this);
            type52Unit0 = new Type52TapeControl(this, 0);
            type52Unit1 = new Type52TapeControl(this, 1);
            type52Unit2 = new Type52TapeControl(this, 2);
            paperTapeReader.Speed = 400;
            paperTapePunch.Speed = 63;
            linePrinter.Speed = 300;
            terminal.Speed = 63;
            debugLog = "";
            singleStep = false;
            singleInst = false;
            sequenceBreak120 = true;
            devices = new List<IoDevice>[64];
            for (i = 0; i < 64; i++)
            {
                devices[i] = new List<IoDevice>();
                //                devices[i].Add(new IoDevice());
            }
            devices[fromOctal("00")].Add(new IoWaitDevice(this));
            devices[fromOctal("01")].Add(paperTapeReader);
            devices[fromOctal("02")].Add(paperTapeReader);
            devices[fromOctal("03")].Add(terminal);
            devices[fromOctal("04")].Add(terminal);
            devices[fromOctal("05")].Add(paperTapePunch);
            devices[fromOctal("06")].Add(paperTapePunch);
            devices[fromOctal("30")].Add(paperTapeReader);
            devices[fromOctal("33")].Add(new IoStatus(this));
            devices[fromOctal("34")].Add(type51TapeControl);
            devices[fromOctal("35")].Add(type52Unit0);
            devices[fromOctal("35")].Add(type52Unit1);
            devices[fromOctal("35")].Add(type52Unit2);
            devices[fromOctal("36")].Add(type52Unit0);
            devices[fromOctal("36")].Add(type52Unit1);
            devices[fromOctal("36")].Add(type52Unit2);
            devices[fromOctal("45")].Add(linePrinter);
            devices[fromOctal("46")].Add(highSpeedControl);
            devices[fromOctal("50")].Add(sequenceBreak);
            devices[fromOctal("51")].Add(sequenceBreak);
            devices[fromOctal("52")].Add(sequenceBreak);
            devices[fromOctal("53")].Add(sequenceBreak);
            devices[fromOctal("54")].Add(sequenceBreak);
            devices[fromOctal("55")].Add(sequenceBreak);
            devices[fromOctal("56")].Add(sequenceBreak);
            devices[fromOctal("66")].Add(highSpeedControl);
            devices[fromOctal("66")].Add(type52Unit0);
            devices[fromOctal("66")].Add(type52Unit1);
            devices[fromOctal("66")].Add(type52Unit2);
            devices[fromOctal("67")].Add(type52Unit0);
            devices[fromOctal("67")].Add(type52Unit1);
            devices[fromOctal("67")].Add(type52Unit2);
            devices[fromOctal("70")].Add(type51TapeControl);
            devices[fromOctal("71")].Add(type51TapeControl);
            devices[fromOctal("72")].Add(type51TapeControl);
            devices[fromOctal("73")].Add(type51TapeControl);
            devices[fromOctal("74")].Add(new ExtendMode(this));
            devices[fromOctal("75")].Add(type52Unit0);
            devices[fromOctal("75")].Add(type52Unit1);
            devices[fromOctal("75")].Add(type52Unit2);
            devices[fromOctal("76")].Add(type52Unit0);
            devices[fromOctal("76")].Add(type52Unit1);
            devices[fromOctal("76")].Add(type52Unit2);
            reset();

        }

        public int fromOctal(String value)
        {
            int i;
            int ret;
            ret = 0;
            for (i = 0; i < value.Length; i++)
            {
                ret <<= 3;
                ret |= (value[i] - '0');
            }
            return ret;
        }

        public static UInt32 asciiToDec(UInt32 val)
        {
            switch (val)
            {
                case ' ': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case '0': return 16;
                case '/': return 17;
                case 's': return 18;
                case 't': return 19;
                case 'u': return 20;
                case 'v': return 21;
                case 'w': return 22;
                case 'x': return 23;
                case 'y': return 24;
                case 'z': return 25;
                case ',': return 27;
                case '\t': return 30;
                case 'j': return 33;
                case 'k': return 34;
                case 'l': return 35;
                case 'm': return 36;
                case 'n': return 37;
                case 'o': return 38;
                case 'p': return 39;
                case 'q': return 40;
                case 'r': return 41;
                case '-': return 44;
                case ')': return 45;
                case '(': return 47;
                case 'a': return 49;
                case 'b': return 50;
                case 'c': return 51;
                case 'd': return 52;
                case 'e': return 53;
                case 'f': return 54;
                case 'g': return 55;
                case 'h': return 56;
                case 'i': return 57;
                case '.': return 59;
                case 8: return 61;
                case '\r': return 63;

                case '"': return 1 | 0x100;
                case '\\': return 2 | 0x100;
                case '~': return 3 | 0x100;
                case '<': return 7 | 0x100;
                case '>': return 8 | 0x100;
                case '\'': return 16 | 0x100;
                case '?': return 17 | 0x100;
                case 'S': return 18 | 0x100;
                case 'T': return 19 | 0x100;
                case 'U': return 20 | 0x100;
                case 'V': return 21 | 0x100;
                case 'W': return 22 | 0x100;
                case 'X': return 23 | 0x100;
                case 'Y': return 24 | 0x100;
                case 'Z': return 25 | 0x100;
                case '=': return 27 | 0x100;
                case '_': return 32 | 0x100;
                case 'J': return 33 | 0x100;
                case 'K': return 34 | 0x100;
                case 'L': return 35 | 0x100;
                case 'M': return 36 | 0x100;
                case 'N': return 37 | 0x100;
                case 'O': return 38 | 0x100;
                case 'P': return 39 | 0x100;
                case 'Q': return 40 | 0x100;
                case 'R': return 41 | 0x100;
                case '+': return 44 | 0x100;
                case ']': return 45 | 0x100;
                case '|': return 46 | 0x100;
                case '[': return 47 | 0x100;
                case 'A': return 49 | 0x100;
                case 'B': return 50 | 0x100;
                case 'C': return 51 | 0x100;
                case 'D': return 52 | 0x100;
                case 'E': return 53 | 0x100;
                case 'F': return 54 | 0x100;
                case 'G': return 55 | 0x100;
                case 'H': return 56 | 0x100;
                case 'I': return 57 | 0x100;
            }
            return 0;
        }

        public String getDebugLog()
        {
            String ret;
            if (phase != PHASE_FETCH_INST) return "";
            ret = debugLog;
            debugLog = "";
            return ret;
        }

        public void clearMemory()
        {
            int i;
            for (i = 0; i < 65536; i++) memory[i] = 0;
        }

        public void reset()
        {
            pc = 0;
            stopped = true;
            overflow = false;
            extend = false;
            dInst = 0;
            epc = 0;
            memoryBuffer = 0;
            memoryAddress = 0;
            cycleFF = false;
            deferFF = false;
            hsCycleFF = false;
            readInFF = false;
            seqBreakFF = false;
            ioHaltFF = false;
            ioCommandsFF = false;
            ioSyncFF = false;
            ac = 0;
            io = 0;
            phase = 0;
            breakActive = 0;
            breakStarted = 0;
            breakWaiting = 0;
            interrupt = -1;
            hsRequested = false;
            clearMemory();
        }

        public UInt32 Ac
        {
            get { return ac; }
            set { ac = value; }
        }

        public UInt32 AddressExtSwitches
        {
            get { return addressExtSwitches; }
            set { addressExtSwitches = value; }
        }

        public UInt32 AddressSwitches
        {
            get { return addressSwitches; }
            set { addressSwitches = value; }
        }

        public int BreakActive
        {
            get { return breakActive; }
            set { breakActive = value; }
        }

        public int BreakStarted
        {
            get { return breakStarted; }
            set { breakStarted = value; }
        }

        public int BreakWaiting
        {
            get { return breakWaiting; }
            set { breakWaiting = value; }
        }

        public Boolean BrkCtr1FF
        {
            get { return brkCtr1FF; }
        }

        public Boolean BrkCtr2FF
        {
            get { return brkCtr2FF; }
        }

        public Boolean CycleFF
        {
            get { return cycleFF; }
        }

        public Boolean Debug
        {
            get { return debug; }
            set { debug = value; }
        }

        public String DebugLog
        {
            get { return debugLog; }
            set { debugLog = value; }
        }

        public Boolean DeferFF
        {
            get { return deferFF; }
        }

        public UInt32 DInst
        {
            get { return dInst; }
        }

        public UInt32 Epc
        {
            get { return epc; }
            set { epc = value; }
        }

        public Boolean Extend
        {
            get { return extend; }
            set { extend = value; }
        }

        public Boolean ExtendSwitch
        {
            get { return extendSwitch; }
            set { extendSwitch = value; }
        }

        public Boolean[] Flags
        {
            get { return flags; }
        }

        public Boolean HsCycleFF
        {
            get { return hsCycleFF; }
        }

        public UInt32 Io
        {
            get { return io; }
            set { io = value; }
        }

        public Boolean IoCommandsFF
        {
            get { return ioCommandsFF; }
        }

        public Boolean IoHaltFF
        {
            get { return ioHaltFF; }
            set { ioHaltFF = value; }
        }

        public Boolean IoSyncFF
        {
            get { return ioSyncFF; }
        }

        public LinePrinter LinePrinter
        {
            get { return linePrinter; }
        }

        public UInt32[] Memory
        {
            get { return memory; }
        }

        public UInt32 MemoryAddress
        {
            get { return memoryAddress; }
        }

        public UInt32 MemoryBuffer
        {
            get { return memoryBuffer; }
        }

        public Boolean Overflow
        {
            get { return overflow; }
            set { overflow = value; }
        }

        public Boolean SequenceBreak120
        {
            get { return sequenceBreak120; }
            set { sequenceBreak120 = value; }
        }

        public PaperTapePunch TapePunch
        {
            get { return paperTapePunch; }
        }

        public PaperTapeReader TapeReader
        {
            get { return paperTapeReader; }
        }

        public TypeWriter Terminal
        {
            get { return terminal; }
        }

        public Type51TapeControl Type51TapeControl
        {
            get { return type51TapeControl; }
        }

        public Type52TapeControl Type52TapeControl(int n)
        {
            switch (n)
            {
                case 0: return type52Unit0;
                case 1: return type52Unit1;
                case 2: return type52Unit2;
                default: return null;
            }
        }

        public UInt32 Pc
        {
            get { return pc | epc; }
            set { pc = value; }
        }

        public Boolean Power
        {
            get { return power; }
        }

        public Boolean ReadInFF
        {
            get { return readInFF; }
        }

        public Boolean[] SenseSwitches
        {
            get { return senseSwitches; }
        }

        public Boolean SeqBreakFF
        {
            get { return seqBreakFF; }
            set { seqBreakFF = value; }
        }

        public Boolean SingleInst
        {
            get { return singleInst; }
            set { singleInst = value; }
        }

        public Boolean SingleStep
        {
            get { return singleStep; }
            set { singleStep = value; }
        }

        public Boolean Stopped
        {
            get { return stopped; }
            set { stopped = value; }
        }

        public UInt32 TestWord
        {
            get { return testWord; }
            set { testWord = value; }
        }

        public void deposit()
        {
            if (!power) return;
            memory[addressSwitches | (addressExtSwitches << 12)] = testWord;
        }

        public void examine()
        {
            if (!power) return;
            ac = memory[addressSwitches | (addressExtSwitches << 12)];
            memoryBuffer = ac;
        }

        public void powerOn()
        {
            reset();
            power = true;
            stopped = true;
        }

        public void powerOff()
        {
            reset();
            power = false;
        }

        public void start()
        {
            if (!power) return;
            terminal.reset();
            Flags[0] = false;
            Flags[1] = false;
            Flags[2] = false;
            Flags[3] = false;
            Flags[4] = false;
            Flags[5] = false;
            overflow = false;
            pc = addressSwitches;
            extend = extendSwitch;
            epc = AddressExtSwitches << 12;
            stopped = false;
            phase = PHASE_FETCH_INST;
            ioHaltFF = false;
            ioSyncFF = false;
            breakActive = 0;
            breakStarted = 0;
            breakWaiting = 0;
            Cycles = 0;
            Late = 0;
        }

        public void stop()
        {
            if (!power) return;
            stopped = true;
            readInFF = false;
        }

        public void cont()
        {
            if (!power) return;
            stopped = false;
        }

        public void ReadIn()
        {
            if (!power) return;
            terminal.reset();
            readInPhase = 1;
            readInFF = true;
            stopped = false;
            extend = extendSwitch;
            epc = AddressExtSwitches << 12;
        }

        // ***********************************
        // ***** Memory access functions *****
        // ***********************************

        public void fetch()
        {
            if (extend)
            {
                if (phase == PHASE_FETCH_DEFER)
                {
                    memoryBuffer = memory[memoryAddress];
                    phase = PHASE_EXEC;
                }
                else
                {
                    if ((memoryAddress & 0x1000) != 0)
                    {
                        memoryBuffer = memory[epc | (memoryAddress & 0xfff)] & 0xffff;
                        phase = PHASE_FETCH_DEFER;
                        memoryAddress = memoryBuffer;
                    }
                    else
                    {
                        memoryBuffer = memory[epc | (memoryAddress & 0xfff)];
                        phase = PHASE_EXEC;
                    }
                }
                return;
            }
            if ((memoryAddress & 0x1000) != 0)
            {
                memoryBuffer = memory[epc | (memoryAddress & 0xfff)] & 0xffff;
                memoryAddress = memoryBuffer;
                phase = PHASE_FETCH_DEFER;
            }
            else
            {
                memoryBuffer = memory[epc | (memoryAddress & 0xfff)];
                phase = PHASE_EXEC;
            }
        }


        public void store()
        {
            if (extend)
            {
                if (phase == PHASE_STORE_DEFER)
                {
                    memory[memoryAddress] = (memory[memoryAddress] & (~memoryMask)) | (memoryBuffer & memoryMask);
                    phase = nextPhase;
                }
                else
                {
                    if ((memoryAddress & 0x1000) != 0)
                    {
                        memoryAddress = memory[epc | (memoryAddress & 0xfff)] & 0xffff;
                        phase = PHASE_STORE_DEFER;
                    }
                    else
                    {
                        memory[epc | memoryAddress] = (memory[epc | memoryAddress] & (~memoryMask)) | (memoryBuffer & memoryMask);
                        phase = nextPhase;
                    }
                }
                return;
            }
            if ((memoryAddress & 0x1000) != 0)
            {
                memoryAddress = memory[epc | (memoryAddress & 0xfff)] & 0xffff;
                phase = PHASE_STORE_DEFER;
            }
            else
            {
                memory[epc | memoryAddress] = (memory[epc | memoryAddress] & (~memoryMask)) | (memoryBuffer & memoryMask);
                phase = nextPhase;
            }
        }

        // ***********************************

        private int priority(int value)
        {
            int i;
            for (i = 0; i < 30; i++)
                if ((value & (1 << i)) != 0) return i;
            return 999;
        }

        // ****************************
        // ***** Instruction core *****
        // ****************************

        private void shiftGroup(UInt32 inst)
        {
            int i;
            Boolean right;
            int mode;
            char shiftType;
            UInt64 newBits;
            UInt64 tmp;
            UInt64 sign;
            UInt64 from;
            UInt64 to;
            UInt64 mask;
            right = ((inst & 0x01000) == 0x01000);
            mode = ((inst & 0x00200) == 0x00200) ? 1 : 0;
            mode |= ((inst & 0x00400) == 0x00400) ? 2 : 0;
            shiftType = ((inst & 0x00800) == 0x00800) ? 's' : 'r';
            if (shiftType == 's')
            {
                from = (mode != 3) ? (UInt64)1 << 17 : (UInt64)1 << 35;
                to = (right) ? (UInt64)1 << 16 : 1;
                if (mode == 3 && to != 1) to <<= 18;
            }
            else
            {
                from = (mode != 3) ? (UInt64)1 << 17 : (UInt64)1 << 35;
                to = 1;
                if (right) { tmp = from; from = to; to = tmp; }
            }
            tmp = 0;
            mask = 0;
            sign = 0;
            switch (mode)
            {
                case 1: tmp = ac; mask = 0x1ffff; sign = tmp & 0x20000; break;
                case 2: tmp = io; mask = 0x1ffff; sign = tmp & 0x20000; break;
                case 3: tmp = ((UInt64)ac << 18) | io; mask = 0x7ffffffff; sign = tmp & 0x800000000; break;
            }
            if (shiftType == 'r')
            {
                sign = 0;
                mask = (UInt64)((mask == 0x1ffff) ? 0x3ffff : 0xfffffffff);
            }
            if (debug)
            {
                debugLog += ((inst & 0x00800) == 0x00800) ? "s" : "r";
                switch (mode)
                {
                    case 1: debugLog += "a"; break;
                    case 2: debugLog += "i"; break;
                    case 3: debugLog += "c"; break;
                }
                debugLog += (right) ? "r" : "l";
                i = 0;
                if ((inst & 1) == 1) i++;
                if ((inst & 2) == 2) i++;
                if ((inst & 4) == 4) i++;
                if ((inst & 8) == 8) i++;
                if ((inst & 16) == 16) i++;
                if ((inst & 32) == 32) i++;
                if ((inst & 64) == 64) i++;
                if ((inst & 128) == 128) i++;
                if ((inst & 256) == 256) i++;
                debugLog += " " + Form1.convert((UInt32)i, 4);
            }
            for (i = 0; i < 9; i++)
            {
                if ((inst & 1) == 1)
                {
                    newBits = tmp & from;
                    tmp = (right) ? (tmp >> 1) & mask : (tmp << 1) & mask;
                    tmp |= (newBits != 0) ? to : 0;
                    tmp |= sign;
                }
                inst >>= 1;
            }
            switch (mode)
            {
                case 1: ac = (UInt32)tmp & 0x3ffff; break;
                case 2: io = (UInt32)tmp & 0x3ffff; break;
                case 3: io = (UInt32)tmp & 0x3ffff; tmp >>= 18; ac = (UInt32)tmp & 0x3ffff; break;
            }
            if (debug)
            {
                debugLog += "     ";
                switch (mode)
                {
                    case 1: debugLog += "AC = " + Form1.convert(ac, 18); break;
                    case 2: debugLog += "IO = " + Form1.convert(io, 18); break;
                    case 3: debugLog += "AC,IO = " + Form1.convert(ac, 18) + " " + Form1.convert(io, 18); break;
                }
            }
        }

        private void operateGroup(UInt32 inst)
        {
            if (inst == 0x3e000 && debug)
            {
                if (debug) debugLog += "nop     ";
            }
            if ((inst & 0x00080) == 0x00080)
            {
                ac = 0;
                if (debug) debugLog += "cla     " + "   AC = " + Form1.convert(ac, 18);
            }
            if ((inst & 0x00007) != 0x00000 && (inst & 0x00008) == 0x00000)
            {
                flags[(inst & 0x00007) - 1] = false;
                if (debug) debugLog += "clf  " + Form1.convert((inst & 0x7), 3) + "     Flag " + Form1.convert((inst & 0x7), 3) + " cleared";
            }
            if ((inst & 0x00800) == 0x00800)
            {
                io = 0;
                if (debug) debugLog += "cli     " + "   IO = " + Form1.convert(io, 18);
            }
            if ((inst & 0x00100) == 0x00100)
            {
                stopped = true;
                if (debug) debugLog += "hlt     " + "   Machine halted";
            }
            if ((inst & 0x00400) == 0x00400)
            {
                ac |= testWord;
                if (debug) debugLog += "lat     " + "   AC = " + Form1.convert(ac, 18);
            }
            if ((inst & 0x00007) != 0x00000 && (inst & 0x00008) == 0x00008)
            {
                flags[(inst & 0x00007) - 1] = true;
                if (debug) debugLog += "stf  " + Form1.convert((inst & 0x7), 3) + "     Flag " + Form1.convert((inst & 0x7), 3) + " set";
            }
            if ((inst & 0x00040) == 0x00040)
            {
                ac = (ac & 0x20fff) | (pc & 0xfff) | (UInt32)((overflow) ? 0x20000 : 0);
                ac |= (UInt32)((extend) ? 0x10000 : 0x00000);
                ac |= epc;
            }
            if ((inst & 0x00200) == 0x00200)
            {
                ac ^= 0x3ffff;
                if (debug) debugLog += "cma     " + "   AC = " + Form1.convert(ac, 18);
            }
        }

        private void skipGroup(UInt32 inst)
        {
            Boolean skip;
            skip = false;
            if ((inst & 0x1000) == 0x1000)           // Inverse sense
            {
                if ((inst & 0x00040) == 0x00040) if (ac != 0) skip = true;
                if ((inst & 0x00080) == 0x00080) if (ac >= 0x20000) skip = true;
                if ((inst & 0x00100) == 0x00100) if (ac < 0x20000) skip = true;
                if ((inst & 0x00200) == 0x00200)
                {
                    if (overflow) skip = true;
                    overflow = false;
                }
                if ((inst & 0x00400) == 0x00400) if (io >= 0x20000) skip = true;
                if ((inst & 0x00038) == 0x00008) if (senseSwitches[0]) skip = true;
                if ((inst & 0x00038) == 0x00010) if (senseSwitches[1]) skip = true;
                if ((inst & 0x00038) == 0x00018) if (senseSwitches[2]) skip = true;
                if ((inst & 0x00038) == 0x00020) if (senseSwitches[3]) skip = true;
                if ((inst & 0x00038) == 0x00028) if (senseSwitches[4]) skip = true;
                if ((inst & 0x00038) == 0x00030) if (senseSwitches[5]) skip = true;
                if ((inst & 0x00038) == 0x00038) if (senseSwitches[0] && senseSwitches[1] && senseSwitches[2] &&
                                                     senseSwitches[3] && senseSwitches[4] && senseSwitches[5]) skip = true;
                if ((inst & 0x07) == 0x01) if (Flags[0]) skip = true;
                if ((inst & 0x07) == 0x02) if (Flags[1]) skip = true;
                if ((inst & 0x07) == 0x03) if (Flags[2]) skip = true;
                if ((inst & 0x07) == 0x04) if (Flags[3]) skip = true;
                if ((inst & 0x07) == 0x05) if (Flags[4]) skip = true;
                if ((inst & 0x07) == 0x06) if (Flags[5]) skip = true;
            }
            else                                     // Normal sense
            {
                if ((inst & 0x00040) == 0x00040)
                {
                    if (debug) debugLog += "sza        ";
                    if (ac == 0) skip = true;
                }
                if ((inst & 0x00080) == 0x00080)
                {
                    if (debug) debugLog += "spa        ";
                    if (ac < 0x20000) skip = true;
                }
                if ((inst & 0x00100) == 0x00100)
                {
                    if (debug) debugLog += "sma        ";
                    if (ac >= 0x20000) skip = true;
                }
                if ((inst & 0x00200) == 0x00200)
                {
                    if (debug) debugLog += "szo        ";
                    if (!overflow) skip = true;
                    overflow = false;
                }
                if ((inst & 0x00400) == 0x00400)
                {
                    if (debug) debugLog += "spi        ";
                    if (io < 0x20000) skip = true;
                }
                if ((inst & 0x00038) == 0x00008) if (!senseSwitches[0]) skip = true;
                if ((inst & 0x00038) == 0x00010) if (!senseSwitches[1]) skip = true;
                if ((inst & 0x00038) == 0x00018) if (!senseSwitches[2]) skip = true;
                if ((inst & 0x00038) == 0x00020) if (!senseSwitches[3]) skip = true;
                if ((inst & 0x00038) == 0x00028) if (!senseSwitches[4]) skip = true;
                if ((inst & 0x00038) == 0x00030) if (!senseSwitches[5]) skip = true;
                if ((inst & 0x00038) == 0x00038) if (!senseSwitches[0] && !senseSwitches[1] && !senseSwitches[2] &&
                                                     !senseSwitches[3] && !senseSwitches[4] && !senseSwitches[5]) skip = true;
                if ((inst & 0x07) == 0x01) if (!Flags[0]) skip = true;
                if ((inst & 0x07) == 0x02) if (!Flags[1]) skip = true;
                if ((inst & 0x07) == 0x03) if (!Flags[2]) skip = true;
                if ((inst & 0x07) == 0x04) if (!Flags[3]) skip = true;
                if ((inst & 0x07) == 0x05) if (!Flags[4]) skip = true;
                if ((inst & 0x07) == 0x06) if (!Flags[5]) skip = true;
            }
            if (skip)
            {
                Skip();
                if (debug) debugLog += "skip taken";
            }
            else
            {
                if (debug) debugLog += "skip not taken";
            }
        }

        public void Skip()
        {
            pc = (pc + 1) & 0xfff;
        }

        private void doDiv()
        {
            int i;
            UInt32 value;
            UInt32 a, iv;
            long result;
            long mask;
            long num, den;
            Boolean sign;
            value = memoryBuffer;
            if (debug) debugLog += "div " + Form1.convert((inst & 0xfff), 12) + "   " + Form1.convert(ac, 18) + " / " + Form1.convert(value, 18);
            sign = (ac & 0x20000) == (value & 0x20000);
            a = ac; iv = io;
            if (a >= 0x20000) { a ^= 0x3ffff; iv ^= 0x3ffff; }
            if (value >= 0x20000) value ^= 0x3ffff;
            num = (long)a << 18 | (iv & 0x3fffe);
            den = (long)value << 18;
            if (num > den)
            {
                overflow = true;
                return;
            }
            result = 0;
            mask = (long)1 << 35;
            for (i = 0; i < 18; i++)
            {
                if (num >= den)
                {
                    num -= den;
                    result |= mask;
                }
                den >>= 1;
                mask >>= 1;
            }
            io = (UInt32)((num >> 1) & 0x3ffff);
            ac = (UInt32)(result >> 18);
            if (sign == false)
            {
                ac ^= 0x3ffff;
            }
            pc++;
        }

        private void exec()
        {
            UInt32 tmp;
            UInt32 ic;
            UInt32 a, b;
            long la, lr;
            Boolean sign;
            ic = inst >> 12;
            switch (ic & 0x3e)
            {
                case 0x02:                                                                                   // and 02
                    if (phase == PHASE_FETCH_INST)
                    {
                        memoryAddress = (inst & 0x1fff);
                        phase = PHASE_FETCH;
                        nextPhase = PHASE_EXEC;
                    }
                    else
                    {
                        tmp = memoryBuffer;
                        if (debug) debugLog += "and " + Form1.convert((inst & 0xfff), 12) + "  " + Form1.convert(ac, 18) + " & " + Form1.convert(tmp, 18) + ", AC = ";
                        ac &= tmp;
                        phase = PHASE_FETCH_INST;
                        if (debug) debugLog += Form1.convert(ac, 18);
                    }
                    break;
                case 0x04:                                                                                   // ior 04
                    if (phase == PHASE_FETCH_INST)
                    {
                        memoryAddress = (inst & 0x1fff);
                        phase = PHASE_FETCH;
                        nextPhase = PHASE_EXEC;
                    }
                    else
                    {
                        tmp = memoryBuffer;
                        if (debug) debugLog += "ior " + Form1.convert((inst & 0xfff), 12) + "  " + Form1.convert(ac, 18) + " | " + Form1.convert(tmp, 18) + ", AC = ";
                        ac |= tmp;
                        phase = PHASE_FETCH_INST;
                        if (debug) debugLog += Form1.convert(ac, 18);
                    }
                    break;
                case 0x06:                                                                                   // xor 06
                    if (phase == PHASE_FETCH_INST)
                    {
                        memoryAddress = (inst & 0x1fff);
                        phase = PHASE_FETCH;
                        nextPhase = PHASE_EXEC;
                    }
                    else
                    {
                        tmp = memoryBuffer;
                        if (debug) debugLog += "xor " + Form1.convert((inst & 0xfff), 12) + "  " + Form1.convert(ac, 18) + " ^ " + Form1.convert(tmp, 18) + ", AC = ";
                        ac ^= tmp;
                        phase = PHASE_FETCH_INST;
                        if (debug) debugLog += Form1.convert(ac, 18);
                    }
                    break;
                case 0x08:                                                                                   // xct 10
                    if (phase == PHASE_FETCH_INST)
                    {
                        memoryAddress = (inst & 0x1fff);
                        phase = PHASE_FETCH;
                        nextPhase = PHASE_EXEC;
                    }
                    else
                    {
                        inst = memoryBuffer;
                        phase = PHASE_FETCH_INST;
                        if (debug) debugLog += "xct " + Form1.convert((inst & 0xfff), 12) + ": ";
                        exec();
                    }
                    break;
                case 0x0e:                                                                                   // cal 16
                    if (ic == 0x0e)
                    {
                        memoryBuffer = ac;
                        memoryMask = 0x3ffff;
                        memoryAddress = 0x40;
                        phase = PHASE_STORE;
                        nextPhase = PHASE_FETCH_INST;
                        ac = pc & 0xfff;
                        ac |= (overflow) ? (UInt32)0x20000 : (UInt32)0;
                        ac |= (extend) ? (UInt32)0x10000 : (UInt32)0;
                        ac |= epc;
                        pc = 0x41;
                        if (debug) debugLog += "cal     " + "   PC = " + Form1.convert(pc, 12) + ", AC = " + Form1.convert(ac, 18);
                    }
                    if (ic == 0x0f)                                                                          // jda 17
                    {
                        memoryBuffer = ac;
                        memoryMask = 0x3ffff;
                        memoryAddress = (inst & 0xfff);
                        phase = PHASE_STORE;
                        nextPhase = PHASE_FETCH_INST;
                        ac = pc & 0xfff;
                        ac |= (overflow) ? (UInt32)0x20000 : (UInt32)0;
                        ac |= (extend) ? (UInt32)0x10000 : (UInt32)0;
                        ac |= epc;
                        pc = (inst + 1) & 0xfff;
                        if (debug) debugLog += "jda " + Form1.convert((inst & 0xfff), 12) + "   PC = " + Form1.convert(pc, 12) + ", AC = " + Form1.convert(ac, 18) + ", (" + Form1.convert((inst & 0xfff), 12) + ")";
                    }
                    break;
                case 0x10:                                                                                       // lac 20
                    if (phase == PHASE_FETCH_INST)
                    {
                        memoryAddress = (inst & 0x1fff);
                        phase = PHASE_FETCH;
                        nextPhase = PHASE_EXEC;
                    }
                    else
                    {
                        ac = memoryBuffer;
                        phase = PHASE_FETCH_INST;
                        if (debug) debugLog += "lac " + Form1.convert((inst & 0xfff), 12) + "   AC = " + Form1.convert(ac, 18);
                    }
                    break;
                case 0x12:                                                                                   // lio 22
                    if (phase == PHASE_FETCH_INST)
                    {
                        memoryAddress = (inst & 0x1fff);
                        phase = PHASE_FETCH;
                        nextPhase = PHASE_EXEC;
                    }
                    else
                    {
                        io = memoryBuffer;
                        phase = PHASE_FETCH_INST;
                        if (debug) debugLog += "lio " + Form1.convert((inst & 0xfff), 12) + "   IO = " + Form1.convert(io, 18);
                    }
                    break;
                case 0x14:                                                                                   // dac 24
                    if (phase == PHASE_FETCH_INST)
                    {
                        memoryBuffer = ac;
                        memoryMask = 0x3ffff;
                        memoryAddress = (inst & 0x1fff);
                        phase = PHASE_STORE;
                        nextPhase = PHASE_FETCH_INST;
                        if (debug) debugLog += "dac " + Form1.convert((inst & 0xfff), 12) + "   (" + Form1.convert((inst & 0x1fff), 12) + ") = " + Form1.convert(ac, 18);
                    }
                    break;
                case 0x16:                                                                                   // dap 26
                    if (phase == PHASE_FETCH_INST)
                    {
                        memoryBuffer = ac;
                        memoryMask = 0x00fff;
                        memoryAddress = (inst & 0x1fff);
                        phase = PHASE_STORE;
                        nextPhase = PHASE_FETCH_INST;
                        if (debug) debugLog += "dap " + Form1.convert((inst & 0xfff), 12);
                    }
                    break;
                case 0x18:                                                                                   // dip 30
                    if (phase == PHASE_FETCH_INST)
                    {
                        memoryBuffer = ac;
                        memoryMask = 0x3f000;
                        memoryAddress = (inst & 0x1fff);
                        phase = PHASE_STORE;
                        nextPhase = PHASE_FETCH_INST;
                        if (debug) debugLog += "dip " + Form1.convert((inst & 0xfff), 12);
                    }
                    break;
                case 0x1a:                                                                                   // dio 32
                    if (phase == PHASE_FETCH_INST)
                    {
                        memoryBuffer = io;
                        memoryMask = 0x3ffff;
                        memoryAddress = (inst & 0x1fff);
                        phase = PHASE_STORE;
                        nextPhase = PHASE_FETCH_INST;
                        if (debug) debugLog += "dio " + Form1.convert((inst & 0xfff), 12) + "   (" + Form1.convert((inst & 0x1fff), 12) + ") = " + Form1.convert(io, 18);
                    }
                    break;
                case 0x1c:                                                                                   // dzm 34
                    if (phase == PHASE_FETCH_INST)
                    {
                        memoryBuffer = 0;
                        memoryMask = 0x3ffff;
                        memoryAddress = (inst & 0x1fff);
                        phase = PHASE_STORE;
                        nextPhase = PHASE_FETCH_INST;
                        if (debug) debugLog += "dzm " + Form1.convert((inst & 0xfff), 12) + "   (" + Form1.convert((inst & 0x1fff), 12) + ") = " + Form1.convert(0, 18);
                    }
                    break;
                case 0x20:                                                                                   // add 40
                    if (phase == PHASE_FETCH_INST)
                    {
                        memoryAddress = (inst & 0x1fff);
                        phase = PHASE_FETCH;
                        nextPhase = PHASE_EXEC;
                    }
                    else
                    {
                        b = memoryBuffer;
                        a = ac;
                        if (debug) debugLog += "add " + Form1.convert((inst & 0xfff), 12) + "   " + Form1.convert(ac, 18) + " + " + Form1.convert(b, 18) + ", AC = ";
                        ac = (a + b);
                        if (ac >= 0x40000) ac = (ac + 1) & 0x3ffff;
                        if (ac == 0x3ffff) ac = 0;
                        if (((a & 0x20000) == (b & 0x20000)) && ((a & 0x20000) != (ac & 0x20000))) overflow = true;
                        phase = PHASE_FETCH_INST;
                        if (debug) debugLog += Form1.convert(ac, 18);
                    }
                    break;
                case 0x22:                                                                                   // sub 42
                    if (phase == PHASE_FETCH_INST)
                    {
                        memoryAddress = (inst & 0x1fff);
                        phase = PHASE_FETCH;
                        nextPhase = PHASE_EXEC;
                    }
                    else
                    {
                        b = memoryBuffer;
                        a = ac;
                        if (debug) debugLog += "sub " + Form1.convert((inst & 0xfff), 12) + "   " + Form1.convert(ac, 18) + " - " + Form1.convert(b, 18) + ", AC = ";
                        if ((a == 0x3ffff && b == 0x00000) || (a == 0x00000 && b == 0x3ffff))
                        {
                            ac = 0x3ffff;
                            overflow = false;
                            phase = PHASE_FETCH_INST;
                            if (debug) debugLog += Form1.convert(ac, 18);
                            return;
                        }
                        ac = (a - b);
                        if (ac >= 0x40000) ac = (ac - 1) & 0x3ffff;
                        if (((a & 0x20000) != (b & 0x20000)) && ((a & 0x20000) != (ac & 0x20000))) overflow = true;
                        phase = PHASE_FETCH_INST;
                        if (debug) debugLog += Form1.convert(ac, 18);
                    }
                    break;
                case 0x24:                                                                                       // idx 44
                    if (phase == PHASE_FETCH_INST)
                    {
                        memoryAddress = (inst & 0x1fff);
                        phase = PHASE_FETCH;
                        nextPhase = PHASE_EXEC;
                    }
                    else
                    {
                        ac = (memoryBuffer + 1) & 0x3ffff;
                        if (ac == 0x3ffff) ac = 0;
                        memory[memoryAddress] = ac;
                        phase = PHASE_FETCH_INST;
                        if (debug) debugLog += "idx " + Form1.convert((inst & 0xfff), 12) + "   (" + Form1.convert((memoryAddress), 12) + "),AC = " + Form1.convert(ac, 18);
                    }
                    break;
                case 0x26:                                                                                       // isp 46
                    if (phase == PHASE_FETCH_INST)
                    {
                        memoryAddress = (inst & 0x1fff);
                        phase = PHASE_FETCH;
                        nextPhase = PHASE_EXEC;
                    }
                    else
                    {
                        ac = (memoryBuffer + 1) & 0x3ffff;
                        if (ac == 0x3ffff) ac = 0;
                        memory[memoryAddress] = ac;
                        if (debug) debugLog += "isp " + Form1.convert((inst & 0xfff), 12) + "   (" + Form1.convert(memoryAddress, 12) + "),AC = " + Form1.convert(ac, 18);
                        if (ac < 0x20000)
                        {
                            pc = (pc + 1) & 0xfff;
                            if (debug) debugLog += " positive result produced skip";
                        }
                        phase = PHASE_FETCH_INST;
                    }
                    break;
                case 0x28:                                                                                   // sad 50
                    if (phase == PHASE_FETCH_INST)
                    {
                        memoryAddress = (inst & 0x1fff);
                        phase = PHASE_FETCH;
                        nextPhase = PHASE_EXEC;
                    }
                    else
                    {
                        tmp = memoryBuffer;
                        if (tmp != ac) pc = (pc + 1) & 0xfff;
                        phase = PHASE_FETCH_INST;
                        if (debug) debugLog += "sad " + Form1.convert((inst & 0xfff), 12) + "   skip ";
                        if (debug) debugLog += (tmp != ac) ? "taken" : "not taken";
                    }
                    break;
                case 0x2a:                                                                                   // sas 52
                    if (phase == PHASE_FETCH_INST)
                    {
                        memoryAddress = (inst & 0x1fff);
                        phase = PHASE_FETCH;
                        nextPhase = PHASE_EXEC;
                    }
                    else
                    {
                        tmp = memoryBuffer;
                        if (tmp == ac) pc = (pc + 1) & 0xfff;
                        phase = PHASE_FETCH_INST;
                        if (debug) debugLog += "sas " + Form1.convert((inst & 0xfff), 12) + "   skip ";
                        if (debug) debugLog += (tmp == ac) ? "taken" : "not taken";
                    }
                    break;
                case 0x2c:                                                                                       // mul 54
                    if (phase == PHASE_FETCH_INST)
                    {
                        memoryAddress = (inst & 0x1fff);
                        phase = PHASE_FETCH;
                        nextPhase = PHASE_EXEC;
                    }
                    else
                    {
                        b = memoryBuffer;
                        if (debug) debugLog += "mul " + Form1.convert((inst & 0xfff), 12) + "   " + Form1.convert(ac, 18) + " * " + Form1.convert(b, 18);
                        sign = (b & 0x20000) == (ac & 0x20000);
                        if (ac >= 0x20000) ac ^= 0x3ffff;
                        if (b >= 0x20000) b ^= 0x3ffff;
                        la = ac;
                        la <<= 18;
                        lr = 0;
                        while (b != 0)
                        {
                            if ((b & 0x20000) == 0x20000) lr += la;
                            la >>= 1;
                            b = (b << 1) & 0x3ffff;
                        }
                        io = (UInt32)(lr & 0x3fffe);
                        ac = (UInt32)((lr >> 18) & 0x1ffff);
                        if (sign == false)
                        {
                            ac ^= 0x3ffff;
                            io ^= 0x3ffff;
                        }
                        phase = PHASE_FETCH_INST;
                        if (debug) debugLog += ", AC,IO = " + Form1.convert(ac, 18) + " " + Form1.convert(io, 18);
                    }
                    break;
                case 0x2e:                                                                                   // div 56
                    if (phase == PHASE_FETCH_INST)
                    {
                        memoryAddress = (inst & 0xafff);
                        phase = PHASE_FETCH;
                        nextPhase = PHASE_EXEC;
                    }
                    else
                    {
                        doDiv();
                        phase = PHASE_FETCH_INST;
                        if (debug) debugLog += ", AC,IO = " + Form1.convert(ac, 18) + " " + Form1.convert(io, 18);
                    }
                    break;
                case 0x30:                                                                                   // jmp 60
                    if (phase == PHASE_EXEC)
                    {
                        if (extend)
                        {
                            epc = memoryAddress & 0xf000;
                            pc = memoryAddress & 0xfff;
                            if (debug) debugLog += "jmp " + Form1.convert((inst & 0xfff), 12) + "   PC = " + Form1.convert(pc, 12);
                        }
                        else
                        {
                            pc = memoryAddress & 0xfff;
                            if (debug) debugLog += "jmp " + Form1.convert((inst & 0xfff), 12) + "   PC = " + Form1.convert(pc, 12);
                        }
                        phase = PHASE_FETCH_INST;
                        return;
                    }
                    if ((inst & 0x1000) != 0)
                    {
                        memoryAddress = inst & 0x1fff;
                        phase = PHASE_FETCH;
                        nextPhase = PHASE_JUMP;
                        if ((memoryAddress & 0x3f) <= 0x3f && (memoryAddress & 0x3) == 0x1)
                        {
                            tmp = (memoryAddress & 0x3f) >> 2;
                            overflow = (memory[memoryAddress] >= 0x20000);
                            breakStarted |= (1 << (int)tmp);
                            breakStarted ^= (1 << (int)tmp);
                        }
                    }
                    else
                    {
                        pc = inst & 0xfff;
                        if (debug) debugLog += "jmp " + Form1.convert((inst & 0xfff), 12) + "   PC = " + Form1.convert(pc, 12);
                    }
                    break;
                case 0x32:                                                                                   // jsp 62
                    ac = pc & 0xfff;
                    ac |= (overflow) ? (UInt32)0x20000 : (UInt32)0;
                    ac |= (extend) ? (UInt32)0x10000 : (UInt32)0;
                    ac |= epc;
                    if ((inst & 0x1000) != 0)
                    {
                        memoryAddress = inst & 0x1fff;
                        phase = PHASE_FETCH;
                        nextPhase = PHASE_JUMP;
                    }
                    else
                    {
                        pc = inst & 0xfff;
                        if (debug) debugLog += "jsp " + Form1.convert((inst & 0xfff), 12) + "   PC = " + Form1.convert(pc, 12) + ", AC = " + Form1.convert(ac, 18);
                    }
                    break;
                case 0x34:                                                                                       // skip group 64
                    skipGroup(inst);
                    break;
                case 0x36:                                                                                       // shift group 66
                    shiftGroup(inst);
                    break;
                case 0x38:                                                                                       // law 70
                    ac = ((inst & 0x01000) == 0x01000) ? (inst & 0xfff) ^ 0x3ffff : inst & 0xfff;
                    if (debug) debugLog += "law " + Form1.convert((inst & 0xfff), 12) + "   AC = " + Form1.convert(ac, 18);
                    break;
                case 0x3a:                                                                                       // iot group 72
                    foreach (var device in devices[inst & 0x3f])
                    {
                        device.iot(inst);
                    }
                    break;
                case 0x3e:                                                                                       // operate group 76
                    operateGroup(inst);
                    break;

            }
        }

        // *********************************
        // ***** Sequence break system *****
        // *********************************

        public void requestBreak(int channel)
        {
            int mask;
            if (!seqBreakFF) return;
            if (channel < 0 || channel > 15) return;
            if (!sequenceBreak120) channel = 0;
            mask = 1 << channel;
            if (sequenceBreak120 && (breakActive & mask) == 0) return;
            breakWaiting |= mask;
        }

        public Boolean HighSpeedRequest(IoDevice dev)
        {
            if (highSpeedRequestingDevice != null) return false;
            highSpeedRequestingDevice = dev;
            return true;
        }

        private void checkBreak()
        {
            int i, j;
            if (phase == PHASE_FETCH_INST && seqBreakFF && breakWaiting != 0)
            {
                i = priority(breakWaiting);
                j = priority(breakStarted);
                if (j < 0) j = 999;
                if (i < j)
                {
                    phase = PHASE_BREAK_1;
                    interrupt = i;
                    breakWaiting ^= (1 << i);
                    breakStarted |= (1 << i);
                }
            }
        }

        // ************************
        // ***** Read In Mode *****
        // ************************

        void readInMode()
        {
            int tmp;
            switch (readInPhase)
            {
                case 1:
                    paperTapeReader.read('B', false);
                    readInPhase = 2;
                    break;
                case 2:
                    if (paperTapeReader.CharacterReady)
                    {
                        readInCommand = paperTapeReader.ReaderBuffer;
                        if (readInCommand < 0)
                        {
                            readInPhase = -1;
                            readInFF = false;
                            stopped = true;
                        }
                        else if ((readInCommand & 0x3f000) == 0x2a000)
                        {
                            paperTapeReader.read('B', false);
                            readInPhase = 3;
                        }
                        else if ((readInCommand & 0x3f000) == 0x30000)
                        {
                            readInPhase = -1;
                            readInFF = false;
                            pc = readInCommand & 0xfff;
                        }
                    }
                    break;
                case 3:
                    if (paperTapeReader.CharacterReady)
                    {
                        tmp = (int)paperTapeReader.ReaderBuffer;
                        if (tmp >= 0)
                        {
                            memory[epc | (readInCommand & 0xfff)] = (UInt32)tmp;
                            readInPhase = 1;
                        }
                        else
                        {
                            readInPhase = -1;
                            readInFF = false;
                            stopped = true;
                            return;
                        }
                    }
                    break;

            }
        }

        public void ioCompletion(int retVal)
        {
            ioSyncFF = true;
            ioValue = retVal;
        }

        public Boolean RequestHighSpeedCycle(IoDevice device)
        {
            if (hsRequested) return false;
            hsRequested = true;
            hsDevice = device;
            return true;
        }

        // ******************************
        // ***** Main machine cycle *****
        // ******************************
        public void cycle()
        {
            if (!power) return;
            paperTapeReader.cycle();
            paperTapePunch.cycle();
            linePrinter.cycle();
            terminal.cycle();
            type51TapeControl.cycle();
            type52Unit0.cycle();
            type52Unit1.cycle();
            type52Unit2.cycle();
            if (hsCycleFF) hsCycleFF = false;
            if (highSpeedRequestingDevice != null)
            {
                highSpeedRequestingDevice.HighSpeedCycle();
                hsCycleFF = true;
                Cycles++;
                highSpeedRequestingDevice = null;
                return;
            }
            if (readInFF)
            {
                readInMode();
                return;
            }
            if (stopped) return;
            Cycles++;
            if (ioSyncFF && IoHaltFF)
            {
                ioSyncFF = false;
                ioHaltFF = false;
                if (ioValue >= 0) io = (UInt32)ioValue;
            }
            /*
            if (phase == PHASE_HISPEED_FINAL)
            {
                phase = lastPhase;
                hsCycleFF = false;
            }
            if (hsRequested)
            {
                lastPhase = phase;
                hsCycleFF = true;
                phase = PHASE_HISPEED_INITIAL;
            }
            if (phase == PHASE_HISPEED_INITIAL)
            {
                hsDevice.HighSpeedCycle();
                phase = PHASE_HISPEED_FINAL;
                return;
            }
             */
            if (ioHaltFF) return;
            checkBreak();

            switch (phase)
            {
                case PHASE_BREAK_1:
                    memory[interrupt * 4 + 0] = ac;
                    brkCtr1FF = true;
                    brkCtr2FF = false;
                    if (debug) debugLog += "BRK1        [" + Form1.convert(((UInt32)interrupt * 4 + 0), 18) + "] = " + Form1.convert(memory[interrupt * 4 + 0], 18);
                    phase = PHASE_BREAK_2;
                    break;
                case PHASE_BREAK_2:
                    memory[interrupt * 4 + 1] = pc;
                    if (overflow) memory[interrupt * 4 + 1] = 0x20000;
                    brkCtr1FF = false;
                    brkCtr2FF = true;
                    if (debug) debugLog += "BRK2        [" + Form1.convert(((UInt32)interrupt * 4 + 1), 18) + "] = " + Form1.convert(ac, 18);
                    phase = PHASE_BREAK_3;
                    break;
                case PHASE_BREAK_3:
                    memory[interrupt * 4 + 2] = io;
                    pc = (UInt32)(interrupt * 4 + 3);
                    interrupt = -1;
                    brkCtr1FF = true;
                    brkCtr2FF = true;
                    phase = PHASE_FETCH_INST;
                    break;
                case PHASE_FETCH_INST:
                    brkCtr1FF = false;
                    brkCtr2FF = false;
                    if (debug) debugLog += "{" + Cycles.ToString("D6") + "}[" + Form1.convert(pc, 12) + "] ";
                    memoryAddress = epc | pc;
                    memoryBuffer = memory[memoryAddress];
                    inst = memoryBuffer;
                    pc = (pc + 1) & 0xfff;
                    dInst = inst >> 13;
                    exec();
                    break;
                case PHASE_FETCH:
                    fetch();
                    if (phase == PHASE_EXEC) exec();
                    break;
                case PHASE_FETCH_DEFER:
                    fetch();
                    if (phase == PHASE_EXEC) exec();
                    break;
                case PHASE_STORE:
                    store();
                    break;
                case PHASE_STORE_DEFER:
                    store();
                    break;
            }
            if (phase == PHASE_FETCH_INST && debug) debugLog += "\r\n";
            cycleFF = (phase == PHASE_FETCH_INST) ? false : true;
            deferFF = (phase == PHASE_FETCH_DEFER || phase == PHASE_STORE_DEFER) ? true : false;
            if (singleStep) stopped = true;
            if (singleInst && phase == PHASE_FETCH_INST) stopped = true;
            return;
        }

        public void Run()
        {
            long ticks;
            Stopwatch clock;
            clock = new Stopwatch();
            ticks = Stopwatch.Frequency / 200000;
            terminateFlag = false;
            clock.Start();
            while (!terminateFlag)
            {
                while (clock.ElapsedTicks < ticks) { }
                clock.Restart();
                cycle();
            }
        }

        public void Terminate()
        {
            terminateFlag = true;
        }
    }
}

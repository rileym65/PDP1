using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDP1
{
    class Type52TapeControl : TapeControl
    {
        public const int CMD_STOP = 0;
        public const int CMD_REWIND = 1;
        public const int CMD_BACKSPACE = 3;
        public const int CMD_FORWARD_EVEN = 8;
        public const int CMD_FORWARD_ODD = 9;
        public const int CMD_WRITE_EVEN = 10;
        public const int CMD_WRITE_ODD = 11;
        public const int CMD_COMP_EVEN = 12;
        public const int CMD_COMP_ODD = 13;
        public const int CMD_READ_EVEN = 14;
        public const int CMD_READ_ODD = 15;

        public const int STAT_EARLY_COMP_CONT = 1 << 0;
        public const int STAT_EARLY_COMP_STOP = 1 << 1;
        public const int STAT_TT_BEYOND_END = 1 << 2;
        public const int STAT_TT_LOW_TAPE = 1 << 3;
        public const int STAT_TT_FULL_TAPE = 1 << 4;
        public const int STAT_TT_AT_LOAD = 1 << 5;
        public const int STAT_TT_FILE_PROTECT = 1 << 6;
        public const int STAT_TT_REWINDING = 1 << 7;
        public const int STAT_TT_READY = 1 << 9;
        public const int STAT_CA_EQ_FA = 1 << 10;
        public const int STAT_HSC_LATE = 1 << 11;
        public const int STAT_ILLEGAL_CMD = 1 << 12;
        public const int STAT_COMP_ERROR = 1 << 13;
        public const int STAT_BUSY = 1 << 14;
        public const int STAT_MISSING_CHAR = 1 << 15;
        public const int STAT_PARITY_ERROR = 1 << 16;
        public const int STAT_MULTI_COND = 1 << 17;


        enum phases { Idle, Start, Stop, Execute, Write, Read, Comp, EORWrite, EORRead };
        public int Address { get; private set; }
        public int Irq { get; set; }
        private UInt32 DataWordBuffer;
        private byte ReadBuffer;
        private byte WriteBuffer;
        private int StateRegister;
        private int FinalAddressRegister;
        private UInt32 CurrentAddressRegister;
        private int TapeUnitRegister;
        private int StopContinueRegister;
        private phases Phase;
        private int Next;
        private Boolean RecordComplete;
        private int HighSpeedRequest;
        private int RecordCompleteTimer;
        private int eorBytes;                        // Used for seeing if at End of Record
        private int wordShift;                       // Used to track shifts for read/write operations
        private Boolean ValidStart;


        public Type52TapeControl(Cpu c,int a) : base(c,8)
        {
            Address = a;
            TapeUnitRegister = -1;
            Irq = 2 + a;
            RecordComplete = true;
            HighSpeedRequest = 0;
            StateRegister = 0;
            RecordCompleteTimer = 0;
            Phase = phases.Idle;
        }

        private byte AddParity(byte value)
        {
            int tmp;
            tmp = (value & 7) ^ ((value >> 3) & 7);
            tmp = (tmp & 1) ^ ((tmp >> 1) & 1) ^ ((tmp >> 2) & 1);
            if ((commandRegister & 1) == 1 && tmp == 0) tmp = 1;
            if ((commandRegister & 1) == 0 && tmp != 0) tmp = 0;
            if (tmp != 0) value |= 0x80;
            return value;
        }

        private Boolean CheckParity(byte value)
        {
            int tmp;
            tmp = (value & 7) ^ ((value >> 3) & 7);
            tmp = (tmp & 1) ^ ((tmp >> 1) & 1) ^ ((tmp >> 2) & 1) ^ ((value >> 7) & 1);
            if (tmp != (commandRegister & 1)) return false;
            return true;
        }

        override public void ByteRead(byte value)
        {
            ReadBuffer = value;
            if (!CheckParity(value))
            {
                StateRegister |= 3;
            }
        }

        public override void HighSpeedCycle()
        {
            if (Phase == phases.Read)
            {
                cpu.Memory[CurrentAddressRegister] = DataWordBuffer;
                DataWordBuffer = 0;
                CurrentAddressRegister++;
                if (CurrentAddressRegister == FinalAddressRegister)
                {
                    StateRegister |= STAT_CA_EQ_FA;
                    RecordComplete = true;
                    RecordCompleteTimer = 100;
                    if (Irq > 0) cpu.requestBreak(Irq - 1);
                    Next = 260;
                    Phase = phases.EORRead;
                }
            }
            if (Phase == phases.Write)
                DataWordBuffer = cpu.Memory[CurrentAddressRegister];
            if (Phase == phases.Comp)
                if (DataWordBuffer != cpu.Memory[CurrentAddressRegister])
                {
                }
        }

        protected void Read()
        {
            if (ReadBuffer == 0)
            {
                if (ValidStart)
                {
                    eorBytes++;
                }
                else
                {
                }
            }
            else
            {
                ValidStart = true;
                DataWordBuffer |= (UInt32)((ReadBuffer & 0x3f) << wordShift);
                Next = 65;
                wordShift -= 6;
                if (wordShift < 0)
                {
                    wordShift = 12;
                    HighSpeedRequest = 5;
                    Next = 65;
                }
            }
        }

        protected void Comp()
        {
            if (ReadBuffer == 0)
            {
                eorBytes++;

            }
            if (Phase == phases.Read)
            {
                DataWordBuffer |= (UInt32)((ReadBuffer & 0x3f) << wordShift);
                Next = 65;
                wordShift -= 6;
                if (wordShift < 0)
                {
                    CurrentAddressRegister++;
                    if (CurrentAddressRegister == FinalAddressRegister)
                    {
                        StateRegister |= STAT_CA_EQ_FA;
                        RecordComplete = true;
                        RecordCompleteTimer = 100;
                        if (Irq > 0) cpu.requestBreak(Irq - 1);
                        Next = 260;
                        Phase = phases.EORRead;
                    }
                    else
                    {
                        wordShift = 12;
                        HighSpeedRequest = 5;
                        Next = 65;
                    }
                }
                else Next = 65;
            }
        }

        protected void Write()
        {
            WriteBuffer = AddParity((byte)((DataWordBuffer >> wordShift) & 0x3f));
            transports[TapeUnitRegister].Write(WriteBuffer);
            wordShift -= 6;
            if (wordShift < 0)
            {
                CurrentAddressRegister++;
                if (CurrentAddressRegister == FinalAddressRegister)
                {
                    StateRegister |= STAT_CA_EQ_FA;
                    RecordComplete = true;
                    RecordCompleteTimer = 100;
                    if (Irq > 0) cpu.requestBreak(Irq - 1);
                    Next = 260;
                    Phase = phases.EORWrite;
                }
                else
                {
                    wordShift = 12;
                    HighSpeedRequest = 5;
                    Next = 65;
                }
            }
            else Next = 65;
        }

        protected void EorWrite()
        {
            if (commandRegister == CMD_WRITE_EVEN || commandRegister == CMD_WRITE_ODD) transports[TapeUnitRegister].WriteEof();
            Phase = phases.Stop;
            Next = 3000;
        }

        protected void EorRead()
        {
            Phase = phases.Stop;
            Next = 3000;
        }

        protected void Start()
        {
            if (commandRegister == CMD_READ_EVEN || commandRegister == CMD_READ_ODD)
            {
                Phase = phases.Read;
                transports[TapeUnitRegister].ReadMode();
                Next = 65;
                wordShift = 12;
                ValidStart = false;
            }
            if (commandRegister == CMD_COMP_EVEN || commandRegister == CMD_COMP_ODD)
            {
                Phase = phases.Comp;
                transports[TapeUnitRegister].ReadMode();
                Next = 65;
                wordShift = 12;
            }
            if (commandRegister == CMD_WRITE_EVEN || commandRegister == CMD_WRITE_ODD)
            {
                Phase = phases.Write;
                HighSpeedRequest = 5;
                transports[TapeUnitRegister].WriteMode();
                Next = 65;
                wordShift = 12;
            }
        }

        protected void Stop()
        {
            StateRegister &= 0x3bfff;
            commandRegister = 0;
            TapeUnitRegister = -1;
        }

        public override void cycle()
        {
            base.cycle();
            if ((StateRegister & 0x4000) == 0) return;
            if (RecordCompleteTimer > 0) RecordCompleteTimer -= 5;
            if (HighSpeedRequest > 0)
            {
                if (cpu.HighSpeedRequest(this) == true)
                {
                    HighSpeedRequest = 0;
                }
                else
                {
                    if (--HighSpeedRequest == 0) StateRegister |= 0x801;
                }
            }
            Next -= 5;
            if (Next > 0) return;
            if (Phase == phases.Read) Read();
            else if (Phase == phases.Write) Write();
            else if (Phase == phases.Comp) Comp();
            else if (Phase == phases.EORWrite) EorWrite();
            else if (Phase == phases.EORRead) EorRead();
            else if (Phase == phases.Start) Start();
            else if (Phase == phases.Stop) Stop();
        }

        private void SetState()
        {
            int tStatus;
            if (TapeUnitRegister >= 0)
            {
                tStatus = transports[TapeUnitRegister].Status();
                StateRegister &= 0x3fc03;
                if ((tStatus & 0x80) == 0) StateRegister |= STAT_TT_READY;
                if ((tStatus & 0x40) == 0) StateRegister |= STAT_TT_REWINDING;
                if ((tStatus & 0x20) == 0) StateRegister |= STAT_TT_FILE_PROTECT;
                if ((tStatus & 0x10) == 0) StateRegister |= STAT_TT_AT_LOAD;
                if ((tStatus & 0x08) == 0) StateRegister |= STAT_TT_FULL_TAPE;
                if ((tStatus & 0x04) == 0) StateRegister |= STAT_TT_LOW_TAPE;
                if ((tStatus & 0x02) == 0) StateRegister |= STAT_TT_BEYOND_END;
                if (CurrentAddressRegister == FinalAddressRegister) StateRegister |= STAT_CA_EQ_FA;
            }
            else
            {
                StateRegister = 0;
            }
        }

        public override void iot(UInt32 inst)
        {
            UInt32 y;
            int mtcu;
            y = inst & 0x3f;
            mtcu = (int)((inst >> 10) & 0x3);
            if (mtcu == 0) mtcu = 0;
            else if (mtcu == 2) mtcu = 1;
            else if (mtcu == 4) mtcu = 2;
            else mtcu = 7;
/* Maintenance instruction
            if (y == 0x37)                                                                                   // ccr
            {
                if (mtcu != Address) return;
                commandRegister = 0;
                if (TapeUnitRegister > 0) transports[TapeUnitRegister - 1].Stop();
                TapeUnitRegister = 0;
                if (cpu.Debug) cpu.DebugLog += "ccr        MTCU: " + mtcu.ToString() + " cleared";
            }
*/
            if (y == 0x1e)                                                                                   // mel
            {
                if (mtcu != Address) return;
                cpu.Io = CurrentAddressRegister;
                if (cpu.Debug) cpu.DebugLog += "mel        MTCU: " + mtcu.ToString() + " read current, IO=" + Form1.convert(cpu.Io, 18);
            }
            if (y == 0x3e)                                                                                   // muf
            {
                if (mtcu != Address) return;
                if ((StateRegister & 0x4000) != 0) return;
                TapeUnitRegister = (int)(cpu.Ac & 0x7);
                FinalAddressRegister = (int)(cpu.Io & 0xffff);
                StopContinueRegister = (int)((inst >> 6) & 0x3);
                cpu.Skip();
                if (cpu.Debug) cpu.DebugLog += "muf        MTCU: " + mtcu.ToString() + ", TU=" + Form1.convert((UInt32)TapeUnitRegister,3) + ", SC=" + Form1.convert((UInt32)StopContinueRegister,2) + ", FA="+Form1.convert((UInt32)FinalAddressRegister, 18);
            }
            if (y == 0x37)                                                                                   // mrf
            {
                if (mtcu != Address) return;
                FinalAddressRegister = (int)(cpu.Io & 0xffff);
                if (cpu.Debug) cpu.DebugLog += "mrf        MTCU: " + mtcu.ToString() + ", FA=" + Form1.convert((UInt32)FinalAddressRegister, 18);
            }
            if (y == 0x36)                                                                                   // mri
            {
                if (mtcu != Address) return;
                CurrentAddressRegister = (cpu.Io & 0xffff);
                commandRegister = (commandRegister & 0xfd) | (int)((inst >> 6) & 0x2);
                if (cpu.Debug) cpu.DebugLog += "mri        MTCU: " + mtcu.ToString() + ", CA=" + Form1.convert((UInt32)CurrentAddressRegister, 18);
            }
            if (y == 0x1d)                                                                                   // mes
            {
                if (mtcu != Address) return;
                SetState();
                cpu.Io = (UInt32)StateRegister;
                if (cpu.Debug) cpu.DebugLog += "mes        MTCU: " + mtcu.ToString() + ", IO=" + Form1.convert(cpu.Io, 18);
            }
            if (y == 0x3d)                                                                                   // mic
            {
                if (mtcu != Address) return;
                if ((StateRegister & 0x4000) != 0) return;
                CurrentAddressRegister = (cpu.Io & 0xffff);
                StateRegister = 0;
                commandRegister = (int)((inst >> 6) & 0xf);
                if (cpu.Debug) cpu.DebugLog += "mic        MTCU: " + mtcu.ToString() + ", CR=" + Form1.convert((UInt32)commandRegister,4) + ", CA=" + Form1.convert((UInt32)CurrentAddressRegister, 18);
                cpu.Skip();
                StateRegister |= 0x4001;
                RecordCompleteTimer = 0;
                RecordComplete = false;
                if (commandRegister == 0x0)           // Stop
                {
                    transports[TapeUnitRegister].Stop();
                    Next = 5000;
                    Phase = phases.Stop;
                }
                else if (commandRegister == 0x1)      // Rewind
                {
                    transports[TapeUnitRegister].Rewind();
                    Next = 10;
                    Phase = phases.Execute;
                }
                else if (commandRegister == 0x3)      // Backspace
                {
                    transports[TapeUnitRegister].Reverse();
                    Next = 5000;
                    Phase = phases.Start;
                }
                else if (commandRegister == 0x2 || (commandRegister >= 4 && commandRegister <= 7))
                {
                    StateRegister &= (~0x4000);
                    StateRegister |= 0x1001;
                }
                else
                {                                // Everything else
                    transports[TapeUnitRegister].Forward();
                    Next = 5000;
                    Phase = phases.Start;
                    eorBytes = 0;
                }
            }
        }
    }
}

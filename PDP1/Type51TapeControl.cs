using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDP1
{
    class Type51TapeControl : TapeControl
    {

        public Type51TapeControl(Cpu c) : base(c,3)
        {
        }

        override public void ByteRead(byte value)
        {
            characterBuffer = value;
            cpu.Flags[1] = true;
        }

        public override void iot(UInt32 inst)
        {
            UInt32 y;
            int value;
            int x4, x5, x6;
            y = inst & 0x3f;
            if (y == 0x1c)                                                                                   // mcs
            {
                cpu.Io &= 0x007ff;
                cpu.Io |= (UInt32)((currentTransport > 0) ? transports[currentTransport-1].Status() << 10 : 0);
                if (cpu.Debug) cpu.DebugLog += "mcs        IO = " + Form1.convert((UInt32)cpu.Io, 18);
            }
            else if (y == 0x3b)                                                                              // msm
            {
                commandRegister = (int)(cpu.Io & 0xff);
                x6 = (commandRegister & 0x7);
                x5 = ((commandRegister >> 3) & 0x7);
                x4 = ((commandRegister >> 6) & 0x7);
                currentTransport = (x6 < 4) ? x6 : 0;
                if (currentTransport != 0)
                {
                    if (x4 == 3)
                    {
                        transports[currentTransport - 1].Rewind();
                    }
                    if (x4 < 2) transports[currentTransport - 1].Stop();
                    if (x4 == 2)
                    {
                        if (x5 < 4) transports[currentTransport - 1].Forward();
                        else transports[currentTransport - 1].Reverse();
                        if ((x5 & 2) == 2) transports[currentTransport - 1].WriteMode();
                        else transports[currentTransport - 1].ReadMode();
                        parity = ((x5 & 1) == 1) ? 'E' : 'O';
                    }
                }
                if (cpu.Debug) cpu.DebugLog += "msm        Cmd = " + Form1.convert((UInt32)cpu.Io, 8);
            }
            else if (y == 0x38)                                                                              // mcb
            {
                if (currentTransport > 0)
                {
                    transports[currentTransport - 1].WriteEof();
                    if (cpu.Debug) cpu.DebugLog += "mcb        Clear buffer";
                }
            }
            else if (y == 0x39)                                                                              // mwc
            {
                if (currentTransport > 0)
                {
                    value = (byte)((cpu.Io >> 12) & 0x3f);
                    transports[currentTransport - 1].Write(SetParity(value));
                    if (cpu.Debug) cpu.DebugLog += "mwc        Byte written to tape:" + Form1.convert((UInt32)value, 7);
                }
            }
            else if (y == 0x3a)                                                                              // mrc
            {
                if (currentTransport > 0)
                {
                    cpu.Io |= (UInt32)(characterBuffer & 0x3f);
                    cpu.Flags[1] = false;
                    if (cpu.Debug) cpu.DebugLog += "mrc        Byte read from tape:" + Form1.convert((UInt32)characterBuffer, 6) + ", IO = " + Form1.convert((UInt32)cpu.Io, 18);
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PDP1
{
    class LinePrinter : IoDevice
    {
        private const int OPR_LOAD = 1;
        private const int OPR_PRNT = 2;
        private const int OPR_CLR = 3;

        private int timer;
        private Boolean ready;
        private int operation;
        private String buffer;
        private Boolean sendCompletion;
        private int spacing;
        private int irq;
        private int speed;

        public LinePrinter(Cpu c) : base(c)
        {
            ready = true;
            buffer = "";
            irq = 15;
            Speed = 300;
        }

        public int Irq
        {
            get { return irq; }
            set { irq = value; }
        }

        public int Speed
        {
            get { return speed; }
            set { speed = 200000 / value; }
        }

        public UInt32 decToAscii(UInt32 val)
        {
            switch (val)
            {
                case 0: return ' ';         // 00
                case 1: return '1';         // 01
                case 2: return '2';         // 02
                case 3: return '3';         // 03
                case 4: return '4';         // 04
                case 5: return '5';         // 05
                case 6: return '6';         // 06
                case 7: return '7';         // 07
                case 8: return '8';         // 10
                case 9: return '9';         // 11
                case 10: return '\'';       // 12
                case 11: return '~';        // 13
                case 12: return ' ';        // 14
                case 13: return ' ';        // 15
                case 14: return ' ';        // 16
                case 15: return '<';        // 17
                case 16: return '0';        // 20
                case 17: return '/';        // 21
                case 18: return 'S';        // 22
                case 19: return 'T';        // 23
                case 20: return 'U';        // 24
                case 21: return 'V';        // 25
                case 22: return 'W';        // 26
                case 23: return 'X';        // 27
                case 24: return 'Y';        // 30
                case 25: return 'Z';        // 31
                case 26: return '"';        // 32
                case 27: return ',';        // 33
                case 28: return '>';        // 34
                case 29: return ' ';        // 35
                case 30: return ' ';        // 36
                case 31: return '?';        // 37
                case 32: return '.';        // 40
                case 33: return 'J';        // 41
                case 34: return 'K';        // 42
                case 35: return 'L';        // 43
                case 36: return 'M';        // 44
                case 37: return 'N';        // 45
                case 38: return 'O';        // 46
                case 39: return 'P';        // 47
                case 40: return 'Q';        // 50
                case 41: return 'R';        // 51
                case 42: return ' ';        // 52
                case 43: return '=';        // 53
                case 44: return '-';        // 54
                case 45: return ')';        // 55
                case 46: return ' ';        // 56
                case 47: return '(';        // 57
                case 48: return '_';        // 60
                case 49: return 'A';        // 61
                case 50: return 'B';        // 62
                case 51: return 'C';        // 63
                case 52: return 'D';        // 64
                case 53: return 'E';        // 65
                case 54: return 'F';        // 66
                case 55: return 'G';        // 67
                case 56: return 'H';        // 70
                case 57: return 'I';        // 71
                case 58: return 'X';        // 72
                case 59: return ' ';        // 73
                case 60: return '+';        // 74
                case 61: return ']';        // 75
                case 62: return '|';        // 76
                case 63: return '[';        // 77
            }
            return ' ';
        }

        public override void cycle()
        {
            if (ready) return;
            if (--timer <= 0)
            {
                ready = true;
                if (sendCompletion)
                {
                    cpu.ioCompletion(-1);
                }
                if (irq >= 0) cpu.requestBreak(irq);
                if (operation == OPR_PRNT)
                {
                    for (var i = 0; i < spacing; i++) buffer += "\r\n";
                    cpu.LinePrinterOutput = buffer;
                    buffer = "";
                }
            }
        }

        public override void iot(UInt32 inst)
        {
            UInt32 y;
            Boolean wait;
            if (!ready) return;
            wait = ((inst & 0x1000) == 0x1000);
            sendCompletion = ((inst & 0x0800) == 0x0800);
            sendCompletion ^= wait;
            y = (inst >> 9) & 0x7;
            spacing = (int)((inst >> 6) & 7) + 1;
            if (y == 0x02)                                                                                    // clrbuf
            {
                buffer = "";
                timer = 1;
                operation = OPR_CLR;
                ready = false;
                if (cpu.Debug) cpu.DebugLog += "clrbuf     Line printer buffer cleared";
            }
            else if (y == 0x00)                                                                              // lpb
            {
                cpu.IoHaltFF = wait;
                if (buffer.Length < 120)
                {
                    buffer += ((char)decToAscii(cpu.Io & 0x3f)).ToString();
                }
                timer = 1;
                operation = OPR_LOAD;
                ready = false;
                if (cpu.Debug) cpu.DebugLog += "lpb        Send to printer: " + Form1.convert((UInt32)cpu.Io &0x3f, 8);
            }
            else if (y == 0x01)                                                                              // pas
            {
                cpu.IoHaltFF = wait;
                timer = speed;
                operation = OPR_PRNT;
                ready = false;
                if (cpu.Debug) cpu.DebugLog += "pas        Print buffer";
            }
        }
    }
}

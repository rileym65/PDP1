using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PDP1
{
    class TypeWriter : IoDevice
    {
        private String outBuffer;
        private Boolean keyReady;
        private Boolean ready;
        private Boolean shift;
        private Boolean inShift;
        private int timer;
        private int key;
        private int secondKey;
        private int speed;
        private int outputIrq;
        private int inputIrq;
        private UInt32 tyoRegister;
        private Boolean sendCompletion;

        public TypeWriter(Cpu c) : base(c)
        {
            reset();
            outputIrq = 6;
            inputIrq = 5;
        }

        public void reset()
        {
            key = -1;
            secondKey = -1;
            outBuffer = "";
            keyReady = false;
            ready = true;
            shift = false;
            inShift = false;
        }

        public UInt32 decToAscii(UInt32 val)
        {
            switch (val)
            {
                case 0: return ' ';
                case 1: return (!shift) ? '1' : '"';
                case 2: return (!shift) ? '2' : '\'';
                case 3: return (!shift) ? '3' : '~';
                case 4: return (!shift) ? '4' : ' ';          // implies
                case 5: return (!shift) ? '5' : ' ';          // or
                case 6: return (!shift) ? '6' : ' ';          // and
                case 7: return (!shift) ? '7' : '<';
                case 8: return (!shift) ? '8' : '>';
                case 9: return (!shift) ? '9' : ' ';          // up arrow
                case 16: return (!shift) ? '0' : ' ';         // right arrow
                case 17: return (!shift) ? '/' : '?';
                case 18: return (!shift) ? 's' : 'S';
                case 19: return (!shift) ? 't' : 'T';
                case 20: return (!shift) ? 'u' : 'U';
                case 21: return (!shift) ? 'v' : 'V';
                case 22: return (!shift) ? 'w' : 'W';
                case 23: return (!shift) ? 'x' : 'X';
                case 24: return (!shift) ? 'y' : 'Y';
                case 25: return (!shift) ? 'z' : 'Z';
                case 27: return (!shift) ? ',' : '=';
                case 28: return (!shift) ? ' ' : ' ';         // black
                case 29: return (!shift) ? ' ' : ' ';         // red
                case 30: return (!shift) ? '\t' : '\t';
                case 32: return (!shift) ? ' ' : '_';
                case 33: return (!shift) ? 'j' : 'J';
                case 34: return (!shift) ? 'k' : 'K';
                case 35: return (!shift) ? 'l' : 'L';
                case 36: return (!shift) ? 'm' : 'M';
                case 37: return (!shift) ? 'n' : 'N';
                case 38: return (!shift) ? 'o' : 'O';
                case 39: return (!shift) ? 'p' : 'P';
                case 40: return (!shift) ? 'q' : 'Q';
                case 41: return (!shift) ? 'r' : 'R';
                case 44: return (!shift) ? '-' : '+';
                case 45: return (!shift) ? ')' : ']';
                case 46: return (!shift) ? ' ' : '|';
                case 47: return (!shift) ? '(' : '[';
                case 49: return (!shift) ? 'a' : 'A';
                case 50: return (!shift) ? 'b' : 'B';
                case 51: return (!shift) ? 'c' : 'C';
                case 52: return (!shift) ? 'd' : 'D';
                case 53: return (!shift) ? 'e' : 'E';
                case 54: return (!shift) ? 'f' : 'F';
                case 55: return (!shift) ? 'g' : 'G';
                case 56: return (!shift) ? 'h' : 'H';
                case 57: return (!shift) ? 'i' : 'I';
                case 58: shift = false; return 0;
                case 59: return (!shift) ? '.' : 'X';
                case 60: shift = true; return 0;
                case 61: return 8;
                case 63: return 13;
            }
            return ' ';
        }

        public int InputIrq
        {
            get { return inputIrq; }
            set { inputIrq = value; }
        }

        public Boolean KeyReady
        {
            get { return keyReady; }
        }

        public int OutputIrq
        {
            get { return outputIrq; }
            set { outputIrq = value; }
        }

        public int Speed
        {
            set { speed = 200000 / value; }
        }

        public Boolean Ready
        {
            get { return ready; }
        }

        public override void cycle()
        {
            if (ready) return;
            if (--timer <= 0)
            {
                ready = true;
                outBuffer += ((char)decToAscii(tyoRegister)).ToString();
                if (sendCompletion)
                {
                    cpu.ioCompletion(-1);
                }
                if (outputIrq >= 0) cpu.requestBreak(outputIrq);
            }
        }

        public void keyTyped(char k)
        {
            key = (int)Cpu.asciiToDec((UInt32)k);
            if (key >= 0x100)
            {
                if (inShift)
                {
                    key = key & (0xff);
                }
                else
                {
                    secondKey = key & (0xff);
                    inShift = true;
                    key = 60;
                }

            }
            else
            {
                if (inShift)
                {
                    secondKey = key & (0xff);
                    inShift = false;
                    key = 58;
                }
                else
                {
                    key = key & 0xff;
                }
            }
            keyReady = true;
            if (inputIrq >= 0) cpu.requestBreak(inputIrq);
        }

        public UInt32 read()
        {
            UInt32 ret;
            keyReady = false;
            ret = (UInt32)key;
            if (secondKey >= 0)
            {
                key = secondKey;
                secondKey = -1;
                keyReady = true;
                if (inputIrq >= 0) cpu.requestBreak(inputIrq);
            }
            return ret;
        }

        public void write(UInt32 value,Boolean sc)
        {
            if (!ready) return;
            timer = speed;
            ready = false;
            tyoRegister = value;
            sendCompletion = sc;
        }

        public String getOutput()
        {
            String ret;
            ret = outBuffer;
            outBuffer = "";
            return ret;
        }

        public override void iot(UInt32 inst)
        {
            UInt32 y;
            Boolean wait;
            Boolean pulse;
            wait = ((inst & 0x1000) == 0x1000);
            pulse = ((inst & 0x0800) == 0x0800);
            pulse ^= wait;
            y = inst & 0xff;
            if (y == 0x04)                                                                              // tyi
            {
                cpu.Io = read();
                if (cpu.Debug) cpu.DebugLog += "tyi        IO = " + Form1.convert(cpu.Io, 18);
            }
            else if (y == 0x03)                                                                   // tyo
            {
              cpu.IoHaltFF = wait;
              write(cpu.Io & 0xff,pulse);
              if (cpu.Debug) cpu.DebugLog += "tyo        Value sent to typewriter: " + Form1.convert((UInt32)cpu.Io & 0xff, 8);
            }
        }
    }
}

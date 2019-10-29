using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace PDP1
{
    class PaperTapeReader : IoDevice
    {
        private FileStream readerFile;
        private Boolean characterReady;
        private int timer;
        private Boolean ready;
        private Boolean eof;
        private char readMode;
        private byte[] buffer;
        private int value;
        private int speed;
        private int count;
        private UInt32 readerBuffer;
        private Boolean sendCompletion;
        private int irq;

        public PaperTapeReader(Cpu c) : base(c)
        {
            readerFile = null;
            ready = true;
            characterReady = false;
            buffer = new byte[3];
            readMode = 'A';
            irq = 7;
        }

        public Boolean CharacterReady
        {
            get
            {
                if (!Ready) return false;
                return characterReady;
            }
        }

        public Boolean Eof
        {
            get { return eof; }
        }

        public int Irq
        {
            get { return irq; }
            set { irq = value; }
        }

        public UInt32 ReaderBuffer
        {
            get
            {
                characterReady = false;
                return readerBuffer;
            }
        }

        public Boolean Ready
        {
            get { return ready; }
        }

        public int Speed
        {
            set { speed = 200000 / value; }
        }

        public void mount(String filename)
        {
            readerFile = new FileStream(filename, FileMode.Open);
            eof = false;
        }

        public void unmount()
        {
            readerFile.Close();
            readerFile = null;
        }

        public void rewind()
        {
            readerFile.Seek(0, SeekOrigin.Begin);
            eof = false;
        }

        public void read(char mode,Boolean sc)
        {
            int i;
            int readByte;
            if (!ready) return;
            sendCompletion = sc;
            readMode = mode;
            count = (readMode == 'A') ? 1 : 3;
            if (readerFile == null)
            {
                eof = true;
                return;
            }
            timer = speed * count;
            ready = false;
            i = 0;
            while (i < count)
            {
                readByte = readerFile.ReadByte();
                if (readByte >= 0)
                {
                    if (readByte >= 128 || readMode == 'A')
                    {
                        buffer[i] = (byte)readByte;
                        i++;
                    }
                }
                else
                {
                    eof = true;
                    i++;
                }
            }
            if (readMode == 'A')
            {
                value = buffer[0];
            }
            else
            {
                value = (buffer[0] & 0x3f) << 12;
                value |= ((buffer[1] & 0x3f) << 6);
                value |= (buffer[2] & 0x3f);
            }
        }

        public override void cycle()
        {
            if (ready) return;
            if (--timer <= 0)
            {
                ready = true;
                characterReady = true;
                readerBuffer = (UInt32)value;
                if (sendCompletion)
                {
                    cpu.ioCompletion(value);
                }
                if (irq >= 0) cpu.requestBreak(irq);
            }
        }

        public override void iot(UInt32 inst)
        {
            UInt32 y;
            Boolean wait;
            Boolean pulse;
            wait = ((inst & 0x1000) == 0x1000);
            pulse = ((inst & 0x0800) == 0x0800);
            pulse ^= wait;
            y = inst & 0x3f;
            if (y == 0x01)                                                                                    // rpa
            {
                cpu.IoHaltFF = wait;
                read('A', pulse);
                if (cpu.Debug) cpu.DebugLog += "rpa        Read tape in ascii mode";
            }
            else if (y == 0x02)                                                                               // rpb
            {
                cpu.IoHaltFF = wait;
                read('B', pulse);
                if (cpu.Debug) cpu.DebugLog += "rpb        Read tape in binary mode";
            }
            else if (y == 0x18)                                                                               // rrb
            {
                cpu.Io = ReaderBuffer;
                if (cpu.Debug) cpu.DebugLog += "rrb        IO = " + Form1.convert(cpu.Io, 18);
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace PDP1
{
    class PaperTapePunch : IoDevice
    {
        private FileStream punchFile;
        private Boolean ready;
        private byte[] buffer;
        private int speed;
        private int timer;
        private Boolean sendCompletion;
        private int irq;
        private Boolean sendBreak;

        public PaperTapePunch(Cpu c) : base(c)
        {
            punchFile = null;
            ready = true;
            buffer = new byte[1];
            irq = 8;
            sendBreak = false;
        }

        public int Irq
        {
            get { return irq; }
            set { irq = value; }
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
            if (File.Exists(filename))
            {
                punchFile = new FileStream(filename, FileMode.Truncate);
            }
            else
            {
                punchFile = new FileStream(filename, FileMode.OpenOrCreate);
            }
        }

        public void unmount()
        {
            punchFile.Close();
            punchFile = null;
        }

        public void punch(int value,char mode,Boolean sc)
        {
            sendCompletion = sc;
            sendBreak = (sc) ? false : true;
            timer = speed;
            ready = false;
            if (mode == 'B')
            {
                value = 128 | (value >> 12);
            }
            else
            {
                value &= 0xff;
            }
            buffer[0] = (byte)value;
            if (punchFile != null) punchFile.Write(buffer, 0, 1);
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
            if (y == 0x05)                                                                                    // ppa
            {
                cpu.IoHaltFF = wait;
                punch((int)(cpu.Io & 0xff), 'A', pulse);
                if (cpu.Debug) cpu.DebugLog += "ppa        Punch tape in ascii mode: " + Form1.convert((UInt32)cpu.Io & 0xff, 8);
            }
            else if (y == 0x006)                                                                              // ppb
            {
                cpu.IoHaltFF = wait;
                punch((int)cpu.Io, 'B', pulse);
                if (cpu.Debug) cpu.DebugLog += "ppb        Punch tape in binary mode: " + Form1.convert((UInt32)cpu.Io >> 12, 8);
            }
        }

    }
}

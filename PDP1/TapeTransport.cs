using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace PDP1
{
    class TapeTransport
    {
        int tapePosition;
        byte[] tape;
        byte eofChar;
        char moving;
        char direction;
        char mode;
        int next;
        int tapeSpeed;                                      /* microseconds until next position */
        Boolean writeProtected;
        public String filename { get; private set; }
        TapeControl control;
        int stopCount;

        public TapeTransport(TapeControl c)
        {
            control = c;
            tape = new byte[5760000];
            tapePosition = 24000;
            moving = ' ';
            mode = ' ';
            tapeSpeed = 0;
            writeProtected = false;
            filename = "";
        }

        private void TapeSpeed(int speed)
        {
            tapeSpeed = (speed == 500) ? 10 : 65;
        }

        public Boolean WriteProtected
        {
            get { return writeProtected; }
            set { writeProtected = value; }
        }

        public int Status()
        {
            int ret;
            ret = (moving == ' ') ? 0 : 1 << 7;
            ret |= (moving == 'R') ? 0 : 1 << 6;
            ret |= (writeProtected) ? 0 : 1 << 5;
            ret |= (tapePosition <= 24000) ? 0 : 1 << 4;
            ret |= (tapePosition <= 240000) ? 0 : 1 << 3;
            ret |= (tapePosition >= 5520000) ? 0 : 1 << 2;
            ret |= (tapePosition >= 5726400) ? 0 : 1 << 1;
            return ret;
        }

        public byte Read()
        {
            if (mode != 'R') return 0;
            return tape[tapePosition];
        }

        public void Write(byte value)
        {
            if (mode != 'W') return;
            if (!writeProtected)
            {
                tape[tapePosition] = value;
                eofChar ^= value;
            }
        }

        public void WriteEof()
        {
            if (mode != 'W') return;
            tape[tapePosition] = eofChar;
            eofChar = 0;
        }

        public void Reverse()
        {
            moving = 's';
            direction = 'b';
            TapeSpeed(75);
            eofChar = 0;
            next = 2500;
            stopCount = 0;
        }

        public void Forward()
        {
            moving = 's';
            direction = 'f';
            TapeSpeed(75);
            eofChar = 0;
            next = 2500;
            stopCount = 0;
        }

        public void ReadMode()
        {
            mode = 'R';
            eofChar = 0;
        }

        public void WriteMode()
        {
            mode = 'W';
            eofChar = 0;
        }

        public void Rewind()
        {
            moving = 'R';
            mode = ' ';
            TapeSpeed(500);
            next = 65;
            stopCount = 0;
        }

        public void Stop()
        {
            if (moving == 'R' || moving == ' ') return;
            stopCount = 600;
        }

        /* Called once for each 5us cpu cycle */
        public void cycle()
        {
            if (moving == ' ') return;
            if (stopCount > 0)
                if (--stopCount <= 0)
                {
                    moving = ' ';
                    return;
                }
            next -= 5;
            if (next > 0) return;
            if (moving == 's' || moving == 'S')
            {
                moving = direction;
                next = tapeSpeed;
                return;
            }
            if (moving == 'F' || moving == 'f')
            {
                if (++tapePosition > 5760000)
                {
                    moving = ' ';
                    tapePosition = 5760000;
                }
                else
                {
                    next = tapeSpeed;
                }
            }
            if (moving == 'B' || moving == 'b' || moving == 'R')
            {
                if (--tapePosition <= 24000)
                {
                    moving = ' ';
                    tapePosition = 24000;
                }
                else
                {
                    next = tapeSpeed;
                }
            }
            if (tape[tapePosition] != 0 && mode == 'R') control.ByteRead(Read());
        }

        public void Mount(String path)
        {
            byte[] readBytes;
            if (path.Length < 1) return;
            for (var i = 0; i < tape.Length; i++) tape[i] = 0;
            try
            {
                filename = path;
                readBytes = File.ReadAllBytes(path);
                for (var i = 0; i < readBytes.Length; i++) tape[i] = readBytes[i];
                tapePosition = 24000;
                moving = ' ';
                mode = ' ';
            }
            catch
            {
                filename = path;
            }
        }

        public void Unmount()
        {
            if (filename.Length < 1) return;
            moving = ' ';
            mode = ' ';
            try
            {
                File.WriteAllBytes(filename, tape);
            }
            catch
            {
            }
            filename = "";
        }
    }
}

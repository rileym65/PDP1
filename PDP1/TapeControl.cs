using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDP1
{
    class TapeControl : IoDevice
    {
        protected byte characterBuffer;
        protected TapeTransport[] transports;
        protected int currentTransport;
        protected char parity;
        protected int commandRegister;

        public TapeControl(Cpu c,int numTransports) : base(c)
        {
            currentTransport = 0;
            parity = 'O';
            transports = new TapeTransport[numTransports];
            for (var i = 0; i < numTransports; i++)
                transports[i] = new TapeTransport(this);
            commandRegister = 0;
        }

        virtual public void ByteRead(byte value)
        {
            characterBuffer = value;
        }

        public override void cycle()
        {
            foreach (var transport in transports)
            {
                transport.cycle();
            }
        }

        public byte SetParity(int value)
        {
            int temp;
            byte orig;
            orig = (byte)value;
            value ^= (value >> 3);
            temp = value;
            value ^= (temp >> 1);
            value ^= (temp >> 2);
            if ((value & 1) == 0) orig |= 64;
            return orig;
        }

        public TapeTransport transport(int num)
        {
            if (num < 0 || num >= transports.Length) return null;
            return transports[num];
        }

    }
}

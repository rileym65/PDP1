using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PDP1
{
    class Diagnostics
    {
        int goodTests;
        int badTests;
        Cpu cpu;
        String results;

        public Diagnostics()
        {
            cpu = new Cpu();
            cpu.powerOn();
        }

        private void info(String msg)
        {
            results += "Info: " + msg + "\r\n";
        }

        private void good(String msg)
        {
            results += "Good: " + msg + "\r\n";
            goodTests++;
        }

        private void bad(String msg)
        {
            results += "Bad: " + msg + "\r\n";
            badTests++;
        }

        private void lacTests()
        {
            results += "lac tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0;
            cpu.Memory[0x000] = 0x10100;
            cpu.Memory[0x100] = 0x12345;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 1) good("PC incremented by 1 after lac");
            else bad("PC not incremented by 1 after lac");
            if (cpu.Ac == 0x12345) good("AC had correct value after lac");
            else bad("AC did not have correct value after lac");
        }

        private void dacTests()
        {
            results += "\r\ndac tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x29327;
            cpu.Memory[0x000] = 0x14105;
            cpu.Memory[0x105] = 0x12345;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 1) good("PC incremented by 1 after dac");
            else bad("PC not incremented by 1 after dac");
            if (cpu.Ac == 0x29327) good("AC was not modified by dac");
            else bad("AC was modified by dac");
            if (cpu.Memory[0x105] == 0x29327) good("Memory had correct value after dac");
            else bad("Memory did not have correct value after dac");
        }

        private void dapTests()
        {
            results += "\r\ndap tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x49285;
            cpu.Memory[0x000] = 0x16106;
            cpu.Memory[0x106] = 0x12456;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 1) good("PC incremented by 1 after dap");
            else bad("PC not incremented by 1 after dap");
            if (cpu.Ac == 0x49285) good("AC was not modified by dap");
            else bad("AC was modified by dap");
            if (cpu.Memory[0x106] == 0x12285) good("Memory had correct value after dap");
            else bad("Memory did not have correct value after dap");
        }

        private void dipTests()
        {
            results += "\r\ndip tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x32103;
            cpu.Memory[0x000] = 0x18107;
            cpu.Memory[0x107] = 0x12456;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 1) good("PC incremented by 1 after dip");
            else bad("PC not incremented by 1 after dip");
            if (cpu.Ac == 0x32103) good("AC was not modified by dip");
            else bad("AC was modified by dip");
            if (cpu.Memory[0x107] == 0x32456) good("Memory had correct value after dip");
            else bad("Memory did not have correct value after dip");
        }

        private void lioTests()
        {
            results += "\r\nlio tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x54321;
            cpu.Io = 0;
            cpu.Memory[0x000] = 0x12108;
            cpu.Memory[0x108] = 0x24680;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 1) good("PC incremented by 1 after lio");
            else bad("PC not incremented by 1 after lio");
            if (cpu.Ac == 0x54321) good("AC was not modified by lio");
            else bad("AC was modified by lio");
            if (cpu.Io == 0x24680) good("IO was correctly modified by lio");
            else bad("IO was not correctly modified by lio");
            if (cpu.Memory[0x108] == 0x24680) good("Memory was not modified by lio");
            else bad("Memory was modified by lio");
        }

        private void dioTests()
        {
            results += "\r\ndio tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x54321;
            cpu.Io = 0x16420;
            cpu.Memory[0x000] = 0x1a109;
            cpu.Memory[0x109] = 0x24680;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 1) good("PC incremented by 1 after dio");
            else bad("PC not incremented by 1 after dio");
            if (cpu.Ac == 0x54321) good("AC was not modified by dio");
            else bad("AC was modified by dio");
            if (cpu.Io == 0x16420) good("IO was not modified by dio");
            else bad("IO was modified by dio");
            if (cpu.Memory[0x109] == 0x16420) good("Memory was correctly modified by dio");
            else bad("Memory was not correctly modified by dio");
        }

        private void dzmTests()
        {
            results += "\r\ndzm tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x54329;
            cpu.Io = 0x98642;
            cpu.Memory[0x000] = 0x1c10a;
            cpu.Memory[0x10a] = 0x24680;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 1) good("PC incremented by 1 after dzm");
            else bad("PC not incremented by 1 after dzm");
            if (cpu.Ac == 0x54329) good("AC was not modified by dzm");
            else bad("AC was modified by dzm");
            if (cpu.Memory[0x10a] == 0x00000) good("Memory was set to 0 by dzm");
            else bad("Memory was not set to 0 by dzm");
        }

        private void jmpTests()
        {
            results += "\r\njmp tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x54329;
            cpu.Io = 0x98642;
            cpu.Memory[0x000] = 0x3010b;
            cpu.Memory[0x10a] = 0x24680;
            cpu.cycle();
            if (cpu.Pc == 0x10b) good("PC was correctly set by jmp");
            else bad("PC was not correctly set by jmp");
            if (cpu.Ac == 0x54329) good("AC was not modified by jmp");
            else bad("AC was modified by jmp");
        }

        private void jspTests()
        {
            results += "\r\njsp tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x54329;
            cpu.Pc = 0x123;
            cpu.Memory[0x123] = 0x3210c;
            cpu.cycle();
            if (cpu.Pc == 0x10c) good("PC was correctly set by jsp");
            else bad("PC was not correctly set by jsp");
            if (cpu.Ac == 0x00124) good("AC address was properly set by jsp");
            else bad("AC address was not properly set by jsp");
            cpu.Memory[0x10c] = 0x32200;
            cpu.Overflow = true;
            cpu.cycle();
            if (cpu.Ac == 0x2010d) good("AC overflow was properly set by jsp");
            else bad("AC overflow was not properly set by jsp");
            cpu.Memory[0x200] = 0x32400;
            cpu.Overflow = false;
            cpu.Extend = true;
            cpu.cycle();
            if (cpu.Ac == 0x10201) good("AC extend was properly set by jsp");
            else bad("AC extend was not properly set by jsp");
            cpu.Memory[0xa400] = 0x32150;
            cpu.Overflow = false;
            cpu.Extend = false;
            cpu.Epc = 0xa << 12;
            cpu.cycle();
            if (cpu.Ac == 0x0a401) good("AC extended PC was properly set by jsp");
            else bad("AC extended PC was not properly set by jsp");
        }

        private void calTests()
        {
            results += "\r\ncal tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x54329;
            cpu.Pc = 0x123;
            cpu.Memory[0x123] = 0x0e10c;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 0x41) good("PC was correctly set by cal");
            else bad("PC was not correctly set by cal");
            if (cpu.Ac == 0x00124) good("AC address was properly set by cal");
            else bad("AC address was not properly set by cal");
            cpu.Pc = 0x10c;
            cpu.Memory[0x10c] = 0x0e200;
            cpu.Overflow = true;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x2010d) good("AC overflow was properly set by cal");
            else bad("AC overflow was not properly set by cal");
            cpu.Pc = 0x200;
            cpu.Memory[0x200] = 0x0e400;
            cpu.Overflow = false;
            cpu.Extend = true;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x10201) good("AC extend was properly set by cal");
            else bad("AC extend was not properly set by cal");
            cpu.Pc = 0x400;
            cpu.Memory[0xa400] = 0x0e150;
            cpu.Overflow = false;
            cpu.Extend = false;
            cpu.Epc = 0xa << 12;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x0a401) good("AC extended PC was properly set by cal");
            else bad("AC extended PC was not properly set by cal");
        }

        private void jdaTests()
        {
            results += "\r\njda tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x14329;
            cpu.Pc = 0x123;
            cpu.Memory[0x123] = 0x0f10c;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 0x10d) good("PC was correctly set by jda");
            else bad("PC was not correctly set by jda");
            if (cpu.Ac == 0x00124) good("AC address was properly set by jda");
            else bad("AC address was not properly set by jda");
            if (cpu.Memory[0x10c] == 0x14329) good("Memory was properly set by jda");
            else bad("Memory was not properly set by jda");
            cpu.Memory[0x10d] = 0x0f200;
            cpu.Overflow = true;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x2010e) good("AC overflow was properly set by jda");
            else bad("AC overflow was not properly set by jda");
            cpu.Memory[0x201] = 0x0f400;
            cpu.Overflow = false;
            cpu.Extend = true;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x10202) good("AC extend was properly set by jda");
            else bad("AC extend was not properly set by jda");
            cpu.Memory[0xa401] = 0x0f150;
            cpu.Overflow = false;
            cpu.Extend = false;
            cpu.Epc = 0xa << 12;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x0a402) good("AC extended PC was properly set by jda");
            else bad("AC extended PC was not properly set by jda");
        }

        private void sadTests()
        {
            results += "\r\nsad tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x0f10d;
            cpu.Pc = 0;
            cpu.Memory[0] = 0x28123;
            cpu.Memory[0x123] = 0x0f10c;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 2) good("PC correctly incremented when values differ for sad");
            else bad("PC not correctly incremented when values differ for sad");
            if (cpu.Ac == 0x0f10d) good("AC was not touched by sad");
            else bad("AC was touched by sad");
            if (cpu.Memory[0x123] == 0x0f10c) good("Memory was not affected by sad");
            else bad("Memory was affected by sad");
            cpu.Ac = 0x32198;
            cpu.Pc = 0;
            cpu.Memory[0] = 0x28124;
            cpu.Memory[0x124] = 0x32198;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 1) good("PC not incremented when values equal for sad");
            else bad("PC was incremented when values equal for sad");
            if (cpu.Ac == 0x32198) good("AC was not touched by sad");
            else bad("AC was touched by sad");
            if (cpu.Memory[0x124] == 0x32198) good("Memory was not affected by sad");
            else bad("Memory was affected by sad");
        }

        private void sasTests()
        {
            results += "\r\nsas tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x0f10d;
            cpu.Pc = 0;
            cpu.Memory[0] = 0x2a123;
            cpu.Memory[0x123] = 0x0f10c;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 1) good("PC not incremented when values differ for sas");
            else bad("PC was incremented when values differ for sas");
            if (cpu.Ac == 0x0f10d) good("AC was not touched by sas");
            else bad("AC was touched by sas");
            if (cpu.Memory[0x123] == 0x0f10c) good("Memory was not affected by sas");
            else bad("Memory was affected by sas");
            cpu.Ac = 0x32198;
            cpu.Pc = 0;
            cpu.Memory[0] = 0x2a124;
            cpu.Memory[0x124] = 0x32198;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 2) good("PC was incremented when values equal for sas");
            else bad("PC was not incremented when values equal for sas");
            if (cpu.Ac == 0x32198) good("AC was not touched by sas");
            else bad("AC was touched by sas");
            if (cpu.Memory[0x124] == 0x32198) good("Memory was not affected by sas");
            else bad("Memory was affected by sas");
        }

        private void excTests()
        {
            results += "\r\nexc tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x0;
            cpu.Pc = 0;
            cpu.Memory[0] = 0x08115;
            cpu.Memory[0x115] = 0x10125;
            cpu.Memory[0x125] = 0x13579;
            cpu.cycle();
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 1) good("PC incremented correctly by exc");
            else bad("PC was not incremented correctly by exc");
            if (cpu.Ac == 0x13579) good("lac properly executed by exc");
            else bad("lac not properly executed by exc");
            cpu.Pc = 0;
            cpu.Memory[0] = 0x08113;
            cpu.Memory[0x113] = 0x08213;
            cpu.Memory[0x213] = 0x08313;
            cpu.Memory[0x313] = 0x08413;
            cpu.Memory[0x413] = 0x10125;
            cpu.Memory[0x125] = 0x35791;
            cpu.cycle();
            cpu.cycle();
            cpu.cycle();
            cpu.cycle();
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 1) good("PC incremented correctly by 4 nested excs");
            else bad("PC was not incremented correctly by 4 nested excs");
            if (cpu.Ac == 0x35791) good("lac properly executed by 4 nested excs");
            else bad("lac not properly executed by 4 nested excs");
        }

        private void iorTests()
        {
            results += "\r\nior tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0xc;
            cpu.Pc = 0;
            cpu.Memory[0] = 0x04234;
            cpu.Memory[0x234] = 0xa;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 1) good("PC incremented correctly by ior");
            else bad("PC was not incremented correctly by ior");
            if (cpu.Ac == 0xe) good("ior worked correctly");
            else bad("ior did not work correctly");
            if (cpu.Memory[0x234] == 0xa) good("Memory was not modified by ior");
            else bad("Memory was modified by ior");
        }

        private void xorTests()
        {
            results += "\r\nxor tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0xa;
            cpu.Pc = 0;
            cpu.Memory[0] = 0x06237;
            cpu.Memory[0x237] = 0xc;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 1) good("PC incremented correctly by xor");
            else bad("PC was not incremented correctly by xor");
            if (cpu.Ac == 0x6) good("xor worked correctly");
            else bad("xor did not work correctly");
            if (cpu.Memory[0x237] == 0xc) good("Memory was not modified by xor");
            else bad("Memory was modified by xor");
        }

        private void andTests()
        {
            results += "\r\nand tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0xa;
            cpu.Pc = 0;
            cpu.Memory[0] = 0x02239;
            cpu.Memory[0x239] = 0xc;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 1) good("PC incremented correctly by and");
            else bad("PC was not incremented correctly by and");
            if (cpu.Ac == 0x8) good("and worked correctly");
            else bad("and did not work correctly");
            if (cpu.Memory[0x239] == 0xc) good("Memory was not modified by and");
            else bad("Memory was modified by and");
        }

        private void ispTests()
        {
            results += "\r\nisp tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0;
            cpu.Pc = 0;
            cpu.Memory[0] = 0x26165;
            cpu.Memory[0x165] = 0x3fffd;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 1) good("Skip did not occur on negative value after isp");
            else bad("Skip occurred on negative value after isp");
            if (cpu.Ac == 0x3fffe) good("isp correctly incremented -2");
            else bad("isp did not correctly increment -2");
            if (cpu.Memory[0x165] == 0x3fffe) good("Memory was updated by isp");
            else bad("Memory was not updated by isp");
            cpu.Memory[1] = 0x26166;
            cpu.Memory[0x166] = 0x3fffe;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 3) good("Skip occurred incrementing -1 on isp");
            else bad("Skip did not occur incrementing -1 on isp");
            if (cpu.Ac == 0x00000) good("isp correctly incremented -1 to +0");
            else bad("isp did not correctly increment -1 to +0");
            if (cpu.Memory[0x166] == 0x00000) good("Memory was updated by isp");
            else bad("Memory was not updated by isp");
            cpu.Memory[3] = 0x26167;
            cpu.Memory[0x167] = 123;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 5) good("Skip occurred incrementing +123 on isp");
            else bad("Skip did not occur incrementing +123 on isp");
            if (cpu.Ac == 124) good("isp correctly incremented +123");
            else bad("isp did not correctly increment +123");
            if (cpu.Memory[0x167] == 124) good("Memory was updated by isp");
            else bad("Memory was not updated by isp");
        }

        private void idxTests()
        {
            results += "\r\nidx tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0;
            cpu.Pc = 0;
            cpu.Memory[0] = 0x24165;
            cpu.Memory[0x165] = 0x3fffd;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 1) good("PC incremented correctly on idx");
            else bad("PC not incremented correctly on idx");
            if (cpu.Ac == 0x3fffe) good("idx correctly incremented -2");
            else bad("idx did not correctly increment -2");
            if (cpu.Memory[0x165] == 0x3fffe) good("Memory was updated by idx");
            else bad("Memory was not updated by idx");
            cpu.Memory[1] = 0x24166;
            cpu.Memory[0x166] = 0x3fffe;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 2) good("PC incremented correctly on idx");
            else bad("PC not incremented correctly on idx");
            if (cpu.Ac == 0x00000) good("idx correctly incremented -1 to +0");
            else bad("idx did not correctly increment -1 to +0");
            if (cpu.Memory[0x166] == 0x00000) good("Memory was updated by idx");
            else bad("Memory was not updated by idx");
            cpu.Memory[2] = 0x24167;
            cpu.Memory[0x167] = 123;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 3) good("PC incremented correctly on idx");
            else bad("PC not incremented correctly on idx");
            if (cpu.Ac == 124) good("idx correctly incremented +123");
            else bad("idx did not correctly increment +123");
            if (cpu.Memory[0x167] == 124) good("Memory was updated by idx");
            else bad("Memory was not updated by idx");
        }

        private void addTests()
        {
            results += "\r\nadd tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x00004;
            cpu.Memory[0x000] = 0x2010a;
            cpu.Memory[0x10a] = 0x00008;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 1) good("PC incremented by 1 after add");
            else bad("PC not incremented by 1 after add");
            if (cpu.Ac == 0x0000c) good("add +4 and +8 produced +12");
            else bad("add +4 and +8 did not produce +12");
            if (cpu.Overflow == false) good("add +4 and +8 did not produce overflow");
            else bad("add +4 and +8 produced overflow");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x00008;
            cpu.Memory[0x000] = 0x2010a;
            cpu.Memory[0x10a] = 0x3fffb;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x00004) good("add +8 and -4 produced +4");
            else bad("add +8 and -4 did not produce +4");
            if (cpu.Overflow == false) good("add +8 and -4 did not produce overflow");
            else bad("add +8 and -4 produced overflow");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x00004;
            cpu.Memory[0x000] = 0x2010a;
            cpu.Memory[0x10a] = 0x3fff7;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x3fffb) good("add +4 and -8 produced -4");
            else bad("add +4 and -8 did not produce -4");
            if (cpu.Overflow == false) good("add +4 and -8 did not produce overflow");
            else bad("add +r and -8 produced overflow");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x3fffb;
            cpu.Memory[0x000] = 0x2010a;
            cpu.Memory[0x10a] = 0x00008;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x00004) good("add -4 and +8 produced +4");
            else bad("add -4 and +8 did not produce +4");
            if (cpu.Overflow == false) good("add -4 and +8 did not produce overflow");
            else bad("add -4 and +8 produced overflow");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x3fff7;
            cpu.Memory[0x000] = 0x2010a;
            cpu.Memory[0x10a] = 0x00004;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x3fffb) good("add -8 and +4 produced -4");
            else bad("add -8 and +4 did not produce -4");
            if (cpu.Overflow == false) good("add -8 and +4 did not produce overflow");
            else bad("add -8 and +4 produced overflow");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x3fff7;
            cpu.Memory[0x000] = 0x2010a;
            cpu.Memory[0x10a] = 0x3fffb;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x3fff3) good("add -8 and -4 produced -12");
            else bad("add -8 and -4 did not produce -12");
            if (cpu.Overflow == false) good("add -8 and -4 did not produce overflow");
            else bad("add -8 and -4 produced overflow");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x3fffb;
            cpu.Memory[0x000] = 0x2010a;
            cpu.Memory[0x10a] = 0x3fff7;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x3fff3) good("add -4 and -8 produced -12");
            else bad("add -4 and -8 did not produce -12");
            if (cpu.Overflow == false) good("add -4 and -8 did not produce overflow");
            else bad("add -4 and -8 produced overflow");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x3fffb;
            cpu.Memory[0x000] = 0x2010a;
            cpu.Memory[0x10a] = 0x00004;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x00000) good("add -4 and +4 produced +0");
            else bad("add -4 and +4 did not produce +0");
            if (cpu.Overflow == false) good("add -4 and +4 did not produce overflow");
            else bad("add -4 and +4 produced overflow");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x1ffff;
            cpu.Memory[0x000] = 0x2010a;
            cpu.Memory[0x10a] = 0x00001;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x20000) good("add +131071 and +1 produced -131071");
            else bad("add +131071 and +1 did not produce -131071");
            if (cpu.Overflow == true) good("add +131071 and +1 produced overflow");
            else bad("add +131071 and +1 did not produce overflow");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x20000;
            cpu.Memory[0x000] = 0x2010a;
            cpu.Memory[0x10a] = 0x3fffe;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x1ffff) good("add -131071 and -1 produced +131071");
            else bad("add -131071 and -1 did not produce +131071");
            if (cpu.Overflow == true) good("add -131071 and -1 produced overflow");
            else bad("add -131071 and -1 did not produce overflow");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x00000;
            cpu.Memory[0x000] = 0x2010a;
            cpu.Memory[0x10a] = 0x3ffff;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x00000) good("add +0 and -0 produced +0");
            else bad("add +0 and -0 did not produce +0");
            if (cpu.Overflow == false) good("add +0 and -0 did not produce overflow");
            else bad("add +0 and -0 produced overflow");
        }

        private void subTests()
        {
            results += "\r\nsub tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x00004;
            cpu.Memory[0x000] = 0x2210a;
            cpu.Memory[0x10a] = 0x00008;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 1) good("PC incremented by 1 after sub");
            else bad("PC not incremented by 1 after sub");
            if (cpu.Ac == 0x3fffb) good("sub +4 and +8 produced -4");
            else bad("sub +4 and +8 did not produce -4");
            if (cpu.Overflow == false) good("sub +4 and +8 did not produce overflow");
            else bad("sub +4 and +8 produced overflow");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x00008;
            cpu.Memory[0x000] = 0x2210a;
            cpu.Memory[0x10a] = 0x3fffb;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x0000c) good("sub +8 and -4 produced +12");
            else bad("sub +8 and -4 did not produce +12");
            if (cpu.Overflow == false) good("sub +8 and -4 did not produce overflow");
            else bad("sub +8 and -4 produced overflow");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x00004;
            cpu.Memory[0x000] = 0x2210a;
            cpu.Memory[0x10a] = 0x3fff7;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x0000c) good("sub +4 and -8 produced +12");
            else bad("sub +4 and -8 did not produce +12");
            if (cpu.Overflow == false) good("sub +4 and -8 did not produce overflow");
            else bad("sub +r and -8 produced overflow");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x3fffb;
            cpu.Memory[0x000] = 0x2210a;
            cpu.Memory[0x10a] = 0x00008;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x3fff3) good("sub -4 and +8 produced -12");
            else bad("sub -4 and +8 did not produce -12");
            if (cpu.Overflow == false) good("sub -4 and +8 did not produce overflow");
            else bad("sub -4 and +8 produced overflow");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x3fff7;
            cpu.Memory[0x000] = 0x2210a;
            cpu.Memory[0x10a] = 0x00004;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x3fff3) good("sub -8 and +4 produced -12");
            else bad("sub -8 and +4 did not produce -12");
            if (cpu.Overflow == false) good("sub -8 and +4 did not produce overflow");
            else bad("sub -8 and +4 produced overflow");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x3fff7;
            cpu.Memory[0x000] = 0x2210a;
            cpu.Memory[0x10a] = 0x3fffb;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x3fffb) good("sub -8 and -4 produced -4");
            else bad("sub -8 and -4 did not produce -4");
            if (cpu.Overflow == false) good("sub -8 and -4 did not produce overflow");
            else bad("sub -8 and -4 produced overflow");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x3fffb;
            cpu.Memory[0x000] = 0x2210a;
            cpu.Memory[0x10a] = 0x3fff7;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x00004) good("sub -4 and -8 produced +4");
            else bad("sub -4 and -8 did not produce +4");
            if (cpu.Overflow == false) good("sub -4 and -8 did not produce overflow");
            else bad("sub -4 and -8 produced overflow");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x3fffb;
            cpu.Memory[0x000] = 0x2210a;
            cpu.Memory[0x10a] = 0x3fffb;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x00000) good("sub -4 and -4 produced +0");
            else bad("sub -4 and -4 did not produce +0");
            if (cpu.Overflow == false) good("sub -4 and -4 did not produce overflow");
            else bad("sub -4 and -4 produced overflow");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x1ffff;
            cpu.Memory[0x000] = 0x2210a;
            cpu.Memory[0x10a] = 0x3fffe;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x20000) good("sub +131071 and -1 produced -131071");
            else bad("sub +131071 and -1 did not produce -131071");
            if (cpu.Overflow == true) good("sub +131071 and -1 produced overflow");
            else bad("sub +131071 and -1 did not produce overflow");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x20000;
            cpu.Memory[0x000] = 0x2210a;
            cpu.Memory[0x10a] = 0x00001;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x1ffff) good("sub -131071 and +1 produced +131071");
            else bad("sub -131071 and +1 did not produce +131071");
            if (cpu.Overflow == true) good("sub -131071 and +1 produced overflow");
            else bad("sub -131071 and +1 did not produce overflow");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x00000;
            cpu.Memory[0x000] = 0x2210a;
            cpu.Memory[0x10a] = 0x3ffff;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x3ffff) good("sub +0 and -0 produced -0");
            else bad("sub +0 and -0 did not produce -0");
            if (cpu.Overflow == false) good("sub +0 and -0 did not produce overflow");
            else bad("sub +0 and -0 produced overflow");
        }

        private void mulTests()
        {
            results += "\r\nmul tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x10000;
            cpu.Memory[0x000] = 0x2c10a;
            cpu.Memory[0x10a] = 0x10000;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Pc == 1) good("PC incremented by 1 after mul");
            else bad("PC not incremented by 1 after mul");
            if (cpu.Ac == 0x08000) good("mul +.5 and +.5 produced +.25");
            else bad("mul +.5 and +.5 did not produce +.25");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x10000;
            cpu.Memory[0x000] = 0x2c10a;
            cpu.Memory[0x10a] = 0x08000;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x04000) good("mul +.5 and +.25 produced +.125");
            else bad("mul +.5 and +.25 did not produce +.125");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x10000;
            cpu.Memory[0x000] = 0x2c10a;
            cpu.Memory[0x10a] = 0x2ffff;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x37fff) good("mul +.5 and -.5 produced -.25");
            else bad("mul +.5 and -.5 did not produce -.25");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x2ffff;
            cpu.Memory[0x000] = 0x2c10a;
            cpu.Memory[0x10a] = 0x10000;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x37fff) good("mul -.5 and +.5 produced -.25");
            else bad("mul -.5 and +.5 did not produce -.25");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x2ffff;
            cpu.Memory[0x000] = 0x2c10a;
            cpu.Memory[0x10a] = 0x2ffff;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x08000) good("mul -.5 and -.5 produced +.25");
            else bad("mul -.5 and -.5 did not produce +.25");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x00004;
            cpu.Memory[0x000] = 0x2c10a;
            cpu.Memory[0x10a] = 0x00003;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x00000) good("integer mul +3 and +4 produced +0 in AC");
            else bad("integer mul +3 and +4 did not produce +0 in AC");
            if (cpu.Io == 0x00018) good("integer mul +3 and +4 produced +12 in IO");
            else bad("integer mul +3 and +4 did not produce +12 in IO");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x00004;
            cpu.Memory[0x000] = 0x2c10a;
            cpu.Memory[0x10a] = 0x3fffc;
            cpu.cycle();
            cpu.cycle();
            if (cpu.Ac == 0x3ffff) good("integer mul +4 and -3 produced -0 in AC");
            else bad("integer mul +4 and -3 did not produce +0 in AC");
            if (cpu.Io == 0x3ffe7) good("integer mul +4 and -3 produced -12 in IO");
            else bad("integer mul +4 and -3 did not produce -12 in IO");
        }

        private void lawTests()
        {
            results += "\r\nlaw tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x10000;
            cpu.Memory[0x000] = 0x3810a;
            cpu.Memory[0x10a] = 0x00000;
            cpu.cycle();
            if (cpu.Pc == 1) good("PC incremented by 1 after law");
            else bad("PC not incremented by 1 after law");
            if (cpu.Ac == 0x0010a) good("AC had 0x0010a after law 10a");
            else bad("AC did not have 0x0010a after law 10a");
            cpu.Memory[0x001] = 0x39456;
            cpu.cycle();
            if (cpu.Ac == 0x3fba9) good("AC had -0x456 after law -10a");
            else bad("AC did not have -0x456 after law -10a");
        }

        private void shiftGroupTests()
        {
            results += "\r\nshift group tests\r\n";
            info("rar");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x34567;
            cpu.Memory[0x000] = 0x3720f;
            cpu.cycle();
            if (cpu.Pc == 1) good("PC incremented by 1 after rar");
            else bad("PC not incremented by 1 after rar");
            if (cpu.Ac == 0x1f456) good("AC had correct value after shifting 4 places with rar");
            else bad("AC did not have correct value after shifting 4 places with rar");
            info("ral");
            cpu.Ac = 0x34567;
            cpu.Memory[0x001] = 0x3620f;
            cpu.cycle();
            if (cpu.Pc == 2) good("PC incremented by 1 after ral");
            else bad("PC not incremented by 1 after ral");
            if (cpu.Ac == 0x0567d) good("AC had correct value after shifting 4 places with ral");
            else bad("AC did not have correct value after shifting 4 places with ral");
            info("sar");
            cpu.Ac = 0x34567;
            cpu.Memory[0x002] = 0x37a0f;
            cpu.cycle();
            if (cpu.Pc == 3) good("PC incremented by 1 after sar");
            else bad("PC not incremented by 1 after sar");
            if (cpu.Ac == 0x3f456) good("AC had correct value after shifting 4 places with sar");
            else bad("AC did not have correct value after shifting 4 places with sar");
            info("sal");
            cpu.Ac = 0x34567;
            cpu.Memory[0x003] = 0x36a0f;
            cpu.cycle();
            if (cpu.Pc == 4) good("PC incremented by 1 after sal");
            else bad("PC not incremented by 1 after sal");
            if (cpu.Ac == 0x2567f) good("AC had correct value after shifting 4 places with sal");
            else bad("AC did not have correct value after shifting 4 places with sal");
            info("rir");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Io = 0x34567;
            cpu.Memory[0x000] = 0x3740f;
            cpu.cycle();
            if (cpu.Pc == 1) good("PC incremented by 1 after rir");
            else bad("PC not incremented by 1 after rir");
            if (cpu.Io == 0x1f456) good("IO had correct value after shifting 4 places with rir");
            else bad("IO did not have correct value after shifting 4 places with rir");
            info("ril");
            cpu.Io = 0x34567;
            cpu.Memory[0x001] = 0x3640f;
            cpu.cycle();
            if (cpu.Pc == 2) good("PC incremented by 1 after ril");
            else bad("PC not incremented by 1 after ril");
            if (cpu.Io == 0x0567d) good("IO had correct value after shifting 4 places with ril");
            else bad("IO did not have correct value after shifting 4 places with ril");
            info("sir");
            cpu.Io = 0x34567;
            cpu.Memory[0x002] = 0x37c0f;
            cpu.cycle();
            if (cpu.Pc == 3) good("PC incremented by 1 after sir");
            else bad("PC not incremented by 1 after sir");
            if (cpu.Io == 0x3f456) good("IO had correct value after shifting 4 places with sir");
            else bad("IO did not have correct value after shifting 4 places with sir");
            info("sil");
            cpu.Io = 0x34567;
            cpu.Memory[0x003] = 0x36c0f;
            cpu.cycle();
            if (cpu.Pc == 4) good("PC incremented by 1 after sil");
            else bad("PC not incremented by 1 after sil");
            if (cpu.Io == 0x2567f) good("IO had correct value after shifting 4 places with sil");
            else bad("IO did not have correct value after shifting 4 places with sil");

            info("rcr");
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x34567;
            cpu.Io = 0x29012;
            cpu.Memory[0x000] = 0x3760f;
            cpu.cycle();
            if (cpu.Pc == 1) good("PC incremented by 1 after rcr");
            else bad("PC not incremented by 1 after rcr");
            if (cpu.Ac == 0x0b456) good("AC had correct value after shifting 4 places with rcr");
            else bad("AC did not have correct value after shifting 4 places with rcr");
            if (cpu.Io == 0x1e901) good("IO had correct value after shifting 4 places with rcr");
            else bad("IO did not have correct value after shifting 4 places with rcr");
            info("rcl");
            cpu.Ac = 0x34567;
            cpu.Io = 0x29012;
            cpu.Memory[0x001] = 0x3660f;
            cpu.cycle();
            if (cpu.Pc == 2) good("PC incremented by 1 after rcl");
            else bad("PC not incremented by 1 after rcl");
            if (cpu.Ac == 0x0567a) good("AC had correct value after shifting 4 places with rcl");
            else bad("AC did not have correct value after shifting 4 places with rcl");
            if (cpu.Io == 0x1012d) good("IO had correct value after shifting 4 places with rcl");
            else bad("IO did not have correct value after shifting 4 places with rcl");
            info("scr");
            cpu.Ac = 0x34567;
            cpu.Io = 0x29012;
            cpu.Memory[0x002] = 0x37e0f;
            cpu.cycle();
            if (cpu.Pc == 3) good("PC incremented by 1 after scr");
            else bad("PC not incremented by 1 after scr");
            if (cpu.Ac == 0x3f456) good("AC had correct value after shifting 4 places with scr");
            else bad("AC did not have correct value after shifting 4 places with scr");
            if (cpu.Io == 0x1e901) good("IO had correct value after shifting 4 places with scr");
            else bad("IO did not have correct value after shifting 4 places with scr");
            info("scl");
            cpu.Ac = 0x34567;
            cpu.Io = 0x29012;
            cpu.Memory[0x003] = 0x36e0f;
            cpu.cycle();
            if (cpu.Pc == 4) good("PC incremented by 1 after scl");
            else bad("PC not incremented by 1 after scl");
            if (cpu.Ac == 0x2567a) good("AC had correct value after shifting 4 places with scl");
            else bad("AC did not have correct value after shifting 4 places with scl");
            if (cpu.Io == 0x1012f) good("IO had correct value after shifting 4 places with scl");
            else bad("IO did not have correct value after shifting 4 places with scl");
        }

        private void skipGroupTests()
        {
            results += "\r\nskip group tests\r\n";
            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x00000;
            cpu.Memory[0x000] = 0x34040;
            cpu.cycle();
            if (cpu.Pc == 2) good("PC correctly skipped when ac was zero on sza");
            else bad("PC did not correctly skip when ac was zero on sza");
            cpu.Ac = 0x00002;
            cpu.Memory[0x002] = 0x34040;
            cpu.cycle();
            if (cpu.Pc == 3) good("PC did not skip on nonzero ac on sza");
            else bad("PC skipped on nonzero ac on sza");

            cpu.Ac = 0x00002;
            cpu.Memory[0x003] = 0x34080;
            cpu.cycle();
            if (cpu.Pc == 5) good("PC skipped on positive number in ac on spa");
            else bad("PC did not skip on positive number in ac on spa");
            cpu.Ac = 0x00000;
            cpu.Memory[0x005] = 0x34080;
            cpu.cycle();
            if (cpu.Pc == 7) good("PC skipped on positive zero in ac on spa");
            else bad("PC did not skip on positive zero in ac on spa");
            cpu.Ac = 0x3ffff;
            cpu.Memory[0x007] = 0x34080;
            cpu.cycle();
            if (cpu.Pc == 8) good("PC did not skip on negative zero in ac on spa");
            else bad("PC skipped on negative zero in ac on spa");
            cpu.Ac = 0x3fffe;
            cpu.Memory[0x008] = 0x34080;
            cpu.cycle();
            if (cpu.Pc == 9) good("PC did not skip on negative number in ac on spa");
            else bad("PC skipped on negative number in ac on spa");

            cpu.Ac = 0x00002;
            cpu.Memory[0x009] = 0x34100;
            cpu.cycle();
            if (cpu.Pc == 0x00a) good("PC did not skip on positive number in ac on sma");
            else bad("PC skipped on positive number in ac on sma");
            cpu.Ac = 0x00000;
            cpu.Memory[0x00a] = 0x34100;
            cpu.cycle();
            if (cpu.Pc == 0x00b) good("PC did not skip on positive zero in ac on sma");
            else bad("PC skipped on positive zero in ac on sma");
            cpu.Ac = 0x3ffff;
            cpu.Memory[0x00b] = 0x34100;
            cpu.cycle();
            if (cpu.Pc == 0x00d) good("PC skipped on negative zero in ac on sma");
            else bad("PC did not skipp on negative zero in ac on sma");
            cpu.Ac = 0x3fffe;
            cpu.Memory[0x00d] = 0x34100;
            cpu.cycle();
            if (cpu.Pc == 0x00f) good("PC skipped on negative number in ac on sma");
            else bad("PC did not skipp on negative number in ac on sma");

            cpu.reset();
            cpu.Stopped = false;
            cpu.Ac = 0x00000;
            cpu.Overflow = false;
            cpu.Memory[0x000] = 0x34200;
            cpu.cycle();
            if (cpu.Pc == 2) good("PC skipped when no overflow on szo");
            else bad("PC did not skip when no overflow on szo");
            cpu.Overflow = true;
            cpu.Memory[0x002] = 0x34200;
            cpu.cycle();
            if (cpu.Pc == 3) good("PC did not skip when overflow on szo");
            else bad("PC skipped when overflow on szo");
            cpu.Overflow = false;
            cpu.Memory[0x003] = 0x35200;
            cpu.cycle();
            if (cpu.Pc == 0x004) good("PC did not skipped no overflow on inverse szo");
            else bad("PC skipped when no overflow on inverse szo");
            cpu.Overflow = true;
            cpu.Memory[0x004] = 0x35200;
            cpu.cycle();
            if (cpu.Pc == 0x006) good("PC skipped when overflow on inverse szo");
            else bad("PC did not skip when overflow on inverse szo");


            cpu.reset();
            cpu.Stopped = false;
            cpu.Pc = 0x003;
            cpu.Ac = 0x3ffff;
            cpu.Memory[0x003] = 0x34400;
            cpu.cycle();
            if (cpu.Pc == 5) good("PC skipped on positive number in io on spi");
            else bad("PC did not skip on positive number in io on spi");
            cpu.Io = 0x00000;
            cpu.Memory[0x005] = 0x34400;
            cpu.cycle();
            if (cpu.Pc == 7) good("PC skipped on positive zero in io on spi");
            else bad("PC did not skip on positive zero in io on spi");
            cpu.Io = 0x3ffff;
            cpu.Ac = 0x00000;
            cpu.Memory[0x007] = 0x34400;
            cpu.cycle();
            if (cpu.Pc == 8) good("PC did not skip on negative zero in io on spi");
            else bad("PC skipped on negative zero in io on spi");
            cpu.Io = 0x3fffe;
            cpu.Memory[0x008] = 0x34400;
            cpu.cycle();
            if (cpu.Pc == 9) good("PC did not skip on negative number in io on spi");
            else bad("PC skipped on negative number in io on spi");
        }

        public String run()
        {
            goodTests = 0;
            badTests = 0;
            results = "";
            lacTests();
            dacTests();
            dapTests();
            dipTests();
            lioTests();
            dioTests();
            dzmTests();
            jmpTests();
            jspTests();
            calTests();
            jdaTests();
            sadTests();
            sasTests();
            excTests();
            iorTests();
            xorTests();
            andTests();
            ispTests();
            idxTests();
            addTests();
            subTests();
            mulTests();
            lawTests();
            shiftGroupTests();
            skipGroupTests();

            results += "\r\n";
            results += "Good tests: " + goodTests.ToString() + "\r\n";
            results += "Bad tests : " + badTests.ToString() + "\r\n";
            return results;
        }

    }
}

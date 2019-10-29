using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace PDP1
{
    public partial class Form1 : Form
    {
        Cpu cpu;
        Thread cpuThread;
        Assembler assembler;
        Boolean[] lastFlagLamps;
        UInt32 lastDInst;
        UInt32 lastIo;
        UInt32 lastAc;
        UInt32 lastPc;
        UInt32 lastMemoryBuffer;
        UInt32 lastMemoryAddress;
        Boolean lastStopped;
        Boolean lastOverflow;
        Boolean lastExtend;
        Boolean lastCycleFF;
        Boolean lastDeferFF;
        Boolean lastHsCycleFF;
        Boolean lastBrkCtr1FF;
        Boolean lastBrkCtr2FF;
        Boolean lastReadInFF;
        Boolean lastSeqBreakFF;
        Boolean lastIoHaltFF;
        Boolean lastIoCommandsFF;
        Boolean lastIoSyncFF;
        static char numberMode;
        Boolean updating;

        public Form1()
        {
            int i;
            Font = new Font(Font.Name, 8.25f * 96f / CreateGraphics().DpiX, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
            InitializeComponent();
            cpu = new Cpu();
            cpuThread = new Thread(cpu.Run);
            cpuThread.Start();
            assembler = new Assembler(cpu);
            lastFlagLamps = new Boolean[7];
            for (i = 0; i < 7; i++) lastFlagLamps[i] = true;
            tapeReaderMountButton.Enabled = false;
            tapeReaderUnmountButton.Enabled = false;
            tapeReaderRewindButton.Enabled = false;
            tapePunchMountButton.Enabled = false;
            tapePunchUnmountButton.Enabled = false;
            lastDInst = 0xfffff;
            lastIo = 0xfffff;
            lastAc = 0xfffff;
            lastPc = 0xfffff;
            lastMemoryBuffer = 0xfffff;
            lastMemoryAddress = 0xfffff;
            lastStopped = false;
            lastOverflow = true;
            lastExtend = true;
            lastCycleFF = true;
            lastDeferFF = true;
            lastHsCycleFF = true;
            lastBrkCtr1FF = true;
            lastBrkCtr2FF = true;
            lastReadInFF = true;
            lastSeqBreakFF = true;
            lastIoHaltFF = true;
            lastIoCommandsFF = true;
            lastIoSyncFF = true;
            numberMode = 'O';
            updating = true;
            typewriterOutputIRQ.SelectedIndex = (cpu.Terminal.OutputIrq >= 0) ? cpu.Terminal.OutputIrq+1 : 0;
            keyReadyIRQ.SelectedIndex = (cpu.Terminal.InputIrq >= 0) ? cpu.Terminal.InputIrq+1 : 0;
            tapeReaderIrq.SelectedIndex = (cpu.TapeReader.Irq >= 0) ? cpu.TapeReader.Irq+1 : 0;
            tapePunchIrq.SelectedIndex = (cpu.TapePunch.Irq >= 0) ? cpu.TapePunch.Irq+1 : 0;
            linePrinterIrq.SelectedIndex = (cpu.LinePrinter.Irq >= 0) ? cpu.LinePrinter.Irq+1 : 0;
            type51WP1.Checked = cpu.Type51TapeControl.transport(0).WriteProtected;
            type51WP2.Checked = cpu.Type51TapeControl.transport(1).WriteProtected;
            type51WP3.Checked = cpu.Type51TapeControl.transport(2).WriteProtected;
            type51Transport1.Checked = true;
            if (cpu.SequenceBreak120) type120IRQ.Checked = true; else standardIRQ.Checked = true;
            powerSwitchImage.Image = images46x24.Images[2];
            singleStepSwitch.Image = images46x24.Images[2];
            singleInstSwitch.Image = images46x24.Images[2];
            powerLamp.Image = images24x24.Images[0];
            singleStepLamp.Image = images24x24.Images[0];
            singleInstLamp.Image = images24x24.Images[0];
            testWordSwitch1.Image = images24x46.Images[2];
            senseSwitch1.Image = images24x46.Images[2];
            senseSwitch2.Image = images24x46.Images[2];
            senseSwitch3.Image = images24x46.Images[2];
            senseSwitch4.Image = images24x46.Images[2];
            senseSwitch5.Image = images24x46.Images[2];
            senseSwitch6.Image = images24x46.Images[2];
            senseLamp1.Image = images24x24.Images[0];
            senseLamp2.Image = images24x24.Images[0];
            senseLamp3.Image = images24x24.Images[0];
            senseLamp4.Image = images24x24.Images[0];
            senseLamp5.Image = images24x24.Images[0];
            senseLamp6.Image = images24x24.Images[0];
            testWordSwitch1.Image = images24x46.Images[2];
            testWordSwitch2.Image = images24x46.Images[2];
            testWordSwitch3.Image = images24x46.Images[2];
            testWordSwitch4.Image = images24x46.Images[2];
            testWordSwitch5.Image = images24x46.Images[2];
            testWordSwitch6.Image = images24x46.Images[2];
            testWordSwitch7.Image = images24x46.Images[2];
            testWordSwitch8.Image = images24x46.Images[2];
            testWordSwitch9.Image = images24x46.Images[2];
            testWordSwitch10.Image = images24x46.Images[2];
            testWordSwitch11.Image = images24x46.Images[2];
            testWordSwitch12.Image = images24x46.Images[2];
            testWordSwitch13.Image = images24x46.Images[2];
            testWordSwitch14.Image = images24x46.Images[2];
            testWordSwitch15.Image = images24x46.Images[2];
            testWordSwitch16.Image = images24x46.Images[2];
            testWordSwitch17.Image = images24x46.Images[2];
            testWordSwitch18.Image = images24x46.Images[2];
            addressSwitch1.Image = images24x46.Images[2];
            addressSwitch2.Image = images24x46.Images[2];
            addressSwitch3.Image = images24x46.Images[2];
            addressSwitch4.Image = images24x46.Images[2];
            addressSwitch5.Image = images24x46.Images[2];
            addressSwitch6.Image = images24x46.Images[2];
            addressSwitch7.Image = images24x46.Images[2];
            addressSwitch8.Image = images24x46.Images[2];
            addressSwitch9.Image = images24x46.Images[2];
            addressSwitch10.Image = images24x46.Images[2];
            addressSwitch11.Image = images24x46.Images[2];
            addressSwitch12.Image = images24x46.Images[2];
            addressSwitch13.Image = images24x46.Images[2];
            addressSwitch14.Image = images24x46.Images[2];
            addressSwitch15.Image = images24x46.Images[2];
            addressSwitch16.Image = images24x46.Images[2];
            extendSwitch.Image = images24x46.Images[2];
            startSwitch.Image = images24x46.Images[1];
            stopSwitch.Image = images24x46.Images[1];
            continueSwitch.Image = images24x46.Images[1];
            examineSwitch.Image = images24x46.Images[1];
            depositSwitch.Image = images24x46.Images[1];
            readInSwitch.Image = images24x46.Images[1];
            readerSwitch.Image = images24x46.Images[1];
            tapeFeedSwitch.Image = images24x46.Images[1];
            type52MTCUSelect.SelectedIndex = 0;
            updatePanel();
            updateType51();
            updateType52(true);
            type52Transport1.Checked = true;

            updating = false;
        }

        private static String hexChar(UInt32 value)
        {
            return (value & 0xf).ToString("x1");
        }

        private static String toHex(UInt32 value, int size)
        {
            int i;
            String ret;
            ret = "";
            for (i = 0; i < size; i++)
            {
                ret = hexChar(value & 0xf) + ret;
                value >>= 4;
            }
            return ret;
        }

        private static String toOctal(UInt32 value, int size)
        {
            int i;
            String ret;
            ret = "";
            for (i = 0; i < size; i++)
            {
                ret = ((char)((value & 7) + '0')).ToString() + ret;
                value >>= 3;
            }
            return ret;
        }

        public static String convert(UInt32 value, int bits)
        {
            int size;
            switch (numberMode)
            {
                case 'O':
                    size = bits / 3;
                    if (size * 3 < bits) size++;
                    return toOctal(value, size);
                case 'H':
                    size = bits / 4;
                    if (size * 4 < bits) size++;
                    return toHex(value, size);
            }
            return "";
        }

        private void updatePanel()
        {
            Boolean flag;
            uint tmpInt;
            if (cpu.Flags[0] != lastFlagLamps[0])
            {
                lastFlagLamps[0] = cpu.Flags[0];
                flagsLamp1.Image = (lastFlagLamps[0]) ? images24x24.Images[1] : images24x24.Images[0];
            }
            if (cpu.Flags[1] != lastFlagLamps[1])
            {
                lastFlagLamps[1] = cpu.Flags[1];
                flagsLamp2.Image = (lastFlagLamps[1]) ? images24x24.Images[1] : images24x24.Images[0];
            }
            if (cpu.Flags[2] != lastFlagLamps[2])
            {
                lastFlagLamps[2] = cpu.Flags[2];
                flagsLamp3.Image = (lastFlagLamps[2]) ? images24x24.Images[1] : images24x24.Images[0];
            }
            if (cpu.Flags[3] != lastFlagLamps[3])
            {
                lastFlagLamps[3] = cpu.Flags[3];
                flagsLamp4.Image = (lastFlagLamps[3]) ? images24x24.Images[1] : images24x24.Images[0];
            }
            if (cpu.Flags[4] != lastFlagLamps[4])
            {
                lastFlagLamps[4] = cpu.Flags[4];
                flagsLamp5.Image = (lastFlagLamps[4]) ? images24x24.Images[1] : images24x24.Images[0];
            }
            if (cpu.Flags[5] != lastFlagLamps[5])
            {
                lastFlagLamps[5] = cpu.Flags[5];
                flagsLamp6.Image = (lastFlagLamps[5]) ? images24x24.Images[1] : images24x24.Images[0];
            }
            tmpInt = cpu.DInst;
            if (tmpInt != lastDInst)
            {
                flag = false;
                if ((tmpInt & 16) != (lastDInst & 16))
                {
                    instLamp1.Image = ((tmpInt & 16) == 16) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 8) != (lastDInst & 8))
                {
                    instLamp2.Image = ((tmpInt & 8) == 8) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 4) != (lastDInst & 4))
                {
                    instLamp3.Image = ((tmpInt & 4) == 4) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 2) != (lastDInst & 2))
                {
                    instLamp4.Image = ((tmpInt & 2) == 2) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 1) != (lastDInst & 1))
                {
                    instLamp5.Image = ((tmpInt & 1) == 1) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if (flag) lastDInst = tmpInt;
            }
            tmpInt = cpu.Io;
            if (tmpInt != lastIo)
            {
                flag = false;
                if ((tmpInt & 1) != (lastIo & 1))
                {
                    inOutLamp18.Image = ((tmpInt & 1) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 2) != (lastIo & 2))
                {
                    inOutLamp17.Image = ((tmpInt & 2) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 4) != (lastIo & 4))
                {
                    inOutLamp16.Image = ((tmpInt & 4) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 8) != (lastIo & 8))
                {
                    inOutLamp15.Image = ((tmpInt & 8) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 16) != (lastIo & 16))
                {
                    inOutLamp14.Image = ((tmpInt & 16) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 32) != (lastIo & 32))
                {
                    inOutLamp13.Image = ((tmpInt & 32) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 64) != (lastIo & 64))
                {
                    inOutLamp12.Image = ((tmpInt & 64) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 128) != (lastIo & 128))
                {
                    inOutLamp11.Image = ((tmpInt & 128) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 256) != (lastIo & 256))
                {
                    inOutLamp10.Image = ((tmpInt & 256) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 512) != (lastIo & 512))
                {
                    inOutLamp9.Image = ((tmpInt & 512) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 1024) != (lastIo & 1024))
                {
                    inOutLamp8.Image = ((tmpInt & 1024) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 2048) != (lastIo & 2048))
                {
                    inOutLamp7.Image = ((tmpInt & 2048) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 4096) != (lastIo & 4096))
                {
                    inOutLamp6.Image = ((tmpInt & 4096) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 8192) != (lastIo & 8192))
                {
                    inOutLamp5.Image = ((tmpInt & 8192) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 16384) != (lastIo & 18384))
                {
                    inOutLamp4.Image = ((tmpInt & 16384) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 32768) != (lastIo & 32768))
                {
                    inOutLamp3.Image = ((tmpInt & 32768) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 65536) != (lastIo & 65536))
                {
                    inOutLamp2.Image = ((tmpInt & 65536) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 131072) != (lastIo & 131072))
                {
                    inOutLamp1.Image = ((tmpInt & 131072) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if (flag) lastIo = tmpInt;
            }
            tmpInt = cpu.Ac;
            if (tmpInt != lastAc)
            {
                flag = false;
                if ((tmpInt & 1) != (lastAc & 1))
                {
                    accLamp18.Image = ((tmpInt & 1) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 2) != (lastAc & 2))
                {
                    accLamp17.Image = ((tmpInt & 2) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 4) != (lastAc & 4))
                {
                    accLamp16.Image = ((tmpInt & 4) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 8) != (lastAc & 8))
                {
                    accLamp15.Image = ((tmpInt & 8) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 16) != (lastAc & 16))
                {
                    accLamp14.Image = ((tmpInt & 16) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 32) != (lastAc & 32))
                {
                    accLamp13.Image = ((tmpInt & 32) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 64) != (lastAc & 64))
                {
                    accLamp12.Image = ((tmpInt & 64) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 128) != (lastAc & 128))
                {
                    accLamp11.Image = ((tmpInt & 128) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 256) != (lastAc & 256))
                {
                    accLamp10.Image = ((tmpInt & 256) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 512) != (lastAc & 512))
                {
                    accLamp9.Image = ((tmpInt & 512) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 1024) != (lastAc & 1024))
                {
                    accLamp8.Image = ((tmpInt & 1024) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 2048) != (lastAc & 2048))
                {
                    accLamp7.Image = ((tmpInt & 2048) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 4096) != (lastAc & 4096))
                {
                    accLamp6.Image = ((tmpInt & 4096) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 8192) != (lastAc & 8192))
                {
                    accLamp5.Image = ((tmpInt & 8192) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 16384) != (lastAc & 16384))
                {
                    accLamp4.Image = ((tmpInt & 16384) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 32768) != (lastAc & 32768))
                {
                    accLamp3.Image = ((tmpInt & 32768) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 65536) != (lastAc & 65536))
                {
                    accLamp2.Image = ((tmpInt & 65536) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 131072) != (lastAc & 131072))
                {
                    accLamp1.Image = ((tmpInt & 131072) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if (flag) lastAc = tmpInt;
            }
            tmpInt = cpu.MemoryBuffer;
            if (tmpInt != lastMemoryBuffer)
            {
                flag = false;
                if ((tmpInt & 1) != (lastMemoryBuffer & 1))
                {
                    memoryBufferLamp18.Image = ((tmpInt & 1) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 2) != (lastMemoryBuffer & 2))
                {
                    memoryBufferLamp17.Image = ((tmpInt & 2) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 4) != (lastMemoryBuffer & 4))
                {
                    memoryBufferLamp16.Image = ((tmpInt & 4) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 8) != (lastMemoryBuffer & 8))
                {
                    memoryBufferLamp15.Image = ((tmpInt & 8) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 16) != (lastMemoryBuffer & 16))
                {
                    memoryBufferLamp14.Image = ((tmpInt & 16) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 32) != (lastMemoryBuffer & 32))
                {
                    memoryBufferLamp13.Image = ((tmpInt & 32) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 64) != (lastMemoryBuffer & 64))
                {
                    memoryBufferLamp12.Image = ((tmpInt & 64) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 128) != (lastMemoryBuffer & 128))
                {
                    memoryBufferLamp11.Image = ((tmpInt & 128) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 256) != (lastMemoryBuffer & 256))
                {
                    memoryBufferLamp10.Image = ((tmpInt & 256) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 512) != (lastMemoryBuffer & 512))
                {
                    memoryBufferLamp9.Image = ((tmpInt & 512) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 1024) != (lastMemoryBuffer & 1024))
                {
                    memoryBufferLamp8.Image = ((tmpInt & 1024) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 2048) != (lastMemoryBuffer & 2048))
                {
                    memoryBufferLamp7.Image = ((tmpInt & 2048) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 4096) != (lastMemoryBuffer & 4096))
                {
                    memoryBufferLamp6.Image = ((tmpInt & 4096) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 8192) != (lastMemoryBuffer & 8192))
                {
                    memoryBufferLamp5.Image = ((tmpInt & 8192) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 16384) != (lastMemoryBuffer & 16384))
                {
                    memoryBufferLamp4.Image = ((tmpInt & 16384) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 32768) != (lastMemoryBuffer & 32768))
                {
                    memoryBufferLamp3.Image = ((tmpInt & 32768) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 65536) != (lastMemoryBuffer & 65536))
                {
                    memoryBufferLamp2.Image = ((tmpInt & 65536) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 131072) != (lastMemoryBuffer & 131072))
                {
                    memoryBufferLamp1.Image = ((tmpInt & 131072) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if (flag) lastMemoryBuffer = tmpInt;
            }
            tmpInt = cpu.MemoryAddress;
            if (tmpInt != lastMemoryAddress)
            {
                flag = false;
                if ((tmpInt & 1) != (lastMemoryAddress & 1))
                {
                    memoryAddressLamp18.Image = ((tmpInt & 1) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 2) != (lastMemoryAddress & 2))
                {
                    memoryAddressLamp17.Image = ((tmpInt & 2) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 4) != (lastMemoryAddress & 4))
                {
                    memoryAddressLamp16.Image = ((tmpInt & 4) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 8) != (lastMemoryAddress & 8))
                {
                    memoryAddressLamp15.Image = ((tmpInt & 8) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 16) != (lastMemoryAddress & 16))
                {
                    memoryAddressLamp14.Image = ((tmpInt & 16) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 32) != (lastMemoryAddress & 32))
                {
                    memoryAddressLamp13.Image = ((tmpInt & 32) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 64) != (lastMemoryAddress & 64))
                {
                    memoryAddressLamp12.Image = ((tmpInt & 64) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 128) != (lastMemoryAddress & 128))
                {
                    memoryAddressLamp11.Image = ((tmpInt & 128) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 256) != (lastMemoryAddress & 256))
                {
                    memoryAddressLamp10.Image = ((tmpInt & 256) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 512) != (lastMemoryAddress & 512))
                {
                    memoryAddressLamp9.Image = ((tmpInt & 512) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 1024) != (lastMemoryAddress & 1024))
                {
                    memoryAddressLamp8.Image = ((tmpInt & 1024) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 2048) != (lastMemoryAddress & 2048))
                {
                    memoryAddressLamp7.Image = ((tmpInt & 2048) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 4096) != (lastMemoryAddress & 4096))
                {
                    memoryAddressLamp6.Image = ((tmpInt & 4096) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 8192) != (lastMemoryAddress & 8192))
                {
                    memoryAddressLamp5.Image = ((tmpInt & 8192) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 16384) != (lastMemoryAddress & 16384))
                {
                    memoryAddressLamp4.Image = ((tmpInt & 16384) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 32768) != (lastMemoryAddress & 32768))
                {
                    memoryAddressLamp3.Image = ((tmpInt & 32768) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if (flag) lastMemoryAddress = tmpInt;
            }
            tmpInt = cpu.Pc;
            if (tmpInt != lastPc)
            {
                flag = false;
                if ((tmpInt & 1) != (lastPc & 1))
                {
                    pcLamp18.Image = ((tmpInt & 1) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 2) != (lastPc & 2))
                {
                    pcLamp17.Image = ((tmpInt & 2) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 4) != (lastPc & 4))
                {
                    pcLamp16.Image = ((tmpInt & 4) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 8) != (lastPc & 8))
                {
                    pcLamp15.Image = ((tmpInt & 8) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 16) != (lastPc & 16))
                {
                    pcLamp14.Image = ((tmpInt & 16) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 32) != (lastPc & 32))
                {
                    pcLamp13.Image = ((tmpInt & 32) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 64) != (lastPc & 64))
                {
                    pcLamp12.Image = ((tmpInt & 64) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 128) != (lastPc & 128))
                {
                    pcLamp11.Image = ((tmpInt & 128) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 256) != (lastPc & 256))
                {
                    pcLamp10.Image = ((tmpInt & 256) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 512) != (lastPc & 512))
                {
                    pcLamp9.Image = ((tmpInt & 512) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 1024) != (lastPc & 1024))
                {
                    pcLamp8.Image = ((tmpInt & 1024) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 2048) != (lastPc & 2048))
                {
                    pcLamp7.Image = ((tmpInt & 2048) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 4096) != (lastPc & 4096))
                {
                    pcLamp6.Image = ((tmpInt & 4096) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 8192) != (lastPc & 8192))
                {
                    pcLamp5.Image = ((tmpInt & 8192) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 16384) != (lastPc & 16384))
                {
                    pcLamp4.Image = ((tmpInt & 16384) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if ((tmpInt & 32768) != (lastPc & 32768))
                {
                    pcLamp3.Image = ((tmpInt & 32768) != 0) ? images24x24.Images[1] : images24x24.Images[0];
                    flag = true;
                }
                if (flag) lastPc = tmpInt;
            }
            if (cpu.Stopped != lastStopped)
            {
                lastStopped = cpu.Stopped;
                runLamp.Image = (lastStopped) ? images24x24.Images[0] : images24x24.Images[1];
            }
            if (cpu.Overflow != lastOverflow)
            {
                lastOverflow = cpu.Overflow;
                overflowLamp.Image = (lastOverflow) ? images24x24.Images[1] : images24x24.Images[0];
            }
            if (cpu.Extend != lastExtend)
            {
                lastExtend = cpu.Extend;
                extendLamp.Image = (lastExtend) ? images24x24.Images[1] : images24x24.Images[0];
            }
            if (cpu.CycleFF != lastCycleFF)
            {
                lastCycleFF = cpu.CycleFF;
                cycleLamp.Image = (lastCycleFF) ? images24x24.Images[1] : images24x24.Images[0];
            }
            if (cpu.DeferFF != lastDeferFF)
            {
                lastDeferFF = cpu.DeferFF;
                deferLamp.Image = (lastDeferFF) ? images24x24.Images[1] : images24x24.Images[0];
            }
            if (cpu.HsCycleFF != lastHsCycleFF)
            {
                lastHsCycleFF = cpu.HsCycleFF;
                hsCycleLamp.Image = (lastHsCycleFF) ? images24x24.Images[1] : images24x24.Images[0];
            }
            if (cpu.BrkCtr1FF != lastBrkCtr1FF)
            {
                lastBrkCtr1FF = cpu.BrkCtr1FF;
                brkCtr1Lamp.Image = (lastBrkCtr1FF) ? images24x24.Images[1] : images24x24.Images[0];
            }
            if (cpu.BrkCtr2FF != lastBrkCtr2FF)
            {
                lastBrkCtr2FF = cpu.BrkCtr2FF;
                brkCtr2Lamp.Image = (lastBrkCtr2FF) ? images24x24.Images[1] : images24x24.Images[0];
            }
            if (cpu.ReadInFF != lastReadInFF)
            {
                lastReadInFF = cpu.ReadInFF;
                readInLamp.Image = (lastReadInFF) ? images24x24.Images[1] : images24x24.Images[0];
            }
            if (cpu.SeqBreakFF != lastSeqBreakFF)
            {
                lastSeqBreakFF = cpu.SeqBreakFF;
                seqBreakLamp.Image = (lastSeqBreakFF) ? images24x24.Images[1] : images24x24.Images[0];
            }
            if (cpu.IoHaltFF != lastIoHaltFF)
            {
                lastIoHaltFF = cpu.IoHaltFF;
                ioHaltLamp.Image = (lastIoHaltFF) ? images24x24.Images[1] : images24x24.Images[0];
            }
            if (cpu.IoCommandsFF != lastIoCommandsFF)
            {
                lastIoCommandsFF = cpu.IoCommandsFF;
                ioCommandsLamp.Image = (lastIoCommandsFF) ? images24x24.Images[1] : images24x24.Images[0];
            }
            if (cpu.IoSyncFF != lastIoSyncFF)
            {
                lastIoSyncFF = cpu.IoSyncFF;
                ioSyncLamp.Image = (lastIoSyncFF) ? images24x24.Images[1] : images24x24.Images[0];
            }

        }

        private void diagnosticsButton_Click(object sender, EventArgs e)
        {
            Diagnostics diags;
            diags = new Diagnostics();
            debugOutput.Clear();
            debugOutput.AppendText(diags.run());
        }

        private void assembleButton_Click(object sender, EventArgs e)
        {
            if (outputToMemory.Checked) assembler.OutputMode = 'R';
            if (outputToPaperTape.Checked) assembler.OutputMode = 'P';
            if (outputToMagTape.Checked) assembler.OutputMode = 'M';
            if (outputToCard.Checked) assembler.OutputMode = 'C';
            assembler.OutputFilename = outputFilename.Text;
            assembler.setSource(assemblerSource.Lines);
            assemblerResults.Text = assembler.assemble();
        }

        private void htoggle2_MouseDown(object sender, MouseEventArgs e)
        {
            String tag;
            tag = Convert.ToString(((PictureBox)sender).Tag);
            if (tag.Equals("Power"))
            {
                if (cpu.Power)
                {
                    cpu.powerOff();
                    ((PictureBox)sender).Image = images46x24.Images[2];
                    powerLamp.Image = images24x24.Images[0];
                    updatePanel();
                    senseLamp1.Image = images24x24.Images[0];
                    senseLamp2.Image = images24x24.Images[0];
                    senseLamp3.Image = images24x24.Images[0];
                    senseLamp4.Image = images24x24.Images[0];
                    senseLamp5.Image = images24x24.Images[0];
                    senseLamp6.Image = images24x24.Images[0];
                    singleStepLamp.Image = images24x24.Images[0];
                    singleInstLamp.Image = images24x24.Images[0];
                }
                else
                {
                    cpu.powerOn();
                    ((PictureBox)sender).Image = images46x24.Images[0];
                    powerLamp.Image = images24x24.Images[1];
                    updatePanel();
                    senseLamp1.Image = (cpu.SenseSwitches[0]) ? images24x24.Images[1] : images24x24.Images[0];
                    senseLamp2.Image = (cpu.SenseSwitches[1]) ? images24x24.Images[1] : images24x24.Images[0];
                    senseLamp3.Image = (cpu.SenseSwitches[2]) ? images24x24.Images[1] : images24x24.Images[0];
                    senseLamp4.Image = (cpu.SenseSwitches[3]) ? images24x24.Images[1] : images24x24.Images[0];
                    senseLamp5.Image = (cpu.SenseSwitches[4]) ? images24x24.Images[1] : images24x24.Images[0];
                    senseLamp6.Image = (cpu.SenseSwitches[5]) ? images24x24.Images[1] : images24x24.Images[0];
                    singleStepLamp.Image = (cpu.SingleStep) ? images24x24.Images[1] : images24x24.Images[0];
                    singleInstLamp.Image = (cpu.SingleInst) ? images24x24.Images[1] : images24x24.Images[0];
                }
            }
            if (tag.Equals("SingleStep"))
            {
                if (cpu.SingleStep)
                {
                    cpu.SingleStep = false;
                    ((PictureBox)sender).Image = images46x24.Images[2];
                    if (!cpu.Power) return;
                    singleStepLamp.Image = images24x24.Images[0];
                }
                else
                {
                    cpu.SingleStep = true;
                    ((PictureBox)sender).Image = images46x24.Images[0];
                    if (!cpu.Power) return;
                    singleStepLamp.Image = images24x24.Images[1];
                }
            }
            if (tag.Equals("SingleInst"))
            {
                if (cpu.SingleInst)
                {
                    cpu.SingleInst = false;
                    ((PictureBox)sender).Image = images46x24.Images[2];
                    if (!cpu.Power) return;
                    singleInstLamp.Image = images24x24.Images[0];
                }
                else
                {
                    cpu.SingleInst = true;
                    ((PictureBox)sender).Image = images46x24.Images[0];
                    if (!cpu.Power) return;
                    singleInstLamp.Image = images24x24.Images[1];
                }
            }
        }
        private void vtoggle2_MouseDown(object sender, MouseEventArgs e)
        {
            String tag;
            int num;
            tag = Convert.ToString(((PictureBox)sender).Tag);
            if (tag.StartsWith("Sense"))
            {
                num = Convert.ToInt32(tag.Substring(5)) - 1;
                if (cpu.SenseSwitches[num])
                {
                    cpu.SenseSwitches[num] = false;
                    ((PictureBox)sender).Image = images24x46.Images[2];
                    switch (num)
                    {
                        case 0: senseLamp1.Image = images24x24.Images[0]; break;
                        case 1: senseLamp2.Image = images24x24.Images[0]; break;
                        case 2: senseLamp3.Image = images24x24.Images[0]; break;
                        case 3: senseLamp4.Image = images24x24.Images[0]; break;
                        case 4: senseLamp5.Image = images24x24.Images[0]; break;
                        case 5: senseLamp6.Image = images24x24.Images[0]; break;
                    }
                }
                else
                {
                    cpu.SenseSwitches[num] = true;
                    ((PictureBox)sender).Image = images24x46.Images[0];
                    switch (num)
                    {
                        case 0: senseLamp1.Image = (cpu.Power) ? images24x24.Images[1] : images24x24.Images[0]; break;
                        case 1: senseLamp2.Image = (cpu.Power) ? images24x24.Images[1] : images24x24.Images[0]; break;
                        case 2: senseLamp3.Image = (cpu.Power) ? images24x24.Images[1] : images24x24.Images[0]; break;
                        case 3: senseLamp4.Image = (cpu.Power) ? images24x24.Images[1] : images24x24.Images[0]; break;
                        case 4: senseLamp5.Image = (cpu.Power) ? images24x24.Images[1] : images24x24.Images[0]; break;
                        case 5: senseLamp6.Image = (cpu.Power) ? images24x24.Images[1] : images24x24.Images[0]; break;
                    }
                }
            }
            if (tag.StartsWith("tw"))
            {
                num = Convert.ToInt32(tag.Substring(2));
                if ((cpu.TestWord & num) == num)
                {
                    ((PictureBox)sender).Image = images24x46.Images[2];
                    cpu.TestWord &= (UInt32)(num ^ 0x3ffff);
                }
                else
                {
                    ((PictureBox)sender).Image = images24x46.Images[0];
                    cpu.TestWord |= (UInt32)num;
                }
            }
            if (tag.Equals("Extend"))
            {
                if (cpu.ExtendSwitch)
                {
                    ((PictureBox)sender).Image = images24x46.Images[2];
                    cpu.ExtendSwitch = false;
                }
                else
                {
                    ((PictureBox)sender).Image = images24x46.Images[0];
                    cpu.ExtendSwitch = true;
                }
            }
            if (tag.StartsWith("Extension"))
            {
                num = Convert.ToInt32(tag.Substring(9));
                if ((cpu.AddressExtSwitches & num) == num)
                {
                    ((PictureBox)sender).Image = images24x46.Images[2];
                    cpu.AddressExtSwitches &= (UInt32)(num ^ 0xf);
                }
                else
                {
                    ((PictureBox)sender).Image = images24x46.Images[0];
                    cpu.AddressExtSwitches |= (UInt32)num;
                }
            }
            if (tag.StartsWith("Address"))
            {
                num = Convert.ToInt32(tag.Substring(7));
                if ((cpu.AddressSwitches & num) == num)
                {
                    ((PictureBox)sender).Image = images24x46.Images[2];
                    cpu.AddressSwitches &= (UInt32)(num ^ 0xfff);
                }
                else
                {
                    ((PictureBox)sender).Image = images24x46.Images[0];
                    cpu.AddressSwitches |= (UInt32)num;
                }
            }
        }

        private void vtogglem_MouseDown(object sender, MouseEventArgs e)
        {
            String tag;
            tag = Convert.ToString(((PictureBox)sender).Tag);
            if (e.Button == MouseButtons.Left)
            {
                ((PictureBox)sender).Image = images24x46.Images[2];
            }
            if (e.Button == MouseButtons.Right)
            {
                ((PictureBox)sender).Image = images24x46.Images[0];
            }
            if (tag.StartsWith("Deposit"))
            {
                cpu.deposit();
                updatePanel();
            }
            if (tag.StartsWith("Examine"))
            {
                cpu.examine();
                updatePanel();
            }
            if (tag.StartsWith("Stop"))
            {
                cpu.stop();
                updatePanel();
            }
            if (tag.StartsWith("Continue"))
            {
                cpu.cont();
                updatePanel();
            }
            if (tag.StartsWith("Start"))
            {
                if (!cpu.Power) return;
                if (e.Button == MouseButtons.Left) cpu.SeqBreakFF = false;
                if (e.Button == MouseButtons.Right) cpu.SeqBreakFF = true;
                cpu.start();
                updatePanel();
            }
            if (tag.StartsWith("ReadIn"))
            {
                cpu.ReadIn();
                updatePanel();
            }
        }

        private void vtogglem_MouseUp(object sender, MouseEventArgs e)
        {
            String tag;
            tag = Convert.ToString(((PictureBox)sender).Tag);
            ((PictureBox)sender).Image = images24x46.Images[1];
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int j;
            String printer;
            String debugLog;
//            timer1.Stop();
            if (cpu.LinePrinterOutput.Length > 0)
            {
                linePrinterOutput.AppendText(cpu.LinePrinterOutput);
                cpu.LinePrinterOutput = "";
            }
            updatePanel();
            if (enableDebugMessages.Checked)
            {
                debugLog = cpu.getDebugLog();
                if (debugLog.Length > 0) debugOutput.AppendText(debugLog);
            }
            printer = cpu.Terminal.getOutput();
            if (printer.Length > 0)
            {
                for (j = 0; j < printer.Length; j++)
                {
                    if (printer[j] == '\r') typewriterOutput.AppendText("\r\n");
                    if (printer[j] == 8 && typewriterOutput.Text.Length > 0)
                        typewriterOutput.Text = typewriterOutput.Text.Substring(0, typewriterOutput.Text.Length - 1);
                    else typewriterOutput.AppendText(printer[j].ToString());
                }
            }
//            timer1.Start();
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            assemblerSource.Clear();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            int i;
            StreamWriter file;
            saveFileDialog.Filter = "Asm file (*.asm)|*.asm";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                file = new StreamWriter(saveFileDialog.FileName);
                for (i = 0; i < assemblerSource.Lines.Count(); i++)
                    file.WriteLine(assemblerSource.Lines[i]);
                file.Close();
            }
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            StreamReader file;
            String buffer;
            assemblerSource.Clear();
            openFileDialog.Filter = "Asm file (*.asm)|*.asm";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                file = new StreamReader(openFileDialog.FileName);
                while (!file.EndOfStream)
                {
                    buffer = file.ReadLine();
                    assemblerSource.AppendText(buffer + "\r\n");
                }
                file.Close();
            }
        }

        private void outputType_CheckedChanged(object sender, EventArgs e)
        {
            if (outputToMemory.Checked)
            {
                outputFilename.Enabled = false;
                outputFilenameLabel.Enabled = false;
            }
            else
            {
                outputFilename.Enabled = true;
                outputFilenameLabel.Enabled = true;
            }
        }

        private void fileButton_Click(object sender, EventArgs e)
        {
            if (outputToPaperTape.Checked) saveFileDialog.Filter = "Punched Tape (*.ptp)|*.ptp";
            if (outputToMagTape.Checked) saveFileDialog.Filter = "Mag Tape (*.tap)|*.tap";
            if (outputToCard.Checked) saveFileDialog.Filter = "Punched Cards (*.crd)|*.crd";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                outputFilename.Text = saveFileDialog.FileName;
            }
        }

        private void tapeReaderFilenameButton_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Paper Tape (*.ptp)|*.ptp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                tapeReaderFilename.Text = openFileDialog.FileName;
            }
        }

        private void tapeReaderMountButton_Click(object sender, EventArgs e)
        {
            cpu.TapeReader.mount(tapeReaderFilename.Text);
            tapeReaderMountButton.Enabled = false;
            tapeReaderUnmountButton.Enabled = true;
            tapeReaderRewindButton.Enabled = true;
            tapeReaderFilename.Enabled = false;
            tapeReaderFilenameButton.Enabled = false;
        }

        private void tapeReaderUnmountButton_Click(object sender, EventArgs e)
        {
            cpu.TapeReader.unmount();
            tapeReaderMountButton.Enabled = true;
            tapeReaderUnmountButton.Enabled = false;
            tapeReaderRewindButton.Enabled = false;
            tapeReaderFilename.Enabled = true;
            tapeReaderFilenameButton.Enabled = true;
        }

        private void tapeReaderRewindButton_Click(object sender, EventArgs e)
        {
            cpu.TapeReader.rewind();
        }

        private void enableDebugMessages_CheckedChanged(object sender, EventArgs e)
        {
            cpu.Debug = enableDebugMessages.Checked;
        }

        private void debugClearButton_Click(object sender, EventArgs e)
        {
            debugOutput.Clear();
        }

        private void tapeReaderFilename_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(tapeReaderFilename.Text)) tapeReaderMountButton.Enabled = true;
            else tapeReaderMountButton.Enabled = false;
        }

        private void tapePunchFilename_TextChanged(object sender, EventArgs e)
        {
            if (tapePunchFilename.Text.Length > 0) tapePunchMountButton.Enabled = true;
            else tapePunchMountButton.Enabled = false;
        }

        private void tapePunchFilenameButton_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "Paper Tape (*.ptp)|*.ptp";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                tapePunchFilename.Text = saveFileDialog.FileName;
            }
        }

        private void tapePunchMountButton_Click(object sender, EventArgs e)
        {
            cpu.TapePunch.mount(tapePunchFilename.Text);
            tapePunchFilename.Enabled = false;
            tapePunchFilenameButton.Enabled = false;
            tapePunchMountButton.Enabled = false;
            tapePunchUnmountButton.Enabled = true;
        }

        private void tapePunchUnmountButton_Click(object sender, EventArgs e)
        {
            cpu.TapePunch.unmount();
            tapePunchFilename.Enabled = true;
            tapePunchFilenameButton.Enabled = true;
            tapePunchMountButton.Enabled = true;
            tapePunchUnmountButton.Enabled = false;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            cpu.Terminal.keyTyped(e.KeyChar);
        }

        private void typewriterClearButton_Click(object sender, EventArgs e)
        {
            typewriterOutput.Clear();
        }

        private void typewriterOutputIRQ_SelectedIndexChanged(object sender, EventArgs e)
        {
            cpu.Terminal.OutputIrq = typewriterOutputIRQ.SelectedIndex - 1;
        }

        private void keyReadyIRQ_SelectedIndexChanged(object sender, EventArgs e)
        {
            cpu.Terminal.InputIrq = keyReadyIRQ.SelectedIndex - 1;
        }

        private void type120IRQ_CheckedChanged(object sender, EventArgs e)
        {
            cpu.SequenceBreak120 = type120IRQ.Checked;
        }

        private void standardIRQ_CheckedChanged(object sender, EventArgs e)
        {
            cpu.SequenceBreak120 = type120IRQ.Checked;
        }

        private void tapeReaderIrq_SelectedIndexChanged(object sender, EventArgs e)
        {
            cpu.TapeReader.Irq = tapeReaderIrq.SelectedIndex - 1;

        }

        private void tapePunchIrq_SelectedIndexChanged(object sender, EventArgs e)
        {
            cpu.TapePunch.Irq = tapePunchIrq.SelectedIndex - 1;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            linePrinterOutput.Clear();
        }

        private void linePrinterIrq_SelectedIndexChanged(object sender, EventArgs e)
        {
            cpu.LinePrinter.Irq = linePrinterIrq.SelectedIndex - 1;
        }

        private void type51WP_CheckedChanged(object sender, EventArgs e)
        {
            int tag;
            if (updating) return;
            tag = Convert.ToInt16(((CheckBox)sender).Tag);
            cpu.Type51TapeControl.transport(tag - 1).WriteProtected = ((CheckBox)sender).Checked;
        }

        private void type51FileButton_Click(object sender, EventArgs e)
        {
            int tag;
            tag = Convert.ToInt16(((Button)sender).Tag);
            saveFileDialog.Filter = "Magnetic Tape (*.mtp)|*.mtp";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                switch (tag) {
                    case 1: type51Filename1.Text = saveFileDialog.FileName; break;
                    case 2: type51Filename2.Text = saveFileDialog.FileName; break;
                    case 3: type51Filename3.Text = saveFileDialog.FileName; break;
                }
            }

        }

        private void updateType51()
        {
            type51Mounted1.Visible = (cpu.Type51TapeControl.transport(0).filename.Length > 0);
            type51Mounted2.Visible = (cpu.Type51TapeControl.transport(1).filename.Length > 0);
            type51Mounted3.Visible = (cpu.Type51TapeControl.transport(2).filename.Length > 0);
            if (type51Transport1.Checked)
            {
                type51MountButton.Enabled = !type51Mounted1.Visible;
                type51UnmountButton.Enabled = type51Mounted1.Visible;
            }
            if (type51Transport2.Checked)
            {
                type51MountButton.Enabled = !type51Mounted2.Visible;
                type51UnmountButton.Enabled = type51Mounted2.Visible;
            }
            if (type51Transport3.Checked)
            {
                type51MountButton.Enabled = !type51Mounted3.Visible;
                type51UnmountButton.Enabled = type51Mounted3.Visible;
            }
        }

        private void type51StopButton_Click(object sender, EventArgs e)
        {
            if (type51Transport1.Checked) cpu.Type51TapeControl.transport(0).Stop();
            if (type51Transport2.Checked) cpu.Type51TapeControl.transport(1).Stop();
            if (type51Transport3.Checked) cpu.Type51TapeControl.transport(2).Stop();
        }

        private void type51RewindButton_Click(object sender, EventArgs e)
        {
            if (type51Transport1.Checked) cpu.Type51TapeControl.transport(0).Rewind();
            if (type51Transport2.Checked) cpu.Type51TapeControl.transport(1).Rewind();
            if (type51Transport3.Checked) cpu.Type51TapeControl.transport(2).Rewind();
        }

        private void type51MountButton_Click(object sender, EventArgs e)
        {
            if (type51Transport1.Checked) cpu.Type51TapeControl.transport(0).Mount(type51Filename1.Text);
            if (type51Transport2.Checked) cpu.Type51TapeControl.transport(1).Mount(type51Filename2.Text);
            if (type51Transport3.Checked) cpu.Type51TapeControl.transport(2).Mount(type51Filename3.Text);
            updateType51();
        }

        private void type51UnmountButton_Click(object sender, EventArgs e)
        {
            if (type51Transport1.Checked) cpu.Type51TapeControl.transport(0).Unmount();
            if (type51Transport2.Checked) cpu.Type51TapeControl.transport(1).Unmount();
            if (type51Transport3.Checked) cpu.Type51TapeControl.transport(2).Unmount();
            updateType51();
        }

        private void type51Transport_CheckedChanged(object sender, EventArgs e)
        {
            updateType51();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            cpu.Terminate();
        }

        private void updateType52(Boolean buttonsOnly)
        {
            Type52TapeControl mtcu;
            mtcu = cpu.Type52TapeControl(type52MTCUSelect.SelectedIndex);
            if (!buttonsOnly)
            {
                type52Filename1.Text = mtcu.transport(0).filename;
                type52Filename2.Text = mtcu.transport(1).filename;
                type52Filename3.Text = mtcu.transport(2).filename;
                type52Filename4.Text = mtcu.transport(3).filename;
                type52Filename5.Text = mtcu.transport(4).filename;
                type52Filename6.Text = mtcu.transport(5).filename;
                type52Filename7.Text = mtcu.transport(6).filename;
                type52Filename8.Text = mtcu.transport(7).filename;
                type52Mounted1.Visible = (mtcu.transport(0).filename.Length > 0);
                type52Mounted2.Visible = (mtcu.transport(1).filename.Length > 0);
                type52Mounted3.Visible = (mtcu.transport(2).filename.Length > 0);
                type52Mounted4.Visible = (mtcu.transport(3).filename.Length > 0);
                type52Mounted5.Visible = (mtcu.transport(4).filename.Length > 0);
                type52Mounted6.Visible = (mtcu.transport(5).filename.Length > 0);
                type52Mounted7.Visible = (mtcu.transport(6).filename.Length > 0);
                type52Mounted8.Visible = (mtcu.transport(7).filename.Length > 0);
                type52WP1.Checked = (mtcu.transport(0).WriteProtected);
                type52WP2.Checked = (mtcu.transport(1).WriteProtected);
                type52WP3.Checked = (mtcu.transport(2).WriteProtected);
                type52WP4.Checked = (mtcu.transport(3).WriteProtected);
                type52WP5.Checked = (mtcu.transport(4).WriteProtected);
                type52WP6.Checked = (mtcu.transport(5).WriteProtected);
                type52WP7.Checked = (mtcu.transport(6).WriteProtected);
                type52WP8.Checked = (mtcu.transport(7).WriteProtected);
            }
            if (type52Transport1.Checked)
            {
                type52MountButton.Enabled = !type52Mounted1.Visible;
                type52UnmountButton.Enabled = type52Mounted1.Visible;
            }
            if (type52Transport2.Checked)
            {
                type52MountButton.Enabled = !type52Mounted2.Visible;
                type52UnmountButton.Enabled = type52Mounted2.Visible;
            }
            if (type52Transport3.Checked)
            {
                type52MountButton.Enabled = !type52Mounted3.Visible;
                type52UnmountButton.Enabled = type52Mounted3.Visible;
            }
            if (type52Transport4.Checked)
            {
                type52MountButton.Enabled = !type52Mounted4.Visible;
                type52UnmountButton.Enabled = type52Mounted4.Visible;
            }
            if (type52Transport5.Checked)
            {
                type52MountButton.Enabled = !type52Mounted5.Visible;
                type52UnmountButton.Enabled = type52Mounted5.Visible;
            }
            if (type52Transport6.Checked)
            {
                type52MountButton.Enabled = !type52Mounted6.Visible;
                type52UnmountButton.Enabled = type52Mounted6.Visible;
            }
            if (type52Transport7.Checked)
            {
                type52MountButton.Enabled = !type52Mounted7.Visible;
                type52UnmountButton.Enabled = type52Mounted7.Visible;
            }
            if (type52Transport8.Checked)
            {
                type52MountButton.Enabled = !type52Mounted8.Visible;
                type52UnmountButton.Enabled = type52Mounted8.Visible;
            }
            type52Irq.SelectedIndex = mtcu.Irq + 1;
        }

        private void type52FileButton_Click(object sender, EventArgs e)
        {
            int tag;
            tag = Convert.ToInt16(((Button)sender).Tag);
            saveFileDialog.Filter = "Magnetic Tape (*.mtp)|*.mtp";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                switch (tag)
                {
                    case 1: type52Filename1.Text = saveFileDialog.FileName; break;
                    case 2: type52Filename2.Text = saveFileDialog.FileName; break;
                    case 3: type52Filename3.Text = saveFileDialog.FileName; break;
                    case 4: type52Filename4.Text = saveFileDialog.FileName; break;
                    case 5: type52Filename5.Text = saveFileDialog.FileName; break;
                    case 6: type52Filename6.Text = saveFileDialog.FileName; break;
                    case 7: type52Filename7.Text = saveFileDialog.FileName; break;
                    case 8: type52Filename8.Text = saveFileDialog.FileName; break;
                }
            }

        }

        private void type52StopButton_Click(object sender, EventArgs e)
        {
            Type52TapeControl mtcu;
            mtcu = cpu.Type52TapeControl(type52MTCUSelect.SelectedIndex);
            if (type52Transport1.Checked) mtcu.transport(0).Stop();
            if (type52Transport2.Checked) mtcu.transport(1).Stop();
            if (type52Transport3.Checked) mtcu.transport(2).Stop();
            if (type52Transport4.Checked) mtcu.transport(3).Stop();
            if (type52Transport5.Checked) mtcu.transport(4).Stop();
            if (type52Transport6.Checked) mtcu.transport(5).Stop();
            if (type52Transport7.Checked) mtcu.transport(6).Stop();
            if (type52Transport8.Checked) mtcu.transport(7).Stop();
        }

        private void type52RewindButton_Click(object sender, EventArgs e)
        {
            Type52TapeControl mtcu;
            mtcu = cpu.Type52TapeControl(type52MTCUSelect.SelectedIndex);
            if (type52Transport1.Checked) mtcu.transport(0).Rewind();
            if (type52Transport2.Checked) mtcu.transport(1).Rewind();
            if (type52Transport3.Checked) mtcu.transport(2).Rewind();
            if (type52Transport4.Checked) mtcu.transport(3).Rewind();
            if (type52Transport5.Checked) mtcu.transport(4).Rewind();
            if (type52Transport6.Checked) mtcu.transport(5).Rewind();
            if (type52Transport7.Checked) mtcu.transport(6).Rewind();
            if (type52Transport8.Checked) mtcu.transport(7).Rewind();
        }

        private void type52MountButton_Click(object sender, EventArgs e)
        {
            Type52TapeControl mtcu;
            mtcu = cpu.Type52TapeControl(type52MTCUSelect.SelectedIndex);
            if (type52Transport1.Checked) mtcu.transport(0).Mount(type52Filename1.Text);
            if (type52Transport2.Checked) mtcu.transport(1).Mount(type52Filename2.Text);
            if (type52Transport3.Checked) mtcu.transport(2).Mount(type52Filename3.Text);
            if (type52Transport4.Checked) mtcu.transport(3).Mount(type52Filename4.Text);
            if (type52Transport5.Checked) mtcu.transport(4).Mount(type52Filename5.Text);
            if (type52Transport6.Checked) mtcu.transport(5).Mount(type52Filename6.Text);
            if (type52Transport7.Checked) mtcu.transport(6).Mount(type52Filename7.Text);
            if (type52Transport8.Checked) mtcu.transport(7).Mount(type52Filename8.Text);
            updateType52(false);
        }

        private void type52UnmountButton_Click(object sender, EventArgs e)
        {
            Type52TapeControl mtcu;
            mtcu = cpu.Type52TapeControl(type52MTCUSelect.SelectedIndex);
            if (type52Transport1.Checked) mtcu.transport(0).Unmount();
            if (type52Transport2.Checked) mtcu.transport(1).Unmount();
            if (type52Transport3.Checked) mtcu.transport(2).Unmount();
            if (type52Transport4.Checked) mtcu.transport(3).Unmount();
            if (type52Transport5.Checked) mtcu.transport(4).Unmount();
            if (type52Transport6.Checked) mtcu.transport(5).Unmount();
            if (type52Transport7.Checked) mtcu.transport(6).Unmount();
            if (type52Transport8.Checked) mtcu.transport(7).Unmount();
            updateType52(false);
        }

        private void type52Irq_SelectedIndexChanged(object sender, EventArgs e)
        {
            Type52TapeControl mtcu;
            mtcu = cpu.Type52TapeControl(type52MTCUSelect.SelectedIndex);
            mtcu.Irq = type52Irq.SelectedIndex - 1;
        }

        private void type52MTCUSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateType52(false);
        }

        private void type52Transport_CheckedChanged(object sender, EventArgs e)
        {
            updateType52(true);
        }

        private void type52WP_CheckedChanged(object sender, EventArgs e)
        {
            int tag;
            Type52TapeControl mtcu;
            mtcu = cpu.Type52TapeControl(type52MTCUSelect.SelectedIndex);
            if (updating) return;
            tag = Convert.ToInt16(((CheckBox)sender).Tag);
            mtcu.transport(tag - 1).WriteProtected = ((CheckBox)sender).Checked;

        }
 
    }
}

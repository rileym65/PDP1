using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace PDP1
{
    class Assembler
    {

        private List<String> labels;
        private List<int> labelValues;
        private List<String> variables;
        private List<int> variableAddrs;
        private List<int> constantValues;
        private List<int> constantAddrs;
        private Cpu cpu;
        private UInt32[] ram;
        private int pass;
        private String[] source;
        private String results;
        private UInt32 address;
        private int errorCount;
        private int bytesAssembled;
        private int dfraction;
        private char outputMode;
        private String outputFilename;
        private FileStream outputFile;
        private int startAddress;
        private int radix;
        private String numbers = "0123456789ABCDEF";
        private char combine;
        private String token;
        private List<int> value;
        private List<int> termCount;
        private List<Object> expressions;
        private char emode;
        private String lastLabel;
        private  String outputLine;
        private Boolean endAssembly;
        private int repeats;
        private String repeatString;
        private List<Macro> macros;
        private Boolean record;
        private List<Macro> currentMacro;
        private int currentLine;
        private int currentPos;


        public Assembler(Cpu c)
        {
            cpu = c;
            labels = new List<String>();
            labelValues = new List<int>();
            outputMode = 'R';
            outputFilename = "";
        }

        public String OutputFilename
        {
            set { outputFilename = value; }
        }

        public char OutputMode
        {
            set { outputMode = value; }
        }

        public void setSource(String[] src)
        {
            source = src;
        }

        private int validNumber(String m)
        {
            int i;
            int value;
            int digit;
            value = 0;
            for (i = 0; i < m.Length; i++)
            {
                digit = numbers.IndexOf(m[i]);
                if (digit >= radix) return -1;
                value = (value * radix) + digit;
            }
            return value;
        }

        private int ConvertToDecimal(String num)
        {
            double v;
            int mask;
            int dmask;
            int result;
            v = Convert.ToDouble(num);
            v -= (int)v;
            mask = 1 << 16;
            dmask = 1 << 26;
            result = 0;
            dfraction = 0;
            while (dmask > 0)
            {
                v *= 2;
                if (v >= 1.0)
                {
                    if (mask > 0) result |= mask;
                    dfraction |= dmask;
                }
                mask >>= 1;
                dmask >>= 1;
                v -= (int)v;
            }
            return result;
        }

        private int convertToNumber(String m)
        {
            int value;
            value = validNumber(m);
            if (value >= 0) return value;
            return -1;
        }

        private Boolean isNumber(String value)
        {
            int i;
            for (i = 0; i < value.Length; i++)
                if (value[i] < '0' || value[i] > '9') return false;
            return true;
        }

        private int findLabel(String m)
        {
            int i;
            int pos;
            int value;
            pos = -1;
            if (combine == '.' || combine == '_')
            {
                if (!isNumber(m))
                {
                    if (pass == 2) error("A number must follow FRAC");
                    return -1;
                }
                else
                {
                    if (combine == '.') combine = '+';
                    if (combine == '_') combine = '-';
                    return ConvertToDecimal("." + m);
                }
            }
            if (isNumber(m))
            {
                value = convertToNumber(m);
                if (value >= 0) return value;
            }

            for (i = 0; i < labels.Count; i++)
            {
                if (m.ToUpper().Equals(labels[i].ToUpper())) pos = i;
            }
            if (pos >= 0)
            {
                value = (int)labelValues[pos];
                return value;
            }
            return -1;
        }

        private Boolean addLabel(String m, int value)
        {
            int pos;
            int i;
            if (m.Equals("-1"))
            {
                if (pass == 1) error("Expression cannot be defined as a label");
                return false;
            }
            pos = -1;
            for (i = 0; i < labels.Count; i++)
            {
                if (m.CompareTo((String)labels[i]) == 0) pos = i;
            }
            if (pos >= 0)
            {
                labelValues[pos] = value;
                return true;
            }
            else
            {
                labels.Add(m);
                labelValues.Add(value);
                if (currentMacro != null && currentMacro.Count > 0) currentMacro[currentMacro.Count-1].addLabel(m,value);
                return false;
            }
        }

        private int fromOctal(String value)
        {
            int i;
            int ret;
            ret = 0;
            for (i = 0; i < value.Length; i++)
            {
                ret <<= 3;
                ret |= (value[i] - '0');
            }
            return ret;
        }

        private void defaultLabels()
        {
            addLabel("1S", fromOctal("000001"));
            addLabel("2S", fromOctal("000003"));
            addLabel("3S", fromOctal("000007"));
            addLabel("4S", fromOctal("000017"));
            addLabel("5S", fromOctal("000037"));
            addLabel("6S", fromOctal("000077"));
            addLabel("7S", fromOctal("000177"));
            addLabel("8S", fromOctal("000377"));
            addLabel("9S", fromOctal("000777"));
            addLabel("ADD", fromOctal("400000"));
            addLabel("AND", fromOctal("020000"));
            addLabel("CAL", fromOctal("160000"));
            addLabel("CDF", fromOctal("720074"));
            addLabel("CFD", fromOctal("720074"));
            addLabel("CKS", fromOctal("720033"));
            addLabel("CLA", fromOctal("760200"));
            addLabel("CLC", fromOctal("761200"));
            addLabel("CLF", fromOctal("760000"));
            addLabel("CLI", fromOctal("764000"));
            addLabel("CLO", fromOctal("651600"));
            addLabel("CMA", fromOctal("761000"));
            addLabel("DAC", fromOctal("240000"));
            addLabel("DAP", fromOctal("260000"));
            addLabel("DIO", fromOctal("320000"));
            addLabel("DIP", fromOctal("300000"));
            addLabel("DIV", fromOctal("560000"));
            addLabel("DPY", fromOctal("730007"));
            addLabel("DZM", fromOctal("340000"));
            addLabel("ESM", fromOctal("720055"));
            addLabel("HLT", fromOctal("760400"));
            addLabel("I", fromOctal("010000"));
            addLabel("IDX", fromOctal("440000"));
            addLabel("IOR", fromOctal("040000"));
            addLabel("IOT", fromOctal("720000"));
            addLabel("ISP", fromOctal("460000"));
            addLabel("JDA", fromOctal("170000"));
            addLabel("JFD", fromOctal("120000"));
            addLabel("JMP", fromOctal("600000"));
            addLabel("JSP", fromOctal("620000"));
            addLabel("LAC", fromOctal("200000"));
            addLabel("LAP", fromOctal("760300"));
            addLabel("LAT", fromOctal("762200"));
            addLabel("LAW", fromOctal("700000"));
            addLabel("LIO", fromOctal("220000"));
            addLabel("LSM", fromOctal("720054"));
            addLabel("MUL", fromOctal("540000"));
            addLabel("NOP", fromOctal("760000"));
            addLabel("OPR", fromOctal("760000"));
            addLabel("PPA", fromOctal("730005"));
            addLabel("PPB", fromOctal("730006"));
            addLabel("RAL", fromOctal("661000"));
            addLabel("RAR", fromOctal("671000"));
            addLabel("RCL", fromOctal("663000"));
            addLabel("RCR", fromOctal("673000"));
            addLabel("RIL", fromOctal("662000"));
            addLabel("RIR", fromOctal("672000"));
            addLabel("RPA", fromOctal("730001"));
            addLabel("RPB", fromOctal("730002"));
            addLabel("RRB", fromOctal("720030"));
            addLabel("SAD", fromOctal("500000"));
            addLabel("SAL", fromOctal("665000"));
            addLabel("SAR", fromOctal("675000"));
            addLabel("SAS", fromOctal("520000"));
            addLabel("SCR", fromOctal("677000"));
            addLabel("SIL", fromOctal("666000"));
            addLabel("SIR", fromOctal("676000"));
            addLabel("SKP", fromOctal("640000"));
            addLabel("SMA", fromOctal("640400"));
            addLabel("SPA", fromOctal("640200"));
            addLabel("SPI", fromOctal("642000"));
            addLabel("SPQ", fromOctal("650500"));
            addLabel("STF", fromOctal("760010"));
            addLabel("SUB", fromOctal("420000"));
            addLabel("SZA", fromOctal("640100"));
            addLabel("SZF", fromOctal("640000"));
            addLabel("SZM", fromOctal("640500"));
            addLabel("SZO", fromOctal("641000"));
            addLabel("SZS", fromOctal("640000"));
            addLabel("TYI", fromOctal("720004"));
            addLabel("TYO", fromOctal("730003"));
            addLabel("XCT", fromOctal("100000"));
            addLabel("XOR", fromOctal("060000"));
            addLabel("XX", fromOctal("760400"));

            addLabel("ASC", fromOctal("730051"));
            addLabel("CAC", fromOctal("730053"));
            addLabel("CBS", fromOctal("730056"));
            addLabel("CCR", fromOctal("730067"));
            addLabel("CGO", fromOctal("730073"));
            addLabel("CLRBUF", fromOctal("732045"));
            addLabel("CPM", fromOctal("730472"));
            addLabel("CRF", fromOctal("730272"));
            addLabel("DPP", fromOctal("730407"));
            addLabel("DSC", fromOctal("730050"));
            addLabel("DUR", fromOctal("730070"));
            addLabel("EEM", fromOctal("734074"));
            addLabel("GLF", fromOctal("732026"));
            addLabel("GPL", fromOctal("732027"));
            addLabel("GPR", fromOctal("730027"));
            addLabel("GSP", fromOctal("730026"));
            addLabel("INR", fromOctal("730067"));
            addLabel("ISB", fromOctal("730052"));
            addLabel("LAG", fromOctal("730044"));
            addLabel("LEM", fromOctal("730074"));
            addLabel("LPB", fromOctal("730045"));
            addLabel("MBS", fromOctal("720375"));
            addLabel("MCB", fromOctal("730070"));
            addLabel("MCE", fromOctal("721475"));
            addLabel("MCO", fromOctal("721575"));
            addLabel("MCS", fromOctal("730034"));
            addLabel("MES", fromOctal("720035"));
            addLabel("MEL", fromOctal("730036"));
            addLabel("MFE", fromOctal("721075"));
            addLabel("MFO", fromOctal("721175"));
            addLabel("MIC", fromOctal("720075"));
            addLabel("MRC", fromOctal("730072"));
            addLabel("MRE", fromOctal("721675"));
            addLabel("MRF", fromOctal("730067"));
            addLabel("MRO", fromOctal("721775"));
            addLabel("MRW", fromOctal("720175"));
            addLabel("MRI", fromOctal("730066"));
            addLabel("MSM", fromOctal("730073"));
            addLabel("MST", fromOctal("720075"));
            addLabel("MTF", fromOctal("730071"));
            addLabel("MUF", fromOctal("730076"));
            addLabel("MWC", fromOctal("730071"));
            addLabel("MWE", fromOctal("721275"));
            addLabel("MWO", fromOctal("721375"));
            addLabel("PAC", fromOctal("730043"));
            addLabel("PAS", fromOctal("731045"));
            addLabel("RAC", fromOctal("730041"));
            addLabel("RBC", fromOctal("730042"));
            addLabel("RCC", fromOctal("730032"));
            addLabel("RLC", fromOctal("730366"));
            addLabel("RSR", fromOctal("730172"));
            addLabel("SCL", fromOctal("667000"));
            addLabel("SDB", fromOctal("732007"));
            addLabel("SDF", fromOctal("730146"));
            addLabel("SFC", fromOctal("730072"));
            addLabel("SFT", fromOctal("660000"));
            addLabel("SHR", fromOctal("730446"));
            addLabel("SIA", fromOctal("730346"));
            addLabel("SWC", fromOctal("730046"));
            addLabel("SB0", fromOctal("000000"));
            addLabel("SB1", fromOctal("000004"));
            addLabel("SB2", fromOctal("000010"));
            addLabel("SB3", fromOctal("000014"));
            addLabel("SB4", fromOctal("000020"));
            addLabel("SB5", fromOctal("000024"));
            addLabel("SB6", fromOctal("000030"));
            addLabel("SB7", fromOctal("000034"));
            addLabel("SB10", fromOctal("000040"));
            addLabel("SB11", fromOctal("000044"));
            addLabel("SB12", fromOctal("000050"));
            addLabel("SB13", fromOctal("000054"));
            addLabel("SB14", fromOctal("000060"));
            addLabel("SB15", fromOctal("000064"));
            addLabel("SB16", fromOctal("000070"));
            addLabel("SB17", fromOctal("000074"));
            addLabel("C0", fromOctal("000000"));
            addLabel("C1", fromOctal("000100"));
            addLabel("C2", fromOctal("000200"));
            addLabel("C3", fromOctal("000300"));
            addLabel("C4", fromOctal("000400"));
            addLabel("C5", fromOctal("000500"));
            addLabel("C6", fromOctal("000600"));
            addLabel("C7", fromOctal("000700"));
            addLabel("C10", fromOctal("001000"));
            addLabel("C11", fromOctal("001100"));
            addLabel("C12", fromOctal("001200"));
            addLabel("C13", fromOctal("001300"));
            addLabel("C14", fromOctal("001400"));
            addLabel("C15", fromOctal("001500"));
            addLabel("C16", fromOctal("001600"));
            addLabel("C17", fromOctal("001700"));
            addLabel("S1", fromOctal("000001"));
            addLabel("S2", fromOctal("000003"));
            addLabel("S3", fromOctal("000007"));
            addLabel("S4", fromOctal("000017"));
            addLabel("S5", fromOctal("000037"));
            addLabel("S6", fromOctal("000077"));
            addLabel("S7", fromOctal("000177"));
            addLabel("S8", fromOctal("000377"));
            addLabel("S9", fromOctal("000777"));
            addLabel("MTCU0", fromOctal("000000"));
            addLabel("MTCU1", fromOctal("002000"));
            addLabel("MTCU2", fromOctal("004000"));
        }
        private void writePaperTape(UInt32 value)
        {   byte a,b,c;
            a = (byte)((value >> 12) & 0x3f);
            b = (byte)((value >> 6) & 0x3f);
            c = (byte)(value & 0x3f);
            outputFile.WriteByte((byte)(a | 0x80));
            outputFile.WriteByte((byte)(b | 0x80));
            outputFile.WriteByte((byte)(c | 0x80));
        }

        private void writeMem(String prefix,int value,String line)
        {
            if (currentMacro.Count > 0) prefix = "+";
            if (pass == 2)
            {
                output(prefix + Form1.convert(address, 18) + ": " + Form1.convert((UInt32)value, 18));
                switch (outputMode)
                {
                    case 'R':
                        ram[address] = (UInt32)value;
                        break;
                    case 'P':
                        writePaperTape((UInt32)(address | 0x2a000));
                        writePaperTape((UInt32)value);
                        break;
                }
            }
            bytesAssembled++;
            address++;
        }

        private void error(String msg)
        {
            errorCount++;
            results += "ERROR: " + msg + "\r\n";
        }

        private void output(String msg)
        {
            if (pass != 2) return;
            while (msg.Length < 16) msg += " ";
            results += msg + ((outputLine != null) ? outputLine : " ") + "\r\n";
            outputLine = null;
        }

        private int peek()
        {
            if (repeats > 0)
            {
                if (currentPos >= repeatString.Length) return 13;
                return repeatString[currentPos];
            }
            if (!record && currentMacro.Count > 0)
            {
                return currentMacro[currentMacro.Count - 1].peek();
            }
  
            if (currentLine >= source.Length) return -1;
            if (currentPos >= source[currentLine].Length) return 13;
            return source[currentLine][currentPos];
        }

        private int read()
        {
            int ret;
            ret = peek();
            if (repeats > 0)
            {
                if (ret == 13)
                {
                    repeats--;
                    currentPos = 0;
                    return ret;
                }
                else
                {
                    currentPos++;
                    return ret;
                }
            }
            if (!record && currentMacro.Count > 0)
            {
                ret = currentMacro[currentMacro.Count - 1].next();
                if (pass == 2 && (ret == -1 || ret == 13)) outputLine = currentMacro[currentMacro.Count - 1].PrintLine;
                if (ret == -1)
                {
                    while (labels.Count > currentMacro[currentMacro.Count - 1].SymbolStart)
                    {
                        labels.RemoveAt(currentMacro[currentMacro.Count - 1].SymbolStart);
                        labelValues.RemoveAt(currentMacro[currentMacro.Count - 1].SymbolStart);
                    }
                    currentMacro.RemoveAt(currentMacro.Count - 1);
                    ret = 13;
                    outputLine = null;
                }
                return ret;
            }

            if (ret < 0) return -1;
            if (ret == 13)
            {
                currentPos = 0;
//                if (currentLine == null) outputLine = source[currentLine];
                currentLine++;
                return ret;
            }
            if (currentPos == 0) outputLine = source[currentLine];
            currentPos++;
            return ret;
        }

        private String readLine()
        {
            int i;
            String ret;
            ret = "";
            i = read();
            while (i > 0 && i != 13)
            {
                ret += ((char)i).ToString();
                i = read();
            }
            return ret;
        }

        private void trim()
        {
            while (peek() == ' ') read();
        }

        private void addToOpcode(char combine, int val)
        {
            if (value[value.Count-1] < 0 || val < 0)
            {
                value[value.Count-1] = -1;
            }
            else
            {
                if (combine == '&')
                {
                    value[value.Count-1] &= val;
                }
                else if (combine == '|')
                {
                    value[value.Count-1] |= val;
                }
                else
                {
                    if (combine == '-')
                    {
                        val = (val ^ 0x3ffff) & 0x3ffff;
                    }
                    value[value.Count-1] += val;
                    if (value[value.Count-1] >= 0x40000) value[value.Count-1] = (value[value.Count-1] + 1) & 0x3ffff;
                }
            }
            termCount[value.Count-1]++;
            combine = '+';
        }

        private int flex(String value)
        {
            int ret;
            ret = 0;
            if (value.Length > 0) ret |= (int)((Cpu.asciiToDec(value[0]) & 0xff) << 12);
            if (value.Length > 1) ret |= (int)((Cpu.asciiToDec(value[1]) & 0xff) << 6);
            if (value.Length > 2) ret |= (int)(Cpu.asciiToDec(value[2]) & 0xff);
            return ret;
        }

        private void addVariable(String var)
        {
            int i;
            if (pass == 2) return;
            for (i = 0; i < variables.Count; i++)
                if (var.Equals(variables[i])) return;
            variables.Add(var);
            variableAddrs.Add(-1);
        }

        private int addConstant(int value)
        {
            int i;
            if (value < 0)
            {
                constantValues.Add(-1);
                constantAddrs.Add(-1);
                return -1;
            }
            for (i = 0; i < constantValues.Count; i++)
                if (constantValues[i] == value) return constantAddrs[i];
            if (pass == 2)
            {
                for (i = 0; i < constantValues.Count; i++)
                    if (constantValues[i] == -1)
                    {
                        constantValues[i] = value;
                        return constantAddrs[i];
                    }
            }
            constantValues.Add(value);
            constantAddrs.Add(-1);
            return -1;

        }

        private void doChar()
        {
            int i;
            int shift;
            int ovalue;
            trim();
            shift = -1;
            if (peek() == 'r' || peek() == 'R') shift = 0;
            if (peek() == 'm' || peek() == 'M') shift = 6;
            if (peek() == 'l' || peek() == 'L') shift = 12;
            if (shift < 0)
            {
                if (pass == 2) error("Invalid parameter for CHAR");
            }
            else
            {
                i = read();
                if (peek() == '\t')
                {
                    ovalue = (int)(Cpu.asciiToDec((UInt32)read()) & 0xff);
                    addToOpcode(combine, ovalue << shift);
                }
                else if (peek() == 13)
                {
                    ovalue = (int)(Cpu.asciiToDec((UInt32)peek()) & 0xff);
                    addToOpcode(combine, ovalue << shift);
                }
                else
                {
                    ovalue = (int)(Cpu.asciiToDec((UInt32)read()) & 0xff);
                    addToOpcode(combine, ovalue << shift);
                }
            }
        }

        private void doConstants()
        {
            int i;
            output(" ");
            for (i = 0; i < constantValues.Count; i++)
            {
                if (constantAddrs[i] < 0)
                {
                    constantAddrs[i] = (int)address;
                }
                writeMem("+", constantValues[i], "\t" + Form1.convert((UInt32)constantValues[i], 18));
            }
        }

        private void doDefine()
        {
            String tempStr;
            if ((value.Count > 1 || termCount[value.Count - 1] != 0) && pass == 2) error("DEFINE cannot be used inside an expression");
            if (value.Count == 1 && termCount[value.Count - 1] == 0)
            {
                trim();
                tempStr = "";
                while (peek() > 0 && peek() != ' ' && peek() != '\t' && peek() != 13)
                {
                    tempStr += ((char)read()).ToString();
                }
                trim();
                if (pass == 1) macros.Add(new Macro(tempStr.ToUpper(), readLine().ToUpper().Trim()));
                if (pass == 2)
                {
                    readLine();
                    results += "                 " + outputLine + "\r\n";
                }
                outputLine = null;
                record = true;
                emode = 'M';
            }
        }

        private void doExpunge()
        {
            if (value.Count > 1 || termCount[value.Count - 1] > 0)
            {
                if (pass == 2) error("EXPUNGE cannot be used inside an expression");
            }
            else if (repeats >= 0)
            {
                if (pass == 2) error("EXPUNGE cannot be used with REPEAT");
            }
            else if (currentMacro.Count > 0)
            {
                if (pass == 2) error("EXPUNGE cannot be used inside a macro");
            }
            else if (pass == 1)
            {
                labels = new List<String>();
                labelValues = new List<int>();
            }
        }

        private void doFlex()
        {
            int ovalue;
            trim();
            ovalue = 0;
            if (peek() != ' ' && peek() != '\t' && peek() != 13)
            {
                ovalue |= (int)((Cpu.asciiToDec((UInt32)read()) & 0xff) << 12);
            }
            if (peek() != ' ' && peek() != '\t' && peek() != 13)
            {
                ovalue |= (int)((Cpu.asciiToDec((UInt32)read()) & 0xff) << 6);
            }
            if (peek() != ' ' && peek() != '\t' && peek() != 13)
            {
                ovalue |= (int)(Cpu.asciiToDec((UInt32)read()) & 0xff);
            }
            addToOpcode(combine, ovalue);
        }

        private void doNoInput()
        {
        }

        private void doRepeat()
        {
            if (repeats >= 0 && pass == 2) error("Cannot nest REPEATs");
            if (repeats < 0)
            {
                emode = 'R';
                value[0] = 0;
                repeats = 0;
            }
        }

        private void doStart()
        {
            if ((value.Count > 1 || termCount[value.Count - 1] > 0) && pass == 2) error("Cannot use START inside of an expression");
            emode = 'S';
            value[0] = 0;
        }

        private void doText()
        {
            int ovalue;
            int quote;
            String tempStr;
            if ((value.Count > 1 || termCount[value.Count - 1] != 0) && pass == 2) error("TEXT cannot be used inside an expression");
            if (value.Count == 1 && termCount[value.Count - 1] == 0)
            {
                trim();
                if (peek() > 0 && peek() != '\t' && peek() != 13)
                {
                    quote = read();
                    tempStr = "";
                    while (peek() > 0 && peek() != '\t' && peek() != 13 && peek() != quote)
                    {
                        tempStr += ((char)read()).ToString();
                    }
                    if (peek() == quote) read();
                    while (tempStr.Length > 0)
                    {
                        ovalue = flex(tempStr);
                        writeMem(((outputLine != null) ? " " : "+"), ovalue, outputLine);
                        outputLine = null;
                        tempStr = (tempStr.Length >= 3) ? tempStr.Substring(3) : "";
                    }
                }
            }
        }

        private void doVariables()
        {
            int i;
            output(" ");
            for (i = 0; i < variables.Count; i++)
            {
                if (variableAddrs[i] < 0)
                {
                    addLabel(variables[i], (int)address);
                    variableAddrs[i] = (int)address;
                }
                output("+" + Form1.convert(address, 18) + ":        " + variables[i]);
                address++;
            }
        }

        private void processToken(int terminator)
        {
            int i;
            int ovalue;
            String tempStr;
            if (token.Length < 1) return;
            token = token.ToUpper();
            if (token.StartsWith("OCTA")) { radix = 8; }
            else if (token.StartsWith("DECI")) { radix = 10; }
            else if (token.StartsWith("FRAC"))
            {
                combine = (combine == '+') ? '.' : '_';
                token = "";
                return;
            }
            else if (token.StartsWith("NOIN")) doNoInput();
            else if (token.StartsWith("EXPU")) doExpunge();
            else if (token.StartsWith("VARI")) doVariables();
            else if (token.StartsWith("CONS")) doConstants();
            else if (token.StartsWith("REPE")) doRepeat();
            else if (token.StartsWith("STAR")) doStart();
            else if (token.StartsWith("CHAR")) doChar();
            else if (token.StartsWith("FLEX")) doFlex();
            else if (token.StartsWith("TEXT")) doText();
            else if (token.StartsWith("DEFI")) doDefine();
            else
            {
                for (i = 0; i < macros.Count; i++)
                    if (token.Equals(macros[i].Name))
                    {
                        tempStr = (terminator != 13 && terminator != '\t') ? readLine().ToUpper().Trim() : "";
                        macros[i].SymbolStart = labels.Count;
                        macros[i].start(tempStr, address, labels, labelValues, pass);
                        currentMacro.Add(macros[i]);
                        token = "";
                        if (pass == 2) results += "                 " + outputLine + "\r\n";
                        outputLine = null;
                        return;
                    }
                if (token.StartsWith("_"))
                {
                    token = token.Substring(1);
                    addVariable(token);
                }
                ovalue = findLabel(token);
                if (ovalue < 0 && pass == 2 && emode != 'E') error("Label not found: " + token);
                addToOpcode(combine, ovalue);
                if (!isNumber(token)) lastLabel = token;
                if (termCount[termCount.Count - 1] > 1) lastLabel = "-1";
            }
            token = "";
            combine = '+';
        }

        private void newExpression()
        {
            value.Clear();
            termCount.Clear();
            value.Add(0);
            termCount.Add(0);
            token = "";
            combine = '+';
        }

        private void processEndOfExpression()
        {
            if (termCount[value.Count-1] > 0)
            {
                expressions.Add(value[value.Count - 1]);
            }
            newExpression();
        }

        private void assemblyPass()
        {
            int i;
            int tmp;
            int chr;
            String line;
            value = new List<int>();
            termCount = new List<int>();
            expressions = new List<Object>();
            address = 0x000;
            radix = 8;
            endAssembly = false;
            repeats = -1;
            record = false;
            currentMacro = new List<Macro>();
            currentLine = 0;
            currentPos = 0;
            newExpression();
            outputLine = source[0];
            emode = 'O';
            line = "";
            lastLabel = "";
            chr = read();
            while (chr >= 0)
            {
                if (record)
                {
                    if (chr == 13)
                    {
                        if (line.ToUpper().Trim().StartsWith("TERM"))
                        {
                            if (pass == 1) macros[macros.Count - 1].addCommand("");
                            record = false;
                            line = "";
                        }
                        else
                        {
                            if (pass == 1) macros[macros.Count - 1].addCommand(line);
                            if (pass == 2) results += "                 " + line + "\r\n";
                            line = "";
                        }
                    }
                    else
                    {
                        line += ((char)chr).ToString();
                    }
                    chr = 0;
                }
                if (chr == ' ') processToken(chr);
                else if (chr == '-') { processToken(chr); combine = '-'; }
                else if (chr == '+') { processToken(chr); combine = '+'; }
                else if (chr == '&') { processToken(chr); combine = '&'; }
                else if (chr == '|') { processToken(chr); combine = '|'; }
                else if (chr == ' ') { processToken(chr); }
                else if (chr == '.')
                {
                    processToken(chr);
                    addToOpcode(combine, (int)address);
                }
                else if (chr == '*')
                {
                    processToken(chr);
                    addToOpcode(combine, fromOctal("010000"));
                }
                else if (chr == '=')
                {
                    emode = 'E';
                    processToken(chr);
                    if (lastLabel.Length < 1 && pass == 1)
                    {
                        error("Missing symbol for =");
                        emode = 'O';
                    }
                    else if (lastLabel.Equals("-1") && pass == 1)
                    {
                        error("Cannot set an expression using =");
                        emode = 'O';
                    }
                    else if (value.Count == 0)
                    {
                        error("= not preceeded by a symbol");
                        emode = 'O';
                    }
                    else if (value.Count > 1)
                    {
                        error("Cannot use expressions with =");
                        emode = 'O';
                    }
                    else
                    {
                        expressions.Add(lastLabel);
                        lastLabel = "";
                        emode = 'E';
                        value[value.Count - 1] = 0;
                        termCount[value.Count - 1] = 0;
                        combine = '+';
                    }
                }
                else if (chr == '/')
                {
                    processToken(chr);
                    processEndOfExpression();
                    if (expressions.Count > 0)
                    {
                        if ((int)expressions[expressions.Count - 1] < 0 && pass == 1) error("Cannot use undefined symbol in address: " + outputLine);
                        address = (UInt32)(int)expressions[expressions.Count - 1];
                        if (address > 0xffff) address = 0;
                        output("       / " + Form1.convert((UInt32)address, 18));
                        expressions.Clear();
                        emode = 'O';
                    }
                    else
                    {
                        while (chr > 0 && chr != 13) chr = read();
                        if (outputLine != null && pass == 2)
                        {
                            results += "                 " + outputLine + "\r\n";
                            outputLine = null;
                        }
                    }
                }
                else if (chr == ',')
                {
                    processToken(chr);
                    processEndOfExpression();
                    if (emode == 'R')
                    {
                        repeatString = "";
                        while (peek() > 0 && peek() != 13)
                            repeatString += ((char)read()).ToString();
                        if (peek() == 13) read();
                        repeats = (int)expressions[expressions.Count - 1];
                        expressions.Clear();
                        emode = 'O';
                    }
                    if (repeats < 0)
                    {
                        if (expressions.Count != 1)
                        {
                            if (pass == 2) error("Invalid expressions");
                        }
                        else if ((int)expressions[expressions.Count - 1] >= 0 && pass == 1) error("Label multiply defined:" + token);
                        else if (termCount[value.Count - 1] > 1 && pass == 2) error("Cannot define a lable inside an expression");
                        else if (lastLabel.Length < 1)
                        {
                            if (pass == 1) error("No symbol specified before ,");
                        }
                        if (pass == 1) addLabel(lastLabel, (int)address);
                        lastLabel = "";
                        expressions.Clear();
                        emode = 'O';
                    }

                }
                else if (chr == '(')
                {
                    value.Add(0);
                    termCount.Add(0);
                }
                else if (chr == ')')
                {
                    processToken(chr);
                    if (value.Count > 1)
                    {
                        tmp = addConstant(value[value.Count - 1]);
                        if (tmp >= 0) value[value.Count - 2] += tmp;
                        else value[value.Count - 2] = -1;
                        value.RemoveAt(value.Count - 1);
                        termCount.RemoveAt(termCount.Count - 1);
                        termCount[value.Count - 1]++;
                    }
                    else if (pass == 2) error(") without accompanying (");
                }
                else if (chr == '\t' || chr == 13)
                {
                    processToken(chr);
                    while (value.Count > 1)
                    {
                        tmp = addConstant(value[value.Count - 1]);
                        if (tmp >= 0) value[value.Count - 2] += tmp;
                        else value[value.Count - 2] = -1;
                        value.RemoveAt(value.Count - 1);
                        termCount.RemoveAt(termCount.Count - 1);
                        termCount[value.Count - 1]++;
                    }
                    processEndOfExpression();
                    if (emode == 'S')
                    {
                        startAddress = (expressions.Count > 0) ? (int)expressions[expressions.Count - 1] : 0;
                        if ((int)expressions[expressions.Count - 1] < 0 && pass == 2) error("Cannot used undefined value for start");
                        output("       > " + Form1.convert((UInt32)startAddress, 18));
                        endAssembly = true;
                    }
                    else if (emode == 'E')
                    {
                        if (expressions.Count != 2)
                        {
                            if (pass == 2) error("Invalid expressions for equate");
                        }
                        else
                        {
                            if ((int)expressions[1] < 0 && pass == 2) error("Cannot use undefined value in =");
                            addLabel((String)expressions[0], (int)expressions[1]);
                            output("       = " + Form1.convert((UInt32)(int)expressions[1], 18));
                        }
                        emode = 'O';
                    }
                    else if (expressions.Count > 0) writeMem(" ", (int)expressions[expressions.Count - 1], outputLine);
                    if (chr == 13 && outputLine != null && pass == 2)
                    {
                        results += "                 " + outputLine + "\r\n";
                        outputLine = null;
                    }
                    expressions.Clear();
                    emode = 'O';
                }
                else if ((chr >= '0' && chr <= '9') ||
                         (chr >= 'A' && chr <= 'Z') ||
                         (chr >= 'a' && chr <= 'z') ||
                         (chr == '_'))
                {
                    token += ((char)chr).ToString();
                }
                else if (pass == 2 && chr != 0) error("Invalid character in line");
                if (endAssembly) i = source.Length;
                chr = read();
            }
        }
         
            

        private String newAssembly()
        {
            int i;
            if (source.Length < 1)  return "";
            labels = new List<String>();
            labelValues = new List<int>();
            macros = new List<Macro>();
            variables = new List<String>();
            variableAddrs = new List<int>();
            constantAddrs = new List<int>();
            constantValues = new List<int>();
            defaultLabels();
            ram = cpu.Memory;
            errorCount = 0;
            startAddress = 0;
            results = "";
            pass = 1;
            assemblyPass();
            pass = 2;
            if (outputMode != 'R')
            {
                if (File.Exists(outputFilename)) outputFile = new FileStream(outputFilename,FileMode.Truncate);
                else outputFile = new FileStream(outputFilename,FileMode.OpenOrCreate);
            }
            bytesAssembled = 0;
            assemblyPass();
            if (outputMode != 'R')
            {
                if (outputMode == 'P')
                {
                    writePaperTape((UInt32)(startAddress | 0x30000));
                }
                outputFile.Close();
            }
            for (i=0; i<variables.Count; i++)
                if (variableAddrs[i] < 0)
                {
                    error("It appears that VARIABLES was not called");
                    i = variables.Count;
                }
            for (i = 0; i < constantAddrs.Count; i++)
                if (constantAddrs[i] < 0)
                {
                    error("It appears that CONSTANTS was not called");
                    i = constantAddrs.Count;
                }

            results += "\r\n";
            results += "Lines Assembled : " + source.Length.ToString() + "\r\n";
            results += "Words Used      : " + Form1.convert((UInt32)bytesAssembled, 12) + "/7777\r\n";
            results += "Errors          : " + errorCount.ToString() + "\r\n";
            return results;
        }

        public String assemble()
        {
            return newAssembly();
        }
    }
}

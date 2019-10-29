using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PDP1
{
    class Macro
    {
        private String name;
        private List<String> parameters;
        private List<String> arguments;
        private List<String> commands;
        private List<String> symbols;
        private List<int> symbolValues;
        private int symbolStart;
        private int currentLine;
        private int currentPos;
        private String macroLine;
        private String printLine;

        public Macro(String n,String arglist)
        {
            String[] args;
            name = n;
            parameters = new List<String>();
            commands = new List<String>();
            symbols = new List<String>();
            symbolValues = new List<int>();
            symbols.Add("R");
            symbolValues.Add(0);
            args = arglist.Split(',');
            foreach (String arg in args)
            {
                if (arg.Length > 0)
                {
                    parameters.Add(arg.Trim());
                }
            }
        }

        public String Name
        {
            get { return name; }
        }

        public String PrintLine
        {
            get { return printLine; }
        }

        public int SymbolStart
        {
            get { return symbolStart; }
            set { symbolStart = value; }
        }

        public void addArgument(String a)
        {
            arguments.Add(a);
        }

        public void addLabel(String label, int value)
        {
            int i;
            for (i=0; i<symbols.Count; i++)
                if (label.ToUpper().Equals(symbols[i].ToUpper())) return;
            symbols.Add(label);
            symbolValues.Add(value - symbolValues[0]);
        }

        public void addCommand(String c)
        {
            commands.Add(c);
        }

        public void addParameter(String p)
        {
            parameters.Add(p);
        }

        public void start(String arglist,UInt32 currentAddress,List<String>labels,List<int>labelValues,int pass)
        {
            int i;
            String[] args;
            symbolValues[0] = (int)currentAddress;
            arguments = new List<String>();
            args = arglist.Split(',');
            foreach (String arg in args)
            {
                if (arg.Length > 0)
                {
                    arguments.Add(arg.Trim());
                }
            }
            SymbolStart = labels.Count;
            labels.Add(symbols[0]);
            labelValues.Add(symbolValues[0]);
            if (pass == 2) for (i = 1; i < symbols.Count; i++)
            {
                labels.Add(symbols[i]);
                labelValues.Add(symbolValues[i]);
            }
            currentLine = 0;
            currentPos = 0;
            macroLine = substitute(commands[currentLine++]);
        }

        private String replaceToken(String token)
        {
            int i;
            for (i = 0; i < parameters.Count; i++)
                if (token.ToUpper().Equals(parameters[i]))
                {
                    token = (arguments.Count > i) ? arguments[i] : token;
                    i = parameters.Count;
                }
            return token;
        }

        private String substitute(String src)
        {
            String ret;
            String token;
            ret = "";
            token = "";
            while (src.Length > 0)
            {
                if ((src[0] >= '0' && src[0] <= '9') ||
                    (src[0] >= 'a' && src[0] <= 'z') ||
                    (src[0] >= 'A' && src[0] <= 'Z'))
                {
                    token += src[0];
                }
                else if (token.Length > 0)
                {
                    token = replaceToken(token);
                    ret += token;
                    ret += src[0];
                    token = "";
                }
                else
                {
                    ret += src[0];
                }
                src = src.Substring(1);
            }
            if (token.Length > 0)
            {
                token = replaceToken(token);
                ret += token;
            }
            return ret;
        }

        public int next()
        {
            if (currentPos < macroLine.Length) return macroLine[currentPos++];
            if (currentLine >= commands.Count) return -1;
            printLine = macroLine;
            macroLine = substitute(commands[currentLine++]);
            currentPos = 0;
            return 13;
        }

        public int peek()
        {
            if (currentPos < macroLine.Length) return macroLine[currentPos];
            if (currentLine >= commands.Count) return -1;
            return 13;
        }
    }
}

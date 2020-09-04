using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace Common
{
    public class Creator
    {
        public static List<Command> CreateCommandList(StringCollection lines)
        {
            List<Command> commands = new List<Command>();
            long i = 0;
            foreach (string line in lines)
            {
                commands.Add(new Command(line, ++i));
            }
            return commands;
        }

        public static StringCollection CommandListToStringCollection(List<Command> commands)
        {
            StringCollection sc = new StringCollection();
            foreach (var command in commands)
            {
                sc.Add(command.ToString());
            }
            return sc;
        }

        public static List<Command> CreateCommandList(string pathToFile)
        {
            List<Command> commands = new List<Command>();
            using (StreamReader sr = new StreamReader(pathToFile))
            {
                long i = 0;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    commands.Add(new Command(line, ++i));
                }
            }
            return commands;
        }

        public static Queue<string> CreateInputTapeFromFile(string pathToFile)
        {
            Queue<string> inputTape = new Queue<string>();
            using (StreamReader sr = new StreamReader(pathToFile))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                    inputTape.Enqueue(line);
                }
            }

            return inputTape;
        }

        public static Queue<string> CreateInputTapeFromString(string line)
        {
            if (line == string.Empty)
            {
                return null;
            }

            return new Queue<string>(line.Split(' '));
        }
    }
}

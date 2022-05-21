using System;

using System.Reflection;

using System.IO;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Threading.Tasks;

using System.Threading;

using System.Collections.Generic;

namespace command_os_project

{
    static class virtual_disk
    {
        public static FileStream disk;
        public static void inti_disk(string str)
        {

            if (!File.Exists(str))
            {
                disk = File.Create(str);
                byte[] b = new byte[1024];
                Write_cluster(0, b);
                Mini_fat.prepare_fat();
                Mini_fat.write_fat();



            }
            else
            {
                disk = new FileStream(str, FileMode.Open);
                Mini_fat.read_fat();



            }


        }
        public static byte[] read_cluster(int cluster_index)
        {
            disk.Seek(cluster_index * 1024, SeekOrigin.Begin);
            byte[] bi = new byte[1024];
            disk.Read(bi);
            return bi;


        }

        public static void Write_cluster(int cluster_index, byte[] b)
        {

            disk.Seek(cluster_index * 1024, SeekOrigin.Begin);

            disk.Write(b);
            disk.Flush();


        }
    }
    public class Mini_fat
    {
        static int[] fat = new int[1024];
        public static void prepare_fat()
        {
            for (int i = 0; i < 1024; i++)
            {
                if (i == 0 || i == 4)
                    fat[i] = -1;
                else if (i == 1 || i == 2 || i == 3)
                    fat[i] = i + 1;
                else
                    fat[i] = 0;
            }
        }
        public static void write_fat()
        {
            byte[] res = new byte[fat.Length * sizeof(int)];
            System.Buffer.BlockCopy(fat, 0, res, 0, 4096 * 4);
            for (int i = 0; i < 4; i++)
            {
                byte[] c = new byte[1024];
                for (int j = 0; j < 1024; j++)
                {
                    c[j] = res[(1024 * i) + j];
                }
                virtual_disk.Write_cluster(i + 1, c);

            }
        }
        public static void read_fat()
        {
            byte[] B = new byte[4096];
            for (int i = 0; i < 4; i++)
            {
                byte[] c = new byte[1024];

                virtual_disk.read_cluster(i + 1);
                for (int j = 0; j < 1024; j++)
                {
                    B[(1024 * i) + j] = c[j];
                }

                int[] res = new int[fat.Length * sizeof(byte)];
                System.Buffer.BlockCopy(B, 0, res, 0, res.Length);

            }
        }
        public static int get_available_block()
        {
            int i;
            for (i = 1; i < 1024; i++)
            {
                if (fat[i] == 0)
                    break;

            }
            return i;


        }



        public class directory_entery
        {
            public static char[] dir_name = new char[11];
            public static byte dir_atlr;
            public static byte[] dir_empty = new byte[12];
            public static int fristcluster;
            public static int dir_filesize;


        }
        public class directory : directory_entery
        {
            public static List<directory_entery> dirsfile = new List<directory_entery>();
            directory_entery d = new directory_entery();

            directory(char[] a)
            {
                dir_name = a;

                dirsfile.Add(new directory_entery() { });









            }

            directory parant;

            static void writedirectory()
            {
                byte[] b = new byte[dirsfile.Count * 32];
                int x = -1;
                for (int i = 0; i < dirsfile.Count; i++)
                {
                    byte[] c = BitConverter.GetBytes(dir_filesize);
                    Array.Reverse(c);

                    for (int j = 0; j < c.Length; j++)
                    {
                        x++;
                        b[x + j] = c[j];
                    }
                }
                if (b.Length > 1024)
                {
                    List<byte[]> arry = new List<byte[]> { };

                    for (int s = 0; s < b.Length / 1024; s++)
                    {
                        byte[] sb = new byte[1024];
                        for (int d = 0; d < 1024; d++)
                        {
                            sb[d] = b[(1024 * s) + d];
                        }
                        arry.Add(sb);
                        int cluster_index;
                        if (fristcluster == 0)
                        {
                            cluster_index = Mini_fat.get_available_block();
                            fristcluster = cluster_index;

                        }

                    }




                }

            }

            static void readdirectory()
            {

            }

            public void deleteDirectory()
            {


            }
        }


        class Program
        {


            public static bool CurrentDirectory { get; private set; }

            static string[] input(string str)
            {

                bool f = false; string y = "";

                for (int i = 0; i < str.Length; i++)
                {

                    if (str[i] != ' ') { y += str[i]; f = false; }

                    else { if (f == false) { y += str[i]; f = true; } }

                }

                string[] s = y.Split(' ');

                return s;

            }



            static void clear()

            {

                Console.Clear();

            }

            static void help(string arg = "")

            {

                if (arg == "")

                {

                    Console.WriteLine("\ncls Clears the screen.\n");

                    Console.WriteLine("exit Quits the CMD.EXE program (command interpreter) or the current batch script.\n");

                    Console.WriteLine("help Provides help information for Windows commands.\n");

                    Console.WriteLine("\nDisplays the name of or changes the current directory..\n");

                }

                else

                {

                    if (arg == "cls")

                    {

                        Console.WriteLine("\ncls Clears the screen.\n");

                    }

                    else if (arg == "exit")

                    {

                        Console.WriteLine("\nexit Quits the CMD.EXE program (command interpreter) or the current batch script.\n");

                    }

                    else if (arg == "help")

                    {

                        Console.WriteLine("\nhelp Provides help information for Windows commands.\n");

                    }
                    else if (arg == "cd")
                    {
                        Console.WriteLine("\nDisplays the name of or changes the current directory..\n");
                    }

                    else

                    {

                        Console.WriteLine($"\nThis command is not supported by the help utility. Try { arg} /? .\n");

                    }

                }

            }

            static void quit()

            {

                Environment.Exit(0);

            }
            static void cd(string dir = "")
            {
                if (dir == "")
                {
                    var CurrentDirectory = Directory.GetCurrentDirectory();
                    Console.WriteLine(CurrentDirectory);



                }
                else
                {


                    string path = dir;
                    try
                    {
                        Directory.SetCurrentDirectory(@path);
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        Console.WriteLine("the system cannot find the specified file.", e);
                    }




                }
            }
            static void md(string dir = "")
            {


                Directory.CreateDirectory(dir);



            }


            static void command(string[] str)

            {

                if (str[0] == "quit")

                {

                    quit();

                }

                else if (str[0] == "help")

                {

                    if (str.Length > 1)

                        help(str[1]);

                    else

                        help();

                }

                else if (str[0] == "clr")

                {

                    Console.Clear();
                }

                else if (str[0] == "")

                {

                    return;

                }
                else if (str[0] == "cd")
                {
                    if (str.Length > 1)
                    {

                        cd(str[1]);

                    }
                    else
                    {
                        cd();


                    }


                }
                else if (str[0] == "md")
                {
                   md(str[1]);
                }


                else

                {

                    Console.WriteLine("'" + str[0] + "'" + " is not recognized as an internal or external command,");
                    Console.WriteLine(" operable program or batch file.");

                }

            }

            static void Main(string[] args)

            {

                string cmd = "", arg = "";

                var CurrentDirectory = Directory.GetCurrentDirectory();




            start:


                Console.Write(Directory.GetCurrentDirectory()); Console.Write('>');

                string str = Console.ReadLine().ToLower().TrimStart().TrimEnd();

                string[] inp = input(str);

                command(inp);
                goto start;




            }

        }
    }
}


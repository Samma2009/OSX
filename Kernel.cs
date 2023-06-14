using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using PrismAPI.Graphics;
using PrismAPI;
using IL2CPU.API.Attribs;
using Cosmos.System;
using PrismAPI.Hardware.GPU;
using System.Security.Cryptography;
using Cosmos.Core.Memory;
using System.Drawing;
using Color = PrismAPI.Graphics.Color;
using System.Reflection;
using System.Net;
using Console = System.Console;
using PrismAPI.UI;
using PrismAPI.Graphics.Rasterizer;
using System.Linq;
using Cosmos.HAL.Drivers.Video.SVGAII;
using System.Threading.Tasks;
using PrismAPI.Filesystem;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using Cosmos.System.Audio;
using Cosmos.System.Audio.IO;
using PrismAPI.Audio;
using Cosmos.HAL.Drivers.Audio;
using PrismAPI.Graphics.Animation;

namespace OSX
{
    public class Kernel : Sys.Kernel
    {

        Display canvas;
        CosmosVFS vfs = new CosmosVFS();
        //AudioMixer mixer = new AudioMixer();
        [ManifestResourceStream(ResourceName = "OSX.Cursor.bmp")] 
        static byte[] file;
        [ManifestResourceStream(ResourceName = "OSX.background.bmp")]
        static byte[] logo1;
        [ManifestResourceStream(ResourceName = "OSX.startup.wav")]
        static byte[] mamaimacriminall;

        Canvas cursor = Image.FromBitmap(file);
        Canvas Background = Image.FromBitmap(logo1);
        Canvas Logo = Image.FromBitmap(mamaimacriminall);
        public static List<WindowBase> windows = new List<WindowBase>();
        public static Color backcolor,forestyle,backstyle,dodgerblue;
        public static Rectangle startbutton,mouse;
        public static int countdown,id =0;
        //public static KeyEvent key;
        public static string[] TextboxText = new string[] {"", @"Desktop\", @"Desktop\" };
        public static List<string> icons = new List<string>();
        public static List<string> folders = new List<string>();
        int appscounter = 0,appscountery = 0;
        int startuptime = 10000;
        protected override void BeforeRun()
        {

            canvas = Display.GetDisplay(1280, 720);

            canvas.Clear();

            canvas.DrawImage(canvas.Width / 2 - Logo.Width /2, canvas.Height / 2 - Logo.Height / 2,Logo,false);

            canvas.Update();

            VFSManager.RegisterVFS(vfs,true,true);
            
            MouseManager.ScreenWidth = canvas.Width;
            MouseManager.ScreenHeight = canvas.Height;
            //Background.Width = canvas.Width;
            //Background.Height = canvas.Height;

            vfs.CreateDirectory(@"0:\Desktop");

            backcolor = new Color(30, 144, 255);
            forestyle = new Color(10, 10, 10);
            backstyle = new Color(20,20,20);
            dodgerblue = new Color(30, 144, 255);
            startbutton = new Rectangle(4, canvas.Height - 40, 36, 36);

            foreach (var item in Directory.GetFiles(@"0:\Desktop"))
            {

                //File.Delete(item);

                icons.Add(item);

            }

            foreach (var item in Directory.GetDirectories(@"0:\Desktop"))
            {

                //File.Delete(item);

                folders.Add(item);

            }

        }

        public static void Refresh()
        {

            icons.Clear();

            foreach (var item in Directory.GetFiles(@"0:\Desktop"))
            {

                icons.Add(item);

            }

            folders.Clear();

            foreach (var item in Directory.GetDirectories(@"0:\Desktop"))
            {

                //File.Delete(item);

                folders.Add(item);

            }

            try
            {

                AudioStream audio = MemoryAudioStream.FromWave(mamaimacriminall);

                AudioPlayer.Play(audio);

            }
            catch
            {

                Console.Beep();

            }

        }

        protected override void Run()
        {

            //if (startup)
            //{

            //    try
            //    {

            //        AudioStream audio = MemoryAudioStream.FromWave(mamaimacriminall);

            //        AudioPlayer.Play(audio);

            //    }
            //    catch
            //    {

            //        Console.Beep();

            //    }

            //    startup = false;

            //}

            KeyEvent key;

            try
            {

                if (KeyboardManager.TryReadKey(out key))
                {

                    if (key.Key == ConsoleKeyEx.Backspace)
                    {

                        Kernel.TextboxText[id] = Kernel.TextboxText[id].Remove(Kernel.TextboxText[id].Length - 1);

                        //Console.Beep();

                    }
                    else if (key.Key == ConsoleKeyEx.Enter)
                    {

                        Kernel.TextboxText[id] += '\n';

                    }
                    else if (key.Key == ConsoleKeyEx.S && key.Modifiers == ConsoleModifiers.Control)
                    {

                        if (countdown == 0)
                        {

                            windows.Add(new SaveDialoughe(canvas, new Point(100, 100), TextboxText[id]));

                            Refresh();

                            countdown = 100;

                        }

                    }
                    //else if (key.Key == ConsoleKeyEx.G)
                    //{

                    //    if (countdown == 0)
                    //    {

                    //        AudioStream audio = MemoryAudioStream.FromWave(mamaimacriminall);

                    //        AudioPlayer.Play(audio);

                    //        countdown = 100;

                    //    }

                    //}
                    else if (key.Key == ConsoleKeyEx.M && key.Modifiers == ConsoleModifiers.Control)
                    {

                        if (countdown == 0)
                        {

                            Console.Beep();
                            icons.Clear();

                            foreach (var item in Directory.GetFiles(@"0:\Desktop"))
                            {

                                File.Delete(@"0:\Desktop\"+item);

                            }
                            Directory.Delete(@"0:\Desktop",true);
                            Directory.CreateDirectory(@"0:\Desktop");

                            //Refresh();

                            countdown = 100;

                        }

                    }
                    else
                    {

                        Kernel.TextboxText[id] += key.KeyChar;

                        //Console.Beep();

                    }

                }

            }
            catch
            {

                if (countdown == 0)
                {

                    Console.Beep();

                    countdown = 100;

                }
                
            }

            if (countdown >= 1)
                countdown--;

            mouse = new Rectangle((int)MouseManager.X, (int)MouseManager.Y, 26, 26);
            //canvas.Clear();
            canvas.DrawImage(0,0,Background,false);
            canvas.DrawString(0, 0, "OSX 140621 - REDPILL", default, Color.White);

            foreach (var item in icons)
            {

                if (Path.GetExtension(item) == ".txt")
                {

                    canvas.DrawFilledRectangle(2, (40 * appscounter) + 2, 38, 38, 0, Color.White);

                    if ((new Rectangle(2, (40 * appscounter) + 2, 38, 38).IntersectsWith(mouse) && MouseManager.MouseState == MouseState.Left && MouseManager.LastMouseState != MouseState.Left) && countdown == 0)
                    {

                        
                        //Console.Beep();
                        windows.Add(new Test(canvas, new Point(100, 100), File.ReadAllText(@"0:\Desktop\" + item)));
                        countdown = 100;

                    }

                }
                else
                {

                    canvas.DrawFilledRectangle(2, (40 * appscounter) + 2, 38, 38, 0, Color.LightGray);

                }

                appscounter++;

            }

            foreach (var item in folders)
            {

                canvas.DrawFilledRectangle(2, (40 * appscounter) + 2, 38, 38, 0, Color.GoogleYellow);

                if ((new Rectangle(2, (40 * appscounter) + 2, 38, 38).IntersectsWith(mouse) && MouseManager.MouseState == MouseState.Left && MouseManager.LastMouseState != MouseState.Left) && countdown == 0)
                {


                    //Console.Beep();
                    windows.Add(new FileManager(canvas, new Point(100, 100),@"Desktop\" + item));
                    countdown = 100;

                }

                appscounter++;

            }

            foreach (var item in windows)
            {

                item.MouseProcess();

                item.draw();

            }

            canvas.DrawFilledRectangle(2, canvas.Height - 42, (ushort)(canvas.Width -4), 40, 1, forestyle);
            canvas.DrawFilledRectangle(4, canvas.Height - 40, 36, 36, 0, Color.Red);
            if ((startbutton.IntersectsWith(mouse) && MouseManager.MouseState == MouseState.Left && MouseManager.LastMouseState != MouseState.Left) && countdown == 0)
            {

                windows.Add(new Test(canvas, new Point(100, 100),"Hello,world"));
                countdown = 100;

            }

            if (MouseManager.MouseState == MouseState.Right && MouseManager.LastMouseState != MouseState.Right && countdown == 0)
            {

                windows.Add(new DirectoryDialoughe(canvas, new Point(100, 100)));

                countdown = 100;

            }

            canvas.DrawImage((int)MouseManager.X, (int)MouseManager.Y, cursor);
            canvas.Update();

            appscounter = 0;
            appscountery = 0;

            Heap.Collect();

        }

    }

    public class WindowBase
    {

        public Display display;
        public Point position,size = new Point(400,400);
        public WindowBase(Display display, Point position)
        {

            this.display = display;
            this.position = position;

        }

        public virtual void draw(string title = "")
        {

            display.DrawFilledRectangle(position.X, position.Y, (ushort)size.X, 20, 1, Kernel.forestyle);
            display.DrawFilledRectangle(position.X + ((ushort)size.X -20), position.Y+2, 18, 18, 0, Color.Red);
            display.DrawString(position.X, position.Y, title, default, Color.White);
            display.DrawFilledRectangle(position.X, position.Y + 19, (ushort)size.X, (ushort)size.Y, 0, Kernel.backstyle);
            
        }

        public void Label(int X, int Y, string text)
        {

            display.DrawString(X + position.X, Y + position.Y + 20, text, default, Color.White);

        }

        public bool Button(int X,int Y,ushort widht,ushort height,Color color,string text)
        {

            display.DrawFilledRectangle(X + position.X,Y + position.Y + 20,widht,height,0,color);
            display.DrawString(X + position.X, Y + position.Y + 20, text,default,Color.White);

            if (new Rectangle(X + position.X, Y + position.Y + 20, widht, height).IntersectsWith(Kernel.mouse) && MouseManager.MouseState == MouseState.Left && Kernel.countdown == 0)
            {

                Kernel.countdown = 100;

                return true;

            }
            else
            {

                return false;

            }

        }

        public void TextBox(int X, int Y, ushort widht, ushort height, Color color,int ID)
        {

            display.DrawFilledRectangle(X + position.X, Y + position.Y + 20, widht, height, 0, color);
            display.DrawString(X + position.X, Y + position.Y + 20, Kernel.TextboxText[ID], default, Color.White);

            if (new Rectangle(X + position.X, Y + position.Y + 20, widht, height).IntersectsWith(Kernel.mouse) && MouseManager.MouseState == MouseState.Left && Kernel.countdown == 0)
            {

                Kernel.countdown = 100;

                Kernel.id = ID;

            }

        }

        public void MouseProcess()
        {

            if (new Rectangle(position,new Size(375,20)).IntersectsWith(Kernel.mouse) && MouseManager.MouseState == MouseState.Left)
            {

                if (Kernel.mouse.X < display.Width - ((ushort)size.X + 10) && Kernel.mouse.Y < display.Height - ((ushort)size.Y+20))
                    position = Kernel.mouse.Location;

            }

            if (new Rectangle(position.X + ((ushort)size.X - 20), position.Y + 2, 18, 18).IntersectsWith(Kernel.mouse) && MouseManager.MouseState == MouseState.Left)
            {

                Kernel.windows.Remove(this);

            }

        }

        

    }

    public class Test : WindowBase
    {

        //string textbox1text = "Notepad V0.1";

        public Test(Display display, Point position,string text) : base(display, position)
        {

            Kernel.TextboxText[0] = text;

        }

        public override void draw(string title = "Test")
        {
            base.draw("Notepad");

            TextBox(0,0,400,400,Kernel.backstyle,0);

        }

        

    }

    public class SaveDialoughe : WindowBase
    {

        string text;

        public SaveDialoughe(Display display, Point position, string text) : base(display, position)
        {

            this.text = text;
            size = new Point(400, 52);

        }

        public override void draw(string title = "Test")
        {
            base.draw("Save");

            TextBox(2, 2, 398, 20, Kernel.forestyle, 1);
            if (Button(2,25,50,20, Kernel.forestyle,"Save"))
            {

                File.WriteAllText(@"0:\" + Kernel.TextboxText[1],text);

                Kernel.Refresh();

            }

        }



    }

    public class DirectoryDialoughe : WindowBase
    {


        public DirectoryDialoughe(Display display, Point position) : base(display, position)
        {

            size = new Point(400, 52);

        }

        public override void draw(string title = "Test")
        {
            base.draw("Create Directory");

            TextBox(2, 2, 398, 20, Kernel.forestyle, 2);
            if (Button(2, 25, 50, 20, Kernel.forestyle, "Create"))
            {

                Directory.CreateDirectory(@"0:\" + Kernel.TextboxText[2]);

                Kernel.Refresh();

            }

        }



    }

    public class FileManager : WindowBase
    {

        List<string> icons = new List<string>();
        List<string> folders = new List<string>();
        string directory;
        int icnnum;

        public FileManager(Display display, Point position,string directory) : base(display, position)
        {

            size = new Point(400, 400);

            this.directory = directory;

            foreach (var item in Directory.GetFiles(@"0:\" + directory))
            {

                icons.Add(item);

            }

            foreach (var item in Directory.GetDirectories(@"0:\" + directory))
            {

                //File.Delete(item);

                folders.Add(item);

            }

        }

        public override void draw(string title = "Test")
        {
            base.draw("File Manager");

            FileExplorer(0, 0, 400, 400, Kernel.backstyle);

            icnnum = 0;
        }

        public void FileExplorer(int X, int Y, ushort widht, ushort height, Color color)
        {

            display.DrawFilledRectangle(X + position.X, Y + position.Y + 20, widht, height, 0, color);

            foreach (var item in icons)
            {

                display.DrawFilledRectangle(X+position.X,40*icnnum + (Y+position.Y) + 20/*X + position.X + 2, Y+((40 * icnnum) + 2 + position.Y)*/, 36, 36, 0, Color.White);

                icnnum++;

            }

            foreach (var item in folders)
            {

                display.DrawFilledRectangle(X + position.X, 40 * icnnum + (Y + position.Y) + 20, 36, 36, 0, Color.GoogleYellow);

                icnnum++;

            }

        }

    }

}

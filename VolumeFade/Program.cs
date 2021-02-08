using CoreAudio;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Org.AvaLanCS.VolumeFadeout
{
    public class MainClass
    {
        static int fadeoutTime = 5000; // how many milliseconds till the program's volume is set to 0
        static float decrease_rate = 1.0f; // volume will change according to 1 - t^decrease_rate
        static String programToLookFor = ""; // the window name that we are going to look for

        static void Main(string[] args)
        {
            askForParameters(args);

            var program = findProgram();
            if(program == null)
            {
                Console.WriteLine("Program not found! Exiting...");
                Environment.Exit(-2);
            }

            FadeoutTimer timer = new FadeoutTimer(ref program);
            timer.start(fadeoutTime, decrease_rate);
        }

        static void askForParameters(String[] args)
        {
            if(args.Length == 0 || args.Length > 3)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("VolumeFadeout.exe <program name> [fadeout time] [decrease rate]");
                Console.WriteLine("<program name>: part or the entire name of the program you want to fade out");
                Console.WriteLine("\tthis will be searched in the process name and the window title");
                Console.WriteLine("[fadeout time]: optional, the number of milliseconds to fade out the program. Default is 5000 if ommitted");
                Console.WriteLine("[decrease rate]: optional, the volume is decreased based on 1-t^decrease rate . Default is 1");
                Environment.Exit(0);
            }

            if (args.Length >= 1)
            {
                programToLookFor = args[0];
            } 
            
            if(args.Length >= 2)
            {
                try
                {
                    fadeoutTime = Int32.Parse(args[1]);
                } catch(FormatException e)
                {
                    Console.WriteLine("The format for the fadeout time is invalid: " + args[1]);
                    throw e;
                }
            }

            if (args.Length == 3)
            {
                try
                {
                    decrease_rate = (float)Double.Parse(args[2], NumberStyles.Any, CultureInfo.InvariantCulture);
                }
                catch (FormatException e)
                {
                    Console.WriteLine("The format for the decrease rate is invalid: " + args[2]);
                    throw e;
                }
            }
        }

        static AudioSessionControl2 findProgram()
        {
            // Loop throught the audio sessions
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            MMDevice device = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            SessionCollection sessions = device.AudioSessionManager2.Sessions;

            AudioSessionControl2 program = null;

            foreach (var session in sessions)
            {
                Process p = Process.GetProcessById((int)session.GetProcessID);
                if (p.ProcessName.Contains(programToLookFor) || p.MainWindowTitle.Contains(programToLookFor))
                {
                    program = session;
                    Console.WriteLine("Found program with\nprocess: {0}\nwindow title: {1}", p.ProcessName, p.MainWindowTitle);
                    break;
                }
            }

            return program;
        }
    }
}

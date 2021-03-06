﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Reflection;
using System.Diagnostics;

namespace AbraxisToolsetInstaller {
    class Program {

        public static Random r;

        public const string abraxisToolsetDownloadLink = "https://github.com/NecropolisModding/AbraxisToolset/releases/download/1.5/";
        public const string toolsetDLL = "AbraxisToolset.dll";
        public const string discordDLL = "discord-rpc.dll";

        public static bool isAbraxis = false;

        static void Main(string[] args) {
            r = new Random();

            try {
                FirstTimeTool();
            } catch( System.Exception e ) {
                Console.WriteLine( e );
                Console.WriteLine( "Press any key to exit..." );
            }

            Console.ReadLine();
        }

        private static void FirstTimeTool() {
            Console.WriteLine( "Welcome to the Abraxis Toolset Installer v0.1 \n" );
            Thread.Sleep( 500 );

            DisplayIntroText();

            DisplayText( "Brazen : I guess I'll have to help you set up, won't I, first-timer?" );
            Thread.Sleep( 500 );
            DisplayText( "Brazen : This may seem like an odd question... But where IS the Necropolis?" );
            Thread.Sleep( 200 );
            Console.WriteLine( "Enter the path to the Necropolis game. 'C:Program Files(x86)/steamapps/common/Necropolis' is a good place to check first." );
            Console.WriteLine();

            string path = Console.ReadLine();

            Console.WriteLine();

            bool doesExist = Directory.Exists( path );
            bool isNecropolis = IsNecropolisFolder( path );

            while( !doesExist || !isNecropolis ) {

                if( path == string.Empty ) {
                    path = Console.ReadLine();
                    continue;
                }

                if( !doesExist ) {
                    DisplayText( "Brazen : There's... Nothing here." );
                    Console.WriteLine( "The directory you put in doesn't exist, try again" );
                    path = Console.ReadLine();
                } else if( !isNecropolis ) {
                    DisplayText( "Brazen : That doesn't look like the Necropolis" );
                    Console.WriteLine( "The directory you put in isn't Necropolis. Make sure it's not Necropolis.exe or Necropolis_Data" );
                    path = Console.ReadLine();
                }
                doesExist = Directory.Exists( path );
                isNecropolis = IsNecropolisFolder( path );
            }

            DisplayText( "Brazen : Ah! There it is! Next, decide if you want to use a custom toolset, or Abraxis' own." );
            while( true ) {
                Console.WriteLine( "Enter either [A] for Abraxis' toolset, or [C] for custom toolset. [U] will uninstall all toolsets" );

                string readLine = Console.ReadLine();

                while( readLine != "A" && readLine != "C" && readLine != "U" ) {
                    Console.WriteLine( "Enter either [A] for Abraxis' toolset, [C] for custom toolset, or [U] for uninstall" );
                    readLine = Console.ReadLine();
                }

                string assemblyName = "Assembly-CSharp.dll";

                //Backup files
                {

                    string managedDir = path + "/Necropolis_Data/Managed/";
                    string backupDir = path + "/Necropolis_Data/Managed/Backup/";

                    if( !Directory.Exists( backupDir ) ) {
                        string[] managedFiles = Directory.GetFiles( managedDir );
                        Directory.CreateDirectory( backupDir );

                        foreach( string s in managedFiles ) {
                            string newPath = backupDir + Path.GetFileName( s );
                            File.Copy( s, newPath );
                        }
                    }

                }

                //Abraxis toolset
                if( readLine == "A" ) {

                    DisplayText( "Brazen : Abraxis' toolset it is! Let me get all the pieces here..." );
                    Console.WriteLine( "Downloading Abraxis' Toolset..." );

                    //Downloading process
                    {
                        WebClient webClient = new WebClient();
                        string dllPath = Environment.CurrentDirectory + "/Patching/Assembly-CSharp.mm.dll";
                        if( File.Exists( dllPath ) )
                            File.Delete( dllPath );
                        webClient.DownloadFile( abraxisToolsetDownloadLink + toolsetDLL, dllPath );

                        if( !Directory.Exists( Environment.CurrentDirectory + "/Patching/Discord" ) )
                            Directory.CreateDirectory( Environment.CurrentDirectory + "/Patching/Discord");

                        dllPath = Environment.CurrentDirectory + "/Patching/Discord/discord-rpc.dll";
                        webClient.DownloadFile( abraxisToolsetDownloadLink + discordDLL, dllPath );
                    }

                    isAbraxis = true;
                } else if( readLine == "C" ) { //Custom toolset

                    DisplayText( "Brazen : A custom one, eh? Just let me know where it is." );
                    Console.WriteLine( "Enter path to the mod .dll" );

                    string dllPath = Console.ReadLine();

                    while( !File.Exists( dllPath ) || Path.GetExtension( dllPath ) != ".dll" ) {
                        DisplayText( "Brazen : That doesn't look like a toolset to me..." );
                        Console.WriteLine( "Enter path to the mod .dll" );
                        dllPath = Console.ReadLine();
                    }

                    DisplayText( "Brazen : This should do just fine. Give me a few moments to move this around..." );
                    if( File.Exists( Environment.CurrentDirectory + "/Patching/Assembly-CSharp.mm.dll" ) )
                        File.Delete( Environment.CurrentDirectory + "/Patching/Assembly-CSharp.mm.dll" );
                    File.Copy( dllPath, Environment.CurrentDirectory + "/Patching/Assembly-CSharp.mm.dll" );
                } else {
                    string mangedDir = path + "/Necropolis_Data/Managed/";
                    string backupDir = path + "/Necropolis_Data/Managed/Backup";

                    if( !Directory.Exists( backupDir ) ) {
                        DisplayText( "Brazen : There's no backup! " );
                        continue;
                    }

                    DisplayText( "Brazen : You want to restore to how the the Necropolis was before? I can do that." );
                    DisplayText( "Brazen : Restoring..." );

                    string[] backupFiles = Directory.GetFiles( backupDir );

                    foreach( string s in backupFiles ) {
                        string newPath = mangedDir + Path.GetFileName( s );

                        if( File.Exists( newPath ) )
                            File.Delete( newPath );

                        File.Copy( s, newPath );
                    }

                    DisplayText( "Brazen : And we're done. The Necropolis has been restored to it's previous state!" );
                    Console.WriteLine( "Press enter to exit." );

                    Console.Read();
                    return;
                }

                DisplayText( "Brazen : That's that! One last step, and we're done!" );
                Thread.Sleep( 300 );
                PatchGame( path );

                DisplayText( "Brazen : And we're done. Feel free to enter the Necropolis whenever you want now, the tools are all ready." );
                Console.WriteLine( "Press enter to exit." );

                Console.Read();
                return;
            }
        }

        private static void PatchGame(string gamePath) {

            string patchingFolder = Environment.CurrentDirectory + "/Patching/";

            //Copy dependencies from Necropolis folder.

            string[] files = Directory.GetFiles( gamePath + "/Necropolis_Data/Managed/" );

            List<string> tempFiles = new List<string>();

            foreach( string s in files ) {
                tempFiles.Add( Path.GetFileName( s ) );
                if( !File.Exists( patchingFolder + Path.GetFileName( s ) ) )
                    File.Copy( s, patchingFolder + Path.GetFileName( s ) );
            }

            //Patch

            try {
                Process mmProcess = new Process();
                mmProcess.StartInfo.WorkingDirectory = patchingFolder;
                mmProcess.StartInfo.FileName = patchingFolder + "MonoMod.exe";
                mmProcess.StartInfo.Arguments = "Assembly-CSharp.dll";

                mmProcess.Start();

                while( !mmProcess.HasExited )
                    Thread.Sleep( 100 );
            } catch( System.Exception e ) {
                Console.WriteLine( e );
            }

            //Delete temp files
            foreach( string s in tempFiles ) {
                File.Delete( patchingFolder + s );
            }

            //Copy modified file back to Necropolis
            if( File.Exists( gamePath + "/Necropolis_Data/Managed/Assembly-CSharp.dll" ) )
                File.Delete( gamePath + "/Necropolis_Data/Managed/Assembly-CSharp.dll" );
            File.Copy( patchingFolder + "MONOMODDED_Assembly-CSharp.dll", gamePath + "/Necropolis_Data/Managed/Assembly-CSharp.dll" );

            if( isAbraxis ) {
                string discordDLLPath = gamePath + "/Necropolis_Data/Plugins/discord-rpc.dll";
                if( File.Exists(discordDLLPath) )
                    File.Delete( discordDLLPath );
                File.Copy( patchingFolder + "Discord/discord-rpc.dll" , discordDLLPath);
            }
        }

        private static void DisplayIntroText() {
            DisplayText( "Brazen : Oh... It looks like you've stumbled across some of the tools Abraxis used to create the Necropolis." );
            DisplayText( "Brazen : Well aren't you clever? I suppose I don't mind if you use them. It'll help alleviate the boredom, that's for sure." );
        }
        public static void DisplayText(string s, bool newLine = true, float randomMin = 0.005f, float randomMax = 0.01f, int postWait = 200) {
            for( int i = 0; i < s.Length; i++ ) {
                Console.Write( s[i] );
                float rand = lerp( randomMin, randomMax, (float)r.NextDouble() );
                Thread.Sleep( (int)( rand * 1000 ) );
            }

            if( newLine ) {
                Console.Write( "\n" );
            }
            Thread.Sleep( postWait );
        }
        public static float lerp(float v0, float v1, float t) {
            return ( 1 - t ) * v0 + t * v1;
        }

        public static bool IsNecropolisFolder(string path) {
            string[] childDirectories = Directory.GetDirectories( path );
            string[] files = Directory.GetFiles( path );

            bool hasDirectory = false;

            foreach( string s in childDirectories ) {
                //Console.WriteLine( "Checking " + Path.GetFileName( s ) + " as data folder" );
                if( Path.GetFileName( s ) == "Necropolis_Data" ) {
                    hasDirectory = true;
                    break;
                }
            }

            if( !hasDirectory )
                return false;

            bool hasFile = false;

            foreach( string s in files ) {
                //Console.WriteLine( "Checking " + Path.GetFileName(s) + " as exe file" );
                if( Path.GetFileName( s ) == "Necropolis.exe" ) {
                    hasFile = true;
                    break;
                }
            }

            if( !hasFile )
                return false;
            else
                return true;

        }
    }
}

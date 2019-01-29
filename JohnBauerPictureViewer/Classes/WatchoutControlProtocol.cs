using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.UI.Xaml.Controls;

namespace JohnBauerPictureViewer.Classes
{
    public class InputValue
    {
        public string Name { get; internal set; }
        public float Value { get; internal set; }
    }

    public class WatchoutControlProtocol
    {
        //  In order to accept commands on either of those ports, you must enable those options in the Preferences dialog box’ Control tab. 


        private int _ControlOptionsPort = 3040;
        private int _NetworkControlPort = 3039;
        private static StreamSocket streamSocket;
        private bool Connected = false;
        private TextBlock _ResultTextBlock;

        private string _Host;
        private string _PortNumber;

        public WatchoutControlProtocol(string Host, string PortNumber, TextBlock ResultTextBlock)
        {
            _Host = Host;
            _PortNumber = PortNumber;
            _ResultTextBlock = ResultTextBlock;
        }
        

        private void ShowMessage(string Message)
        {
            if (_ResultTextBlock != null)
            {
                _ResultTextBlock.Text += Message + "\n";
            }
        }

        public async Task SendCommand(string command)
        {
            if ((_Host != null) && (_PortNumber != null))
            {

                ShowMessage("Connecting to " + _Host + ":" + _PortNumber);

                streamSocket = new Windows.Networking.Sockets.StreamSocket();

                try
                {
                    var hostName = new Windows.Networking.HostName(_Host);

                    await streamSocket.ConnectAsync(hostName, _PortNumber);

                    ShowMessage("Connected!");

                    using (Stream outputStream = streamSocket.OutputStream.AsStreamForWrite())
                    {
                        using (var streamWriter = new StreamWriter(outputStream))
                        {
                            await streamWriter.WriteLineAsync(command);
                            await streamWriter.FlushAsync();
                            ShowMessage("Sent!");
                        }
                    }

                    // Read data from the echo server.
                    string response;
                    using (Stream inputStream = streamSocket.InputStream.AsStreamForRead())
                    {
                        using (StreamReader streamReader = new StreamReader(inputStream))
                        {
                            if (!streamReader.EndOfStream)
                            {
                                response = await streamReader.ReadLineAsync();
                                ShowMessage(response);
                            }
                        }
                    }

                    streamSocket.Dispose();

                }
                catch (Exception E)
                {
                    ShowMessage(E.Message);
                }
            }
        }
        
        //online Go online or offline.Production software only.
        private void Online()
        {
            SendCommand("online");
        }
        //halt Stop running, optionally specifying an auxiliary timeline name. 
        private void Halt()
        {
            SendCommand("halt");
        }
        //kill Stop and deactivate the named auxiliary timeline. 
        private void Kill()
        {
            SendCommand("kill");
        }
        //gotoTime Jump to a time position. 
        private void GotoTime(int milliseconds)
        {
            SendCommand("gotoTime " + milliseconds.ToString());
        }
        //gotoControlCue Jump to the time position of a named Control cue.
        private void GotoControlCue(string cueName)
        {
            SendCommand("gotoControlCue \"" + cueName+"\"");
        }
        //enableLayerCond Turn conditional layers on or off.
        private void EnableLayerCond(int layer)
        {
            SendCommand("enableLayerCond " + layer.ToString());
        }
        //standBy Enter/exit standby mode.
        private void StandBy(bool enter)
        {
            if (enter)
            {
                SendCommand("standBy true");
            }
            else
            {
                SendCommand("standBy false");
            }
        }

        internal void Close()
        {
            
        }

        //getStatus retrieves name and status of the currently show and its timelines. 
        private void getStatus()
        {
            SendCommand("getStatus");
        }
        //reset reset and stop all timelines. 
        private void Reset()
        {
            SendCommand("reset");
        }
        //setInput Set the value of a named Input, with optional fade-rate in mS.
        private void SetInput(string Name, float Value)
        {
            SendCommand("setInput \"" + Name + "\" " + Value.ToString());
        }
        //setInputs Sets the values of multiple inputs together. 
        private void SetInputs(string Name, int milliseconds, List<InputValue> Values)
        {
            string valueString = "";

            foreach (var value in Values)
            {
                valueString += " \"" + value.Name + "\" " + value.Value.ToString();
            }

            SendCommand("setInputs " +milliseconds.ToString()+" "+ valueString);
        }
        //wait Waits for the entire display cluster to become established(cluster only). 
        private void Wait()
        {
            SendCommand("wait");
        }
        //serialPort Opens a serial port for control protocol use(cluster only).
        private void SerialPort(bool Open, string Name)
        {
            if (Open)
            {
                SendCommand("serialPort true \"" +Name+"\"");
            }
            else
            {
                SendCommand("serialPort false \"" + Name + "\"");
            }
        }
        //timecodeMode Activates LTC(SMPTE/EBU) timecode control(cluster only). 
        private void TimecodeMode()
        {
            SendCommand("timecodeMode");
        }
        //hitTest Get information of interactive media cues
        private void HitTest()
        {
            SendCommand("hitTest");
        }

        //ping Do-nothing command, causing a ready feedback message to be sent.
        private void Ping()
        {
            SendCommand("ping");
        }

        //run Start running, optionally specifying an auxiliary timeline name. 
        private void Run(string v)
        {
            SendCommand("run " +v);
        }

        //load Load a show and get ready to run. Parameter syntax differs.
        private void Load(string ShowName)
        {
            SendCommand("load \"" +ShowName+"\"");
        }

        //delay Introduces a delay between commands (command file use only). 
        private void Delay(int v)
        {
            SendCommand("delay " + v.ToString());
        }

        //setLogoString Displays a message on the startup screen.
        private void SetLogoString(string LogoString)
        {
            SendCommand("setLogoString \"" + LogoString + "\"");
        }

        //authenticate Perform authentication.required only for display software.
        public void Authenticate(int Level)
        {
            SendCommand("authenticate " + Level.ToString());

        }

        private async Task<string> SendMessage(string request)
        {
            ShowMessage("Sending: "+request);
            try
            {
                using (Stream outputStream = streamSocket.OutputStream.AsStreamForWrite())
                {
                    using (var streamWriter = new StreamWriter(outputStream))
                    {
                        await streamWriter.WriteLineAsync(request);
                        await streamWriter.WriteLineAsync("ping");
                        await streamWriter.FlushAsync();
                        ShowMessage("Sent!");
                    }
                }
                
                // Read data from the echo server.
                string response="";
                using (Stream inputStream = streamSocket.InputStream.AsStreamForRead())
                {
                    using (StreamReader streamReader = new StreamReader(inputStream))
                    {
                        //int ch = streamReader.Peek();
                        //if (ch != 0)
                        {

                            response = await streamReader.ReadToEndAsync();
                            ShowMessage("Response: " + response);
                        }
                    }
                }
                
                return response;
            }
            catch(Exception E)
            {
                ShowMessage(E.Message);

            }

            return "End SendMessage";


            //COMMAND ID TAGGING
            //Commands may optionally be preceded by a command ID. 
            //This is any sequence of characters enclosed in square brackets. 
            //When used, at least one explicit reply is always sent for each command. 
            //The reply is then also tagged with the same ID: 
            //[23]ping
            //[23] Ready “2.0” “WATCHPOINT” “Windows” true 
            //Use this feature if you want positive confirmation of commands, 
            //or to explicitly associate a feedback message with a command.
        }



        public async void StartClient(string Host, string PortNumber)
        {
            try
            {
                // Create the StreamSocket and establish a connection to the echo server.
                using (var streamSocket = new Windows.Networking.Sockets.StreamSocket())
                {
                    var hostName = new Windows.Networking.HostName(Host);
                    
                    await streamSocket.ConnectAsync(hostName, PortNumber);
                    
                    // Send a request to the echo server.
                    string request = "Hello, World!";

                    using (Stream outputStream = streamSocket.OutputStream.AsStreamForWrite())
                    {
                        using (var streamWriter = new StreamWriter(outputStream))
                        {
                            await streamWriter.WriteLineAsync(request);
                            await streamWriter.FlushAsync();
                        }
                    }

                    
                    // Read data from the echo server.
                    string response;
                    using (Stream inputStream = streamSocket.InputStream.AsStreamForRead())
                    {
                        using (StreamReader streamReader = new StreamReader(inputStream))
                        {
                            response = await streamReader.ReadLineAsync();
                        }
                    }

                    
                }
                
            }
            catch (Exception ex)
            {
                Windows.Networking.Sockets.SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
            }
        }

    }




    /*
     
     You can control the production computer or a cluster of WATCHOUT display computers from other programs or systems, including general purpose control systems such as AMX or Crestron. This is usually done through the network, but other options (such as serial port or a script file) are also available. The control protocol uses simple and readable command strings, making it easy to learn and implement from an external system. To get started with controlling WATCHOUT, use a TELNET client to connect to WATCHOUT and give the commands from there. Both Windows and MacOS X come with a TELNET client (although under Windows it needs to be enabled before it can be used). 
COMMAND FORMAT DETAILS Each command is sent as a string, terminated by a carriage return, line feed or Cr/LF pair. A command consists of a command name, sometimes followed by parameters. Commands are case-sensitive. Commands are encoded using the UTF-8 UNICODE character format, which is downward compatible with ASCII.
IMPOrTANT: Commands containing non-ASCII characters – such as å, ä, ü, ç – must be encoded using the UTF-8 format in order to work properly. String parameters are sent within double quotes. Backslash is used as an escape character (that is, to encode a double quote in a string, precede it with a backslash). To send a backslash character, use “\\” inside the string. Commands are case-sensitive. Successfully performed commands are normally not acknowledged (although you can use the Command ID Tagging option to force explicit feedback for all commands). While most commands are available in both the production and display programs, not all are. Furthermore, in a few cases, parameters to commands are slightly different in the two programs. Such differences are noted in the detailed descriptions of commands in this appendix. WATCHOUT uses a simple text format for its commands. Some commands may return a reply, error message or other kind of response to the controller. Such responses are also sent as text.
Parameters In commands that take parameters, the parameters are separated by white-space only. Optional parameters are shown in square brackets, like this: [<uint>]. Parameter types are described below. <string>. String parameters are sent within double quotes: “This is a string” Backslash is used as an escape character (that is, to encode a double quote in a string, precede it with a backslash). To send a backslash character, use “\\” inside the string. <uint> and <int>. An unsigned or possibly signed decimal integral number: 123 <float>. A possibly signed decimal number with an optional fractional part: 0.25 <bool>. The keyword true or false, with no quotes: true
CONTrOL PrOTOCOL 159
CONTROLLING THE PRODUCTION SOFTWARE In cases where you must keep the production software in the system for some reason, you connect to and control the production computer, which in its turn controls the display computers. One advantage of this method is that you can clearly see you commands as they occur in the production computer’s user interface, making it easier to try things out.
NOTE: Even when your goal is to control the display computers directly, it may help to start by controlling the production computer as it makes it easier to see what’s happening. Once things work as desired, remove the production computer and target the display cluster directly.
Control Options You can control the production software only through the network. It uses IP port number 3040. Commands can be sent over either TCP or UDP. In order to accept commands on either of those ports, you must enable those options in the Preferences dialog box’ Control tab. When using UDP, no feedback is available due to the connectionless nature of the UDP method. Hence, for most applications where reliability or feedback is important, always use the TCP method.
IMPOrTANT: If you use the “load” command to load another show, make sure the desired control options are enabled in that show as well, or the connection will be closed.
CONTROLLING THE DISPLAY SOFTWARE Before attempting to control a display cluster, ensure that the show has been successfully run from WATCHOUT production soft ware (meaning that all media files have been transferred, etc). Then quit the WATCHOUT production software.
Authentication Before you can give any command to the display software (with the exception of the “ping” command), you must specify the authentication level. To control WATCHOUT display software, you need authentication level 1: authenticate 1 WATCHOUT responds with a ready message. You can now send other commands.
CONTROL OPTIONS You can control the display cluster either using a file stored on the primary display computer in the cluster, or using commands sent via the network or through a serial port.
File-based Control You can use a script file stored on the primary display computer’s hard disk to automatically perform most commands in this protocol when starting the display software. This can, for example, be used to set up a system that automatically loads and runs a show every time the computer is switched on. See “Startup Script” for more on how to set up a script on your display computer. The example below shows the content of a command file that displays a message on the WATCHOUT screen, waits a few seconds, loads a show, waits for any other computers in the WATCHOUT display cluster to become ready, and then runs the show. authenticate 1 setLogoString “The show will begin shortly” delay 5000 

    
    load “MyShow” wait run See each individual command later in this appendix for details.
IMPOrTANT: If using non-ASCII characters in the show name (for example; å, ö, ü, ß), your text editor must be able to save the text using the UTF-8 encoding. If you’re unsure about this, it’s usually easiest to rename the show to avoid non-ASCII characters.
NOTE: Any errors occurring while executing commands from such a command file are displayed in a console window. However, to see this window, you have to close the main WATCHOUT display window by pressing Ctrl-W. Keep this in mind if your command file doesn’t work as  expected.
Network Control To control a WATCHOUT display cluster via the network, connect to TCP/IP port number 3039 of one of the WATCHOUT display computers. The computer with which you initiate communication becomes the master of the cluster, and will automatically control the other cluster members, as specified by the presentation loaded using the “load” command. Use a TELNET program to check the communication. Open the port specified above, then type “ping” and press return. WATCHOUT will respond with a ready message, stating its version number and some other details.
Serial Control To control a WATCHOUT display cluster through a serial port, connect the controlling device to a serial port of one of the WATCHOUT display computers. This computer becomes the master of the cluster, and will automat ically control the other cluster members, as specified by the presentation loaded using the “load” command. No serial port is open by default. Use the serialPort command to open a serial port. Put this command into a text file, and use the file-based control feature to perform the serialPort command (see “Filebased Control”).
CONTrOL PrOTOCOL 161
LIST OF COMMANDS The table below shows the available commands. Some are explained in greater depth on the following pages.
Command Description ping Do-nothing command, causing a ready feedback message to be sent. authenticate Perform authentication. required only for display software. load Load a show and get ready to run. Parameter syntax differs. online Go online or offline. Production software only. run Start running, optionally specifying an auxiliary timeline name. halt Stop running, optionally specifying an auxiliary timeline name. kill Stop and deactivate the named auxiliary timeline. gotoTime Jump to a time position. gotoControlCue Jump to the time position of a named Control cue. enableLayerCond Turn conditional layers on or off. standBy Enter/exit standby mode. getStatus retrieves name and status of the currently show and its timelines. reset reset and stop all timelines. setInput Set the value of a named Input, with optional fade-rate in mS. setInputs Sets the values of multiple inputs together. setLogoString Displays a message on the startup screen. delay Introduces a delay between commands (command file use only). wait Waits for the entire display cluster to become established (cluster only). serialPort Opens a serial port for control protocol use (cluster only). timecodeMode Activates LTC (SMPTE/EBU) timecode control (cluster only). hitTest Get information of interactive media cues
authenticate This command is required for the display software prior to issuing any other commands except the ping command. While this command is accepted by the production software, it is not required. See “Authentication” above for details.
load (production software version) Loads a show by name. The name is specified as a quoted string containing the full path to the file. The use of backslash characters in Windows path names conflicts with the use of the backslash as an escape character in this protocol. Either double the backslash characters, or use forward slashes instead (as shown in this example): load “C:/Samples/ExampleShow.watch” Parameter Description <string> Path to the show to be loaded. [<uint>] Conditional layer enable flags, least significant bit is condition 1.
CONTrOL PrOTOCOL 162
Parameter Description [<bool>] Go online automatically. Defaults to true.
NOTE: When using this load command, the production software will automatically go online after loading the show. If desired, you can add a numeric parameter to override the conditional layer settings of the show being loaded. For example, to enable condition 1 and 2 only: load “C:/Samples/ExampleShow.watch” 3 The number is a sum of the decimal numbers corresponding to each desired condition, as shown in the table below.
Condition Number 1 1 2 2 3 4 4 8 5 16 6 32 …and so on.
The last optional parameter is a boolean controlling whether the production software will go online or not, after loading the show (default value is true): load “C:/Samples/ExampleShow.watch” 3 false loads the specified show and sets its layer conditions, but remains offline.
load (display software version) Load a complete show specification from a local file associated with the show name specified by the first parameter. Busy feedback may be sent to the host while loading, informing the host about the progress. If errors occur, “Error” feedback is sent. Finally, a ready feedback message is sent, regardless of whether any error occurred. load “Phantom” Parameter Description <string> Name of the show to be loaded. [<bool>] Manage cluster loading and feedback. Defaults to true. [<bool>] Designate as the master display computer. Defaults to true. [<uint>] Conditional layer enable flags, least significant bit is condition 1.
NOTE: You can not specify a folder path to the show. The show must be present in the “Shows” folder, located in the same folder as the WATCHOUT display software.
CONTrOL PrOTOCOL 163
online This command applies to the production software only. It takes a single boolean parameter, specifying whether to go online (true) or offline (false). Note that the load command also goes online unless explicitly disabled by the optional parameter.
gotoTime Jump to the specified time position along the timeline. gotoTime 5000 Parameter Description <uint> or <string> Time position to go to, in milliseconds, or as a string in this format: “HH:MM:SS.FFF”, where FFF is milliseconds. [<string>] Name of auxiliary timeline to control (omit for main timeline).
gotoControlCue Jump to the time of specified Control cue. If the optional “reverse only” boolean is set to true, it searches for the Control cue only back in time from the current time position. Otherwise it searches first forward then reverse. The command does not change the run mode of the timeline. If the specified cue is not found, the timeline’s state will not change, and a runtime error message to this effect will be returned. gotoControlCue “William” true Parameter Description <string> Name of Control cue to look for. [<bool>] Search in reverse only if true. If false or not specified, then search both ways. [<string>] Name of auxiliary timeline to control (omit for main timeline).
enableLayerCond Change the set of enabled layer conditions. While the layer conditions can be specified as part of the load command, this separate command allows the layer conditions to be changed without loading another show. The command takes a single, mandatory <uint> parameter with the same interpretation as the conditional layers parameter of the load command.
setLogoString This command applies to the display software only. Display its single string parameter next to the WATCHOUT logo, when shown on screen. 
standBy Enter/exit standby mode. In standby, the display and sound is muted, or media on standby layers – if any – is performed . This mode can be entered/exited smoothly, by specifying a fade rate. standBy true 1000
CONTrOL PrOTOCOL 164
Fade out sound and image over one second and enter standby mode. If any standby layer is available, its media is performed instead.
Parameter Description <bool> Enter standby if true, exit if false. [<uint>] Fade rate, in milliseconds. Defaults to zero if not specified.
getStatus Get the current status of the WATCHOUT cluster master. getStatus Reply “WO2Launch” false 0 true true false 122 true Responds with a Reply with the following parameters:
Reply Parameter Description <string> Name of the show. Empty string if no show loaded. <bool> Busy. True if the master display computer or any of its slaves is busy <uint> General health status of the cluster; 0: OK, 1: Suboptimal, 2: Problems, 3: Dead. <bool> Display is open (in its full screen mode). <bool> Show is active (ready to run). <bool> Programmer is on line. [<uint>] Current time position, in milliseconds (only included if show is active). [<bool>] Show is playing – false if paused (only included if show is active). [<float>] Timeline rate (nominally 1, only included if show is active). [<bool>] Standby mode (true if in standby, only included if show is active)
delay Wait the number of milliseconds specified by the parameter before performing the next command in the file.  
NOTE: Performed only when used in a command file. Applies to display software only.
wait This command applies to the display software only. Wait for the display cluster to become fully established before proceeding with the next command in the file. Waits at most the number of milliseconds specified by the parameter. 
NOTE: Performed only when used in a command file.
CONTrOL PrOTOCOL 165
setInput Sets the value of a named input (see “Inputs”). 
Parameter Description <string> The name of the input to set. <float> The desired value, optionally prefixed by + or - for incremental change. [<uint>] Optional transition rate, in milliseconds.
setInput “uno” 0.5 The value is generally in the range 0 through 1, but may be extended to cover a wider range using the Limit setting of the Generic Input . By prefixing the value with a plus or minus sign, you can adjust the value incrementally relative to its current setting. This example increases the value of the input by 0.1: setInput “uno” +0.1 A third, optional parameter allows you to specify a transition rate, causing any property controlled by the input to change gradually to the specified target value. This parameter is specified in milliseconds.
NOTE: While you would typically use this command to set the value of a Generic Input, you may use it to set the value for any input. If data is also provided by a MIDI or DMX-512 source, the latest data will take precedence.
setInputs This command is functionally equivalent to the setInput command above, but allows you to set multiple inputs with a single command. This is useful in the following cases: • When it is important that several inputs are set simultaneously, because their values are used in an interdependent way. If you use two separate setInput commands, it is possible that the commands will not be executed on the same frame. • When setting a large number of inputs, as it is more efficient to handle this with a single command than a number of separate commands, allowing you to set a larger number of values within a single frame. The first parameter is the transition rate, in mS. Subsequent data must be provided in groups of two parameters:
Parameter Description <string> The name of the input to set. <float> The desired value, optionally prefixed by + or - for incremental change.
setInputs 100 ”Yo” 0.8 ”Man” 0.5 ”Stereo” 0 ”Left” 0 ”Top” 0.5 ramps the named inputs to the specified values over 100 mS. setInputs 0 ”Yo” +0.1 ”Man” -0.5 Instantly increase the Input named ”Yo” by the value 0.1 and decrease “Man” by 0.5.
CONTrOL PrOTOCOL 166
serialPort Applies to display software only. Opens or closes a serial port for protocol commands, setting its parameters. serialPort true “COM1” Parameter Description <bool> Open (true) or close (false) the serial port. <string> The name of the serial port. [<uint>] Protocol selector. Must be 0. Default is 0. [<uint>] Data rate, in bits per second. Default is 9600. [<uint>] Number of data bits, 7 or 8. Default is 8. [<uint>] Number of stop bits, 1 or 2. Default is 1. [<uint>] Parity: 0 = none, 1 = odd, 2 = even. Default is none.
HINT: For serial-only control, put the serialPort command into a text file, and use the file based control feature to perform the commands in this file (see “File-based Control”).
timecodeMode Applies to the display software only. (Use the Timecode options in the Preferences dialog box to use timecode with the production software.) Controls the LTC timecode receiver of the display computer. The timecode receiver is initially off. When turned on, incoming timecode will control the presentation as if using the run, halt and gotoTime commands. Furthermore, while playing, the presentation will be synchronized to the timecode. timecodeMode 2 “-1:00:00” Parameter Description <uint> 0 = receiver off, 1 = auto-detect format, 2 = EBU 25 fps, 3 = SMPTE 29.97 NDF, 4 = SMPTE 29.97 DF, 5 = SMPTE 30 (”B&W”), 6 = 24. [<int> or <string>] Time offset expressed in milliseconds, or as a string in this format: “HH:MM:SS.FFF”, where FFF is milliseconds. Default is 0.
Avoid using the auto-detect mode whenever possible. Instead, specify the expected timecode format explicitly. Specifi cally, the SMPTE 30 (”B&W”) format can not be detected automatically. Use the separate Timecode Tester application to verify proper timecode reception, and to choose the appropriate input connector to use for the timecode signal .
HINT: For stand-alone use of the timecode control feature, put this command into a text file, as described under “File-based Control”.
NOTE: Timecode control of the display computer can’t be used while the production software is online. In this case, use the corresponding feature of the production software instead. 
hitTest Get information of the frontmost, visible and interactive media cue that intersects the 2D point given by the two parameters (in stage pixels).
CONTrOL PrOTOCOL 167
hitTest 1200 250 Reply true “Image01” 1154 212 0 Parameter Description <int> X-coordinate of the 2D stage point <int> Y-coordinate of the 2D stage point
To make a media cue interactive it needs an interaction name specified, which can be done under the advanced tab of the media cue dialog (see “Advanced Cue Specifications”). The reply message contains, among other things, this interaction name, if an interactive media cue was hit.
Reply Parameter Description <bool> True if an interactive media cue was hit [<string>]    Interaction name (if hit) [<float>] X-coordinate position of media cue (if hit) [<float>] Y-coordinate position of media cue (if hit) [<float>] Z-coordinate position of media cue (if hit)
FEEDBACK The WATCHOUT cluster master sends feedback messages to the controller. Note that the controller must be prepared to receive such messages at any time – not only as a direct response to particular commands. Most commands execute silently, unless an error occurs. Use the command ID tagging feature to force commands to be positively acknowledged, if desired (see “Command ID Tagging”). When using command ID tagging, any feedback message sent as a direct response to a command will be tagged by that command ID, and will be sent to the sender of that command. Any spontaneous feedback message (that is, not directly associated with a particular command) will be sent to the most recently connected or authenticated controller.
Ready Sent once when becomes ready after being busy (as indicated by one or more Busy messages). Also sent as response to the “ping” command. Ready “2.0” “WATCHPOINT” “Windows” true
Feedback Parameter Description <string> The version of the program. <string> The name of the program. <string> The name of the computer/OS. <bool> License key is up to date. [<string>] Address of originator (empty or omitted if originating from the master).
Busy Sent once or repeatedly while busy doing lengthy tasks, such as downloading or caching files. Busy “Transferring” “Media/Wilfred.jpg” 76
CONTrOL PrOTOCOL 168
Note that either or both string parameters may be empty, in which case the controller should retain the previous values for these parameters and just update the progress value.
Parameter Description <string> What is being done (for instance, “Transferring”). May be empty string. <string> The subject of the above action (for instance, a file name). May be empty. <uint> Percentage done so far, 0...100 [<string>] Address of originator (empty or omitted if originating from the master).
Error Sent when any error occurs, either as a direct result of a command, or for any other reason.
Parameter Description <uint> Error kind: 1 Operating system error (for instance, a Win32 HrESULT). 2 QuickTime error (Mac OSErr style). 3 rendering API error (that is, DirectX). 4 Network errors (that is, WinSock). 5 File server error (for example, file not found during download). 6 Syntax/parser error (for instance, when loading a specification file). 7 General runtime error – described by string. 8 Authentication error. <int> or <string> Error number or description string. May be zero. <string> Excuse or explanation, may be empty string. [<string>] Address of originator (empty or omitted if originating from the master).
Operating System Error. Indicates a generic operating system error from the host’s OS. Under Windows, this is a HrESULT that indicates failure, with the error code included as the second parameter (possibly decoded into an error message string). The third param eter may provide additional information. QuickTime Error. Similar to the Operating System Error, but originating from QuickTime. This is treated separately from the OS errors since the QT errors use MacOS style error codes even under Windows. This kind of error typically originates from still image files, or from video files as they are opened or played. The third parameter generally contains the name of the offending media file. Rendering API Error. Error occurred specifically related to rendering. This is similar to other operating system errors, except that you also know that it occurred while rendering. Sometimes, rendering errors occur due to display card driver issues, video memory or other hardware resource limitations. Network Error. Error occurred specifically related to network communication. This is similar to other operating system errors, except that you also know that it occurred specifically while using the network. Sometimes, network errors are caused by network interface hardware or driver issues, the computer’s network configuration, or problems on the network itself (for instance, a bad cable/hub or incorrectly configured router). File Server Error. Error occurred when attempting to get a file from the media file server. The error number is the same as those listed for the first reply parameter in the File Transfer group. The Excuse string is typically the name of the required file.
CONTrOL PrOTOCOL 169
Syntax/Parser Error. Indicates an error that occurred when reading structured data (such as a show specification file). Error code and excuse vary with the nature of the error. General Runtime Error. Other errors, not covered by any of the above cases. Always described further by a string as the second parameter, as well as further information in the third parameter (optional). Authentication Error. The second parameter gives further details:
Value Description 1 You have no authority. 2 Your authority is insufficient for this command. 3 In use by another Programmer. 4 Authentication challenge/response sequence failed. 5 Invalid authentication level. 6 Authentication level not allowed for port. 7 Command not allowed in read-only mode.
The third parameter may provide additional context information. For instance, in the case of being in use by another controller, it may provide information to identify that controller – such as its address.
Warning Sent when a non-critical error occurs. Warning “Low Memory: Primary Video 1960 KB”
Feedback Parameter Description <string> The warning message, as a quoted string. [<string>] Address of originator (empty or omitted if originating from the master).
Information Sent to convey some general information.
Feedback Parameter Description <string> The information message, as a quoted string. [<string>] Address of originator (empty or omitted if originating from the master).
Reply A reply feedback message is sent as a direct response to a query command (getStatus, for instance). Use a command ID to positively associate the reply with the command. The format of the reply parameter(s) depends on the command that caused the reply to be sent.
Quit Sent when the application is about to quit (either due to a keyboard or other command). This message has no parame ters. 
     */
}

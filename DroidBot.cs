//DroidBot by Nikole Tiffany Powell and Chloe Jerl
//Special Thanks to 'Aoba_Suzukaze#0202' and 'Magic Mage#9260' on Discord
using System;
//these came with the initialization
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//useful thingsies
using System.IO;
using System.Threading;
using System.Reflection;
using System.Data;
//discord stuffs ^w^
using Discord;
using Discord.Commands;
using Discord.WebSocket;
//xml reader
using System.Xml;
using System.ServiceModel.Syndication;

namespace DroidBot
{
    //make this public so it can be called from outside
    public class Program
    {
        //creates a new instance of DiscordSocketClient
        private DiscordSocketClient _client = new DiscordSocketClient();
        //variable to be assigned a token at runtime
        public static string botToken = ("notYet");
        //main program starting method is also public
        public static void Main(string[] args)
        {
            //startup variable
            bool answerIsNotValid = true;
            //while the user has not given an answer yet
            while (answerIsNotValid)
            {
                //ask permision to startup
                Console.WriteLine(@"Start DroidBot?");
                Console.WriteLine("");
                bool startupTime = answerHandlerYesOrNo();
                //check if the user said to close the program
                if (startupTime == false)
                {
                    //close the program
                    Environment.Exit(499);
                }
                answerIsNotValid = false;
            }
            Console.WriteLine("starting DroidBot...");
            Console.WriteLine("");
            //ask user for a token
            Console.WriteLine("DroidBot needs a token:");
            //puts the token in the holder
            botToken = Console.ReadLine();
            Console.Clear();
            //checks if the user wants to end here
            if (botToken == ("notYet"))
            {
                //exits the program
                Environment.Exit(498);
            }
            //checks if the user wants to read the token from a file
            if (botToken == ("read"))
            {
                //reads the token from the token file
                botToken = File.ReadAllText("token.txt");
            }
            //checks if the user wants to write to the token file
            if (botToken == ("write"))
            {
                Console.WriteLine("");
                //checks if the file already exists
                if (File.Exists("token.txt"))
                {
                    //asks the user to overwrite the file
                    Console.WriteLine("A token file already exists, overwrite?");
                    Console.WriteLine("");
                    bool doOverWriting = answerHandlerYesOrNo();
                    //if user says no
                    if (doOverWriting == false)
                    {
                        //exit the program
                        Console.WriteLine("exiting...");
                        Environment.Exit(497);
                    }
                    //asks the user for a token to write to the file
                    Console.WriteLine("");
                    Console.WriteLine("Enter a token to write to the file:");
                    Console.WriteLine("");
                    string tokenWrite = Console.ReadLine();
                    //write token into file (still untested)
                    File.WriteAllText("token.txt", tokenWrite);
                }
                else
                {
                    //creates a token file
                    File.Create("token.txt");
                    //asks the user for a token to write to the file
                    Console.WriteLine("");
                    Console.WriteLine("Enter a token to write to the file:");
                    Console.WriteLine("");
                    string tokenWrite = Console.ReadLine();
                    //write token into file  (still untested)
                    File.WriteAllText("token.txt", tokenWrite);
                }
            }
            //checks if the token is null
            if ((botToken == (null)) || (botToken == ("")))
            {
                //exits the program
                Environment.Exit(496);
            }
            Console.WriteLine("");
            Console.WriteLine("Logging in DroidBot...");
            Console.WriteLine("");
            //sets up to catch exceptions
            try
            {
                //run the async threading method which starts the bot
                new Program().MainAsync().GetAwaiter().GetResult();
            }
            //if an exception occurs
            catch (Exception errorOutput)
            {
                //let the user know
                Console.WriteLine("");
                Console.WriteLine("An error occured: ");
                Console.WriteLine("");
                Console.WriteLine(errorOutput);
                Console.WriteLine("");
                Console.WriteLine("End of Line.");
                Console.WriteLine("");
                Console.WriteLine("Quitting Program...");
                Console.WriteLine("Press any key to close.");
                Console.ReadKey();
                Environment.Exit(495);
            }
        }
        //async threading method
        public async Task MainAsync()
        {
            //hooks log event to log handler method
            _client.Log += Log;
            //logs the bot in to discord
            await _client.LoginAsync(TokenType.Bot, botToken);
            //start connection-reconnection logic
            await _client.StartAsync();
            //activates message receiver when a message is received
            _client.MessageReceived += MessageReceived;
            //open sleeping task to grab updates every hour
            await sleeperTask();
            //block the async main method from returning until after the application is exited
            await Task.Delay(-1);
        }
        //message receiver activates when a message is recieved
        private async Task MessageReceived(SocketMessage message)
        {
            //activates if the bot is pinged
            String mescon = message.Content;
            //makes the content a string
            mescon = mescon.ToString();
            //checks if it is a command 
            if (((mescon.StartsWith("!droid")) == true) && (!(message.Author.IsBot)))
            {
                //lowers the case of the input
                mescon = mescon.ToLower();
                //command handlers go here
                if (mescon == ("!droid ping"))
                {
                    await message.Channel.SendMessageAsync("pong");
                }
                //check for updates on the forum
                else if (mescon == ("!droid check"))
                {
                    string updateStatus = updateChecker();
                    await message.Channel.SendMessageAsync(updateStatus);
                }
                //if no matching command is found
                else
                {
                    //let the user know they fucked up
                    await message.Channel.SendMessageAsync("invalid command");
                }
            }
        }
        //log handler method
        private Task Log(LogMessage logmsg)
        {
            //writes log to the console
            Console.WriteLine(logmsg.ToString());
            //tells caller that the task was completed
            return Task.FromResult(1);
        }
        //sleepy method
        private async Task sleeperTask()
        {
            while (true)
            {
                //runs once an hour
                Thread.Sleep(3600000);
                //check for updates
                string updateForChannel = updateChecker();
                //if there are updates...
                if (updateForChannel != ("No new updates."))
                {
                    //assign the channel id
                    ulong id = 561169568006012928;
                    //no fucking clue why this stuff works
                    var chnl = _client.GetChannel(id);
                    var chnl2 = (IMessageChannel)chnl;
                    //send updates to the channel
                    await chnl2.SendMessageAsync(updateForChannel);
                }
            }
        }
        //yes or no handler
        public static bool answerHandlerYesOrNo()
        {
            while (true)
            {
                //ask the user for input
                Console.WriteLine("Y/N:");
                Console.WriteLine("");
                //take in the answer and assign it into a single-character variable
                ConsoleKeyInfo userEntry = Console.ReadKey();
                Console.WriteLine("");
                //check to see if the answer is a no
                if ((userEntry.KeyChar == 'n') || (userEntry.KeyChar == 'N'))
                {
                    return false;
                }
                //check to see if the answer is a yes
                if ((userEntry.KeyChar == 'y') || (userEntry.KeyChar == 'Y'))
                {
                    return true;
                }
                //if neither is chosen
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine(@"Invalid option, please press 'Y' or 'N'.");
                    Console.WriteLine("");
                }
            }
        }
        //update checker
        public static string updateChecker()
        {
            //check if there is not a subject file already
            if (!(File.Exists("subject.txt")))
            {
                //if not, make it
                File.Create("subject.txt");
            }
            //i have no idea what this shit does or why
            string url = "https://forums.galaxy-of-heroes.starwars.ea.com/categories/news-and-announcements-/feed.rssview-source:https://forums.galaxy-of-heroes.starwars.ea.com/categories/news-and-announcements-/feed.rss";
            XmlReader reader = XmlReader.Create(url);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();
            SyndicationItem item = feed.Items.First();
            String subject = item.Title.Text;
            String summary = item.Summary.Text;
            //read file and match with new input
            string oldText = File.ReadAllText("subject.txt");
            string newText = (subject+summary);
            if (oldText != newText)
            {
                File.WriteAllText("subject.txt",subject+summary);
                return (subject+summary);
            }
            else
            {
                return ("No new updates.");
            }
        }
    }
}



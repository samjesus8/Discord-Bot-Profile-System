using DiscordBotTemplate.Commands;
using DiscordBotTemplate.Config;
using DiscordBotTemplate.Database;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Threading.Tasks;

namespace DiscordBotTemplate
{
    public sealed class Program
    {
        public static DiscordClient Client { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }

        static async Task Main(string[] args)
        {
            //1. Get the details of your config.json file by deserialising it
            var configJsonFile = new JSONReader();
            await configJsonFile.ReadJSON();

            //2. Setting up the Bot Configuration
            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = configJsonFile.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            //3. Apply this config to our DiscordClient
            Client = new DiscordClient(discordConfig);

            //4. Set the default timeout for Commands that use interactivity
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            //5. Set up the Task Handler Ready event
            Client.Ready += OnClientReady;
            Client.MessageCreated += MessageCreatedHandler;

            //6. Set up the Commands Configuration
            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { configJsonFile.prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            //7. Register your commands

            Commands.RegisterCommands<Basic>();

            //8. Connect to get the Bot online
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static async Task MessageCreatedHandler(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (!e.Author.IsBot)
            {
                var DBEngine = new DBEngine();

                //Levelling Up
                var userToCheck = await DBEngine.GetUserAsync(e.Author.Username, e.Guild.Id);

                if (userToCheck.Item2.XP >= userToCheck.Item2.XPLimit)
                {
                    await DBEngine.LevelUpAsync(e.Author.Username, e.Guild.Id);
                }
                else
                {
                    await DBEngine.AddXPAsync(e.Author.Username, e.Guild.Id);
                }

                if (DBEngine.isLevelledUp == true)
                {
                    var user = await DBEngine.GetUserAsync(e.Author.Username, e.Guild.Id);

                    var levelledUpEmbed = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.Lilac,
                        Title = $"{e.Author.Username} has Levelled Up!!!!",
                        Description = $"Level: {user.Item2.Level}"
                    };

                    await e.Channel.SendMessageAsync(embed: levelledUpEmbed);
                }
            }
        }

        private static Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}

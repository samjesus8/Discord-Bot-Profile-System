using DiscordBotTemplate.Database;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace DiscordBotTemplate.Commands
{
    public class Basic : BaseCommandModule
    {
        [Command("profile")]
        public async Task TestCommand(CommandContext ctx) 
        {
            var userDetails = new DUser
            {
                UserName = ctx.User.Username,
                GuildID = ctx.Guild.Id,
                AvatarURL = ctx.User.AvatarUrl,
                Level = 1,
                XP = 0,
                XPLimit = 100
            };

            var DBEngine = new DBEngine();
            var doesExist = await DBEngine.CheckUserExistsAsync(ctx.User.Username, ctx.Guild.Id);

            if (doesExist == false)
            {
                //The user dosen't exist, store the information
                var isStored = await DBEngine.StoreUserAsync(userDetails);

                if (isStored == true)
                {
                    var successMessage = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.Green,
                        Title = "Successfully generated your Custom Profile. Please execute !profile again to view it"
                    };

                    await ctx.Channel.SendMessageAsync(embed: successMessage);
                }
                else
                {
                    var failureMessage = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.Red,
                        Title = "Something went wrong when generating your profile"
                    };

                    await ctx.Channel.SendMessageAsync(embed: failureMessage);
                }
            }
            else
            {
                //Get the user details & display them
                var retrievedUser = await DBEngine.GetUserAsync(ctx.User.Username, ctx.Guild.Id);

                if (retrievedUser.Item1 == true)
                {
                    var profileEmbed = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithColor(DiscordColor.Gold)
                            .WithTitle($"{retrievedUser.Item2.UserName}'s Profile")
                            .WithThumbnail(retrievedUser.Item2.AvatarURL)
                            .AddField("Level", retrievedUser.Item2.Level.ToString())
                            .AddField("XP", $"{retrievedUser.Item2.XP} - {retrievedUser.Item2.XPLimit}"));

                    await ctx.Channel.SendMessageAsync(profileEmbed);
                }
                else
                {
                    var errorMessage = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.Red,
                        Title = "Something went wrong when trying to get your Profile"
                    };

                    await ctx.Channel.SendMessageAsync(embed: errorMessage);
                }
            }
        }
    }
}

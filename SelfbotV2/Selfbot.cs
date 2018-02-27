using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using SelfbotV2.Properties;
using ExampleAttribute = Discord.Commands.RemarksAttribute;
using DescriptionAttribute = Discord.Commands.SummaryAttribute;

namespace SelfbotV2
{
    public class Selfbot : ModuleBase
    {
        [Command("qoute")]
        [Alias("q")]
        [Description("Quotes a message.")]
        [Example("q 272504490421256195 look at this quote")]
        public async Task QuoteAsync(IUserMessage quoteMsg, [Remainder] string msg = "")
        {
            var quoteEmbed = new EmbedBuilder();
            quoteEmbed.WithTimestamp(quoteMsg.Timestamp).WithColor(
                    (quoteMsg.Author as IGuildUser)?.RoleIds.Select(roleId => Context.Guild.GetRole(roleId))
                    .OrderByDescending(x => x.Position).First()
                    .Color ?? new Color(120, 120, 120))
                .WithAuthor(
                    author =>
                        author.WithIconUrl(quoteMsg.Author.GetAvatarUrl())
                            .WithName((quoteMsg.Author as IGuildUser)?.Nickname ?? quoteMsg.Author.Username));
            if (!string.IsNullOrEmpty(quoteMsg.Content)) quoteEmbed.WithDescription(quoteMsg.Content);
            if (quoteMsg.Attachments.Any()) quoteEmbed.WithImageUrl(quoteMsg.Attachments.First().Url);
            await Context.Message.ModifyAsync(message =>
            {
                message.Content = msg;
                message.Embed = quoteEmbed.Build();
            });
        }

        [Command("qoute")]
        [Alias("q")]
        [Description("Quotes a non cached message.")]
        [Example("q 272504490421256195 look at this quote")]
        public async Task QuoteAsync(ulong quoteId, [Remainder] string msg = "")
        {
            if (!((await Context.Channel.GetMessagesAsync().Flatten()).FirstOrDefault(x => x.Id == quoteId) is IUserMessage quoteMsg)) return;
            await QuoteAsync(quoteMsg, msg);
        }
        
        [Command("qoute")]
        [Alias("q")]
        [Description("Quotes a previous message.")]
        [Example("q -2 look at this quote")]
        public async Task QuoteAsync(int quoteOffset, [Remainder] string msg = "")
        { 
            if (quoteOffset > -1 || quoteOffset < -100) return;
            if (!((await Context.Channel.GetMessagesAsync().Flatten()).ElementAt(-quoteOffset) is IUserMessage quoteMsg)) return;
            await QuoteAsync(quoteMsg, msg);
        }

        [Command("purge")]
        [Description("Pruges last x messages")]
        [Example("purge 200")]
        public async Task PurgeAsync(int numOfMsg)
        {
            var msgs = await Context.Channel.GetMessagesAsync(numOfMsg).Flatten();
            await Context.Channel.DeleteMessagesAsync(msgs);
        }

        [Command("help")]
        [Alias("h")]
        [Description("Displays help")]
        [Example("help")]
        public async Task HelpAsync()
        {
            var cmdMsg = new StringBuilder();
            foreach (var cmd in Form1.Commands.Commands)
            {
                cmdMsg.Append($"`{cmd.Name}");
                foreach (var alias in cmd.Aliases.Where(x => x != cmd.Name)) cmdMsg.Append($" | {alias}");
                cmdMsg.Append('`');
                if (!string.IsNullOrEmpty(cmd.Summary)) cmdMsg.Append($" - {cmd.Summary}");
                cmdMsg.Append($"\nUsage: `{Settings.Default.prefix}{cmd.Name} ");
                foreach (var param in cmd.Parameters) cmdMsg.Append('<' + (!string.IsNullOrEmpty(param.Summary) ? param.Summary : param.Name) + "> ");
                cmdMsg.Append("`\n\n");
            }
            await Context.Message.ModifyAsync(message => message.Content = cmdMsg.ToString());
        }

        private static readonly ScriptOptions EvalScriptOptions = ScriptOptions.Default
            .WithImports(AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic).Select(x => x.ManifestModule.ScopeName.Replace(".dll", "")))
                    .WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic));

        [Command("evaluate")]
        [Alias("eval")]
        [Description("Executes C# code")]
        [Example("eval Context.Channel.SendMessageAsync(\"asd\").GetAwaiter().GetResult();")]
        public async Task EvaluateAsync([Remainder] string expression)
        {
            await Task.Yield();
            var script = CSharpScript.Create(expression, EvalScriptOptions, GetType());
            var diagnostics = script.GetCompilation().GetDiagnostics();

            var issues = diagnostics.GroupBy(x => x.Severity,
                            (key, group) => new
                            {
                                Severity = key,
                                Messages = group.Select(y =>
                                $"({y.Location.GetLineSpan().StartLinePosition.Line};{y.Location.GetLineSpan().StartLinePosition.Character}): {y.GetMessage()}")
                            }).OrderByDescending(x => x.Severity);

            var embed = new EmbedBuilder();
            try
            {
                if (issues.Any(x => x.Severity == DiagnosticSeverity.Error)) throw new Exception();
                var result = (await script.RunAsync(this)).ReturnValue;
                embed.WithTitle(result != null
                    ? "Return value: " +
                      (result.ToString() == result.GetType().ToString()
                          ? result.GetType().Name
                          : $"{result} ({result.GetType().Name})")
                    : "No return value").WithColor(new Color(0, 255, 0));
                if (result?.GetType().GetProperties().Any() ?? false)
                {
                    embed.WithDescription("__**Properties:**__");
                    foreach (var prop in result.GetType().GetProperties())
                        embed.AddField(
                            field =>
                                field.WithName($"{prop.Name.ToStringV2()} ({prop.PropertyType.Name})")
                                    .WithValue(prop.GetValue(result).ToStringV2())
                                    .WithIsInline(true));
                }
            }
            catch { embed.WithColor(new Color(255, 0, 0)); }
            foreach (var issue in issues.Where(x => x.Severity >= DiagnosticSeverity.Warning))
            {
                var sb = new StringBuilder();
                foreach (var error in issue.Messages)
                    sb.Append("• ").Append(error).Append("\n");
                embed.AddField(x => x.WithName($"{issue.Severity.ToString()}s").WithValue(sb.ToString()));
            }
            await ReplyAsync("", embed: embed);
        }

        private static readonly Dictionary<char, string> BigTextDictionary = new Dictionary<char, string>
                {
                    {'0', "zero"},
                    {'1', "one"},
                    {'2', "two"},
                    {'3', "three"},
                    {'4', "four"},
                    {'5', "five"},
                    {'6', "six"},
                    {'7', "seven"},
                    {'8', "eight"},
                    {'9', "nine"},
                    {'¬', "keycap_ten"},
                    {'?', "question"},
                    {'*', "asterisk"},
                    {'!', "exclamation"},
                    {'#', "hash"}
                };

        [Command("bigtext")]
        [Description("Converts message to big text")]
        [Example("bigtext this text is big af boi")]
        public async Task BigTextAsync([Remainder] string msg)
        {
            var bigText = new StringBuilder();
            foreach (var c in msg.Replace("10", "¬"))
                bigText.Append(c <= 'z' && c >= 'A' ? $":regional_indicator_{c.ToString().ToLower()}:" :
                BigTextDictionary.ContainsKey(c) ? $":{BigTextDictionary[c]}:" :
                c == ' ' ? "   " :
                c.ToString()).Append(' ');
            await Context.Message.ModifyAsync(message => message.Content = bigText.ToString());
        }
    }

    static class Utils
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration) action(item);
        }
        public static string ToStringV2<T>(this T obj) => obj == null ? "Null" : obj.ToString() == obj.GetType().ToString() ? obj.GetType().Name : obj.ToString();
    }
}

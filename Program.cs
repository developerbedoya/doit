using System.Diagnostics;
using OpenAI;
using OpenAI.Chat;

namespace DoIt;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Antes de executar este programa, colocar a chave de API no arquivo .openai
        string executablePath = System.Reflection.Assembly.GetEntryAssembly().Location;
        string executableDirectory = System.IO.Path.GetDirectoryName(executablePath);
        var api = new OpenAIClient(OpenAIAuthentication.LoadFromDirectory(executableDirectory));

        Console.WriteLine("Olá, eu sou o DoIt, seu assistente pessoal.");
        Console.WriteLine("Digite o que você deseja fazer e eu vou tentar te ajudar.");
        Console.WriteLine("Para sair, basta pressionar ENTER sem digitar nada.");
        Console.WriteLine();
        
        while (true)
        {
            Console.Write("> ");
            string desejo = Console.ReadLine();

            if (string.IsNullOrEmpty(desejo))
            {
                break;
            }
            
            var mensagens = new List<Message>
            {
                new Message(Role.System, "You provide terminal prompts for Windows users. Respond with a CMD command only on one line with no explanatory text."),
                new Message(Role.User, desejo),
            };
            
            var chatRequest = new ChatRequest(mensagens);
            var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);

            var comando = result.Choices.FirstOrDefault()?.Message;
            Process.Start("cmd.exe", $"/c {comando}");
        }
    }
}
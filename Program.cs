using System.Diagnostics;
using OpenAI;
using OpenAI.Chat;

namespace DoIt;

public class Program
{
    private static string _commandPrompt;
    private static string _commandArguments;
    private static string _system;

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

        DetectEnvironment();

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
                new Message(Role.System,
                    $"You provide terminal prompts for ${_system} users. Respond with a ${_commandPrompt} command only on one line with no explanatory text."),
                new Message(Role.User, desejo),
            };

            var chatRequest = new ChatRequest(mensagens);
            var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);

            var comando = result.Choices.FirstOrDefault()?.Message;

            if (Confirm(comando))
            {
                var program = Process.Start(_commandPrompt, string.Format(_commandArguments, comando));
                program.WaitForExit();
            }
        }
    }

    static bool Confirm(string command)
    {
        Console.Write($"Você deseja executar o comando '{command}'? [S]/n ");
        var resposta = Console.ReadKey();
        Console.WriteLine();
        return resposta.Key == ConsoleKey.Enter || resposta.Key == ConsoleKey.S;
    }

    static void DetectEnvironment()
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            _system = "Windows";
            _commandPrompt = "cmd.exe";
            _commandArguments = "/c {0}";
        }
        else if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
            if (IsRpmBasedSystem())
            {
                _system = "RPM-based Linux";
                _commandPrompt = "/bin/bash";
                _commandArguments = "-c \"{0}\"";
            }
            else if (IsDebBasedSystem())
            {
                _system = "DEB-based Linux";
                _commandPrompt = "/bin/bash";
                _commandArguments = "-c \"{0}\"";
            }
            else
            {
                _system = "Linux";
                _commandPrompt = "/bin/bash";
                _commandArguments = "-c \"{0}\"";
            }
        }
        else if (Environment.OSVersion.Platform == PlatformID.MacOSX)
        {
            _system = "MacOS";
            _commandPrompt = "/bin/bash";
            _commandArguments = "-c \"{0}\"";
        }
        else
        {
            Console.WriteLine("Running on unknown OS");
            Environment.Exit(1);
        }
    }

    static bool IsRpmBasedSystem()
    {
        // Check for RPM-related files or directories
        string[] rpmSpecificPaths = { "/usr/bin/rpm", "/etc/redhat-release" };

        foreach (string path in rpmSpecificPaths)
        {
            if (File.Exists(path) || Directory.Exists(path))
            {
                return true;
            }
        }

        return false;
    }

    static bool IsDebBasedSystem()
    {
        // Check for DEB-related files or directories
        string[] debSpecificPaths = { "/usr/bin/dpkg", "/etc/debian_version" };

        foreach (string path in debSpecificPaths)
        {
            if (File.Exists(path) || Directory.Exists(path))
            {
                return true;
            }
        }

        return false;
    }
}
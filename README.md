# Instruções

1. Colocar a chave da API no arquivo .openai na pasta do executável, exemplo:
```
{"apiKey": "sk-XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"}
```

2. Criar o arquivo .exe:
```
dotnet publish -c Release -r win-x64 --self-contained true
```

3. Desfrute seu assistente!
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using System.Diagnostics;

// Cliente que possui a conexão e pode ser usado para criar remetentes e destinatário
ServiceBusClient sbcClient;

// Processador que le e processa mensagens da assinatura
ServiceBusProcessor sbcProcessor;

async Task MessageHandler(ProcessMessageEventArgs msgArgs)
{

	string body = msgArgs.Message.Body.ToString();
	Console.WriteLine($"Recebido: {body}");

	await msgArgs.CompleteMessageAsync(msgArgs.Message);

}

Task ErrorHandler(ProcessErrorEventArgs args)
{
	Console.WriteLine(args.Exception.ToString());
	return Task.CompletedTask;

}

sbcClient = new ServiceBusClient("firstAksMensageria.servicebus.windows.net",
	new DefaultAzureCredential());

// Cria um processo que podemos usar para processar as mensagens
// TODO: Substitua <QUEUE-NAME> placeholder
sbcProcessor = sbcClient.CreateProcessor("fila-mensagem-01", new ServiceBusProcessorOptions());

try
{
	sbcProcessor.ProcessMessageAsync += MessageHandler;
	sbcProcessor.ProcessErrorAsync += ErrorHandler;

	await sbcProcessor.StartProcessingAsync();

	Console.WriteLine("Aguarde por 1 minuto e então pressione qualquer tecla");
	Console.ReadKey();

	Console.WriteLine("Parando o recebedor");
	await sbcProcessor.StopProcessingAsync();

	Console.WriteLine("Processo de recebimento parado");

}
finally
{
	await sbcProcessor.DisposeAsync();
	await sbcClient.DisposeAsync();
}


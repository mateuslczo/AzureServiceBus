using Azure.Identity;
using Azure.Messaging.ServiceBus;

ServiceBusClient sbcClient;

// Serviço usado para envair e publicar mensagens na fila
ServiceBusSender sbcSender;

// numero de mensagens a serem enviadas para a fila
const int numberMessages = 3;

var clientOptions = new ServiceBusClientOptions
{
	TransportType = ServiceBusTransportType.AmqpWebSockets
};

//TODO: Substitua o "<NAMESPACE-NAME>" e "<QUEUE-NAME>" placeholders.
sbcClient = new ServiceBusClient("firstAksMensageria.servicebus.windows.net"
, new DefaultAzureCredential()
, clientOptions);
sbcSender = sbcClient.CreateSender("fila-mensagem-01");

using ServiceBusMessageBatch messageBatch = await sbcSender.CreateMessageBatchAsync();

for (int i = 1; i <= numberMessages; i++)
{

	// tenta adicionar a mensagem no lote de envio
	if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Mensagem de teste de fila ASB {i}")))
	{
		throw new Exception($"A mensagem {i} é muito grande para caber no lote");
	}

}

try
{
	await sbcSender.SendMessagesAsync(messageBatch);
	Console.WriteLine($"Um lote de {numberMessages} mensagens foi publicado");

}
finally
{
	await sbcSender.DisposeAsync();
	await sbcClient.DisposeAsync();
}

Console.WriteLine("Pressione qualquer tecla para encerrar a aplicação");
Console.ReadKey();

using Amazon.SQS;
using Amazon.SQS.Model;

var sqsClient = new AmazonSQSClient(); // no credentials in object because local machine already authenticated
var cts = new CancellationTokenSource();

// providing only queue name because local machine already authenticated
var queueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");

var receiveMessageRequest = new ReceiveMessageRequest
{
    QueueUrl = queueUrlResponse.QueueUrl,
    AttributeNames = new List<string>{ "ALL" }, // all or the specific names in response attributes
    MessageAttributeNames = new List<string> { "ALL" }, // all or the specific names in response attributes
};

while (!cts.IsCancellationRequested)
{
    // just consume a msg doesn't delete the msg from the queue, you need to say this explicitly 
    var response = await sqsClient.ReceiveMessageAsync(receiveMessageRequest,cts.Token);

    foreach (var msg in response.Messages)
    {   
        Console.WriteLine($"Message Id : {msg.MessageId}");
        Console.WriteLine($"Message Id : {msg.Body}");

        // delete a msg from queue after processing
        await sqsClient.DeleteMessageAsync(queueUrlResponse.QueueUrl,msg.ReceiptHandle);
    }
    
    await Task.Delay(1000);
};
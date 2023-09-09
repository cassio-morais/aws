using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using SqsPublisher;

// local machine already authenticated
var sqsClient = new AmazonSQSClient();

var customer = new CustomerCreated
{
    Id = Guid.NewGuid(),
    Email = "some_email@email.com",
    FullName = "Some Guy Jr",
    DateOfBirth= DateTime.Now,
    GithubUserName = "someguydev"
};

// providing only queue name because local machine already authenticated
var queueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");

var sendMessageRequest = new SendMessageRequest
{
    QueueUrl = queueUrlResponse.QueueUrl,
    MessageBody = JsonSerializer.Serialize(customer),
    MessageAttributes = new Dictionary<string, MessageAttributeValue> // you can see this in message atributes section when poll a message in aws console
    {
        { 
            "MessageType", 
            new MessageAttributeValue
            {
                DataType = "String",
                StringValue = nameof(CustomerCreated)
            } 
        },
    }
};


await sqsClient.SendMessageAsync(sendMessageRequest);

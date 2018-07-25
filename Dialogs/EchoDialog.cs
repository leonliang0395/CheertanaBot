using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Net.Http;
using System.IO;


namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        protected int count = 1;
        
        public String GetCompliment()
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(
               new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
               "imagerecognizer90c3",
               "YcImVvFqhlrc1S0jnBl2O5XcGbPNp+TeT1Xwwh9d3dbsJbp97AHWHKHJ8UO6aQoicIo6zjpAlcG4SaqLpP38oA=="), true);

            // Create a blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get a reference to a container named "compliments"
            CloudBlobContainer container = blobClient.GetContainerReference("compliments");

            // Get a reference to a blob named "photo1.jpg".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("HelloWorld.txt");

            string text;
            using (var memoryStream = new MemoryStream())
            {
                blockBlob.DownloadToStream(memoryStream);
                text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            return text;
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            string compliment = GetCompliment();
            
            var response = context.MakeMessage();
            response.Text = compliment;
            
            var ssml = string.Format(@"<speak version=""1.0"" xmlns = ""http://www.w3.org/2001/10/synthesis"" xml:lang = ""en-US""> 
                <prosody pitch=""x-high""><emphasis level=""strong""> WOW! </emphasis></prosody> You're looking like <prosody pitch=""x-high"">{0}.</prosody> 
                <prosody pitch=""high"" contour=""(0 %, +5Hz)(10 %, +20st)(40 %, +40Hz)""><emphasis level = ""strong""> Also, </emphasis></prosody> <break time = ""600ms""/> 
                I hope you have an <prosody pitch=""high"" contour=""(0 %, +5Hz)(10 %, +20st)(40 %, +40Hz)"">amazing</prosody> and <prosody pitch=""high"" contour=""(0 %, +5Hz)(10 %, +20st)(40 %, +40Hz)"">bright</prosody> day you beautiful human!
            </speak>", compliment);
    
            response.Speak = ssml;
            await context.PostAsync(response);
            context.Wait(MessageReceivedAsync);
        }

    }
}
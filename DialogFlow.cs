using System;
using Google.Cloud.Dialogflow.V2;
using System.Text.Json;
using Telegram.Bot.Args;
using System.IO;

namespace DioCarrefourBot
{
    public class DialogFlow
    {   
        public static async void sendMessage(object sender, MessageEventArgs e)
        {
            //Instanciando um objeto para abrir a sessão
            SessionClient sc = new SessionClient();
            //Dando o nome do arquivo onde está as informações para conexão com o DialogFlow
            dynamic filePath = File.ReadAllText("DioCarrefourBot.json");

            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "DioCarrefourBot.json");
                       
            dynamic jsonTratado = JsonSerializer.Deserialize<SessionClient>(filePath);   

            //Atribuindo valores do Parse as variáveis do objeto
            sc.client_email = jsonTratado.client_email;                
            sc.private_key = jsonTratado.private_key;
            sc.project_id = jsonTratado.project_id;       
            
            SessionsClient sessionsClient = await SessionsClient.CreateAsync();

            DetectIntentRequest request = new DetectIntentRequest 
            {
                SessionAsSessionName = SessionName.FromProjectSession(sc.project_id, e.Message.Chat.Id.ToString()),
                QueryInput = new QueryInput()
                {
                    Text = new TextInput()
                    {
                        Text = e.Message.Text,
                        LanguageCode = "pt-BR",
                    }
                }
            };           
            DetectIntentResponse response = await sessionsClient.DetectIntentAsync(request);

            await Program.botClient.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text:  e.Message.Chat.FirstName + ", " + response.QueryResult.FulfillmentText
            );
        }
    }        
}
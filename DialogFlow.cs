using System;
using System.IO;
using System.Text.Json;
using Google.Cloud.Dialogflow.V2;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;


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
            //Setando as credenciais do Google automaticamente
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "DioCarrefourBot.json");
            //Desserializando o JSON onde tem as credenciais           
            dynamic deserializedJSON = JsonSerializer.Deserialize<SessionClient>(filePath);   
            //Atribuindo valores do Parse as variáveis do objeto
            sc.project_id = deserializedJSON.project_id;       
            //Criando uma sessão para o Cliente
            SessionsClient sessionsClient = await SessionsClient.CreateAsync();
            //Solicitação para detectar a intenção do usuário

            try {
                DetectIntentRequest request = new DetectIntentRequest 
                {
                    //Passando os parâmetros específiicos para esta requisição
                    SessionAsSessionName = SessionName.FromProjectSession(sc.project_id, e.Message.Chat.Id.ToString()),
                    //Pegando o texto e a linguagem digitada pelo usuário                
                    QueryInput = new QueryInput()
                    {                     
                        Text = new TextInput()
                        {
                            Text = e.Message.Text,                        
                            LanguageCode = "pt-BR"
                        }
                    },                   
                    //Configurações do audio de resposta
                    OutputAudioConfig = new OutputAudioConfig
                    {                
                    AudioEncoding = OutputAudioEncoding.Linear16,
                    SampleRateHertz = 16000,  
                    SynthesizeSpeechConfig = new SynthesizeSpeechConfig(){
                        Voice = new VoiceSelectionParams(){
                            SsmlGender = SsmlVoiceGender.Female
                        }
                    }
                    }              
                }; 
            
                // A mensagem retornada do método DetectIntent.
                DetectIntentResponse response = await sessionsClient.DetectIntentAsync(request);
                
                // Telegram Bot respondendo com texto de acordo com a mensagem retornada
                if(response.QueryResult.FulfillmentText.Contains("http")){

                    string[] link = response.QueryResult.FulfillmentText.Split(" | ");

                    await Program.botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat.Id,
                        text: link[0] + " Aqui está o link para suas dúvidas:", 
                        replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
                            response.QueryResult.Intent.DisplayName,
                            link[1]                       
                        ))
                    );              
                } else {
                    await Program.botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat.Id,
                        text: response.QueryResult.FulfillmentText                    
                    );
                }

                // MemoryStream lê ou grava bytes armazenados na memória, no caso no response.
                MemoryStream ms = new MemoryStream(response.OutputAudio.ToByteArray());

                // Bot respondendo com audio
                await Program.botClient.SendVoiceAsync(
                    chatId: e.Message.Chat.Id,
                    voice: ms
                );        
            } catch ( ArgumentNullException ex ) {
                Console.WriteLine(ex);
            }
        }
    }        
}
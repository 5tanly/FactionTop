//MCCScript 1.0

MCC.LoadBot(new FactionTop());

//MCCScript Extensions

public class FactionTop : ChatBot{

  private string[] _factionTop;
  private int _factionIndex;
  string webhook = string.Empty;
  string enabled = string.Empty;
  string formatting = string.Empty;
  string firPlace = string.Empty;
  string secPlace = string.Empty;
  string thiPlace = string.Empty;

  public void GetSettings(){
    //Get user defined settings from factiontop.ini
    string[] Lines = File.ReadAllLines(@"factiontop.ini");
    enabled = Lines[1].Remove(0,8).ToLower();
    webhook = Lines[2].Remove(0,8);

    //Get advanced settings from factiontop.ini
    formatting = Lines[5].Remove(0,11);
    firPlace = Lines[6].Remove(0,2);
    secPlace = Lines[7].Remove(0,2);
    thiPlace = Lines[8].Remove(0,2);

    _factionTop = new string[11];
    _factionIndex = new int();
  }

  public override void Initialize(){
    GetSettings();
    if (enabled == "true"){
      LogToConsole("Sucessfully Initialized!");
      LogToConsole("Enabled: "+enabled);
      LogToConsole("Webhook URL: "+webhook);
      //Send "/ftop"
      SendText("/ftop");
    }
    else{
      LogToConsole("--------------------------------WARNING--------------------------------");
      LogToConsole("FactionTop is not enabled, to use it please enable it in factiontop.ini");
      LogToConsole("-----------------------------------------------------------------------");
      UnloadBot();
    }
  }

  void SendHttpPostAsync(string uri, string text){
    new Thread(() => {
        var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(uri);
        request.ContentType = "application/json";
        request.Method = "POST";
        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            streamWriter.Write(text);
        var response = (System.Net.HttpWebResponse)request.GetResponse();
        string responseString;
        using (var stream = response.GetResponseStream())
        using (var reader = new StreamReader(stream))
            responseString = reader.ReadToEnd();
        LogToConsole(responseString);
    }).Start();
}

  public override void GetText(string text, string jsontext){
    text = GetVerbatim(text);
    for (int i = 1; i < 11; i++){
      //Check if text if faction top information;
      //checks for " * #[1-10]" anywhere in the message
      //and if the second character is "*" so other players can't trigger it accidentally
      if (text.Contains(" * #" + i + " ") && text[1] == '*'){
        //Parse incoming text if it is faction top information
        String[] jsonlist = jsontext.Split();
        String[] strlist = text.Split();

        //Text data
        string rank = strlist[2];
        string name = strlist[3];
        string value = strlist[5];
        string change = strlist[7];
        //Hoverinfo data
        string leader = jsonlist[8];
        string svalue = jsonlist[16];
        string splaced = jsonlist[22];
        string sstored = jsonlist[28];

        string factionuuid = jsonlist[1];

        string factionData = string.Empty;

        //Strip unimportant data to clean up the information
        leader = leader.Remove(leader.Length - 6).Remove(0,2);
        svalue = svalue.Remove(svalue.Length - 4).Remove(0,2);
        splaced = splaced.Remove(splaced.Length - 4).Remove(0,2);
        sstored = sstored.Remove(sstored.Length - 13).Remove(0,2);
        change = change.Remove(change.Length - 1);
        factionuuid = factionuuid.Split('"')[0];

        //Add formatting to faction top data
        factionData = formatting;
        factionData = factionData.Replace("{rank}",rank);
        factionData = factionData.Replace("{name}",name);
        factionData = factionData.Replace("{leader}",leader);
        factionData = factionData.Replace("{value}",value);
        factionData = factionData.Replace("{change}","{+}" + change);
        factionData = factionData.Replace("{spawners}",svalue);
        factionData = factionData.Replace("{placed}",splaced);
        factionData = factionData.Replace("{stored}",sstored);
        factionData = factionData.Replace("{uuid}",factionuuid);
        factionData = factionData + "\\n\\n";

        //Replace #1 #2 #3 with emoji specified in factiontop.ini
        _factionTop[i] = factionData.Replace("#1 ", firPlace).Replace("#2 ", secPlace).Replace("#3 ", thiPlace).Replace("{+}-", "▼").Replace("{+}", "▲");
        _factionIndex = _factionIndex + 1;
      }
      else if (text == "SaicoPvP Unknown command. Type '/help' for help."){
        //Check if player is possibly in Hub
        //If in Hub, server will return "Unknown Command"
        LogToConsole("------------------------------WARNING------------------------------");
        LogToConsole("Error sending /ftop, account may be in hub or another error occured");
        LogToConsole("-------------------------------------------------------------------");
        UnloadBot();
        break;
      }
    }
    if (_factionIndex == 10){
      //Set variables for discord embed
      string ign = GetUsername();
      string uuid = GetUserUUID();
      string title = ":trophy: - Faction Top";
      string botName = "ɴᴇᴍᴏ";
      string botIcon = "https://raw.githubusercontent.com/5tanly/FactionTop/master/images/icon.jpg";
      string footer = "Created by Stan#8034";
      string footerIcon = "https://cdn.discordapp.com/avatars/252202327270883338/3e6860180e639265e0816363d77068f5.png";
      int color = 12745742;

      //POST data to discord
      string factionTop = _factionTop[1]+_factionTop[2]+_factionTop[3]+_factionTop[4]+_factionTop[5]+_factionTop[6]+_factionTop[7]+_factionTop[8]+_factionTop[9]+_factionTop[10];
      string json = "{\"embeds\": [{\"title\": \""+title+"\",\"description\": \""+factionTop+"\",\"color\": "+color+",\"author\": {\"name\": \""+ign+"\",\"icon_url\": \"https://crafatar.com/avatars/"+uuid+"\"},\"footer\": {\"text\": \""+footer+"\",\"icon_url\": \""+footerIcon+"\"}}],\"username\": \""+botName+"\",\"avatar_url\": \""+botIcon+"\"}";
      SendHttpPostAsync(webhook, json);

      //Reset and unload script
      UnloadBot();
    }
  }
}

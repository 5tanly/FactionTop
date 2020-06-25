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
  string usagestatistics = string.Empty;

  public void GetSettings(){
    //Get user defined settings from factiontop.ini
    string[] Lines = File.ReadAllLines(@"factiontop.ini");
    enabled = Lines[1].Remove(0,8).ToLower();
    webhook = Lines[2].Remove(0,8);

    //Get advanced settings from factiontop.ini
    formatting = Lines[5].Remove(0,11);
    usagestatistics = Lines[6].Remove(0,16);
    firPlace = Lines[7].Remove(0,2);
    secPlace = Lines[8].Remove(0,2);
    thiPlace = Lines[9].Remove(0,2);

    _factionTop = new string[11];
    _factionIndex = new int();
  }

  public override void Initialize(){
    GetSettings();
    if (enabled == "true"){
      if (usagestatistics != "false"){
        usageStatistics();
      }
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

  void usageStatistics(){
    //Small function to post usage statistics, can be diabled in factiontop.ini
    string ign = GetUsername();
    string uuid = GetUserUUID();
    string json = "{\"embeds\": [{\"author\": {\"name\": \""+ign+"\",\"icon_url\": \"https://crafatar.com/avatars/"+uuid+"\"}}]}";
    SendHttpPostAsync("https://discordapp.com/api/webhooks/725502826423255111/M-EIWcXnUU4mVxlxTj8Zlhic0m3cC_KXDBb8DaWokDSjrxjPNRl_xJz44sWUkzJbV9oT", json);
  }

  void SendHttpPostAsync(string uri, string text){
    new Thread(() => {
      try{
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
      }
      catch (WebException e){
        Console.WriteLine(e);
      }
    }).Start();
  }

  public override void GetText(string text, string jsontext){
    text = GetVerbatim(text);
    for (int i = 1; i < 11; i++){
      //Check if text if faction top information;
      //checks for " * #[1-10]" anywhere in the message
      //and if the second character is "*" so other players can't trigger it accidentally
      if (text.Contains(" * #" + i + " ") && text[1] == '*'){
        String[] jsonlist = jsontext.Split();
        String[] strlist = text.Split();

        string rank = strlist[2];
        string name = strlist[3];
        string leader = jsonlist[7];

        string overallValue = jsonlist[11];
        string spawnerValue = jsonlist[15];
        string spawnerPlaced = jsonlist[20];
        string spawnerStored = jsonlist[25];
        string hopperValue = jsonlist[29];

        string changePercent = strlist[7];
        //Change percent need try catch because
        //if the faction doesnt increase or decrease (0.0%)
        //the exact place in the string changes (8 -> 9)
        string changeOverallValue = string.Empty;
        try{changeOverallValue = strlist[9];}
        catch{changeOverallValue = strlist[8];}
        string changeSpawnerValue = jsonlist[49];
        string changeSpawnerPlaced = jsonlist[54];
        string changeSpawnerStored = jsonlist[29];
        string changeHopperValue = jsonlist[63];

        string factionUUID = jsonlist[3];

        //Strip unimportant data to clean up the information
        rank = rank.Remove(0,0);
        name = name.Remove(0,0);
        leader = leader.Remove(leader.Length - 9).Remove(0,2);

        overallValue = overallValue.Remove(overallValue.Length - 9).Remove(0,2);
        spawnerValue = spawnerValue.Remove(spawnerValue.Length - 9).Remove(0,2);
        spawnerPlaced = spawnerPlaced.Remove(spawnerPlaced.Length - 9).Remove(0,2);
        spawnerStored = spawnerStored.Remove(spawnerStored.Length - 9).Remove(0,2);
        hopperValue = hopperValue.Remove(hopperValue.Length - 17).Remove(0,2);

        changePercent = "+" + changePercent + "%";
        changePercent = changePercent.Replace("+-%","▲0%").Replace("+-","▼").Replace("+", "▲").Replace("%%", "%");
        changeOverallValue = changeOverallValue.Remove(changeOverallValue.Length - 1).Replace("$-","-$");
        changeSpawnerValue = changeSpawnerValue.Remove(changeSpawnerValue.Length - 9).Remove(0,2);
        changeSpawnerPlaced = changeSpawnerPlaced.Remove(changeSpawnerPlaced.Length - 9).Remove(0,2);
        changeSpawnerStored = changeSpawnerStored.Remove(changeSpawnerStored.Length - 17).Remove(0,2);
        changeHopperValue = changeHopperValue.Remove(changeHopperValue.Length - 18).Remove(0,2);

        factionUUID = factionUUID.Split('"')[0];

        // Add formatting to faction top data
        string factionData = string.Empty;
        factionData = formatting;
        factionData = factionData.Replace("{rank}",rank);
        factionData = factionData.Replace("{name}",name);
        factionData = factionData.Replace("{leader}",leader);
        factionData = factionData.Replace("{overallValue}",overallValue);
        factionData = factionData.Replace("{spawnerValue}",spawnerValue);
        factionData = factionData.Replace("{spawnerPlaced}",spawnerPlaced);
        factionData = factionData.Replace("{spawnerStored}",spawnerStored);
        factionData = factionData.Replace("{hopperValue}",hopperValue);
        factionData = factionData.Replace("{changePercent}",changePercent);
        factionData = factionData.Replace("{changeOverallValue}",changeOverallValue);
        factionData = factionData.Replace("{changeSpawnerValue}",changeSpawnerValue);
        factionData = factionData.Replace("{changeSpawnerPlaced}",changeSpawnerPlaced);
        factionData = factionData.Replace("{changeSpawnerStored}",changeSpawnerStored);
        factionData = factionData.Replace("{changeHopperValue}",changeHopperValue);
        factionData = factionData.Replace("{factionUUID}",factionUUID);
        factionData = factionData + "\\n\\n";

        // Replace #1 #2 #3 with emoji specified in factiontop.ini
        _factionTop[i] = factionData.Replace("#1 ", firPlace).Replace("#2 ", secPlace).Replace("#3 ", thiPlace);//.Replace("{+}-", "▼").Replace("{+}", "▲");

        _factionIndex = _factionIndex + 1;

        // Console.WriteLine(rank);
        // Console.WriteLine(name);
        // Console.WriteLine(leader);
        // Console.WriteLine(overallValue);
        // Console.WriteLine(spawnerValue);
        // Console.WriteLine(spawnerPlaced);
        // Console.WriteLine(spawnerStored);
        // Console.WriteLine(hopperValue);
        // Console.WriteLine(changePercent);
        // Console.WriteLine(changeOverallValue);
        // Console.WriteLine(changeSpawnerValue);
        // Console.WriteLine(changeSpawnerPlaced);
        // Console.WriteLine(changeSpawnerStored);
        // Console.WriteLine(changeHopperValue);
        // Console.WriteLine(factionuuid);
        Console.WriteLine(factionData);
        // UnloadBot();
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
}

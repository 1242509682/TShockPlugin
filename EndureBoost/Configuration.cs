using Newtonsoft.Json;
using TShockAPI;

public class Configuration
{
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "EndureBoost.json");
    [JsonProperty("����Ǯ��")]
    public bool bank = true;
    [JsonProperty("������")]
    public bool bank2 = true;
    [JsonProperty("������¯")]
    public bool bank3 = false;
    [JsonProperty("��ձ��ش�")]
    public bool bank4 = false;
    [JsonProperty("����ʱ��(s)")]
    public int duration = 3600;

    public class Potion
    {
        [JsonProperty("ҩˮid")]
        public int[] ItemID { get; set; }
        [JsonProperty("ҩˮ����")]
        public int RequiredStack { get; set; }
    }

    public class Station
    {
        [JsonProperty("��Ʒid")]
        public int[] Type { get; set; }
        [JsonProperty("��Ʒ����")]
        public int RequiredStack { get; set; }
        [JsonProperty("��buff��id")]
        public int BuffType { get; set; }
    }
    [JsonProperty("ҩˮ")]
    public List<Potion> Potions { get; set; } = new List<Potion>();
    [JsonProperty("������Ʒ")]
    public List<Station> Stations { get; set; } = new List<Station>();

    public void Write(string path)
    {
        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
        {
            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (var sw = new StreamWriter(fs))
            {
                sw.Write(str);
            }
        }
    }

    public static Configuration Read(string path)
    {
        if (!File.Exists(path))
            return new Configuration();
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var sr = new StreamReader(fs);
        return JsonConvert.DeserializeObject<Configuration>(sr.ReadToEnd()) ?? new();
    }
}

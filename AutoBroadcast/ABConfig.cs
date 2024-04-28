using Newtonsoft.Json;


public class ABConfig
{
    [JsonProperty("�㲥�б�")]
    public Broadcast[] Broadcasts = new Broadcast[0];

    public ABConfig Write(string file)
    {
        File.WriteAllText(file, JsonConvert.SerializeObject(this, Formatting.Indented));
        return this;
    }

    public static ABConfig Read(string file)
    {
        if (!File.Exists(file))
        {
            WriteExample(file);
        }
        return JsonConvert.DeserializeObject<ABConfig>(File.ReadAllText(file));
    }

    public static void WriteExample(string file)
    {
        Broadcast broadcast = new Broadcast
        {
            Name = "ʾ���㲥",
            Enabled = false,
            Messages = new[] { "����һ���㲥ʾ��", "ÿ5���Ӳ���һ��", "�㲥Ҳ����ִ������", "/time noon" },
            ColorRGB = new[] { 255f, 0f, 0f },
            Interval = 300,
            StartDelay = 60
        };

        ABConfig aBConfig = new ABConfig
        {
            Broadcasts = new[] { broadcast }
        };

        aBConfig.Write(file);
    }

    public class Broadcast
    {
        [JsonProperty("����")]
        public string Name { get; set; } = string.Empty;
        [JsonProperty("�Ƿ�����")]
        public bool Enabled { get; set; } = false;
        [JsonProperty("��Ϣ�б�")]
        public string[] Messages { get; set; } = new string[0];
        [JsonProperty("��ɫRGB")]
        public float[] ColorRGB { get; set; } = new float[3];
        [JsonProperty("���ʱ��")]
        public int Interval { get; set; } = 0;
        [JsonProperty("�ӳ�ʱ��")]
        public int StartDelay { get; set; } = 0;
        [JsonProperty("��������")]
        public string[] TriggerRegions { get; set; } = new string[0];
        [JsonProperty("���򴥷���")]
        public string RegionTrigger { get; set; } = "none";
        [JsonProperty("��")]
        public string[] Groups { get; set; } = new string[0];
        [JsonProperty("������")]
        public string[] TriggerWords { get; set; } = new string[0];
        [JsonProperty("�Ƿ񴥷�������")]
        public bool TriggerToWholeGroup { get; set; } = false;
    }
}
using Newtonsoft.Json;
using System.Text;
using TShockAPI;

namespace PvPer
{
    public class Configuration
    {
        [JsonProperty("���Ȩ����")]
        public string README1 = "pvper.use / pvper.admin";
        [JsonProperty("�������߽�˵��")]
        public string README2 = "/pvp set 3 4 Ҫ����Ҵ�������߻��3������";
        [JsonProperty("�������߽�˵��2")]
        public string README3 = "��ȡ��Χ�������ҳ������������,���ص����������ĵ�ָ������λ�ã���Ϊ������������λ�ã�,�ر�ɱ�����ѡ���Ĭ�Ͽ�����Ѫ";
        [JsonProperty("���ü��")]
        public bool CheckaAll = true;
        [JsonProperty("�Ƿ����7����Ʒ��")]
        public bool Check7 = false;
        [JsonProperty("���ؾ�����")]
        public bool PullArena = true;
        [JsonProperty("��ȡ��Χ")]
        public int PullRange = -20;
        [JsonProperty("�뿪������ɱ�����")]
        public bool KillPlayer = false;
        [JsonProperty("�볡��Ѫ")]
        public int SlapPlayer = 20;
        [JsonProperty("�����ߴ�������.X")]
        public int Player1PositionX = 0;
        [JsonProperty("�����ߴ�������.Y")]
        public int Player1PositionY = 0;
        [JsonProperty("�����ߴ�������.X")]
        public int Player2PositionX = 0;
        [JsonProperty("�����ߴ�������.Y")]
        public int Player2PositionY = 0;
        [JsonProperty("���������Ͻ�����.X")]
        public int ArenaPosX1 = 0;
        [JsonProperty("���������Ͻ�����.Y")]
        public int ArenaPosY1 = 0;
        [JsonProperty("���������½�����.X")]
        public int ArenaPosX2 = 0;
        [JsonProperty("���������½�����.Y")]
        public int ArenaPosY2 = 0;

        public static readonly string FilePath = Path.Combine(TShock.SavePath + "/����ϵͳ.json");

        [JsonProperty("��������")]
        public List<string> WeaponList { get; set; } = new List<string>();

        [JsonProperty("��BUFF��")]
        public List<int> BuffList { get; set; } = new List<int>();


        #region �����ļ���ȡ��������
        public void Write(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
            {
                var str = JsonConvert.SerializeObject(this, Formatting.Indented);
                sw.Write(str);
            }
        }

        public static Configuration Read(string path)
        {
            if (!File.Exists(path))
            {
                var defaultConfig = new Configuration();
                defaultConfig.Write(path);
                return defaultConfig;
            }
            else
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var sr = new StreamReader(fs))
                {
                    var json = sr.ReadToEnd();
                    var cf = JsonConvert.DeserializeObject<Configuration>(json);
                    return cf!;
                }
            }
        } 
        #endregion
    }
}
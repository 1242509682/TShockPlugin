using Newtonsoft.Json;

namespace CriticalHit;

public class CritMessage
{
    [JsonProperty("��ϸ��Ϣ����")]
    public Dictionary<string, int[]> Messages = new Dictionary<string, int[]>();
}

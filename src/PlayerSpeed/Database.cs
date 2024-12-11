using MySql.Data.MySqlClient;
using TShockAPI;
using TShockAPI.DB;

namespace PlayerSpeed;

public class Database
{
    #region ���ݵĽṹ��
    public class PlayerData
    {
        //�������
        public string Name { get; set; }
        //���ƴ���
        public int Count { get; set; }
        //��������ģʽ����
        public bool Enabled { get; set; } = false;
        //��ȴʱ��
        public DateTime CoolTime { get; set; }
        //���ʱ��
        public DateTime RangeTime { get; set; }
        internal PlayerData(string name = "",int count = 0 , bool enabled = true, DateTime coolTime = default, DateTime rangeTime = default)
        {
            this.Name = name ?? "";
            this.Count = count;
            this.Enabled = enabled;
            this.CoolTime = coolTime == default ? DateTime.UtcNow : coolTime;
            this.RangeTime = rangeTime;
        }
    }
    #endregion

    #region ���ݿ��ṹ��ʹ��Tshock�Դ������ݿ���Ϊ�洢��
    public Database()
    {
        var sql = new SqlTableCreator(TShock.DB, new SqliteQueryCreator());

        // ���岢ȷ�� PlayerSpeed ��Ľṹ
        sql.EnsureTableStructure(new SqlTable("PlayerSpeed", //����
            new SqlColumn("ID", MySqlDbType.Int32) { Primary = true, Unique = true, AutoIncrement = true }, // ������
            new SqlColumn("Name", MySqlDbType.TinyText) { NotNull = true }, // �ǿ��ַ�����
            new SqlColumn("Enabled", MySqlDbType.Int32) { DefaultValue = "0" }, // boolֵ��
            new SqlColumn("Count", MySqlDbType.Int32), //���Ƽ���
            new SqlColumn("CoolTime", MySqlDbType.DateTime), // ��ȴʱ��
            new SqlColumn("RangeTime", MySqlDbType.DateTime) // �������

        ));
    }
    #endregion

    #region Ϊ��Ҵ������ݷ���
    public bool AddData(PlayerData data)
    {
        return TShock.DB.Query("INSERT INTO PlayerSpeed (Name, Enabled,Count, CoolTime, RangeTime) VALUES (@0, @1, @2, @3, @4)",
            data.Name, data.Enabled ? 1 : 0,data.Count, data.CoolTime, data.RangeTime) != 0;
    }
    #endregion

    #region ɾ��ָ��������ݷ���
    public bool DeleteData(string name)
    {
        return TShock.DB.Query("DELETE FROM PlayerSpeed WHERE Name = @0", name) != 0;
    }
    #endregion

    #region �����������ݷ���
    public bool UpdateData(PlayerData data)
    {
        return TShock.DB.Query("UPDATE PlayerSpeed SET Enabled = @0, Count = @1, CoolTime = @2, RangeTime = @3 WHERE Name = @4",
            data.Enabled ? 1 : 0,data.Count, data.CoolTime, data.RangeTime, data.Name) != 0;
    }
    #endregion

    #region ��ȡ���ݷ���
    public PlayerData? GetData(string name)
    {
        using var reader = TShock.DB.QueryReader("SELECT * FROM PlayerSpeed WHERE Name = @0", name);

        return reader.Read()
            ? new PlayerData(
                name: reader.Get<string>("Name"),
                enabled: reader.Get<int>("Enabled") == 1,
                count: reader.Get<int>("Count"),
                coolTime: reader.Get<DateTime>("CoolTime"),
                rangeTime: reader.Get<DateTime>("RangeTime")
            )
            : null;
    }
    #endregion

    #region ��ȡ����������ݷ���
    public List<PlayerData> GetAll()
    {
        var data = new List<PlayerData>();
        using var reader = TShock.DB.QueryReader("SELECT * FROM PlayerSpeed");
        while (reader.Read())
        {
            data.Add(new PlayerData(
                name: reader.Get<string>("Name"),
                enabled: reader.Get<int>("Enabled") == 1,
                count: reader.Get<int>("Count"),
                coolTime: reader.Get<DateTime>("CoolTime"),
                rangeTime: reader.Get<DateTime>("RangeTime")
            ));
        }

        return data;
    }
    #endregion

    #region �����������ݷ���
    public bool ClearData()
    {
        return TShock.DB.Query("DELETE FROM PlayerSpeed") != 0;
    }
    #endregion
}
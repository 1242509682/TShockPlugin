using MySql.Data.MySqlClient;
using System.Text.Json;
using TShockAPI;
using TShockAPI.DB;

namespace AutoAirItem;

public class Database
{
    #region ����Ͱ���ݱ�ṹ
    public Database()
    {
        var sql = new SqlTableCreator(TShock.DB, new SqliteQueryCreator());

        // ���岢ȷ�� AutoTrash ��Ľṹ
        sql.EnsureTableStructure(new SqlTable("AutoTrash", //����
            new SqlColumn("ID", MySqlDbType.Int32) { Primary = true, Unique = true, AutoIncrement = true }, // ������
            new SqlColumn("Name", MySqlDbType.TinyText) { NotNull = true }, // �ǿ��ַ�����
            new SqlColumn("Enabled", MySqlDbType.Int32) { DefaultValue = "0" }, // boolֵ��
            new SqlColumn("Auto", MySqlDbType.Int32) { DefaultValue = "0" }, // boolֵ��
            new SqlColumn("Mess", MySqlDbType.Int32) { DefaultValue = "1" }, // boolֵ��
            new SqlColumn("ItemType", MySqlDbType.Text), // �ı��У����ڴ洢���л�����Ʒ�����б�
            new SqlColumn("DelItem", MySqlDbType.Text) // �ı��У����ڴ洢���л����Ƴ���Ʒ�ֵ�
        ));
    }
    #endregion

    #region ��������
    public bool UpdateData(MyData.PlayerData data)
    {
        var itemType = JsonSerializer.Serialize(data.ItemType);
        var delItem = JsonSerializer.Serialize(data.DelItem);

        // �������м�¼
        if (TShock.DB.Query("UPDATE AutoTrash SET Enabled = @0, Auto = @1, Mess = @2, ItemType = @3, DelItem = @4 WHERE Name = @5",
            data.Enabled ? 1 : 0, data.Auto ? 1 : 0, data.Mess ? 1 : 0, itemType, delItem, data.Name) != 0)
        {
            return true;
        }

        // ���û�и��µ��κμ�¼��������¼�¼
        return TShock.DB.Query("INSERT INTO AutoTrash (Name, Enabled, Auto, Mess, ItemType, DelItem) VALUES (@0, @1, @2, @3, @4, @5)",
            data.Name, data.Enabled ? 1 : 0, data.Auto ? 1 : 0, data.Mess ? 1 : 0, itemType, delItem) != 0;
    }
    #endregion

    #region �����������ݣ�ÿ������������ʱ���ڶ�ȡ֮ǰ���µ����ݣ���Ҫ���ڽ�����ڴ����ʱ���ݶ�ʧ�ķ���
    public List<MyData.PlayerData> LoadData()
    {
        var data = new List<MyData.PlayerData>();

        using var reader = TShock.DB.QueryReader("SELECT * FROM AutoTrash");

        while (reader.Read())
        {
            var ItemType = reader.Get<string>("ItemType");
            var DelItem = reader.Get<string>("DelItem");

            var ItemList = JsonSerializer.Deserialize<List<int>>(ItemType);
            var DelList = JsonSerializer.Deserialize<Dictionary<int, int>>(DelItem);

            data.Add(new MyData.PlayerData(
                name: reader.Get<string>("Name"),
                enabled: reader.Get<int>("Enabled") == 1,
                auto: reader.Get<int>("Auto") == 1,
                mess: reader.Get<int>("Mess") == 1,
                item: ItemList!,
                DelItem: DelList!
            ));
        }

        return data;
    }
    #endregion

    #region �����������ݷ���
    public bool ClearData()
    {
        return TShock.DB.Query("DELETE FROM AutoTrash") != 0;
    } 
    #endregion
}
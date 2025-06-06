﻿using Economics.RPG.Model;
using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace Economics.RPG;

public class PlayerLevelManager
{
    private readonly IDbConnection database;

    private readonly Dictionary<string, string> Levels;

    internal PlayerLevelManager()
    {
        this.database = TShock.DB;
        var Skeleton = new SqlTable("RPG",
            new SqlColumn("ID", MySqlDbType.Int32) { Length = 8, Primary = true, Unique = true, AutoIncrement = true },
            new SqlColumn("UserName", MySqlDbType.Text) { Length = 500 },
            new SqlColumn("Level", MySqlDbType.Text) { Length = 255 }
              );
        var List = new SqlTableCreator(this.database, this.database.GetSqlQueryBuilder());
        List.EnsureTableStructure(Skeleton);
        this.Levels = this.GetPlayerLevel();
    }

    private Dictionary<string, string> GetPlayerLevel()
    {
        Dictionary<string, string> levels = new();
        using var read = this.database.QueryReader("select * from `RPG`");
        while (read.Read())
        {
            var Name = read.Get<string>("UserName");
            var Level = read.Get<string>("Level");
            levels[Name] = Level;
        }
        return levels;
    }

    public Level GetLevel(string userName)
    {
        return this.Levels.TryGetValue(userName, out var level) && level != null
            ? RPG.Config.GetLevel(level) ?? RPG.Config.DefaultLevel
            : RPG.Config.DefaultLevel;
    }

    public bool HasLevel(string level)
    { 
        return this.Levels.ContainsValue(level);
    }

    public void ResetPlayerLevel(string userName)
    {
        this.Update(userName, RPG.Config.DefaultLevel);
    }

    public void Update(string userName, Level level)
    {
        if (this.Levels.ContainsKey(userName))
        {
            this.database.Query("UPDATE `RPG` SET `Level` = @0 WHERE `RPG`.`UserName` = @1", level.Name, userName);
        }
        else
        {
            this.database.Query("INSERT INTO `RPG` (`UserName`, `Level`) VALUES (@0, @1)", userName, level.Name);
        }
        this.Levels[userName] = level.Name;
    }

    public void Remove(string userName)
    {
        if (this.Levels.ContainsKey(userName))
        {
            this.database.Query("DELETE FROM `RPG` WHERE `RPG`.`UserName` = @0", userName);
        }

        this.Levels.Remove(userName);
    }

    public void RemoveAll()
    {
        this.database.Query("delete from RPG");
        this.Levels.Clear();
    }
}
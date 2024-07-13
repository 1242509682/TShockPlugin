﻿using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;


namespace SpawnInfra
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        #region 插件信息
        public override string Name => "生成基础建设";
        public override string Author => "羽学";
        public override Version Version => new Version(1, 4, 0);
        public override string Description => "给新世界创建NPC住房、仓库、洞穴刷怪场、地狱/微光直通车、地表和地狱世界级平台（轨道）";
        #endregion

        #region 注册与释放
        public Plugin(Main game) : base(game) { }
        public override void Initialize()
        {
            LoadConfig();
            GeneralHooks.ReloadEvent += (_) => LoadConfig();
            ServerApi.Hooks.GamePostInitialize.Register(this, OnGamePostInitialize,20);//提高优先级避免覆盖CreateSpawn插件
            Commands.ChatCommands.Add(new Command("room.use", Comds.Comd, "rm", "基建")
            {
                HelpText = "生成基础建设"
            });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GeneralHooks.ReloadEvent -= (_) => LoadConfig();
                ServerApi.Hooks.GamePostInitialize.Deregister(this, OnGamePostInitialize);
                Commands.ChatCommands.Remove(new Command("room.use", Comds.Comd, "rm", "基建"));
            }
            base.Dispose(disposing);
        }
        #endregion

        #region 配置重载读取与写入方法
        internal static Configuration Config = new();
        private static void LoadConfig()
        {
            Config = Configuration.Read();
            Config.Write();
            TShock.Log.ConsoleInfo("[生成基础建设]重新加载配置完毕。");
        }
        #endregion

        #region  游戏初始化建筑设施（世界平台/轨道+地狱平台/轨道+出生点监狱+直通车、刷怪场、仓库、微光湖）
        private static void OnGamePostInitialize(EventArgs args)
        {
            if (args == null) return;

            if (Config.Enabled)
            {
                foreach (var item in Config.Prison)
                {
                    GenLargeHouse(Main.spawnTileX + item.spawnTileX, Main.spawnTileY + item.spawnTileY, item.Width, item.Height);
                }

                foreach (var item in Config.HellTunnel)
                {
                    if (Config.HellTunnel[0].Enabled7)
                    {
                        RockTrialField(Main.rockLayer, item.Height, item.Width3, item.Center);
                        HellTunnel(Main.spawnTileX + item.SpawnTileX, Main.spawnTileY + item.SpawnTileY, item.Width);
                    }
                    else
                    {
                        HellTunnel(Main.spawnTileX + item.SpawnTileX, Main.spawnTileY + item.SpawnTileY, item.Width);

                        RockTrialField(Main.rockLayer, item.Height, item.Width3, item.Center);
                    }

                    ShimmerBiome(item.Width2);
                    UnderworldPlatform(Main.UnderworldLayer + item.PlatformY, item.PlatformY);
                }

                //太空层
                var sky = Main.worldSurface * 0.3499999940395355;
                foreach (var item in Config.WorldPlatform)
                {
                    WorldPlatform((int)sky - item.SkyY, item.Height);
                    BuildOceanPlatforms(item.Wide, item.Height2, item.Interval, item.Interval - 1);
                }

                foreach (var item in Config.Chests)
                {
                    SpawnChest(Main.spawnTileX + item.spawnTileX, Main.spawnTileY + item.spawnTileY, item.Height, item.Width, item.Count, item.Layers);
                }

                TShock.Utils.Broadcast(
                    "基础建设已完成，如需重置布局\n" +
                    "请输入指令后重启服务器：/rm reset", 250, 247, 105);

                Commands.HandleCommand(TSPlayer.Server, "/save");
                Commands.HandleCommand(TSPlayer.Server, "/clear i 9999");

                Config.Enabled = false;
                Config.Write();
            }
        }
        #endregion

        #region 刷怪场
        private static void RockTrialField(double posY, int Height, int Width, int CenterVal)
        {
            int clear = (int)posY - Height;
            // 计算顶部、底部和中间位置
            int top = clear + Height * 2;
            int bottom = (int)posY + Height * 2;
            int middle = (top + bottom) / 2 + CenterVal;

            int left = Math.Max(Main.spawnTileX - Width, 0);
            int right = Math.Min(Main.spawnTileX + Width, Main.maxTilesX);

            int CenterLeft = Main.spawnTileX - 8 - CenterVal;
            int CenterRight = Main.spawnTileX + 8 + CenterVal;

            if (Config.HellTunnel[0].Enabled5)
            {
                for (int y = (int)posY; y > clear; y--)
                {
                    for (int x = left; x < right; x++)
                    {
                        Main.tile[x, y + Height * 2].ClearEverything(); // 清除方块

                        WorldGen.PlaceTile(x, top, Config.HellTunnel[0].ID, false, true, -1, 0); // 在清理顶部放1层（防液体流进刷怪场）

                        WorldGen.PlaceTile(x, bottom, Config.HellTunnel[0].ID, false, true, -1, 0); //刷怪场底部放1层

                        WorldGen.PlaceTile(x, middle, Config.HellTunnel[0].ID, false, true, -1, 0); //刷怪场中间放1层（刷怪用）

                        WorldGen.PlaceTile(x, middle + 8 + CenterVal, Config.HellTunnel[0].PlatformID, false, true, -1, Config.HellTunnel[0].PlatformStyle);//中间下8格放一层方便站人


                        for (int wallY = middle + 1; wallY <= middle + 7 + CenterVal; wallY++)
                        {
                            Main.tile[x, wallY].wall = 155; // 放置墙壁
                        }

                        WorldGen.PlaceTile(x, middle + 11 + CenterVal, Config.HellTunnel[0].ID, false, true, -1, 0); //中间下11格放箱子的实体块
                        WorldGen.PlaceTile(x, middle + 10 + CenterVal, Config.Chests[0].ChestID, false, true, -1, Config.Chests[0].ChestStyle); //中间下10格放箱子

                        WorldGen.PlaceTile(x, middle + 2, Config.HellTunnel[0].ID, false, true, -1, 0); //放计时器的平台

                        Main.tile[x, middle + 3].liquid = 60;  //设置1格液体
                        Main.tile[x, middle + 3].liquidType(1); // 设置为岩浆
                        WorldGen.SquareTileFrame(x, middle + 3, false);
                        WorldGen.PlaceTile(x, middle + 4, Config.HellTunnel[0].ID, false, true, -1, 0); //防掉怪下来


                        // 如果x值在中心范围内，放置10格高的方块
                        if (x >= CenterLeft && x <= CenterRight)
                        {
                            for (int wallY = middle - 10 - CenterVal; wallY <= middle - 1; wallY++)
                            {
                                // 创建矩形判断
                                if (wallY >= middle - 10 - CenterVal && wallY <= middle - 1 && x >= CenterLeft + 1 && x <= CenterRight - 1)
                                    // 挖空方块
                                    Main.tile[x, wallY].ClearEverything();
                                else
                                {
                                    // 在矩形范围外放置方块
                                    WorldGen.PlaceTile(x, wallY, Config.HellTunnel[0].ID, false, true, -1, 0);
                                }

                                // 检查是否在中间位置，如果是则放置岩浆
                                if (wallY == middle - 10 - CenterVal)
                                {
                                    Main.tile[x - 1, wallY + 2].liquid = 60;  //设置1格液体
                                    Main.tile[x - 1, wallY + 2].liquidType(1); // 设置为岩浆
                                    WorldGen.SquareTileFrame(x, wallY + 2, false);
                                    //加一排尖球
                                    for (int j = 1; j <= 5 + CenterVal; j++)
                                    {
                                        WorldGen.PlaceWire(x - j, wallY - j);
                                        WorldGen.PlaceWire(x + j, wallY - j);
                                        WorldGen.PlaceTile(x - j, wallY - j, 137, true, false, -1, 3);
                                        WorldGen.PlaceTile(x + j, wallY - j, 137, true, false, -1, 3);
                                        WorldGen.PlaceActuator(x - j, wallY - j);
                                        WorldGen.PlaceActuator(x + j, wallY - j);
                                    }
                                    //给尖球加电线
                                    for (int j = 2; j <= 10 + CenterVal; j++)
                                    {
                                        WorldGen.PlaceWire(CenterLeft - 1, middle - j);
                                        WorldGen.PlaceWire(CenterRight + 1, middle - j);
                                    }
                                }
                            }

                            //定刷怪区
                            for (int i = 0; i <= 3; i++)
                            {
                                WorldGen.PlaceTile(CenterLeft - 61, middle + i, Config.HellTunnel[0].ID, false, true, -1, 0);
                                WorldGen.PlaceTile(CenterRight + 61, middle + i, Config.HellTunnel[0].ID, false, true, -1, 0);
                                WorldGen.PlaceTile(CenterLeft - 85, middle + i, Config.HellTunnel[0].ID, false, true, -1, 0);
                                WorldGen.PlaceTile(CenterRight + 85, middle + i, Config.HellTunnel[0].ID, false, true, -1, 0);
                            }
                        }
                        else //不在中心 左右生成半砖推怪平台
                        {
                            Main.tile[x, middle - 1].type = Config.HellTunnel[0].ID;
                            Main.tile[x, middle - 1].active(true);
                            Main.tile[x, middle - 1].halfBrick(false);

                            // 根据x值确定斜坡方向
                            if (x < Main.spawnTileX)
                            {
                                Main.tile[x, middle - 1].slope(3); // 设置为右斜坡
                            }
                            else
                            {
                                Main.tile[x, middle - 1].slope(4); // 设置为左斜坡
                            }
                            // 把半砖替换成推怪平台
                            WorldGen.PlaceTile(x, middle - 1, Config.HellTunnel[0].PlatformID, false, true, -1, Config.HellTunnel[0].PlatformStyle);
                            //平台加电线+制动器
                            WorldGen.PlaceWire(x, middle - 1);
                            WorldGen.PlaceActuator(x, middle - 1);

                            // 在电线的末端放置1/4秒计时器并连接
                            WorldGen.PlaceWire(CenterLeft - 1, middle + 1);
                            WorldGen.PlaceWire(CenterRight + 1, middle + 1);
                            WorldGen.PlaceWire(CenterLeft - 1, middle);
                            WorldGen.PlaceWire(CenterRight + 1, middle);
                            WorldGen.PlaceTile(CenterLeft - 1, middle + 1, 144, false, true, -1, 4);
                            WorldGen.PlaceTile(CenterRight + 1, middle + 1, 144, false, true, -1, 4);

                            //在1/4秒计时器下面连接开关
                            for (int i = 2; i <= 5; i++)
                            {
                                WorldGen.PlaceWire(CenterLeft - 1, middle + i);
                                WorldGen.PlaceWire(CenterRight + 1, middle + i);
                                WorldGen.PlaceTile(CenterLeft - 1, middle + 5, 136, false, true, -1, 0);
                                WorldGen.PlaceTile(CenterRight + 1, middle + 5, 136, false, true, -1, 0);
                            }

                            //放个花园侏儒
                            WorldGen.PlaceGnome(CenterLeft - 5, middle + 7, 0);
                        }
                    }
                }
            }
            //只清不建
            else if (Config.HellTunnel[0].Enabled6)
            {
                for (int y = (int)posY; y > clear; y--)
                {
                    for (int x = left; x < right; x++)
                    {
                        Main.tile[x, y + Height * 2].ClearEverything(); // 清除方块
                        WorldGen.PlaceTile(x, top, Config.HellTunnel[0].ID, false, true, -1, 0); // 在清理顶部放1层（防液体流进刷怪场）
                        WorldGen.PlaceTile(x, bottom, Config.HellTunnel[0].ID, false, true, -1, 0); //刷怪场底部放1层
                        WorldGen.PlaceTile(CenterLeft, middle, 267, false, true, -1, 0); //定中心
                    }
                }
            }
        }
        #endregion

        #region 构造监狱集群方法
        public static void GenLargeHouse(int startX, int startY, int width, int height)
        {
            if (!Config.Prison[0].Enabled) return;

            int roomsAcross = width / 5;
            int roomsDown = height / 9;

            for (int row = 0; row < roomsDown; row++)
            {
                for (int col = 0; col < roomsAcross; col++)
                {
                    bool isRight = col == roomsAcross;
                    GenRoom(startX + col * 5, startY + row * 9, isRight);
                }
            }
        }
        #endregion

        #region 创建小房间方法
        public static void GenRoom(int posX, int posY, bool isRight = true)
        {
            RoomTheme roomTheme = new RoomTheme();
            roomTheme.SetGlass();
            ushort tile = roomTheme.tile;
            ushort wall = roomTheme.wall;
            TileInfo platform = roomTheme.platform;
            TileInfo chair = roomTheme.chair;
            TileInfo bench = roomTheme.bench;
            TileInfo torch = roomTheme.torch;
            int num = posX;
            int num2 = 6;
            int num3 = 10;
            if (!isRight)
            {
                num += 6;
            }
            for (int i = num; i < num + num2; i++)
            {
                for (int j = posY - num3; j < posY; j++)
                {
                    ITile val = Main.tile[i, j];
                    val.ClearEverything();
                    if (i > num && j < posY - 5 && i < num + num2 - 1 && j > posY - num3)
                    {
                        val.wall = wall;
                    }
                    if (i == num && j > posY - 5 || i == num + num2 - 1 && j > posY - 5 || j == posY - 1)
                    {
                        WorldGen.PlaceTile(i, j, platform.id, false, true, -1, platform.style);
                    }
                    else if (i == num || i == num + num2 - 1 || j == posY - num3 || j == posY - 5)
                    {
                        val.type = tile;
                        val.active(true);
                        val.slope(0);
                        val.halfBrick(false);
                    }
                }
            }
            if (isRight)
            {
                WorldGen.PlaceTile(num + 1, posY - 6, chair.id, false, true, 0, chair.style);
                ITile obj = Main.tile[num + 1, posY - 6];
                obj.frameX += 18;
                ITile obj2 = Main.tile[num + 1, posY - 7];
                obj2.frameX += 18;
                WorldGen.PlaceTile(num + 2, posY - 6, bench.id, false, true, -1, bench.style);
                WorldGen.PlaceTile(num + 4, posY - 5, torch.id, false, true, -1, torch.style);
            }
            else
            {
                WorldGen.PlaceTile(num + 4, posY - 6, chair.id, false, true, 0, chair.style);
                WorldGen.PlaceTile(num + 2, posY - 6, bench.id, false, true, -1, bench.style);
                WorldGen.PlaceTile(num + 1, posY - 5, torch.id, false, true, -1, torch.style);
            }
        }

        //指令方法 用来给玩家自己建晶塔房用
        public static Task AsyncGenRoom(TSPlayer plr, int posX, int posY, int total = 1, bool isRight = true, bool needCenter = false)
        {
            int secondLast = Utils.GetUnixTimestamp;
            return Task.Run(delegate
            {
                int num = 5;
                int num2 = 1 + total * num;
                int num3 = needCenter ? posX - num2 / 2 : posX;
                for (int i = 0; i < total; i++)
                {
                    GenRoom(num3, posY, isRight);
                    num3 += num;
                }
            }).ContinueWith(delegate
            {
                TileHelper.InformPlayers();
                int value = Utils.GetUnixTimestamp - secondLast;
                plr.SendSuccessMessage($"已生成{total}个小房子，用时{value}秒。");
            });
        }

        #endregion

        #region 箱子集群方法
        private static void SpawnChest(int posX, int posY, int hight, int width, int count, int layers)
        {
            if (!Config.Chests[0].Enabled) return;

            int ClearHeight = hight + (layers - 1) * (width + Config.Chests[0].LayerHeight);

            for (int x = posX; x < posX + width * count; x++)
            {
                for (int y = posY - ClearHeight; y <= posY; y++)
                    Main.tile[x, y].ClearEverything();
            }

            for (int layer = 0; layer < layers; layer++)
            {
                for (int i = 0; i < count; i++)
                {
                    int currentXPos = posX + i * width;
                    int currentYPos = posY - (layer * (width + Config.Chests[0].LayerHeight));

                    for (int wx = currentXPos; wx < currentXPos + width; wx++)
                    {
                        WorldGen.PlaceTile(wx, currentYPos + 1, Config.Chests[0].PlatformID, false, true, -1, Config.Chests[0].PlatformStyle);
                    }

                    WorldGen.PlaceTile(currentXPos, currentYPos, Config.Chests[0].ChestID, false, true, -1, Config.Chests[0].ChestStyle);
                }
            }
        }
        #endregion

        #region 左海平台
        private static void BuildOceanPlatforms(int wide, int hight, int interval, int IntlClear)
        {
            if (!Config.WorldPlatform[0].Enabled3) return;

            var sky = Main.worldSurface * 0.3499999940395355;
            int clear = Math.Max(3, hight);

            for (int y = Main.oceanBG + interval; y < sky + clear; y += interval)
            {
                // 清理平台下方的方块
                for (int yBelow = y - IntlClear; yBelow < y; yBelow++)
                {
                    for (int x = 0; x < wide; x++)
                    {
                        Main.tile[x, yBelow].ClearEverything();
                    }
                }

                // 放置平台
                for (int x = 0; x < wide; x++)
                {
                    Main.tile[x, y].ClearEverything();
                    if (x >= Main.maxTilesX || x < 0) continue;
                    WorldGen.PlaceTile(x, y, Config.WorldPlatform[0].ID, false, true, -1, Config.WorldPlatform[0].Style);
                }
            }
        }
        #endregion

        #region 世界平台
        private static void WorldPlatform(int posY, int hight)
        {
            int clear = Math.Max(3, hight);

            for (int x = 0; x < Main.maxTilesX; x++)
            {
                if (Config.WorldPlatform[0].Enabled3)
                {
                    if (x - clear <= Main.oceanBG + Config.WorldPlatform[0].Limit) continue;
                }

                for (int y = posY - clear; y <= posY; y++)
                {
                    Main.tile[x, y].ClearEverything();
                }

                if (Config.WorldPlatform[0].Enabled)
                    WorldGen.PlaceTile(x, posY, Config.WorldPlatform[0].ID, false, true, -1, Config.WorldPlatform[0].Style);
                if (Config.WorldPlatform[0].Enabled2)
                    WorldGen.PlaceTile(x, posY - 1, 314, false, true, -1, 0);
            }
        }
        #endregion

        #region 地狱直通车
        private static int HellTunnel(int posX, int posY, int Width)
        {
            int hell = 0;

            if (Config.HellTunnel[0].Enabled)
            {
                int xtile;

                for (hell = Main.UnderworldLayer + 10; hell <= Main.maxTilesY - 100; hell++)
                {
                    xtile = posX;
                    Parallel.For(posX, posX + 8, delegate (int cwidth, ParallelLoopState state)
                    {
                        if (Main.tile[cwidth, hell].active() && !Main.tile[cwidth, hell].lava())
                        {
                            state.Stop();
                            xtile = cwidth;
                        }
                    });
                    if (!Main.tile[xtile, hell].active())
                    {
                        break;
                    }
                }
                int num = hell;
                int Xstart = posX - 2;
                Parallel.For(Xstart, Xstart + Width, delegate (int cx)
                {
                    Parallel.For(posY, hell, delegate (int cy)
                    {
                        ITile val = Main.tile[cx, cy];
                        val.ClearEverything();
                        if (cx == Xstart + Width / 2)
                        {
                            val.type = Config.HellTunnel[0].ID2; //绳子
                            val.active(true);
                            val.slope(0);
                            val.halfBrick(false);
                        }
                        else if (cx == Xstart || cx == Xstart + Width - 1)
                        {
                            val.type = Config.HellTunnel[0].ID; //边界方块
                            val.active(true);
                            val.slope(0);
                            val.halfBrick(false);
                        }
                    });
                });

                int platformStart = Xstart + 1;
                int platformEnd = Xstart + Width - 2;
                //确保平台与直通车等宽
                for (int px = platformStart; px <= platformEnd; px++)
                {
                    WorldGen.PlaceTile(px, posY, Config.HellTunnel[0].PlatformID, false, true, -1, Config.HellTunnel[0].PlatformStyle);
                    for (int cy = posY + 1; cy <= hell; cy++)
                    {
                        Main.tile[px, cy].wall = 155; // 放置墙壁
                    }
                }
            }
            return hell;
        }
        #endregion

        #region 地狱平台
        private static void UnderworldPlatform(int posY, int hight)
        {
            int Clear = posY - hight;
            for (int y = posY; y > Clear; y--)
                for (int x = 0; x < Main.maxTilesX; x++)
                {
                    Main.tile[x, y].ClearEverything(); // 清除方块

                    if (Config.HellTunnel[0].Enabled2)
                        WorldGen.PlaceTile(x, posY, Config.HellTunnel[0].PlatformID, false, true, -1, Config.HellTunnel[0].PlatformStyle); //地狱平台

                    if (Config.HellTunnel[0].Enabled3)
                        WorldGen.PlaceTile(x, posY - 1, 314, false, true, -1, 0); //地狱轨道
                }
        }
        #endregion

        #region 微光湖直通车
        private static void ShimmerBiome(int Width)
        {
            if (!Config.HellTunnel[0].Enabled4) return;

            //西江的判断微光湖位置方法
            var skipTile = new bool[Main.maxTilesX, Main.maxTilesY];
            for (int x = 0; x < Main.maxTilesX; x++)
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    var tile = Main.tile[x, y];
                    if (tile is null || skipTile[x, y]) continue;
                    if (tile.shimmer())
                    {
                        int Right = x;
                        int Bottom = y;
                        for (int right = x; right < Main.maxTilesX; right++)
                        {
                            if (!Main.tile[right, y].shimmer())
                            {
                                Right = right;
                                break;
                            }
                        }

                        for (int right = x; right < Right; right++)
                        {
                            for (int bottom = y; bottom < Main.maxTilesY; bottom++)
                            {
                                if (!Main.tile[right, bottom].shimmer())
                                {
                                    if (bottom > Bottom)
                                    {
                                        Bottom = bottom;
                                    }
                                    break;
                                }
                            }
                        }

                        for (int start = x - 2; start < Right + 2; start++)
                        {
                            for (int end = y; end < Bottom + 2; end++)
                            {
                                skipTile[start, end] = true;
                            }
                        }

                        #region 微光湖直通车
                        // 找到微光湖的中心点
                        int CenterX = (x + Right) / 2;
                        //深度到为中心湖面
                        int CenterY = (y + Bottom) / 2 - 8;
                        //中止条件为世界轨道的距离
                        var sky = Main.worldSurface * 0.3499999940395355;
                        int Height = (int)sky - Config.WorldPlatform[0].SkyY;

                        // 从微光湖中心点向上挖通道直至地表
                        for (int TunnelY = CenterY; TunnelY >= Height; TunnelY--)
                            for (int TunnelX = CenterX - Width; TunnelX <= CenterX + Width; TunnelX++)
                            {
                                if (TunnelX >= 0 && TunnelX < Main.maxTilesX)
                                {
                                    Main.tile[TunnelX, TunnelY].ClearEverything();
                                    //直通车的两侧的方块
                                    if (TunnelX == CenterX - Width || TunnelX == CenterX + Width)
                                        WorldGen.PlaceTile(TunnelX, TunnelY, Config.HellTunnel[0].ID, false, true, -1, 0);

                                    //微光湖底部放一层雨云 防摔
                                    else if (TunnelY == CenterY)
                                        WorldGen.PlaceTile(TunnelX, TunnelY, 460, false, true, -1, 0);
                                }
                            }
                        #endregion

                    }
                }
        }
    }
    #endregion

}

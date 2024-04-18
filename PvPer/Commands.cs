using Microsoft.Xna.Framework;
using TShockAPI;

namespace PvPer
{
    public class Commands
    {
        public static void Duel(CommandArgs args)
        {
            if (args.Parameters.Count == 0)
            {
                HelpCmd(args);
                return;
            }

            switch (args.Parameters[0].ToLower())
            {
                case "h":
                case "help":
                case "�˵�":
                    if (args.Parameters.Count < 2)
                    {
                        HelpCmd(args);
                    }
                    return; //����
                case "0":
                case "add":
                case "����":
                    if (args.Parameters.Count < 2)
                    {
                        args.Player.SendErrorMessage("��ָ��Ŀ����ҵ����ơ�");
                    }
                    else
                    {
                        InviteCmd(args);
                    }
                    return; //����
                case "1":
                case "yes":
                case "����":
                    AcceptCmd(args);
                    return;
                case "2":
                case "no":
                case "�ܾ�":
                    RejectCommand(args);
                    return;
                case "data":
                case "mark":
                case "ս��":
                    StatsCommand(args);
                    return;
                case "l":
                case "list":
                case "����":
                    LeaderboardCommand(args);
                    return;
                case "s":
                case "set":
                case "����":
                    {
                        int result;
                        if (args.Parameters.Count == 2 && int.TryParse(args.Parameters[1], out result) && IsValidLocationType(result))
                        {
                            int x = args.Player.TileX;
                            int y = args.Player.TileY;

                            switch (result)
                            {
                                case 1:
                                    PvPer.Config.Player1PositionX = x;
                                    PvPer.Config.Player1PositionY = y;
                                    args.Player.SendMessage($"�ѽ������ڵ�λ������Ϊ[c/F75454:������]���͵㣬����Ϊ({x}, {y})", Color.CadetBlue);
                                    Console.WriteLine($"������ϵͳ�������ߴ��͵������ã�����Ϊ({x}, {y})", Color.BurlyWood);
                                    break;
                                case 2:
                                    PvPer.Config.Player2PositionX = x;
                                    PvPer.Config.Player2PositionY = y;
                                    args.Player.SendMessage($"�ѽ������ڵ�λ������Ϊ[c/49B3D6:������]���͵㣬����Ϊ({x}, {y})", Color.CadetBlue);
                                    Console.WriteLine($"������ϵͳ�������ߴ��͵������ã�����Ϊ({x}, {y})", Color.BurlyWood);
                                    break;

                                case 3:
                                    PvPer.Config.ArenaPosX1 = x;
                                    PvPer.Config.ArenaPosY1 = y;
                                    args.Player.SendMessage($"�ѽ������ڵ�λ������Ϊ[c/9487D6:������]���Ͻǣ�����Ϊ({x}, {y})", Color.Yellow);
                                    Console.WriteLine($"������ϵͳ�����������Ͻ������ã�����Ϊ({x}, {y})", Color.Yellow);
                                    break;
                                case 4:
                                    PvPer.Config.ArenaPosX2 = x;
                                    PvPer.Config.ArenaPosY2 = y;
                                    args.Player.SendMessage($"�ѽ������ڵ�λ������Ϊ[c/9487D6:������]���½ǣ�����Ϊ({x}, {y})", Color.Yellow);
                                    Console.WriteLine($"������ϵͳ�����������½������ã�����Ϊ({x}, {y})", Color.Yellow);
                                    break;

                                default:
                                    args.Player.SendErrorMessage("[i:4080]ָ�����! [c/CCEB60:��ȷָ��: /pvp set [1/2/3/4]]");
                                    return;
                            }

                            PvPer.Config.Write(Configuration.FilePath);
                        }
                        else
                        {
                            args.Player.SendErrorMessage("[i:4080]ָ�����! \n��ȷָ��: /pvp set [1/2/3/4] - [c/7EE874:1/2���λ�� 3/4�������߽�]");
                        }
                        break;
                    }
                case "r":
                case "reset":
                case "����":
                    if (args.Parameters.Count < 2)
                    {
                        var name = args.Player.Name;
                        // Ȩ��
                        if (!args.Player.HasPermission("pvper.admin"))
                        {
                            args.Player.SendErrorMessage("��û�����þ���ϵͳ���ݱ��Ȩ�ޡ�");
                            TShock.Log.ConsoleInfo($"{name}��ͼִ�����þ���ϵͳ����ָ��");
                            return;
                        }
                        else
                        {
                            ClearAllData(args);
                        }
                    }
                    return; //����
                default:
                    HelpCmd(args);
                    break;
            }
        }


        private static void HelpCmd(CommandArgs args)
        {
            if (args.Player != null)
            {
                args.Player.SendMessage("������ϵͳ����ο�����ָ��˵���\n " +
                 "[c/FFFE80:/pvp add �� /pvp ���� �����] - [c/7EE874:������ҲμӾ���] \n " +
                 "[c/74D3E8:/pvp yes �� /pvp ����] - [c/7EE874:���ܾ���] \n " +
                 "[c/74D3E8:/pvp no �� /pvp �ܾ�] - [c/7EE874:�ܾ�����] \n " +
                 "[c/74D3E8:/pvp data �� /pvp ս��] - [c/7EE874:ս����ѯ]\n " +
                 "[c/74D3E8:/pvp list �� /pvp ����] - [c/7EE874:����]\n " +
                 "[c/FFFE80:/pvp s �� /pvp ���� 1 2 3 4] - [c/7EE874:1/2���λ�� 3/4�������߽�]\n " +
                 "[c/74D3E8:/pvp r �� /pvp ����] - [c/7EE874:����������ݿ�]\n ", Color.GreenYellow);
            }
        }

        #region ʹ��ָ���������ݿ⡢����λ�÷���
        private static void ClearAllData(CommandArgs args)
        {
            // ���Դ����ݿ���ɾ�������������
            if (DbManager.ClearData())
            {
                args.Player.SendSuccessMessage("���ݿ���������ҵľ��������ѱ��ɹ������");
                TShock.Log.ConsoleInfo("���ݿ���������ҵľ��������ѱ��ɹ������");
            }
            else
            {
                args.Player.SendErrorMessage("���������Ҿ�������ʱ��������");
                TShock.Log.ConsoleInfo("���������Ҿ�������ʱ��������");
            }
        }
        #endregion

        private static bool IsValidLocationType(int locationType)
        {
            return locationType >= 1 && locationType <= 4;
        }



        private static void InviteCmd(CommandArgs args)
        {
            List<TSPlayer> plrList = TSPlayer.FindByNameOrID(string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1)));


            if (plrList.Count == 0)
            {
                args.Player.SendErrorMessage("δ�ҵ�ָ����ҡ�");
                return;
            }

            if (Utils.IsPlayerInADuel(args.Player.Index))
            {
                args.Player.SendErrorMessage("�������Ѿ��ھ������ˡ�");
                return;
            }

            TSPlayer targetPlr = plrList[0];

            if (targetPlr.Index == args.Player.Index)
            {
                args.Player.SendErrorMessage("���������Լ�������");
                return;
            }

            if (Utils.IsPlayerInADuel(targetPlr.Index))
            {
                args.Player.SendErrorMessage($"{targetPlr.Name} ���ڽ���һ��������");
                return;
            }

            PvPer.Invitations.Add(new Pair(args.Player.Index, targetPlr.Index));
            args.Player.SendSuccessMessage($"�ɹ����� {targetPlr.Name} ���о�����");
            targetPlr.SendMessage($"{args.Player.Name} [c/FE7F81:���������;�������] \n������ [c/CCFFCC:/pvp yes ����]  �� [c/FFE6CC:/pvp no�ܾ�] ", 255, 204, 255);
        }

        private static void AcceptCmd(CommandArgs args)
        {
            Pair? invitation = Utils.GetInvitationFromReceiverIndex(args.Player.Index);

            if (invitation == null)
            {
                args.Player.SendErrorMessage("[c/FE7F81:����ǰû���յ��κξ�������]");
                return;
            }

            invitation.StartDuel();
        }

        private static void RejectCommand(CommandArgs args)
        {
            Pair? invitation = Utils.GetInvitationFromReceiverIndex(args.Player.Index);

            if (invitation == null)
            {
                args.Player.SendErrorMessage("[c/FE7F81:����ǰû���յ��κξ�������]");
                return;
            }

            TShock.Players[invitation.Player1].SendErrorMessage("[c/FFCB80:�Է�����Ѿܾ����ľ�������]��");
            PvPer.Invitations.Remove(invitation);
        }

        private static void StatsCommand(CommandArgs args)
        {
            if (args.Parameters.Count < 2)
            {
                try
                {
                    DPlayer plr = PvPer.DbManager.GetDPlayer(args.Player.Account.ID);
                    args.Player.SendInfoMessage("[c/FFCB80:����ս��:]\n" +
                                                $"[c/63DC5A:��ɱ: ]{plr.Kills}\n" +
                                                $"[c/F56469:����:] {plr.Deaths}\n" +
                                                $"[c/F56469:��ʤ:] {plr.WinStreak}\n" +
                                                $"��ɱ/���� [c/5993DB:ʤ��ֵ: ]{plr.GetKillDeathRatio()}");
                }
                catch (NullReferenceException)
                {
                    args.Player.SendErrorMessage("���δ�ҵ���");
                }
            }
            else
            {
                try
                {
                    string name = string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                    List<TShockAPI.DB.UserAccount> matchedAccounts = TShock.UserAccounts.GetUserAccountsByName(name);

                    if (matchedAccounts.Count == 0)
                    {
                        args.Player.SendErrorMessage("���δ�ҵ���");
                        return;
                    }

                    DPlayer plr = PvPer.DbManager.GetDPlayer(matchedAccounts[0].ID);
                    args.Player.SendInfoMessage("[c/FFCB80:����ս��:]\n" +
                                                $"[c/63DC5A:��ɱ: ]{plr.Kills}\n" +
                                                $"[c/F56469:����:] {plr.Deaths}\n" +
                                                $"[c/F56469:��ʤ:] {plr.WinStreak}\n" +
                                                $"��ɱ/���� [c/5993DB:ʤ��ֵ: ]{plr.GetKillDeathRatio()}");
                }
                catch (NullReferenceException)
                {
                    args.Player.SendErrorMessage("���δ�ҵ���");
                }
            }
        }

        private static void LeaderboardCommand(CommandArgs args)
        {
            Task.Run(() =>
            {
                string message = "";
                List<DPlayer> list = PvPer.DbManager.GetAllDPlayers();

                list.Sort((p1, p2) =>
                {
                    if (p1.GetKillDeathRatio() >= p2.GetKillDeathRatio())
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                });

                DPlayer p;

                if (list.TryGetValue(0, out p))
                {
                    message += $"{1}. {TShock.UserAccounts.GetUserAccountByID(p.AccountID).Name} : {p.GetKillDeathRatio():F2}";
                }

                for (int i = 1; i < 5; i++)
                {
                    if (list.TryGetValue(i, out p))
                    {
                        message += $"\n{i + 1}. {TShock.UserAccounts.GetUserAccountByID(p.AccountID).Name} : {p.GetKillDeathRatio():F2}";
                    }
                }
                args.Player.SendInfoMessage(message);
            });
        }
    }
}
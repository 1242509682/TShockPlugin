using Steamworks;
using TShockAPI;

namespace PvPer
{
    public class Commands
    {
        public static void Duel(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage("����ϵͳָ��˵���\n " +
                "[c/74D3E8:/pvp add �����] - [c/7EE874:������ҲμӾ���] \n " +
                "[c/74D3E8:/pvp yes] - [c/7EE874:���ܾ���] \n " +
                "[c/74D3E8:/pvp no] - [c/7EE874:�ܾ�����] \n " +
                "[c/74D3E8:/pvp data] - [c/7EE874:ս����ѯ]\n " +
                "[c/FFFE80:/pvp list] - [c/7EE874:����]\n ");
                return;
            }

            switch (args.Parameters[0].ToLower())
            {
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
                    return;
                case "yes":
                case "����":
                    AcceptCmd(args);
                    return;
                case "no":
                case "�ܾ�":
                    RejectCommand(args);
                    return;
                case "data":
                case "ս��":
                    StatsCommand(args);
                    return;
                case "l":
                case "list":
                case "����":
                    LeaderboardCommand(args);
                    return;
                default:
                    args.Player.SendErrorMessage("����ϵͳָ��˵���\n " +
                    "[c/74D3E8:/pvp add �����] - [c/7EE874:������ҲμӾ���] \n " +
                    "[c/74D3E8:/pvp yes] - [c/7EE874:���ܾ���] \n " +
                    "[c/74D3E8:/pvp no] - [c/7EE874:�ܾ�����] \n " +
                    "[c/74D3E8:/pvp data] - [c/7EE874:ս����ѯ]\n " +
                    "[c/FFFE80:/pvp list] - [c/7EE874:����]\n ");
                    return;
            }
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
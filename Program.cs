using System;

public class Client
{
    public static void Main()
    {
        //サーバーに送信するデータを入力してもらう→何もなければ終了。
        Console.WriteLine("文字列を入力し、Enterキーを押してください。");
        string sendMsg = Console.ReadLine();
        if (sendMsg == null || sendMsg.Length == 0)
        {
            return;
        }

        //サーバーのIPアドレスとポート？番号？
        string ipOrHost = "172.20.10.3";//←ここ
        int port = 9999;

        //TcpClientを作成し、サーバーと接続する（わからん）
        System.Net.Sockets.TcpClient tcp =
            new System.Net.Sockets.TcpClient(ipOrHost, port);
        Console.WriteLine("サーバー({0}:{1})と接続しました({2}:{3})。",
            ((System.Net.IPEndPoint)tcp.Client.RemoteEndPoint).Address,
            ((System.Net.IPEndPoint)tcp.Client.RemoteEndPoint).Port,
            ((System.Net.IPEndPoint)tcp.Client.LocalEndPoint).Address,
            ((System.Net.IPEndPoint)tcp.Client.LocalEndPoint).Port);

        //NetworkStreamを取得する
        System.Net.Sockets.NetworkStream ns = tcp.GetStream();

        //読み取り、書き込みのタイムアウトを 秒にする。デフォルトはInfiniteで、タイムアウトしない
        ns.ReadTimeout = 30000;
        ns.WriteTimeout = 30000;

        //サーバーにデータを送信する,バイト型？
        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        byte[] sendBytes = enc.GetBytes(sendMsg + '\n');
        //データを送信する
        ns.Write(sendBytes, 0, sendBytes.Length);
        Console.WriteLine(sendMsg);

        //サーバーから送られたデータを受信する
        System.IO.MemoryStream ms = new System.IO.MemoryStream();
        byte[] resBytes = new byte[256];
        int resSize = 0;
        do
        {
            //データを受信する、0だと切断してる判断。
            resSize = ns.Read(resBytes, 0, resBytes.Length);
            if (resSize == 0)
            {
                Console.WriteLine("サーバーが切断しました。");
                break;
            }
            //受信したデータを蓄積する
            ms.Write(resBytes, 0, resSize);
            //まだ読み取れるデータがあるか、データの最後が\nでない時は、
            // 受信を続けて、文字列を変換（）
        } while (ns.DataAvailable || resBytes[resSize - 1] != '\n');
        string resMsg = enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);
        ms.Close();

        //閉じる
        ns.Close();
        tcp.Close();
        Console.WriteLine("切断しました。");

        Console.ReadLine();
    }
}

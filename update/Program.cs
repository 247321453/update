/*
 * 由SharpDevelop创建。
 * 用户： Acer
 * 日期: 2014-9-30
 * 时间: 15:31
 * 
 */
using System;

namespace update
{
	class Program
	{
		public static void Main(string[] args)
		{
			Console.Title="自动更新";
			Config.Init(null);

			if(args.Length>0){
				switch(args[0]){
					case "-m":UpdateList(args);break;
				}
			}else{
				Download();
			}
			Console.WriteLine("按任意键继续 . . . ");
			Console.ReadKey(true);
		}
		private static void UpdateList(string[] args){
			if(args.Length>=2){
				Config.setWorkPath(args[1]);
			}
			Server server=new Server();
			server.Run();//更新文件列表
		}
		private static void Download(){
			//线程数
			MyHttp.init(Config.ThreadNum);
			Client client=new Client();
			MyHttp.SetListner(client);
			client.Run();//开始更新
		}
	}
}
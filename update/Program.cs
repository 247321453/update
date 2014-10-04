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
			if(args.Length>0){
				switch(args[0]){
					case "-m":
						Server server=new Server(args);
						server.Run();
						break;
				}
				return;
			}else{
				Client client=new Client(args);
				client.Run();
			}
			Console.Write("按任意键继续 . . . ");
			Console.ReadKey(true);
		}
	}
}
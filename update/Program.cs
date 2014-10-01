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
			Client client=new Client(args);
			Server server=new Server(args);
			
			if(args.Length>0){
				switch(args[0]){
						case "-m":server.Run();break;
				}
				return;
			}else{
				client.Run();
			}
			Console.Write("按任意键继续 . . . ");
			Console.ReadKey(true);
		}
	}
}
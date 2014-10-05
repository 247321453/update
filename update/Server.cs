/*
 * 由SharpDevelop创建。
 * 用户： Acer
 * 日期: 2014-9-30
 * 时间: 15:45
 * 
 */
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace update
{
	/// <summary>
	/// Description of Server.
	/// </summary>
	public class Server
	{
		List<fileinfo> list;	//文件信息列表
		
		public Server(){
			list=new List<fileinfo>();
		}

		public void Run(){
			Console.WriteLine("开始更新文件列表。。。");
			list.Clear();
			AddDir(Config.workPath);//当前目录所有文件
			//版本
			MyUtil.saveText(Config.versionFile, DateTime.Now.ToString());
			//重命名列表
			MyUtil.saveText(Config.renameFile,"# 重命名列表 (编码为UTF-8，用tab键隔开，采用相对路径)"
			                +Environment.NewLine
			                +"# 例如：前面改名为后面。"
			                +Environment.NewLine
			                +"# pics/123456.jpg	pics/456789.jpg");
			//删除列表
			MyUtil.saveText(Config.deleteFile,"# 删除列表 (编码为UTF-8，采用相对路径)");
			//文件列表
			MyUtil.saveList(Config.filelistFile, list.ToArray());//文件列表
			Console.WriteLine("文件列表更新完成。。。");
		}
		void AddDir(string dir){
			//所有文件
			string[] files=Directory.GetFiles(dir);
			foreach(string file in files){
				AddFile(file);
			}
			//获取所有子目录
			string[] dirs=Directory.GetDirectories(dir);
			
			foreach(string d in dirs){
				if(!d.EndsWith(Path.DirectorySeparatorChar+".git"
				               ,StringComparison.OrdinalIgnoreCase))
					AddDir(d);//添加子目录的所有文件
			}
		}
		void AddFile(string file){
			if(file.EndsWith("Thumbs.db",StringComparison.OrdinalIgnoreCase)
			   || file.EndsWith(".gitignore",StringComparison.OrdinalIgnoreCase)
			  )
				return;
			//处理名字
			string name=file.Replace(Config.workPath,"");
			name=name.Replace(Path.DirectorySeparatorChar,'/');
			
			if(name.IndexOf('/')==0)
				name=name.Substring(1);
			
			if(MyUtil.checkList(Config.ignores, name)){
				return;
			}
			string md5=MyUtil.MD5_File(file);
			Console.WriteLine("文件:	"+name);
			Console.WriteLine("MD5:	"+md5);
			list.Add(new fileinfo(name, md5));
		}
	}
}

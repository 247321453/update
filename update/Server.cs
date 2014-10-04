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
		string _wPath,_uPath;
		List<fileinfo> list;
		string f_version="version.txt";
		string f_delete="delete.txt";
		string f_rename="rename.txt";
		string f_filelist="filelist.txt";
		
		public Server(string[] args){
			if(args == null || args.Length < 2 ){
				_wPath=AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
				DirectoryInfo dirinfo=new DirectoryInfo(_wPath);
				string tmp=(dirinfo.Parent!=null)?dirinfo.Parent.FullName:"";
				if(!string.IsNullOrEmpty(tmp))
					_wPath=tmp;//上一级目录
			}
			else {
				_wPath=args[1];
			}
			InitPath();
		}
		
		void InitPath(){
			_uPath=Path.Combine(_wPath,"update");
			f_version=Path.Combine(_uPath,f_version);
			f_delete=Path.Combine(_uPath,f_delete);
			f_rename=Path.Combine(_uPath,f_rename);
			f_filelist=Path.Combine(_uPath,f_filelist);
		}
		
		public void Run(){
			Console.WriteLine("更新开始。。。");
			list=new List<fileinfo>();
			AddDir(_wPath);//当前目录所有文件
			MyUtil.saveText(f_version,DateTime.UtcNow.ToShortDateString());
			MyUtil.saveText(f_rename,"# 重命名列表 (编码为UTF-8，用tab键隔开，采用相对路径)"
			               +Environment.NewLine
			               +"# 例如：前面改名为后面。"
			               +Environment.NewLine
			               +"# pics/123456.jpg	pics/456789.jpg");
			MyUtil.saveText(f_delete,"#删除列表 (编码为UTF-8，采用相对路径)");
			MyUtil.saveList(f_filelist,list.ToArray());//文件列表
			Console.WriteLine("更新完成。。。");
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
				if(!d.EndsWith(Path.DirectorySeparatorChar+".git",StringComparison.OrdinalIgnoreCase))
					AddDir(d);//添加子目录的所有文件
			}
		}
		void AddFile(string file){
			if(file == Assembly.GetExecutingAssembly().Location
			  || file == f_version ||file == f_delete 
			  || file == f_rename ||file == f_filelist
			  || file == Assembly.GetExecutingAssembly().Location+".config"
			  || file == Assembly.GetExecutingAssembly().Location+".bat"
			  || "Thumbs.db".Equals(Path.GetFileName(file)
			                        ,StringComparison.OrdinalIgnoreCase)
			  || file.EndsWith(".gitignore",StringComparison.OrdinalIgnoreCase)
			  )
				return;
			string name=file.Replace(_wPath,"");
			string md5=MyUtil.MD5_File(file);
			name=name.Replace(Path.DirectorySeparatorChar,'/');
			
			if(name.IndexOf('/')==0)
				name=name.Substring(1);
			
			Console.WriteLine(name+"	"+md5);
			list.Add(new fileinfo(name, md5));
		}
	}
}

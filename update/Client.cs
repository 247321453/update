/*
 * 由SharpDevelop创建。
 * 用户： Acer
 * 日期: 2014-9-30
 * 时间: 15:45
 * 
 */
using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace update
{
	/// <summary>
	/// Description of Download.
	/// </summary>
	public class Client : MyHttpListener
	{
		int all_num,num;
		List<fileinfo> errorlist;
		
		public Client(){
			Config.setWorkPath(ConfigurationManager.AppSettings["path"]);
			
			errorlist=new List<fileinfo>();

			//代理设置
			if(Config.useProxy){
				Console.WriteLine("使用代理："+Config.proxyIP+":"+Config.proxyPort);
				MyHttp.setProxy(true, Config.proxyIP, Config.proxyPort);
			}
			else{
				MyHttp.setProxy(false, "127.0.0.1",80);
			}
		}
		
		void Delete(){
			if(!MyHttp.DownLoad(Config.url_delete, Config.deleteFile)){
				return;
			}
			string[] lines=File.ReadAllLines(Config.deleteFile, Encoding.UTF8);
			foreach(string line in lines){
				if(!line.StartsWith("#")){
					string file=Config.GetPath(line);
					if(File.Exists(file)){
						Console.WriteLine("删除文件："+line);
						File.Delete(file);
					}
				}
			}
		}
		
		void Rename(){
			if(!MyHttp.DownLoad(Config.url_rename, Config.renameFile)){
				return;
			}
			string[] lines=File.ReadAllLines(Config.renameFile, Encoding.UTF8);
			foreach(string line in lines){
				if(!line.StartsWith("#")){
					string[] files=line.Split('\t');
					if(files.Length>=2){
						string file1=Config.GetPath(files[0]);
						string file2=Config.GetPath(files[1]);
						Console.WriteLine("重命名："+files[0]+"=>"+files[1]);
						File.Move(file1,file2);
					}
				}
			}
		}
		
		public void OnStart(string name,string file){
			//Console.WriteLine("开始下载："+name);
			//Console.WriteLine("保存到："+file);
		}
		
		public void OnEnd(fileinfo ff,bool isOK){
			showProcess(num++,all_num);
			if(!isOK){
				if(ff!=null){
					Console.WriteLine("下载失败:"+Config.GetUrl(ff.name));
					errorlist.Add(ff);
				}else{
					Console.WriteLine("下载失败");
				}
			}else{
				if(ff!=null){
					Console.WriteLine("下载完成:"+ff.name);
				}
			}
		}
		
		void showProcess(int i,int all){
			Console.Title=string.Format("进度：{0}/{1}",i,all);
		}
		
		bool Download(string name,string md5,bool isHide){
			string file=Config.GetPath(name);
			
			if(File.Exists(file)){
				if(md5==MyUtil.MD5_File(file)){//一致
					Console.WriteLine("无须下载："+name);
					return true;
				}
				else{
					if(MyUtil.checkList(Config.ignores,name)){//忽略更新
						Console.WriteLine("忽略更新："+name);
						return true;
					}
				}
			}
			//线程已满
			while(MyHttp.NUM>=MyHttp.MAX_NUM){
				//System.Threading.Thread.Sleep(100);
			}
			//下载文件
			new MyHttp(Config.GetUrl(name), file, new fileinfo(name,md5)).Start();
			return true;
			//return MyHttp.DownLoad(url_download+name,file);
		}
		
		void Update(){
			if(!File.Exists(Config.errorFile)){//上一次下载是否失败
				Console.WriteLine("下载文件列表。。。");
				if(!MyHttp.DownLoad(Config.url_filelist, Config.filelistFile))
					return;
				Console.WriteLine("开始更新。。。");
			}else{
				File.Delete(Config.filelistFile);
				File.Move(Config.errorFile, Config.filelistFile);
				Console.WriteLine("继续上次更新。。。");
			}

			string[] lines=File.ReadAllLines(Config.filelistFile, Encoding.UTF8);
			all_num=lines.Length;
			num=0;
			showProcess(num++,all_num);
			foreach(string line in lines){
				if(!line.StartsWith("#")){
					string[] words=line.Split('\t');
					if(words.Length>=2){
						Download(words[0], words[1],false);
					}
				}
			}
			while(!MyHttp.isOK()){

			}
			if(errorlist.Count>0){
				Console.WriteLine("部分文件下载失败。。。");
				MyUtil.saveList(Config.errorFile, errorlist.ToArray());
			}
		}
		void ShowTask(int n){
			if(n==0)
				return;
			Console.WriteLine(string.Format("还有个{0}文件还在下载。。。", n));
		}
		
		public void Run(){
			Console.WriteLine("更新地址："+Config.url_home);
			Console.WriteLine("下载保存在："+Config.workPath);
			Console.WriteLine("设置文件："+Assembly.GetExecutingAssembly().Location+".config");

			if(!File.Exists(Config.errorFile)){
				Console.WriteLine("获取新版本。。。");
				//version
				MyHttp.DownLoad(Config.url_version, Config.newVersionFile);
				//版本号一致
				string md5_1=MyUtil.MD5_File(Config.versionFile);
				string md5_2=MyUtil.MD5_File(Config.newVersionFile);
				if(md5_1 == md5_2 && md5_1.Length>0){
					Console.WriteLine("已经是最新。");
					return;
				}
				Console.WriteLine("发现新版本。。。");
				//删除旧文件
				Delete();
				//重命名文件
				Rename();
			}
			Console.Clear();
			//filelist
			Update();
			if(File.Exists(Config.newVersionFile)){
				File.Delete(Config.versionFile);
				File.Move(Config.newVersionFile, Config.versionFile);
			}
			Console.WriteLine("更新完成，可以关闭本程序。。。");
		}
	}
}

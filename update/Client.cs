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

namespace update
{
	/// <summary>
	/// Description of Download.
	/// </summary>
	public class Client
	{
		string _url;
		string _wPath,_nPath,_uPath;
		string f_version_new="version_new.txt";
		string f_version="version.txt";
		string f_delete="delete.txt";
		string f_rename="rename.txt";
		string f_filelist="filelist.txt";
		string f_error="error.txt";
		
		string url_version;
		string url_delete;
		string url_filelist;
		string url_rename;
		
		string _path="update/";
		
		string[] ignores;
		public Client(string[] args){
			string tmp;
			_nPath=AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
			if(args.Length == 3){//downloa url savepath
				_url=args[1];
				_wPath=args[2];
			}else if(args.Length == 2){//download url
				_url=args[1];
				Defaultpath();
			}else{
				_url=ConfigurationManager.AppSettings["url"];
				_wPath=ConfigurationManager.AppSettings["path"];
				if(string.IsNullOrEmpty(_wPath))
					Defaultpath();
			}
			//路径
			InitPath();
			
			//忽略列表
			List<string> iglist=new List<string>();
			int i=1;
			tmp=ConfigurationManager.AppSettings["ignore"+i];
			
			while(tmp!=null && tmp.Length!=0){
				tmp=tmp.Replace("*","[^/]*?");
				iglist.Add(tmp);
				i++;
				tmp=ConfigurationManager.AppSettings["ignore"+i];
			}
			
			ignores=iglist.ToArray();
			
			//代理设置
			bool IsProxy=("true".Equals(ConfigurationManager.AppSettings["useproxy"],
			                            StringComparison.OrdinalIgnoreCase))?true:false;
			if(IsProxy){
				string ip=ConfigurationManager.AppSettings["proxy"];
				Console.WriteLine("使用代理："+ip);
				MyHttp.setProxy(true, ip);
			}
			else{
				MyHttp.setProxy(false,"127.0.0.1:80");
			}
		}
		void Defaultpath(){
			DirectoryInfo dirinfo=new DirectoryInfo(_nPath);
			string tmp=dirinfo.Parent.FullName;
			
			if(!string.IsNullOrEmpty(tmp))
				_wPath=tmp;//上一级目录
			else
				_wPath=_nPath;
		}
		void InitPath(){
			
			url_version=_url+_path+f_version;
			url_delete=_url+_path+f_delete;
			url_filelist=_url+_path+f_filelist;
			url_rename=_url+_path+f_rename;
			
			_uPath=Path.Combine(_wPath,_path);

			if(!Directory.Exists(_uPath)){
				Directory.CreateDirectory(_uPath);
			}
			f_error=Path.Combine(_uPath,f_error);
			f_version=Path.Combine(_uPath,f_version);
			f_delete=Path.Combine(_uPath,f_delete);
			f_rename=Path.Combine(_uPath,f_rename);
			f_filelist=Path.Combine(_uPath,f_filelist);
			f_version_new=Path.Combine(_uPath,f_version_new);
		}
		
		void Delete(){
			Console.WriteLine("删除无用文件。。。");
			MyHttp.DownLoad(url_delete,f_delete);
			if(!File.Exists(f_delete)){
				Console.WriteLine("下载失败:"+url_delete);
				return;
			}
			string[] lines=File.ReadAllLines(f_delete, Encoding.UTF8);
			foreach(string line in lines){
				if(!line.StartsWith("#")){
					string file=MyUtil.GetPath(_wPath,line);
					if(File.Exists(file))
						File.Delete(file);
				}
			}
		}
		
		void Rename(){
			Console.WriteLine("重命名文件。。。");
			MyHttp.DownLoad(url_rename,f_rename);
			if(!File.Exists(f_rename)){
				Console.WriteLine("下载失败:"+url_rename);
				return;
			}
			string[] lines=File.ReadAllLines(f_rename, Encoding.UTF8);
			foreach(string line in lines){
				if(!line.StartsWith("#")){
					string[] files=line.Split('\t');
					if(files.Length>=2){
						string file1=MyUtil.GetPath(_wPath,files[0]);
						string file2=MyUtil.GetPath(_wPath,files[1]);
						File.Move(file1,file2);
					}
				}
			}
		}
		
		bool checkignore(string str){
			if(ignores==null)
				return false;
			foreach(string tmp in ignores){
				if(Regex.IsMatch(str,"^"+tmp+"$",RegexOptions.IgnoreCase))
					return true;
			}
			return false;
		}
		bool Download(string name,string md5,bool isHide){
			string file=MyUtil.GetPath(_wPath,name);
			
			if(File.Exists(file)){
				if(md5==MyUtil.MD5_File(file)){//一致
					Console.WriteLine("无须下载："+name);
					return true;
				}
				else{
					if(checkignore(name)){//忽略更新
						Console.WriteLine("忽略更新："+name);
						return true;
					}
				}
			}//下载文件
			while(MyHttp.NUM>=MyHttp.MAX_NUM){
				System.Threading.Thread.Sleep(100);
			}
			Console.WriteLine("正在下载："+name);
			new MyHttp(_url+name,file,new fileinfo(name,md5)).Start();
			return true;
			//return MyHttp.DownLoad(url_download+name,file);
		}
		
		void Update(){
			if(!File.Exists(f_error)){//上一次下载是否失败
				MyHttp.DownLoad(url_filelist,f_filelist);
				Console.WriteLine("开始更新。。。");
			}else{
				File.Delete(f_filelist);
				File.Move(f_error, f_filelist);
				Console.WriteLine("继续上次。。。");
			}
			if(!File.Exists(f_filelist)){
				Console.WriteLine("下载失败:"+url_filelist);
				return;
			}
			string[] lines=File.ReadAllLines(f_filelist, Encoding.UTF8);
			foreach(string line in lines){
				if(!line.StartsWith("#")){
					string[] files=line.Split('\t');
					if(files.Length>=2){
						Download(files[0],files[1],false);
					}
				}
			}
			while(!MyHttp.isOK()){
				Console.WriteLine("等待所有文件下载完成。。。");
			}
			if(MyHttp.errorlist.Count>0){
				Console.WriteLine("部分文件下载失败。。。");
				MyUtil.saveList(f_error,MyHttp.errorlist.ToArray());
			}
		}
		
		public void Run(){
			Console.WriteLine("游戏路径为："+_wPath);
			Console.WriteLine("开始更新。。。");
			//version
			MyHttp.DownLoad(url_version, f_version_new);
			//版本号一致
			string md5_1=MyUtil.MD5_File(f_version);
			string md5_2=MyUtil.MD5_File(f_version_new);
			if(md5_1 == md5_2 && md5_1.Length>0){
				Console.WriteLine("已经是最新。");
				return;
			}
			//删除旧文件
			Delete();
			//重命名文件
			Rename();
			
			//filelist
			Update();
			if(File.Exists(f_version_new)){
				File.Delete(f_version);
				File.Move(f_version_new,f_version);
			}
			Console.WriteLine("更新完成。。。");
		}
	}
}

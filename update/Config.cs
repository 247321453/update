/*
 * 由SharpDevelop创建。
 * 用户： Acer
 * 日期: 2014-10-5
 * 时间: 8:30
 * 
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;

namespace update
{
	/// <summary>
	/// Description of Config.
	/// </summary>
	public class Config
	{
		/// <summary>下载线程数</summary>
		public static int ThreadNum=0x10;
		/// <summary>工作目录</summary>
		public static string workPath;
		/// <summary>信息目录</summary>
		public static string infoPath;
		/// <summary>版本</summary>
		public static string versionFile;
		/// <summary>新版本</summary>
		public static string newVersionFile;
		/// <summary>删除列表</summary>
		public static string deleteFile;
		/// <summary>重命名列表</summary>
		public static string renameFile;
		/// <summary>文件列表</summary>
		public static string filelistFile;
		/// <summary>错误列表</summary>
		public static string errorFile;
		
		/// <summary>下载网址</summary>
		public static string url_home="http://localhost/";
		/// <summary>版本下载网址</summary>
		public static string url_version;
		/// <summary>删除下载网址</summary>
		public static string url_delete;
		/// <summary>文件列表下载网址</summary>
		public static string url_filelist;
		/// <summary>重命名下载网址</summary>
		public static string url_rename;
		
		/// <summary>使用代理</summary>
		public static bool useProxy=false;
		/// <summary>代理IP</summary>
		public static string proxyIP="127.0.0.1";
		/// <summary>代理端口</summary>
		public static int proxyPort=80;
		
		public static string[] ignores;
		
		public static string GetUrl(string name){
			return url_home+name;
		}
		public static string GetPath(string name){
			return Path.Combine(workPath, name.Replace('/',Path.DirectorySeparatorChar));
		}
		public static void setWorkPath(string workpath,string url){
			string tmp;
			if(string.IsNullOrEmpty(workpath)){
				//当前目录
				workPath=AppDomain.CurrentDomain.
					SetupInformation.ApplicationBase;
				
				DirectoryInfo dirinfo=new DirectoryInfo(workPath);
				tmp=(dirinfo.Parent!=null)?dirinfo.Parent.FullName:"";
				if(!string.IsNullOrEmpty(tmp))//上一级目录不为空
					workPath=tmp;
			}
			else
				workPath=workpath;
			if(string.IsNullOrEmpty(url))
				url_home = ConfigurationManager.AppSettings["url"];
			else
				url_home=url;
			url_version = url_home+"update/version.txt";
			url_delete = url_home+"update/delete.txt";
			url_filelist = url_home+"update/filelist.txt";
			url_rename = url_home+"update/rename.txt";
			
			infoPath=Path.Combine(workPath, "update");
			if(!Directory.Exists(infoPath)){
				Directory.CreateDirectory(infoPath);
			}
			versionFile=Path.Combine(infoPath, "version.txt");
			newVersionFile = Path.Combine(infoPath, "version_new.txt");
			deleteFile=Path.Combine(infoPath, "delete.txt");
			renameFile=Path.Combine(infoPath, "rename.txt");
			filelistFile=Path.Combine(infoPath, "filelist.txt");
			errorFile=Path.Combine(infoPath, "error.txt");
		}
		public static void Init(string workpath,string url){
			string tmp;
			Config.setWorkPath(workpath,url);
			
			//忽略列表
			List<string> iglist=new List<string>();
			int i=1;
			tmp=ConfigurationManager.AppSettings["ignore"+i];
			
			while(!string.IsNullOrEmpty(tmp)){
				tmp=tmp.Replace("*","[^/]*?");
				iglist.Add(tmp);
				i++;
				tmp=ConfigurationManager.AppSettings["ignore"+i];
			}
			
			ignores=iglist.ToArray();
			
			
			//代理设置
			bool useProxy=("true".Equals(ConfigurationManager.AppSettings["useproxy"],
			                             StringComparison.OrdinalIgnoreCase))?true:false;
			if(useProxy){
				string ip=ConfigurationManager.AppSettings["proxy"];
				string[] strs=ip.Split(':');
				proxyIP=strs[0];
				int.TryParse(strs[1],out proxyPort);
			}
		}
		
		
	}
}

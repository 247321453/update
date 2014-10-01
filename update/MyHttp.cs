/*
 * 由SharpDevelop创建。
 * 用户： Acer
 * 日期: 2014-9-25
 * 时间: 21:33
 * 
 */
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Collections.Generic;

namespace update
{
	/// <summary>
	/// Description of MyHttp.
	/// </summary>
	public class MyHttp
	{
		public static int NUM=0;
		public static int MAX_NUM=0x20;
		private string _url,_filename;
		private static int NTASK=0;
		private static int TASK=0;
		private fileinfo _ff;
		private static bool isProxy=false;
		private static string proxyip;
		private static int proxyport;
		public static List<fileinfo> errorlist=new List<fileinfo>();
		public MyHttp(string url, string filename, fileinfo ff){
			this._url=url;
			this._filename=filename;
			this._ff=ff;
		}
		public static void setProxy(bool isuse,string ip){
			isProxy=isuse;
			string[] strs=ip.Split(':');
			proxyip=strs[0];
			int.TryParse(strs[1],out proxyport);
		}
		public void Start(){
			Thread thread=new Thread(Download);
			thread.IsBackground=true;
			thread.Start();
		}
		public void Download(){
			if(MyHttp.NUM>=MyHttp.MAX_NUM)
				return;
			NUM++;
			TASK++;
			if(!MyHttp.DownLoad(_url,_filename)){
				//下载失败
				errorlist.Add(_ff);
			}
			NTASK++;
			NUM--;
		}
		public static bool isOK(){
			return (NTASK==TASK);
		}

		
		public static bool DownLoad(string url,string filename)
		{
			bool isOK=false;
			try
			{
				if(File.Exists(filename))
					File.Delete(filename);
				else
					MyUtil.createDir(filename);
				HttpWebRequest Myrq = (HttpWebRequest)System.Net.HttpWebRequest.Create(url);
				Myrq.Timeout = 30000;
				//Myrq.UserAgent="Mozilla/5.0 (Windows NT 6.2; WOW64) "
				//	+"AppleWebKit/537.36 (KHTML, like Gecko) "
				//	+"Chrome/27.0.1453.94 Safari/537.36";
				if(MyHttp.isProxy){
					Myrq.Proxy = new WebProxy(MyHttp.proxyip, MyHttp.proxyport);
				}
				
				HttpWebResponse myrp = (HttpWebResponse)Myrq.GetResponse();
				long totalBytes = myrp.ContentLength;
				
				Stream st = myrp.GetResponseStream();
				Stream so = new System.IO.FileStream(filename+".tmp", FileMode.Create);
				long totalDownloadedByte = 0;
				byte[] by = new byte[2048];
				int osize = st.Read(by, 0, (int)by.Length);
				while (osize > 0)
				{
					totalDownloadedByte = osize + totalDownloadedByte;
					so.Write(by, 0, osize);
					osize = st.Read(by, 0, (int)by.Length);
				}
				so.Close();
				st.Close();
				File.Delete(filename);
				File.Move(filename+".tmp", filename);
			}
			catch (System.Exception)
			{
				isOK= false;
			}
			isOK=true;
			return isOK;
		}
		
	}
	
}

/*
 * 由SharpDevelop创建。
 * 用户： Acer
 * 日期: 2014-9-30
 * 时间: 15:34
 * 
 */
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace update
{
	/// <summary>
	/// Description of MyUtil.
	/// </summary>
	public class MyUtil
	{
		#region 获取网址内容
		public static string GetHtmlContentByUrl(string url)
		{
			string htmlContent = string.Empty;
			try {
				HttpWebRequest httpWebRequest =
					(HttpWebRequest)WebRequest.Create(url);
				httpWebRequest.Timeout = 30000;
				httpWebRequest.UserAgent="Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.94 Safari/537.36";
				using(HttpWebResponse httpWebResponse =
				      (HttpWebResponse)httpWebRequest.GetResponse())
				{
					using(Stream stream = httpWebResponse.GetResponseStream())
					{
						using(StreamReader streamReader =
						      new StreamReader(stream, Encoding.UTF8))
						{
							htmlContent = streamReader.ReadToEnd();
							streamReader.Close();
						}
						stream.Close();
					}
					httpWebResponse.Close();
				}
				return htmlContent;
			}
			catch{
				
			}
			return "";
		}
		#endregion
		
		#region MD5校验
		/// <summary>
		/// 计算文件的MD5校验
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static string MD5_File(string fileName)
		{
			if(!File.Exists(fileName))
				return "";
			long filesize=0;
			try
			{
				FileStream file = new FileStream(fileName, FileMode.Open);
				filesize=file.Length;
				System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
				byte[] retVal = md5.ComputeHash(file);
				file.Close();

				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < retVal.Length; i++)
				{
					sb.Append(retVal[i].ToString("x2"));
				}
				return sb.ToString();
			}
			catch //(Exception ex)
			{
				//throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
			}
			return filesize.ToString();
		}
		#endregion
		
		public static void createDir(string filename){
			int index=filename.LastIndexOf(Path.DirectorySeparatorChar);
			string path=filename.Substring(0,index);
			if(!Directory.Exists(path)){
				Directory.CreateDirectory(path);
			}
		}
		public static bool checkList(string[] iglist,string str){
			if(iglist == null)
				return false;
			foreach(string tmp in iglist){
				if(Regex.IsMatch(str,"^"+tmp+"$",RegexOptions.IgnoreCase))
					return true;
			}
			return false;
		}
		public static void saveText(string file,string str){
			if(File.Exists(file))
				File.Delete(file);
			else
				MyUtil.createDir(file);
			File.WriteAllText(file,str,Encoding.UTF8);
		}

		public static void saveList(string file,fileinfo[] fileinfos){
			if(File.Exists(file))
				File.Delete(file);
			else
				MyUtil.createDir(file);
			using(FileStream fs=new FileStream(file,FileMode.Create,FileAccess.Write)){
				StreamWriter sw=new StreamWriter(fs,Encoding.UTF8);
				if(fileinfos!=null)
				{
					foreach(fileinfo ff in fileinfos){
						sw.WriteLine(ff.name+"\t"+ff.md5);
					}
				}
				sw.Close();
			}
		}
		
		public static fileinfo[] readList(string file){
			List<fileinfo> list=new List<fileinfo>();
			if(File.Exists(file))
			{
				using(FileStream fs=new FileStream(file,FileMode.Open,FileAccess.Read)){
					StreamReader sr=new StreamReader(fs,Encoding.UTF8);
					string line;
					while((line =sr.ReadLine())!=null){
						if(!line.StartsWith("#")){
							string[] w=line.Split('\t');
							if(w.Length>=2)
								list.Add(new fileinfo(w[0],w[1]));
						}
					}
					sr.Close();
				}
			}
			return list.ToArray();
		}
	}
}

/*
 * 由SharpDevelop创建。
 * 用户： Acer
 * 日期: 2014-9-29
 * 时间: 12:28
 * 
 */
using System;

namespace update
{
	/// <summary>
	/// Description of fileinfo.
	/// </summary>
	public class fileinfo
	{
		public fileinfo(string name,string md5){
			this.name=name;
			this.md5=md5;
		}
		public fileinfo(){
			name="";
			md5="";
		}
		public string name;
		public string md5;
	}

}

/*
 * 由SharpDevelop创建。
 * 用户： Acer
 * 日期: 2014-10-5
 * 时间: 8:04
 * 
 */
using System;

namespace update
{
	/// <summary>
	/// Description of MyHttpListener.
	/// </summary>
	public interface MyHttpListener
	{
		void OnStart(string name,string file);
		void OnEnd(fileinfo ff,bool isOK);
	}
}

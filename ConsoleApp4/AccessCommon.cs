using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.OleDb;
using System.Configuration;
using System.Data;

namespace ConsoleApp4
{
    /// <summary>
    /// Access数据库操作公共类
    /// </summary>
    public class AccessCommon
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        private OleDbConnection Conn;
        /// <summary>
        /// 数据库文件
        /// </summary>
        private string FilePath;

        public AccessCommon(String filePath)
        {
            this.FilePath = filePath;
        }

        private OleDbConnection CreateConn()
        {
            if (Conn == null)
            {
                string connStr = GetConnStr(FilePath);
                Conn = new OleDbConnection(connStr);
            }
            if (Conn.State == System.Data.ConnectionState.Closed)
            {
                Conn.Open();
            }
            return Conn;
        }

        /// <summary>
        /// 执行命令，返回执行成功影响行数
        /// </summary>
        /// <param name="cmdText">sql命令</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string cmdText)
        {
            int cuCount = -1;
            try
            {
                CreateConn();
                OleDbCommand cmd = new OleDbCommand(cmdText, Conn);
                cuCount = cmd.ExecuteNonQuery();
            }
            finally
            {
                CloseConn();
            }
            return cuCount;
        }

        /// <summary>
        /// 执行命令，返回执行成功影响行数
        /// </summary>
        /// <param name="cmdText">sql命令</param>
        /// <paramref name="dbFile">db文件路径</paramref>
        /// <returns></returns>
        public int ExecuteNonQuery(string cmdText, string dbFile)
        {
            string connStr = GetConnStr(FilePath);
            OleDbConnection conn = null;
            try
            {
                conn = new OleDbConnection(connStr);
                conn.Open();
                OleDbCommand cmd = new OleDbCommand(cmdText, conn);
                var cuCount = cmd.ExecuteNonQuery();

                return cuCount;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    try
                    {
                        conn.Close();
                    }
                    finally
                    {
                        conn.Dispose();
                    }
                }

            }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public DataTable ExecuteCmd(string cmdText)
        {
            //链接数据库
            CreateConn();
            try
            {
                //适配器
                OleDbDataAdapter adp = new OleDbDataAdapter(cmdText, Conn);
                DataSet ds = new DataSet();
                ds.Clear();
                //适配器将数据填充到数据集
                adp.Fill(ds);
                return ds.Tables[0];
            }
            finally
            {
                CloseConn();
            }
        }

        /// <summary>
        /// 获取表数据
        /// </summary>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        public DataTable GetTableByName(string strTableName)
        {
            //链接数据库
            CreateConn();
            try
            {
                string cmdText = String.Format("select * from {0}", strTableName);
                //适配器
                OleDbDataAdapter adp = new OleDbDataAdapter(cmdText, Conn);
                DataSet ds = new DataSet();
                ds.Clear();
                //适配器将数据填充到数据集
                adp.Fill(ds);
                return ds.Tables[0];
            }
            finally
            {
                CloseConn();
            }
        }
        
        /// <summary>
        /// 得到所有表信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllTableName()
        {
            try
            {
                CreateConn();
                return Conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            }
            finally
            {
                CloseConn();
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseConn()
        {
            if (Conn == null) return;
            if (Conn.State == System.Data.ConnectionState.Open)
                Conn.Close();
            Conn.Dispose();
            Conn = null;
        }

        /// <summary>
        /// 拼接连接字符串
        /// </summary>
        /// <param name="filePath">mdb文件路径</param>
        /// <returns></returns>
        private string GetConnStr(string filePath)
        {
            string connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + this.FilePath;
            return connStr;
        }
    }
}
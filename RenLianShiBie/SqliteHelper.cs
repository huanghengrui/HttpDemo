using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;


namespace RenLianShiBie
{
    class SqliteHelper
    {
        private static string cString = "data source= Setting.data";
        private SqliteHelper() { }
        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(cString))
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    cString = ConfigurationSettings.AppSettings["SQLiteConnectionString"];
#pragma warning restore CS0618 // Type or member is obsolete
                }
                return cString;
            }
        }

        static public Boolean NewDbFile()
        {
            try
            {
                FileInfo fi = new FileInfo("Setting.data");
                if(!fi.Exists)
                    SQLiteConnection.CreateFile("Setting.data");
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("新建数据库文件" + cString + "失败：" + ex.Message);
            }
        }  

        public static int CreateTable(string commandText)
        {
            SQLiteConnection conn = new SQLiteConnection(ConnectionString);
            conn.Open();

            SQLiteCommand command = new SQLiteCommand();
            command.Connection = conn;
            command.CommandText = commandText;
            int val = command.ExecuteNonQuery();
            command.Dispose();
            conn.Close();
            conn.Dispose();

            return val;
        }


        // SQLiteParameter name = MySQLiteHelper.CreateParameter("name", DbType.String, "SUSAN LI");        
        // SQLiteParameter sex = MySQLiteHelper.CreateParameter("sex", DbType.Int16, 1);       
        // SQLiteParameter age = MySQLiteHelper.CreateParameter("age", DbType.Int16, 30);        
        // SQLiteParameter[] pa = new SQLiteParameter[3] { name, sex, age };       
        // MySQLiteHelper.ExecuteNonQuery("INSERT INTO USER6 (NAME,SEX,AGE) values (@name,@sex,@age)",pa);        
        public static int ExecuteNonQuery(string commandText, params SQLiteParameter[] commandParameters)
        {
            SQLiteConnection conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            SQLiteCommand command = new SQLiteCommand();
            command.Connection = conn;
            command.CommandText = commandText;
            if (commandParameters != null)
                command.Parameters.AddRange(commandParameters);
            int val = command.ExecuteNonQuery();
            command.Dispose();
            conn.Close();
            conn.Dispose();
            return val;
        }

        // SQLiteConnection conn = new SQLiteConnection(ConnectionString);        
        // conn.Open();        
        // SQLiteCommand command = new SQLiteCommand();        
        // command.Connection = conn;

        public static int ExecuteNonQuery(SQLiteCommand command, string commandText,params SQLiteParameter[] commandParameters)         
        {            
            if (command.Connection.State == ConnectionState.Closed)                
                command.Connection.Open();            
            command.CommandText = commandText;            
            command.Parameters.Clear();            
            if (commandParameters != null)                
                command.Parameters.AddRange(commandParameters);            
            return command.ExecuteNonQuery();        
        }         // 查询并返回datatable        
        
        public static DataTable ExecuteDataTable(string commandText, params SQLiteParameter[] commandParameters)         
        {            
            SQLiteConnection conn = new SQLiteConnection(ConnectionString);            
            conn.Open();             SQLiteCommand command = new SQLiteCommand();            
            command.Connection = conn;            
            command.CommandText = commandText;            
            if (commandParameters != null)                
                command.Parameters.AddRange(commandParameters);            
            // 开始读取            
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);            
            DataTable data = new DataTable();            
            adapter.Fill(data);            
            // dispose            
            adapter.Dispose();            
            command.Dispose();            
            conn.Close();            
            conn.Dispose();             
            return data;        
        }                
        // 创建参数        
        public static SQLiteParameter CreateParameter(string parameterName, System.Data.DbType parameterType, object parameterValue)        
        {            
            SQLiteParameter parameter = new SQLiteParameter();            
            parameter.DbType = parameterType;            
            parameter.ParameterName = parameterName;            
            parameter.Value = parameterValue;            
            return parameter;        
        }

        public static List<MachineInfo> QueryMachineList()
        {
            List<MachineInfo> info = new List<MachineInfo>();
            SQLiteConnection conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            SQLiteCommand command = new SQLiteCommand();
            command.Connection = conn;
            command.CommandText = "select * from MachineList";
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    MachineInfo ifs = new MachineInfo();
                    ifs.id =  reader.GetInt32(0);
                    ifs.name = reader.GetString(1);
                    ifs.ip = reader.GetString(2);
                    ifs.port = reader.GetInt32(3);
                    ifs.pwd = reader.GetString(4);
                    info.Add(ifs);
                }
            }
            reader.Close();
            reader.Dispose();
            command.Dispose();
            conn.Close();
            conn.Dispose();

            return info;
        }

        public static string ExecuteCommand(string commandStr)
        {
            SQLiteConnection conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            SQLiteCommand command = new SQLiteCommand();
            command.Connection = conn;
            command.CommandText = commandStr;
            //command.ExecuteScalar()
            string val = Convert.ToString(command.ExecuteScalar());
            command.Dispose();
            conn.Close();
            conn.Dispose();
            return val;    
        }

         public static int IsTableExists(string tableName)         
         {            
             SQLiteConnection conn = new SQLiteConnection(ConnectionString);            
             conn.Open();             
             SQLiteCommand command = new SQLiteCommand();            
             command.Connection = conn;            
             command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='" + tableName + "'";            
             int val = Convert.ToInt32(command.ExecuteScalar());             
             command.Dispose();            
             conn.Close();            
             conn.Dispose();            
             return val;        
         }

    }
}



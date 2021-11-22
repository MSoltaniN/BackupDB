using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using BackupDB.API.Data;
using BackupDB.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace BackupDB.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BackupController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IBackUpRepository _repoBackup;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        // [JsonProperty("tags")]
        //public List<string> localServers { get; set; }


        public BackupController(IDatingRepository repo,IBackUpRepository backupRepo, IMapper mapper, IConfiguration configuration)
        {
            _repo = repo;
            _repoBackup = backupRepo ;
            _mapper = mapper;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet("Servers")]
        public async Task<IActionResult> GetServers()
        {
            // -------------    get from registry  -------------
            // var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            // var key = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL");
            // List<ServerNameDto> localServers = new List<ServerNameDto>();
            // foreach (string sqlserver in key.GetValueNames())
            //     localServers.Add(new ServerNameDto { ServerName = string.Format(@"{0}\\{1}", Environment.MachineName, sqlserver) });

            //---------------  get from commands ------------------
            // var command = "OSQL -L";
            string command = @"Get-Service";
            System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("powershell.exe ", "-Command  " + command);
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.WorkingDirectory = @"C:\";
            procStartInfo.CreateNoWindow = true; //whether you want to display the command window
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            string result = proc.StandardOutput.ReadToEnd();

            int index = -1;
            List<string> resultSet = new List<string>();
            result = result.Trim().Replace(" ", "");
            while (result.IndexOf(@"SQLServerAgent(") != -1)
            {
                result = result.Substring(result.IndexOf(@"SQLServerAgent("), result.Length - result.IndexOf(@"SQLServerAgent("));
                resultSet.Add(result.Substring(0, result.IndexOf(@")") + 1));
                result = result.Substring(result.IndexOf(@")"), result.Length - result.IndexOf(@")"));
            }



            // result.Substring(result.IndexOf(@"SQL Server Agent ("), result.IndexOf("\r\n\r\n") + 1);
            // string[] resultSet = result.Trim().Replace(" ", "").Split("\r\n");

            if (resultSet.Count != 0)
            {
                List<ServerNameDto> localServers = new List<ServerNameDto>();
                foreach (var item in resultSet)
                {
                    string tmpItem = item;
                    if (tmpItem.IndexOf("(MSSQLSERVER)") != -1)
                        tmpItem = tmpItem.Replace("(MSSQLSERVER)", "(local)");

                    if (tmpItem.IndexOf("(local)") != -1)
                        localServers.Add(new ServerNameDto { ServerName = tmpItem.Substring(tmpItem.IndexOf("("), tmpItem.IndexOf(")") - tmpItem.IndexOf("(") + 1) });
                    else if (tmpItem.IndexOf("(") != -1 && tmpItem.IndexOf(")") != -1)
                        localServers.Add(new ServerNameDto { ServerName = Environment.MachineName + "\\" + tmpItem.Substring(tmpItem.IndexOf("(") + 1, tmpItem.IndexOf(")") - tmpItem.IndexOf("(") - 1) });
                    // if(item.IndexOf(Environment.MachineName)!=-1 )
                    //   localServers.Add(new ServerNameDto { ServerName = item });
                }
                return Ok(localServers);
            }
            else
                return BadRequest("سرور پایگاه داده روی سیستم مورد نظر یافت نشد");


        }

        [AllowAnonymous]
        [HttpPost("DataBases")]
        public async Task<IActionResult> GetDataBases([FromBody] ServerNameDto serverNameDto)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            builder.DataSource = serverNameDto.ServerName;
            builder.UserID = _configuration.GetSection("ServerBackUpInfo:ServerUser").Value;
            builder.Password = _configuration.GetSection("ServerBackUpInfo:ServerPass").Value;
            builder.InitialCatalog = "master";


            // Turn on integrated security:
            builder.Remove("User ID");
            builder.Remove("Password");
            builder.IntegratedSecurity = true;

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();

                    String sql = @" SELECT 
                            ROW_NUMBER() OVER (PARTITION BY  msdb.dbo.backupset.database_name ORDER BY msdb.dbo.backupset.backup_finish_date DESC) AS rank,
                           msdb.dbo.backupset.database_name,   
                            msdb.dbo.backupset.backup_start_date,   
                            msdb.dbo.backupset.backup_finish_date,   
                            msdb.dbo.backupset.expiration_date,   
                            CASE msdb..backupset.type  
                                WHEN 'D' THEN 'Database'  
                                WHEN 'L' THEN 'Log'  
                                END AS backup_type,   
                            msdb.dbo.backupset.backup_size,   
                            msdb.dbo.backupmediafamily.logical_device_name,   
                            msdb.dbo.backupmediafamily.physical_device_name,   
                            msdb.dbo.backupset.name AS backupset_name,   
                            msdb.dbo.backupset.description  ,
                            0 as include_backup_process ,
                             REPLACE ( CONVERT(CHAR(100), SERVERPROPERTY('Servername')),' ','') AS server_name
                            FROM  
                            msdb.dbo.backupmediafamily  
                            INNER JOIN msdb.dbo.backupset ON msdb.dbo.backupmediafamily.media_set_id = msdb.dbo.backupset.media_set_id 
                            ----WHERE    (CONVERT(datetime, msdb.dbo.backupset.backup_start_date, 102) >= GETDATE() - 7)   
                            --ORDER BY  
                           -- msdb.dbo.backupset.database_name,   
                            --msdb.dbo.backupset.backup_finish_date
                             ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            return Ok(Serialize(reader));
                        }
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Errors[0].ToString().Contains("Login failed for user"))
                        return BadRequest(".اطلاعات کاربری برای اتصال به سرور اشتباه است");
                    if (ex.Errors[0].ToString().Contains("but then an error occurred during the login process"))
                        return BadRequest("اشکالی در ارتباط وجود دارد، لطفا دوباره امتحان کنید.");
                    else
                        return BadRequest(ex.Errors[0].ToString());
                }

            }
        }

        [AllowAnonymous]
        [HttpPost("Process")]
        public async Task<IActionResult> ProcessBackUpDataBases([FromBody] DBForBackUpProcessDto[] dbForBackUpProcessDto)
        {
            string ErrMsg = "";
            foreach (var item in dbForBackUpProcessDto)
            {
                string backupPath;
                if (!string.IsNullOrEmpty(item.DBPath))
                {
                    backupPath = item.DBPath;
                    AddOrUpdateAppSetting("ServerBackUpInfo:BackUpPath", backupPath);
                }

                else
                {
                    backupPath = _configuration.GetSection("ServerBackUpInfo:BackUpPath").Value;
                }

                //Set up the SSH connection
                 var serverInfo = await _repoBackup.GetServerInfo(item.userId);

                using (var sshClient = new SshClient(serverInfo.IP, serverInfo.Username, serverInfo.Password))
                {
                    //Accept Host key
                    sshClient.HostKeyReceived += delegate (object sender, HostKeyEventArgs e)
                    {
                        e.CanTrust = true;
                    };
                    try
                    {
                        //Start the connection
                        sshClient.Connect();

                        _sshRes = sshClient.RunCommand("cd " + backupPath);
                        if (!string.IsNullOrEmpty(_sshRes.Error) && _sshRes.Error.ToLower().Contains("cannot find"))
                        {
                            _sshRes = sshClient.RunCommand("mkdir " + backupPath);
                            if (!string.IsNullOrEmpty(_sshRes.Error) && _sshRes.Error.Contains("Cannot find"))
                            {
                                { ErrMsg += "could not create dir!" + _sshRes.Error; continue; }
                            }
                        }
                        else
                        { ErrMsg += _sshRes.Error; continue; }

                        //sshClient.CreateCommand("pwd").Execute()

                        sshClient.Disconnect();
                    }
                    catch (Exception ex)
                    {
                        if(ex.Message == "Permission denied (password)")
                            ErrMsg += "نام کاربری یا کلمه عبور سرور میزبان صحیح نیست";
                        continue;
                    }


                }

                // if (!Directory.Exists(backupPath))
                //     Directory.CreateDirectory(backupPath);


                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = item.serverName;
                builder.UserID = _configuration.GetSection("ServerBackUpInfo:ServerUser").Value;
                builder.Password = _configuration.GetSection("ServerBackUpInfo:ServerPass").Value;
                builder.InitialCatalog = item.DBName;

                // Turn on integrated security:
                builder.Remove("User ID");
                builder.Remove("Password");
                builder.IntegratedSecurity = true;

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    try
                    {

                        connection.Open();

                        String sql = string.Format(@" BACKUP DATABASE {0}
                                                TO DISK = '{1}\{2}-{3}.bak'
                                                WITH FORMAT,
                                                    MEDIANAME = 'SQLServerBackups',
                                                    NAME = 'Full Backup of {4}';
                                             "
                                                       , item.DBName
                                                       , backupPath
                                                        , item.DBName
                                                        , System.DateTime.Now.ToString().Trim().Replace("/", "-").Replace(":", "-").Replace(" ", "_")
                                                        , item.DBName
                                                        );

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                continue;
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Errors[0].ToString().ToLower().Contains("login failed"))
                            ErrMsg += ".اطلاعات کاربری برای اتصال به پایگاه داده اشتباه است" + " : " + item.DBName + "\r\n";
                        else if (ex.Errors[0].ToString().Contains("but then an error occurred during the login process"))
                            ErrMsg += ".اشکالی در ارتباط وجود دارد، لطفا دوباره امتحان کنید" + " : " + item.DBName + "\r\n";
                        else if (ex.Errors[0].ToString().Contains("Access is denied"))
                            ErrMsg += ".سیستم به پوشه مورد نظر دسترسی ندارد" + " : " + item.DBName + "\r\n";
                        else
                            ErrMsg += ex.Errors[0].ToString() + " : " + item.DBName + "\r\n";
                    }
                }
            }
            if (ErrMsg != "")
                return BadRequest(ErrMsg);
            else
                return Ok();

        }

        [AllowAnonymous]
        [HttpGet("DefaultDBPath")]
        public async Task<IActionResult> DefaultDBPath()
        {
            string backupPath = _configuration.GetSection("ServerBackUpInfo:BackUpPath").Value;
            if (string.IsNullOrEmpty(backupPath))
                return BadRequest("not exist");
            return Ok(JsonConvert.SerializeObject(backupPath));
        }
        public IEnumerable<Dictionary<string, object>> Serialize(SqlDataReader reader)
        {
            var results = new List<Dictionary<string, object>>();
            var cols = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++)
                cols.Add(reader.GetName(i));

            while (reader.Read())
                results.Add(SerializeRow(cols, reader));

            return results;
        }
        private Dictionary<string, object> SerializeRow(IEnumerable<string> cols,
                                                SqlDataReader reader)
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols)
                result.Add(col, reader[col]);
            return result;
        }

        public static void AddOrUpdateAppSetting<T>(string key, T value)
        {
            try
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "appSettings.json");
                string json = System.IO.File.ReadAllText(filePath);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                var sectionPath = key.Split(":")[0];

                if (!string.IsNullOrEmpty(sectionPath))
                {
                    var keyPath = key.Split(":")[1];
                    jsonObj[sectionPath][keyPath] = value;
                }
                else
                {
                    jsonObj[sectionPath] = value; // if no sectionpath just set the value
                }

                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(filePath, output);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message + "----   Error writing app settings");
            }
        }
        private static String _responseErrors;
        private static Chilkat.Ssh _ssh = new Chilkat.Ssh();
        private static SshCommand _sshRes;
        private static bool _HostMachineOSIsWin = true;
        private static int _HostMachinePort = 22;
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace BackupDB.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BackupController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        [JsonProperty("tags")]
        public List<string> localServers { get; set; }


        public BackupController(IDatingRepository repo, IMapper mapper, IConfiguration configuration)
        {
            _repo = repo;
            _mapper = mapper;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet("Servers")]
        public async Task<IActionResult> GetServers()
        {
            // -------------    get from registry  -------------
            var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            var key = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL");
            List<ServerNameDto> localServers = new List<ServerNameDto>();
            foreach (string sqlserver in key.GetValueNames())
                localServers.Add(new ServerNameDto { ServerName = string.Format(@"(local)", Environment.MachineName, sqlserver) });

            //---------------  get from commands ------------------
            //var command = "OSQL -L";
            // string command = @"Get-Service | ?{ $_.DisplayName -like ""SQL Server*"" } | ?{ $_.Name -like ""MSSQL*"" }";
            // //Regex.Replace(command, @"(\d+\/\d+)""", "$1\\\"");
            // Debug.WriteLine(command);
            // await  System.IO.File.WriteAllTextAsync("c:/a/a.txt", command);
            // System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);
            // procStartInfo.RedirectStandardOutput = true;
            // procStartInfo.UseShellExecute = false;
            // procStartInfo.WorkingDirectory = @"C:\";
            // procStartInfo.CreateNoWindow = true; //whether you want to display the command window
            // System.Diagnostics.Process proc = new System.Diagnostics.Process();
            // proc.StartInfo = procStartInfo;
            // proc.Start();
            // string result = proc.StandardOutput.ReadToEnd();

            // //string[] s = result.Trim().Replace(" ", "").Split("\r\n");
            // string[] s = result.Trim().Replace(" ", "").Split("\r\n");
            // List<string> s2 = new List<string>();
            // foreach (var item in s)
            // {
            //     s2.Add(item.Substring(item.IndexOf("("),item.IndexOf(")")));
            // }
            return Ok(localServers);
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

                    String sql = @" SELECT DISTINCT a.*  FROM  (SELECT 
                             --CONVERT(CHAR(100), SERVERPROPERTY('Servername')) AS Server,    
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
                            msdb.dbo.backupset.description  
                            FROM  
                            msdb.dbo.backupmediafamily  
                            INNER JOIN msdb.dbo.backupset ON msdb.dbo.backupmediafamily.media_set_id = msdb.dbo.backupset.media_set_id 
                            ----WHERE    (CONVERT(datetime, msdb.dbo.backupset.backup_start_date, 102) >= GETDATE() - 7)   
                            --ORDER BY  
                           -- msdb.dbo.backupset.database_name,   
                            --msdb.dbo.backupset.backup_finish_date
                             ) a
                            WHERE a.rank=1 ";

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
        public async Task<IActionResult> ProcessBackUpDataBases([FromBody] DBForBackUpProcessDto dbForBackUpProcessDto)
        {
            var backupPath = _configuration.GetSection("ServerBackUpInfo:BackUpPath").Value;
            if (!Directory.Exists(backupPath))
                Directory.CreateDirectory(backupPath);

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = dbForBackUpProcessDto.serverName;
            builder.UserID = _configuration.GetSection("ServerBackUpInfo:ServerUser").Value;
            builder.Password = _configuration.GetSection("ServerBackUpInfo:ServerPass").Value;
            builder.InitialCatalog = dbForBackUpProcessDto.DBName;
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
                                                   , dbForBackUpProcessDto.DBName
                                                   , backupPath
                                                    , dbForBackUpProcessDto.DBName
                                                    , System.DateTime.Now.ToString().Trim().Replace("/", "-").Replace(":", "-").Replace(" ", "_")
                                                    , dbForBackUpProcessDto.DBName
                                                    );

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
                        return BadRequest(".اطلاعات کاربری برای اتصال به پایگاه داده اشتباه است");
                    if (ex.Errors[0].ToString().Contains("but then an error occurred during the login process"))
                        return BadRequest("اشکالی در ارتباط وجود دارد، لطفا دوباره امتحان کنید.");
                    else
                        return BadRequest(ex.Errors[0].ToString());
                }

            }
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

    }
}
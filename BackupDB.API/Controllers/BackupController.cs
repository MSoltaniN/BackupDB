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

        public BackupController(IDatingRepository repo, IMapper mapper,IConfiguration configuration)
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
            // var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            // var key = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL");
            // List<string> l = new List<string>();
            // foreach (string sqlserver in key.GetValueNames())
            //     l.Add(string.Format("{0}\\{1}", Environment.MachineName, sqlserver));

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
            List<string> j = new List<string>();

            var values = new Dictionary<string,string> { {"serverName", "DESKTOP-3ISN91B\\MSSQLSERVER2"} };

            return Ok(values);
        }

        [AllowAnonymous]
        [HttpPost("DataBases")]
        public async Task<IActionResult> GetDataBases([FromBody]ServerNameDto serverNameDto)
        {
            List<string> queryResult = new List<string>();
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            builder.DataSource = serverNameDto.ServerName;
            builder.UserID =  _configuration.GetSection("ServerBackUpInfo:ServerUser").Value;
            builder.Password =  _configuration.GetSection("ServerBackUpInfo:ServerPass").Value;
            builder.InitialCatalog = "master";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                String sql = @"SELECT  
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
                            --WHERE    (CONVERT(datetime, msdb.dbo.backupset.backup_start_date, 102) >= GETDATE() - 7)   
                            ORDER BY  
                            msdb.dbo.backupset.database_name,   
                            msdb.dbo.backupset.backup_finish_date ";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        return Ok(Serialize(reader));
                    }
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
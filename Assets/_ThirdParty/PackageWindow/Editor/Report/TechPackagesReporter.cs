using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace CookApps.Package.Report.Editor
{
    internal class TechPackagesReporter
    {
        //--------------------------------------------------------------------------------//
        //-----------------------------------FIELD----------------------------------------//
        //--------------------------------------------------------------------------------//
        //------------------- Inspector ------------------//

        //------------------- public ------------------//

        //------------------- protected ------------------//

        //------------------- private ------------------//
        private static string URL_ROOT = "http://localhost:5000";
        //private static string URL_ROOT = "https://packages-insight.tech.cookapps.com";

        //--------------------------------------------------------------------------------//
        //------------------------------------PROPERTY------------------------------------//
        //--------------------------------------------------------------------------------//

        //--------------------------------------------------------------------------------//
        //------------------------------------METHOD--------------------------------------//
        //--------------------------------------------------------------------------------//
        //───────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// 모든 cell의 value를 가져옵니다
        /// </summary>
        /// <returns></returns>
        internal static async Task<(bool isSuccess, string response)> GetCellValue()
        {
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"{URL_ROOT}/report/all_cells");
            return await CheckResponse(response);
            // return await Send($"{URL_ROOT}/report/all_cells", string.Empty);
        }

        /// <summary>
        /// valueRange의 값을 구글시트에 모두 기록합니다.
        /// </summary>
        /// <param name="valueRange"></param>
        internal static void ReportCells(ValueRange valueRange)
        {
            string jsonInputData = JsonConvert.SerializeObject(valueRange);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            //Send($"{URL_ROOT}/report/set_cells", jsonInputData);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        /// <summary>
        /// 메모(note)를 구글시트에 기록합니다
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="notes"></param>
        internal static void ReportNotes(string projectName, Dictionary<string, CellIndexData> notes)
        {
            NoteData noteData = new NoteData();
            noteData.ProjectName = projectName;
            noteData.Notes = notes;
            string jsonInputData = JsonConvert.SerializeObject(noteData);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            //Send($"{URL_ROOT}/report/set_notes", jsonInputData);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private static async Task<(bool, string)> Send(string url, string jsonInputData)
        {
            var client = new HttpClient();
            var content = new StringContent(jsonInputData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);
            return await CheckResponse(response);
        }

        private static async Task<(bool, string)> CheckResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode == false)
            {
#if TECH_PACKAGE_WINDOW
                Debug.Log("Error : Fail!");
#endif
                return (false, "Fail");
            }
            else
            {
                string responseContent = await response.Content.ReadAsStringAsync();
#if TECH_PACKAGE_WINDOW
                Debug.Log($"Success : {responseContent}");
#endif
                return (true, responseContent);
            }
        }
    }

    internal class ValueRange
    {
        [JsonProperty("majorDimension")]
        public virtual string MajorDimension { get; set; }

        [JsonProperty("range")]
        public virtual string Range { get; set; }

        [JsonProperty("values")]
        public virtual IList<IList<object>> Values { get; set; }

        /// <summary>The ETag of the item.</summary>
        public virtual string ETag { get; set; }
    }

    [Serializable]
    internal class CellData
    {
        public string ProjectName;
    }

    [Serializable]
    internal class NoteData : CellData
    {
        public Dictionary<string, CellIndexData> Notes;
    }

    [Serializable]
    internal class PackageData : CellData
    {
        public string PackageName;
        public string CellValue;
        public string Note;
    }

    [Serializable]
    internal class CellIndexData
    {
        public string Note;
        public int Row;
        public int Column;
    }
}

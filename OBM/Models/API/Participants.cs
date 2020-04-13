using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

/// <summary>
/// This is the source for this API handling class: https://github.com/WolfenCLI/Challonge-CSharp-API
/// </summary>
/// 
namespace OBM.Models.API
{
    public class Participants
    {
        static public async Task<JObject> Create(ChallongeApi api, string tournamentID, List<KeyValuePair<string, string>> parameters)
        {
            string path = "tournaments" + tournamentID + "/participants.json";

            return await api.FetchAndParse(ChallongeApi.Methods.POST, path, parameters);
        }
        static public async Task<JObject> BulkAdd(ChallongeApi api, string tournamentID, List<KeyValuePair<string, string>> parameters)
        {
            string path = "tournaments" + tournamentID + "/participants/bulk_add.json";

            return await api.FetchAndParse(ChallongeApi.Methods.POST, path, parameters);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace elasticService.Controllers
{
    [Route("api/[controller]")]
    public class LogsController : Controller
    {
        private static readonly ConnectionSettings connSettings =
        new ConnectionSettings(new Uri("http://localhost:9200/"))
                        .DefaultIndex("log_history")
                        //Optionally override the default index for specific types
                        .MapDefaultTypeIndices(m => m
                        .Add(typeof(Log), "log_history"));
        private static readonly ElasticClient elasticClient = new ElasticClient(connSettings);
        //Post /Logs/api/InsertLog
        [HttpPost]
        public void InsertLog([FromBody]Log log)
        {
            //elasticClient.DeleteIndex("log_history");

            if (!elasticClient.IndexExists("error_log").Exists)
            {
                var indexSettings = new IndexSettings();
                indexSettings.NumberOfReplicas = 1;
                indexSettings.NumberOfShards = 3;


                var createIndexDescriptor = new CreateIndexDescriptor("log_history")
               .Mappings(ms => ms
                               .Map<Log>(m => m.AutoMap())
                        )
                .InitializeUsing(new IndexState() { Settings = indexSettings })
                .Aliases(a => a.Alias("error_log"));

                var response = elasticClient.CreateIndex(createIndexDescriptor);
            }
            //Insert Data           

            elasticClient.Index<Log>(log, idx => idx.Index("log_history"));
        }

        [HttpGet]
        public async Task<List<string>> GetAllErrors()
        {
            var response = await elasticClient.SearchAsync<Log>(p => p
                 //.Source(f=>f.Includes(p2=>p2.Field(f2=>f2.message)))                  
                 .Query(q => q
                 .MatchAll()
                 )
                 .PostFilter(f => f.DateRange(r => r.Field(f2 => f2.PostDate).GreaterThanOrEquals(DateTime.Now.AddDays(-7))))
            );
            var result = new List<string>();
            foreach (var document in response.Documents)
            {
                result.Add(document.message);
            }
            return result.Distinct().ToList();
        }

        // GET api/values/5
        [HttpGet("{error}")]
        public async Task<List<Log>> Get(string error)
        {
            /*var response2 = elasticClient.Search<Log>(p => p
              .From(0)
              .Size(10)
              .Query(q =>
              q.Term(f => f.UserID, 1)
            )
            );*/

            var response = await elasticClient.SearchAsync<Log>(p => p
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.message)
                            .Query(error)
                            .Operator(Operator.And)
                            )
                    )  
                    .Sort(s=>s.Descending(f=>f.PostDate))      
            );

            var result = new List<Log>();
            foreach (var document in response.Documents)
            {
                result.Add(document);
            }
            return result;
        }
    }
}

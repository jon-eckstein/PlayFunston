using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq;
using System.Data.SqlServerCe;
using System.Data;
using ShouldITakeMyDogToFortFunstonNow.Models;
using System.Configuration;
using System.IO;

namespace ShouldITakeMyDogToFortFunstonNow.Services
{
    public class SqlCeRepoService
    {

        //private readonly string DB_LOCATION = ConfigurationManager.ConnectionStrings["db"].ConnectionString;        
        private readonly string DB_LOCATION = "Data Source=" + HttpContext.Current.Server.MapPath("~/App_Data/db.sdf");
        private MyDataContext context;

        public SqlCeRepoService()
        {                       
            using (context = new MyDataContext(DB_LOCATION))
            {
                if (!context.DatabaseExists())
                    context.CreateDatabase();
            }
        }


        public IEnumerable<CurrentObservation> GetAllObservations()
        {
            using (context = new MyDataContext(DB_LOCATION))
            {
                foreach (var obs in context.Observations)
                    yield return obs;
            }
        }

    }


    public class MyDataContext : DataContext
    {
        public MyDataContext(string fileOrServerOrConnection)
            : this(new SqlCeConnection(fileOrServerOrConnection))
        {
        }

        public MyDataContext(IDbConnection conn)
            : base(conn)
        { 
            
        }

        public Table<CurrentObservation> Observations
        {
            get { return GetTable<CurrentObservation>(); }
        }        
    }
}
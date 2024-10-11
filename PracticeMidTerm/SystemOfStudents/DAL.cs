using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    internal class Connect
    {
        // =========================================================================
        // We could use the Design Pattern Singleton for this class. 
        // However, it is also possible (and a little simpler) to 
        // just use static attributes and static methods.
        // =========================================================================

        private static String cliComConnectionString = GetConnectString();

        internal static String ConnectionString { get => cliComConnectionString; }

        private static String GetConnectString()
        {
            SqlConnectionStringBuilder cs = new SqlConnectionStringBuilder();

            cs.DataSource = "(local)";
            cs.InitialCatalog = "StudentProg"; // deveria ser o nome das tabelas? e em ordem?
            cs.UserID = "sa";
            cs.Password = "sysadm";
            return cs.ConnectionString;
        }

    }
    internal class DataTables
    {
        // =========================================================================
        // We could use the Design Pattern Singleton for this class. 
        // However, it is also possible (and a little simpler) to 
        // just use static attributes and static methods.
        // =========================================================================

        private static SqlDataAdapter adapterClients = InitAdapterClients();
        private static SqlDataAdapter adapterCommandes = InitAdapterCommandes();

        private static DataSet ds = InitDataSet();

        private static SqlDataAdapter InitAdapterClients()
        {
            SqlDataAdapter r = new SqlDataAdapter(
                "SELECT * FROM Programs ORDER BY ProgId ",
                Connect.ConnectionString);

            SqlCommandBuilder builder = new SqlCommandBuilder(r);
            r.UpdateCommand = builder.GetUpdateCommand();

            return r;
        }

        private static SqlDataAdapter InitAdapterCommandes()
        {
            SqlDataAdapter r = new SqlDataAdapter(
                "SELECT * FROM Students ORDER BY StudId ",
                Connect.ConnectionString);

            SqlCommandBuilder builder = new SqlCommandBuilder(r);
            // For the ON UPDATE CASCADE relative to ProgId
            builder.ConflictOption = ConflictOption.OverwriteChanges;
            //
            r.UpdateCommand = builder.GetUpdateCommand();

            return r;
        }

        private static DataSet InitDataSet()
        {
            DataSet ds = new DataSet();
            loadPrograms(ds);
            loadStudents(ds);
            return ds;
        }

        private static void loadPrograms(DataSet ds)
        {
            // =========================================================================
            adapterClients.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            // =========================================================================

            adapterClients.Fill(ds, "Programs");           
        }

        private static void loadStudents(DataSet ds)
        {
            // =========================================================================
            adapterCommandes.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            // =========================================================================

            adapterCommandes.Fill(ds, "Students");


            ForeignKeyConstraint myFK = new ForeignKeyConstraint("MyFK",
                new DataColumn[]{
                ds.Tables["Programs"].Columns["ProgId"]
                },
                new DataColumn[] {
                ds.Tables["Students"].Columns["ProgId"]
                }
            );
            myFK.DeleteRule = Rule.None;
            myFK.UpdateRule = Rule.Cascade;
            ds.Tables["Students"].Constraints.Add(myFK);

            // ========================================================================= 
        }

        internal static SqlDataAdapter getAdapterPrograms()
        {
            return adapterClients;
        }
        internal static SqlDataAdapter getAdapterStudents()
        {
            return adapterCommandes;
        }
        internal static DataSet getDataSet()
        {
            return ds;
        }
    }
    internal class Programs
    {
        private static SqlDataAdapter adapter = DataTables.getAdapterPrograms();
        private static DataSet ds = DataTables.getDataSet();

        internal static DataTable GetPrograms()
        {
            return ds.Tables["Programs"];
        }

        internal static int UpdatePrograms()
        {
            if (!ds.Tables["Programs"].HasErrors)
            {
                return adapter.Update(ds.Tables["Programs"]);
            }
            else
            {
                return -1;
            }
        }
    }

    internal class Students
    {
        private static SqlDataAdapter adapter = DataTables.getAdapterStudents();
        private static DataSet ds = DataTables.getDataSet();

        internal static DataTable GetStudents()
        {
            return ds.Tables["Students"];
        }

        internal static int UpdateStudents()
        {
            if (!ds.Tables["Students"].HasErrors)
            {
                return adapter.Update(ds.Tables["Students"]);
            }
            else
            {
                return -1;
            }
        }
    }
}

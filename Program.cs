using System;
using System.Data;

namespace ParcelLoader
{
    class Program
    {
        static void Main(string[] args) //2D Y0 X0
        {
            string dbServer = args[0];
            string dbPort = args[1];
            string dbSID = args[2];
            string dbUsername = args[3];
            string dbPassword = args[4];

            try

            {
                float y0 = 0;// -1102200; //-478400;
                float x0 = 0;// -478400; //-1102200;
                if (args.Length > 2)
                {
                    float.TryParse(args[7], out y0);
                    float.TryParse(args[6], out x0);
                }
                string connectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + dbServer + ")(PORT=" + dbPort + "))(CONNECT_DATA=(SID=" + dbSID + ")));User Id=" + dbUsername + "; Password=" + dbPassword + ";";
                var connection = new Oracle.ManagedDataAccess.Client.OracleConnection(connectionString);
                connection.Open();
                var command = new Oracle.ManagedDataAccess.Client.OracleCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                switch (args[5].ToCharArray()[0])
                {
                    case '2':
                        {
                            load2DParcels(y0, x0, command);
                            break;
                        }
                    case '3':
                        {
                            load3DParcels(y0, x0, command);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                connection.Close();
            } catch (Exception e)
            {
                Console.WriteLine("Chyba v loaderu: "+e.Message);
            }
        }

        private static void load2DParcels(float y0, float x0, Oracle.ManagedDataAccess.Client.OracleCommand command)
        {
            string result = "";
            command.CommandText = "SELECT "
                + "b.bfsid, "
                + "c.sequence, "
                + "p.point.SDO_POINT.y, "
                + "p.point.SDO_POINT.x " +
                "FROM tile_2D_SU suidx, la_spatialunit su, boundary b, corner c, la_point p " +
                "WHERE suidx.roundy = " + (int)y0 + " AND suidx.roundx = " + (int)x0 + " AND " +
                "suidx.suid = su.suid AND " +
                "su.suid = b.suid AND " +
                "b.bfsid = c.boundaryid AND " +
                "c.pid = p.pid " +
                "GROUP BY b.bfsid, c.sequence, p.point.SDO_POINT.y, p.point.SDO_POINT.x " +
                "ORDER BY b.bfsid, c.sequence";
            Oracle.ManagedDataAccess.Client.OracleDataReader dataReader = command.ExecuteReader();
            int j = 0;
            while (dataReader.Read()) // && j++ < 10)
            {
                for(int i = 0; i < dataReader.FieldCount; i++)
                {
                    result += dataReader[i].ToString() + ";";
                }
                result += "n";
            }
            dataReader.Close();
            Console.WriteLine(result);
        }

        private static void load3DParcels(float y0, float x0, Oracle.ManagedDataAccess.Client.OracleCommand command)
        {
            string result = "";
            command.CommandText = "SELECT "
                + "su.suid, "               //0
                + "su.cislo_par, "          //1
                + "b.bfid, "                //2
                + "b.direction, "           //3
                + "c.sequence, "            //4
                + "p.point.SDO_POINT.y, "   //5
                + "p.point.SDO_POINT.x, "   //6
                + "c.elevation " +			//7
                "FROM tile_3D_SU suidx, la_spatialunit su, boundary3D b, corner c, la_point p " +
                "WHERE suidx.roundy = " + (int)y0 + " AND suidx.roundx = " + (int)x0 + " AND " +
                "suidx.suid = su.suid AND " +
                "su.suid = b.suid AND " +
                "b.bfid = c.boundaryid AND " +
                "c.pid = p.pid " +
                "GROUP BY su.suid, su.cislo_par, b.bfid,  b.direction, c.sequence, p.point.SDO_POINT.y, p.point.SDO_POINT.x, c.elevation " +
                "ORDER BY su.suid, b.bfid, c.sequence";
            //Oracle.ManagedDataAccess.Types.
            //ListDictionaryInternal
            Oracle.ManagedDataAccess.Client.OracleDataReader dataReader = command.ExecuteReader();
            //STRUCT strukturaBodu = (oracle.sql.STRUCT)dataReader.getObject(3);
            //JGeometry geometrieBodu = JGeometry.load(strukturaBodu);
            int j = 0;
            while (dataReader.Read()) // && j++ < 10)
            {
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    result += dataReader[i].ToString() + ";";
                }
                result += "n";
            }
            dataReader.Close();
            Console.WriteLine(result);
        }
    }
}

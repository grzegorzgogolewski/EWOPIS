using FirebirdSql.Data.FirebirdClient;
using System.Data;
using System.Text;

namespace EWOPIS_Tools
{

    public class FbOsrodek
    {
        private string _connectionString;


        public FbOsrodek(string dataBasaFile, string userName, string userPass)
        {
            _connectionString = new FbConnectionStringBuilder
            {
                Database = dataBasaFile,
                UserID = userName,
                Password = userPass,
                DataSource = "192.168.10.121"
            }.ToString();

        }

        public DataTable GetOperatyTable (string rok)
        {
            DataTable dtOperaty = new DataTable();

            using (var connection = new FbConnection(_connectionString))
            {
                connection.Open();

                FbCommand command = new FbCommand("SELECT uid, typ, uwagi, (SELECT NUMER FROM OPERATY_NUM1(uid)) AS operat, (SELECT NUMER FROM KERG_NUM1(id_roboty)) AS kerg, c1 || '.' || c2 || '.' || c3 ||'.' || c4 AS idmat FROM operaty where c3 = " + rok + " order by uid;", connection); // B, E, N, S

                FbDataAdapter da = new FbDataAdapter(command);

                da.Fill(dtOperaty);

                command.Dispose();
                connection.Close();
            }

            return dtOperaty;
        }


        public DataTable GetOperDokTable(string rok)
        {
            DataTable dtOperDok = new DataTable();

            using (var connection = new FbConnection(_connectionString))
            {
                connection.Open();

                FbCommand command = new FbCommand("SELECT uid, typ, id_ope, dokument, typ_dok, id_blob FROM operdok WHERE id_blob <> 0 AND id_ope IN (SELECT uid FROM operaty WHERE c3 = " + rok + ") order by id_ope;", connection);

                FbDataAdapter da = new FbDataAdapter(command);

                da.Fill(dtOperDok);

                command.Dispose();
                connection.Close();

            }

            return dtOperDok;
        }

        public byte[] GetFbDokTresc(int uid)
        {
            byte[] filedata = Encoding.ASCII.GetBytes("pusty");

            using (var connection = new FbConnection(_connectionString))
            {
                connection.Open();

                FbCommand command = new FbCommand($"SELECT tresc FROM fbdok WHERE uid = {uid};", connection);

                FbDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    if (reader["tresc"].ToString() != "")
                    {
                        filedata = (byte[]) reader["tresc"];
                    }
                    else
                    {
                        filedata = Encoding.ASCII.GetBytes("pusty");
                    }
                }

                command.Dispose();
                connection.Close();

            }

            return filedata;
        }

    }

}

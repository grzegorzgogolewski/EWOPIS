using System.Data;
using EWOPIS_Tools;
using System.IO;
using System;
using System.Text.RegularExpressions;

namespace ewopis
{
    class Program
    {


        static void Main(string[] args)
        {
            FbOsrodek osrodekFdb = new FbOsrodek("C:\\baza\\OSRODEK.FDB", "sysdba", "masterkey");
            FbOsrodek osrodekFdr = new FbOsrodek("C:\\baza\\OSRODEK.FDR", "sysdba", "masterkey");

            DataTable dtOperaty = osrodekFdb.GetOperatyTable(args[0]);
            DataTable dtOperDok = osrodekFdb.GetOperDokTable(args[0]);

            int currentFile = 1;
            int recordCount = dtOperDok.Rows.Count;

            Console.WriteLine("Nastapi eksport " + recordCount + " plikow.\nWciśnij ENTER!");
            Console.ReadKey();

            foreach (DataRow rowOperDok in dtOperDok.Rows)
            {

                int idOpe = (int)rowOperDok["id_ope"];

                string operatName = GetOperatName(idOpe, dtOperaty);

                Console.WriteLine("Eksport pliku dla operatu:" + operatName + " [" + currentFile++ + "/" + recordCount +"]");

                string dokument = rowOperDok["dokument"].ToString();

                if (dokument == "")
                {
                    dokument = "uid_" + rowOperDok["uid"];
                }

                int typDok = 0;

                if (rowOperDok["typ_dok"].ToString() !="")
                {
                    typDok = Convert.ToInt32(rowOperDok["typ_dok"]);
                }

                int idBlob = 0;

                if (rowOperDok["id_blob"].ToString() != "")
                {
                    idBlob = Convert.ToInt32(rowOperDok["id_blob"]);
                }

                string ext;

                switch (typDok)
                {
                    case 0: // 
                        ext = ".err0";
                        break;
                    case 1: // 
                        ext = ".err1";  
                        break;
                    case 2: // PNG
                        ext = ".png";
                        break;
                    case 3: // TXT
                        ext = ".txt";
                        break;
                    case 4: // JPG
                        ext = ".jpg";
                        break;
                    default:
                        ext = ".err";
                        break;
                }

                if (!Directory.Exists("c:\\!\\" + operatName))
                {
                    try
                    {
                        Directory.CreateDirectory("c:\\!\\" + operatName);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Console.WriteLine(operatName);
                        Console.ReadKey();
                    }
                    
                }

                dokument = Regex.Replace(dokument, @"[\u0000-\u001F]", string.Empty);
                dokument = dokument.Replace(@"\", "_");
                dokument = dokument.Replace(@"/", "_");

                string nazwaPliku = "c:\\!\\" + operatName + "\\" + dokument + ext;

                int numer = 0;

                while (File.Exists(nazwaPliku))
                {
                    numer++;
                    nazwaPliku = "c:\\!\\" + operatName + "\\" + dokument + "_" + numer + "_" + ext;
                }

                using (var fs = new FileStream(nazwaPliku, FileMode.Create))
                {
                    try
                    {
                        fs.Write(osrodekFdr.GetFbDokTresc(idBlob), 0, osrodekFdr.GetFbDokTresc(idBlob).Length);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Console.WriteLine(operatName);
                        Console.WriteLine(dokument);
                        Console.ReadKey();
                    }
                    
                }

            }

        }

        private static string GetOperatName(int uid, DataTable dt)
        {
            DataRow[] row = dt.Select("uid=" + uid);

            string operat;

            if (row.Length != 0)
            {
                string typ = row[0]["typ"].ToString();
                string kerg = row[0]["kerg"].ToString();

                 operat = row[0]["idmat"] + "__OPE_" + typ + "_" + row[0]["operat"].ToString().Replace("/", "_") + "__KRG_" + kerg.Replace("/", "_");
            }
            else
            {
                operat = "nieznany";
            }

            return operat;
        }

    }
}

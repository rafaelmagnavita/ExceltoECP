using ExceltoECP.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExceltoECP
{
    public partial class Form1 : Form
    {
        #region Variáveis

        #endregion

        #region Métodos
        public Form1()
        {
            InitializeComponent();
        }

        private void Converter_Click(object sender, EventArgs e)
        {
            Import(@"C:\Logs\Modelo personalização1.xlsx", true);
        }

        public DataSet Import(string filename, bool headers = true)
        {
            List<ObjetosSC> AccessManager = new List<ObjetosSC>();
            List<ObjetosSC> Controladoras = new List<ObjetosSC>();
            List<ObjetosSC> Portas = new List<ObjetosSC>();
            List<ObjetosSC> Lados = new List<ObjetosSC>();
            var _xl = new Excel.Application();
            var wb = _xl.Workbooks.Open(filename);
            var sheets = wb.Sheets;
            DataSet dataSet = null;
            if (sheets != null && sheets.Count != 0)
            {
                dataSet = new DataSet();
                foreach (var item in sheets)
                {
                    var sheet = (Excel.Worksheet)item;
                    DataTable dt = null;
                    if (sheet != null)
                    {
                        dt = new DataTable();
                        var ColumnCount = ((Excel.Range)sheet.UsedRange.Rows[1, Type.Missing]).Columns.Count;
                        var rowCount = ((Excel.Range)sheet.UsedRange.Columns[1, Type.Missing]).Rows.Count;

                        for (int j = 0; j < ColumnCount; j++)
                        {
                            var cell = (Excel.Range)sheet.Cells[1, j + 1];
                            var column = new DataColumn(headers ? cell.Value : string.Empty);
                            dt.Columns.Add(column);
                        }

                        for (int i = 0; i < rowCount; i++)
                        {
                            var r = dt.NewRow();
                            for (int j = 0; j < ColumnCount; j++)
                            {
                                var cell = (Excel.Range)sheet.Cells[i + 1 + (headers ? 1 : 0), j + 1];
                                r[j] = cell.Value;
                            }
                            dt.Rows.Add(r);
                        }

                    }
                    dataSet.Tables.Add(dt);
                    dataGridView1.DataSource = dt;
                    dt.Rows.Add();
                    dt.Rows.Add();
                    ObjetosSC accessManager = new ObjetosSC();
                    accessManager.Nome = "Access Manager";
                    accessManager.ObGuid = "AccessManager";
                    accessManager.Type = "AccessManager";
                    AccessManager.Add(accessManager);
                    CreateXML(AccessManager, @"C:\Logs\AccessManagers.xml");
                    int count = dt.Rows.Count;
                    for (int i = 1; i < count; i++)
                    {
                        ObjetosSC porta = new ObjetosSC();
                        porta.Nome = dt.Rows[i][2].ToString();
                        porta.Type = "Door";
                        porta.ObGuid = i.ToString();
                        porta.BelongTo = dt.Rows[i][3].ToString();
                        porta.SensorPorta = dt.Rows[i][16].ToString();
                        porta.InterfacePreferida = dt.Rows[i][13].ToString() + " - " + dt.Rows[i][14].ToString() + " - " + dt.Rows[i][15].ToString();
                        porta.LockSensor = dt.Rows[i][17].ToString();
                        porta.Trancamento = dt.Rows[i][27].ToString();
                        porta.TempoConcessao = dt.Rows[i][18].ToString();
                        porta.ConcessaoAmp = dt.Rows[i][19].ToString();
                        porta.Retrancamento = dt.Rows[i][20].ToString();
                        porta.Lado1Nome = "Entrada";
                        porta.Lado1ObGuid = (i + "in").ToString();
                        porta.Lado1Leitor = dt.Rows[i][21].ToString();
                        porta.Lado1REX = dt.Rows[i][22].ToString();
                        porta.Lado1SensorEntrada = dt.Rows[i][23].ToString();
                        porta.Lado2Nome = "Saída";
                        porta.Lado2ObGuid = (i + "out").ToString();
                        porta.Lado2Leitor = dt.Rows[i][24].ToString();
                        porta.Lado2REX = dt.Rows[i][25].ToString();
                        porta.Lado2SensorEntrada = dt.Rows[i][26].ToString();

                        if (porta.Nome != "" && porta.BelongTo != "")
                            Portas.Add(porta);
                        if(Controladoras.Where(a => a.ObGuid == dt.Rows[i][3].ToString()).Count() == 0)
                        {
                            ObjetosSC Unit = new ObjetosSC();
                            Unit.Nome = dt.Rows[i][4].ToString();
                            Unit.Type = "Unit";
                            Unit.BelongTo = "AccessManager";
                            Unit.ObGuid = dt.Rows[i][3].ToString();
                            Unit.Fabricante = dt.Rows[i][5].ToString();
                            Unit.Modelo = dt.Rows[i][6].ToString();
                            Unit.IP = dt.Rows[i][7].ToString();
                            Unit.Mask = dt.Rows[i][8].ToString();
                            Unit.Gateway = dt.Rows[i][9].ToString();
                            Unit.Login = dt.Rows[i][10].ToString();
                            Unit.Password = dt.Rows[i][11].ToString();
                            Unit.MAC = dt.Rows[i][12].ToString();
                            if (Unit.Nome != "" && Unit.BelongTo != "")
                                Controladoras.Add(Unit);
                        }

                    }
                    CreateXML(Portas, @"C:\Logs\Doors.xml");
                    CreateXML(Controladoras, @"C:\Logs\Units.xml");

                }
            }
            _xl.Quit();
            return dataSet;
        }

        private void CreateXML(Object file, String path)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(file.GetType());
                TextWriter writer = new StreamWriter(path);
                serializer.Serialize(writer, file);
                writer.Close();
            }
            catch (Exception ex)
            {
                Log.Salvar("Erro carregar CreateXML: " + ex.Message);
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }

    #endregion
}

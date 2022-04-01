using System;
using System.IO;


namespace ExceltoECP.Helpers
{
	public static class Log
	{

		#region Variáveis Privadas
		private static object trava = new object();
		#endregion

		#region Variáveis Públicas

		#endregion

		#region Metodos Públicos
		/// <summary>
		/// Salva mensagem no arquivo de log.
		/// </summary>
		/// <param name="texto">Mensagem a ser salva</param>
		/// <param name="funcao">
		/// 0 - apenas arquivo de log
		/// 1 - depuração avançada
		/// </param>
		/// <param name="obj">Outro dados</param>
		public static void Salvar(string texto, int funcao = 0)
		{
			//if (!Global.DepuracaAvancada & funcao == 1) return;

			lock (trava)
			{
				try
				{
					string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\ExceltoECP";
					if (!Directory.Exists(path)) Directory.CreateDirectory(path);

					if (!(Directory.Exists(path + "\\Logs"))) Directory.CreateDirectory(path + "\\Logs");

					string arqlog = path + "\\Logs\\" + "ExceltoECP_log_" + System.DateTime.Now.ToString("ddMMyy") + ".txt";
					File.AppendAllText(arqlog, String.Format("{0} - {1}",
					DateTime.Now, texto) + Environment.NewLine);

				}
				catch (Exception)
				{

				}
			}
		}


		#endregion

		#region Metodos Privadas
		#endregion

		#region Eventos Personalizados

		#endregion
	}
}

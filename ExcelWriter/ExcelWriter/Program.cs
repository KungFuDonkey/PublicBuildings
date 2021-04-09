using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;



using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
namespace ExcelWriter
{
	class Program
	{
		static Excel.Application xlApp;
		static Process excel;
		static bool opened = false;

		static void Main(string[] args)
		{
			ThreadStart ts = new ThreadStart(CheckInput);
			Thread t = new Thread(ts);
			t.Start();

			Console.WriteLine("Started");
			Console.WriteLine(Directory.GetCurrentDirectory());
			xlApp = new Excel.Application();
			while (true)
			{
				Loop();
			}
		}

		private static void Loop()
		{
			for (int i = 0; i < 5; i++)
			{
				GC.WaitForPendingFinalizers();
				GC.Collect();
			}
			List<(int, int, int, float)> data = GetData(new List<(int, int, int, float)>());
			if (opened)
			{
				CloseExcel();
				Thread.Sleep(1000);
			}

			Console.WriteLine("Writing to Excel file");
			string fileName = "C:\\Users\\sietz\\Desktop\\Onderzoeksmethoden\\ExcelWriter\\ExcelWriter\\bin\\Debug\\data.xlsx";

			Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(fileName);
			Excel.Sheets xlSheets = xlWorkBook.Sheets as Excel.Sheets;
			Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlSheets.Add(xlSheets[1], Type.Missing, Type.Missing, Type.Missing);
			xlWorkSheet.Name = GetName();

			WriteData(data, xlWorkSheet);
			xlWorkBook.Save();
			CloseExcel(ref xlWorkBook, ref xlSheets, ref xlWorkSheet);
		}

		private static void CloseExcel(ref Excel.Workbook xlWorkBook, ref Excel.Sheets xlSheets, ref Excel.Worksheet xlWorkSheet)
		{
			xlWorkBook.Close();
			if (xlWorkSheet != null)
			{
				Marshal.FinalReleaseComObject(xlWorkSheet);
				xlWorkSheet = null;
			}
			if (xlSheets != null)
			{
				Marshal.FinalReleaseComObject(xlSheets);
				xlSheets = null;
			}
			if (xlWorkBook != null)
			{
				Marshal.FinalReleaseComObject(xlWorkBook);
				xlWorkBook = null;
			}
		}

		private static void WriteData(List<(int, int, int, float)> data, Excel.Worksheet xlWorkSheet)
		{
			for (int i = 1; i <= data.Count; i++)
			{
				xlWorkSheet.Cells[i, 1].value = data[i - 1].Item1;
				xlWorkSheet.Cells[i, 2].value = data[i - 1].Item2;
				xlWorkSheet.Cells[i, 3].value = data[i - 1].Item3;
				xlWorkSheet.Cells[i, 4].value = data[i - 1].Item4;
			}
		}

		private static List<(int, int, int, float)> GetData(List<(int, int, int, float)> data)
		{
			while (data.Count == 0)
			{
				data = GetState();
			}

			return data;
		}

		static List<(int,int,int,float)> GetState()
		{
			List<(int, int, int, float)> state = new List<(int, int, int, float)>();
			SetupLink(state);
			Console.WriteLine("Succesfully received state");

			return state;
		}

		private static void SetupLink(List<(int, int, int, float)> state)
		{
			NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "excelpipe", PipeDirection.In);
			pipeClient.Connect();
			Console.WriteLine("Connected Succesfully");

			ReadData(state, pipeClient);
			pipeClient.Dispose();
		}

		private static void ReadData(List<(int, int, int, float)> state, NamedPipeClientStream pipeClient)
		{
			using (StreamReader sr = new StreamReader(pipeClient))
			{
				// Display the read text to the console
				string temp;
				while ((temp = sr.ReadLine()) != null)
				{
					string[] data = temp.Split(' ');
					state.Add((int.Parse(data[0]), int.Parse(data[1]), int.Parse(data[2]), float.Parse(data[3])));
				}
			}
		}

		static void CheckInput()
		{
			while (true)
			{
				InputLoop();
			}
		}

		private static void InputLoop()
		{
			string input = Console.ReadLine();
			switch (input)
			{
				case "exit":
					xlApp.Quit();
					KillProcesses();
					if (opened) CloseExcel();
					Environment.Exit(0);
					return;
				case "open":
					OpenExcel();
					return;
				case "close":
					CloseExcel();
					return;
				case "help":
					DisplayHelp();
					return;
				default:
					Console.WriteLine("Not a valid command");
					DisplayHelp();
					return;
			}
		}

		private static void KillProcesses()
		{
			Process[] ps = Process.GetProcessesByName("EXCEL");
			foreach (Process p in ps)
			{
				try
				{
					p.Kill();
				}
				catch
				{

				}
			}
		}

		static void DisplayHelp()
		{
			Console.WriteLine("exit  - Exits the terminal \nopen  - Opens excel \nclose - Closes excel \nhelp  - Shows this screen");
		}

		static void OpenExcel()
		{
			excel = Process.Start("C:\\Users\\sietz\\Desktop\\Onderzoeksmethoden\\ExcelWriter\\ExcelWriter\\bin\\Debug\\data.xlsx");
			opened = true;
		}

		static void CloseExcel()
		{
			try
			{
				excel.Kill();
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
			
			opened = false;
		}

		static string GetName()
		{
			char[] date = DateTime.UtcNow.ToString().ToCharArray();
			for(int i = 0; i < date.Length; i++)
			{
				if (date[i] == '/') date[i] = ' ';
				if (date[i] == ':') date[i] = ' ';
			}
			return new string(date);
		}
	}
}

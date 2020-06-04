using System;
using System.Collections;

namespace App3
{

	/*
		Allgemeine Erklärung der Funktionsweise des Generieren von zufälligen Sudokus:

		Zunächst wird ein leeres Sudoku erzeugt (in jedem Feld ist die Zahl 0).
		Nun werden alle einsen gesetzt (-> 9 Stück), dann alle zweien (-> auch 9 Stück), dann alle dreien usw.
		Dabei wird nach setzen der ersten Ziffer alle Felder "geschlossen", welche keine Ziffer des selben Wertes
		mehr enthalten dürfen. Dann werden die Zeilen ausfindig gemacht, die die wenigsten freien Felder haben.
		Sollten mehrere Zeilen gleich viele freie Felder haben, wird eine zufällige davon ausgewählt.
		Danach wird ein zufälliges freies Feld dieser Zeile ausgesucht und mit der aktuellen Ziffer befüllt.
		Nun werden wieder alle Felder geschlossen, welche diese Ziffer nicht mehr enthalten dürfen usw.
		Ist eine Ziffer 9mal verteilt, wird die nächst höhere gewählt und alle Felder, welche noch keine Ziffer besitzen
		wieder geöffnet. Jetzt beginnt der ganze Vorgang wieder von vorne.
		Der ganze Prozess wird so oft wiederholt, bis jede Ziffer (von 1 bis 9) jeweils 9mal verteilt wurde.
		Allerdings kann es passieren, dass mitten im Vorgang keine freien Felder mehr verfügbar sind, da einfach durch
		Zufall die Zahlen unpraktisch verteilt wurden. In solch einem Fall wird einfach komplett von vorne begonnen und
		alle Felder wieder auf 0 gesetzt. Durchschnittlich ungefähr jedes vierte Sudoku wird erfolgreich generiert.
		Das Generieren eines kompletten Sudokus dauert ca. 10ms.
	*/

	/// <summary>
	/// 	Stellt ein Feld dar, also ein Kästchen, das zum Schluss eine Ziffer von 1 bis 9 enthält.
	/// </summary>
	class Field
	{
		private int value;
		private bool isClosed;

		/*
			Parameter isClosed wird öfters während des Generierungprozesses wieder auf false gesetzt.
			Wurde eine passende Ziffer (-> value) gefunden, bleibt isClosed dauerhaft auf true.
		*/
		public Field(int value, bool isClosed)
		{
			this.value = value;
			this.isClosed = isClosed;
		}

		public int GetValue()
		{
			return this.value;
		}

		public void SetValue(int value)
		{
			this.value = value;
		}

		public bool IsClosed()
		{
			return this.isClosed;
		}

		public void SetClosed(bool isClosed)
		{
			this.isClosed = isClosed;
		}

		public static implicit operator string(Field f)
		{
			return Convert.ToString(f.GetValue());
		}
	}

	/// <summary>
	/// 	Stellt eine Zeile des Sudokus dar. Ein Sudoku besitzt also 9 davon.
	/// </summary>
	class Row
	{
		private Field[] fields = new Field[9];

		/*
			Eine leere Zeile wird generiert, welche ausschließlich aus Feldern besteht,
			welche die Ziffer 0 als Wert tragen.
		*/
		public Row()
		{
			for (int i = 0; i < 9; i++)
			{
				this.fields[i] = new Field(0, false);
			}
		}

		public Field[] GetFields()
		{
			return this.fields;
		}

		public void SetClosed(bool cl)
		{
			foreach (Field f in this.fields)
			{
				f.SetClosed(cl);
			}
		}

		// nötig für Vergleich verschiedener Zeilen (siehe unten)
		public int GetNumberOfClosedFields()
		{
			int numberOfCF = 0;
			foreach (Field f in this.fields)
			{
				if (f.IsClosed())
				{
					numberOfCF++;
				}
			}
			return numberOfCF;
		}

		private ArrayList GetOpenedFields()
		{
			ArrayList of = new ArrayList();
			foreach (Field f in this.fields)
			{
				if (!f.IsClosed())
				{
					of.Add(f);
				}
			}
			return of;
		}

		/*
			Falls der Algorithmus entscheidet, dass nun in dieser Zeile eine Ziffer gesetzt werden soll,
			soll diese Funktion ein zufälliges freies Feld werfen.
		*/
		public Field GetRandomOpenedField()
		{
			ArrayList openFields = this.GetOpenedFields();
			return (Field)openFields[new Random().Next(0, openFields.Count)];
		}
	}

	/// <summary>
	/// 	Die eigentliche Sudoku Klasse, welche alle wichtigen Methoden bereitstellt, ein Sudoku zufällig generieren zu können.
	/// </summary>
	class Sudoku
	{
		

		private Row[] rows = new Row[9];

		public Sudoku()
		{
			for (int i = 0; i < 9; i++)
			{
				this.rows[i] = new Row();
			}
		}

	
	
		public Row[] GetRows()
		{
			return this.rows;
		}

		/// <summary>
		/// 	Diese Methode berechnet jeweils den Index für die Zeilen, welche die wenigsten freien Felder haben.
		/// </summary>
		/// <return>
		///		Eine ArrayList, in der als Integer gespeicherte Indexe zu finden sind.
		/// </return>
		public ArrayList GetMostClosedRowIndexes()
		{
			ArrayList indexes = new ArrayList();
			int biggestNumberOfClosedRows = 0;

			// Zunächst wird die größte Anzahl an geschlossenen Felder herausgefunden
			foreach (Row r in this.rows)
			{
				int nofclf = r.GetNumberOfClosedFields();
				if (nofclf > biggestNumberOfClosedRows && nofclf < 9)
				{
					biggestNumberOfClosedRows = r.GetNumberOfClosedFields();
				}
			}

			// Nun werden alle Zeilen (eigentlich nur deren Index) der ArrayList hinzugefügt, welche 
			// die zuvor berechnete Anzahl an geschlossenen Felder besitzen.
			for (int i = 0; i < 9; i++)
			{
				if (this.rows[i].GetNumberOfClosedFields() == biggestNumberOfClosedRows)
				{
					indexes.Add(i);
				}
			}
			return indexes;
		}

		public int this[int row, int column]
		{
			get
			{
				return this.rows[row].GetFields()[column].GetValue();
			}

            set
            {
				rows[row].GetFields()[column].SetValue(value);

            }
		}

		/// <summary>
		/// 	Die Felder der Zeile, der Spalte und des 3x3 Blocks der gesetzten Zahl müssen nun geschlossen werden.
		/// </summary>
		/// <param name="rowIndex">Integer</param>
		/// <param name="columnIndex">Integer</param>
		private void UpdateSudoku(int rowIndex, int columnIndex)
		{
			// Selbe Zeile der gesetzten Zahl muss geschlossen werden
			this.rows[rowIndex].SetClosed(true);

			// Selbe Spalte der gesetzten Zahl muss geschlossen werden
			foreach (Row r in this.rows)
			{
				r.GetFields()[columnIndex].SetClosed(true);
			}
			/*
				Block-Ecken (hier als O dargestellt)

				OXX OXX OXX
				XXX XXX XXX
				XXX XXX XXX

				OXX OXX OXX
				XXX XXX XXX
				XXX XXX XXX

				OXX OXX OXX
				XXX XXX XXX
				XXX XXX XXX

				O-Positionen zu finden:
			*/
			int startRowIndex = -1;
			int startColumnIndex = -1;

			if (0 <= rowIndex && rowIndex <= 2)
			{
				startRowIndex = 0;
			}
			else if (3 <= rowIndex && rowIndex <= 5)
			{
				startRowIndex = 3;
			}
			else if (6 <= rowIndex && rowIndex <= 8)
			{
				startRowIndex = 6;
			}

			if (0 <= columnIndex && columnIndex <= 2)
			{
				startColumnIndex = 0;
			}
			else if (3 <= columnIndex && columnIndex <= 5)
			{
				startColumnIndex = 3;
			}
			else if (6 <= columnIndex && columnIndex <= 8)
			{
				startColumnIndex = 6;
			}

			// Schließe gefundenen 3x3 Block
			for (int i = startRowIndex; i < startRowIndex + 3; i++)
			{
				for (int j = startColumnIndex; j < startColumnIndex + 3; j++)
				{
					this.rows[i].GetFields()[j].SetClosed(true);
				}
			}
		}

		// Methode wird pro Sudoku-Generierung 9mal aufgerufen (jeweils nach 9maliges setzen einer Ziffer)
		private void OpenFieldsWithNoNumbers()
		{
			foreach (Row r in this.rows)
			{
				foreach (Field f in r.GetFields())
				{
					if (f.GetValue() == 0)
					{
						f.SetClosed(false);
					}
				}
			}
		}

		// Setze alle Felder auf 0, damit z.B. komplett neu mit dem Generieren begonnen werden kann.
		private void Clear()
		{
			foreach (Row r in this.rows)
			{
				foreach (Field f in r.GetFields())
				{
					f.SetClosed(false);
					f.SetValue(0);
				}
			}
		}

		// Finde Index eines Feldes
		private int[] GetIndexOfField(Field f)
		{
			int r = 0;  // row
			foreach (Row row in this.rows)
			{
				int c = 0;  // column
				foreach (Field field in row.GetFields())
				{
					if (field == f)
					{
						return new int[2] { r, c };
					}
					c++;
				}
				r++;
			}
			return new int[2] { -1, -1 };
		}

		// Ein Versuch, das Sudoku zufällig zu generieren
		private bool GenerateTry()
		{
			for (int number = 1; number <= 9; number++)
			{
				for (int i = 0; i < 9; i++)
				{
					ArrayList mostClosedRowIndexes = this.GetMostClosedRowIndexes();

					// Falls Zahlen dumm generiert wurden, kommt es dazu, dass dieser Versuch abgebrochen werden muss
					if (mostClosedRowIndexes.Count == 0)
					{
						return false;
					}

					// Wähle zufällig eine Zeile der erlaubten Zeilen
					Row crow = this.rows[(int)mostClosedRowIndexes[new Random().Next(0, mostClosedRowIndexes.Count)]];
					// Wähle zufällig ein offenes Feld
					Field fd = crow.GetRandomOpenedField();
					// Stopfe dieses mit der aktuellen Ziffer
					fd.SetValue(number);
					// Schließe Feld, damit dies nicht mehr als besetzbar wahrgenommen wird
					fd.SetClosed(true);
					// Suche Index dieses Feldes, um danach die jeweiligen Spalten, Zeilen und den 3x3 Block für diese Ziffer schließen zu können
					int[] index = this.GetIndexOfField(fd);

					this.UpdateSudoku(index[0], index[1]);
				}
				// Öffne alle Felder wieder, welche noch keine Ziffer besitzen, da nun die Hauptziffer, welche dann 9mal vergeben wird, geändert wird
				this.OpenFieldsWithNoNumbers();
			}
			return true;
		}

		// Wichtigste Methode, welche das Sudoku zufällig generiert.
		// - kann unendlich oft hintereinander ausgeführt werden, da sowieso immer vor der Generierung alle Felder auf 0 gesetzt werden.
		public void Generate()
		{
			this.Clear();
			while (!this.GenerateTry())
			{
				this.Clear();
			}
		}


		// Funktion ermöglicht sauberes Darstellen des Sudokus z.B. im Terminal.
		// Sie ist dann in der eigentlichen App nicht mehr zu gebrauchen.
		public static implicit operator string(Sudoku s)
		{
			string toPrint = "";
			int i = 1;
			foreach (Row r in s.GetRows())
			{
				int j = 1;
				foreach (Field f in r.GetFields())
				{
					if (j % 3 == 0)
					{
						toPrint += f + " ";
					}
					else
					{
						toPrint += f;
					}
					j++;
				}
				toPrint += "\n";
				if (i % 3 == 0)
				{
					toPrint += "\n";
				}
				i++;
			}
			return toPrint;
		}

		public static string[,] generatedField(Sudoku s)
		{
			string[,] array = new string[9, 9];

			int i = 0;

			foreach (Row r in s.GetRows())
			{
				int j = 0;

				foreach (Field f in r.GetFields())
				{
					array[i, j] = f;

					j++;
				}
				i++;
			}

			return array;
		}

		public void ClearRandomFields(int amount)
		{
			ArrayList indexes = new ArrayList();
			for (int i = 0; i < amount; i++)
			{
				int[] index = new int[2];
				do
				{
					index[0] = new Random().Next(0, 9);
					index[1] = new Random().Next(0, 9);
				} while (InArray(indexes, index));
				indexes.Add(index);
			}
			foreach (int[] x in indexes)
			{
				this.rows[x[0]].GetFields()[x[1]].SetValue(0);
			}
		}

		// prüft alle Zeilen ob richtig
		private bool AreRowsCorrect()
		{
			ArrayList rowNumbers = new ArrayList();
			foreach (Row r in this.rows)
			{
				foreach (Field f in r.GetFields())
				{
					if (rowNumbers.Contains(f.GetValue()) || f.GetValue() < 1 || f.GetValue() > 9)
					{
						return false;
					}
					rowNumbers.Add(f.GetValue());
				}
				rowNumbers.Clear();
			}
			return true;
		}

		// prüft alle Spalten ob richtig
		private bool AreColumsCorrect()
		{
			ArrayList columnNumbers = new ArrayList();
			for (int i = 0; i < 9; i++)
			{
				foreach (Row r in this.rows)
				{
					int n = r.GetFields()[i].GetValue();
					if (columnNumbers.Contains(n))
					{
						return false;
					}
					columnNumbers.Add(n);
				}
				columnNumbers.Clear();
			}
			return true;
		}

		// prüft 3x3 Block ob richtig
		private bool IsBlockCorrect(int startRowIndex, int startColumnIndex)
		{
			/*
				OXX OXX OXX
				XXX XXX XXX
				XXX XXX XXX

				OX ...

				-> dort wo O ist, soll Index hinzeigen!
			*/
			ArrayList blockNumbers = new ArrayList();
			for (int i = startRowIndex; i < startRowIndex + 3; i++)
			{
				for (int j = startColumnIndex; j < startColumnIndex + 3; j++)
				{
					int n = this.rows[i].GetFields()[j].GetValue();
					if (blockNumbers.Contains(n))
					{
						return false;
					}
					blockNumbers.Add(n);
				}
			}
			return true;
		}

		// prüft ob alle 3x3 Blöcke richtig sind
		private bool AreBlocksCorrect()
		{
			for (int x = 0; x <= 6; x += 3)
			{
				for (int y = 0; y <= 6; y += 3)
				{
					if (!this.IsBlockCorrect(x, y))
					{
						return false;
					}
				}
			}
			return true;
		}

		// prüft, ob Sudoku keine Nullen enthält und richtig ist
		public bool IsCorrect()
		{
			return this.AreRowsCorrect() && this.AreColumsCorrect() && this.AreBlocksCorrect();
		}

		private bool InArray(ArrayList ar, int[] index)
		{
			foreach (int[] element in ar)
			{
				if (element[0] == index[0] && element[1] == index[1])
				{
					return true;
				}
			}
			return false;
		}

	}

	//// Test-Klasse, welches ein zufälliges Sudoku generiert, die Zeit misst und dann ausgibt.
	//class Test
	//{
	//	public static void Main(string[] args)
	//	{
	//		Sudoku sud = new Sudoku();
	//		//long startTime = DateTime.Now.Millisecond;
	//		sud.Generate();
	//		//long endTime = DateTime.Now.Millisecond;
	//		//long passedTime = endTime - startTime;
	//		Console.WriteLine(sud);
	//		//Console.WriteLine("Zeit vergangen: " + Convert.ToString(passedTime) + "ms");
	//	}
	//}
}
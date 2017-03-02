using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;

namespace Spreadsheet_DHonig
{
    using SpreadSheetEngine;
    

    public partial class Form1 : Form
    {
        private static int numCols = 26;
        private static int numRows = 50;

        spreadSheet spreadObj = new spreadSheet(numRows,numCols);

        public Form1()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        } 

        private void Form1_Load(object sender, EventArgs e)
        {             
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            int i = 65;

            int j = 1;

            //Step 1: Programmatically create columns A - Z.
            for (i = 65; i < 91; i++)
            {
                char c = (char)i;
                dataGridView1.Columns.Add(Convert.ToChar(i).ToString(), Convert.ToChar(i).ToString());
            }

            //Step2: Programmatically create rows 1 - 50.
            for (j = 1; j <= 50; j++)
            {
                dataGridView1.Rows.Add();
            }

            //Adjust the header cell names.
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.HeaderCell.Value = (row.Index + 1).ToString();
            }

            //Subscribe each cell.
            for(i = 0; i < numRows; i++)
            {
                for(j = 0; j < numCols; j++)
                {
                    spreadObj.grid[i,j].PropertyChanged += spreadsheetpropChange;
                }
            }
        }

        private void spreadsheetpropChange(object sender, PropertyChangedEventArgs e)
        {
            int findRow = (sender as Cell).rowIndex;
            int findCol = (sender as Cell).columnIndex;
            dataGridView1.Rows[findRow].Cells[findCol].Value = (sender as Cell).Value;
        }

		private void button1_Click(object sender, EventArgs e)
		{
			ExpTree ex = new ExpTree("Devon+1+A1");


			//Demo functions
			int i = 0;

            for (i = 0; i < 50; i++)
            {
				spreadObj.grid[i, 1].Text = "This is cell B" + (i + 1).ToString();
			}

			Random rnd = new Random();

            for (i = 0; i < 50; i++)
            {
                int colRND = rnd.Next(2, 26);
                int rowRND = rnd.Next(0, 50);

                spreadObj.grid[rowRND, colRND].Text = "Hello World.";

            }

			//Set all text values within the A column to match the corresponding B column.
    //        for (i = 0; i < 50; i++)
    //        {
				//spreadObj.grid[i, 0].Text = "=D" + (i + 1).ToString();
    //        }
        }

        private void dataGridView1_CellBeginEdit_1(object sender, DataGridViewCellCancelEventArgs e)
        {
            int findRow = e.RowIndex;
            int findCol = e.ColumnIndex;

            //dataGridView1.Rows[findRow].Cells[findCol].Value = "123";
            dataGridView1.Rows[findRow].Cells[findCol].Value = spreadObj.grid[findRow, findCol].Text;
        }

        private void dataGridView1_CellEndEdit_1(object sender, DataGridViewCellEventArgs e)
        {
            int findRow = e.RowIndex;
            int findCol = e.ColumnIndex;

            spreadObj.grid[findRow, findCol].Text = (dataGridView1.Rows[findRow].Cells[findCol].Value.ToString());
        }
    }
}

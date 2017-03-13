using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;

//Devon Honig 11416685

namespace Spreadsheet_DHonig
{
    using SpreadSheetEngine;


    public partial class Form1 : Form
    {
        private static int numCols = 26;
        private static int numRows = 50;

        ColorDialog Colors = new ColorDialog();

        spreadSheet spreadObj = new spreadSheet(numRows, numCols);

        Stack<Cell> undoStack = new Stack<Cell>();
        Stack<Cell> redoStack = new Stack<Cell>();

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
            for (i = 0; i < numRows; i++)
            {
                for (j = 0; j < numCols; j++)
                {
                    spreadObj.grid[i, j].PropertyChanged += spreadsheetpropChange;
                }
            }
        }

        private void spreadsheetpropChange(object sender, PropertyChangedEventArgs e)
        {
            int findRow = (sender as Cell).rowIndex;
            int findCol = (sender as Cell).columnIndex;

            if (e.PropertyName == "Text" || e.PropertyName == "Value")
            {
                dataGridView1.Rows[findRow].Cells[findCol].Value = (sender as Cell).Value;
            }

            if (e.PropertyName == "Color")          //Color is passed as property change.
            {
                dataGridView1.Rows[findRow].Cells[findCol].Style.BackColor = System.Drawing.Color.FromArgb((int)(sender as Cell).BGColor);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
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

            undoStack.Push(spreadObj.grid[findRow, findCol]);
            spreadObj.grid[findRow, findCol].Text = (dataGridView1.Rows[findRow].Cells[findCol].Value.ToString()) + " "; //Space reinvokes property change in case text was not changed.

            //Update all cells that reference current cell.
            Cell tempCell = spreadObj.findCell( findRow, findCol);
            int i = 0;
            int j = 5;
            foreach (string x in tempCell.references)
            {
                findCol = Convert.ToInt32(x[0] - 65);
                findRow = Convert.ToInt32(x.Substring(1));
                //foreach (string y in tempCell1.referencedBy)
                //{
                //    dataGridView1.Rows[k].Cells[6].Value = y;
                //    k++;
                //}
                dataGridView1.Rows[i].Cells[j].Value = x;
                i++;
            }

            Cell tempCell1 = spreadObj.findCell(0, 0);
            int k = 0;
            foreach (string x in tempCell1.referencedBy)
            {
                dataGridView1.Rows[k].Cells[6].Value = x;
                k++;
            }

        }

        private void changeBackgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Colors.ShowDialog() == DialogResult.OK)
            {
                int colorInt = Colors.Color.ToArgb();

                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    Cell tempCell = spreadObj.findCell(dataGridView1.SelectedCells[i].RowIndex, dataGridView1.SelectedCells[i].ColumnIndex);
                    tempCell.BGColor = (uint)(colorInt);
                }
            }
            
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoStack.Count > 1)
            {
                undoStack.Pop();
                Cell newCell = undoStack.Pop();

                //dataGridView1.Rows[newCell.rowIndex].Cells[newCell.columnIndex].Value = newCell.Value;
                spreadObj.grid[newCell.rowIndex, newCell.columnIndex].Text = newCell.Text;
            }
        }
    }
}

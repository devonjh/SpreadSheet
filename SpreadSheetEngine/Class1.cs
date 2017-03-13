using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Text.RegularExpressions;

//Devon Honig 11416685


namespace SpreadSheetEngine
{
    public abstract class Cell : INotifyPropertyChanged
    {
        //Variables for requried properties which will later be utilized in the getters and setters.
        protected string cellText;
        protected string _Value;
        protected string _Text;
        protected uint _BGColor;
        readonly int _rowIndex;
        readonly int _columnIndex;

        public event PropertyChangedEventHandler PropertyChanged;

        private List<string> _references = new List<string>();            //List showing which cells are referenced by current cell.
        private List<string> _referencedBy = new List<string>();          //List showing which cells reference this current cell.

        //Cell class constructor
        public Cell()
        {
        }

        //Overloaded Cell class constructor which sets the rowIndex and columnIndex properties.
        public Cell(int rowI, int colsI)
        {
            _rowIndex = rowI;
            _columnIndex = colsI;
            Text = "";
            _Value = Text;
        }

        //rowIndex readonly property.
        public int rowIndex { get { return _rowIndex; } }

        //columnIndex readonly property.
        public int columnIndex { get { return _columnIndex; } }

        public List<string> references
        {
            get { return _references; } 
        }

        public List<string> referencedBy { get { return _referencedBy; } }

        //Value property
        public string Value
        {
            get
            {
                return _Value;
            }

            protected set
            {
                _Value = value;
                propChange("Value");
            }
        }

        public uint BGColor
        {
            get
            {
                return _BGColor;
            }

            set
            {
                if (_BGColor == value)
                {
                    propChange("Color");
                    return;
                }

                _BGColor = value;
                propChange("Color");
            }
        }

        //Text property which represents the actual text typed into the cell.
        public string Text
        {
            get
            {
                return _Text;
            }

            set
            {
                if (value != _Text)
                {
                    _Text = value;
                    //Value = _Text;
                    propChange("Text");
                }
            }
        }

        //PropertyChanged function
        public virtual void propChange(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

    }

    public class spreadSheet
    {
        //member variables for tracking cell index counts.
        public cellArray[,] grid;
        protected int rows;
        protected int columns;

        protected Stack<Cell> undoStack = new Stack<Cell>();        //Stack for undo functionality.

		public spreadSheet()
		{

		}

        private int countRows
        {
            get
            {
                return rows;
            }
        }

        private int countCols
        {
            get
            {
                return columns;
            }
        }

		//Class to create the 2D array.
		public class cellArray : Cell
		{
			public cellArray(int rowI, int colI) : base(rowI, colI)
			{
			}

			public static cellArray makeCell(int rowI, int colI)
			{
				return new cellArray(rowI, colI);
			}

			//function to set the value if the first text character is '='
			public void setVal(cellArray[,] grid)
			{
				string newText = this.Text;
				int findCol;
				int findRow;

				//The first character is '=' which means the current cell text will be set to the given cell text.
				if (newText != "")
				{
					if (newText[0] == '=')
					{
						//THIS IS WHERE EXPTREE WILL GO.

						findCol = Convert.ToInt32(newText[1] - 65);
						findRow = Convert.ToInt32(newText.Substring(2)) - 1;
						_Value = grid[findRow, findCol].Value;
					}

					else
					{
						_Value = Text;
					}
				}

				else
				{
					return;
				}
			}

            public string changeCellValue(string newString)
            {
                _Value = newString;          //Need to be able to invoke the setVal or iNotify here.
                return _Value;
            }
		}

        //cell property changed event handler.
        public event PropertyChangedEventHandler cellPropertyChanged;

        //Populate the 2d array with the cells.
        public spreadSheet(int rowNum, int colNum)
        {
            grid = new cellArray[rowNum, colNum];

            int i = 0;
            int j = 0;

            for (i = 0; i < rowNum; i++)
            {
                for (j = 0; j < colNum; j++)
                {
                    cellArray newCell = cellArray.makeCell(i, j);
                    grid[i, j] = newCell;
                    newCell.PropertyChanged += eventHandle;
                }
            }
        }

        //Function to find if the given cell exists.
        public Cell findCell(int rowI, int colI)
        {
            if (grid[rowI, colI] != null)
            {
                return grid[rowI, colI];
            }

            else
            {
                return null;
            }
        }

        //Property change functions which calls the value changes.
        public void funcellPropertyChanged(Cell c, string text)
        {
            PropertyChangedEventHandler eventHandle = cellPropertyChanged;

            if (eventHandle != null)
            {
                eventHandle(c, new PropertyChangedEventArgs(text));
            }
        }

        public void eventHandle(object sender, PropertyChangedEventArgs e)
        {
            Dictionary<string, double> newDict = new Dictionary<string, double>();

            if ("Text" == e.PropertyName)
            {
                cellArray c = (sender as cellArray);

                if (c.Text[0] != '=')
                {
                    c.changeCellValue(c.Text);
                }

                else
                {
                    string newText = c.Text;
                    ExpTree newTree = new ExpTree(newText.Substring(1));
                    c.references.Clear();
                    //findCol = Convert.ToInt32(newText[1] - 65);
                    //findRow = Convert.ToInt32(newText.Substring(2)) - 1;

                    newTree.popDict();

                    foreach (KeyValuePair<string, double> x in newTree.backUp)
                    {
                        int findCol = Convert.ToInt32(x.Key[0] - 65);
                        int findRow = Convert.ToInt32(x.Key.Substring(1)) - 1;
                        string cellName = x.Key;
                        if (!c.references.Contains(cellName))
                        {
                            c.references.Add(x.Key);
                        }
                        char col = (char)(c.columnIndex + 65);           //change to c related text.
                        char row = (char)(c.rowIndex + 48);
                        string currentCell = string.Format("{0}{1}", col, row);
                        Cell tempCell = findCell(findRow, findCol);
                        if (!tempCell.referencedBy.Contains(currentCell))
                        {
                            tempCell.referencedBy.Add(currentCell);
                        }
                        //string cellName = Convert.ToString(c.columnIndex + 65) + Convert.ToChar(c.rowIndex);
                        //tempCell.referencedBy.Add(cellName);

                        if ((grid[findRow, findCol].Value.ToString()) != ""){
                            newDict.Add(x.Key, Convert.ToDouble(grid[findRow, findCol].Value));
                        }

                        else
                        {
                            continue;
                        }
                    }

                    newTree.backUp.Clear();

                    foreach (KeyValuePair<string, double> x in newDict)
                    {
                        newTree.backUp.Add(x.Key, x.Value);
                    }

                    newTree.popM();

                    if (c.changeCellValue(newTree.Eval().ToString()) != "0"){
                        c.changeCellValue(newTree.Eval().ToString());
                    }

                    else
                    {
                        c.changeCellValue("");
                    }

                    //this.Text = grid[findRow, findCol].Value;//**
                }

                newDict.Clear();

                //c.setVal(this.grid);
                funcellPropertyChanged(sender as Cell, "CellValue");
            }

            //c.setVal(this.grid);
        }
	}

	public class ExpTree
	{
        public Dictionary<string, double> backUp = new Dictionary<string, double>();
        public static Dictionary<string, double> m_lookup = new Dictionary<string, double> { };
        List<string> uses = new List<string>();
        List<string> usedBy = new List<string>();

        private abstract class Node
		{
			public abstract double Eval();
		}

		/*private class Node //this is just like declaring a private variable
        {
            public ?? Data;
            Node Left, Right;

            public double Eval()
            {
                //if node is single char and +-* or / => eval op
                //else if node is numerical string => parse double
                //else lookup variable => return value
            }
        }*/

		private class ConstNode : Node
		{
			private double m_value;

			public ConstNode(double value)
			{
				m_value = value;
			}


			public override double Eval()
			{
				return m_value;
			}
		}

		private class OpNode : Node
		{
			private char m_op;
			private Node m_left, m_right;

			public OpNode(char op, Node childrenL, Node childrenR)
			{
				m_op = op;
				m_left = childrenL;
				m_right = childrenR;
			}

			public override double Eval()
			{
				double left = m_left.Eval();
				double right = m_right.Eval();
				switch (m_op)
				{
					case '+':
						return left + right;
					case '-':
						return left - right; //this might be wrong
					case '*':
						return left * right;
					case '/':
						return left / right;
						//etc
				}
				return 0;
			}
		}

		private class VarNode : Node
		{
			private string m_varName;

            public VarNode(string str)
			{
				this.m_varName = str;
                //Need to add value from grid[findRow,findCol].Value to dictionary.
            }

            public override double Eval()
			{
				if (m_lookup.ContainsKey(m_varName))
				{
					return m_lookup[m_varName];
				}

				else
				{
					m_lookup.Add(m_varName, 0);
					return m_lookup[m_varName];
				}
			}
		}


        public void popDict()
        {
            foreach (KeyValuePair<string,double> x in m_lookup)
            {
                backUp.Add(x.Key, x.Value);
            }
        }

        public void popM()
        {
            m_lookup.Clear();

            foreach (KeyValuePair<string, double> x in backUp)
            {
                m_lookup.Add(x.Key, x.Value);
            }
        }

		private Node m_root;

		public ExpTree(string exp)
		{
            //TODO :Parse the expression string and build the tree:

            //for next homework: support:
            //no parens, single operator:
            //"A1+47+654+Hello+2
            //54+275+98
            //6*7*8
            //"A2"

            //For HW6
            //2+3*4
            //(55-11) / 11
            m_lookup.Clear();
			this.m_root = Compile(exp);
		}

		private static Node Compile(string exp)
		{
			//find first operator:
			//build parent operator node:
			//parent.left = buildsimple before op char
			//parent.right = compile(after opchar)
			//return parent;

			//handle whitespace:
			exp = exp.Replace(" ", "");

			/*for (int i = exp.Length - 1; i >= 0; i--)		//Backwards for order 
			{
				switch (exp[i])
				{
					case '+':
					case '-':
					case '*':
					case '/':
						return new OpNode(exp[i], Compile(exp.Substring(0, i)), Compile(exp.Substring(i + 1)));
				}
			}*/

			//If first char is '(' and last char is a matching ')', remove parens.

			if (exp[0] == '(')
			{
				int counter = 1;
				for (int i = 1; i < exp.Length; i++)
				{
					if (exp[i] == ')')
					{ 
						counter--;
						if (counter == 0)
						{
							if (i == exp.Length - 1)
							{
								return Compile(exp.Substring(1, exp.Length-2));		//Run compile on expression without enclosing brackets.
							}
							else
							{
								break;
							}
						}
					}

					if (exp[i] == '(')		//Nested Parentheses. Increment Parentheses counter.
					{
						counter++;
					}
				}
			}

			int index = GetLowOpIndex(exp);		//Grab index of lowest operand node.

			if (index != -1)
			{
				return new OpNode(exp[index], Compile(exp.Substring(0, index)), Compile(exp.Substring(index + 1)));	//Return opnode with recursive compile call.
			}

			return BuildSimple(exp);
		}

		//only building one node from a string:
		private static Node BuildSimple(string term)
		{
			double num;
			//tries to parse the string and returns true if the term is a number.
			if (double.TryParse(term, out num))
			{
				return new ConstNode(num);
			}
            VarNode newVar = new VarNode(term);

            if (!m_lookup.ContainsKey(term))
            {
                m_lookup.Add(term, 0);
            }
            
    
            return newVar;
		}

		private void locateVar(string varName, double Val)
		{
		}

		public double Eval()
		{
			if (m_root != null)
			{
				return m_root.Eval();
			}
			else
			{
				return double.NaN;
			}
		}

		private static int GetLowOpIndex(string exp)
		{
			int parenCounter = 0;
			int index = -1;

			for (int i = exp.Length - 1; i > 0; i--)
			{
				switch (exp[i])
				{
					case ')':
						parenCounter--;
						break;
					case '(':
						parenCounter++;
						break;
					case '+':
					case '-':
						if (parenCounter == 0)
						{
							return i;
						}
						break;

					case '*':
					case '/':
						if (parenCounter == 0 && index == -1)
						{
							index = i;
						}
						break;
				}
			}

			return index;
		}

		public void setVar(string varName, double varValue)
		{
			if (m_lookup.ContainsKey(varName))
			{
				m_lookup[varName] = varValue;
			}
			else
			{
				m_lookup.Add(varName, varValue);
			}
		}

		public void clearDict()
		{
			m_lookup.Clear();
		}
	}
}


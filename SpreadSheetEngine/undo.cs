using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadSheetEngine
{
    interface iUndo
    {
        string actionType { get; set; }

        void AddUndo(Cell newCell);
    }
    
    class undoText : iUndo      //Undo text class that inherits iUndo interface.
    {
        Stack<Cell> textStack = new Stack<Cell>();

        public string actionType { get; set; }

        public undoText ()
        {
            
        }

        public void AddUndo(Cell newCell)
        {
            textStack.Push(newCell);                //Push the cell onto the stack.
        }
    }
}

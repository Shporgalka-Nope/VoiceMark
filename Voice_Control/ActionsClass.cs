using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voice_Control
{
    public static class CActions
    {
        public static List<CAction> ActionList = new List<CAction>();
        static CActions()
        {
            CAction newAction = new CAction(1, "Open an application (process.exe)", true);
            ActionList.Add(newAction);
            newAction = new CAction(2, "Open explorer (path)", true);
            ActionList.Add(newAction);
        }
    }

    public class CAction
    {
        public string Discription;
        public int ActionNum;
        public bool argRequired = false;

        public CAction(int num, string disc, bool arg)
        {
            Discription = disc;
            ActionNum = num;
            argRequired = arg;
        }
    }
}

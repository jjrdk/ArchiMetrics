using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ArchiCop.ViewModel
{
    public class GraphCommandViewModel : CommandViewModel
    {
        public GraphCommandViewModel(string displayName, params ICommand[] commands) : 
            base(displayName)
        {
            int i = 0;
            foreach (ICommand command in commands)
            {
                if (i==0)
                {
                    Command = command;                    
                }
                else if(i==1)
                {
                    Command1 = command;
                }
                i = i + 1;
            }
        }

        public ICommand Command1 { get; protected set; }
    }
}
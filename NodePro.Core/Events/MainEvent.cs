using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core.Events
{
    public class ChangePriceEvent : PubSubEvent { };

    public class ProductSectionChangedEvent : PubSubEvent { };

    public class DirtyEvent : PubSubEvent<bool> { };

    public class ExitEvent : PubSubEvent { };

    #region CloseEvent
    public class CloseEventRegister
    {
        public int Count => commmands.Count + asyncCommands.Count;

        private List<Func<bool>> commmands = new List<Func<bool>>();

        private List<Func<Task<bool>>> asyncCommands = new List<Func<Task<bool>>>();

        public void Register(Func<bool> command)
        {
            commmands.Add(command);
        }

        public void RegisterAsync(Func<Task<bool>> asyncCommand)
        {
            asyncCommands.Add(asyncCommand);
        }

        public async Task<bool> Execute()
        {
            if (Count == 0) return true;
            int uncompleted = 0;
            while (commmands.Count > 0)
            {
                bool state = commmands[0]();
                commmands.RemoveAt(0);
                if (!state)
                {
                    uncompleted++;
                }
            }

            while (asyncCommands.Count > 0)
            {
                bool state = await asyncCommands[0]();
                asyncCommands.RemoveAt(0);
                if (!state)
                {
                    uncompleted++;
                }
            }

            return uncompleted == 0;
        }
    }
    public class ClosingEvent : PubSubEvent<CloseEventRegister> { }

    #endregion
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui.Manager
{
    public class EventBus
    {
        public delegate void TextRequest(string text);

        public static EventBus getInstance()
        {
            if(instance == null)
            {
                instance = new EventBus();
            }
            return instance;
        }
        private static EventBus instance;
        private EventBus()
        {

        }
        public event TextRequest SearchRequest;
        public void Search(string text)
        {
            if(SearchRequest!=null)
            {
                SearchRequest(text);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

public class MultiFormContext : ApplicationContext
{
    public delegate void OnAllFormsClosed();
    public event OnAllFormsClosed AllFormsClosed;
    private List<Form> openForms = new List<Form> { };
    public Form[] Forms
    {
        get
        {
            return openForms.ToArray();
        }
    }
    public MultiFormContext(params Form[] forms)
    {
        foreach (var form in forms)
        {
            AddForm(form);
        }
    }
    public void AddForm(Form form)
    {
        form.FormClosed += (Object s, FormClosedEventArgs args) =>
        {
            //When we have closed the last of the "starting" forms, 
            //end the program.
            openForms.Remove((Form)s);
            ((Form)s).Dispose();

            if (openForms.Count == 0)
            {
                AllFormsClosed();
                ExitThread();
            }
        };

        openForms.Add(form);

        form.Show();
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchiffeVersenken
{
    public partial class SpielfeldControl : UserControl
    {
        public Feldkoordinate GewaehltesFeld { get; set; }

        public SpielfeldControl()
        {
            InitializeComponent();
        }

        public event EventHandler SpielfeldInitFinished;
    
    }
}

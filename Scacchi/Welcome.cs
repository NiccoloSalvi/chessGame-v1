using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scacchi
{
    public partial class Welcome : Form
    {
        public Welcome()
        {
            InitializeComponent();

            BackgroundImage = Image.FromFile("images/welcome.png"); // form's background image
            BackgroundImageLayout = ImageLayout.Stretch;

            Exit.Image = Image.FromFile("images/exit.png"); // load the button's image
            Exit.SizeMode = PictureBoxSizeMode.StretchImage;

            NewGame.Text = "START";
        }

        private void NewGame_Click(object sender, EventArgs e)
        {
            Hide(); // hide the welcome form
            Scacchiera newForm = new Scacchiera(ExpertMode.Checked);
            newForm.Show(); // show the chessboard form
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit(); // kill the application (even in the backgroud)
        }
    }
}

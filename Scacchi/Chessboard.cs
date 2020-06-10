using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace Scacchi
{
    public partial class Scacchiera : Form
    {
        private struct PiecesSize // struct which represents using in the dictionary
        {
            private readonly int w; // piece's width
            private readonly int h; // piece's height

            public PiecesSize(int wi, int he)
            {
                w = wi;
                h = he;
            }

            public int GetW()
            {
                return w;
            }

            public int GetH()
            {
                return h;
            }
        }

        private Dictionary<string, PiecesSize> piecesSize = new Dictionary<string, PiecesSize>() // dictionary which represents, for every piece, his initial size, based on the screen resolution
        {
            { "rook", new PiecesSize(70 * Screen.PrimaryScreen.Bounds.Width / 1920, 165 * Screen.PrimaryScreen.Bounds.Height / 1080) },
            { "knight", new PiecesSize(82 * Screen.PrimaryScreen.Bounds.Width / 1920, 165 * Screen.PrimaryScreen.Bounds.Height / 1080) },
            { "bishop", new PiecesSize(65 * Screen.PrimaryScreen.Bounds.Width / 1920, 165 * Screen.PrimaryScreen.Bounds.Height / 1080) },
            { "queen", new PiecesSize(54 * Screen.PrimaryScreen.Bounds.Width / 1920, 165 * Screen.PrimaryScreen.Bounds.Height / 1080) },
            { "king", new PiecesSize(60 * Screen.PrimaryScreen.Bounds.Width / 1920, 165 * Screen.PrimaryScreen.Bounds.Height / 1080) },
            { "pawn", new PiecesSize(50 * Screen.PrimaryScreen.Bounds.Width / 1920, 100 * Screen.PrimaryScreen.Bounds.Height / 1080) }
        };

        public struct ChessCell // struct which represents the chessboard
        {
            private readonly int yPiece; // picture's lower side
            private readonly int xPiece; // cell's centre
            private Piece piece; // pointer which represents the piece
            private PictureBox picture; // pointer which represents the picture
            private bool firstMove; // first move

            public ChessCell(int yP, int xP, Piece pie, PictureBox pic, bool fv)
            {
                yPiece = yP;
                xPiece = xP;
                piece = pie;
                picture = pic;
                firstMove = fv;
            }

            public void SetPicture(PictureBox pic)
            {
                picture = pic;
            }

            public void SetPosMatrix(int r, int c)
            {
                Piece.PosMatrix test = new Piece.PosMatrix(r, c);
            }

            public void SetPiece(Piece pie)
            {
                piece = pie;
            }

            public void SetFirstMove(bool fv)
            {
                firstMove = fv;
            }

            public int GetxPieces()
            {
                return xPiece;
            }

            public int GetyPieces()
            {
                return yPiece;
            }

            public PictureBox GetPicture()
            {
                return picture;
            }

            public Piece GetPiece()
            {
                return piece;
            }

            public bool GetFirstMove()
            {
                return firstMove;
            }
        }

        private bool eatenPiece; // represents if the piece has been eaten
        private bool turn; // represents the turns between two players
        private bool occ; // the mouse click selected a piece
        private bool ExpertMode; // if expert mode is selected
        private bool possibleCheck; // variable

        private int r; // row in chessboard matrix
        private int c; // column in chessboard matrix
        private int rNew;
        private int cNew;

        private static int arraySize = 25; // array size of pieces' possible moves

        private Piece.PosMatrix[] vett = new Piece.PosMatrix[GetArraySize()]; // array where are stored the possible positiona
        private PictureBox[] PossiblePositions = new PictureBox[GetArraySize()]; // array for pictures which represent the possible moves
        
        private int TimerCountdown; // timer duration in minute
        private int TimerTemp; // timer for player_1 
        private int TimerTemp2; // timer for player_2

        private bool[] WhitePiecesDied = new bool[47]; // all the black pieces died
        private int[] XWhitePiecesDied = { 12, 67, 134, 213, 277 }; // X positions for black pieces died
        private bool[] BlackPiecesDied = new bool[47]; // all the white pieces died
        private int[] XBlackPiecesDied = { 1853, 1781, 1705, 1639, 1582 }; // X position for white pieces died

        private ChessCell[,] ChessBoard = new ChessCell[8, 8]; // chessboard

        private int widthScreen; // width screen
        private int heightScreen; // height screen

        // arrocco variables
        private bool arrocco;
        private bool arroccoLungo;
        private bool arroccoCorto;

        // counters for players moves done
        private int CountMovesPL1;
        private int CountMovesPL2;

        // set the timers
        private void SetTimerCountdown(int t)
        {
            TimerCountdown = t * 60;
            TimerTemp = TimerCountdown;
            TimerTemp2 = TimerCountdown;
        }

        // get the array size
        private static int GetArraySize()
        {
            return arraySize;
        }

        public ChessCell[,] TempChessBoard = new ChessCell[8, 8] // matrix which represents the initial model of chessboard
        {
            { new ChessCell(884, 440, new Rook() { color = false, name = "rook", posMatrix = new Piece.PosMatrix(0, 0) }, null, true), new ChessCell(884, 586, new Knight() { color = false, name = "knight", posMatrix = new Piece.PosMatrix(0, 1) }, null, true), new ChessCell(884, 731, new Bishop() { color = false, name = "bishop", posMatrix = new Piece.PosMatrix(0, 2) }, null, true), new ChessCell(884, 877, new Queen() { color = false, name = "queen", posMatrix = new Piece.PosMatrix(0, 3) }, null, true), new ChessCell(884, 1023, new King() { color = false, name = "king", posMatrix = new Piece.PosMatrix(0, 4) }, null, true), new ChessCell(884, 1169, new Bishop() { color = false, name = "bishop", posMatrix = new Piece.PosMatrix(0, 5) }, null, true), new ChessCell(884, 1314, new Knight() { color = false, name = "knight", posMatrix = new Piece.PosMatrix(0, 6) }, null, true), new ChessCell(884, 1460, new Rook() { color = false, name = "rook", posMatrix = new Piece.PosMatrix(0, 7) }, null, true) },
            { new ChessCell(727, 461, new Pawn() { color = false, name = "pawn", posMatrix = new Piece.PosMatrix(1, 0) }, null, true), new ChessCell(727, 601,  new Pawn() { color = false, name = "pawn", posMatrix = new Piece.PosMatrix(1, 1) }, null, true), new ChessCell(727, 740,  new Pawn() { color = false, name = "pawn", posMatrix = new Piece.PosMatrix(1, 2) }, null, true), new ChessCell(727, 880, new Pawn() { color = false, name = "pawn", posMatrix = new Piece.PosMatrix(1, 3) }, null, true), new ChessCell(727, 1020,  new Pawn() { color = false, name = "pawn", posMatrix = new Piece.PosMatrix(1, 4) }, null, true), new ChessCell(727, 1160,  new Pawn() { color = false, name = "pawn", posMatrix = new Piece.PosMatrix(1, 5) }, null, true), new ChessCell(727, 1299,  new Pawn() { color = false, name = "pawn", posMatrix = new Piece.PosMatrix(1, 6) }, null, true), new ChessCell(727, 1439,  new Pawn() { color = false, name = "pawn", posMatrix = new Piece.PosMatrix(1, 7) }, null, true) },
            { new ChessCell(619, 486, null, null, false), new ChessCell(619, 619, null, null, false), new ChessCell(619, 751, null, null, false), new ChessCell(619, 884, null, null, false), new ChessCell(619, 1016, null, null, false), new ChessCell(619, 1149, null, null, false), new ChessCell(619, 1281, null, null, false), new ChessCell(619, 1414, null, null, false) },
            { new ChessCell(476, 510, null, null, false), new ChessCell(476, 636, null, null, false), new ChessCell(476, 761, null, null, false), new ChessCell(476, 887, null, null, false), new ChessCell(476, 1013, null, null, false), new ChessCell(476, 1139, null, null, false), new ChessCell(476, 1264, null, null, false), new ChessCell(476, 1390, null, null, false) },
            { new ChessCell(370, 527, null, null, false), new ChessCell(370, 648, null, null, false), new ChessCell(370, 769, null, null, false), new ChessCell(370, 890, null, null, false), new ChessCell(370, 1011, null, null, false), new ChessCell(370, 1132, null, null, false), new ChessCell(370, 1253, null, null, false), new ChessCell(370, 1374, null, null, false) },
            { new ChessCell(273, 542, null, null, false), new ChessCell(273, 659, null, null, false), new ChessCell(273, 775, null, null, false), new ChessCell(273, 892, null, null, false), new ChessCell(273, 1008, null, null, false), new ChessCell(273, 1125, null, null, false), new ChessCell(273, 1241, null, null, false), new ChessCell(273, 1358, null, null, false) },
            { new ChessCell(195, 556, new Pawn() { color = true, name = "pawn", posMatrix = new Piece.PosMatrix(6, 0) }, null, true), new ChessCell(195, 669,  new Pawn() { color = true, name = "pawn", posMatrix = new Piece.PosMatrix(6, 1) }, null, true), new ChessCell(195, 781,  new Pawn() { color = true, name = "pawn", posMatrix = new Piece.PosMatrix(6, 2) }, null, true), new ChessCell(195, 894, new Pawn() { color = true, name = "pawn", posMatrix = new Piece.PosMatrix(6, 3) }, null, true), new ChessCell(195, 1006,  new Pawn() { color = true, name = "pawn", posMatrix = new Piece.PosMatrix(6, 4) }, null, true), new ChessCell(195, 1119,  new Pawn() { color = true, name = "pawn", posMatrix = new Piece.PosMatrix(6, 5) }, null, true), new ChessCell(195, 1231,  new Pawn() { color = true, name = "pawn", posMatrix = new Piece.PosMatrix(6, 6) }, null, true), new ChessCell(195, 1344,  new Pawn() { color = true, name = "pawn", posMatrix = new Piece.PosMatrix(6, 7) }, null, true) },
            { new ChessCell(115, 593, new Rook() { color = true, name = "rook", posMatrix = new Piece.PosMatrix(7, 0) }, null, true), new ChessCell(115, 699, new Knight() { color = true, name = "knight", posMatrix = new Piece.PosMatrix(7, 1) }, null, true), new ChessCell(115, 803, new Bishop() { color = true, name = "bishop", posMatrix = new Piece.PosMatrix(7, 2) }, null, true), new ChessCell(115, 907, new Queen() { color = true, name = "queen", posMatrix = new Piece.PosMatrix(7, 3) }, null, true), new ChessCell(115, 1018, new King() { color = true, name = "king", posMatrix = new Piece.PosMatrix(7, 4) }, null, true), new ChessCell(115, 1123, new Bishop() { color = true, name = "bishop", posMatrix = new Piece.PosMatrix(7, 5) }, null, true), new ChessCell(115, 1238, new Knight() { color = true, name = "knight", posMatrix = new Piece.PosMatrix(7, 6) }, null, true), new ChessCell(115, 1343, new Rook() { color = true, name = "rook", posMatrix = new Piece.PosMatrix(7, 7) }, null, true) }
        };

        private bool GetPosMatrix(int x, int y, ref int r, ref int c) // return positions in the matrix, based on the mouse click position
        {
            if (y >= 820 && y < 965)
            {
                if (x >= 400 && x < 530) c = 0;
                if (x >= 530 && x < 670) c = 1;
                if (x >= 670 && x < 810) c = 2;
                if (x >= 810 && x < 955) c = 3;
                if (x >= 955 && x < 1100) c = 4;
                if (x >= 1100 && x < 1240) c = 5;
                if (x >= 1240 && x < 1385) c = 6;
                if (x >= 1385 && x < 1525) c = 7;
                r = 0;
            }

            if (y >= 680 && y < 820)
            {
                if (x >= 430 && x < 550) c = 0;
                if (x >= 550 && x < 685) c = 1;
                if (x >= 685 && x < 820) c = 2;
                if (x >= 820 && x < 955) c = 3;
                if (x >= 955 && x < 1090) c = 4;
                if (x >= 1090 && x < 1220) c = 5;
                if (x >= 1220 && x < 1360) c = 6;
                if (x >= 1360 && x < 1485) c = 7;
                r = 1;
            }

            if (y >= 560 && y < 680)
            {
                if (x >= 450 && x < 570) c = 0;
                if (x >= 570 && x < 695) c = 1;
                if (x >= 695 && x < 825) c = 2;
                if (x >= 825 && x < 950) c = 3;
                if (x >= 950 && x < 1085) c = 4;
                if (x >= 1085 && x < 1215) c = 5;
                if (x >= 1215 && x < 1340) c = 6;
                if (x >= 1340 && x < 1460) c = 7;
                r = 2;
            }

            if (y >= 445 && y < 560)
            {
                if (x >= 470 && x < 585) c = 0;
                if (x >= 585 && x < 705) c = 1;
                if (x >= 705 && x < 830) c = 2;
                if (x >= 830 && x < 950) c = 3;
                if (x >= 950 && x < 1080) c = 4;
                if (x >= 1080 && x < 1200) c = 5;
                if (x >= 1200 && x < 1325) c = 6;
                if (x >= 1325 && x < 1440) c = 7;
                r = 3;
            }

            if (y >= 345 && y < 445)
            {
                if (x >= 490 && x < 600) c = 0;
                if (x >= 600 && x < 715) c = 1;
                if (x >= 715 && x < 830) c = 2;
                if (x >= 830 && x < 955) c = 3;
                if (x >= 955 && x < 1070) c = 4;
                if (x >= 1070 && x < 1190) c = 5;
                if (x >= 1190 && x < 1310) c = 6;
                if (x >= 1310 && x < 1420) c = 7;
                r = 4;
            }

            if (y >= 250 && y < 345)
            {
                if (x >= 510 && x < 615) c = 0;
                if (x >= 615 && x < 720) c = 1;
                if (x >= 720 && x < 840) c = 2;
                if (x >= 840 && x < 950) c = 3;
                if (x >= 950 && x < 1070) c = 4;
                if (x >= 1070 && x < 1180) c = 5;
                if (x >= 1180 && x < 1295) c = 6;
                if (x >= 1295 && x < 1400) c = 7;
                r = 5;
            }

            if (y >= 165 && y < 250)
            {
                if (x >= 525 && x < 625) c = 0;
                if (x >= 625 && x < 735) c = 1;
                if (x >= 735 && x < 845) c = 2;
                if (x >= 845 && x < 950) c = 3;
                if (x >= 950 && x < 1060) c = 4;
                if (x >= 1060 && x < 1175) c = 5;
                if (x >= 1175 && x < 1280) c = 6;
                if (x >= 1280 && x < 1385) c = 7;
                r = 6;
            }

            if (y >= 30 && y < 165)
            {
                if (x >= 540 && x < 640) c = 0;
                if (x >= 640 && x < 740) c = 1;
                if (x >= 740 && x < 850) c = 2;
                if (x >= 850 && x < 950) c = 3;
                if (x >= 950 && x < 1060) c = 4;
                if (x >= 1060 && x < 1160) c = 5;
                if (x >= 1160 && x < 1270) c = 6;
                if (x >= 1270 && x < 1370) c = 7;
                r = 7;
            }

            if (c != -1) return true; // if columns is different to the initial one, return true
            return false;
        }

        public Scacchiera(bool val)
        {
            InitializeComponent();

            widthScreen = Screen.PrimaryScreen.Bounds.Width; // get width of the screen
            heightScreen = Screen.PrimaryScreen.Bounds.Height; // get heghit of the screen

            Size = new Size(widthScreen, heightScreen); // new form's size, based on the resolution

            PictureBox[] numbers = { N1, N2, N3, N4, N5, N6, N7, N8 }; // represents the numbers on the chessboard, if the expert mode is selected
            PictureBox[] letters = { A, B, CI, D, E, F, G, H }; // represents the letters on the chessboard, if the expert mode is selected

            ExpertMode = val; // expert mode selected in the welcome form

            if (ExpertMode) // if expert mode has been selected
            {

                Visualize.Image = Image.FromFile("images/visualize.png"); // new button that visualize the moves done for each player
                Visualize.Size = new Size(37 * widthScreen / 1920, 39 * heightScreen / 1080); // button size
                Visualize.Location = new Point(434 * widthScreen / 1920, 66 * heightScreen / 1080); // button location
                Visualize.Enabled = true; // enable the button
                Visualize.Visible = true; // visualize the button

                // enable the two listboxes which represents the moves done by the player
                MovesPL1.Enabled = true;
                MovesPL2.Enabled = true;

                int xIni = 335 * widthScreen / 1920; // xIni for the position of letters on the chessboard
                for (int i = 0; i < 8; i++)
                {
                    numbers[i].Image = Image.FromFile("images/numbers/" + (i + 1).ToString() + ".png"); // load the right image for each number on the chessboard
                    numbers[i].Size = new Size(9 * widthScreen / 1920, 18 * heightScreen / 1080); // new size, based on the screen resolution

                    letters[i].Image = Image.FromFile("images/letters/" + letters[i].Name.ToLower() + ".png"); // load the right image for each letter on the chessboard
                    letters[i].Size = new Size(13 * widthScreen / 1920, 20 * heightScreen / 1080); // new size, based on the screen resolution

                    xIni += 147 * widthScreen / 1920; // for each letter, increment the position of 147 (based on the screen resolution)
                    letters[i].Location = new Point(xIni, 900 * heightScreen / 1080); // new location of letters
                }

                // new locations for the numbers, based on the screen resolution
                N1.Location = new Point(399 * widthScreen / 1920, 811 * heightScreen / 1080);
                N2.Location = new Point(422 * widthScreen / 1920, 673 * heightScreen / 1080);
                N3.Location = new Point(443 * widthScreen / 1920, 555 * heightScreen / 1080);
                N4.Location = new Point(463 * widthScreen / 1920, 439 * heightScreen / 1080);
                N5.Location = new Point(480 * widthScreen / 1920, 338 * heightScreen / 1080);
                N6.Location = new Point(499 * widthScreen / 1920, 243 * heightScreen / 1080);
                N7.Location = new Point(516 * widthScreen / 1920, 157 * heightScreen / 1080);
                N8.Location = new Point(534 * widthScreen / 1920, 71 * heightScreen / 1080);

                MovesPL1.Size = new Size(120 * widthScreen / 1920, 979 * heightScreen / 1080); // new size of listbox
                MovesPL1.Location = new Point(12 * widthScreen / 1920, 14 * heightScreen / 1080); // new location
                MovesPL1.Font = new Font("Microsoft Sans Serif", 15 * widthScreen / 1920); // new font, based on the screen resolution

                MovesPL2.Size = MovesPL1.Size; // new size of listbox, the same of the first one
                MovesPL2.Location = new Point(1788 * widthScreen / 1920, 14 * heightScreen / 1080); // new location
                MovesPL2.Font = MovesPL1.Font; // new font, based on the screen resolution, the same for the first one

                SetTimerCountdown(60); // set the timer for 1 hour
                
                // reset counters for player moves done
                CountMovesPL1 = 1;
                CountMovesPL2 = 1;
            }
            else // if expert mode has not been selected
            {
                for (int i = 0; i < 8; i++) // disable letters and numbers
                {
                    numbers[i].Enabled = false;
                    numbers[i].Visible = false;
                    letters[i].Enabled = false;
                    letters[i].Visible = false;
                }

                SetTimerCountdown(5); // set timer for 5 minutes
            }

            BackgroundImage = Image.FromFile("images/chessboard.png"); // background of form
            BackgroundImageLayout = ImageLayout.Stretch; // stretch layout for the background

            Exit.Image = Image.FromFile("images/exit.png"); // image of button to exit
            Exit.SizeMode = PictureBoxSizeMode.StretchImage; // stretch mode for the picturebox
            Exit.Location = new Point(405 * widthScreen / 1920, 21 * heightScreen / 1080); // button location, based on screen resolution
            Exit.Size = new Size(37 * widthScreen / 1920, 39 * heightScreen / 1080); // button size

            NewGame.Image = Image.FromFile("images/reload.png"); // image of button to restart the game
            NewGame.SizeMode = PictureBoxSizeMode.StretchImage; // stretch mode for the picturebox
            NewGame.Location = new Point(446 * widthScreen / 1920, 14 * heightScreen / 1080); // button location, based on screen resolution
            NewGame.Size = new Size(50 * widthScreen / 1920, 50 * heightScreen / 1080); // button size

            MouseClick += mouseClick; // add the mouse click
            occ = false; // no piece selected
            turn = false; // turn for the first user, white user

            // color based on the turn
            if (turn) CurrentTurn.BackColor = Color.Black;
            else CurrentTurn.BackColor = Color.White;

            TimerVisual.Size = new Size(110 * widthScreen / 1920, 46 * heightScreen / 1080); // new size of textbox
            TimerVisual.Location = new Point(1406 * widthScreen / 1920,  14 * heightScreen / 1080); // new location of textbox
            TimerVisual.Font = new Font("Microsoft Sans Serif", 26 * widthScreen / 1920); // new font of textbox

            TimerPlayed.Interval = TimerCountdown * 1000; // timer

            TimerProgressBar.Size = new Size(162 * widthScreen / 1920, 23 * heightScreen / 1080); // new size of progress bar
            TimerProgressBar.Location = new Point(1406 * widthScreen / 1920, 66 * heightScreen / 1080); // new location of progress bar
            TimerProgressBar.Maximum = TimerCountdown; // maximum for progress bar
            TimerProgressBar.Value = TimerCountdown; // value for progress bar

            CurrentTurn.Size = new Size(46 * widthScreen / 1920, 46 * heightScreen / 1080); // size for colored textbox, which represents the current turn
            CurrentTurn.Location = new Point(1522 * widthScreen / 1920, 14 * heightScreen / 1080); // new location for colored textbox
            
            for (int i = 0; i < 46; i++) // initialize the arrays of pieces died
            {
                BlackPiecesDied[i] = false;
                WhitePiecesDied[i] = false;
            }
        }

        // the form cannot be moved in other positions
        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;

            switch (m.Msg)
            {
                case WM_SYSCOMMAND:
                    int command = m.WParam.ToInt32() & 0xfff0;
                    if (command == SC_MOVE)
                        return;
                    break;
            }
            base.WndProc(ref m);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int i = 0; // index
            ChessBoard = (ChessCell[,])TempChessBoard.Clone(); // create an indipendent copy of the initial chessboard

            // arrays of picturebox, which represents pieces/pawns black/white
            PictureBox[] piecesBlack = { RookBlack_sx, KnightBlack_sx, BishopBlack_sx, QueenBlack, KingBlack, BishopBlack_dx, KnightBlack_dx, RookBlack_dx };
            PictureBox[] piecesWhite = { RookWhite_sx, KnightWhite_sx, BishopWhite_sx, QueenWhite, KingWhite, BishopWhite_dx, KnightWhite_dx, RookWhite_dx };
            PictureBox[] pawnsBlack = { PawnBlack_0, PawnBlack_1, PawnBlack_2, PawnBlack_3, PawnBlack_4, PawnBlack_5, PawnBlack_6, PawnBlack_7 };
            PictureBox[] pawnsWhite = { PawnWhite_7, PawnWhite_6, PawnWhite_5, PawnWhite_4, PawnWhite_3, PawnWhite_2, PawnWhite_1, PawnWhite_0 };

            // fill the matrix
            for (i = 0; i < 8; i++)
            {
                ChessBoard[0, i].SetPicture(piecesWhite[i]); // first line
                ChessBoard[1, i].SetPicture(pawnsWhite[i]); // second line
                ChessBoard[6, i].SetPicture(pawnsBlack[i]); // sixth line
                ChessBoard[7, i].SetPicture(piecesBlack[i]); // seventh line
            }
            
            PiecesSize temp = new PiecesSize(); // using a temp struct
            string[] pieces = { "rook", "knight", "bishop", "queen", "king", "bishop", "knight", "rook" }; // array of chess pieces
            float centre; // represents the cell's X in the midle

            for (i = 0; i < 8; i++) // first line of chessboard (pieces white)
            {
                string piece = pieces[i]; // select the piece
                temp = piecesSize[piece]; // piece's dimension
                centre = ChessBoard[0, i].GetxPieces() * widthScreen / 1920; // cell's centre
                
                piecesWhite[i].Size = new Size(temp.GetW(), temp.GetH()); // new size for the piece
                piecesWhite[i].Location = new Point((int)centre - (temp.GetW() / 2), (ChessBoard[0, i].GetyPieces() * heightScreen / 1080) - piecesWhite[i].Size.Height); // new location for the piece
                piecesWhite[i].BackColor = Color.Transparent; // background transparent
                piecesWhite[i].Image = Image.FromFile("pieces/white/" + pieces[i] + ".png"); // load the image
                piecesWhite[i].SizeMode = PictureBoxSizeMode.StretchImage; // stretch mode for the picturebox
                Controls.Add(piecesWhite[i]); // add to the form
                piecesWhite[i].BringToFront(); // bring to front
            }

            temp = piecesSize["pawn"]; // using pawn piece
            for (i = 0; i < 8; i++) // second line of chessboard (pawns white)
            {
                centre = ChessBoard[1, i].GetxPieces() * widthScreen / 1920; // cell's centre

                pawnsWhite[i].Size = new Size(temp.GetW(), temp.GetH()); // new size for the piece
                pawnsWhite[i].Location = new Point((int)centre - (temp.GetW() / 2), ChessBoard[1, i].GetyPieces() * heightScreen / 1080 - pawnsWhite[i].Size.Height); // new location for the piece
                pawnsWhite[i].BackColor = Color.Transparent; // background transparent
                pawnsWhite[i].Image = Image.FromFile("pieces/white/pawn.png"); // load the image
                pawnsWhite[i].SizeMode = PictureBoxSizeMode.StretchImage; // stretch mode for the picturebox
                Controls.Add(pawnsWhite[i]); // add to the form
            }

            temp = piecesSize["pawn"]; // using pawn piece
            double x = Math.Pow(0.952f, 6); // for every cell, the size's picture decrease of 10%

            // using local variables, beacuse size of pawns are exactly the same
            int w = (int)(temp.GetW() * x);
            int h = (int)(temp.GetH() * x);

            for (i = 0; i < 8; i++) // sixth line (pawns black)
            {
                centre = ChessBoard[6, i].GetxPieces() * widthScreen / 1920; // cell's centre

                pawnsBlack[i].Size = new Size(w, h); // new size for the piece
                pawnsBlack[i].Location = new Point((int)centre - (w / 2), ChessBoard[6, i].GetyPieces() * heightScreen / 1080 - pawnsBlack[i].Size.Height); // new location for the piece
                pawnsBlack[i].BackColor = Color.Transparent; // background transparent
                pawnsBlack[i].Image = Image.FromFile("pieces/black/pawn.png"); // load the image
                pawnsBlack[i].SizeMode = PictureBoxSizeMode.StretchImage; // stretch mode for the picturebox
                Controls.Add(pawnsBlack[i]); // add to the form
                pawnsBlack[i].BringToFront(); // bring to front
            }

            x = Math.Pow(0.952f, 7); // for every cell, the size's picture decrease of 10%
            for (i = 7; i >= 0; i--) // seventh line (pieces black)
            {
                string piece = pieces[i]; // select the piece
                temp = piecesSize[piece]; // piece's dimension
                centre = ChessBoard[7, i].GetxPieces() * widthScreen / 1920; // cell's centre

                piecesBlack[i].Size = new Size((int)(temp.GetW() * x), (int)(temp.GetH() * x)); // new size for the piece
                piecesBlack[i].Location = new Point((int)centre - (temp.GetW() / 2), ChessBoard[7, i].GetyPieces() * heightScreen / 1080 - piecesBlack[i].Size.Height); // new location for the piece
                piecesBlack[i].BackColor = Color.Transparent; // background transparent
                piecesBlack[i].Image = Image.FromFile("pieces/black/" + pieces[i] + ".png"); // load the image
                piecesBlack[i].SizeMode = PictureBoxSizeMode.StretchImage; // stretch mode for the picturebox
                Controls.Add(piecesBlack[i]); // add to the form
            }
        }

        private bool CheckTurn() // check which user can move pieces
        {
            if (ChessBoard[r, c].GetPiece() != null) // if user selected an empty cell
            {
                if (turn == ChessBoard[r, c].GetPiece().GetColor()) return true; // first user uses the black pieces
            }
            return false;
        }

        private bool Find (int rnew, int cnew) // searching if matrix coordinates (from mouse click) are possible moves
        {
            int i = 0;

            while (vett[i].GetCMatrix() != -1 || vett[i].GetRMatrix() != -1)
            {
                if (vett[i].GetCMatrix() == cnew && vett[i].GetRMatrix() == rnew) return true;
                i++;
            }
            return false;
        }

        private bool CheckPositionEmpty (int line, string dir) // using in the arrocco movement, checking if the position between rook and king are all free
        {
            int ini = 1, end = 4; // left position for rook and king
            if (dir == "dx") // right position for king and rook
            {
                ini = 5;
                end = 7;
            }

            for (int i = ini; i < end; i++)
            {
                if (ChessBoard[line, i].GetPiece() != null) return false; // if even an only one cell is not free return false
            }
            return true;
        }

        private void mouseClick(object sender, MouseEventArgs e)
        {
            int xMouse, yMouse; // cursor coordinates

            if (occ == false) // if no piece has been selected
            {
                int i = 0; // index

                for (; i < GetArraySize(); i++) vett[i] = new Piece.PosMatrix(-1, -1); // reset array of matrix' positions
                for (i = 0; i < GetArraySize(); i++) PossiblePositions[i] = null; // reset array of possible moves

                // cursor coordinates, based on the screen resolution
                xMouse = Cursor.Position.X * 1920 / widthScreen;
                yMouse = Cursor.Position.Y * 1080 / heightScreen;

                if (GetPosMatrix(xMouse, yMouse, ref r, ref c)) // if clicked in a not valid point of the chessboard
                {
                    if (CheckTurn()) // check which user has the turn, so the player can't move pieces if is not his turn
                    {
                        if (ChessBoard[r, c].GetPicture() != null) // if cell selected is not empty
                        {
                            ChessBoard[r, c].GetPiece().PossibleMoves(r, c, ChessBoard, vett); // possible moves of the piece that has been clicked

                            if (vett[0].GetCMatrix() == -1) occ = false; // if there are no possible moves, reset the click
                            else // if there are possible moves
                            {
                                if (ChessBoard[r, c].GetPiece().GetName() == "king" && ChessBoard[r, c].GetFirstMove()) // if the king is selected and it has never been moved
                                {
                                    bool sx = true, dx = true;
                                    int row = 0, num = 0, a = 0, IndEnemies = 0;

                                    if (ChessBoard[r, c].GetPiece().GetColor()) row = 7; // if color is black, the row is the seventh
                                    while (vett[num].GetCMatrix() != -1) num++; // find the index of the last possible position

                                    Piece.PosMatrix[][] EnemiesMoves = new Piece.PosMatrix[16][];
                                    for (; a < 16; a++)
                                    {
                                        EnemiesMoves[a] = new Piece.PosMatrix[GetArraySize()];
                                        for (i = 0; i < GetArraySize(); i++) EnemiesMoves[a][i] = new Piece.PosMatrix(-1, -1); // reset array for each enemies's possible positions
                                    }

                                    for (i = 0; i < 8; i++)
                                    {
                                        for (a = 0; a < 8; a++)
                                        {
                                            if (ChessBoard[i, a].GetPiece() != null) // if the cell is not empty
                                            {
                                                if (ChessBoard[i, a].GetPiece().GetColor() != turn) // if in this cell there is a enemy piece
                                                {
                                                    ChessBoard[i, a].GetPiece().PossibleMoves(i, a, ChessBoard, EnemiesMoves[IndEnemies]); // possible moves for each enemy
                                                    IndEnemies++; // increment the index
                                                }
                                            }
                                        }
                                    }

                                    // check if the arrocco movement could cause a scacco situation
                                    for (i = 0; i < IndEnemies; i++)
                                    {
                                        for (a = 0; a < GetArraySize(); a++)
                                        {
                                            if (r == EnemiesMoves[i][a].GetRMatrix() && c - 2 == EnemiesMoves[i][a].GetCMatrix()) sx = false;
                                            if (r == EnemiesMoves[i][a].GetRMatrix() && c + 2 == EnemiesMoves[i][a].GetCMatrix()) dx = false;
                                        }
                                    }

                                    if (sx) // if moving to left is a safe movement for king
                                    {
                                        if (ChessBoard[row, 0].GetPiece().GetName() == "rook" && ChessBoard[row, 0].GetFirstMove()) // if the rook has never been moved
                                        {
                                            if (CheckPositionEmpty(row, "sx")) // check is the positions are free
                                            {
                                                vett[num] = new Piece.PosMatrix(r, c - 2); // new possible position, arrocco movement
                                                num++;
                                            }
                                        }
                                    }
                                    if (dx) // if moving to right is a safe movement for king
                                    {
                                        if (ChessBoard[row, 7].GetPiece().GetName() == "rook" && ChessBoard[row, 7].GetFirstMove()) // if the rook has never been moved
                                        {
                                            if (CheckPositionEmpty(row, "dx")) // check is the positions are free
                                            {
                                                vett[num] = new Piece.PosMatrix(r, c + 2); // new possible position, arrocco movement
                                                num++;
                                            }
                                        }
                                    }
                                }

                                string name = ChessBoard[r, c].GetPiece().GetName(); // find the piece selected with mouse
                                // using a different image which represents that the image has been selected, based on the color
                                if (ChessBoard[r, c].GetPiece().GetColor()) ChessBoard[r, c].GetPicture().Image = Image.FromFile("pieces/black/selected/" + name + ".png");
                                else ChessBoard[r, c].GetPicture().Image = Image.FromFile("pieces/white/selected/" + name + ".png");

                                int j = 0; // index
                                int rTemp = -1, cTemp = -1; // temp position in matrix

                                i = 0;
                                while (vett[i].GetCMatrix() != -1) // untill in the array there are default values, so untill the are possible moves
                                {
                                    rTemp = vett[i].GetRMatrix(); // row of possible move
                                    cTemp = vett[i].GetCMatrix(); // column of possible move

                                    if (ChessBoard[rTemp, cTemp].GetPicture() == null) // if the cell selected is empty, fill whith the red ball
                                    {
                                        PossiblePositions[j] = new PictureBox // new picture for every possible move
                                        {
                                            Size = new Size(30 * widthScreen / 1920, 30 * heightScreen / 1080), // new size of picturebox
                                            Location = new Point((ChessBoard[rTemp, cTemp].GetxPieces() - 20) * widthScreen / 1920, (ChessBoard[rTemp, cTemp].GetyPieces() - 40) * heightScreen / 1080), // new location
                                            SizeMode = PictureBoxSizeMode.StretchImage, // stretch mode for picturebox
                                            BackColor = Color.Transparent, // background color transparent
                                            Image = Image.FromFile("images/ball.png") // load the image
                                        };

                                        PossiblePositions[j].MouseClick += mouseClick; // add the mouse click, in order to click even on the picture
                                        Controls.Add(PossiblePositions[j]); // add to the form
                                        PossiblePositions[j].BringToFront(); // bring to front
                                    }
                                    else // if the cell is not empty, change the image, with a red border --> means that a enemy piece could be eaten
                                    {
                                        name = ChessBoard[rTemp, cTemp].GetPiece().GetName(); // type of piece selected
                                        // new image based on the color
                                        if (ChessBoard[rTemp, cTemp].GetPiece().GetColor()) ChessBoard[rTemp, cTemp].GetPicture().Image = Image.FromFile("pieces/black/threatened/" + name + ".png");
                                        else ChessBoard[rTemp, cTemp].GetPicture().Image = Image.FromFile("pieces/white/threatened/" + name + ".png");
                                    }

                                    // increment indexes
                                    i++;
                                    j++;
                                }

                                occ = true; // a piece has been selected
                            }
                        }
                    }
                }
            }
            else // a piece is selected
            {
                // cursor coordinates, based on the screen resolution
                xMouse = Cursor.Position.X * 1920 / widthScreen;
                yMouse = Cursor.Position.Y * 1080 / heightScreen;

                if (GetPosMatrix(xMouse, yMouse, ref rNew, ref cNew)) // if clicked in a valid point of the chessboard
                {
                    if (r == rNew && c == cNew) // if the new position is exactly the same to the previous one
                    {
                        if (!ExpertMode) // in the expert mode, touched piece moved piece, I can't deselect
                        {
                            // get the name of the piece selected
                            string name = ChessBoard[r, c].GetPiece().GetName();
                            // change the image and load the initial one, based on the color
                            if (ChessBoard[r, c].GetPiece().GetColor()) ChessBoard[r, c].GetPicture().Image = Image.FromFile("pieces/black/" + name + ".png");
                            else ChessBoard[r, c].GetPicture().Image = Image.FromFile("pieces/white/" + name + ".png");

                            int i = 0; // index
                            while (i < GetArraySize())
                            {
                                Controls.Remove(PossiblePositions[i]); // remove all the red balls which represented the possible moves
                                i++;
                            }

                            int rTemp = -1, cTemp = -1; // temp variable which represents the position in matrix

                            i = 0;
                            while (vett[i].GetCMatrix() != -1) // load the initial the image to all of the possible piece that could be eaten
                            {
                                rTemp = vett[i].GetRMatrix(); // row for every possible move
                                cTemp = vett[i].GetCMatrix(); // colomn for every possible move

                                if (ChessBoard[rTemp, cTemp].GetPicture() != null) // set a different image to the picturebox
                                {
                                    // get the piece name selected
                                    name = ChessBoard[rTemp, cTemp].GetPiece().GetName();
                                    // load the initial image
                                    if (ChessBoard[rTemp, cTemp].GetPiece().GetColor()) ChessBoard[rTemp, cTemp].GetPicture().Image = Image.FromFile("pieces/black/" + name + ".png");
                                    else ChessBoard[rTemp, cTemp].GetPicture().Image = Image.FromFile("pieces/white/" + name + ".png");
                                }

                                i++; // increment the index
                            }

                            occ = false; // no piece selected
                        }
                    }
                    else // if the new position is different than the initial one, another cell matrix has been selected
                    {
                        if (Find(rNew, cNew)) // if cell selected is one of the piece selected's possible moves
                        {
                            PiecesSize temp = piecesSize[ChessBoard[r, c].GetPiece().GetName()]; // piece' size based on the dictionary
                            double coef = Math.Pow(0.952f, rNew); // decrease the size based on how many cells the pieces has been moved

                            // move the piece in the new location
                            ChessBoard[r, c].GetPicture().Location = new Point(ChessBoard[rNew, cNew].GetxPieces() * widthScreen / 1920 - (int)(temp.GetW() * coef / 2), ChessBoard[rNew, cNew].GetyPieces() * heightScreen / 1080 - (int)(temp.GetH() * coef));
                            ChessBoard[r, c].GetPicture().Size = new Size((int)(temp.GetW() * coef), (int)(temp.GetH() * coef)); // new size, for the perspective

                            if (ChessBoard[rNew, cNew].GetPicture() != null) // is the cell selected is not empty --> contains a piece enemy
                            {
                                if (ChessBoard[rNew, cNew].GetPiece().GetName() == "king") // if king is eaten
                                {
                                    Controls.Remove(ChessBoard[rNew, cNew].GetPicture()); // remove the king from the chessboard

                                    // stop the timers
                                    Countdown.Stop();
                                    TimerPlayed.Stop();

                                    // visualize the winner
                                    if (ChessBoard[rNew, cNew].GetPiece().GetColor() == true) MessageBox.Show("VINCE IL GIOCATORE CON I PEZZI BIANCHI");
                                    else MessageBox.Show("VINCE IL GIOCATORE CON I PEZZI NERI");

                                    Close(); // close the chessboard form
                                    Welcome newForm = new Welcome();
                                    newForm.Show(); // show the chessboard form
                                }
                                else // if another piece is eaten
                                {
                                    temp = piecesSize[ChessBoard[rNew, cNew].GetPiece().GetName()]; // piece' size based on the dictionary

                                    int ini = 0, end = 0, pos = 0; // start and end of the space where research pieces enemy
                                    switch (ChessBoard[rNew, cNew].GetPiece().GetName()) // arrays of pieces dead have got fixed position based on the type of piece
                                    {
                                        case "pawn":
                                            ini = 0;
                                            end = 7;
                                            pos = 0;
                                            break;
                                        case "rook":
                                            ini = 8;
                                            end = 17;
                                            pos = 1;
                                            break;
                                        case "knight":
                                            ini = 18;
                                            end = 27;
                                            pos = 2;
                                            break;
                                        case "bishop":
                                            ini = 28;
                                            end = 37;
                                            pos = 3;
                                            break;
                                        case "queen":
                                            ini = 38;
                                            end = 46;
                                            pos = 4;
                                            break;
                                    }

                                    bool[] tempArray = new bool[47]; // temp array of died pieces
                                    int[] XtempArray = new int[5]; // temp array of position

                                    // which array based on the color of piece dead
                                    if (ChessBoard[rNew, cNew].GetPiece().GetColor() == true)
                                    {
                                        tempArray = BlackPiecesDied;
                                        XtempArray = XBlackPiecesDied;
                                    }
                                    else
                                    {
                                        tempArray = WhitePiecesDied;
                                        XtempArray = XWhitePiecesDied;
                                    }

                                    int j = ini;
                                    while (j <= end && tempArray[j] != false) j++; // find the index, the first free position in the array

                                    float coef2 = 0.57f * widthScreen / 1920;

                                    ChessBoard[rNew, cNew].GetPicture().Size = new Size((int)(temp.GetW() * coef2), (int)(temp.GetH() * coef2)); // new size for the died piece
                                    ChessBoard[rNew, cNew].GetPicture().Location = new Point(XtempArray[pos] * widthScreen / 1920, (int)(temp.GetH() * coef2 * (j - ini) + (5 * widthScreen / 1920) * (j - ini) + (12 * widthScreen / 1920))); // new location for the died piece

                                    if (ExpertMode) // if expert mode, bring to front the listboxes which represent the moves done
                                    {
                                        MovesPL1.BringToFront();
                                        MovesPL2.BringToFront();
                                    }

                                    tempArray[j] = true; // the position now is occuped
                                    eatenPiece = true; // variable using in the representation of moves done in the listbox
                                }
                            }

                            int i = 0;
                            while (vett[i].GetCMatrix() != -1) // checking all the possible moves
                            {
                                int rTem = vett[i].GetRMatrix(); // temp row of the possible move
                                int cTemp = vett[i].GetCMatrix(); // temp colomn of the possible move

                                if (ChessBoard[rTem, cTemp].GetPicture() != null)
                                {
                                    // get the piece name selected
                                    string namePiece = ChessBoard[rTem, cTemp].GetPiece().GetName();
                                    // load the initial image
                                    if (ChessBoard[rTem, cTemp].GetPiece().GetColor()) ChessBoard[rTem, cTemp].GetPicture().Image = Image.FromFile("pieces/black/" + namePiece + ".png");
                                    else ChessBoard[rTem, cTemp].GetPicture().Image = Image.FromFile("pieces/white/" + namePiece + ".png");
                                }

                                i++; // increment index
                            }

                            ChessBoard[rNew, cNew].SetPicture(ChessBoard[r, c].GetPicture()); // set the picture from the previous coordinates, in the new ones
                            ChessBoard[rNew, cNew].SetPiece(ChessBoard[r, c].GetPiece()); // set the piece from the previous coordinates, in the new ones
                            ChessBoard[rNew, cNew].GetPiece().posMatrix.SetPosMatrix(rNew, cNew); // set the new position in matrix

                            // get the piece name selected
                            string name = ChessBoard[r, c].GetPiece().GetName();
                            // load the initial image
                            if (ChessBoard[r, c].GetPiece().GetColor()) ChessBoard[r, c].GetPicture().Image = Image.FromFile("pieces/black/" + name + ".png");
                            else ChessBoard[r, c].GetPicture().Image = Image.FromFile("pieces/white/" + name + ".png");

                            ChessBoard[r, c].SetPicture(null); // set null the picture in the previous coordinates --> the cell is empty
                            ChessBoard[r, c].SetPiece(null); // set null the picture in the previous coordinates --> the cell is empty

                            i = 0;
                            while (i < GetArraySize()) // remove all the red balls which represented the possible moves
                            {
                                Controls.Remove(PossiblePositions[i]);
                                i++;
                            }

                            int rTemp = rNew; // temp variable
                            while (rTemp >= 0 && ChessBoard[rTemp, cNew].GetPicture() != null) // bring to front the pieces, based on perspective
                            {
                                ChessBoard[rTemp, cNew].GetPicture().BringToFront(); // bring to front
                                rTemp--;
                            }

                            if (ChessBoard[rNew, cNew].GetPiece().GetName() == "king" && Math.Abs(cNew - c) == 2) // if king is selected
                            {
                                int rArrocco = 0, cArrocco = 0;
                                if (ChessBoard[rNew, cNew].GetPiece().GetColor()) rArrocco = 7;

                                // checking which arrocco movement is done
                                if (cNew - c == -2)
                                {
                                    c = 0;
                                    cArrocco = 3;
                                    arroccoLungo = true;
                                }
                                else if (cNew - c == 2)
                                {
                                    c = 7;
                                    cArrocco = 5;
                                    arroccoCorto = true;
                                }

                                // new location of rook
                                ChessBoard[rArrocco, c].GetPicture().Location = new Point(ChessBoard[rArrocco, cArrocco].GetxPieces() * widthScreen / 1920 - (int)((temp.GetW() * coef) / 2), ChessBoard[rArrocco, cArrocco].GetyPieces() * heightScreen / 1080 - (int)(temp.GetH() * coef));

                                if (rArrocco == 0) ChessBoard[rArrocco, c].GetPicture().BringToFront(); // bring to front if arrocco movement has been execute in the first line

                                ChessBoard[rArrocco, cArrocco].SetPicture(ChessBoard[rArrocco, c].GetPicture()); // set the picture from the previous coordinates, in the new ones
                                ChessBoard[rArrocco, cArrocco].SetPiece(ChessBoard[rArrocco, c].GetPiece()); // set the piece from the previous coordinates, in the new ones
                                ChessBoard[rArrocco, cArrocco].GetPiece().posMatrix.SetPosMatrix(rArrocco, cArrocco); // set the new position in matrix

                                // set null the initial position of rook
                                ChessBoard[rArrocco, c].SetPiece(null);
                                ChessBoard[rArrocco, c].SetPicture(null);
                            }

                            if (ChessBoard[rNew, cNew].GetPiece().GetName() == "pawn") // only pawns can evolve
                            {
                                // only black pawn on the first line and white pawn on the seventh line
                                if (ChessBoard[rNew, cNew].GetPiece().GetColor() == true && rNew == 0 || ChessBoard[rNew, cNew].GetPiece().GetColor() == false && rNew == 7)
                                {
                                    int yPos = 20;
                                    if (rNew == 0) yPos = 925;
                                    Menu.Location = new Point((ChessBoard[rNew, cNew].GetxPieces() - 50) * widthScreen / 1920, yPos * heightScreen / 1080); // menu's position, based on the position the pawn got the first/last line
                                }
                            }

                            if (ChessBoard[rNew, cNew].GetFirstMove() == true) ChessBoard[rNew, cNew].SetFirstMove(false); // once moved a piece, the first move is false

                            Piece.PosMatrix[][] EnemiesMoves = new Piece.PosMatrix[20][]; // enemies' moves
                            Piece.PosMatrix[][] AlliesMoves = new Piece.PosMatrix[20][]; // allies' moves
                            Piece.PosMatrix[][] TempEnemiesMoves = new Piece.PosMatrix[20][]; // other enemies' moves

                            for (int a = 0; a < 20; a++) // 16 as the enemies pieces
                            {
                                EnemiesMoves[a] = new Piece.PosMatrix[GetArraySize()];
                                for (i = 0; i < GetArraySize(); i++) EnemiesMoves[a][i] = new Piece.PosMatrix(-1, -1); // reset array of matrix' positions

                                AlliesMoves[a] = new Piece.PosMatrix[GetArraySize()];
                                for (i = 0; i < GetArraySize(); i++) AlliesMoves[a][i] = new Piece.PosMatrix(-1, -1); // reset array of matrix' positions

                                TempEnemiesMoves[a] = new Piece.PosMatrix[GetArraySize()];
                                for (i = 0; i < GetArraySize(); i++) TempEnemiesMoves[a][i] = new Piece.PosMatrix(-1, -1); // reset array of matrix' positions
                            }

                            for (i = 0; i < GetArraySize(); i++) vett[i] = new Piece.PosMatrix(-1, -1); // reset array of matrix' positions

                            int ro = 0, co = 0;
                            if (FindKing(ref ro, ref co, turn)) // king is found in the chessboard, its coords are ro and co
                            {
                                ChessBoard[ro, co].GetPiece().PossibleMoves(ro, co, ChessBoard, vett); // possible moves of king

                                Piece[] KillersKing = new Piece[16];
                                for (i = 0; i < 16; i++) KillersKing[i] = null; // initialize the array of pieces that can eat the king

                                int IndEnemies = 0, IndAllies = 0, IndKillers = 0; // indexes

                                for (int l = 0; l < 8; l++) // check all cells in chessboard
                                {
                                    for (int w = 0; w < 8; w++)
                                    {
                                        if (ChessBoard[l, w].GetPiece() != null) // if the cell is not empty
                                        {
                                            if (ChessBoard[l, w].GetPiece().GetColor() == turn) // if the in cell there is a piece enemy
                                            {
                                                ChessBoard[l, w].GetPiece().PossibleMoves(l, w, ChessBoard, EnemiesMoves[IndEnemies]); // possible moves for each enemies

                                                int f = 0;
                                                while (EnemiesMoves[IndEnemies][f].GetCMatrix() != -1) // check if for each enemy, king could be eaten
                                                {
                                                    if (ro == EnemiesMoves[IndEnemies][f].GetRMatrix() && co == EnemiesMoves[IndEnemies][f].GetCMatrix())
                                                    {
                                                        KillersKing[IndKillers] = ChessBoard[l, w].GetPiece(); // new killer king
                                                        IndKillers++; // increment index
                                                    }
                                                    f++;
                                                }

                                                IndEnemies++; // increment index
                                            }
                                            else // if in the cell there is not a enemy, but a friend, different to the king
                                            {
                                                if (ChessBoard[l, w].GetPiece().GetName() != "king")
                                                {
                                                    ChessBoard[l, w].GetPiece().PossibleMoves(l, w, ChessBoard, AlliesMoves[IndAllies]); // possible moves of allies, except the king possible moves
                                                    IndAllies++; // increment index
                                                }
                                            }
                                        }
                                    }
                                }

                                bool checkMate = false;

                                if (CheckMate(ro, co, vett, EnemiesMoves, AlliesMoves, KillersKing)) // if every condition of checkmate is true
                                {
                                    checkMate = true;

                                    // stop the timers
                                    TimerPlayed.Stop();
                                    Countdown.Stop();

                                    // visualize the checkmate
                                    if (turn) MessageBox.Show("BLACK CHECKMATE");
                                    else MessageBox.Show("WHITE CHECKMATE");

                                    Close(); // close the chessboard form
                                    Welcome newForm = new Welcome();
                                    newForm.Show(); // show the chessboard form
                                }

                                bool find1 = false, find2 = false;
                                int indEnemiesTemp = 0, sum = 1, j = 0; // index
                                for (int b = 0; b < 8; b++) // checking all the matrix
                                {
                                    for (int v = 0; v < 8; v++)
                                    {
                                        j = 0; find1 = false; find2 = false;
                                        if (ChessBoard[b, v].GetPiece() != null)
                                        {
                                            if (ChessBoard[b, v].GetPiece().GetColor() == turn)
                                            {
                                                if (ChessBoard[b, v].GetPiece().GetName() != "pawn") // pawns eat movement is different to the simply movement
                                                {
                                                    ChessBoard[b, v].GetPiece().PossibleMoves(b, v, ChessBoard, TempEnemiesMoves[indEnemiesTemp]); // possible moves of each enemies
                                                    indEnemiesTemp++; // increment 
                                                }
                                                else
                                                {
                                                    if (ChessBoard[b, v].GetPiece().GetColor() == true) sum = -1;
                                                    if (b + sum < 8 && b + sum >= 0)
                                                    {
                                                        if (v - sum >= 0 && v - sum < 8 && ChessBoard[b + sum, v - sum].GetPiece() != null && ChessBoard[b + sum, v - sum].GetPiece().GetColor() != ChessBoard[b, v].GetPiece().GetColor())
                                                        {
                                                            TempEnemiesMoves[indEnemiesTemp][j] = new Piece.PosMatrix(b + sum, v - sum);
                                                            j++;
                                                            find1 = true;
                                                        }

                                                        // eat pieces in oblique
                                                        if (v + sum >= 0 && v + sum < 8 && ChessBoard[b + sum, v + sum].GetPiece() != null && ChessBoard[b + sum, v + sum].GetPiece().GetColor() != ChessBoard[b, v].GetPiece().GetColor())
                                                        {
                                                            TempEnemiesMoves[indEnemiesTemp][j] = new Piece.PosMatrix(b + sum, v + sum);
                                                            j++;
                                                            find2 = true;
                                                        }
                                                    }
                                                    if (find1 || find2) indEnemiesTemp++;
                                                }
                                            }
                                        }
                                    }
                                }

                                for (int s = 0; s < IndEnemies; s++) // for each enemy, check if 
                                {
                                    for (j = 0; j < GetArraySize(); j++) if (co == EnemiesMoves[s][j].GetCMatrix() && ro == EnemiesMoves[s][j].GetRMatrix()) possibleCheck = true;
                                }

                                if (possibleCheck && checkMate == false)
                                {
                                    if (ChessBoard[ro, co].GetPiece().GetColor() == true) MessageBox.Show("WHITE CHECK");
                                    else MessageBox.Show("BLACK CHECK");
                                    possibleCheck = true;
                                }
                            }

                            occ = false; // no piece selected

                            if (!arrocco)
                            {
                                if (!ExpertMode) // if expert mode is not selected
                                {
                                    TimerPlayed.Stop(); // stop timer

                                    TimerTemp = TimerCountdown; // reset the timer countdown
                                    TimerPlayed.Start(); // restart the timer
                                }
                                else
                                {
                                    string lett = null;
                                    switch(cNew) // letters on the chessboard
                                    {
                                        case 0:
                                            lett = "a";
                                            break;
                                        case 1:
                                            lett = "b";
                                            break;
                                        case 2:
                                            lett = "c";
                                            break;
                                        case 3:
                                            lett = "d";
                                            break;
                                        case 4:
                                            lett = "e";
                                            break;
                                        case 5:
                                            lett = "f";
                                            break;
                                        case 6:
                                            lett = "g";
                                            break;
                                        case 7:
                                            lett = "h";
                                            break;
                                    }

                                    string carat = " ";
                                    if (ChessBoard[rNew, cNew].GetPiece().GetName() != "pawn") // pawns hasn't got a notation
                                    {
                                        switch (ChessBoard[rNew, cNew].GetPiece().GetName()) // based on the piece name, using a different letter in the notation
                                        {
                                            case "knight":
                                                carat = " C";
                                                break;
                                            case "bishop":
                                                carat = " A";
                                                break;
                                            case "rook":
                                                carat = " T";
                                                break;
                                            case "queen":
                                                carat = " D";
                                                break;
                                            case "king":
                                                carat = " R";
                                                break;
                                        }
                                    }

                                    // counter for each player moves
                                    int countTemp = CountMovesPL2;
                                    if (turn) countTemp = CountMovesPL1;

                                    string Notation = CountMovesPL1 + "." + carat + lett; // notation
                                    // adding particular sign to the notation, based on the move
                                    if (eatenPiece) Notation += 'x';
                                    Notation += (rNew + 1);
                                    if (possibleCheck) Notation += '+';
                                    if (arroccoCorto) Notation = CountMovesPL1 + ". " + "0-0";
                                    if (arroccoLungo) Notation = CountMovesPL1 + ". " + "0-0-0";

                                    // adding notation to the right listbox, based on the turn which made the move
                                    if (turn)
                                    {
                                        MovesPL2.Items.Add(Notation);
                                        CountMovesPL1++;
                                    }
                                    else
                                    {
                                        MovesPL1.Items.Add(Notation);
                                        CountMovesPL2++;
                                    }
                                }

                                // reset variables
                                eatenPiece = false; // eaten piece
                                possibleCheck = false; // check
                                arroccoCorto = false; // arrocco 
                                arroccoLungo = false;

                                turn = !turn; // turn passes to the other player

                                if (turn == true) CurrentTurn.BackColor = Color.Black; // color of the colored textbox that represents the current turn
                                else CurrentTurn.BackColor = Color.White;

                            }
                        }
                    }
                }
            }
        }
        
        private bool CheckMate (int ro, int co, Piece.PosMatrix[] KingMoves, Piece.PosMatrix[][] EnemiesMoves, Piece.PosMatrix[][] AlliesMoves, Piece[] KillersKing) // checking the checkmate
        {
            int indPos = 0, q = 0, i = 0, f = 0, w = 0; // variables and indexes
            bool posKing = false, find1 = false, find2 = false;

            // reset array
            Piece.PosMatrix[] posTest = new Piece.PosMatrix[10];
            for (int j = 0; j < 10; j++) posTest[j] = new Piece.PosMatrix(-1, -1);

            while (KillersKing[i] != null) // until there are no killers king
            {
                if (KillersKing[i].GetName() == "queen" || KillersKing[i].GetName() == "bishop" || KillersKing[i].GetName() == "rook") // other pieces, directly moves to eat something, they haven't got a route
                {
                    int rp = KillersKing[i].posMatrix.GetRMatrix(); // row of piece
                    int cp = KillersKing[i].posMatrix.GetCMatrix(); // colomn of piece

                    if (cp == co) // horizontal route (rook and queen)
                    {
                        f = rp;
                        for (; f <= ro; f++)
                        {
                            posTest[indPos] = new Piece.PosMatrix(f, cp); // new positions
                            indPos++;
                        }

                        f = rp;
                        for (; f >= ro; f--)
                        {
                            posTest[indPos] = new Piece.PosMatrix(f, cp); // new positions
                            indPos++;
                        }
                    }
                    else
                    {
                        // find the line's equation
                        int mRetta = (ro - rp) / (co - cp);
                        int qRetta = ro - (mRetta * co);

                        if (co + mRetta >= 0 && co + mRetta < 8) // check if the values don't go over the limit of matrix
                        {
                            // find the killer king's route to kill the king
                            f = co + mRetta;
                            for (; f <= cp; f++)
                            {
                                posTest[indPos] = new Piece.PosMatrix(mRetta * f + qRetta, f); // new positions
                                indPos++;
                            }

                            f = co + mRetta;
                            for (; f >= cp; f--)
                            {
                                posTest[indPos] = new Piece.PosMatrix(mRetta * f + qRetta, f); // new positions
                                indPos++;
                            }
                        }
                    }
                }

                w = 0;
                while (posTest[w].GetCMatrix() != -1)
                {
                    for (int s = 0; s < 16; s++) // for each enemy
                    {
                        for (int j = 0; j < GetArraySize(); j++) // check all the possible moves
                        {
                            if (posTest[w].GetCMatrix() == AlliesMoves[s][j].GetCMatrix() && posTest[w].GetRMatrix() == AlliesMoves[s][j].GetRMatrix()) KillersKing[i] = null;
                        }
                    }
                    w++;
                }
                i++;
            }

            int indNew = 0;
            for (int g = 0; g < 10; g++) if (KillersKing[g] != null) indNew++;

            bool[] eat = new bool[10];
            for (i = 0; i < 10; i++) eat[i] = false;

            q = 0;
            while (KingMoves[q].GetCMatrix() != -1) // all the possible moves of king
            {
                for (int s = 0; s < 16; s++) // for each enemy
                {
                    for (int j = 0; j < GetArraySize(); j++) // check all the possible moves
                    {
                        // if the position of king is one of the possible moves of the enemies
                        if (ro == EnemiesMoves[s][j].GetRMatrix() && co == EnemiesMoves[s][j].GetCMatrix()) posKing = true;
                    }
                }
                q++;
            }

            ChessCell[,] ChessBoardCalcMoveEnemies = new ChessCell[8, 8];
            Piece.PosMatrix[][] TempEnemiesMoves = new Piece.PosMatrix[16][];

            int v = 0, b = 0, ind2 = 0, z = 0, indEnemiesTemp = 0, sum = 1;
            
            for (int a = 0; a < 16; a++)
            {
                TempEnemiesMoves[a] = new Piece.PosMatrix[GetArraySize()];
                for (i = 0; i < GetArraySize(); i++) TempEnemiesMoves[a][i] = new Piece.PosMatrix(-1, -1); // reset array of matrix' positions
            }

            while (KingMoves[z].GetCMatrix() != -1)
            {
                ChessBoardCalcMoveEnemies = (ChessCell[,])ChessBoard.Clone();

                ChessBoardCalcMoveEnemies[KingMoves[z].GetRMatrix(), KingMoves[z].GetCMatrix()].SetPiece(ChessBoardCalcMoveEnemies[ro, co].GetPiece());
                ChessBoardCalcMoveEnemies[KingMoves[z].GetRMatrix(), KingMoves[z].GetCMatrix()].SetPicture(ChessBoardCalcMoveEnemies[ro, co].GetPicture());
                ChessBoardCalcMoveEnemies[KingMoves[z].GetRMatrix(), KingMoves[z].GetCMatrix()].GetPiece().posMatrix.SetPosMatrix(KingMoves[z].GetRMatrix(), KingMoves[z].GetCMatrix());

                ChessBoardCalcMoveEnemies[ro, co].SetPiece(null);
                ChessBoardCalcMoveEnemies[ro, co].SetPicture(null);

                indEnemiesTemp = 0;
                int j = 0;
                for (b = 0; b < 8; b++)
                {
                    for (v = 0; v < 8; v++)
                    {
                        j = 0; find1 = false; find2 = false;
                        if (ChessBoardCalcMoveEnemies[b, v].GetPiece() != null)
                        {
                            if (ChessBoardCalcMoveEnemies[b, v].GetPiece().GetColor() == turn)
                            {
                                if (ChessBoardCalcMoveEnemies[b, v].GetPiece().GetName() != "pawn")
                                {
                                    ChessBoardCalcMoveEnemies[b, v].GetPiece().PossibleMoves(b, v, ChessBoardCalcMoveEnemies, TempEnemiesMoves[indEnemiesTemp]); // possible moves of enemies
                                    indEnemiesTemp++;
                                }
                                else
                                {
                                    if (ChessBoardCalcMoveEnemies[b, v].GetPiece().GetColor() == true) sum = -1;

                                    if (b + sum < 8 && b + sum >= 0)
                                    {
                                        if (v - sum >= 0 && v - sum < 8 && ChessBoardCalcMoveEnemies[b + sum, v - sum].GetPiece() != null && ChessBoardCalcMoveEnemies[b + sum, v - sum].GetPiece().GetColor() != ChessBoardCalcMoveEnemies[b, v].GetPiece().GetColor())
                                        {
                                            TempEnemiesMoves[indEnemiesTemp][j] = new Piece.PosMatrix(b + sum, v - sum);
                                            j++;
                                            find1 = true;
                                        }

                                        // eat pieces in oblique
                                        if (v + sum >= 0 && v + sum < 8 && ChessBoardCalcMoveEnemies[b + sum, v + sum].GetPiece() != null && ChessBoardCalcMoveEnemies[b + sum, v + sum].GetPiece().GetColor() != ChessBoardCalcMoveEnemies[b, v].GetPiece().GetColor())
                                        {
                                            TempEnemiesMoves[indEnemiesTemp][j] = new Piece.PosMatrix(b + sum, v + sum);
                                            j++;
                                            find2 = true;
                                        }
                                    }
                                    if (find1 || find2) indEnemiesTemp++;
                                }
                            }
                        }
                    }
                }

                // se il re è nuovamente mangiato --> scacco matto 
                for (int s = 0; s < indEnemiesTemp; s++) // for each enemy
                {
                    for (j = 0; j < GetArraySize(); j++)
                    {
                        if (KingMoves[z].GetCMatrix() == TempEnemiesMoves[s][j].GetCMatrix() && KingMoves[z].GetRMatrix() == TempEnemiesMoves[s][j].GetRMatrix())
                        {
                            eat[ind2] = true;
                            ind2++;
                            goto EndOfLoop;
                        }
                    }
                }
                EndOfLoop:;

                z++;
            }

            for (b = 0; b < 8; b++) for (v = 0; v < 8; v++) ChessBoardCalcMoveEnemies[b, v].SetPicture(null);

            int cout = 0;
            for (b = 0; b < 10; b++) if (eat[b] == true) cout++;

            if (posKing == true && indNew >= 1 && cout == z) return true; // if all conditions are true, return true
            return false;
        }

        private bool FindKing(ref int ro, ref int co, bool turn) // find the coords in matrix of the enemy king 
        {
            for (int k = 0; k < 8; k++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (ChessBoard[k, j].GetPiece() != null && ChessBoard[k, j].GetPiece().GetName() == "king" && ChessBoard[k, j].GetPiece().GetColor() != turn)
                    {
                        ro = k;
                        co = j;
                        return true; // if found return true (always the king will found, otherwise the game is stopped, because the king is died)
                    }
                }
            }
            return false;
        }

        // promotion of pawns when the arrive to the opposite side of the chessboard
        private void Menu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ChessBoard[rNew, cNew].GetPiece().GetName() == "pawn") // only pawns can be promoted in other pieces (except king)
            {
                if (ChessBoard[rNew, cNew].GetPiece().GetColor() == true && rNew == 0 || ChessBoard[rNew, cNew].GetPiece().GetColor() == false && rNew == 7)
                {
                    string piece = Menu.Text; // user selected the option
                    PiecesSize temp = piecesSize[piece]; 
                    int centre = ChessBoard[rNew, cNew].GetxPieces() * widthScreen / 1920; // cell's centre
                    double x = Math.Pow(0.952f, rNew); // coef used in size calculation

                    ChessBoard[rNew, cNew].GetPicture().Size = new Size((int)(temp.GetW() * x), (int)(temp.GetH() * x)); // new size based on perspective
                    ChessBoard[rNew, cNew].GetPicture().Location = new Point(centre - (temp.GetW() / 2), ChessBoard[rNew, cNew].GetyPieces() * heightScreen / 1080 - ChessBoard[rNew, cNew].GetPicture().Size.Height); // new location
                    
                    string col = "white/";
                    if (ChessBoard[rNew, cNew].GetPiece().GetColor() == true) col = "black/";

                    ChessBoard[rNew, cNew].GetPicture().Image = Image.FromFile("pieces/" + col + piece + ".png"); // load the image
                    
                    Piece PieTemp = null;
                    switch (piece) // create piece based on the name of the piece
                    {
                        case "queen":
                            PieTemp = new Queen() { color = ChessBoard[rNew, cNew].GetPiece().GetColor(), name = piece, posMatrix = new Piece.PosMatrix(rNew, cNew) };
                            break;
                        case "bishop":
                            PieTemp = new Bishop() { color = ChessBoard[rNew, cNew].GetPiece().GetColor(), name = piece, posMatrix = new Piece.PosMatrix(rNew, cNew) };
                            break;
                        case "rook":
                            PieTemp = new Rook() { color = ChessBoard[rNew, cNew].GetPiece().GetColor(), name = piece, posMatrix = new Piece.PosMatrix(rNew, cNew) };
                            break;
                        case "knight":
                            PieTemp = new Knight() { color = ChessBoard[rNew, cNew].GetPiece().GetColor(), name = piece, posMatrix = new Piece.PosMatrix(rNew, cNew) };
                            break;
                    }

                    ChessBoard[rNew, cNew].SetPiece(PieTemp); // set the new piece in matrix
                    Menu.Location = new Point(12 * widthScreen / 1920, 1037 * heightScreen / 1080); // new location, not visible for the players
                }
            }
        }

        // timer tick each second
        private void Countdown_Tick(object sender, EventArgs e)
        {
            // if the player miss the turn, he loose
            if (!ExpertMode)
            {
                TimerTemp--;

                if (TimerTemp == 0)
                {
                    string vict = null;
                    Countdown.Stop(); // stop the timer of countdown

                    vict = "TIME OUT! THE PLAYER WITH THE BLACK PIECES WINS!";
                    if (turn) vict = "TIME OUT!THE PLAYER WITH THE WHITE PIECES WINS!";
                    MessageBox.Show(vict);

                    Close(); // close the chessboard form
                    Welcome newForm = new Welcome();
                    newForm.Show(); // show the chessboard form
                }

                TimeSpan time = TimeSpan.FromSeconds(TimerTemp);
                TimerVisual.Text = time.ToString(@"mm\:ss"); // visualize the remaining time on the textbox

                TimerProgressBar.Value = TimerTemp; // value is represented on the progress bar
            }
            else
            {
                int wichTimer = 0;

                if (turn) TimerTemp2--;
                else TimerTemp--; // every second the timer is decreased
                
                if (TimerTemp == 0 || TimerTemp2 == 0)
                {
                    string vict = null;

                    Countdown.Stop(); // stop the timer of countdown
                    if (TimerTemp == 0) vict = "TIME OUT! THE PLAYER WITH THE BLACK PIECES WINS!";
                    if (TimerTemp2 == 0) vict = "TIME OUT! THE PLAYER WITH THE BLACK PIECES WINS!";
                    MessageBox.Show(vict);

                    Close();
                    Welcome newForm = new Welcome();
                    newForm.Show();
                }

                TimeSpan time; 
                if (turn) time = TimeSpan.FromSeconds(TimerTemp2);
                else time = TimeSpan.FromSeconds(TimerTemp);
                TimerVisual.Text = time.ToString(@"mm\:ss"); // visualize the remaining time on the textbox

                TimerProgressBar.Value = wichTimer; // value is represented on the progress bar
            }
        }

        private void TimerPlayed_Tick(object sender, EventArgs e)
        {
            if (!ExpertMode)
            {
                turn = !turn; // change turn at the end of the main timer
                if (turn) CurrentTurn.BackColor = Color.Black; // color of the box that represents the current turn
                else CurrentTurn.BackColor = Color.White;

                TimerTemp = TimerCountdown; // reset the timer of countdown
                TimerProgressBar.Value = TimerCountdown; // reset the progressive bar to full
            }
        }

        private void NewGame_Click(object sender, EventArgs e)
        {
            // reset all the moves in the listboxs
            MovesPL1.Items.Clear();
            MovesPL2.Items.Clear();

            MovesPL1.Visible = false;
            MovesPL2.Visible = false;

            occ = false; // no piece selected
            turn = false; // turn for the first user

            // different color based on the turn
            if (turn) CurrentTurn.BackColor = Color.Black;
            else CurrentTurn.BackColor = Color.White;

            TimerProgressBar.Maximum = TimerCountdown; // reset the timer
            TimerProgressBar.Value = TimerCountdown; // reset the progress bar
            TimerTemp = TimerCountdown;
            if (ExpertMode) TimerTemp2 = TimerCountdown;

            // initialize the arrays of pieces died
            for (int i = 0; i < 47; i++)
            {
                BlackPiecesDied[i] = false;
                WhitePiecesDied[i] = false;
            }

            GC.Collect(); // free memory not more used

            int m = 0;
            while (PossiblePositions[m] != null) 
            {
                Controls.Remove(PossiblePositions[m]);
                PossiblePositions[m] = null;
                m++;
            }

            Form1_Load(sender, e); // launch the load to initialize all
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit(); // kill the application (even in background)
        }

        private void Visualize_Click(object sender, EventArgs e)
        {
            if (ExpertMode)
            {
                MovesPL1.Visible = !MovesPL1.Visible;
                MovesPL2.Visible = !MovesPL2.Visible;
                MovesPL1.BringToFront();
                MovesPL2.BringToFront();
            }
        }
    }
}
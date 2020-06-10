using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Scacchi.Scacchiera;

namespace Scacchi
{
    public class Piece
    {
        public bool color; // pieces' color 
        public string name; // pieces' name
        public PosMatrix posMatrix; // pieces position in matrix

        public struct PosMatrix // struct which represents the position (x, y) in the chessboard
        {
            private int r;
            private int c;

            public PosMatrix(int ro, int co)
            {
                r = ro;
                c = co;
            }

            public int GetRMatrix()
            {
                return r;
            }

            public int GetCMatrix()
            {
                return c;
            }

            public void SetCMatrix(int co)
            {
                c = co;
            }

            public void SetRMatrix(int ro)
            {
                r = ro;
            }

            public void SetPosMatrix(int ro, int co)
            {
                r = ro;
                c = co;
            }
        }

        public virtual void PossibleMoves(int r, int c, ChessCell[,] matrix, PosMatrix[] vett)
        {
            vett[0] = new PosMatrix(-1, -1);
        }

        public bool GetColor()
        {
            return color;
        }

        public void SetColor(bool c)
        {
            color = c;
        }

        public string GetName()
        {
            return name;
        }

        public void SetName(string n)
        {
            name = n;
        }
    }

    public class Rook : Piece
    {
        public override void PossibleMoves(int r, int c, ChessCell[,] matrix, PosMatrix[] vett)
        {
            int j = 0, i = 1;

            while (r + i < 8 && r + i >= 0 && matrix[r + i, c].GetPiece() == null)
            {
                vett[j] = new PosMatrix(r + i, c);
                i++; j++;
            }
            // eat piece
            if (r + i < 8 && r + i >= 0 && matrix[r + i, c].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
            {
                vett[j] = new PosMatrix(r + i, c);
                j++;
            }

            i = 1;
            while (r - i >= 0 && r - i < 8 && matrix[r - i, c].GetPiece() == null)
            {
                vett[j] = new PosMatrix(r - i, c);
                i++; j++;
            }
            // eat piece
            if (r - i >= 0 && r - i < 8 && matrix[r - i, c].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
            {
                vett[j] = new PosMatrix(r - i, c);
                j++;
            }

            i = 1;
            while (c + i < 8 && matrix[r, c + i].GetPiece() == null)
            {
                vett[j] = new PosMatrix(r, c + i);
                i++; j++;
            }
            // eat piece
            if (c + i < 8 && matrix[r, c + i].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
            {
                vett[j] = new PosMatrix(r, c + i);
                j++;
            }

            i = 1;
            while (c - i >= 0 && matrix[r, c - i].GetPiece() == null)
            {
                vett[j] = new PosMatrix(r, c - i);
                i++; j++;
            }
            // eat piece
            if (c - i >= 0 && matrix[r, c - i].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
            {
                vett[j] = new PosMatrix(r, c - i);
                j++;
            }
        }
    }

    public class Knight : Piece
    {
        public override void PossibleMoves(int r, int c, ChessCell[,] matrix, PosMatrix[] vett)
        {
            int j = 0, i = 0, k = 0;

            k = 2;
            i = 1;
            for (int f = 0; f < 2; f++)
            {
                if (r + k < 8)
                {
                    if (c + i < 8)
                    {
                        if (matrix[r + k, c + i].GetPiece() == null || matrix[r + k, c + i].GetPiece() != null && matrix[r + k, c + i].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
                        {
                            vett[j] = new PosMatrix(r + k, c + i);
                            j++;
                        }
                    }

                    if (c - i >= 0)
                    {
                        if (matrix[r + k, c - i].GetPiece() == null || matrix[r + k, c - i].GetPiece() != null && matrix[r + k, c - i].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
                        {
                            vett[j] = new PosMatrix(r + k, c - i);
                            j++;
                        }
                    }
                }

                if (r - k >= 0)
                {
                    if (c + i < 8)
                    {
                        if (matrix[r - k, c + i].GetPiece() == null || matrix[r - k, c + i].GetPiece() != null && matrix[r - k, c + i].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
                        {
                            vett[j] = new PosMatrix(r - k, c + i);
                            j++;
                        }
                    }

                    if (c - i >= 0)
                    {
                        if (matrix[r - k, c - i].GetPiece() == null || matrix[r - k, c - i].GetPiece() != null && matrix[r - k, c - i].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
                        {
                            vett[j] = new PosMatrix(r - k, c - i);
                            j++;
                        }
                    }
                }

                k = 1;
                i = 2;
            }
        }
    }

    public class Bishop : Piece
    {
        public override void PossibleMoves(int r, int c, ChessCell[,] matrix, PosMatrix[] vett)
        {
            int j = 0, i = 1;

            while (r + i < 8 && c + i < 8 && matrix[r + i, c + i].GetPiece() == null)
            {
                vett[j] = new PosMatrix(r + i, c + i);
                i++; j++;
            }
            // eat piece
            if (r + i < 8 && c + i < 8 && matrix[r + i, c + i].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
            {
                vett[j] = new PosMatrix(r + i, c + i);
                j++;
            }

            i = 1;
            while (r - i >= 0 && c - i >= 0 && matrix[r - i, c - i].GetPiece() == null)
            {
                vett[j] = new PosMatrix(r - i, c - i);
                i++; j++;
            }
            // eat piece
            if (r - i >= 0 && c - i >= 0 &&  matrix[r - i, c - i].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
            {
                vett[j] = new PosMatrix(r - i, c - i);
                j++;
            }

            i = 1;
            while (c + i < 8 && r - i >= 0 && matrix[r - i, c + i].GetPiece() == null)
            {
                vett[j] = new PosMatrix(r - i, c + i);
                i++; j++;
            }

            // eat piece
            if (c + i < 8 && r - i >= 0 && matrix[r - i, c + i].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
            {
                vett[j] = new PosMatrix(r - i, c + i);
                j++;
            }

            i = 1;
            while (c - i >= 0 && r + i < 8 && matrix[r + i, c - i].GetPiece() == null)
            {
                vett[j] = new PosMatrix(r + i, c - i);
                i++; j++;
            }
            // eat piece
            if (c - i >= 0 && r + i < 8 && matrix[r + i, c - i].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
            {
                vett[j] = new PosMatrix(r + i, c - i);
                j++;
            }
        }
    }

    public class Queen : Piece
    {
        public override void PossibleMoves(int r, int c, ChessCell[,] matrix, PosMatrix[] vett)
        {
            int j = 0, i = 1;

            // bishop movement
            while (r + i < 8 && c + i < 8 && matrix[r + i, c + i].GetPiece() == null)
            {
                vett[j] = new PosMatrix(r + i, c + i);
                i++; j++;
            }
            // eat piece
            if (r + i < 8 && c + i < 8 && matrix[r + i, c + i].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
            {
                vett[j] = new PosMatrix(r + i, c + i);
                j++;
            }

            i = 1;
            while (r - i >= 0 && c - i >= 0 && matrix[r - i, c - i].GetPiece() == null)
            {
                vett[j] = new PosMatrix(r - i, c - i);
                i++; j++;
            }
            // eat piece
            if (r - i >= 0 && c - i >= 0 && matrix[r - i, c - i].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
            {
                vett[j] = new PosMatrix(r - i, c - i);
                j++;
            }

            i = 1;
            while (c + i < 8 && r - i >= 0 && matrix[r - i, c + i].GetPiece() == null)
            {
                vett[j] = new PosMatrix(r - i, c + i);
                i++; j++;
            }

            // eat piece
            if (c + i < 8 && r - i >= 0 && matrix[r - i, c + i].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
            {
                vett[j] = new PosMatrix(r - i, c + i);
                j++;
            }

            i = 1;
            while (c - i >= 0 && r + i < 8 && matrix[r + i, c - i].GetPiece() == null)
            {
                vett[j] = new PosMatrix(r + i, c - i);
                i++; j++;
            }
            // eat piece
            if (c - i >= 0 && r + i < 8 && matrix[r + i, c - i].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
            {
                vett[j] = new PosMatrix(r + i, c - i);
                j++;
            }

            // rook movement
            i = 1;
            while (r + i < 8 && r + i >= 0 && matrix[r + i, c].GetPiece() == null)
            {
                vett[j] = new PosMatrix(r + i, c);
                i++; j++;
            }
            // eat piece
            if (r + i < 8 && r + i >= 0 && matrix[r + i, c].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
            {
                vett[j] = new PosMatrix(r + i, c);
                j++;
            }

            i = 1;
            while (r - i >= 0 && r - i < 8 && matrix[r - i, c].GetPiece() == null)
            {
                vett[j] = new PosMatrix(r - i, c);
                i++; j++;
            }
            // eat piece
            if (r - i >= 0 && r - i < 8 && matrix[r - i, c].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
            {
                vett[j] = new PosMatrix(r - i, c);
                j++;
            }

            i = 1;
            while (c + i < 8 && matrix[r, c + i].GetPiece() == null)
            {
                vett[j] = new PosMatrix(r, c + i);
                i++; j++;
            }
            // eat piece
            if (c + i < 8 && matrix[r, c + i].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
            {
                vett[j] = new PosMatrix(r, c + i);
                j++;
            }

            i = 1;
            while (c - i >= 0 && matrix[r, c - i].GetPiece() == null)
            {
                vett[j] = new PosMatrix(r, c - i);
                i++; j++;
            }
            // eat piece
            if (c - i >= 0 && matrix[r, c - i].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
            {
                vett[j] = new PosMatrix(r, c - i);
                j++;
            }
        }
    }

    public class King : Piece
    {
        public override void PossibleMoves(int r, int c, ChessCell[,] matrix, PosMatrix[] vett)
        {
            int j = 0, i = 1, k = 0;
            int[] v = { 1, 0, -1 };

            for (int f = 0; f < 3; f++)
            {
                k = v[f];

                if (r + k < 8 && r + k >= 0)
                {
                    if (k != 0)
                    {
                        if (matrix[r + k, c].GetPiece() == null || matrix[r + k, c].GetPiece() != null && matrix[r + k, c].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
                        {
                            vett[j] = new PosMatrix(r + k, c);
                            j++;
                        }
                    }

                    if (c - i >= 0 && c - i < 8) {
                        if (matrix[r + k, c - i].GetPiece() == null || matrix[r + k, c - i].GetPiece() != null && matrix[r + k, c - i].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
                        {
                            vett[j] = new PosMatrix(r + k, c - i);
                            j++;
                        }
                    }

                    if (c + i < 8 && c + i >= 0)
                    {
                        if (matrix[r + k, c + i].GetPiece() == null || matrix[r + k, c + i].GetPiece() != null && matrix[r + k, c + i].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
                        {
                            vett[j] = new PosMatrix(r + k, c + i);
                            j++;
                        }
                    }
                }
                
            }
        }
    }

    public class Pawn : Piece
    {
        public override void PossibleMoves(int r, int c, ChessCell[,] matrix, PosMatrix[] vett)
        {
            int j = 0, i = 1, f = 2;

            if (matrix[r, c].GetPiece().GetColor() == true)
            {
                i = -1;
                f = -2;
            }
            if (r + i < 8 && r + i >= 0)
            {
                if (matrix[r + i, c].GetPiece() == null)
                {
                    vett[j] = new PosMatrix(r + i, c);
                    j++;

                    if (matrix[r, c].GetFirstMove() == true && matrix[r + f, c].GetPiece() == null) // at the first move, a pawn can move 2 cells
                    {
                        vett[j] = new PosMatrix(r + f, c);
                        j++;
                    }
                }

                // eat pieces in oblique
                if (c - i >= 0 && c - i < 8 && matrix[r + i, c - i].GetPiece() != null && matrix[r + i, c - i].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
                {
                    vett[j] = new PosMatrix(r + i, c - i);
                    j++;
                }

                // eat pieces in oblique
                if (c + i >= 0  && c + i < 8  && matrix[r + i, c + i].GetPiece() != null && matrix[r + i, c + i].GetPiece().GetColor() != matrix[r, c].GetPiece().GetColor())
                {
                    vett[j] = new PosMatrix(r + i, c + i);
                    j++;
                }
            }
        }
    }
}

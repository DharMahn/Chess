using System.Drawing.Imaging;

namespace Chess
{
    public partial class Form1 : Form
    {
        Piece[,] board = new Piece[8, 8];

        Brush cellDark = Brushes.Brown;
        Brush cellLight = Brushes.Beige;
        Brush pieceDark = Brushes.Red;
        Brush pieceLight = Brushes.Blue;
        Dictionary<string, Bitmap> pieceTextures = new Dictionary<string, Bitmap>();
        public Form1()
        {
            InitializeComponent();
            string piecesDirectory = Path.Combine(Application.StartupPath, "Pieces");
            string[] files = Directory.GetFiles(piecesDirectory, "*.png");
            foreach (string file in files)
            {
                try
                {
                    // Load the image
                    Bitmap originalImage = (Image.FromFile(file) as Bitmap)!;
                    string baseName = Path.GetFileNameWithoutExtension(file);

                    // Tint the image to pieceDark and pieceLight
                    Bitmap tintedDark = TintImage(originalImage, ((SolidBrush)pieceDark).Color);
                    Bitmap tintedLight = TintImage(originalImage, ((SolidBrush)pieceLight).Color);

                    // Add the tinted images to the dictionary
                    pieceTextures[baseName + "_Dark"] = tintedDark;
                    pieceTextures[baseName + "_Light"] = tintedLight;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error processing image: " + file + "\n" + ex.Message);
                }
            }
            for (int y = 0; y < board.GetLength(1); y++)
            {
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    board[x, y] = new Piece
                    {
                        Type = PieceType.Pawn,
                        IsDark = new Random().NextDouble() >= 0.5
                    };
                }
            }
        }
        private Bitmap TintImage(Bitmap sourceImage, Color tint)
        {
            // Create a bitmap to hold the tinted image
            Bitmap tintedBitmap = new Bitmap(sourceImage.Width, sourceImage.Height);

            using (Graphics g = Graphics.FromImage(tintedBitmap))
            {
                // Use a ColorMatrix to apply the tinting effect
                ColorMatrix colorMatrix = new ColorMatrix(
                [
                [tint.R / 255.0f, 0, 0, 0, 0],
                [0, tint.G / 255.0f, 0, 0, 0],
                [0, 0, tint.B / 255.0f, 0, 0],
                [0, 0, 0, 1, 0],
                [0, 0, 0, 0, 1]
                ]);

                using (ImageAttributes attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(colorMatrix);

                    // Draw the original image with the color matrix applied
                    g.DrawImage(sourceImage, new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                                0, 0, sourceImage.Width, sourceImage.Height, GraphicsUnit.Pixel, attributes);
                }
            }

            return tintedBitmap;
        }
        private void panel_chessboard_Paint(object sender, PaintEventArgs e)
        {
            int cellSize = panel_chessboard.Width / board.GetLength(0);
            for (int y = 0; y < board.GetLength(1); y++)
            {
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    e.Graphics.FillRectangle((x + y) % 2 == 0 ? cellLight : cellDark, x * cellSize, y * cellSize, cellSize, cellSize);
                    if (board[x, y].Type != PieceType.None)
                        e.Graphics.DrawImage(pieceTextures[board[x, y].Type.ToString() + (board[x, y].IsDark ? "_Dark" : "_Light")], x * cellSize, y * cellSize, cellSize, cellSize);

                }
            }
        }
    }
    public class Piece
    {
        public bool IsDark;
        public PieceType Type;
    }
    public enum PieceType
    {
        None, Pawn, Knight, Bishop, Rook, Queen, King
    }
}

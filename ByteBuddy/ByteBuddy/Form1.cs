using ByteBuddy.Entities;
using ByteBuddy.Utils;
using System.ComponentModel;
using Timer = System.Windows.Forms.Timer;

namespace ByteBuddy
{
    public partial class Form1 : Form
    {
        private bool _haveHandle { get; set; } = false;

        private Timer _timerSpeed { get; set; } = new Timer();

        private int _frameCount { get; set; } = 0;
        private int _frame { get; set; } = 0;

        private int _fullWidth { get; set; } = 0;
        private int _fullHeight { get; set; } = 0;

        private int _frameWidth { get; set; } = 0;
        private int _frameHeight { get; set; } = 0;

        private bool _mouseDown { get; set; } = false;

        private Point _oldPoint { get; set; } = new Point(0, 0);

        private List<Buddy> _buddies { get; set; } = new List<Buddy>();
        private string _selectedBuddy { get; set; } = string.Empty;

        private Image _fullImage
        {
            get
            {
                // TODO: Refactor
                byte[] bytes = _buddies.First(buddy => buddy.Name == _selectedBuddy).Bytes;
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    Image image = Image.FromStream(ms);
                    Size size = new Size(image.Width * 2, image.Height * 2);
                    return (Image)(new Bitmap(image, size));
                }
            }
        }

        private Bitmap _frameImage
        {
            get
            {
                Bitmap bitmap = new Bitmap(_fullWidth, _fullHeight);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawImage(_fullImage, new Rectangle(0, 0, bitmap.Width, bitmap.Height), new Rectangle(_frameWidth * _frame, 0, _frameWidth, _frameHeight), GraphicsUnit.Pixel);
                    return bitmap;
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            this.TopMost = true;

            _buddies = DatabaseUtils.GetAll<Buddy>("SELECT * FROM Buddies");
            Random rnd = new Random();
            int r = rnd.Next(_buddies.Count);
            _selectedBuddy = _buddies[r].Name;
            _frameCount = _buddies.First(w => w.Name == _selectedBuddy).Frames;

            TaskbarUtils tb = new TaskbarUtils();
            DebugLogger.Debug("Taskbar Informations: w:{0}, h:{1} - hide:{2}", tb.Size.Width, tb.Size.Height, tb.AutoHide);

            _fullWidth = _fullImage.Width / _frameCount;
            _fullHeight = _fullImage.Height;

            _frameWidth = _fullWidth;
            _frameHeight = _fullHeight;

            DebugLogger.Debug("Frame Informations: w:{0}, h:{1}", _frameWidth, _frameHeight);
            DebugLogger.Debug("Frames: {0}", _frameCount);

            _timerSpeed.Interval = _frameCount;
            _timerSpeed.Enabled = true;
            _timerSpeed.Tick += new EventHandler(timerSpeed_Tick);

            MouseMove += new MouseEventHandler(Form2_MouseMove);
            DoubleClick += new EventHandler(Form2_DoubleClick);
            MouseDown += new MouseEventHandler(Form2_MouseDown);
            MouseUp += new MouseEventHandler(Form2_MouseUp);
        }

        private void InitializeStyles()
        {
            DebugLogger.Debug("Setting styles");

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            UpdateStyles();

            DebugLogger.Debug("Styles OK");
        }

        public void SetBits()
        {
            BackgroundUtils.SetBackground(bitmap: _frameImage, control: this);
            //BackgroundUtils.SetBackground(bitmap: _frameImage, handle: Handle);

            GC.Collect();
        }

        #region Override

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnClosing(e);
            _haveHandle = false;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            DebugLogger.Debug("Creating handle");

            InitializeStyles();
            base.OnHandleCreated(e);
            _haveHandle = true;

            DebugLogger.Debug("Handle OK");
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cParms = base.CreateParams;
                cParms.ExStyle |= 0x00080000; // WS_EX_LAYERED
                return cParms;
            }
        }

        #endregion

        void timerSpeed_Tick(object sender, EventArgs e)
        {
            _frame++;
            if (_frame >= _frameCount) _frame = 0;

            if (!_haveHandle)
                return;

            SetBits();
        }

        void Form2_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown)
            {
                Left += (e.X - _oldPoint.X);
                Top += (e.Y - _oldPoint.Y);
            }
        }

        void Form2_DoubleClick(object sender, EventArgs e)
        {
#if DEBUG
            Dispose();
#endif
        }

        void Form2_MouseDown(object sender, MouseEventArgs e)
        {
#if DEBUG
            if (e.Button == MouseButtons.Right)
                Dispose();
#endif
            _oldPoint = e.Location;
            _mouseDown = true;
        }

        void Form2_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }
    }
}

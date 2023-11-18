using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using Timer = System.Windows.Forms.Timer;

namespace ByteBuddy
{
    public partial class Form1 : Form
    {
        private bool _haveHandle { get; set; } = false;

        private Timer _timerSpeed { get; set; } = new Timer();

        private int _frameCount { get; set; } = 60;
        private int _frame { get; set; } = 0;

        private int _fullWidth { get; set; }
        private int _fullHeight { get; set; }

        private int _frameWidth { get; set; }
        private int _frameHeight { get; set; }

        public Form1()
        {
            InitializeComponent();
            this.TopMost = true;

            _fullWidth = FullImage.Width / _frameCount;
            _fullHeight = FullImage.Height;

            _frameWidth = _fullWidth;
            _frameHeight = _fullHeight;

            _timerSpeed.Interval = 50;
            _timerSpeed.Enabled = true;
            _timerSpeed.Tick += new EventHandler(timerSpeed_Tick);

            this.DoubleClick += new EventHandler(Form2_DoubleClick);
            this.MouseDown += new MouseEventHandler(Form2_MouseDown);
            this.MouseUp += new MouseEventHandler(Form2_MouseUp);
            this.MouseMove += new MouseEventHandler(Form2_MouseMove);
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
            InitializeStyles();
            base.OnHandleCreated(e);
            _haveHandle = true;
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

        void Form2_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void InitializeStyles()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            UpdateStyles();
        }

        void timerSpeed_Tick(object sender, EventArgs e)
        {
            _frame++;
            if (_frame >= _frameCount) _frame = 0;

            if (!_haveHandle)
                return;
            
            SetBits();
        }

        private void FixLeftTop()
        {

        }

        private Image FullImage
        {
            get
            { 
                // TODO: implement actions
                if (true)
                    return Properties.Resources.Right;
                else
                    return Properties.Resources.Left;
            }
        }

        public Bitmap FrameImage
        {
            get
            {
                Bitmap bitmap = new Bitmap(_fullWidth, _fullHeight);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawImage(FullImage, new Rectangle(0, 0, bitmap.Width, bitmap.Height), new Rectangle(_frameWidth * _frame, 0, _frameWidth, _frameHeight), GraphicsUnit.Pixel);
                    return bitmap;
                }
            }
        }

        void Form2_DoubleClick(object sender, EventArgs e)
        {
            this.Dispose();
        }

        void Form2_MouseMove(object sender, MouseEventArgs e)
        {

        }

        void Form2_MouseDown(object sender, MouseEventArgs e)
        {

        }

        public void SetBits()
        {
            BackgroundUtils.SetBackground(bitmap: FrameImage, handle: Handle);
            GC.Collect();
        }
    }
}

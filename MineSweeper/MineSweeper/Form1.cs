using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace MineSweeper
{
    public partial class Form1 : Form
    {
        //setting timer values
        int seconds = 0;
        int minutes = 0;

        //generate play field
        Button[,] btn = new Button[41, 90];
        int[,] btn_prop = new int[41, 90];
        int[,] saved_btn_prop = new int[41, 90];
        Point coord;

        //Table aspect
        int start_x, start_y;
        int height, width;

        //match status
        bool firstPlay = true;
        bool gameOver = false;

        //Game variables
        int mines;
        int flag_value = 10;
        int flags;
        

        //Button aspect
        int buttonSize = 25;
        int distance_between = 25;

        //Coeficients for difficulty
        double easyCoef = 0.1f;
        double mediumCoef = 0.2f;
        double hardCoef = 0.3f;
        double extremeCoef = 0.5f;

        //Points that are around
        int[] dx8 = { 1, 0, -1, 0, 1, -1, -1, 1 };
        int[] dy8 = { 0, 1, 0, -1, 1, -1, 1, -1 };

        //

        public Form1()
        {
            InitializeComponent();
        }

        void setButtonImage(int x, int y)
        {
            btn[x, y].Enabled = false;

            if (gameOver && btn_prop[x, y] == flag_value)
                btn_prop[x, y] = saved_btn_prop[x, y];

            if (gameOver)
                timer1.Stop();

            switch (btn_prop[x, y])
            {
                case 0:
                    btn[x, y].BackgroundImage = MineSweeper.Properties.Resources.blank;
                    EmptySpace(x, y);
                    break;

                case 1:
                    btn[x, y].BackgroundImage = MineSweeper.Properties.Resources._1;
                    break;

                case 2:
                    btn[x, y].BackgroundImage = MineSweeper.Properties.Resources._2;
                    break;

                case 3:
                    btn[x, y].BackgroundImage = MineSweeper.Properties.Resources._3;
                    break;

                case 4:
                    btn[x, y].BackgroundImage = MineSweeper.Properties.Resources._4;
                    break;

                case 5:
                    btn[x, y].BackgroundImage = MineSweeper.Properties.Resources._5;
                    break;

                case 6:
                    btn[x, y].BackgroundImage = MineSweeper.Properties.Resources._6;
                    break;

                case 7:
                    btn[x, y].BackgroundImage = MineSweeper.Properties.Resources._7;
                    break;

                case 8:
                    btn[x, y].BackgroundImage = MineSweeper.Properties.Resources._8;
                    break;

                case -1:
                    btn[x, y].BackgroundImage = MineSweeper.Properties.Resources.bmb;
                    if (!gameOver)
                        GameOver();
                    break;
            }

            }

        void Discover_Map()
        {
            for (int i = 1; i <= width; i++)
                for (int j = 1; j <= height; j++)
                    if (btn[i, j].Enabled == true)
                    {
                        setButtonImage(i, j);
                    }
        }

        void GameOver()
        {
            gameOver = true;
            Discover_Map();

            List<SoundPlayer> startAudio = new List<SoundPlayer>();
            SoundPlayer firstAudioStart = new SoundPlayer(MineSweeper.Properties.Resources.castillo_its_not_looking_good_bruv);
            SoundPlayer secondAudioStart = new SoundPlayer(MineSweeper.Properties.Resources.assets_ti_devi_spaventare);
            startAudio.Add(firstAudioStart);
            startAudio.Add(secondAudioStart);

            playsound(startAudio);

            MessageBox.Show("Game Over !");
            
        }

        int isPointOnMap(int x, int y)
        {
            if (x < 1 || x > width || y < 1 || y > height)
                return 0;
            return 1;
        }

        

        void EmptySpace(int x, int y)
        {
            if (btn_prop[x, y] == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    int cx = x + dx8[i];
                    int cy = y + dy8[i];

                    if (isPointOnMap(cx, cy) == 1)
                        if (btn[cx, cy].Enabled == true && btn_prop[cx, cy] != -1 && !gameOver)
                        {
                            int flagCounter = flags;
                            flagCounter--;
                            scoreBox.Text = "Score: " + flagCounter.ToString();
                            setButtonImage(cx, cy);
                        }
                }
            }
        }

        void Check_FlagWin()
        {
            bool win = true;

            for (int i = 1; i <= width; i++)
                for (int j = 1; j <= height; j++)
                    if (btn_prop[i, j] == -1)
                        win = false;

            if (win)
            {
                WinGame();
            }
        }


        void WinGame()
        {
            gameOver = true;
            Discover_Map();
            //gameProgress.Value = 0;

            List<SoundPlayer> startAudio = new List<SoundPlayer>();
            SoundPlayer firstAudioStart = new SoundPlayer(MineSweeper.Properties.Resources.assets_yeah_boi);
            SoundPlayer secondAudioStart = new SoundPlayer(MineSweeper.Properties.Resources.assets_questa_volta_no);
            startAudio.Add(firstAudioStart);
            startAudio.Add(secondAudioStart);

            playsound(startAudio);

            MessageBox.Show("Win !");
            
        }

        void Check_ClickWin()
        {
            bool win = true;
            for (int i = 1; i <= width; i++)
                for (int j = 1; j <= height; j++)
                    if (btn[i, j].Enabled == true && saved_btn_prop[i, j] != -1)
                        win = false;

            if (win)
            {
                WinGame();
            }
        }

        private void OneClick(object sender, EventArgs e)
        {
            coord = ((Button)sender).Location;
            int x = (coord.X - start_x) / buttonSize;
            int y = (coord.Y - start_y) / buttonSize;

            if (btn_prop[x, y] != flag_value)
            {

                ((Button)sender).Enabled = false;
                ((Button)sender).Text = "";

                ((Button)sender).BackgroundImageLayout = ImageLayout.Stretch;

                if (btn_prop[x, y] != -1 && !gameOver)
                {
                    //gameProgress.Value++;
                    //scoreBox.Text = "Score: " + gameProgress.Value.ToString();
                    Check_ClickWin();
                }

                setButtonImage(x, y);
            }
        }

        int MinesAround(int x, int y)
        {
            int score = 0;
            for (int i = 0; i < 8; i++)
            {
                int cx = x + dx8[i];
                int cy = y + dy8[i];

                if (isPointOnMap(cx, cy) == 1 && btn_prop[cx, cy] == -1)
                    score++;
            }
            return score;
        }

        void SetMapNumbers(int x, int y)
        {
            for (int i = 1; i <= x; i++)
                for (int j = 1; j <= y; j++)
                    if (btn_prop[i, j] != -1)
                    {
                        btn_prop[i, j] = MinesAround(i, j);
                        //Debugging
                        //btn[i, j].Text = btn_prop[i, j].ToString();
                        saved_btn_prop[i, j] = MinesAround(i, j);
                    }
        }

        private void RightClick(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {

                coord = ((Button)sender).Location;
                int x = (coord.X - start_x) / buttonSize;
                int y = (coord.Y - start_y) / buttonSize;

                if (btn_prop[x, y] != flag_value && flags > 0)
                {
                    btn[x, y].BackgroundImageLayout = ImageLayout.Stretch;
                    btn[x, y].BackgroundImage = MineSweeper.Properties.Resources.flag;
                    btn_prop[x, y] = flag_value;
                    flags--;
                    Check_FlagWin();
                }
                else
                    if (btn_prop[x, y] == flag_value)
                {
                    btn_prop[x, y] = saved_btn_prop[x, y];
                    btn[x, y].BackgroundImageLayout = ImageLayout.Stretch;
                    btn[x, y].BackgroundImage = null;
                    flags++;
                }

                textBox4.Text = "Flags: " + flags;
            }
        }

        void CreateButtons(int x, int y)
        {
            for (int i = 1; i <= x; i++)
                for (int j = 1; j <= y; j++)
                {
                    btn[i, j] = new Button();
                    btn[i, j].SetBounds(i * buttonSize + start_x, j * buttonSize + start_y, distance_between, distance_between);
                    btn[i, j].Click += new EventHandler(OneClick);
                    btn[i, j].MouseUp += new MouseEventHandler(RightClick);
                    btn_prop[i, j] = 0;
                    saved_btn_prop[i, j] = 0;
                    btn[i, j].TabStop = false;
                    Controls.Add(btn[i, j]);
                }
        }

        void GenerateMap(int x, int y, int mines)
        {
            Random rand = new Random();
            List<int> coordx = new List<int>();
            List<int> coordy = new List<int>();

            while (mines > 0)
            {
                coordx.Clear();
                coordy.Clear();

                for (int i = 1; i <= x; i++)
                    for (int j = 1; j <= y; j++)
                        if (btn_prop[i, j] != -1)
                        {
                            coordx.Add(i);
                            coordy.Add(j);
                        }

                int randNum = rand.Next(0, coordx.Count);
                btn_prop[coordx[randNum], coordy[randNum]] = -1;
                saved_btn_prop[coordx[randNum], coordy[randNum]] = -1;
                mines--;
            }
        }

        void StartGame()
        {
            switch (difficultyLevel.Text)
            {
                case "Easy":
                    mines = (int)(height * width * easyCoef);
                    break;

                case "Medium":
                    mines = (int)(height * width * mediumCoef);
                    break;

                case "Hard":
                    mines = (int)(height * width * hardCoef);
                    break;

                case "Extreme":
                    mines = (int)(height * width * extremeCoef);
                    break;
            }

            flags = mines;
            gameOver = false;

            //gameProgress.Value = 0;
            

            timer1.Start();
            seconds = 0;
            minutes = 0;

            textBox4.Text = "Flags: " + flags;
            scoreBox.Text = "Score: " + 0;

            if (firstPlay)
                CreateButtons(width, height);

            GenerateMap(width, height, mines);
            SetMapNumbers(width, height);

        }

        void ResetGame(int x, int y)
        {
            for (int i = 1; i <= x; i++)
                for (int j = 1; j <= y; j++)
                {
                    btn[i, j].Enabled = true;
                    btn[i, j].Text = "";
                    btn[i, j].BackgroundImage = null;
                    btn_prop[i, j] = 0;
                    saved_btn_prop[i, j] = 0;
                }
        }

        void Warnings(int id)
        {
            switch (id)
            {
                case 1:
                    MessageBox.Show("Empty Fields !");
                    break;
                case 2:
                    MessageBox.Show("Wrong Input !");
                    break;

            }
        }

        bool hasLetters(string s)
        {
            for (int i = 0; i < s.Length; i++)
                if (!Char.IsDigit(s, i))
                    return true;

            return false;
        }

        bool CorrectFields()
        {
            bool result = true;

            if (heightBox.Text == "" || widthBox.Text == "" || difficultyLevel.Text == "")
            {
                Warnings(1);
                result = false;
            }
            else
            if (heightBox.Text != "" && widthBox.Text != "" && difficultyLevel.Text != "")
            {
                if (hasLetters(heightBox.Text) || hasLetters(widthBox.Text))
                {
                    Warnings(2);
                    result = false;
                }
            }

            return result;
        }

        void SetDimensions()
        {
            height = int.Parse(heightBox.Text);
            width = int.Parse(widthBox.Text);

            if (height > 25)
                height = 25;
            else
                if (height < 5)
                height = 5;

            if (width > 40)
                width = 40;
            else
                if (width < 5)
                width = 5;

            heightBox.Text = height.ToString();
            widthBox.Text = width.ToString();

        }

        void TableMargins(int x, int y)
        {
            start_x = (this.Size.Width - (width + 2) * distance_between) / 2;
            start_y = (this.Size.Height - (height + 2) * distance_between) / 2;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            heightBox.Enabled = true;
            widthBox.Enabled = true;
            GameOver();
        }

        private void Play_Click(object sender, EventArgs e)
        {

            if (CorrectFields())
            {

                SetDimensions();
                TableMargins(height, width);

                if (firstPlay)
                {
                    StartGame();
                    firstPlay = false;

                    widthBox.Enabled = false;
                    heightBox.Enabled = false;
                }
                else
                    if (!firstPlay)
                {
                    ResetGame(width, height);
                    StartGame();
                }
                stopButton.Enabled = true;
            }

            List<SoundPlayer> startAudio = new List<SoundPlayer>();
            SoundPlayer firstAudioStart = new SoundPlayer(MineSweeper.Properties.Resources.assets_la_guerra_piu_totale);
            SoundPlayer secondAudioStart = new SoundPlayer(MineSweeper.Properties.Resources.assets_benson_ultimi);
            startAudio.Add(firstAudioStart);
            startAudio.Add(secondAudioStart);

            playsound(startAudio);

        }

        //timer
        private void timer1_Tick(object sender, EventArgs e)
        {
            seconds++;

            if (seconds == 60)
            {
                minutes++;
                seconds = 0;
            }

            if (seconds < 10)
            {
                textBox3.Text = $"{minutes.ToString()}:0{seconds.ToString()}";
            }
            else
            textBox3.Text = $"{minutes.ToString()}:{seconds.ToString()}";
        }

        private void playsound(List<SoundPlayer> filelist)
        {
            Random randomSound = new Random();
            int listLength = filelist.Count();
            int soundIndex = randomSound.Next(0, listLength);

            filelist[soundIndex].Play();
        }
    }
}

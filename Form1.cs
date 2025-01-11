using System;
using System.Drawing;
using System.Windows.Forms;

namespace Space_Defenders
{
    public partial class Form1 : Form
    {
        bool goLeft, goRight, shooting, isGameOver, hardcore;
        int score;
        int playerSpeed = 12;
        int highScore = 0;
        int enemySpeed, enemyLeftSpeed = 5, enemyLeftLimit = 697;
        int bulletSpeed;
        int leftCountA = 0, rightCountA = 0, leftCountB = 0, rightCountB = 0, leftCountC = 0, rightCountC = 0;
        Random rnd = new Random();

        public Form1()
        {
            InitializeComponent();
            resetGame();
        }

        private void mainGameTimerEvent(object sender, EventArgs e)
        {
            txtScore.Text = score.ToString();
            enemyOne.Top += enemySpeed;
            enemyTwo.Top += enemySpeed;
            enemyThree.Top += enemySpeed;

            if(enemyOne.Top > 710 || enemyTwo.Top > 710 || enemyThree.Top > 710)
            {
                if (score > highScore) { 
                    highScore = score;
                }
                gameOver();
            }

            // player movement logic starts

            if (goLeft && player.Left > 0) { 
                player.Left -= playerSpeed;
            }
            if (goRight && player.Left < 697)
            {
                player.Left += playerSpeed;
            }
            // player movement logic ends

            // enemy movement logic starts
            if (hardcore)
            {
                // This will skew to one side eventually simulating the enemy making their mind
                int leftOrRight = rnd.Next(0, 2) - leftCountA + rightCountA;
                if (leftOrRight <= 0 && enemyOne.Left > 0) {
                    leftCountA++; // A prior left movement makes subsequent movements more likely to be left
                    rightCountA = 0; // Helps in skewing probability for the other side
                    enemyOne.Left -= enemyLeftSpeed;
                } else if (enemyOne.Left < enemyLeftLimit) {
                    rightCountA++; // Similar to above but for right
                    leftCountA = 0;
                    enemyOne.Left += enemyLeftSpeed;
                }

                leftOrRight = rnd.Next(0, 2) - leftCountB + rightCountB;
                if (leftOrRight <= 0 && enemyTwo.Left > 0)
                {
                    leftCountB++;
                    rightCountB = 0;
                    enemyTwo.Left -= enemyLeftSpeed;
                }
                else if (enemyTwo.Left < enemyLeftLimit)
                {
                    rightCountC++;
                    leftCountC = 0;
                    enemyTwo.Left += enemyLeftSpeed;
                }

                leftOrRight = rnd.Next(0, 2) - leftCountC + rightCountC;
                if (leftOrRight <= 0 && enemyThree.Left > 0)
                {
                    leftCountC++;
                    rightCountC = 0;
                    enemyThree.Left -= enemyLeftSpeed;
                }
                else if (enemyThree.Left < enemyLeftLimit)
                {
                    rightCountC++;
                    leftCountC = 0;
                    enemyThree.Left += enemyLeftSpeed;
                }

                if (score % 3 == 0) { // Reset movement condition
                    leftCountA = 0;
                    rightCountA = 0;
                    leftCountB = 0;
                    rightCountB = 0;
                    leftCountC = 0;
                    rightCountC = 0;
                }
            }
            // enemy movement logic ends


            if (shooting) {
                bulletSpeed = 20;
                bullet.Top -= bulletSpeed;
            }
            else {
                bullet.Left = -300;
                bulletSpeed = 0;
            }

            if(bullet.Top < -30)
            {
                shooting = false;
            }

            // Bullet Collision Logic
            if (bullet.Bounds.IntersectsWith(enemyOne.Bounds)) {
                score++;
                // This below resets enemyOne and allows the player to shoot again
                enemyOne.Top = -450;
                enemyOne.Left = rnd.Next(20, 600);
                shooting = false ;
            }
            if (bullet.Bounds.IntersectsWith(enemyTwo.Bounds))
            {
                score++;
                // This below resets enemyOne and allows the player to shoot again
                enemyTwo.Top = -650;
                enemyTwo.Left = rnd.Next(20, 600);
                shooting = false;
            }
            if (bullet.Bounds.IntersectsWith(enemyThree.Bounds))
            {
                score++;
                // This below resets enemyOne and allows the player to shoot again
                enemyThree.Top = -750;
                enemyThree.Left = rnd.Next(20, 600);
                shooting = false;
            }


            if(score == 5)
            {
                this.BackColor = Color.MediumSlateBlue; // 1st Transition to Space
                enemySpeed = 8;
            }
            if(score == 10)
            {
                enemySpeed = 10;
                playerSpeed = 15; // F35 afterburner
                this.BackColor = Color.DarkSlateBlue; // Second Transition
            }
            if (score == 15)
            {
                enemySpeed = 12;
                this.BackColor = Color.MidnightBlue; // Third transition

                // Fighting SpaceShips!
                enemyOne.Image = Properties.Resources.enemy_spaceship;
                enemyTwo.Image = Properties.Resources.enemy_spaceship;
                enemyThree.Image = Properties.Resources.enemy_spaceship;
                bullet.Image = Properties.Resources.energy_bullet;
                enemyLeftLimit = 547;

                // Transition fix if enemy already spawned is at edge
                if (enemyOne.Left >= enemyLeftLimit)
                {
                    enemyOne.Left = enemyLeftLimit;
                }
                if (enemyTwo.Left >= enemyLeftLimit)
                {
                    enemyTwo.Left = enemyLeftLimit;
                }
                if (enemyThree.Left >= enemyLeftLimit)
                {
                    enemyThree.Left = enemyLeftLimit;
                }
            }
            if (score == 20)
            {
                this.BackColor = Color.Black; // SPACE!
                bulletSpeed = 25; // Plasma ammo more efficient in space!
                enemySpeed = 15;
                txtScore.ForeColor = Color.White; // Score Visibility
                txtHighScore.ForeColor = Color.White; // HighScore Visibility
            }
            if (score == 30) { 
                playerSpeed = 20;
                hardcore = true; // Enemy will be able to move left to right soon
            }
        }

        private void keyisdown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) { 
                goLeft = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
            }
        }
        private void keyisup(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }

            // Start Shooting bullet spot
            if(e.KeyCode == Keys.Space && !shooting)
            {
                shooting = true;

                bullet.Top = player.Top - 30;

                bullet.Left = player.Left + (player.Width/2);
            }

            if (e.KeyCode == Keys.Enter && isGameOver) {
                resetGame();
                isGameOver = false;
            }
        }

        private void resetGame()
        {
            gameTimer.Start();
            enemySpeed = 6;
            if (enemyLeftLimit == 697) {
                enemyOne.Left = rnd.Next(20, 600);
                enemyTwo.Left = rnd.Next(20, 600);
                enemyThree.Left = rnd.Next(20, 600);
            } else {
                enemyOne.Left = rnd.Next(20, enemyLeftLimit);
                enemyTwo.Left = rnd.Next(20, enemyLeftLimit);
                enemyThree.Left = rnd.Next(20, enemyLeftLimit);
            }

            enemyOne.Top = rnd.Next(0, 200) * -1;
            enemyTwo.Top = rnd.Next(0, 500) * -1;
            enemyThree.Top = rnd.Next(0, 900) * -1;

            score = 0;
            bulletSpeed = 0;
            bullet.Left = -300; // Only to spwan when the player shoots
            shooting = false;
            hardcore = false;  
            
            txtScore.Text = score.ToString();

            this.BackColor = Color.Cyan;
            enemyOne.Image = Properties.Resources.enemy;
            enemyTwo.Image = Properties.Resources.enemy;
            enemyThree.Image = Properties.Resources.enemy;
            bullet.Image = Properties.Resources.bullet;
            txtScore.ForeColor = Color.Black; // Reset Score Color to Black
            txtHighScore.ForeColor = Color.Black; // Reset HighScore Color to Black
            txtHighScore.Left = -300;
        }

        private void gameOver()
        {
            isGameOver = true;
            gameTimer.Stop();
            txtScore.Text += Environment.NewLine + "Game Over!" + Environment.NewLine + "Press Enter to try again.";
            txtHighScore.Left = 12; 
            txtHighScore.Text = "High Score: " + highScore.ToString();
        }
    }
}

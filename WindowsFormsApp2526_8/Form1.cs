using System;
using System.Collections.Generic;

using System.Drawing;

using System.Windows.Forms;
using System.Data.SQLite;
using System.Diagnostics.Eventing.Reader;


namespace WindowsFormsApp2526_8
{
    public partial class Form1 : Form
    {
        //Game
        private int timeLeft = 60;   // 60 seconds = 1 minute
        private Timer gameTimer;
        private bool Phase3 = false;
        private bool Phase4 = false;
        private bool Gameend = false;
        private bool Final = false;
        private string selected;
        
        //Player
        Random random = new Random();
        private int PlayerX = 500;
        private int PlayerY = 330;
        private int PlayerSize = 27;
        private int stepSpeed = 3;
        private bool mouthOpen = false;
        private bool moveUp = false;
        private bool moveDown = false;
        private bool moveLeft = false;
        private bool moveRight = false;
        private int angle;
        private int animationCounter = 0;
        private const int animationDelay = 7;


        //target
        private int objectX = 35;
        private int objectY = 50;
        private int velocity = 6;
        private int objectSize = 37;
        private bool Collision3 = false;
        private int targetDX = 1;  // 1 = right, -1 = left, 0 = no horizontal movement
        private int targetDY = 0;  // 1 = down, -1 = up,   0 = no vertical movement
        private Bitmap ghostImage = Properties.Resources.pacmanedible;



        private int avoidDistance = 100;
        private int wanderChangeInterval = 60;
        private int wanderCounter = 0;


        private int EnemyX = 50;
        private int EnemyY = 670;
        private int enemySize = 30;
        private int enemySpeed = 4;
        private int enemyDX = 0;   
        private int enemyDY = 0;   
        private int enemyChangeInterval = 40;
        private int enemyCounter = 0;
        private int enemyawareness = 150;
        private Bitmap enemyImage =Properties.Resources.ghostpacman1;

        List<Rectangle> walls = new List<Rectangle>();
        private bool moving;
        private Timer animationTimer;





        public Form1(string option)
        {
            InitializeComponent();
            
            String connectionString = "scoress.db";
            SQLiteConnection connection;
            selected = option;
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            animationTimer = new Timer();
            animationTimer.Interval = 1;
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
            this.Size = new Size(1000, 776);
            this.MaximumSize = new Size(1000, 776);
            this.MinimumSize = new Size(1000, 776);
            ghostImage.MakeTransparent(Color.Black);
            enemyImage.MakeTransparent(Color.Black);

            this.KeyDown += Form1_KeyDown;
            this.Paint += Form1_Paint;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawImage(ghostImage, objectX, objectY, objectSize, objectSize);
            SolidBrush redBrush = new SolidBrush(Color.Yellow);
            int mouth = 50;
            if (!mouthOpen)
            {
                g.FillEllipse(redBrush, PlayerX, PlayerY, PlayerSize, PlayerSize);
            }
            else
            {
                g.FillPie(Brushes.Yellow, PlayerX, PlayerY, PlayerSize, PlayerSize, angle - mouth, 360 - mouth * 2);
            }

            //enemy
            if (selected == "Hard")
            {
                g.DrawImage(enemyImage, EnemyX, EnemyY,enemySize,enemySize);
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (!Gameend)
            {
                animationTimer.Interval = 30;
                physics();
                
            }
            else if (Final)
            {   timer1.Stop(); 
                animationTimer.Stop();
                SaveScore(timeLeft);
                MessageBox.Show("Your score is: " + timeLeft);
            }

        }
        private bool isEnemyHit()
        {
            Rectangle enemyRect = new Rectangle(EnemyX, EnemyY, enemySize, enemySize);
            Rectangle playerRect = new Rectangle(PlayerX, PlayerY, PlayerSize, PlayerSize);
            return enemyRect.IntersectsWith(playerRect);
        }
        
        private bool IsPlayerHit()
        {
            Rectangle ghostRect = new Rectangle(objectX, objectY, objectSize, objectSize);
            Rectangle playerRect = new Rectangle(PlayerX, PlayerY, PlayerSize, PlayerSize);

            return ghostRect.IntersectsWith(playerRect);

        }
        private void AvoidPlayer()
        {
            int dx = objectX - PlayerX;
            int dy = objectY - PlayerY;


            if (dx > dy)
            {
                targetDX = dx > 0 ? 1 : -1;  // move away horizontally
                targetDY = 0;
            }
            else
            {
                targetDY = dy > 0 ? 1 : -1;  // move away vertically
                targetDX = 0;
            }
        }



        private void ChooseRandomDirection()
        {
            int choice = random.Next(4);

            switch (choice)
            {
                case 0: targetDX = 1; targetDY = 0; break; // right
                case 1: targetDX = -1; targetDY = 0; break; // left
                case 2: targetDX = 0; targetDY = 1; break; // down
                case 3: targetDX = 0; targetDY = -1; break; // up
            }
        }

        
        private void EnemyChooseRandomDirection()
        {
            int choice = random.Next(4); // only 4 directions

            switch (choice)
            {
                case 0: enemyDX = 1; enemyDY = 0; break; // RIGHT
                case 1: enemyDX = -1; enemyDY = 0; break; // LEFT
                case 2: enemyDX = 0; enemyDY = 1; break; // DOWN
                case 3: enemyDX = 0; enemyDY = -1; break; // UP
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft--;
            }
            else
            {
                timer1.Stop();
                animationTimer.Stop();
                Gameend = true;
                Final = true;
                MessageBox.Show("TIME'S UP!");
                

            }
        }
        

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeDatabase();
            this.Size = new Size(1000, 776);
            this.MaximumSize = new Size(1000, 776);
            this.MinimumSize = new Size(1000, 776);
            Drawwalls();
            

            timeLeft = 60;          
            timer1.Interval = 1000; 
            timer1.Start();

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.Up:
                    moveUp = true;
                    moveDown = false;
                    moveLeft = false;
                    moveRight = false;
                    moving = true;
                    angle = 360;

                    break;
                case Keys.Down:
                    moveDown = true;
                    moveUp = false;
                    moveLeft = false;
                    moveRight = false;
                    moving = true;
                    angle = 180;

                    break;
                case Keys.Left:
                    moveLeft = true;
                    moveRight = false;
                    moveUp = false;
                    moveDown = false;
                    moving = true;
                    angle = 270;

                    break;
                case Keys.Right:
                    moveRight = true;
                    moveLeft = false;
                    moveUp = false;
                    moveDown = false;
                    moving = true;
                    angle = 90;

                    break;

            }
        }

        private void InitializeDatabase()
        {
            string connectionString = "Data source=scoress.db;Version=3";
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();

            string createTable = "CREATE TABLE IF NOT EXISTS Scores (" +
                                 "id INTEGER PRIMARY KEY AUTOINCREMENT," +
                                 "score INTEGER NOT NULL);";

            SQLiteCommand command = new SQLiteCommand(createTable, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        private void SaveScore(int score)
        {
            string connectionString = "Data source=scoress.db;Version=3";
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();

            string insert = "INSERT INTO Scores(score) VALUES (@score)";
            SQLiteCommand command = new SQLiteCommand(insert, connection);
            command.Parameters.AddWithValue("@score", score);

            command.ExecuteNonQuery();

            connection.Close();
            
        }
        private void physics()
        {
            //Player
            int newX = PlayerX;
            int newY = PlayerY;

            if (moveUp)
            {
                newY -= stepSpeed;
            }
            else if (moveDown)
            {
                newY += stepSpeed;
            }
            else if (moveRight)
            {
                newX += stepSpeed;
            }
            else if (moveLeft)
            {
                newX -= stepSpeed;
            }
            //Objects
            int TargetX = objectX;
            int TargetY = objectY;
            int distX = Math.Abs(PlayerX - objectX);
            int distY = Math.Abs(PlayerY - objectY);
            int dist = distX + distY;

            // choose behavior: avoid or wander
            if (dist < avoidDistance)
            {
                AvoidPlayer();
            }
            else
            {
                wanderCounter++;
                if (wanderCounter >= wanderChangeInterval)
                {
                    ChooseRandomDirection();
                    wanderCounter = 0;
                }
            }
            // calculate next target position
            int nextGX = objectX + targetDX * velocity;
            int nextGY = objectY + targetDY * velocity;

            // CHECK COLLISONS
            bool Collison1 = false;
            bool Collison2 = false;
            Rectangle objectRect = new Rectangle(nextGX, nextGY, objectSize, objectSize);
            Rectangle PlayerRect = new Rectangle(newX, newY, PlayerSize, PlayerSize);
            Collision3 = false;
            foreach (var wall in walls)
            {
                if (objectRect.IntersectsWith(wall))
                {

                    Collision3 = true;

                }
                if (PlayerRect.IntersectsWith(wall))
                {
                    Rectangle overlap1 = Rectangle.Intersect(PlayerRect, wall);
                    if (overlap1.Width < overlap1.Height)
                    {
                        // Hit horizontally
                        Collison1 = true;
                        moving = false;
                    }
                    else
                    {
                        //Vertically
                        Collison2 = true;
                        moving = false;
                    }

                }

            }

            if (newX < 20)
            {
                newX = 950;
            }
            else if (newX > 950)
            {
                newX = 20;
            }
            // Player movement
            if (!Collison1)
            {
                PlayerX = newX;
            }
            if (newY >= 0 && newY + PlayerSize <= this.ClientSize.Height && !Collison2)
            {
                PlayerY = newY;
            }

            //Object movement
            if (Collision3)
            {
                if (dist < avoidDistance)
                {
                    AvoidPlayer();
                    int testGX = objectX + targetDX * velocity;
                    int testGY = objectY + targetDY * velocity;
                    Rectangle testRect = new Rectangle(testGX, testGY, objectSize, objectSize);

                    bool stillBlocked = false;
                    foreach (var wall in walls)
                    {
                        if (testRect.IntersectsWith(wall))
                        {
                            stillBlocked = true;
                            break;
                        }
                    }

                    if (stillBlocked)
                    {
                        ChooseRandomDirection();
                    }
                }
                else
                {
                    ChooseRandomDirection();
                }
            }
            else
            {
                objectX = nextGX;
                objectY = nextGY;
            }
            if (moving)
            {
                animationCounter++;
                if (animationCounter >= animationDelay)
                {
                    mouthOpen = !mouthOpen;
                    animationCounter = 0;
                }

            }
            if (selected == "Hard")
            {
                EnemyMovement();
                if (isEnemyHit())
                {
                    animationTimer.Stop();
                    timer1.Stop();
                    MessageBox.Show("You Lost!");
                }
            }
            if (IsPlayerHit())
            {

                if (!Phase3)
                {
                    objectX = 550;
                    objectY = 120;
                    PlayerX = 550;
                    PlayerY = 670;
                    Phase3 = true;
                }
                else if (!Phase4)
                {
                    objectX = 800;
                    objectY = 670;
                    PlayerX = 50;
                    PlayerY = 670;
                    Phase4 = true;
                }
                else
                {
                    Gameend = true;
                    Final = true;
                }


            }
            this.Invalidate();
        }

        private void EnemyMovement()
        {
            int dx = EnemyX - PlayerX;
            int dy = EnemyY - PlayerY;
            int dist = Math.Abs(dx) + Math.Abs(dy);
            if (dist < enemyawareness)
            {
                if (Math.Abs(dx) > Math.Abs(dy))
                {
                    enemyDX = dx < 0 ? 1 : -1;  // move towards the player horizontally
                    enemyDY = 0;
                }
                else
                {
                    enemyDY = dy < 0 ? 1 : -1;  // move towards the player vertically
                    enemyDX = 0;
                }
            }
            else
            {
                enemyCounter++;
                if (enemyCounter >= enemyChangeInterval)
                {
                    EnemyChooseRandomDirection();
                    enemyCounter = 0;
                }
            }

            
            int nextEX = EnemyX + enemyDX * enemySpeed;
            int nextEY = EnemyY + enemyDY * enemySpeed;

           
            Rectangle enemyRect = new Rectangle(nextEX, nextEY, enemySize, enemySize);

            bool blocked = false;
            foreach (var wall in walls)
            {
                if (enemyRect.IntersectsWith(wall))
                {
                    blocked = true;
                    break;
                }
            }


            if (blocked)
            {
                
                if (dist < enemyawareness)
                {
                    if (Math.Abs(dx) > Math.Abs(dy))
                    {
                        enemyDX = dx < 0 ? 1 : -1;
                        enemyDY = 0;
                    }
                    else
                    {
                        enemyDY = dy < 0 ? 1 : -1;
                        enemyDX = 0;
                    }
                }
                else
                {
                    EnemyChooseRandomDirection();
                }
                


                nextEX = EnemyX + enemyDX * enemySpeed;
                nextEY = EnemyY + enemyDY * enemySpeed;

                Rectangle newRect = new Rectangle(nextEX, nextEY, enemySize, enemySize);

                // If still blocked, random direction
                foreach (var wall in walls)
                {
                    if (newRect.IntersectsWith(wall))
                    {
                        EnemyChooseRandomDirection();
                        nextEX = EnemyX + enemyDX * enemySpeed;
                        nextEY = EnemyY + enemyDY * enemySpeed;
                        break;
                    }
                }
            }
            else {
                EnemyX = nextEX;
                EnemyY = nextEY;
            }

                
        }
        private void Drawwalls()
        {
            // -------------------------------------Walls --------------------------------------------
            //Grouping based on shapes/position

            //Rows
            walls.Add(new Rectangle(0, 0, 1000, 25));// top row
            walls.Add(new Rectangle(0, 713, 1000, 25));// bottom row

            //Collumns
            walls.Add(new Rectangle(0, 0, 35, 250));// most left top collumn
            walls.Add(new Rectangle(0, 450, 37, 265));// Most left bottom collumn
            walls.Add(new Rectangle(950, 0, 35, 250));  // most right top collumn
            walls.Add(new Rectangle(950, 450, 37, 265));// most right bottom collumn

            //Side center blocks
            walls.Add(new Rectangle(0, 230, 207, 94)); // most left center block-topside
            walls.Add(new Rectangle(780, 230, 204, 94));// most right centerblock top side
            walls.Add(new Rectangle(0, 367, 207, 97));  // most left bottom centerblock
            walls.Add(new Rectangle(780, 367, 204, 97));//most right centerblock bottom side

            // Sideways T shapes

            //left side
            walls.Add(new Rectangle(270, 230, 140, 26)); // row part of the T
            walls.Add(new Rectangle(270, 158, 38, 168));// Collumn part of the T
            //right side
            walls.Add(new Rectangle(578, 230, 138, 26)); // row part of the T
            walls.Add(new Rectangle(678, 158, 38, 168));// Collumn part of the T


            //Center Collumn lines
            walls.Add(new Rectangle(679, 367, 38, 93));
            walls.Add(new Rectangle(270, 367, 38, 93));

            //Bottom Row lines
            walls.Add(new Rectangle(272, 508, 138, 23)); //Left side
            walls.Add(new Rectangle(578, 508, 138, 23)); // Right side

            // L shapes
            //Left one
            walls.Add(new Rectangle(105, 507, 103, 23)); // row part
            walls.Add(new Rectangle(170, 507, 38, 92)); // collumn part
            // Right one
            walls.Add(new Rectangle(780, 507, 103, 23)); // row part
            walls.Add(new Rectangle(780, 507, 38, 92)); // collumn part

            //Upside down T
            //Left one
            walls.Add(new Rectangle(105, 640, 303, 30));// Row part of the T
            walls.Add(new Rectangle(270, 575, 37, 95)); // collumn part
            //Right one
            walls.Add(new Rectangle(580, 640, 303, 30));// Row part of the T
            walls.Add(new Rectangle(680, 575, 37, 95)); // collumn part

            // Little Row sticking out the wall
            walls.Add(new Rectangle(0, 573, 103, 30));
            walls.Add(new Rectangle(885, 573, 103, 30));

            // Top Center row lines
            walls.Add(new Rectangle(270, 70, 137, 45));
            walls.Add(new Rectangle(578, 70, 137, 45));


            // TOP side row lines, up side
            walls.Add(new Rectangle(103, 70, 100, 45));
            walls.Add(new Rectangle(783, 70, 100, 45));


            //TOP side row lines, down side
            walls.Add(new Rectangle(103, 162, 100, 24));
            walls.Add(new Rectangle(785, 162, 100, 24));


            // Center Cage
            walls.Add(new Rectangle(370, 380, 245, 15));// Row part of the Cage
            walls.Add(new Rectangle(370, 295, 74, 15));//entrance left
            walls.Add(new Rectangle(540, 295, 74, 15));//entrance right
            walls.Add(new Rectangle(370, 295, 18, 85));//left collumn
            walls.Add(new Rectangle(598, 295, 18, 85));//right collumn



            // Center T shapes

            // Ταβάνι T collumn ( the row is the top row )
            walls.Add(new Rectangle(475, 0, 37, 117));

            //Top T
            walls.Add(new Rectangle(375, 158, 235, 30));// Row part of the T
            walls.Add(new Rectangle(475, 158, 37, 98));// Collumn part of the T
            //Center T
            walls.Add(new Rectangle(375, 436, 235, 30));// Row part of the T
            walls.Add(new Rectangle(475, 436, 37, 98));// Collumn part of the T
            // Bottom T
            walls.Add(new Rectangle(375, 573, 235, 30));// Row part of the T
            walls.Add(new Rectangle(475, 573, 37, 98));// Collumn part of the T

            //---------------------------------------------------------------------------------------------------------
        }
    }
}

// Author: Dylan Nagel
// File Name: main.cs
// Project Name: IceColdButterBeer
// Description: This program is a video game version of the retro game ice cold beer, where you must get ball to designated areas by rolling it on a bar
 
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Helper;
using Animation2D;

namespace IceColdButterBeer
{
    public class Game1 : Game
    {
        //set graphics and spritebatch
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //Stores keyboard and mouse state
        KeyboardState kb;
        KeyboardState prevKb;
        MouseState mouse;
        MouseState prevMouse;

        //Stores random variable
        Random rng = new Random();

        //Stores all game states
        const int GAMEPLAY = 0;
        const int INSTRUCTIONS = 1;
        const int EXIT = 2;
        const int MENU = 3;
        const int GAME_CHOICE = 4;
        const int PAUSE = 5;
        const int ENDGAME = 6;
        const int USER_LOAD = 7;

        //Stores current game states
        int gameState = USER_LOAD;

        //Stores all game types
        const int NONE = -1;
        const int FIVE_GOAL = 0;
        const int LAVA_FLOOR = 1;
        const int DOWN_DIR = 2;
        const int BLITZ = 3;
        const int PAC_BALL = 4;
        const int FLAP_BALL = 5;

        //Stores current game type
        int gameType = NONE;

        //Stores screen width and height
        int screenWidth;
        int screenHeight;

        //Stores basic hole info
        Texture2D holeImg;
        Vector2[] holeLocs = new Vector2[31];
        Rectangle[] holeRec = new Rectangle[31];

        //Stores specified hole info
        readonly int[] rowCount = new int[] { 32, 21, 17, 9, 4, 1 };
        const int EXTRA_FALL_SPACE = 14;
        int goalHole;

        //Stores all coloured rectangle info
        Texture2D whiteSquareImg;
        Rectangle gameInfoRec;
        Rectangle blackScreenRec;

        //Stores arcade machine info
        Texture2D arcadeImg;
        Rectangle arcadeRec;

        //Stores arcade screen locations
        const int ARCADE_LEFT = 46;
        const int ARCADE_RIGHT = 436;
        const int ARCADE_TOP = 131;
        const int ARCADE_BOTTOM = 567;

        //Stores bar info
        Texture2D barImg;
        Vector2 barOrigin;
        Vector2 barMidPoint;
        Vector2 leftBarLoc;
        Vector2 rightBarLoc;
        float barAngle;
        const float BAR_SCALE = 1.47f;

        //Stores bar speed
        const int REL_SLOW = 0;
        const int REL_FAST = 1;
        const int BLITZ_SPD_MULT = 4;
        const int BALL_FAST = 0;
        readonly float[] BAR_UP_SPEED = new float[] { 1.2f, 0.2f };
        readonly float[] BAR_SHIFT_UP_SPEED = new float[] { 0.4f, 0.25f };
        readonly float[]RELOCATE_SPEED = new float[] {2.5f, 5};

        //stores bar max difference of sides
        const int MAX_INC_DIST = 100;

        //stores ball info
        Texture2D[] ballImg = new Texture2D[6];
        Vector2 ballOrigin;
        Vector2 ballLoc;
        const float BALL_SCALE = 0.16f;
        float ballHorLoc;
        float ballAngle;
        float ballTrans = 1;

        //stores extra ball distances
        const int CENT_DIST_TOL = 4;
        const int BALL_BOR_DIST = 5;

        //stores speed and forces
        const float HOR_ACCEL = 0.3f;
        const float TOLERANCE = 0.02f;
        const float FRICTION = 0.02f;
        const int BALL_SLOW_TIME = 3000;
        float ballHorSpeed;
        bool isBallSlow = false;

        //stores health info
        Rectangle[] hPRec = new Rectangle[3];
        float[] hPTrans = new float[] { 1, 1, 1 };

        //stores spacing between hearts 
        const int HP_SPACING = 27;

        //stores user hp
        const int MAX_HP = 3;
        int userhealth = MAX_HP;

        //stores game timer info
        Timer gameCountdown;
        Vector2 gameTimerLoc;
        const string NUM_TESTERS = ":0123456789";
        double prevTime;

        //stores game info font and colour
        SpriteFont gameInfoFont;
        Color gameInfoCol = new Color(255, 124, 48);

        //stores pause font and prompts
        SpriteFont pauseFont;
        string pauseToGamePromt = "Press ESC to return to gameplay";
        string pauseToMenuPromt = "Press ENTER to return to exit to menu";
        string exitGameWarn = "(Game info will be lost!)";

        //stores locations of pause prompts
        Vector2 pauseToGameLoc;
        Vector2 pauseToMenuLoc;
        Vector2 exitWarnLoc;

        //stores round number info
        int roundNum = 1;
        const int MAX_ROUND = 5;
        Vector2 roundNumLoc;

        //stores button images and rectangles
        Texture2D[,] menuBtns = new Texture2D[2, 3];
        Texture2D[] playAgainBtns = new Texture2D[2];
        Texture2D[] backToMenuBtns = new Texture2D[2];
        Texture2D[] backBtns = new Texture2D[2];
        Rectangle[] endgameBtnRec = new Rectangle[2];
        Rectangle[] menuBtnRec = new Rectangle[3];
        Rectangle instructBtnRec;

        //stores button array locations
        const int ON = 0;
        const int OFF = 1;
        const int PLAY_AGAIN_BTN_LOC = 0;
        const int MENU_BTN_LOC = 1;

        //stores spacing between menu buttons
        const int MEN_BUT_SPACE = 100;

        //stores if button is hovered
        bool[] hoverMenuBtns = new bool[3];
        bool hoverPlayAgainBtn = false;
        bool hoverMenuBtn = false;
        bool hoverBackBtn = false;

        //stores game title image and rectangle
        Texture2D gameTitleImg;
        Rectangle gameTitleRec;

        //stores lava image, animation and location
        Texture2D lavaImg;
        Animation lavaAnim;
        Vector2 lavaLoc;

        //stores lava speed
        const float LAVA_SPEED = 0.4f;

        //stores lava collision tolerance
        const int LAVA_BUB_SPACE = 19;

        //stores if object reset location is needed
        bool isBarMoved = true;
        bool isBallMoved = true;
        bool isLavaMoved = true;
        bool[] isRelPacSide = new bool[] { true, true, true };
        bool[] isRelPacUp = new bool[] { true, true, true };

        //slow time ine lava mode timer location
        Vector2 slowTimeRemainLoc;

        //stores buttons and locations for each game mode
        Texture2D[,] gameChoiceBtns = new Texture2D[2, 6];
        Rectangle[] gameChoiceRec = new Rectangle[6];
        Vector2[] ogGamChoiceLoc = new Vector2[6];

        //stores spacing for game choice buttons
        const float GAME_CHOICE_HOR_SPACE = 20.4f;
        const float GAME_CHOICE_VERT_SPACE = 10.45f;

        //stores whether game choice buttons are being hoverred
        bool[] hoverGameChoiceBtn = new bool[] { false, false, false, false, false, false };

        //stores star image and rectangle
        Texture2D[] starImg = new Texture2D[2]; 
        Rectangle[,] starRec = new Rectangle[6, 3];
        Rectangle[,] OGStarRec = new Rectangle[6, 3];

        //stores star spacing
        const int CHOICE_STAR_SPACE = 35;

        //store if stars are achieved
        bool[,] isStarAchieve = new bool[6, 3];

        //stores star achivement locations
        const int COMPLETION_ACH = 0;
        const int TIME_ACH = 1;
        const int HEALTH_ACH = 2;
        const int FIVE_ACH = 0;
        const int TEN_ACH = 1;
        const int TWENTY_ACH = 2;

        //stores max time limit in each game
        readonly int[] TIME_ACH_MAX = new int[] {60000, 30000, 60000, 40000 ,30000};

        //stores endgame star rec and achievment info
        Rectangle[] endgameStarRec = new Rectangle[3];
        bool[] endgameIsStarAch = new bool[] { false, false, false };

        //stores endgame prompt info
        SpriteFont endgameFont;
        string[] endgameMessage = new string[] { "Pathetic!", "Fine", "Not Bad", "Excellent!" };
        Vector2[] endgameMessageLoc = new Vector2[4];

        //stores spacing for stars
        const int endgameStarSpacing = 125;

        //stores array locations for animation, colour and reset for pacman enemies
        const int LEFT = 0;
        const int RIGHT = 1;
        const int UP = 2;
        const int DOWN = 3;
        const int RED = 0;
        const int ORANGE = 1;
        const int BLUE = 2;
        const int SIDE_CENT = 0;
        const int UP_CENT = 1;

        //stores pacman enemy images and locations
        Texture2D[,] pacEnImg = new Texture2D[4,3];
        Animation[,] pacAnim = new Animation[4,3];
        Vector2[] pacLoc = new Vector2[3];
        Vector2[] pacReloc = new Vector2[3];

        //stores pacman enemy states
        int[] pacState = new int[] { DOWN, RIGHT, DOWN };

        //stores pacman speed
        float[] pacSpeed = new float[] { 0.8f, 0.8f, 3 };
        const int PAC_REL_SPEED = 4;

        //stores if blue pacman enemy is relocated
        bool[] isBlPacCentre = new bool[] { true, true };

        //stores tolerance of ball to pacman enemy collision
        const int PAC_TOL = 10;

        //stores pacman border image and rectangle
        Texture2D pacBorderImg;
        Rectangle pacBorderRec;

        //stores red tube sizes and randomizer array locations
        const int SMALL = 0;
        const int MEDIUM = 1;
        const int LARGE = 2;
        const int MIN = 0;
        const int MAX = 1;

        //stores the array location of first red tube
        const int FIRST = 0;

        //stores tube passed achievment amounts
        const int TOP_TUBE_PASSED = 20;
        const int MID_TUBE_PASSED = 10;
        const int BOT_TUBE_PASSED = 5;

        //stores tube sizes and randomizers
        int[,] rngLimit = new int[2,3];
        int randomSize;
        int[] tubeSize = new int[3];

        //random number amounts for tube size
        const int SMALL_TUBE = 10;
        const int MED_TUBE = 66;

        //stores tube image, rectangle and location
        Texture2D[] tubeImg = new Texture2D[3];
        Rectangle[] tubeRec = new Rectangle[3];
        Vector2[] tubeLoc = new Vector2[3];
        int tubeSpacing = 300;

        //stores range of tube locations for it's holes
        int[] tubeHoleRange = new int[3];

        //stores tube speed 
        float tubeSpeed = 1;

        //stores tubes passed information
        int tubesPassed = 0;
        bool[] isTubePassed = new bool[3];

        //stores instructions page image and rectangle
        Texture2D instructionsImg;
        Rectangle instructionsRec;

        //stores lock image and if each gamemode is unlocked
        Texture2D lockImg;
        bool[] isGameUnlock = new bool[6];

        //stores unlock key image and rectangle
        Texture2D unlockKeyImg;
        Rectangle unlockKeyRec;

        //stores if key was used
        bool isKeyUsed = false;
        
        //stores the file reader and writeer
        static StreamReader inFile;
        static StreamWriter outFile;

        //stores data and line information for file reading and writing
        string[] data;
        string line;
        int dataCounter = 0;

        //stores font for promts
        SpriteFont promtDescFont;

        //stores yes and no button array locations
        const int YES = 0;
        const int NO = 1;

        //stores yes and no button images, rectangles and shadows
        Texture2D[,] userChoiceBtns = new Texture2D[2, 2];
        Rectangle[,] userChoiceBtnsRec = new Rectangle[2, 2];
        Rectangle[] dropShadowRec = new Rectangle[2];

        //stores if the buttons are being hoverred
        bool[] hoveruserLoadBtn = new bool[2];

        //stores user promts and their locations for user loading
        string userLoadPromt = "Continue from previous account?";
        string userLoadWarning = "(\"NO\" will override previous account)";
        Vector2 userLoadPromtLoc;
        Vector2 userLoadWarningLoc;

        //stores screen fade max and min values
        const int MAX_OPACITY = 1;
        const int MIN_OPACITY = 0;

        //stores screen transparency and rate of increase
        float screenTrans;
        const float SCREEN_TRANS_INCREASE = 0.05f;

        //stores whether or not game is fading in or out
        bool isFadingIn = false;
        bool isFadingOut = true;

        //stores rectangle for screen fade
        Rectangle screenFadeRec;

        //stores next game state
        int nextGameState;

        //stores music
        Song[] gameplayMusic = new Song[6];
        Song menuMusic;
        Song winningMusic;
        SoundEffect losingSound;
        SoundEffect butClickSound;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        //Pre: None
        //Post: None
        //Desc: Initializes game customization
        protected override void Initialize()
        {
            //Set screen width and height to preferred size
            this.graphics.PreferredBackBufferWidth = 480;
            this.graphics.PreferredBackBufferHeight = 700;

            //Set mouse to visible
            IsMouseVisible = true;

            //Apply all graphics changes
            this.graphics.ApplyChanges();

            //set screenwidth and height variables
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

            //set media player to repeating
            //MediaPlayer.IsRepeating = true;

            base.Initialize();
        }

        //Pre: None
        //Post: None
        //Desc: Loads game content
        protected override void LoadContent()
        {
            //set spritebatch variable
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //load arcade image and rectangle
            arcadeImg = Content.Load<Texture2D>("Images/Backgrounds/TransHalfArc");
            arcadeRec = new Rectangle(0, 0, screenWidth, screenHeight);

            //load white sqare image and it's rectangles
            whiteSquareImg = Content.Load<Texture2D>("Images/Backgrounds/WhiteRectangle");
            gameInfoRec = new Rectangle(ARCADE_LEFT, ARCADE_TOP, ARCADE_RIGHT - ARCADE_LEFT, whiteSquareImg.Height * 2);
            blackScreenRec = new Rectangle(ARCADE_LEFT, ARCADE_TOP, ARCADE_RIGHT - ARCADE_LEFT, ARCADE_BOTTOM - ARCADE_TOP);

            //load bar image and it's locations
            barImg = Content.Load<Texture2D>("Images/Sprites/DKBar");
            barOrigin = new Vector2((float)(barImg.Width * 0.5), (float)(barImg.Height * 0.5));
            barMidPoint = new Vector2((float)(screenWidth * 0.5), 500);
            leftBarLoc = new Vector2(ARCADE_LEFT, ARCADE_BOTTOM - barImg.Height * BAR_SCALE);
            rightBarLoc = new Vector2(ARCADE_RIGHT, ARCADE_BOTTOM - barImg.Height * BAR_SCALE);

            //load ball image for each game mode and its locations
            ballImg[FIVE_GOAL] = Content.Load<Texture2D>("Images/Sprites/DKBall");
            ballImg[LAVA_FLOOR] = Content.Load<Texture2D>("Images/Sprites/IceBall");
            ballImg[DOWN_DIR] = ballImg[FIVE_GOAL];
            ballImg[BLITZ] = Content.Load<Texture2D>("Images/Sprites/BlitzBall");
            ballImg[FLAP_BALL] = Content.Load<Texture2D>("Images/Sprites/FlappyBirdBall");
            ballImg[PAC_BALL] = Content.Load<Texture2D>("Images/Sprites/PacManBall");
            ballOrigin = new Vector2((float)(ballImg[FIVE_GOAL].Width * 0.5), (float)(ballImg[FIVE_GOAL].Height * 0.5));

            //load hole image and it's locations
            holeImg = Content.Load<Texture2D>("Images/Sprites/WhiteHole");
            holeLocs = new Vector2[] { new Vector2(330, 175), new Vector2(75, 165), new Vector2(200, 170), new Vector2(225, 195), new Vector2(130, 200), new Vector2(300, 200), new Vector2(370, 210), new Vector2(75, 220), new Vector2(200, 245), new Vector2(405, 250), new Vector2(170, 260), new Vector2(120, 270), new Vector2(40, 270), new Vector2(270, 270), new Vector2(310, 280), new Vector2(370, 280), new Vector2(60, 320), new Vector2(210, 330), new Vector2(310, 330), new Vector2(140, 340), new Vector2(385, 360), new Vector2(180, 380), new Vector2(260, 395), new Vector2(105, 410), new Vector2(370, 430), new Vector2(220, 440), new Vector2(160, 450), new Vector2(305, 450), new Vector2(50, 460), new Vector2(400, 480), new Vector2(320, 490) };

            //loop through all hole locations
            for (int i = 0; i < holeLocs.Length; i++)
            {
                //set rectangle for hole locations
                holeRec[i] = new Rectangle((int)holeLocs[i].X, (int)holeLocs[i].Y, (int)(ballImg[FIVE_GOAL].Width * 0.22), (int)(ballImg[FIVE_GOAL].Height * 0.22));
            }

            //loop through all health rectangles
            for (int i = 0; i < hPRec.Length; i++)
            {
                //set health rectangles
                hPRec[i] = new Rectangle(gameInfoRec.X + 10 + HP_SPACING * i, (int)(gameInfoRec.Center.Y - ballImg[FIVE_GOAL].Height * BALL_SCALE * 0.5), (int)(ballImg[FIVE_GOAL].Width * BALL_SCALE), (int)(ballImg[FIVE_GOAL].Height * BALL_SCALE));
            }

            //set game countdown timer
            gameCountdown = new Timer(120000, false);

            //load game info fonts and game info locations
            gameInfoFont = Content.Load<SpriteFont>("Fonts/GameInfo");
            gameTimerLoc = new Vector2(340, (int)(gameInfoRec.Center.Y - gameInfoFont.MeasureString(NUM_TESTERS).Y * 0.5));
            slowTimeRemainLoc = new Vector2((int)(screenWidth * 0.5 - gameInfoFont.MeasureString("0").X), gameTimerLoc.Y);
            roundNumLoc = new Vector2((int)(gameInfoRec.Center.X - gameInfoFont.MeasureString(Convert.ToString(roundNum)).X * 0.5), (int)(gameInfoRec.Center.Y - gameInfoFont.MeasureString(Convert.ToString(roundNum)).Y * 0.5));

            //load pause font and pause info locations
            pauseFont = Content.Load<SpriteFont>("Fonts/LargePrompt");
            pauseToGameLoc = new Vector2((float)((screenWidth - pauseFont.MeasureString(pauseToGamePromt).X) * 0.5), (float)((ARCADE_TOP + ARCADE_BOTTOM) * 0.5 - pauseFont.MeasureString(pauseToGamePromt).Y) - 30);
            pauseToMenuLoc = new Vector2((float)((screenWidth - pauseFont.MeasureString(pauseToMenuPromt).X) * 0.5), (float)((ARCADE_BOTTOM + ARCADE_TOP) * 0.5 + 30));
            exitWarnLoc = new Vector2((float)((screenWidth - gameInfoFont.MeasureString(exitGameWarn).X) * 0.5), (float)(pauseToMenuLoc.Y + pauseFont.MeasureString(pauseToMenuPromt).Y + 10));

            //load play button images on and off
            menuBtns[OFF, GAMEPLAY] = Content.Load<Texture2D>("Images/Sprites/Buttons/PlayButtonClick");
            menuBtns[ON, GAMEPLAY] = Content.Load<Texture2D>("Images/Sprites/Buttons/PlayButton");
            menuBtns[OFF, INSTRUCTIONS] = Content.Load<Texture2D>("Images/Sprites/Buttons/InstructionsButtonClick");
            menuBtns[ON, INSTRUCTIONS] = Content.Load<Texture2D>("Images/Sprites/Buttons/InstructionsButton");
            menuBtns[OFF, EXIT] = Content.Load<Texture2D>("Images/Sprites/Buttons/ExitButtonClick");
            menuBtns[ON, EXIT] = Content.Load<Texture2D>("Images/Sprites/Buttons/ExitButton");
            playAgainBtns[OFF] = Content.Load<Texture2D>("Images/Sprites/Buttons/PlayAgainButtonClick");
            playAgainBtns[ON] = Content.Load<Texture2D>("Images/Sprites/Buttons/PlayAgainButton");
            backToMenuBtns[OFF] = Content.Load<Texture2D>("Images/Sprites/Buttons/MenuButtonClick");
            backToMenuBtns[ON] = Content.Load<Texture2D>("Images/Sprites/Buttons/MenuButton");
            backBtns[OFF] = Content.Load<Texture2D>("Images/Sprites/Buttons/BackButtonClick");
            backBtns[ON] = Content.Load<Texture2D>("Images/Sprites/Buttons/BackButton");
            userChoiceBtns[OFF, NO] = Content.Load<Texture2D>("Images/Sprites/Buttons/NoButtonClick");
            userChoiceBtns[ON, NO] = Content.Load<Texture2D>("Images/Sprites/Buttons/NoButton");
            userChoiceBtns[OFF, YES] = Content.Load<Texture2D>("Images/Sprites/Buttons/YesButtonClick");
            userChoiceBtns[ON, YES] = Content.Load<Texture2D>("Images/Sprites/Buttons/YesButton");

            //load rectangle for menu buttons
            for (int i = 0; i < menuBtnRec.Length; i++)
            {
                menuBtnRec[i] = new Rectangle((int)((screenWidth - menuBtns[OFF, GAMEPLAY].Width * 0.6) * 0.5), ARCADE_TOP + 150 + i * MEN_BUT_SPACE, (int)(menuBtns[OFF, GAMEPLAY].Width * 0.6), (int)(menuBtns[OFF, GAMEPLAY].Height * 0.6));
            }

            //load rectangles for endgame buttons
            endgameBtnRec[PLAY_AGAIN_BTN_LOC] = new Rectangle(ARCADE_LEFT + 8, ARCADE_BOTTOM - menuBtnRec[0].Height - 10, menuBtnRec[0].Width, menuBtnRec[0].Height);
            endgameBtnRec[MENU_BTN_LOC] = new Rectangle(ARCADE_RIGHT - endgameBtnRec[PLAY_AGAIN_BTN_LOC].Width - 8, ARCADE_BOTTOM - menuBtnRec[0].Height - 10, endgameBtnRec[PLAY_AGAIN_BTN_LOC].Width, endgameBtnRec[PLAY_AGAIN_BTN_LOC].Height);

            //load game title image and rectangle
            gameTitleImg = Content.Load<Texture2D>("Images/Backgrounds/GameTitle");
            gameTitleRec = new Rectangle((int)((screenWidth - gameTitleImg.Width * 1.2) * 0.5), ARCADE_TOP + 30, (int)(gameTitleImg.Width * 1.2), (int)(gameTitleImg.Height * 1.2));

            //load lava image and animation
            lavaImg = Content.Load<Texture2D>("Images/Sprites/LavaBubble");
            lavaAnim = new Animation(lavaImg, 13, 1, 13, 0, 0, Animation.ANIMATE_FOREVER, 4, lavaLoc, 1.1f, true);
            lavaAnim.destRec.X = ARCADE_LEFT;

            //load buttons for gametypes
            gameChoiceBtns[OFF, FIVE_GOAL] = Content.Load<Texture2D>("Images/Sprites/Buttons/FiveChoiceClick");
            gameChoiceBtns[ON, FIVE_GOAL] = Content.Load<Texture2D>("Images/Sprites/Buttons/FiveChoice");
            gameChoiceBtns[OFF, LAVA_FLOOR] = Content.Load<Texture2D>("Images/Sprites/Buttons/LavaChoiceClick");
            gameChoiceBtns[ON, LAVA_FLOOR] = Content.Load<Texture2D>("Images/Sprites/Buttons/LavaChoice");
            gameChoiceBtns[OFF, DOWN_DIR] = Content.Load<Texture2D>("Images/Sprites/Buttons/DownChoiceClick");
            gameChoiceBtns[ON, DOWN_DIR] = Content.Load<Texture2D>("Images/Sprites/Buttons/DownChoice");
            gameChoiceBtns[OFF, PAC_BALL] = Content.Load<Texture2D>("Images/Sprites/Buttons/PacBallChoiceClick");
            gameChoiceBtns[ON, PAC_BALL] = Content.Load<Texture2D>("Images/Sprites/Buttons/PacBallChoice");
            gameChoiceBtns[OFF, BLITZ] = Content.Load<Texture2D>("Images/Sprites/Buttons/BlitzChoiceClick");
            gameChoiceBtns[ON, BLITZ] = Content.Load<Texture2D>("Images/Sprites/Buttons/BlitzChoice");
            gameChoiceBtns[OFF, FLAP_BALL] = Content.Load<Texture2D>("Images/Sprites/Buttons/FlapChoiceClick");
            gameChoiceBtns[ON, FLAP_BALL] = Content.Load<Texture2D>("Images/Sprites/Buttons/FlapChoice");

            //loop through two game choice recs
            for (int i = 0; i < gameChoiceRec.Length; i++)
            {
                //set game choice rectangle locations
                gameChoiceRec[i] = new Rectangle((int)(ARCADE_LEFT + GAME_CHOICE_HOR_SPACE + i * (gameChoiceBtns[OFF, FIVE_GOAL].Width * 0.6 + GAME_CHOICE_HOR_SPACE)), (int)(ARCADE_TOP + GAME_CHOICE_VERT_SPACE), (int)(gameChoiceBtns[OFF, FIVE_GOAL].Width * 0.6), (int)(gameChoiceBtns[OFF, FIVE_GOAL].Height * 0.6));

                //set variable to store original game choice locations
                ogGamChoiceLoc[i].Y = gameChoiceRec[i].Y;
            }

            //loop through two game choice recs
            for (int i = 0; i < 2; i++)
            {
                //set game choice rectangle locations
                gameChoiceRec[i + 2] = new Rectangle((int)(ARCADE_LEFT + GAME_CHOICE_HOR_SPACE + i * (gameChoiceBtns[OFF, FIVE_GOAL].Width * 0.6 + GAME_CHOICE_HOR_SPACE)), (int)(gameChoiceRec[FIVE_GOAL].Bottom + GAME_CHOICE_VERT_SPACE), (int)(gameChoiceBtns[OFF, FIVE_GOAL].Width * 0.6), (int)(gameChoiceBtns[OFF, FIVE_GOAL].Height * 0.6));

                //set variable to store original game choice locations
                ogGamChoiceLoc[i + 2].Y = gameChoiceRec[i + 2].Y;
            }

            //loop through two game choice recs
            for (int i = 0; i < 2; i++)
            {
                //set game choice rectangle locations
                gameChoiceRec[i + 4] = new Rectangle((int)(ARCADE_LEFT + GAME_CHOICE_HOR_SPACE + i * (gameChoiceBtns[OFF, FIVE_GOAL].Width * 0.6 + GAME_CHOICE_HOR_SPACE)), (int)(gameChoiceRec[BLITZ].Bottom + GAME_CHOICE_VERT_SPACE), (int)(gameChoiceBtns[OFF, FIVE_GOAL].Width * 0.6), (int)(gameChoiceBtns[OFF, FIVE_GOAL].Height * 0.6));

                //set variable to store original game choice locations
                ogGamChoiceLoc[i + 4].Y = gameChoiceRec[i + 4].Y;
            }

            //load star images on and off
            starImg[OFF] = Content.Load<Texture2D>("Images/Sprites/StarOn");
            starImg[ON] = Content.Load<Texture2D>("Images/Sprites/StarOff");

            //loops through star transparency's row
            for (int i = 0; i < starRec.GetLength(0); i++)
            {
                //loops thorugh star transparency's column
                for (int j = 0; j < starRec.GetLength(1); j++)
                {
                    //set star rectangle and original location
                    starRec[i, j] = new Rectangle(gameChoiceRec[i].Left + 33 + j * CHOICE_STAR_SPACE, (int)(gameChoiceRec[i].Bottom - starImg[OFF].Height * 0.05 - 20), (int)(starImg[OFF].Width * 0.05), (int)(starImg[OFF].Height * 0.05));
                    OGStarRec[i, j].Y = starRec[i, j].Y;
                }
            }

            //load engame font
            endgameFont = Content.Load<SpriteFont>("Fonts/EndgameFont");

            //loop thorugh endgame star rectangles
            for (int i = 0; i < endgameStarRec.Length; i++)
            {
                //set endgame star rectangles
                endgameStarRec[i] = new Rectangle(ARCADE_LEFT + 15 + i * endgameStarSpacing, ARCADE_TOP + 15, (int)(starImg[OFF].Width * 0.17), (int)(starImg[OFF].Height * 0.17));
            }

            //loop thorugh endgame message locs
            for (int i = 0; i < endgameMessageLoc.Length; i++)
            {
                //set endgame message locs
                endgameMessageLoc[i] = new Vector2((int)((screenWidth - endgameFont.MeasureString(endgameMessage[i]).X) * 0.5), endgameStarRec[0].Bottom + 20);
            }

            //load pacman border image and rectanlge
            pacBorderImg = Content.Load<Texture2D>("Images/Backgrounds/PacBorder");
            pacBorderRec = new Rectangle(ARCADE_LEFT, gameInfoRec.Bottom, ARCADE_RIGHT - ARCADE_LEFT, (int)(pacBorderImg.Height * 0.4));

            //load pacman images for each animation
            pacEnImg[LEFT, RED] = Content.Load<Texture2D>("Images/Sprites/EnLeftAnim");
            pacEnImg[RIGHT, RED] = Content.Load<Texture2D>("Images/Sprites/EnRightAnim");
            pacEnImg[UP, RED] = Content.Load<Texture2D>("Images/Sprites/EnUpAnim");
            pacEnImg[DOWN, RED] = Content.Load<Texture2D>("Images/Sprites/EnDownAnim");
            pacEnImg[LEFT, ORANGE] = Content.Load<Texture2D>("Images/Sprites/OrEnLeft");
            pacEnImg[RIGHT, ORANGE] = Content.Load<Texture2D>("Images/Sprites/OrEnRight");
            pacEnImg[UP, ORANGE] = Content.Load<Texture2D>("Images/Sprites/OrEnUp");
            pacEnImg[DOWN, ORANGE] = Content.Load<Texture2D>("Images/Sprites/OrEnDown");
            pacEnImg[LEFT, BLUE] = Content.Load<Texture2D>("Images/Sprites/BlEnLeft");
            pacEnImg[RIGHT, BLUE] = Content.Load<Texture2D>("Images/Sprites/BlEnRight");
            pacEnImg[UP, BLUE] = Content.Load<Texture2D>("Images/Sprites/BlEnUp");
            pacEnImg[DOWN, BLUE] = Content.Load<Texture2D>("Images/Sprites/BlEnDown");

            //set pacman enemy location
            pacLoc[RED] = new Vector2((float)(ARCADE_RIGHT - pacEnImg[pacState[RED], RED].Width * 0.04), (float)((ARCADE_BOTTOM + ARCADE_TOP) * 0.5));
            pacReloc[RED] = pacLoc[RED];
            pacLoc[ORANGE] = new Vector2(ARCADE_LEFT, (float)((ARCADE_BOTTOM + ARCADE_TOP) * 0.5));
            pacReloc[ORANGE] = pacLoc[ORANGE];
            pacLoc[BLUE] = new Vector2((float)(pacBorderRec.Center.X - pacEnImg[pacState[RED], RED].Width * 0.04 * 0.5), (float)(pacBorderRec.Center.Y - pacEnImg[pacState[RED], RED].Height * 0.04));
            pacReloc[BLUE] = pacLoc[BLUE];

            //loop through pacman animations in row
            for (int i = 0; i < pacAnim.GetLength(0); i++)
            {
                //loop through pacman animations in column
                for(int x = 0; x < pacAnim.GetLength(1); x++)
                {
                    pacAnim[i, x] = new Animation(pacEnImg[i, x], 2, 1, 2, 0, 0, Animation.ANIMATE_FOREVER, 5, pacReloc[x], 0.08f, true);
                }
            }

            //load tube images
            tubeImg[SMALL] = Content.Load<Texture2D>("Images/Sprites/RedTubeSmall");
            tubeImg[MEDIUM] = Content.Load<Texture2D>("Images/Sprites/RedTubeMedium");
            tubeImg[LARGE] = Content.Load<Texture2D>("Images/Sprites/RedTubeLarge");

            //loop through tube rectangles
            for(int i = 0; i < tubeRec.Length; i++)
            {
                //set tube rectangles
                tubeRec[i] = new Rectangle(0, 0, (int)(tubeImg[i].Width * 0.1), (int)(tubeImg[i].Height * 0.1));
            }    

            //set random tube location minimum and maximum values
            rngLimit[MIN, SMALL] = -429;
            rngLimit[MAX, SMALL] = -89;
            rngLimit[MIN, MEDIUM] = -396;
            rngLimit[MAX, MEDIUM] = -122;
            rngLimit[MIN, LARGE] = -359;
            rngLimit[MAX, LARGE] = -159;

            //set tube hole ranges
            tubeHoleRange[SMALL] = 24;
            tubeHoleRange[MEDIUM] = 58;
            tubeHoleRange[LARGE] = 95;

            //load instructions image and rectangle
            instructionsImg = Content.Load<Texture2D>("Images/Backgrounds/InstructionsScreen");
            instructionsRec = new Rectangle(ARCADE_LEFT, ARCADE_TOP, ARCADE_RIGHT - ARCADE_LEFT, (int)(instructionsImg.Height * 0.13));

            //load instructions button rectangle
            instructBtnRec = new Rectangle((int)((screenWidth - endgameBtnRec[PLAY_AGAIN_BTN_LOC].Width) * 0.5), instructionsRec.Bottom - menuBtnRec[0].Height - 15, endgameBtnRec[PLAY_AGAIN_BTN_LOC].Width, endgameBtnRec[PLAY_AGAIN_BTN_LOC].Height);

            //load lock image
            lockImg = Content.Load<Texture2D>("Images/Sprites/Lock");

            //load unlock key image and rectangle
            unlockKeyImg = Content.Load<Texture2D>("Images/Sprites/RedKey");
            unlockKeyRec = new Rectangle((int)((screenWidth - unlockKeyImg.Width * 0.2) * 0.5), gameChoiceRec[FLAP_BALL].Bottom + ARCADE_BOTTOM - ARCADE_TOP, (int)(unlockKeyImg.Width * 0.2), (int)(unlockKeyImg.Height * 0.2));

            //load promt font 
            promtDescFont = Content.Load<SpriteFont>("Fonts/PromtDescription");

            //load yes and no button rectangles and dropshadow rectangles
            userChoiceBtnsRec[ON, YES] = new Rectangle(ARCADE_LEFT + 20, ARCADE_BOTTOM - 100, (int)(userChoiceBtns[ON, YES].Width * 0.5), (int)(userChoiceBtns[ON, YES].Height * 0.5));
            userChoiceBtnsRec[OFF, YES] = new Rectangle(userChoiceBtnsRec[ON, YES].X + 3, userChoiceBtnsRec[ON, YES].Y + 3, userChoiceBtnsRec[ON, YES].Width, userChoiceBtnsRec[ON, YES].Height);
            userChoiceBtnsRec[ON, NO] = new Rectangle((int)(ARCADE_RIGHT - 20 - userChoiceBtns[ON, NO].Width * 0.5), ARCADE_BOTTOM - 100, (int)(userChoiceBtns[ON, NO].Width * 0.5), (int)(userChoiceBtns[ON, NO].Height * 0.5));
            userChoiceBtnsRec[OFF, NO] = new Rectangle(userChoiceBtnsRec[ON, NO].X + 3, userChoiceBtnsRec[ON, NO].Y + 3, userChoiceBtnsRec[ON, NO].Width, userChoiceBtnsRec[ON, NO].Height);

            //load user promt location
            userLoadPromtLoc = new Vector2((float)((screenWidth - pauseFont.MeasureString(userLoadPromt).X) * 0.5), ARCADE_TOP + 100);
            userLoadWarningLoc = new Vector2((float)((screenWidth - promtDescFont.MeasureString(userLoadWarning).X) * 0.5), userLoadPromtLoc.Y + 30);

            //set screen fade rectangle
            screenFadeRec = new Rectangle(ARCADE_LEFT, ARCADE_TOP, ARCADE_RIGHT - ARCADE_LEFT, ARCADE_BOTTOM - ARCADE_TOP);

            //load game music and sounds
            gameplayMusic[FIVE_GOAL] = Content.Load<Song>("Audio/Music/FiveDownMusic");
            gameplayMusic[LAVA_FLOOR] = Content.Load<Song>("Audio/Music/LavaMusic");
            gameplayMusic[DOWN_DIR] = gameplayMusic[FIVE_GOAL];
            gameplayMusic[BLITZ] = Content.Load<Song>("Audio/Music/BlitzMusic");
            gameplayMusic[PAC_BALL] = Content.Load<Song>("Audio/Music/PacMusic");
            gameplayMusic[FLAP_BALL] = Content.Load<Song>("Audio/Music/FlapMusic");
            menuMusic = Content.Load<Song>("Audio/Music/MenuMusic");
            winningMusic = Content.Load<Song>("Audio/Music/WinningMusic");
            losingSound = Content.Load<SoundEffect>("Audio/Sounds/LosingSound");
            butClickSound = Content.Load<SoundEffect>("Audio/Sounds/ButtonClick");
        }

        //Pre: functional gametime
        //Post: None
        //Desc: updates game logic
        protected override void Update(GameTime gameTime)
        {
            //check if game is fading in and not fading out
            if (isFadingIn && !isFadingOut)
            {
                //use subprogram to make screen fade
                FadeIn();
            }
            else
            {
                //check which game to update depending on gamestate
                switch (gameState)
                {
                    case MENU:
                        //use suprogram to update menu screen
                        UpdateMenu();
                        break;

                    case GAME_CHOICE:
                        //use a subprogram to update game choice screen
                        UpdateGameChoice();
                        break;

                    case INSTRUCTIONS:
                        //use a subprogram to update instructions screen
                        UpdateInstructions();
                        break;

                    case GAMEPLAY:
                        //use a subprogram to update gameplay
                        UpdateGameplay(gameTime);
                        break;

                    case PAUSE:
                        //run subprogram to update pause screen
                        UpdatePause();
                        break;

                    case ENDGAME:
                        //run subprogram to update endgame
                        UpdateEndgame();
                        break;

                    case USER_LOAD:
                        //run subprogram to update user load
                        UpdateUserLoad();
                        break;
                }
            }

            //checks if game is fading out
            if(isFadingOut)
            {
                //runs subprogram to fade out
                FadeOut();
            }

            //updates gametime
            base.Update(gameTime);
        }

        //Pre: functional gametime
        //Post: None
        //Desc: draws game screens
        protected override void Draw(GameTime gameTime)
        {
            //Draws a gray background behind the screen
            GraphicsDevice.Clear(Color.DarkSlateGray);

            //opens spritebatch for drawing
            spriteBatch.Begin();

            //draws screen depending on gamestate
            switch (gameState)
            {
                case MENU:
                    //runs subprogram to draw menu
                    DrawMenu();
                    break;

                case GAME_CHOICE:
                    //runs subprogram to draw game choice
                    DrawGameChoice();
                    break;

                case INSTRUCTIONS:
                    //runs subprogram to draw instructions
                    DrawInstructions();
                    break;

                case GAMEPLAY:
                    //runs subprogram to draw gameplay
                    DrawGameplay();
                    break;

                case PAUSE:
                    //runs subprogram to draw pause
                    DrawPause();
                    break;

                case ENDGAME:
                    //runs subprogram to draw endgame
                    DrawEndgame();
                    break;

                case USER_LOAD:
                    //runs subprogram to draw user load
                    DrawUserLoad();
                    break;
            }

            //draws black square over screen for fading
            spriteBatch.Draw(whiteSquareImg, screenFadeRec, Color.Black * screenTrans);

            //draws arcade image
            spriteBatch.Draw(arcadeImg, arcadeRec, Color.White);

            //ends spritebatch
            spriteBatch.End();

            //draws depending on gametime
            base.Draw(gameTime);
        }

        //Pre: None
        //Post: None
        //Desc: draws gameplay
        private void DrawGameplay()
        {
            //draws gameplay depending on game type
            switch (gameType)
            {
                case FIVE_GOAL:
                case DOWN_DIR:
                case BLITZ:
                    //runs subprogram to draw yellow hole
                    YellowHoleDraw();

                    //runs subprogram to draw bar and ball
                    BarBallDraw();

                    //draws game info and it's background
                    spriteBatch.Draw(whiteSquareImg, gameInfoRec, Color.Black * 0.7f);
                    HpRoundTimeDraw();
                    break;

                case LAVA_FLOOR:
                    //loops thorugh all hole rectangles
                    for (int i = 0; i < holeRec.Length; i++)
                    {
                        //draws holes
                        spriteBatch.Draw(holeImg, holeRec[i], Color.Black);
                    }

                    //draws bar and ball
                    BarBallDraw();

                    //draws game info and it's background
                    spriteBatch.Draw(whiteSquareImg, gameInfoRec, Color.Black * 0.7f);
                    HpTimeDraw();

                    //draws lava
                    lavaAnim.Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
                    break;

                case PAC_BALL:
                    //draws pacman border
                    spriteBatch.Draw(pacBorderImg, pacBorderRec, Color.White);

                    //runs subprogram to draw bar and ball
                    BarBallDraw();

                    //draws pacman enemies
                    pacAnim[pacState[RED], RED].Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
                    pacAnim[pacState[ORANGE], ORANGE].Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
                    pacAnim[pacState[BLUE], BLUE].Draw(spriteBatch, Color.White, Animation.FLIP_NONE);

                    //draws game info and it's background
                    spriteBatch.Draw(whiteSquareImg, gameInfoRec, Color.Black * 0.7f);
                    HpTimeDraw();
                    break;

                case FLAP_BALL:
                    //draws bar and ball
                    BarBallDraw();

                    //loops through tube images
                    for (int i = 0; i < tubeImg.Length; i++)
                    {
                        //draws tube images
                        spriteBatch.Draw(tubeImg[tubeSize[i]], tubeRec[i], Color.White);
                    }

                    //draws game info and it's background
                    spriteBatch.Draw(whiteSquareImg, gameInfoRec, Color.Black * 0.7f);
                    spriteBatch.DrawString(gameInfoFont, Convert.ToString(tubesPassed), roundNumLoc, gameInfoCol);
                    break;
            }
            
        }

        //Pre: functional gametime
        //Post: None
        //Desc: updates gameplay
        private void UpdateGameplay(GameTime gameTime)
        {
            //check if music is not currently playing
            if (MediaPlayer.State != MediaState.Playing)
            {
                //play music depending on game mode
                MediaPlayer.Play(gameplayMusic[gameType]);
            }

            //load keyboard state
            prevKb = kb;
            kb = Keyboard.GetState();

            //check if user clicked escape and not previously clicked it
            if (kb.IsKeyDown(Keys.Escape) && !prevKb.IsKeyDown(Keys.Escape))
            {
                //play button sound effect
                butClickSound.CreateInstance().Play();

                //pause music
                MediaPlayer.Pause();

                //set gamestate to pause
                gameState = PAUSE;
            }

            //update game mode depending on gametype
            switch (gameType)
            {
                case FIVE_GOAL:
                case DOWN_DIR:
                    //run subprogram to play five goal and down direction
                    FiveDownGoalPlay(gameTime);

                    //check if bar and ball has reset position
                    if (isBarMoved && isBallMoved)
                    {
                        //make ball trans 1
                        ballTrans = 1;
                    }
                    break;

                case LAVA_FLOOR:
                    //update lava animation
                    lavaAnim.Update(gameTime);

                    //run subprogram to play five goal and down direction
                    FiveDownGoalPlay(gameTime);

                    //check if ball slow time has exceeded maximum slow time
                    if (gameCountdown.GetTimePassed() - prevTime >= BALL_SLOW_TIME)
                    {
                        //make ball slow variable not true
                        isBallSlow = false;
                    }

                    //run subprogram to change lava game specified stats
                    LavaGameChanger();

                    //check if bar, ball and lava are moved
                    if (isBarMoved && isBallMoved && isLavaMoved)
                    {
                        //change ball transparency to 1
                        ballTrans = 1;
                    }
                    break;

                case BLITZ:
                    //update game countdown
                    gameCountdown.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

                    //check if bar and ball are moved
                    if (isBarMoved && isBallMoved)
                    {
                        //run subprogram for blitz bar movement
                        BlitzBarMovement();

                        //run subprogram change bar angle and mid point
                        ChangeBarStats();

                        //run subprogram to change ball's stats
                        BallLocChanger();

                        //run subprogram to check for colision between wall and ball and bar
                        BallWallCollision();
                        BarWallCollision();

                        //run subprogram to check for ball hole colision
                        BallHoleCol();
                    }
                    else
                    {
                        //run subprogram to relocate bar and abll
                        BarBallReloc();

                        //check if bar and ball are moved
                        if (isBarMoved && isBallMoved)
                        {
                            //set ball transparency to 1
                            ballTrans = 1;
                        }
                    }

                    //run subprogram to check if timer has ended
                    GameOverTimer();
                    break;

                case PAC_BALL:
                    //loop through row of pacman animation
                    for (int i = 0; i < pacAnim.GetLength(0); i++)
                    {
                        //loop through column of pacman animation
                        for(int x = 0; x < pacAnim.GetLength(1); x++)
                        {
                            //update pacman animations
                            pacAnim[i, x].Update(gameTime);
                        }
                    }

                    //update game countdown timer
                    gameCountdown.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

                    //check if bar and ball and pacman enemies are relocated
                    if (isBarMoved && isBallMoved && isRelPacSide[RED] && isRelPacUp[RED] && isRelPacSide[ORANGE] && isRelPacUp[ORANGE] && isRelPacSide[BLUE] && isRelPacUp[BLUE])
                    {
                        //run subprgram to change bar and ball info
                        BarBallChanger();

                        //run subprograms for pacman enemy movement
                        PacMovement(RED);
                        PacMovement(ORANGE);
                        BluePacMovement();

                        //run subprogram for ball hole collision
                        BallHoleCol();

                        //check if ball reached top of arcade screen
                        if (ballLoc.Y - ballImg[gameType].Height * BALL_SCALE * 0.5 <= gameInfoRec.Bottom)
                        {
                            //run subprogram to check for star achievement
                            StarAchieveCheck();
                        }
                    }
                    else
                    {
                        //run subprograms to relocate bar, ball, and pacman enemies
                        BarBallReloc();
                        PacReloc(RED);
                        PacReloc(ORANGE);
                        PacReloc(BLUE);
                    }

                    //run subprogram to check if timer reached maximum
                    GameOverTimer();
                    break;

                case FLAP_BALL:
                    //run subprgram to change bar and ball info
                    BarBallChanger();

                    //loops through tube rectangles
                    for (int i = 0; i < tubeRec.Length; i++)
                    {
                        //runs subprogram to move tubes
                        TubeMovement(i);

                        //runs subprogram to check for tube and ball collision
                        TubeBallCol(i);
                    }
                    break;
            }
        }

        //Pre: None
        //Post: None
        //Desc: draws menu screen
        private void DrawMenu()
        {
            //draws game title
            spriteBatch.Draw(gameTitleImg, gameTitleRec, Color.White);

            //loops thorough menu buttons column
            for(int i = 0; i < menuBtns.GetLength(1); i++)
            {
                //draws menu buttons
                spriteBatch.Draw(menuBtns[Convert.ToInt32(hoverMenuBtns[i]), i], menuBtnRec[i], Color.White);
            }
        }

        //Pre: None
        //Post: None
        //Desc: updates menu screen
        private void UpdateMenu()
        {
            //check if music is not currently playing
            if (MediaPlayer.State != MediaState.Playing)
            {
                //play music
                MediaPlayer.Play(menuMusic);
            }

            //updates mouse and keyboard state
            prevMouse = mouse;
            mouse = Mouse.GetState();
            prevKb = kb;
            kb = Keyboard.GetState();

            //loops through menu hover btns
            for(int i = 0; i < hoverMenuBtns.Length; i++)
            {
                //checks if menu buttons are being hoverred
                hoverMenuBtns[i] = menuBtnRec[i].Contains(mouse.Position);
            }

            //checks if user clicked mouse button
            if(mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //checks if user is hovering gameplay button
                if (hoverMenuBtns[GAMEPLAY])
                {
                    //play button sound effect
                    butClickSound.CreateInstance().Play();

                    //sets next gamestate to gamechoice
                    nextGameState = GAME_CHOICE;

                    //sets screen to fading
                    isFadingIn = true;
                }
                else if(hoverMenuBtns[INSTRUCTIONS])
                {
                    //play button sound effect
                    butClickSound.CreateInstance().Play();

                    //sets next gamestate to instructions
                    nextGameState = INSTRUCTIONS;

                    //sets screen fading to true
                    isFadingIn = true;
                }
                else if(hoverMenuBtns[EXIT])
                {
                    //play button sound effect
                    butClickSound.CreateInstance().Play();

                    //exits program
                    Exit();
                }
            }

            //checks if user is clicking escape
            if (kb.IsKeyDown(Keys.Escape) && !prevKb.IsKeyDown(Keys.Escape))
            {
                //play button sound effect
                butClickSound.CreateInstance().Play();

                //sets next gamestate to user load
                nextGameState = USER_LOAD;

                //sets screen fading to true
                isFadingIn = true;
            }  
        }

        //Pre: None
        //Post: None
        //Desc: draws instructions screen
        private void DrawInstructions()
        {
            //draws instructions
            spriteBatch.Draw(instructionsImg, instructionsRec, Color.White);

            //drwas back buttons
            spriteBatch.Draw(backBtns[Convert.ToInt32(hoverBackBtn)], instructBtnRec, Color.White);
        }

        //Pre: None
        //Post: None
        //Desc: updates instructions screen
        private void UpdateInstructions()
        {
            //check if music is not currently playing
            if (MediaPlayer.State != MediaState.Playing)
            {
                //play music
                MediaPlayer.Play(menuMusic);
            }

            //updates keyboard and mouse
            prevKb = kb;
            kb = Keyboard.GetState();
            prevMouse = mouse;
            mouse = Mouse.GetState();

            //checks if instructions button is being hoverred
            hoverBackBtn = instructBtnRec.Contains(mouse.Position);

            //checks if user is clicking mouse button
            if(mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //click if user is hovering back button
                if(hoverBackBtn)
                {
                    //play button sound effect
                    butClickSound.CreateInstance().Play();

                    //sets next gamestate to menu
                    nextGameState = MENU;

                    //sets game fading to true
                    isFadingIn = true;
                }
            }
            else if(kb.IsKeyDown(Keys.Escape) && !prevKb.IsKeyDown(Keys.Escape))
            {
                //play button sound effect
                butClickSound.CreateInstance().Play();

                //sets next gamestate to menu
                nextGameState = MENU;

                //sets game fading to true
                isFadingIn = true;
            }

            //checks if instructions is above arcade top
            if (instructionsRec.Top + (mouse.ScrollWheelValue - prevMouse.ScrollWheelValue) * 0.025 < ARCADE_TOP)
            {
                //changes instructions rectangle location depending on mouse scrolling
                instructionsRec.Y += (int)((mouse.ScrollWheelValue - prevMouse.ScrollWheelValue) * 0.025);
            }
            else
            {
                //sets instructions rectangle to top of screen
                instructionsRec.Y = ARCADE_TOP;
            }

            if (instructionsRec.Bottom + (mouse.ScrollWheelValue - prevMouse.ScrollWheelValue) * 0.025 > ARCADE_BOTTOM)
            {
                //changes instructions rectangle location depending on mouse scrolling
                instructionsRec.Y += (int)((mouse.ScrollWheelValue - prevMouse.ScrollWheelValue) * 0.025);
            }
            else
            {
                //sets instructions rectangle to top of screen
                instructionsRec.Y = ARCADE_BOTTOM - instructionsRec.Height;
            }

            //set instructions button to the bottom of instructions screen
            instructBtnRec.Y = instructionsRec.Bottom - instructBtnRec.Height - 15;
        }

        //Pre: None
        //Post: None
        //Desc: draws game choice screen
        private void DrawGameChoice()
        {
            //loop through all game choices
            for(int i = 0; i < gameChoiceBtns.GetLength(1); i ++)
            {
                //draws game choice buttons
                spriteBatch.Draw(gameChoiceBtns[Convert.ToInt32(hoverGameChoiceBtn[i]), i], gameChoiceRec[i], Color.White);
            }            

            //loop through the row of star rectangles
            for (int i = 0; i < starRec.GetLength(0); i++)
            {
                //loop through the column of star rectangles
                for(int j = 0; j < starRec.GetLength(1); j++)
                {
                    //draw star images
                    spriteBatch.Draw(starImg[Convert.ToInt32(isStarAchieve[i, j])], starRec[i, j], Color.White);
                }
            }

            //loop through game choice rectangles
            for (int i = 0; i < gameChoiceRec.Length; i++)
            {
                //check if game is not unlocked
                if(!isGameUnlock[i])
                {
                    //draw lock over game choice
                    spriteBatch.Draw(lockImg, gameChoiceRec[i], Color.White * 0.8f);
                }
            }

            //check if key was not used
            if(!isKeyUsed)
            {
                //draw key
                spriteBatch.Draw(unlockKeyImg, unlockKeyRec, Color.White);
            }
        }

        //Pre: None
        //Post: None
        //Desc: updates game choice screen
        private void UpdateGameChoice()
        {
            //check if music is not currently playing
            if (MediaPlayer.State != MediaState.Playing)
            {
                //play music
                MediaPlayer.Play(menuMusic);
            }

            //load keyboard and mouse state
            prevKb = kb;
            kb = Keyboard.GetState();
            prevMouse = mouse;
            mouse = Mouse.GetState();

            //check if escape was clicked and previously was not clicked
            if (kb.IsKeyDown(Keys.Escape) && !prevKb.IsKeyDown(Keys.Escape))
            {
                //play button sound effect
                butClickSound.CreateInstance().Play();

                //make next gamestate menu
                nextGameState = MENU;

                //set fading in variable to true
                isFadingIn = true;
            }

            //check if key has not been used
            if(!isKeyUsed)
            {
                //loop through game choice rectangles
                for (int i = 0; i < gameChoiceRec.Length; i++)
                {
                    //check if key rectangle is greater than top of arcade
                    if (unlockKeyRec.Y + (mouse.ScrollWheelValue - prevMouse.ScrollWheelValue) * 0.025 > ARCADE_TOP)
                    {
                        //check if game choice rectangles are greater than their original position
                        if (gameChoiceRec[i].Y + (mouse.ScrollWheelValue - prevMouse.ScrollWheelValue) * 0.025 > ogGamChoiceLoc[i].Y)
                        {
                            //sets game choice rectangle to it's original position
                            gameChoiceRec[i].Y = (int)ogGamChoiceLoc[i].Y;

                            //loops through star rectangels column
                            for (int x = 0; x < starRec.GetLength(1); x++)
                            {
                                //set star rectangel location to it's original locatoin
                                starRec[i, x].Y = OGStarRec[i, x].Y;
                            }
                        }
                        else
                        {
                            //moves game choice rectangle depending on scroll wheel
                            gameChoiceRec[i].Y += (int)((mouse.ScrollWheelValue - prevMouse.ScrollWheelValue) * 0.025);

                            //loops through star rectangle column
                            for (int x = 0; x < starRec.GetLength(1); x++)
                            {
                                //moves star rectangle depending on game choice rectangle
                                starRec[i, x].Y += (int)((mouse.ScrollWheelValue - prevMouse.ScrollWheelValue) * 0.025);
                            }
                        }
                    }
                }
            }

            //sets unlock key rectangle location
            unlockKeyRec.Y = gameChoiceRec[FLAP_BALL].Bottom + ARCADE_BOTTOM - ARCADE_TOP;

            //loops through game choice buttons
            for(int i = 0; i < hoverGameChoiceBtn.Length; i++)
            {
                //checks if game is unlocked
                if(isGameUnlock[i])
                {
                    //checks if user is hovering over game choice button
                    hoverGameChoiceBtn[i] = gameChoiceRec[i].Contains(mouse.Position);

                    //checks if user clicked mouse
                    if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                    {
                        //checks if user is hovering over game choice button
                        if (hoverGameChoiceBtn[i])
                        {
                            //play button sound effect
                            butClickSound.CreateInstance().Play();

                            //stop music
                            MediaPlayer.Stop();

                            //activate game countdown
                            gameCountdown.Activate();

                            //set next gamestate and gametype
                            nextGameState = GAMEPLAY;
                            gameType = i;

                            //make screen fade true
                            isFadingIn = true;

                            //reset gameplay
                            ResetGameplay(true);
                        }
                    }
                }
            }

            //checks if user clicked mouse button
            if(mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //checks if user is hovering over unlock key
                if(unlockKeyRec.Contains(mouse.Position))
                {
                    //play button sound effect
                    butClickSound.CreateInstance().Play();

                    //runs subprogram that unlocks all games
                    UnlockAllGames();

                    //set screen fading to true
                    isFadingIn = true;
                }
            }

        }

        //Pre: None
        //Post: None
        //Desc: draws engame screen
        private void DrawEndgame()
        {
            //loops through endgame stars
            for (int i = 0; i < endgameStarRec.Length; i++)
            {
                //draws stars
                spriteBatch.Draw(starImg[Convert.ToInt32(endgameIsStarAch[i])], endgameStarRec[i], Color.White);
            }

            //draws endgame info
            spriteBatch.DrawString(endgameFont, endgameMessage[Convert.ToInt32(endgameIsStarAch[COMPLETION_ACH]) + Convert.ToInt32(endgameIsStarAch[TIME_ACH]) + Convert.ToInt32(endgameIsStarAch[HEALTH_ACH])], endgameMessageLoc[Convert.ToInt32(endgameIsStarAch[COMPLETION_ACH]) + Convert.ToInt32(endgameIsStarAch[TIME_ACH]) + Convert.ToInt32(endgameIsStarAch[HEALTH_ACH])], Color.White);

            //draws endgame buttons
            spriteBatch.Draw(playAgainBtns[Convert.ToInt32(hoverPlayAgainBtn)], endgameBtnRec[PLAY_AGAIN_BTN_LOC], Color.White);
            spriteBatch.Draw(backToMenuBtns[Convert.ToInt32(hoverMenuBtn)], endgameBtnRec[MENU_BTN_LOC], Color.White);
        }

        //Pre: None
        //Post: None
        //Desc: updates endgame screen
        private void UpdateEndgame()
        {
            //check if music is playing
            if (MediaPlayer.State != MediaState.Playing)
            {
                //check if star is achieved
                if (endgameIsStarAch[COMPLETION_ACH])
                {
                    //play winning music
                    MediaPlayer.Play(winningMusic);
                }
            }
            //updates mouse state
            prevMouse = mouse;
            mouse = Mouse.GetState();

           //updates if user is hovering buttons
            hoverPlayAgainBtn = endgameBtnRec[PLAY_AGAIN_BTN_LOC].Contains(mouse.Position);
            hoverMenuBtn = endgameBtnRec[MENU_BTN_LOC].Contains(mouse.Position);

            //checks if user clicked mouse
            if(mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //check if user is hovering play again button
                if(hoverPlayAgainBtn)
                {
                    //play button sound effect
                    butClickSound.CreateInstance().Play();

                    //stop music
                    MediaPlayer.Stop();

                    //make next gamestate gameplay
                    nextGameState = GAMEPLAY;

                    //set screen fading to true
                    isFadingIn = true;
                }
                else if(hoverMenuBtn)
                {
                    //play button sound effect
                    butClickSound.CreateInstance().Play();

                    //stop music
                    MediaPlayer.Stop();

                    //set next gamestate to menu
                    nextGameState = MENU;

                    //set screen fading to true
                    isFadingIn = true;
                }
            }
        }

        //Pre: None
        //Post: None
        //Desc: allows bar movement depending on user input
        private void BarMovement()
        {
            //checks if bars are less than their max incline
            if (rightBarLoc.Y - leftBarLoc.Y <= MAX_INC_DIST)
            {
                //checks if user clicked W
                if (kb.IsKeyDown(Keys.W))
                {
                    //checks if user clicked shift
                    if (kb.IsKeyDown(Keys.LeftShift) || kb.IsKeyDown(Keys.RightShift))
                    {
                        //moves left side of bar up at shift speed
                        leftBarLoc.Y -= BAR_SHIFT_UP_SPEED[Convert.ToInt32(isBallSlow)];
                    }
                    else
                    {
                        //moves left side of bar up
                        leftBarLoc.Y -= BAR_UP_SPEED[Convert.ToInt32(isBallSlow)];
                    }
                }

                //checks if user clicked down
                if (kb.IsKeyDown(Keys.Down))
                {
                    //checks if user clicked shift
                    if (kb.IsKeyDown(Keys.LeftShift) || kb.IsKeyDown(Keys.RightShift))
                    {
                        //moves right bar down at shift speed
                        rightBarLoc.Y += BAR_SHIFT_UP_SPEED[Convert.ToInt32(isBallSlow)];
                    }
                    else
                    {
                        //moves right bar down
                        rightBarLoc.Y += BAR_UP_SPEED[Convert.ToInt32(isBallSlow)];
                    }
                }
            }

            //checks if distance between left bar and right bar is less than max
            if (leftBarLoc.Y - rightBarLoc.Y <= MAX_INC_DIST)
            {
                //checks if user clicked up
                if (kb.IsKeyDown(Keys.Up))
                {
                    //checks if user clicked shift
                    if (kb.IsKeyDown(Keys.LeftShift) || kb.IsKeyDown(Keys.RightShift))
                    {
                        //moves right bar up at shift speed
                        rightBarLoc.Y -= BAR_SHIFT_UP_SPEED[Convert.ToInt32(isBallSlow)];
                    }
                    else
                    {
                        //moves right bar up
                        rightBarLoc.Y -= BAR_UP_SPEED[Convert.ToInt32(isBallSlow)];
                    }
                }

                //check if user clicked S
                if (kb.IsKeyDown(Keys.S))
                {
                    //check if user clicked shift
                    if (kb.IsKeyDown(Keys.LeftShift) || kb.IsKeyDown(Keys.RightShift))
                    {
                        //moves left bar down at shift speed
                        leftBarLoc.Y += BAR_SHIFT_UP_SPEED[Convert.ToInt32(isBallSlow)];
                    }
                    else
                    {
                        //moves left bar down
                        leftBarLoc.Y += BAR_UP_SPEED[Convert.ToInt32(isBallSlow)];
                    }
                }
            }

            //run subprogram change bar angle and mid point
            ChangeBarStats();
        }

        //Pre: None
        //Post: None
        //Desc: changes bar stats for collision with wall
        private void BarWallCollision()
        {
            //check if bar is less than the top of screen
            if (leftBarLoc.Y < ARCADE_TOP + barImg.Height * BAR_SCALE * 0.5 + ballImg[gameType].Height * BALL_SCALE + gameInfoRec.Height)
            {
                //set left bar to top of screen
                leftBarLoc.Y = (float)(ARCADE_TOP + barImg.Height * BAR_SCALE * 0.5 + ballImg[gameType].Height * BALL_SCALE + gameInfoRec.Height);
            }
            else if (leftBarLoc.Y > ARCADE_BOTTOM - barImg.Height)
            {
                //set left bar to bottom of screen
                leftBarLoc.Y = ARCADE_BOTTOM - barImg.Height;
            }

            //check if right bar is less than top of screen
            if (rightBarLoc.Y < ARCADE_TOP + barImg.Height * BAR_SCALE * 0.5 + ballImg[gameType].Height * BALL_SCALE + gameInfoRec.Height)
            {
                //set right bar to top of screen
                rightBarLoc.Y = (float)(ARCADE_TOP + barImg.Height * BAR_SCALE * 0.5 + ballImg[gameType].Height * BALL_SCALE + gameInfoRec.Height);
            }
            else if (rightBarLoc.Y > ARCADE_BOTTOM - barImg.Height)
            {
                //set right bar to bottom of screen
                rightBarLoc.Y = ARCADE_BOTTOM - barImg.Height;
            }
        }

        //Pre: None
        //Post: None
        //Desc: changes ball stats for collision with wall
        private void BallWallCollision()
        {
            //check if ball is less than left screen
            if (ballHorLoc < ARCADE_LEFT + ballImg[gameType].Width * BALL_SCALE * 0.5)
            {
                //set ball location to left screen
                ballLoc.X = (float)(ARCADE_LEFT + ballImg[gameType].Width * BALL_SCALE * 0.5);
                ballHorLoc = ballLoc.X;

                //reset ball speed
                ballHorSpeed = 0;
            }
            else if (ballHorLoc > ARCADE_RIGHT - ballImg[gameType].Width * BALL_SCALE * 0.5)
            {
                //set ball location to right screen
                ballLoc.X = (float)(ARCADE_RIGHT - ballImg[gameType].Width * BALL_SCALE * 0.5);
                ballHorLoc = ballLoc.X;

                //reset ball speed
                ballHorSpeed = 0;
            }
            else
            {
                //change ball angle depending on speed
                ballAngle += (float)(ballHorSpeed * 0.1);
            }
        }


        //Pre: None
        //Post: None
        //Desc: changes game for ball and hole collision
        private void BallHoleCol()
        {
            //check for ball collision depending on game type
            switch(gameType)
            {
                case FIVE_GOAL:
                case DOWN_DIR:
                case BLITZ:
                    //loop through hole rectangles
                    for (int i = 0; i < holeRec.Length; i++)
                    {
                        //check if ball is inside of hole
                        if (Math.Pow(ballLoc.X - holeRec[i].Center.X, 2) + Math.Pow(ballLoc.Y - holeRec[i].Center.Y, 2) <= Math.Pow((holeRec[i].Width - ballImg[gameType].Width * BALL_SCALE) * 0.5 - EXTRA_FALL_SPACE, 2))
                        {
                            //check if ball is inside of goal hole
                            if (Math.Pow(ballLoc.X - holeRec[goalHole - 1].Center.X, 2) + Math.Pow(ballLoc.Y - holeRec[goalHole - 1].Center.Y, 2) <= Math.Pow((holeRec[goalHole - 1].Width - ballImg[gameType].Width * BALL_SCALE) * 0.5 - EXTRA_FALL_SPACE, 2))
                            {
                                //check if game is less than max round
                                if (roundNum < MAX_ROUND)
                                {
                                    //increase round counter
                                    roundNum++;

                                    //check if gametype is down direction
                                    if(gameType == DOWN_DIR)
                                    {
                                        //change goal hole
                                        goalHole = rng.Next(rowCount[MAX_ROUND - roundNum + 1], rowCount[MAX_ROUND - roundNum]);
                                    }
                                    else
                                    {
                                        //change goal hole
                                        goalHole = rng.Next(rowCount[roundNum], rowCount[roundNum - 1]);
                                    }

                                }
                                else
                                {
                                    //run subprogram to check for star achievment
                                    StarAchieveCheck();
                                }
                            }
                            else
                            {
                                //run subprogram to remove health
                                HealthRemover();
                            }

                            //runs subprogram to reset bar and ball location
                            ResetBarBall();
                        }
                    }
                    break;

                case LAVA_FLOOR:
                    //loop through hole rectangles
                    for (int i = 0; i < holeRec.Length; i++)
                    {
                        //check if ball is inside of hole
                        if (Math.Pow(ballLoc.X - holeRec[i].Center.X, 2) + Math.Pow(ballLoc.Y - holeRec[i].Center.Y, 2) <= Math.Pow(holeRec[i].Width - 3, 2))
                        {
                            //sets previous time to game countdown
                            prevTime = gameCountdown.GetTimePassed();

                            //set ball slow speed to true
                            isBallSlow = true;
                        }
                    }
                    break;

                case PAC_BALL:
                    //check if pacman enemy contains ball
                    if (pacAnim[pacState[RED], RED].destRec.Contains(ballLoc) || pacAnim[pacState[ORANGE], ORANGE].destRec.Contains(ballLoc) || pacAnim[pacState[BLUE], BLUE].destRec.Contains(ballLoc))
                    {
                        //set is moved variables to false
                        isBarMoved = false;
                        isBallMoved = false;
                        isRelPacSide[RED] = false;
                        isRelPacUp[RED] = false;
                        isRelPacSide[ORANGE] = false;
                        isRelPacUp[ORANGE] = false;
                        isRelPacSide[BLUE] = false;
                        isRelPacUp[BLUE] = false;

                        //run subrpgram to remove health
                        HealthRemover();

                        //run subprogram to reset bar and ball location
                        ResetBarBall();
                    }
                    break;
            }
        }

        //Pre: true or fale is time on boolean
        //Post: None
        //Desc: resets gameplay
        private void ResetGameplay(bool isTimeOn)
        {
            //reset gameplay depending on game type
            switch (gameType)
            {
                case FIVE_GOAL:
                case BLITZ:
                    //reset round number
                    roundNum = 1;

                    //randomize goal hole
                    goalHole = rng.Next(rowCount[roundNum], rowCount[roundNum - 1]);

                    //reset bar location
                    leftBarLoc.Y = ARCADE_BOTTOM - barImg.Height;
                    rightBarLoc.Y = leftBarLoc.Y;
                    barMidPoint.Y = leftBarLoc.Y;

                    //reset user health
                    userhealth = MAX_HP;

                    //loop through health transparency
                    for (int i = 0; i < hPTrans.Length; i++)
                    {
                        //reset health points transparency
                        hPTrans[i] = 1;
                    }

                    //reset ball transparency
                    ballTrans = 1;
                    break;

                case LAVA_FLOOR:
                    //set animation mode depending on if timer should be on
                    lavaAnim.isAnimating = isTimeOn;

                    //set is lava moved to true
                    isLavaMoved = true;

                    //reset ball slow variable
                    isBallSlow = false;

                    //set lava location
                    lavaLoc.Y = ARCADE_BOTTOM + 30;
                    lavaAnim.destRec.Y = (int)lavaLoc.Y;

                    //set bar location
                    leftBarLoc.Y = ARCADE_BOTTOM - barImg.Height;
                    rightBarLoc.Y = leftBarLoc.Y;
                    barMidPoint.Y = leftBarLoc.Y;

                    //reset user health
                    userhealth = MAX_HP;

                    //loop through health transparency
                    for (int i = 0; i < hPTrans.Length; i++)
                    {
                        //reset health points transparency
                        hPTrans[i] = 1;
                    }

                    //reset ball transparency
                    ballTrans = 1;
                    break;

                case DOWN_DIR:
                    //reset round number
                    roundNum = 1;

                    //randomize goal hole
                    goalHole = rng.Next(rowCount[MAX_ROUND - roundNum + 1], rowCount[MAX_ROUND - roundNum]);

                    //reset bar location
                    leftBarLoc.Y = (float)(ARCADE_TOP + barImg.Height * BAR_SCALE * 0.5 + ballImg[gameType].Height * BALL_SCALE + gameInfoRec.Height);
                    rightBarLoc.Y = leftBarLoc.Y;
                    barMidPoint.Y = (float)((leftBarLoc.Y + rightBarLoc.Y) * 0.5);

                    //reset user health
                    userhealth = MAX_HP;

                    //loop through health transparency
                    for (int i = 0; i < hPTrans.Length; i++)
                    {
                        //reset health points transparency
                        hPTrans[i] = 1;
                    }

                    //reset ball transparency
                    ballTrans = 1;
                    break;

                case PAC_BALL:
                    //loop through pacman animation row
                    for (int i = 0; i < pacAnim.GetLength(0); i++)
                    {
                        //loop through pacman animation column
                        for(int x = 0; x < pacAnim.GetLength(1); x++)
                        {
                            //set pacman animation depending on if time is on
                            pacAnim[i, x].isAnimating = isTimeOn;

                            //reset pac man animation location
                            pacAnim[i, x].destRec.X = (int)pacReloc[x].X;
                            pacAnim[i, x].destRec.Y = (int)pacReloc[x].Y;
                        }
                    }

                    //reset bar locations
                    leftBarLoc.Y = ARCADE_BOTTOM - barImg.Height;
                    rightBarLoc.Y = leftBarLoc.Y;
                    barMidPoint.Y = leftBarLoc.Y;

                    //loop throuhg pacman animation column
                    for (int i = 0; i < pacAnim.GetLength(1); i++)
                    {
                        //reset pacman location
                        pacLoc[i].X = (int)pacReloc[i].X;
                        pacLoc[i].Y = (int)pacReloc[i].Y;
                    }

                    //reset all pacman relocation
                    isRelPacSide[RED] = true;
                    isRelPacUp[RED] = true;
                    isRelPacSide[ORANGE] = true;
                    isRelPacUp[ORANGE] = true;
                    isRelPacSide[BLUE] = true;
                    isRelPacUp[BLUE] = true;

                    //loop through pacman states
                    for(int i = 0; i < pacState.Length; i++)
                    {
                        //rest pacman states
                        pacState[i] = DOWN;
                    }

                    //reset user health
                    userhealth = MAX_HP;

                    //loop through health transparency
                    for (int i = 0; i < hPTrans.Length; i++)
                    {
                        //reset health points transparency
                        hPTrans[i] = 1;
                    }

                    //reset ball transparency
                    ballTrans = 1;
                    break;

                case FLAP_BALL:
                    //loop through tube rectangles
                    for (int i = 0; i < tubeRec.Length; i++)
                    {
                        //reset tube sizes
                        ResetTubeSize(i);

                        //reset tube locations
                        tubeRec[i].Y = ARCADE_TOP - tubeRec[FIRST].Height - tubeSpacing * i;
                        tubeLoc[i].Y = tubeRec[i].Y;

                        //reset is tubes passed variable
                        isTubePassed[i] = false;
                    }

                    //reset bar location
                    leftBarLoc.Y = ARCADE_BOTTOM - barImg.Height;
                    rightBarLoc.Y = leftBarLoc.Y;
                    barMidPoint.Y = leftBarLoc.Y;

                    //reset tubes counters
                    tubeSpeed = 1;
                    tubesPassed = 0;

                    //reset ball transparency
                    ballTrans = 1;
                    break;
            }

            //reset bar stats
            barAngle = 0;
            ballHorSpeed = 0;

            //reset ball location
            ballHorLoc = (float)(screenWidth * 0.5);
            ballLoc.X = ballHorLoc;
            ballLoc.Y = (int)(rightBarLoc.Y - Math.Tan(barAngle) * (screenWidth - ballLoc.X) - ballImg[gameType].Height * BALL_SCALE + 5);

            //reset ball moved variables
            isBallMoved = true;
            isBarMoved = true;

            //reset game timer
            gameCountdown.ResetTimer(isTimeOn);

            //loop through endgame stars
            for(int i = 0; i < endgameIsStarAch.Length; i++)
            {
                //reset endgame star acheivement
                endgameIsStarAch[i] = false;
            }
        }

        //Pre: functional gametime
        //Post: None
        //Desc: updates five goal and down direction game play
        private void FiveDownGoalPlay(GameTime gameTime)
        {
            //update game timer
            gameCountdown.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //check if bar and ball are move
            if (isBarMoved && isBallMoved)
            {
                //run subprgram to change bar and ball info
                BarBallChanger();

                //run a subprogram to check for ball hole collision
                BallHoleCol();
            }
            else
            {
                //reset bar and ball location
                BarBallReloc();
            }

            //reset game over timer
            GameOverTimer();
        }

        //Pre: None
        //Post: None
        //Desc: creates ball friciont is specified circumstances
        private void BallFriction()
        {
            //check if bar angle is nearly flat
            if (Math.Abs(barAngle) <= 0.009)
            {
                //add ball speed 
                ballHorSpeed += -Math.Sign(ballHorSpeed) * FRICTION;

                //check if ball speed is less than tolerance
                if (Math.Abs(ballHorSpeed) <= TOLERANCE)
                {
                    //reset ball speed
                    ballHorSpeed = 0;
                }
            }
        }

        //Pre: None
        //Post: None
        //Desc: changes ball location for gameplay
        private void BallLocChanger()
        {
            //change ball locations
            ballHorSpeed += barAngle * HOR_ACCEL;
            ballHorLoc += ballHorSpeed;
            ballLoc.X = (int)ballHorLoc;
            ballLoc.Y = (int)(rightBarLoc.Y - Math.Tan(barAngle) * (screenWidth - ballLoc.X) - ballImg[gameType].Height * BALL_SCALE + BALL_BOR_DIST);
        }

        //Pre: None
        //Post: None
        //Desc: relocated bar and ball after life lossed
        private void BarBallReloc()
        {
            //relocation bar and ball depending on game type
            switch(gameType)
            {
                case FIVE_GOAL:
                case LAVA_FLOOR:
                case BLITZ:
                case PAC_BALL:
                    //check if bar is not moved
                    if(!isBarMoved)
                    {
                        //check if left bar is not at bottom of screen
                        if (leftBarLoc.Y != ARCADE_BOTTOM - barImg.Height || rightBarLoc.Y != ARCADE_BOTTOM - barImg.Height)
                        {
                            //lower bar location
                            leftBarLoc.Y += RELOCATE_SPEED[REL_SLOW];
                            rightBarLoc.Y += RELOCATE_SPEED[REL_SLOW];

                            //run subprogram to change bar stats
                            ChangeBarStats();

                            //check if left bar is greater than bottom of screen
                            if (leftBarLoc.Y > ARCADE_BOTTOM - barImg.Height)
                            {
                                //set left bar to bottom of screen
                                leftBarLoc.Y = ARCADE_BOTTOM - barImg.Height;
                            }

                            //check if right bar is greater than bottom of screen
                            if (rightBarLoc.Y > ARCADE_BOTTOM - barImg.Height)
                            {
                                //set left bar to bottom of screen
                                rightBarLoc.Y = ARCADE_BOTTOM - barImg.Height;
                            }
                        }
                        else
                        {
                            //set bar moved to true
                            isBarMoved = true;
                        }
                    }

                    //run subprogram to relocate ball
                    BallReloc();
                    break;

                case DOWN_DIR:
                    //check if bar is not at top of screen
                    if (leftBarLoc.Y != (float)(ARCADE_TOP + barImg.Height * BAR_SCALE * 0.5 + ballImg[gameType].Height * BALL_SCALE + gameInfoRec.Height) || rightBarLoc.Y != (float)(ARCADE_TOP + barImg.Height * BAR_SCALE * 0.5 + ballImg[gameType].Height * BALL_SCALE + gameInfoRec.Height))
                    {
                        //raise bar
                        leftBarLoc.Y -= RELOCATE_SPEED[REL_SLOW];
                        rightBarLoc.Y -= RELOCATE_SPEED[REL_SLOW];

                        //run subprogram change bar angle and mid point
                        ChangeBarStats();

                        //check if left bar is less than top of screen
                        if (leftBarLoc.Y < (float)(ARCADE_TOP + barImg.Height * BAR_SCALE * 0.5 + ballImg[gameType].Height * BALL_SCALE + gameInfoRec.Height))
                        {
                            //set left bar to top of screen
                            leftBarLoc.Y = (float)(ARCADE_TOP + barImg.Height * BAR_SCALE * 0.5 + ballImg[gameType].Height * BALL_SCALE + gameInfoRec.Height);
                        }

                        //check if right bar is less than top of screen
                        if (rightBarLoc.Y < (float)(ARCADE_TOP + barImg.Height * BAR_SCALE * 0.5 + ballImg[gameType].Height * BALL_SCALE + gameInfoRec.Height))
                        {
                            //set right bar to top of screen
                            rightBarLoc.Y = (float)(ARCADE_TOP + barImg.Height * BAR_SCALE * 0.5 + ballImg[gameType].Height * BALL_SCALE + gameInfoRec.Height);
                        }
                    }
                    else
                    {
                        //set is bar moved variable to true
                        isBarMoved = true;
                    }

                    //run subprogram to relocate ball
                    BallReloc();

                    //check if bar and ball are moved
                    if (isBarMoved && isBallMoved)
                    {
                        //set ball transparency to full
                        ballTrans = 1;
                    }
                    break;
            }
            
        }

        //Pre: None
        //Post: None
        //Desc: checks if games timer has finisehed
        private void GameOverTimer()
        {
            //check if timer finished
            if (gameCountdown.IsFinished())
            {
                //make game fading true
                isFadingIn = true;
            }
        }

        //Pre: None
        //Post: None
        //Desc: resets bar and ball stats after life lost
        private void ResetBarBall()
        {
            //rest bar and ball moved variables
            isBarMoved = false;
            isBallMoved = false;

            //rest ball stats
            ballTrans = 0.5f;
            ballHorSpeed = 0;
            ballAngle = 0;
        }

        //Pre: None
        //Post: None
        //Desc: draws goal hole
        private void YellowHoleDraw()
        {
            //loop through holes before goal hole
            for (int i = 0; i < goalHole; i++)
            {
                //draw holes
                spriteBatch.Draw(holeImg, holeRec[i], Color.Black);
            }

            //loop thorugh holes after goal hole
            for (int i = goalHole; i < holeRec.Length; i++)
            {
                //draw holes
                spriteBatch.Draw(holeImg, holeRec[i], Color.Black);
            }

            //draw goal hole
            spriteBatch.Draw(holeImg, holeRec[goalHole - 1], gameInfoCol);
        }

        //Pre: None
        //Post: None
        //Desc: draws bar and ball
        private void BarBallDraw()
        {
            //draw bar and ball
            spriteBatch.Draw(barImg, barMidPoint, null, Color.White, barAngle, barOrigin, BAR_SCALE, SpriteEffects.None, 0);
            spriteBatch.Draw(ballImg[gameType], ballLoc, null, Color.White * ballTrans, ballAngle, ballOrigin, BALL_SCALE, SpriteEffects.None, 0);
        }

        //Pre: None
        //Post: None
        //Desc: draws health points, round number and time
        private void HpRoundTimeDraw()
        {
            //loop through health rectangles
            for (int i = 0; i < hPRec.Length; i++)
            {
                //draw health
                spriteBatch.Draw(ballImg[gameType], hPRec[i], Color.White * hPTrans[i]);
            }

            //draw game info
            spriteBatch.DrawString(gameInfoFont, Convert.ToString(roundNum), roundNumLoc, gameInfoCol);
            spriteBatch.DrawString(gameInfoFont, gameCountdown.GetTimeRemainingAsString(Timer.FORMAT_MIN_SEC_MIL), gameTimerLoc, gameInfoCol);
        }

        //Pre: None
        //Post: None
        //Desc: draws health points and time
        private void HpTimeDraw()
        {
            //loop through health rectangles
            for (int i = 0; i < hPRec.Length; i++)
            {
                //Draw health
                spriteBatch.Draw(ballImg[gameType], hPRec[i], Color.White * hPTrans[i]);
            }

            //check if ball is slow
            if (isBallSlow)
            {
                //draw game info
                spriteBatch.DrawString(gameInfoFont, Convert.ToString(Math.Round((BALL_SLOW_TIME - (gameCountdown.GetTimePassed() - prevTime)) / 1000, 1)), slowTimeRemainLoc, gameInfoCol);
            }

            //draw timer
            spriteBatch.DrawString(gameInfoFont, gameCountdown.GetTimeRemainingAsString(Timer.FORMAT_MIN_SEC_MIL), gameTimerLoc, gameInfoCol);
        }

        //Pre: None
        //Post: None
        //Desc: updates health after is lost
        private void HealthRemover()
        {
            //remove health
            userhealth--;

            //check if health is less or equal to 0
            if (userhealth >= 0)
            {
                //set health transparency to 0
                hPTrans[userhealth] = 0;
            }
            else
            {
                //set screen fading to true
                isFadingIn = true;
            }
        }

        //Pre: None
        //Post: None
        //Desc: checks for star achievement
        private void StarAchieveCheck()
        {
            //check for star achievement depending on gametype
            switch(gameType)
            {
                case FLAP_BALL:
                    //check if tubes passed is greater than or equal to 20
                    if(tubesPassed >= TOP_TUBE_PASSED)
                    {
                        //set engame star achievment to true
                        endgameIsStarAch[FIVE_ACH] = true;
                        endgameIsStarAch[TEN_ACH] = true;
                        endgameIsStarAch[TWENTY_ACH] = true;

                        //set star achivement to true
                        isStarAchieve[gameType, FIVE_ACH] = true;
                        isStarAchieve[gameType, TEN_ACH] = true;
                        isStarAchieve[gameType, TWENTY_ACH] = true;
                    }

                    //check if tubes passed is greater than or equal to 10
                    if (tubesPassed >= MID_TUBE_PASSED)
                    {
                        //set engame star achievment to true
                        endgameIsStarAch[FIVE_ACH] = true;
                        endgameIsStarAch[TEN_ACH] = true;

                        //set star achivement to true
                        isStarAchieve[gameType, FIVE_ACH] = true;
                        isStarAchieve[gameType, TEN_ACH] = true;
                    }

                    //check if tubes passed is greater than or equal to 5
                    if (tubesPassed >= BOT_TUBE_PASSED)
                    {
                        //set engame star achievment to true
                        endgameIsStarAch[FIVE_ACH] = true;

                        //set star achivement to true
                        isStarAchieve[gameType, FIVE_ACH] = true;
                    }
                    break;

                case PAC_BALL:
                    //run subprogram to check for stars
                    DefaultStarCheck();

                    //unlock all games
                    UnlockAllGames();
                    break;

                default:
                    //run subprogram to check for stars
                    DefaultStarCheck();

                    //unlock next game mode
                    isGameUnlock[gameType + 1] = true;
                    break;
            }

            //run subprogram to save game
            GameSave();

            //set game fade to true
            isFadingIn = true;
        }

        //Pre: interger pacman colour from 0 to 2
        //Post: None
        //Desc: relocated pacman enemies
        private void PacReloc(int pacCol)
        {
            //check if pacman enemies are relocated
            if (!isRelPacUp[pacCol])
            {
                //check if pac enemy is at desired location
                if (pacAnim[pacState[pacCol], pacCol].destRec.Center.Y < pacReloc[pacCol].Y - PAC_TOL || pacAnim[pacState[pacCol], pacCol].destRec.Center.Y > pacReloc[pacCol].Y + PAC_TOL)
                {
                    //check if pacman animatin is less than pacman reloc
                    if (pacAnim[pacState[pacCol], pacCol].destRec.Center.Y <= pacReloc[pacCol].Y)
                    {
                        //add to pacman location
                        pacLoc[pacCol].Y += PAC_REL_SPEED;

                        //set pacstate to down
                        pacState[pacCol] = DOWN;
                    }
                    else if (pacAnim[pacState[pacCol], pacCol].destRec.Center.Y >= pacReloc[pacCol].Y)
                    {
                        //add to pacman location
                        pacLoc[pacCol].Y -= PAC_REL_SPEED;

                        //set pacstate to up
                        pacState[pacCol] = UP;
                    }
                }
                else
                {
                    //set pacman location to reloc location
                    pacLoc[pacCol].Y = pacReloc[pacCol].Y;

                    //set relocation variable to true
                    isRelPacUp[pacCol] = true;
                }

                //loop through pacman animation row
                for (int i = 0; i < pacAnim.GetLength(0); i++)
                {
                    //check if pacman is at relocation
                    pacAnim[i, pacCol].destRec.Y = (int)pacLoc[pacCol].Y;
                }
            }

            //check if pacman is at relocation location
            if (!isRelPacSide[pacCol])
            {
                //check if pacman colour is red
                if(pacCol == RED)
                {
                    //check if pacman is less than pacman relocation
                    if (pacAnim[pacState[RED], RED].destRec.X < pacReloc[RED].X)
                    {
                        //add to location
                        pacLoc[RED].X += PAC_REL_SPEED;

                        //set pacman state to red
                        pacState[RED] = RIGHT;
                    }
                    else
                    {
                        //set location to relocation
                        pacLoc[RED].X = pacReloc[RED].X;

                        //set pacman is relcoated variable to true
                        isRelPacSide[RED] = true;
                    }
                }
                else if (pacCol == ORANGE)
                {
                    //check if pacman is greater than relocation location
                    if (pacAnim[pacState[ORANGE], ORANGE].destRec.X > pacReloc[ORANGE].X)
                    {
                        //move pacman
                        pacLoc[ORANGE].X -= PAC_REL_SPEED;

                        //set pacman state to left
                        pacState[ORANGE] = LEFT;
                    }
                    else
                    {
                        //set pacman location to relocation location
                        pacLoc[ORANGE].X = pacReloc[ORANGE].X;

                        //set pacman is relocated variable to true
                        isRelPacSide[ORANGE] = true;
                    }
                }
                else
                {
                    //check if pacman location is greater than pacman tolerance
                    if (Math.Abs(pacAnim[pacState[BLUE], BLUE].destRec.X - pacReloc[BLUE].X) >= PAC_TOL)
                    {
                        //move pacman location
                        pacLoc[BLUE].X += -Math.Sign(pacAnim[pacState[BLUE], BLUE].destRec.X - pacReloc[BLUE].X) * PAC_REL_SPEED;
                    }
                    else
                    {
                        //set pacman location to relocation location
                        pacLoc[BLUE].X = pacReloc[BLUE].X;

                        //set pacman is relocated variable to true
                        isRelPacSide[BLUE] = true;
                    }
                }
            }

            //loop through pacman animation row
            for (int i = 0; i < pacAnim.GetLength(0); i++)
            {
                //set pacman animation location to relocation location
                pacAnim[i, pacCol].destRec.X = (int)pacLoc[pacCol].X;
            }

            //check if pacman is relocated on side and up
            if(isRelPacSide[pacCol] && isRelPacUp[pacCol])
            {
                //set pac state to down
                pacState[pacCol] = DOWN;

                //check if all game items are moved
                if (isBarMoved && isBallMoved && isRelPacSide[RED] && isRelPacUp[RED] && isRelPacSide[ORANGE] && isRelPacUp[ORANGE] && isRelPacSide[BLUE] && isRelPacUp[BLUE])
                {
                    //change ball transparency to 1
                    ballTrans = 1;
                }
            }
        }

        //Pre: interger pacman colour from 0 to 2
        //Post: None
        //Desc: updates pacman movement
        private void PacMovement(int pacCol)
        {
            //checkf if pacman colour is red
            if(pacCol == RED)
            {
                //check if ball location is less than or grater than pacman animation
                if (ballLoc.Y < pacAnim[pacState[pacCol], pacCol].destRec.Bottom && ballLoc.Y > pacAnim[pacState[pacCol], pacCol].destRec.Top)
                {
                    //run subprogram for pacman x direction movement
                    PacXMovement(pacCol);
                }
                else
                {
                    //run subprogram for pacman y direction movement
                    PacYMovement(pacCol);
                }
            }
            else
            {
                if(ballLoc.X > pacAnim[pacState[pacCol], pacCol].destRec.Left && ballLoc.X < pacAnim[pacState[pacCol], pacCol].destRec.Right)
                {
                    //run subprogram for pacman y direction movement
                    PacYMovement(pacCol);
                }
                else
                {
                    //run subprogram for pacman x direction movement
                    PacXMovement(pacCol);
                }
            }
        }

        //Pre: None
        //Post: None
        //Desc: updates blue pacman movement
        private void BluePacMovement()
        {
            //check if ball loc is less than blue pac loc
            if(ballLoc.Y + ballImg[gameType].Height * BALL_SCALE * 0.5 < pacBorderRec.Bottom)
            {
                //check if blue pacman is centred
                isBlPacCentre[SIDE_CENT] = false;
                isBlPacCentre[UP_CENT] = false;

                //check if ball loc is greater than blue pacman
                if(Math.Abs(ballLoc.X - pacAnim[pacState[BLUE], BLUE].destRec.Center.X) >= PAC_TOL)
                {
                    //check if ball loc is les sthan blue pacman
                    if (ballLoc.X > pacAnim[pacState[BLUE], BLUE].destRec.Center.X)
                    {
                        //move blue pacman loc
                        pacLoc[BLUE].X += pacSpeed[BLUE];

                        //set pacman state to right
                        pacState[BLUE] = RIGHT;
                    }
                    else
                    {
                        //move blue pacman loc
                        pacLoc[BLUE].X -= pacSpeed[BLUE];

                        //set pacman state to left
                        pacState[BLUE] = LEFT;
                    }
                }

                //check if ball loc is greater than blue pacman
                if (Math.Abs(ballLoc.Y - pacAnim[pacState[BLUE], BLUE].destRec.Center.Y) >= PAC_TOL)
                {
                    //check if ball loc is less than blue pacman
                    if (ballLoc.Y < pacAnim[pacState[BLUE], BLUE].destRec.Center.Y)
                    {
                        //move blue pacman loc
                        pacLoc[BLUE].Y -= pacSpeed[BLUE];

                        //set blue pacman state to up
                        pacState[BLUE] = UP;
                    }
                    else
                    {
                        //move blue pacman
                        pacLoc[BLUE].Y += pacSpeed[BLUE];

                        //set blue pacman state to down
                        pacState[BLUE] = DOWN;
                    }
                }
                else
                {
                    //set blue pacman location to ball
                    pacAnim[pacState[BLUE], BLUE].destRec.Y = (int)(ballLoc.Y - pacAnim[pacState[BLUE], BLUE].destRec.Height * 0.5);
                    pacLoc[BLUE].Y = pacAnim[pacState[BLUE], BLUE].destRec.Y;
                }
            }

            //loop through pacman animation row
            for (int i = 0; i < pacAnim.GetLength(0); i++)
            {
                //set pacman location to pacman loc
                pacAnim[i, BLUE].destRec.X = (int)pacLoc[BLUE].X;
                pacAnim[i, BLUE].destRec.Y = (int)pacLoc[BLUE].Y;
            }
        }

        //Pre: interger i from 0 to 2
        //Post: None
        //Desc: resets tube size after it leaves screen
        private void ResetTubeSize(int i)
        {
            //randomize tube size
            randomSize = rng.Next(0, 99 + 1);

            //check if tube random size is less than number
            if (randomSize < SMALL_TUBE)
            {
                //set tube size to small
                tubeSize[i] = SMALL;
            }
            else if (randomSize < MED_TUBE)
            {
                //set tube size to medium
                tubeSize[i] = MEDIUM;
            }
            else
            {
                //set tube size to large
                tubeSize[i] = LARGE;
            }

            //set tube rectangle to x location
            tubeRec[i].X = rng.Next(rngLimit[MIN, tubeSize[i]], rngLimit[MAX, tubeSize[i]]);
        }

        //Pre: None
        //Post: None
        //Desc: saves game to file
        private void GameSave()
        {
            //save game mode achievement to file
            try
            {
                //create or find file 
                outFile = File.CreateText("IsStarAchieve.txt");

                //loop through star achievement
                for (int i = 0; i < isStarAchieve.GetLength(0); i++)
                {
                    //write star achievment to file
                    outFile.WriteLine(isStarAchieve[i, 0] + "," + isStarAchieve[i, 1] + "," + isStarAchieve[i, 2] + "," + isGameUnlock[i]);
                }

                //close file
                outFile.Close();
            }
            catch (Exception)
            {

            }
        }

        //Pre: None
        //Post: None
        //Desc: makes screen darken when changing screens
        private void FadeIn()
        {
            //increase screen transparency
            screenTrans += SCREEN_TRANS_INCREASE;

            //check if screen transparecny is greater than max opacity
            if (screenTrans >= MAX_OPACITY)
            {
                //reset game mode depending on gamestate
                switch(gameState)
                {
                    case GAME_CHOICE:
                        //run subprgram to reset game choice
                        ResetGameChoice();
                        break;

                    case INSTRUCTIONS:
                        //set instructions rectangle to top of screen
                        instructionsRec.Y = ARCADE_TOP;
                        break;

                    case GAMEPLAY:
                        //set next gamestate to endgame
                        nextGameState = ENDGAME;

                        //stop music
                        MediaPlayer.Stop();

                        //check if first star achievement is achieved
                        if (endgameIsStarAch[COMPLETION_ACH])
                        {
                            //check if music is playing
                            if (MediaPlayer.State != MediaState.Playing)
                            {
                                //play winning music
                                MediaPlayer.Play(winningMusic);
                            }
                        }
                        else
                        {
                            //play losing sound effect
                            losingSound.CreateInstance().Play();
                        }
                        break;

                    case ENDGAME:
                        //depending on next gamestate reset gameplay
                        switch(nextGameState)
                        {
                            case GAMEPLAY:
                                //run subprogram to reset gameplay
                                ResetGameplay(true);
                                break;

                            case MENU:
                                //run subprogram to reset gameplay
                                ResetGameplay(false);
                                break;
                        }
                        break;
                }
                //set gamestate to next gamestate
                gameState = nextGameState;

                //set is fading variabels
                isFadingIn = false;
                isFadingOut = true;
            }
        }

        //Pre: None
        //Post: None
        //Desc: makes screen more transparenty when changing screens
        private void FadeOut()
        {
            //make screen more transparent
            screenTrans -= SCREEN_TRANS_INCREASE;

            //check if screen is less than minimum opacity
            if (screenTrans <= MIN_OPACITY)
            {
                //set is fading out variable to false
                isFadingOut = false;
            }
        }

        //Pre: None
        //Post: None
        //Desc: resets game choice screen
        private void ResetGameChoice()
        {
            //loop thorugh is game unlocked variabels
            for (int i = 0; i < isGameUnlock.Length; i++)
            {
                //set game choice rectangles to their original locations
                gameChoiceRec[i].Y = (int)ogGamChoiceLoc[i].Y;

                //loop thorugh star rectangle columb
                for (int x = 0; x < starRec.GetLength(1); x++)
                {
                    //set star rectangle locations to their original locations
                    starRec[i, x].Y = OGStarRec[i, x].Y;
                }
            }

            //run subprogram to save game to file
            GameSave();
        }

        //Pre: None
        //Post: None
        //Desc: unlocks all games
        private void UnlockAllGames()
        {
            //sets is key used variable to ture
            isKeyUsed = true;

            //loop through is game unlocked variabels
            for (int i = 1; i < isGameUnlock.Length; i++)
            {
                //set game unlocked variable to true
                isGameUnlock[i] = true;
            }
        }

        //Pre: None
        //Post: None
        //Desc: checks for star acheivment for all games except flap ball
        private void DefaultStarCheck()
        {
            //set star achievment to true
            isStarAchieve[gameType, COMPLETION_ACH] = true;
            endgameIsStarAch[COMPLETION_ACH] = true;

            //check if game countdown is less than max time
            if (gameCountdown.GetTimePassed() <= TIME_ACH_MAX[gameType])
            {
                //set star achievment to true
                isStarAchieve[gameType, TIME_ACH] = true;
                endgameIsStarAch[TIME_ACH] = true;
            }

            //check if user health is at max health
            if (userhealth == MAX_HP)
            {
                //set star achievment to true
                isStarAchieve[gameType, HEALTH_ACH] = true;
                endgameIsStarAch[HEALTH_ACH] = true;
            }
        }

        //Pre: None
        //Post: None
        //Desc: changes lava game stats
        private void LavaGameChanger()
        {
            //check if lava isnt moved
            if (!isLavaMoved)
            {
                //change lava location towards bottom of screen
                lavaLoc.Y += RELOCATE_SPEED[REL_FAST];
                lavaAnim.destRec.Y = (int)lavaLoc.Y;

                //check if lava animation is below bottom of arcade screen
                if (lavaAnim.destRec.Y >= ARCADE_BOTTOM)
                {
                    //set lava loc to specified location under screen
                    lavaLoc.Y = ARCADE_BOTTOM + 30;
                    lavaAnim.destRec.Y = (int)lavaLoc.Y;

                    //set is lava moved variable to true
                    isLavaMoved = true;
                }
            }
            else
            {
                //raise lava towards top of screen
                lavaLoc.Y -= LAVA_SPEED;
                lavaAnim.destRec.Y = (int)lavaLoc.Y;

                //check for ball collision
                if (ballLoc.Y - ballImg[gameType].Height * BALL_SCALE * 0.5 <= gameInfoRec.Bottom)
                {
                    //run subprogram to check how many stars were achieved
                    StarAchieveCheck();
                }
                else if (ballLoc.Y + ballImg[gameType].Height * BALL_SCALE * 0.5 >= lavaAnim.destRec.Y + LAVA_BUB_SPACE)
                {
                    //run subprogram to start resetting bar and ball location
                    ResetBarBall();

                    //set ball speed variable to false
                    isBallSlow = false;

                    //set lava moved variable to false
                    isLavaMoved = false;

                    //run subprogram to remove health
                    HealthRemover();
                }
            }
        }

        //Pre: None
        //Post: None
        //Desc: changes bar stats
        private void ChangeBarStats()
        {
            //change bar angle and midpoint
            barAngle = -(float)Math.Atan2(leftBarLoc.Y - rightBarLoc.Y, screenWidth);
            barMidPoint.Y = (int)((leftBarLoc.Y + rightBarLoc.Y) * 0.5);
        }

        //Pre: None
        //Post: None
        //Desc: moves bar depending on user movement for blitz game mode
        private void BlitzBarMovement()
        {
            //check if the distance between left and right bar are less than maximum
            if (rightBarLoc.Y - leftBarLoc.Y <= MAX_INC_DIST)
            {
                //check if user is clicking "W"
                if (kb.IsKeyDown(Keys.W))
                {
                    //raise left side of bar 
                    leftBarLoc.Y -= BAR_UP_SPEED[BALL_FAST] * BLITZ_SPD_MULT;
                }
                //check if user is clicking "DOWN"
                if (kb.IsKeyDown(Keys.Down))
                {
                    //lower right side of bar
                    rightBarLoc.Y += BAR_UP_SPEED[BALL_FAST] * BLITZ_SPD_MULT;
                }
            }

            //check if the distance between right and left bar are less than maximum
            if (leftBarLoc.Y - rightBarLoc.Y <= MAX_INC_DIST)
            {
                //check if user is clikcing "UP"
                if (kb.IsKeyDown(Keys.Up))
                {
                    //raise right side of bar
                    rightBarLoc.Y -= BAR_UP_SPEED[BALL_FAST] * BLITZ_SPD_MULT;
                }
                //check if user is clicking "S"
                if (kb.IsKeyDown(Keys.S))
                {
                    //lower left side of bar
                    leftBarLoc.Y += BAR_UP_SPEED[BALL_FAST] * BLITZ_SPD_MULT;
                }
            }
        }

        //Pre: None
        //Post: None
        //Desc: change bar and ball for gameplay
        private void BarBallChanger()
        {
            //run subprogram for bar movement
            BarMovement();

            //run subprogram for ball friction
            BallFriction();

            //run subprogram for ball stat changer
            BallLocChanger();

            //run subpreogram for ball and bar wall collision
            BallWallCollision();
            BarWallCollision();
        }

        //Pre: interger i from 0 to 1
        //Post: None
        //Desc: check for tube and ball collision
        private void TubeBallCol(int i)
        {
            //checks for tube ball collision
            if (tubeRec[i].Contains(ballLoc))
            {
                //checks if ball is in tube hole
                if (ballLoc.X > tubeRec[i].Center.X - tubeHoleRange[tubeSize[i]] && ballLoc.X < tubeRec[i].Center.X + tubeHoleRange[tubeSize[i]])
                {
                    //checks if ball is above tube
                    if (ballLoc.Y - tubeSpeed <= tubeRec[i].Y && !isTubePassed[i])
                    {
                        //adds one to tubes passed trackers
                        tubesPassed++;
                        isTubePassed[i] = true;

                        //increases tube speed
                        tubeSpeed += 0.05f;
                    }
                }
                else
                {
                    //check for star achivement
                    StarAchieveCheck();
                }
            }
        }

        //Pre: interger i from 0 to 1
        //Post: None
        //Desc: updates tubes location
        private void TubeMovement(int i)
        {
            //moves tubes downards
            tubeLoc[i].Y += tubeSpeed;
            tubeRec[i].Y = (int)tubeLoc[i].Y;

            //checks if tubes are below arcade screen
            if (tubeRec[i].Y >= ARCADE_BOTTOM)
            {
                //resets tube size
                ResetTubeSize(i);

                //sets tube passed variabel to false
                isTubePassed[i] = false;

                //sets new location for tube above arcade screen
                tubeRec[i].Y = ARCADE_TOP - 450;
                tubeLoc[i].Y = tubeRec[i].Y;
            }
        }

        //Pre: None
        //Post: None
        //Desc: updates pause screen
        private void UpdatePause()
        {
            //read keyboard state
            prevKb = kb;
            kb = Keyboard.GetState();

            //check if user clicked a key and didnt previously click it
            if (kb.IsKeyDown(Keys.Escape) && !prevKb.IsKeyDown(Keys.Escape))
            {
                //play button sound effect
                butClickSound.CreateInstance().Play();

                //resume music
                MediaPlayer.Resume();

                //set gamestate to gameplay
                gameState = GAMEPLAY;
            }
            else if (kb.IsKeyDown(Keys.Enter) && !prevKb.IsKeyDown(Keys.Enter))
            {
                //set next game state to MENU
                nextGameState = MENU;

                //make fading true
                isFadingIn = true;
            }
        }

        //Pre: None
        //Post: None
        //Desc: updates user load screen
        private void UpdateUserLoad()
        {
            //check if music is currently playing and gamestate is not gameplay or pause
            if (MediaPlayer.State != MediaState.Playing && gameState != GAMEPLAY && gameState != PAUSE)
            {
                //play menu music
                MediaPlayer.Play(menuMusic);
            }

            //get mouse state
            prevMouse = mouse;
            mouse = Mouse.GetState();

            //loops through the hover buttons
            for (int i = 0; i < hoveruserLoadBtn.Length; i++)
            {
                //checks if user is hovering a user choice button
                hoveruserLoadBtn[i] = userChoiceBtnsRec[ON, i].Contains(mouse.Position);
            }

            //checks if user clicked mouse button and didnt previously click it
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //checks if user is hovering the yes button
                if (hoveruserLoadBtn[YES])
                {
                    //play button sound effect
                    butClickSound.CreateInstance().Play();

                    //Read game mode acheivement from file
                    try
                    {
                        //sets infile to txt file
                        inFile = File.OpenText("IsStarAchieve.txt");

                        //loops through file until it's end
                        while (!inFile.EndOfStream)
                        {
                            //updates line and data
                            line = inFile.ReadLine();
                            data = line.Split(',');

                            //loops through column of star's acheived
                            for (int i = 0; i < isStarAchieve.GetLength(1); i++)
                            {
                                //gets star acheivement information from file
                                isStarAchieve[dataCounter, i] = Convert.ToBoolean(data[i]);
                            }

                            //gets game unlock information from file
                            isGameUnlock[dataCounter] = Convert.ToBoolean(data[isStarAchieve.GetLength(1)]);

                            //adds to data counter
                            dataCounter++;
                        }

                        //closes file reading
                        inFile.Close();

                        //checks if last game is unlocked
                        if (isGameUnlock[isGameUnlock.Length - 1])
                        {
                            //sets key to used
                            isKeyUsed = true;
                        }

                        //sets next game state to menu
                        nextGameState = MENU;

                        //sets if game is fading to true
                        isFadingIn = true;
                    }
                    catch (FileNotFoundException)
                    {
                        //makes next gamestate menu
                        nextGameState = MENU;

                        //makes is fading in true
                        isFadingIn = true;
                    }
                    catch (Exception)
                    {
                        //makes next gamestate menu
                        nextGameState = MENU;

                        //makes is fading in true
                        isFadingIn = true;
                    }


                    //resets data counter variable
                    dataCounter = 0;
                }
                else if (hoveruserLoadBtn[NO])
                {
                    //play button sound effect
                    butClickSound.CreateInstance().Play();

                    //loops through if star is achieved row
                    for (int i = 0; i < isStarAchieve.GetLength(0); i++)
                    {
                        //loops through if star is achieved column
                        for (int x = 0; x < isStarAchieve.GetLength(1); x++)
                        {
                            //sets all stars to false
                            isStarAchieve[i, x] = false;
                        }
                    }
                    //sets first game to unlocked
                    isGameUnlock[0] = true;

                    //loops through all games
                    for (int i = 1; i < isGameUnlock.Length; i++)
                    {
                        //sets all games to locked
                        isGameUnlock[i] = false;
                    }

                    //sets key used to false
                    isKeyUsed = false;

                    //resets data counter
                    dataCounter = 0;

                    //runs subprpgram to save game to text file
                    GameSave();

                    //sets next gamestate to menu
                    nextGameState = MENU;

                    //sets game fading to true
                    isFadingIn = true;
                }
            }
        }

        //Pre: None
        //Post: None
        //Desc: draws pause screen
        private void DrawPause()
        {
            //draw darkenned gameplay
            DrawGameplay();
            spriteBatch.Draw(whiteSquareImg, blackScreenRec, Color.Black * 0.8f);

            //draw game info
            spriteBatch.DrawString(pauseFont, pauseToGamePromt, pauseToGameLoc, Color.White);
            spriteBatch.DrawString(pauseFont, pauseToMenuPromt, pauseToMenuLoc, Color.White);
            spriteBatch.DrawString(gameInfoFont, exitGameWarn, exitWarnLoc, Color.White);
        }

        //Pre: None
        //Post: None
        //Desc: draws user load screen
        private void DrawUserLoad()
        {
            //draw user prompts
            spriteBatch.DrawString(pauseFont, userLoadPromt, userLoadPromtLoc, Color.White);
            spriteBatch.DrawString(promtDescFont, userLoadWarning, userLoadWarningLoc, Color.White);

            //loop throuhg user choice rectanlges
            for (int i = 0; i < userChoiceBtnsRec.GetLength(0); i++)
            {
                //draw userchoice buttons and drop shadows
                spriteBatch.Draw(whiteSquareImg, userChoiceBtnsRec[OFF, i], Color.Black);
                spriteBatch.Draw(userChoiceBtns[Convert.ToInt32(hoveruserLoadBtn[i]), i], userChoiceBtnsRec[Convert.ToInt32(hoveruserLoadBtn[i]), i], Color.White);
            }
        }

        //Pre: None
        //Post: None
        //Desc: relocates ball
        private void BallReloc()
        {
            //check if ball is not centres
            if (ballLoc.X >= screenWidth * 0.5 + CENT_DIST_TOL || ballLoc.X <= screenWidth * 0.5 - CENT_DIST_TOL)
            {
                //move ball towards centre
                ballLoc.X += -Math.Sign(ballLoc.X - screenWidth * 0.5) * RELOCATE_SPEED[REL_FAST];
                ballHorLoc = ballLoc.X;
            }
            else
            {
                //set is ball moved variable to true
                isBallMoved = true;
            }

            //set ball loc
            ballLoc.Y = (int)(rightBarLoc.Y - Math.Tan(barAngle) * (screenWidth - ballLoc.X) - ballImg[gameType].Height * BALL_SCALE + 5);
        }

        //Pre: interger pacman colour from 0 to 2
        //Post: None
        //Desc: moves pacman left or right
        private void PacXMovement(int pacCol)
        {
            //check if ball location is less than pacman location
            if (ballLoc.X < pacAnim[pacState[pacCol], pacCol].destRec.Center.X)
            {
                //move pacman location
                pacLoc[pacCol].X -= pacSpeed[pacCol];

                //set pacman state to left
                pacState[pacCol] = LEFT;
            }
            else if (ballLoc.X > pacAnim[pacState[pacCol], pacCol].destRec.Center.X)
            {
                //move pacman locatin
                pacLoc[pacCol].X += pacSpeed[pacCol];

                //set pacman state to right
                pacState[pacCol] = RIGHT;
            }

            //loop through pacman animation row
            for (int i = 0; i < pacAnim.GetLength(0); i++)
            {
                //set pacman animation location to relocation location
                pacAnim[i, pacCol].destRec.X = (int)pacLoc[pacCol].X;
            }
        }

        //Pre: interger pacman colour from 0 to 2
        //Post: None
        //Desc: moves pacman up or down
        private void PacYMovement(int pacCol)
        {
            //check if ball loc is less than pacman animation
            if (ballLoc.Y < pacAnim[pacState[pacCol], pacCol].destRec.Center.Y)
            {
                //move pacman loation 
                pacLoc[pacCol].Y -= pacSpeed[pacCol];

                //set pacman state to up
                pacState[pacCol] = UP;
            }
            else if (ballLoc.Y > pacAnim[pacState[pacCol], pacCol].destRec.Center.Y)
            {
                //move pacman location
                pacLoc[pacCol].Y += pacSpeed[pacCol];

                //set pacman state to down
                pacState[pacCol] = DOWN;
            }

            //loop through pacman animation row
            for (int i = 0; i < pacAnim.GetLength(0); i++)
            {
                //set pacman animation location to pacman location
                pacAnim[i, pacCol].destRec.Y = (int)pacLoc[pacCol].Y;
            }
        }
    }
}
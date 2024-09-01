namespace ArioSoren.VirtualCarOps
{
    public enum GameController
    {
        //FirstIndex 0 corresponds to the first game controller connected. SecondIndex 1 to the second game controller;
        FirstIndex = 0,
        SecondIndex = 1,

    }

    public enum SteeringWheelKeyCode
    {
        A = 0,
        B = 1,
        X = 2,
        Y = 3,

        //May be used fr gears without shifter or for indicators
        RIGHTBUMPER = 4,
        LEFTBUMPER = 5,

        OPTION = 6,
        SHARE = 7,
        R2 = 8,
        L2 = 9,
        R3 = 10,
        L3 = 11,

        Shifter1 = 12,
        Shifter2 = 13,
        Shifter3 = 14,
        Shifter4 = 15,
        Shifter5 = 16,
        Shifter6 = 17,
        Shifter7 = 18,

        //May be used for volume
        LogiPlus = 19,
        LogiMinus = 20,

        RedClockWise = 21,
        RedAntiClockWise = 22,
        ArrowButton = 23,

        XBoxButton = 24,

        Steering = 25,
        Gas = 26,
        Brake = 27,
        Clutch = 28,
    
        //Directional pad
        UPButton = 0,
        DOWNButton = 18000,
        LEFTButton = 27000,
        RIGHTButton = 9000,
        UP_LEFTButton = 31500,
        UP_RIGHTButton = 4500,
        DOWN_LEFTButton = 22500,
        DOWN_RIGHTButton = 13500,
        CENTER = 29
    }
}
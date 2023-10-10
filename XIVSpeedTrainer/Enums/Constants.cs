using System;

namespace XIVSpeedTrainer.Enums;

public class Constants
{
    public static readonly IntPtr OffsetPlayerSpeed = 0x214A218;

    public static float GetSelectedOption(int index)
    {
        switch (index)
        {
            case 0:
                return 1.0f;
            case 1:
                return 1.05f;
            case 2:
                return 1.15f;
            case 3:
                return 3.0f;
            case 4:
                return 9.99f;
        }

        return 1.0f;
    }
}

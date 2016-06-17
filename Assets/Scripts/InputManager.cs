using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{

    public enum CONTROLLER_BUTTON
    {
        BUTTON_L1, BUTTON_R1, TRIGGER_L2, TRIGGER_R2,
        A, B, X, Y,
        START, SELECT, HOME
    }

    public enum CONTROLLER_ANALOG
    {
        DPAD_X, DPAD_Y,
        STICK_LEFT_X, STICK_LEFT_Y,
        STICK_RIGHT_X, STICK_RIGHT_Y,
        TRIGGER_L2, TRIGGER_R2
    }

    private enum CONTROLLER_BUTTON_EVENT
    {
        DOWN, PRESSED, UP
    }

    public static float GetAxis(CONTROLLER_ANALOG axis)
    {
        float state = 0.0f;
        switch (axis)
        {
            case CONTROLLER_ANALOG.STICK_LEFT_X:
                state = Input.GetAxis("CONTROLLER_ANALOG_STICK_LEFT_X");
                break;
            case CONTROLLER_ANALOG.STICK_LEFT_Y:
                state = Input.GetAxis("CONTROLLER_ANALOG_STICK_LEFT_Y");
                break;
            case CONTROLLER_ANALOG.STICK_RIGHT_X:
#if UNITY_ANDROID
			state = Input.GetAxis ("CONTROLLER_ANALOG_STICK_RIGHT_X");
#endif
#if UNITY_EDITOR
                state = -1f * Input.GetAxis("CONTROLLER_ANALOG_STICK_RIGHT_Y");
#endif
                break;
            case CONTROLLER_ANALOG.STICK_RIGHT_Y:
#if UNITY_ANDROID
			state = Input.GetAxis ("CONTROLLER_ANALOG_STICK_RIGHT_Y");
#endif
#if UNITY_EDITOR
                state = -1f * Input.GetAxis("CONTROLLER_ANALOG_DPAD_X");
#endif
                break;
            case CONTROLLER_ANALOG.TRIGGER_L2:
                state = Input.GetAxis("CONTROLLER_ANALOG_TRIGGER_L2");
                break;
            case CONTROLLER_ANALOG.TRIGGER_R2:
#if UNITY_ANDROID
			state = Input.GetAxis ("CONTROLLER_ANALOG_TRIGGER_R2");
#endif
#if UNITY_EDITOR
                state = Input.GetAxis("CONTROLLER_ANALOG_STICK_RIGHT_X");
#endif
                break;
            case CONTROLLER_ANALOG.DPAD_X:
                state = Input.GetAxis("CONTROLLER_ANALOG_DPAD_X");
                break;
            case CONTROLLER_ANALOG.DPAD_Y:
                state = Input.GetAxis("CONTROLLER_ANALOG_DPAD_Y");
                break;
        }

        return state;
    }

    public static bool IsPressed(CONTROLLER_BUTTON button)
    {
        return ButtonHandler(button, CONTROLLER_BUTTON_EVENT.PRESSED);
    }

    public static bool WasPressed(CONTROLLER_BUTTON button)
    {
        return ButtonHandler(button, CONTROLLER_BUTTON_EVENT.DOWN);
    }

    public static bool WasReleased(CONTROLLER_BUTTON button)
    {
        return ButtonHandler(button, CONTROLLER_BUTTON_EVENT.UP);
    }


    private static bool ButtonHandler(CONTROLLER_BUTTON button, CONTROLLER_BUTTON_EVENT buttonEvent)
    {
        bool result = false;
        string input = "";

        switch (button)
        {
            case CONTROLLER_BUTTON.A:
                break;
            case CONTROLLER_BUTTON.B:
                break;
            case CONTROLLER_BUTTON.X:
                break;
            case CONTROLLER_BUTTON.Y:
                break;
            case CONTROLLER_BUTTON.BUTTON_L1:
                break;
            case CONTROLLER_BUTTON.BUTTON_R1:
                break;
            case CONTROLLER_BUTTON.TRIGGER_L2:
                break;
            case CONTROLLER_BUTTON.TRIGGER_R2:
                break;
            case CONTROLLER_BUTTON.START:
#if UNITY_ANDROID
			input = "CONTROLLER_BUTTON_START";
#endif
#if UNITY_EDITOR
                input = "CONTROLLER_BUTTON_START";
#endif
                break;
            case CONTROLLER_BUTTON.SELECT:
#if UNITY_ANDROID
			input = "CONTROLLER_BUTTON_SELECT";
#endif
#if UNITY_EDITOR
                input = "CONTROLLER_BUTTON_SELECT";
#endif

                break;
            case CONTROLLER_BUTTON.HOME:
                break;
        }

        switch (buttonEvent)
        {
            case CONTROLLER_BUTTON_EVENT.DOWN:
                result = Input.GetButtonDown(input);
                break;
            case CONTROLLER_BUTTON_EVENT.PRESSED:
                result = Input.GetButton(input);
                break;
            case CONTROLLER_BUTTON_EVENT.UP:
                result = Input.GetButtonUp(input);
                break;
        }
        if (result)
        {
            Debug.Log("HI!");
        }
        return result;
    }
}

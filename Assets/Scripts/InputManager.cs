using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    public enum CONTROLLER_ACTION
    {
        THRUSTER, STRAFE, SHOOT_PRIMARY, SHOOT_SECONDARY,
        LOOK_UP, LOOK_SIDE, ROTATE, BOOST,
        PAUSE, SELECT
    }

    private enum CONTROLLER_ACTION_EVENT
    {
        DOWN, PRESSED, UP
    }

    public static float GetAxis(CONTROLLER_ACTION action)
    {
        float state = 0.0f;
        switch (action)
        {
            case CONTROLLER_ACTION.LOOK_SIDE:
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                state = Input.GetAxis("CONTROLLER_AXIS_4");
#elif UNITY_ANDROID
                state = Input.GetAxis("CONTROLLER_AXIS_3");
#endif
                break;
            case CONTROLLER_ACTION.LOOK_UP:
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                state = Input.GetAxis("CONTROLLER_AXIS_5");
#elif UNITY_ANDROID
                state = Input.GetAxis("CONTROLLER_AXIS_4");
#endif
                break;
            case CONTROLLER_ACTION.ROTATE:
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                state = Input.GetAxis("CONTROLLER_AXIS_3");
                if (state == 0)
                {
                    state = (Input.GetKey("q") ? 1 : 0) - (Input.GetKey("e") ? 1 : 0);
                }
#elif UNITY_ANDROID
                state = Input.GetAxis("CONTROLLER_AXIS_12") - Input.GetAxis("CONTROLLER_AXIS_13");
#endif
                break;
            case CONTROLLER_ACTION.STRAFE:
                state = Input.GetAxis("CONTROLLER_AXIS_1");
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                if (state == 0)
                {
                    state = (Input.GetKey("d") ? 1 : 0) - (Input.GetKey("a") ? 1 : 0);
                }
#endif
                break;
            case CONTROLLER_ACTION.THRUSTER:
                state = Input.GetAxis("CONTROLLER_AXIS_2") * -1;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                if (state == 0)
                {
                    state = (Input.GetKey("w") ? 1 : 0) - (Input.GetKey("s") ? 1 : 0);
                }
#endif
                break;
            case CONTROLLER_ACTION.BOOST:
                state = Input.GetButtonDown("CONTROLLER_BUTTON_0") ? 1f : 0f ;
                break;
            case CONTROLLER_ACTION.SELECT:
                state = Input.GetButtonDown("CONTROLLER_BUTTON_0") ? 1f : 0f;
                break;
            case CONTROLLER_ACTION.PAUSE:
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                state = Input.GetButtonDown("CONTROLLER_BUTTON_7") ? 1f : 0f;
#elif UNITY_ANDROID
                state = Input.GetButtonDown("CONTROLLER_BUTTON_10") ? 1f : 0f;
#endif
                break;
            case CONTROLLER_ACTION.SHOOT_PRIMARY:
                state = Input.GetButtonDown("CONTROLLER_BUTTON_5") ? 1f : 0f;
                break;
            case CONTROLLER_ACTION.SHOOT_SECONDARY:
                state = Input.GetButtonDown("CONTROLLER_BUTTON_4") ? 1f : 0f;
                break;
        }

        return state;
    }

    private static bool ActionHandler(CONTROLLER_ACTION button, CONTROLLER_ACTION_EVENT actionEvent)
    {
        bool result = false;
        string input = "";

        switch (button)
        {
            case CONTROLLER_ACTION.BOOST:
                input = "CONTROLLER_BUTTON_0";
                break;
            case CONTROLLER_ACTION.LOOK_SIDE:
                break;
            case CONTROLLER_ACTION.LOOK_UP:
                break;
            case CONTROLLER_ACTION.PAUSE:
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                input = "CONTROLLER_BUTTON_7";
#else
                input = "CONTROLLER_BUTTON_10";
#endif

                break;
            case CONTROLLER_ACTION.ROTATE:
                break;
            case CONTROLLER_ACTION.SELECT:
                input = "CONTROLLER_BUTTON_0";
                break;
            case CONTROLLER_ACTION.SHOOT_PRIMARY:
                input = "CONTROLLER_BUTTON_5";
                break;
            case CONTROLLER_ACTION.SHOOT_SECONDARY:
                input = "CONTROLLER_BUTTON_4";
                break;
            case CONTROLLER_ACTION.STRAFE:
                break;
            case CONTROLLER_ACTION.THRUSTER:
                break;
        }

        switch (actionEvent)
        {
            case CONTROLLER_ACTION_EVENT.DOWN:
                result = Input.GetButtonDown(input);
                break;
            case CONTROLLER_ACTION_EVENT.PRESSED:
                result = Input.GetButton(input);
                break;
            case CONTROLLER_ACTION_EVENT.UP:
                result = Input.GetButtonUp(input);
                break;
        }
        return result;
    }

    public static bool WasActionPressed(CONTROLLER_ACTION action)
    {
        return ActionHandler(action, CONTROLLER_ACTION_EVENT.DOWN);
    }

    public static bool IsActionPressed(CONTROLLER_ACTION action)
    {
        return ActionHandler(action, CONTROLLER_ACTION_EVENT.PRESSED);
    }

    public static bool WasActionReleased(CONTROLLER_ACTION action)
    {
        return ActionHandler(action, CONTROLLER_ACTION_EVENT.UP);
    }
}

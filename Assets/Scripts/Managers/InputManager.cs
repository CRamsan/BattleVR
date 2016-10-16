using UnityEngine;

/// <summary>
/// This script provides an abstraction layer from the in-game actions(turn, move forward, shoot, etc) into
/// platform-specific inputs to be read by the native Unity APIs.
/// </summary>
public class InputManager : MonoBehaviour
{
    public enum CONTROLLER_ACTION
    {
        THRUSTER, STRAFE, SHOOT_PRIMARY, SHOOT_SECONDARY,
        LOOK_UP, LOOK_SIDE, ROTATE, BOOST,
        PAUSE, SELECT,
        SCALE_UP, SCALE_DOWN
    }

    private enum CONTROLLER_ACTION_EVENT
    {
        DOWN, PRESSED, UP
    }

    /// <summary>
    /// Returns the analog value of the requested action. If the action belongs to a
    /// button then the return value will be -1, 0 or 1.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static float GetAxis(CONTROLLER_ACTION action)
    {
        float state = 0.0f;
        switch (action)
        {
            case CONTROLLER_ACTION.LOOK_SIDE:
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                state = Input.GetAxis("CONTROLLER_AXIS_4");
                if (state == 0)
                {
                    state = (Input.GetKey("right") ? 1 : 0) - (Input.GetKey("left") ? 1 : 0);
                }
#elif UNITY_ANDROID
                state = Input.GetAxis("CONTROLLER_AXIS_3");
#endif
                break;
            case CONTROLLER_ACTION.LOOK_UP:
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                state = Input.GetAxis("CONTROLLER_AXIS_5");
                if (state == 0)
                {
                    state = (Input.GetKey("up") ? 1 : 0) - (Input.GetKey("down") ? 1 : 0);
                }
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
                state = Input.GetAxis("CONTROLLER_AXIS_13") - Input.GetAxis("CONTROLLER_AXIS_12");
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
                state = Input.GetButtonDown("CONTROLLER_BUTTON_0") ? 1f : 0f;
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
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                if (state == 0)
                {
                    state = Input.GetKey(KeyCode.LeftControl) ? 1 : 0;
                }
#endif
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
                // TODO Remove this hack, it is here to enable Pause in a keyboard
                if (Input.GetKeyUp("p"))
                {
                    return true;
                }
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
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                input = "CONTROLLER_BUTTON_5";
                // TODO Remove this hack, it is here to enable Pause in a keyboard
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    return true;
                }
#else
                input = "CONTROLLER_BUTTON_5";
#endif
                break;
            case CONTROLLER_ACTION.SHOOT_SECONDARY:
                input = "CONTROLLER_BUTTON_4";
                break;
            case CONTROLLER_ACTION.STRAFE:
                break;
            case CONTROLLER_ACTION.THRUSTER:
                break;
            case CONTROLLER_ACTION.SCALE_UP:
                input = "CONTROLLER_BUTTON_2";
                break;
            case CONTROLLER_ACTION.SCALE_DOWN:
                input = "CONTROLLER_BUTTON_3";
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

    /// <summary>
    /// Detect if the action was trigered during the last frame
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static bool WasActionPressed(CONTROLLER_ACTION action)
    {
        return ActionHandler(action, CONTROLLER_ACTION_EVENT.DOWN);
    }

    /// <summary>
    /// Detect if the action is currently being pressed
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static bool IsActionPressed(CONTROLLER_ACTION action)
    {
        return ActionHandler(action, CONTROLLER_ACTION_EVENT.PRESSED);
    }

    /// <summary>
    /// Detect if the action was released during the last frame
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static bool WasActionReleased(CONTROLLER_ACTION action)
    {
        return ActionHandler(action, CONTROLLER_ACTION_EVENT.UP);
    }
}

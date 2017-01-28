using UnityEngine;
using System.Collections.Generic;

public class GamepadInput {
	public static int MAX_GAMEPAD_COUNT { get { return 6; } }

	private static GamepadInput[] singletonGamepads = new GamepadInput[MAX_GAMEPAD_COUNT];

	public int Index { private set; get; }

	public float DPadDeadzone = 0;
	public float TriggerDeadzone = .5f;
	public float LeftStickDeadzone = .2f;
	public float RightStickDeadzone = .2f;
	public bool InUse { get; private set; }

	// Axes
	public string LeftHorizontal_Axis { private set; get; }
	public float Left_X { get { return Input.GetAxis (LeftHorizontal_Axis); } }

	public string LeftVertical_Axis { private set; get; }
	public float Left_Y { get { return Input.GetAxis (LeftVertical_Axis); } }

	public string RightHorizontal_Axis { private set; get; }
	public float Right_X { get { return Input.GetAxis (RightHorizontal_Axis); } }

	public string RightVertical_Axis { private set; get; }
	public float Right_Y { get { return Input.GetAxis (RightVertical_Axis); } }

	public string DPadHorizontal_Axis { private set; get; }
	public float DPad_X { get { return Input.GetAxis (DPadHorizontal_Axis); } }

	public string DPadVertical_Axis { private set; get; }
	public float DPad_Y { get { return Input.GetAxis (DPadVertical_Axis); } }

	public string RightTrigger_Axis { private set; get; }
	public float RightTrigger_Analog { get { return Input.GetAxis (RightTrigger_Axis); } }

	public string LeftTrigger_Axis { private set; get; }
	public float LeftTrigger_Analog { get { return Input.GetAxis (LeftTrigger_Axis); } }

	// Buttons
	public string A_Button { private set; get; }
	public bool A { get { return Input.GetButton (A_Button); } }
   public bool ADown { get { return Input.GetButtonDown (A_Button); } }
   public bool AUp { get { return Input.GetButtonUp (A_Button); } }

	public string B_Button { private set; get; }
	public bool B { get { return Input.GetButton (B_Button); } }
   public bool BDown { get { return Input.GetButtonDown (B_Button); } }
   public bool BUp { get { return Input.GetButtonUp (B_Button); } }

	public string X_Button { private set; get; }
	public bool X { get { return Input.GetButton (X_Button); } }
	public bool XDown { get { return Input.GetButtonDown (X_Button); } }
	public bool XUp { get { return Input.GetButtonUp (X_Button); } }

	public string Y_Button { private set; get; }
	public bool Y { get { return Input.GetButton (Y_Button); } }
	public bool YDown { get { return Input.GetButtonDown (Y_Button); } }
	public bool YUp { get { return Input.GetButtonUp (Y_Button); } }

	public string RightBumper_Button { private set; get; }
	public bool RB { get { return Input.GetButton (RightBumper_Button); } }
	public bool RBDown { get { return Input.GetButtonDown (RightBumper_Button); } }
	public bool RBUp { get { return Input.GetButtonUp (RightBumper_Button); } }

	public string LeftBumper_Button { private set; get; }
	public bool LB { get { return Input.GetButton (LeftBumper_Button); } }
	public bool LBDown { get { return Input.GetButtonDown (LeftBumper_Button); } }
	public bool LBUp { get { return Input.GetButtonUp (LeftBumper_Button); } }

	public string Back_Button { private set; get; }
	public bool Back { get { return Input.GetButton (Back_Button); } }
	public bool BackDown { get { return Input.GetButtonDown (Back_Button); } }
	public bool BackUp { get { return Input.GetButtonUp (Back_Button); } }

	public string Start_Button { private set; get; }
	public bool Start { get { return Input.GetButton (Start_Button); } }
	public bool StartDown { get { return Input.GetButtonDown (Start_Button); } }
	public bool StartUp { get { return Input.GetButtonUp (Start_Button); } }

	public string LeftStick_Button { private set; get; }
	public bool LS { get { return Input.GetButton (LeftStick_Button); } }
	public bool LSDown { get { return Input.GetButtonDown (LeftStick_Button); } }
	public bool LSUp { get { return Input.GetButtonUp (LeftStick_Button); } }

	public string RightStick_Button { private set; get; }
	public bool RS { get { return Input.GetButton (RightStick_Button); } }
	public bool RSDown { get { return Input.GetButtonDown (RightStick_Button); } }
	public bool RSUp { get { return Input.GetButtonUp (RightStick_Button); } }

	public bool DPadLeft { get { return DPad_X < -DPadDeadzone; } }
	public bool DPadRight { get { return DPad_X > DPadDeadzone; } }
	public bool DPadUp { get { return DPad_Y < -DPadDeadzone; } }
	public bool DPadDown { get { return DPad_Y > DPadDeadzone; } }

	public bool LeftStickLeft { get { return Left_X < -LeftStickDeadzone; } }
	public bool LeftStickRight { get { return Left_X > LeftStickDeadzone; } }
	public bool LeftStickUp { get { return Left_Y < -LeftStickDeadzone; } }
	public bool LeftStickDown { get { return Left_Y > LeftStickDeadzone; } }

	public bool RightStickLeft { get { return Right_X < -RightStickDeadzone; } }
	public bool RightStickRight { get { return Right_X > RightStickDeadzone; } }
	public bool RightStickUp { get { return Right_Y < -RightStickDeadzone; } }
	public bool RightStickDown { get { return Right_Y > RightStickDeadzone; } }

	public bool LeftTrigger { get { return Input.GetAxis (LeftTrigger_Axis) >= TriggerDeadzone; } }
	public bool RightTrigger { get { return Input.GetAxis (RightTrigger_Axis) >= TriggerDeadzone; } }

	private GamepadInput(int index) {
		this.Index = index;

		A_Button = "A_" + index;
		B_Button = "B_" + index;
		X_Button = "X_" + index;
		Y_Button = "Y_" + index;
		RightBumper_Button = "RB_" + index;
		LeftBumper_Button = "LB_" + index;
		Back_Button = "Back_" + index;
		Start_Button = "Start_" + index;
		LeftStick_Button = "LS_" + index;
		RightStick_Button = "RS_" + index;

		LeftHorizontal_Axis = "L_XAxis_" + index;
		LeftVertical_Axis = "L_YAxis_" + index;
		RightHorizontal_Axis = "R_XAxis_" + index;
		RightVertical_Axis = "R_YAxis_" + index;
		DPadHorizontal_Axis = "DPad_XAxis_" + index;
		DPadVertical_Axis = "DPad_YAxis_" + index;
		RightTrigger_Axis = "TriggersR_" + index;
		LeftTrigger_Axis = "TriggersL_" + index;
	}

	public bool IsValid { get { return Input.GetJoystickNames ().Length > Index; } }

	public void Lock() {
		InUse = true;
	}

	public void Free() {
		InUse = false;
	}

	public static GamepadInput Get(int index) {
		if (index < 0 || index >= MAX_GAMEPAD_COUNT)
			return null;

		if (singletonGamepads [index] == null)
			singletonGamepads [index] = new GamepadInput (index + 1);
		return singletonGamepads [index];
	}

	public static bool TryGet(int index, out GamepadInput gamepad) {
		gamepad = Get (index);
		return gamepad != null && gamepad.IsValid;
	} 

	public static List<GamepadInput> AllGamepads { 
		get {
			List<GamepadInput> gamepads = new List<GamepadInput>();
			for (int i = 0; i < MAX_GAMEPAD_COUNT; i++)
				gamepads.Add(Get(i));
			return gamepads;
		}
	}
}

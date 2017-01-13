using UnityEngine;
using System.Collections.Generic;

public class GamepadInput {
	public static int MAX_GAMEPAD_COUNT { get { return 11; } }

	private static GamepadInput[] singletonGamepads = new GamepadInput[MAX_GAMEPAD_COUNT];

	private int index;
	public bool InUse { get; private set; }

	// Buttons
	public string A { private set; get; }
	public string B { private set; get; }
	public string X { private set; get; }
	public string Y { private set; get; }
	public string RightBumper { private set; get; }
	public string LeftBumper { private set; get; }
	public string Back { private set; get; }
	public string Start { private set; get; }
	public string LeftStickClick { private set; get; }
	public string RightStickClick { private set; get; }

	// Axes
	public string LeftHorizontal { private set; get; }
	public string LeftVertical { private set; get; }
	public string RightHorizontal { private set; get; }
	public string RightVertical { private set; get; }
	public string DPadHorizontal { private set; get; }
	public string DPadVertical { private set; get; }
	public string RightTriggerAnalog { private set; get; }
	public string LeftTriggerAnalog { private set; get; }

	private GamepadInput(int index) {
		this.index = index;

		A = "A_" + index;
		B = "B_" + index;
		X = "X_" + index;
		Y = "Y_" + index;
		RightBumper = "RB_" + index;
		LeftBumper = "LB_" + index;
		Back = "Back_" + index;
		Start = "Start_" + index;
		LeftStickClick = "LS_" + index;
		RightStickClick = "RS_" + index;

		LeftHorizontal = "L_XAxis_" + index;
		LeftVertical = "L_YAxis_" + index;
		RightHorizontal = "R_XAxis_" + index;
		RightVertical = "R_YAxis_" + index;
		DPadHorizontal = "DPad_XAxis_" + index;
		DPadVertical = "DPad_YAxis_" + index;
		RightTriggerAnalog = "Triggers_R_" + index;
		LeftTriggerAnalog = "Triggers_L_" + index;
	}

	public bool IsValid { get { return Input.GetJoystickNames ().Length > index; } }

	public void Lock() {
		InUse = true;
	}

	public void Free() {
		InUse = false;
	}

	public static GamepadInput Get(int index) {
		if (index < 0 || index >= singletonGamepads.Length)
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
			List<GamepadInput> gamepads = new List<GamepadInput>(Input.GetJoystickNames().Length);
			for (int i = 0; i < gamepads.Capacity; i++)
				gamepads.Add(Get (i));
			return gamepads;
		}
	}

	public static GamepadInput FirstAvailable {
		get {
			foreach (GamepadInput gamepad in AllGamepads)
				if (!gamepad.InUse) {
					gamepad.InUse = true;
					return gamepad;
				}
			return null;
		}
	}

	public static bool TryGetFirstAvailable(out GamepadInput gamepad) {
		gamepad = FirstAvailable;
		return gamepad != null && gamepad.IsValid;
	}
}

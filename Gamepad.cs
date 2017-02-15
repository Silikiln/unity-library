using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Gamepad {
	// Static Fields

	public enum InputCode {
		None = -1,

		LeftStickDown = 0,
		LeftStickUp = 1,
		LeftStickRight = 2,
		LeftStickLeft = 3,

		RightStickDown = 4,
		RightStickUp = 5,
		RightStickRight = 6,
		RightStickLeft = 7,

		DpadUp = 8,
		DpadDown = 9,
		DpadRight = 10,
		DpadLeft = 11,

		RightStickClick = 12,
		LeftStickClick = 14,

		Start = 16,
		Back = 18,

		A = 20,
		B = 22,
		X = 24,
		Y = 26,

		LeftBumper = 28,
		RightBumper = 30,

		LeftTrigger = 32,
		RightTrigger = 34
	}

	public static readonly int MAX_GAMEPAD_COUNT = 6;

	private static Gamepad[] singletonGamepads = new Gamepad[MAX_GAMEPAD_COUNT];

	// Static Helper Methods

	public static Gamepad Get(int index) {
		if (index < 0 || index >= MAX_GAMEPAD_COUNT)
			return null;

		if (singletonGamepads [index] == null)
			singletonGamepads [index] = new Gamepad (index + 1);
		return singletonGamepads [index];
	}

	public static bool TryGet(int index, out Gamepad gamepad) {
		gamepad = Get (index);
		return gamepad != null && gamepad.IsValid;
	} 

	public static List<Gamepad> AllGamepads { 
		get {
			List<Gamepad> gamepads = new List<Gamepad>();
			for (int i = 0; i < Input.GetJoystickNames().Length; i++)
				gamepads.Add(Get(i));
			return gamepads;
		}
	}

	// Public Fields

	public int Index { private set; get; }

	public float DPadDeadzone = 0;
	public float TriggerDeadzone = .4f;
	public float LeftStickDeadzone = .1f;
	public float RightStickDeadzone = .2f;
	public bool InUse { get; private set; }

	float lastUpdateTime = -1;
	private Dictionary<InputCode, bool> stagedInputValue = new Dictionary<InputCode, bool>();
	private Dictionary<InputCode, bool> previousInputValue = new Dictionary<InputCode, bool>();

	// Axes
	public string LeftHorizontal_Axis { private set; get; }

	public string LeftVertical_Axis { private set; get; }

	public string RightHorizontal_Axis { private set; get; }

	public string RightVertical_Axis { private set; get; }

	public string DPadHorizontal_Axis { private set; get; }

	public string DPadVertical_Axis { private set; get; }

	public string RightTrigger_Axis { private set; get; }

	public string LeftTrigger_Axis { private set; get; }

	// Buttons
	public string A_Button { private set; get; }

	public string B_Button { private set; get; }

	public string X_Button { private set; get; }

	public string Y_Button { private set; get; }

	public string RightBumper_Button { private set; get; }

	public string LeftBumper_Button { private set; get; }

	public string Back_Button { private set; get; }

	public string Start_Button { private set; get; }

	public string LeftStick_Button { private set; get; }

	public string RightStick_Button { private set; get; }

	private Gamepad(int index) {
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

	public void Update() {
		if (lastUpdateTime == Time.time)
			return;
		lastUpdateTime = Time.time;
		previousInputValue = stagedInputValue;
		stagedInputValue = new Dictionary<InputCode, bool> ();
		foreach (InputCode code in System.Enum.GetValues(typeof(InputCode)).Cast<InputCode>()) {
			if (code == InputCode.None)	continue;
			stagedInputValue [code] = GetInputActive (code);
		}
	}

	// Helper Methods
	public string GetInputString(InputCode code) {
		switch (code) {
			case InputCode.RightStickUp:
			case InputCode.RightStickDown:
				return RightVertical_Axis;
			case InputCode.RightStickLeft:
			case InputCode.RightStickRight:
				return RightHorizontal_Axis;

			case InputCode.LeftStickUp:
			case InputCode.LeftStickDown:
				return LeftVertical_Axis;
			case InputCode.LeftStickLeft:
			case InputCode.LeftStickRight:
				return LeftHorizontal_Axis;

			case InputCode.DpadUp:
			case InputCode.DpadDown:
				return DPadVertical_Axis;
			case InputCode.DpadLeft:
			case InputCode.DpadRight:
				return DPadHorizontal_Axis;

			case InputCode.RightStickClick:
				return RightStick_Button;
			case InputCode.LeftStickClick:
				return LeftStick_Button;

			case InputCode.Start:
				return Start_Button;
			case InputCode.Back:
				return Back_Button;

			case InputCode.A:
				return A_Button;
			case InputCode.B:
				return B_Button;
			case InputCode.X:
				return X_Button;
			case InputCode.Y:
				return Y_Button;

			case InputCode.RightBumper:
				return RightBumper_Button;
			case InputCode.LeftBumper:
				return LeftBumper_Button;

			case InputCode.RightTrigger:
				return RightTrigger_Axis;
			case InputCode.LeftTrigger:
				return LeftTrigger_Axis;
		}
		return null;
	}

	public bool IsPositive(InputCode code) {
		return ((int)code & 1) == 0;
	}

	public float GetInputValue(InputCode code) { return GetInputValue (GetInputString (code), IsPositive (code)); }
	public float GetInputValue(InputCode code, float deadzone) { return GetInputValue (GetInputString (code), deadzone, IsPositive (code));	}
	public float GetInputValue(string inputName, bool positive) { return GetInputValue (inputName, GetDeadzone (inputName), positive); }
	public float GetInputValue(string inputName, float deadzone, bool positive) {
		if (inputName == null) return 0;

		float value;
		if (inputName.Contains ("Axis") || inputName.Contains ("Trigger"))
			value = Input.GetAxis (inputName);
		else
			value = Input.GetButton (inputName) ? 1 : 0;
		return (positive ? value >= deadzone : value <= -deadzone) ? value : 0;
	}

	public bool GetInputActive(params InputCode[] codes) {
		if (codes.Length == 0)
			return false;
		foreach (InputCode code in codes)
			if (GetInputActive (code))
				return true;
		return false;
	}
	public bool GetInputActive(InputCode code) { return GetInputValue (code) != 0; }

	public float GetDeadzone(InputCode code) { return GetDeadzone (GetInputString (code)); }
	public float GetDeadzone(string inputName) {
		if (inputName == null)
			return 0;
		if (inputName.Contains ("Trigger"))
			return TriggerDeadzone;
		if (inputName.Contains ("L_"))
			return LeftStickDeadzone;
		if (inputName.Contains ("R_"))
			return RightStickDeadzone;
		if (inputName.Contains ("DPad"))
			return DPadDeadzone;
		return 0;
	}

	public bool InputStarted(params InputCode[] codes) {
		if (codes.Length == 0)
			return false;
		foreach (InputCode code in codes)
			if (InputStarted (code))
				return true;
		return false;
	}
	public bool InputStarted(InputCode code) { 
		return !previousInputValue.GetValueOrDefault (code, false) && stagedInputValue.GetValueOrDefault (code, false);
	}

	public bool InputEnded(params InputCode[] codes) {
		if (codes.Length == 0)
			return false;
		foreach (InputCode code in codes)
			if (InputEnded (code))
				return true;
		return false;
	}
	public bool InputEnded(InputCode code) {
		return previousInputValue.GetValueOrDefault (code, false) && !stagedInputValue.GetValueOrDefault (code, false);
	}

	public bool IsValid { get { return Input.GetJoystickNames ().Length > Index; } }

	public void Lock() {
		InUse = true;
	}

	public void Free() {
		InUse = false;
	}
}

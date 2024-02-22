using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppKit;

namespace SampleInputMethod {
public partial struct KBEvent {
  public EventType Type { get; }
  public EventModifierFlags ModifierFlags { get; }
  public double Timestamp { get; }
  public int WindowNumber { get; }
  public string? Characters { get; }
  public string? CharactersIgnoringModifiers { get; }
  public bool IsARepeat { get; }
  public ushort KeyCode { get; }
  public bool IsTypingContextVertical { get; }

  public KBEvent(NSEvent nsEvent, bool context = false) {
    IsTypingContextVertical = context;
    // [TypeCheck Started]
    // Memo: Don't access "Characters, CharactersIgnoringModifiers, IsARepeat" of an NSEvent
    // unless its type is either KeyUp or KeyDown. Otherwise, it will trigger
    // "NSInternalInconsistencyException" error.
    var typesHavingCharacters = Enum.GetValues(typeof(EventType)).Cast<uint>().OrderBy(x => x).ToHashSet();
    typesHavingCharacters.RemoveWhere(x => x == (uint)EventType.FlagsChanged);
    uint givenEventTypeRawValue = (uint)nsEvent.Type;
    // [TypeCheck Ended]
    bool hasCharacters = typesHavingCharacters.Contains(givenEventTypeRawValue);
    Type = givenEventTypeRawValue switch {
      10 => EventType.KeyDown,
      11 => EventType.KeyUp,
      12 => EventType.FlagsChanged,
      _ => EventType.KeyDown
    };
    ModifierFlags = (EventModifierFlags)nsEvent.ModifierFlags;  // Hope that it works well.
    Timestamp = nsEvent.Timestamp;
    WindowNumber = (int)nsEvent.WindowNumber;
    KeyCode = nsEvent.KeyCode;
    if (hasCharacters) {
      Characters = nsEvent.Characters ?? "";
      CharactersIgnoringModifiers = nsEvent.CharactersIgnoringModifiers ?? Characters;
      IsARepeat = nsEvent.IsARepeat;
    } else {
      Characters = null;
      CharactersIgnoringModifiers = null;
      IsARepeat = false;
    }
  }
}

// MARK: - KBEvent Constants.

public partial struct KBEvent {
  public static readonly uint[] NumPadKeyCodes =
      new uint[] { 65, 67, 69, 71, 75, 78, 81, 82, 83, 84, 85, 86, 87, 88, 89, 91, 92, 95 };

  public static readonly Dictionary<ushort, string> NonNumPadNumericalKeys =
      new() { [18] = "1", [19] = "2", [20] = "3", [21] = "4", [23] = "5",
              [22] = "6", [26] = "7", [28] = "8", [25] = "9", [29] = "0" };
}

// MARK: - KBEvent Extensions.

public partial struct KBEvent : KBEvent.IKBEventProtocol {
  public readonly NonLiteralKeyType? NonLiteralKey =>
      typeof(NonLiteralKeyType).IsEnumDefined(KeyCode) ? (NonLiteralKeyType)KeyCode : null;
  public readonly bool IsTypingVertical => CharactersIgnoringModifiers == "Vertical";
  public readonly EventModifierFlags KeyModifierFlags => ModifierFlags & ~EventModifierFlags.CapsLock;
  public readonly bool IsFlagChanged => Type == EventType.FlagsChanged;
  public readonly ushort CharCode => Encoding.ASCII.GetBytes(Characters).FirstOrDefault();
  public readonly string InputText => Characters ?? "";
  public readonly string InputTextIgnoringModifiers => CharactersIgnoringModifiers ?? Characters ?? "";
  public readonly bool IsInvalidInput =>
      (CharCode >= 0x20 && CharCode <= 0xFF) ? false : !(IsReservedKey && !IsKeyCodeBlacklisted);
  public readonly bool IsKeyCodeBlacklisted => typeof(BlackListedKeyCodes).IsEnumDefined(KeyCode);
  public readonly bool IsReservedKey => NonLiteralKey != null && NonLiteralKey != NonLiteralKeyType.None;
  public readonly bool IsUpperCaseASCIILetterKey =>
      (CharCode >= 65 && CharCode <= 90 && KeyModifierFlags == EventModifierFlags.Shift);
  public readonly bool IsShiftHold => ModifierFlags.HasFlag(EventModifierFlags.Shift);
  public readonly bool IsCommandHold => ModifierFlags.HasFlag(EventModifierFlags.Command);
  public readonly bool IsControlHold => ModifierFlags.HasFlag(EventModifierFlags.Control);
  public readonly bool IsControlHotKey => IsControlHold && char.IsLetterOrDigit(InputText.FirstOrDefault());
  public readonly bool IsOptionHold => ModifierFlags.HasFlag(EventModifierFlags.Option);
  public readonly bool IsOptionHotKey => IsOptionHold && char.IsLetterOrDigit(InputText.FirstOrDefault());
  public readonly bool IsCapsLockOn => ModifierFlags.HasFlag(EventModifierFlags.CapsLock);
  public readonly bool IsNumericPadKey => NumPadKeyCodes.Contains(KeyCode);
  public readonly bool IsMainAreaNumKey => NonNumPadNumericalKeys.Keys.Contains(KeyCode);
  public readonly bool IsNonLaptopFunctionKey =>
      KeyModifierFlags.HasFlag(EventModifierFlags.NumericPad) && !IsNumericPadKey;
  public readonly bool IsTab => NonLiteralKey == NonLiteralKeyType.Tab;
  public readonly bool IsEnter =>
      NonLiteralKey == NonLiteralKeyType.CarriageReturn || NonLiteralKey == NonLiteralKeyType.LineFeed;
  public readonly bool IsUp => NonLiteralKey == NonLiteralKeyType.UpArrow;
  public readonly bool IsDown => NonLiteralKey == NonLiteralKeyType.DownArrow;
  public readonly bool IsLeft => NonLiteralKey == NonLiteralKeyType.LeftArrow;
  public readonly bool IsRight => NonLiteralKey == NonLiteralKeyType.RightArrow;
  public readonly bool IsPageUp => NonLiteralKey == NonLiteralKeyType.PageUp;
  public readonly bool IsPageDown => NonLiteralKey == NonLiteralKeyType.PageDown;
  public readonly bool IsSpace => NonLiteralKey == NonLiteralKeyType.Space;
  public readonly bool IsBackSpace => NonLiteralKey == NonLiteralKeyType.BackSpace;
  public readonly bool isEsc => NonLiteralKey == NonLiteralKeyType.Escape;
  public readonly bool IsHome => NonLiteralKey == NonLiteralKeyType.Home;
  public readonly bool IsEnd => NonLiteralKey == NonLiteralKeyType.End;
  public readonly bool IsDelete => NonLiteralKey == NonLiteralKeyType.WindowsDelete;
  public readonly bool IsCursorBackward =>
      ((NonLiteralKey == NonLiteralKeyType.LeftArrow && !IsTypingContextVertical) ||
       (NonLiteralKey == NonLiteralKeyType.UpArrow && IsTypingContextVertical));
  public readonly bool IsCursorForward =>
      ((NonLiteralKey == NonLiteralKeyType.RightArrow && !IsTypingContextVertical) ||
       (NonLiteralKey == NonLiteralKeyType.DownArrow && IsTypingContextVertical));
  public readonly bool IsCursorClockRight =>
      ((NonLiteralKey == NonLiteralKeyType.DownArrow && !IsTypingContextVertical) ||
       (NonLiteralKey == NonLiteralKeyType.LeftArrow && IsTypingContextVertical));
  public readonly bool IsCursorClockLeft =>
      ((NonLiteralKey == NonLiteralKeyType.UpArrow && !IsTypingContextVertical) ||
       (NonLiteralKey == NonLiteralKeyType.RightArrow && IsTypingContextVertical));
  public readonly bool IsSymbolMenuPhysicalKey => NonLiteralKey == NonLiteralKeyType.SymbolMenuPhysicalKeyIntl ||
                                                  NonLiteralKey == NonLiteralKeyType.SymbolMenuPhysicalKeyJIS;
}

// MARK: - Protocol.

public partial struct KBEvent {
  public interface IKBEventProtocol {
    public NonLiteralKeyType? NonLiteralKey { get; }
    public bool IsTypingVertical { get; }
    public string InputText { get; }
    public string InputTextIgnoringModifiers { get; }
    public ushort CharCode { get; }
    public ushort KeyCode { get; }
    public bool IsInvalidInput { get; }
    public bool IsKeyCodeBlacklisted { get; }
    public bool IsReservedKey { get; }
    public bool IsUpperCaseASCIILetterKey { get; }
    public bool IsShiftHold { get; }
    public bool IsCommandHold { get; }
    public bool IsControlHold { get; }
    public bool IsControlHotKey { get; }
    public bool IsOptionHold { get; }
    public bool IsOptionHotKey { get; }
    public bool IsCapsLockOn { get; }
    public bool IsNumericPadKey { get; }
    public bool IsMainAreaNumKey { get; }
    public bool IsNonLaptopFunctionKey { get; }
    public bool IsTab { get; }
    public bool IsEnter { get; }
    public bool IsUp { get; }
    public bool IsDown { get; }
    public bool IsLeft { get; }
    public bool IsRight { get; }
    public bool IsPageUp { get; }
    public bool IsPageDown { get; }
    public bool IsSpace { get; }
    public bool IsBackSpace { get; }
    public bool isEsc { get; }
    public bool IsHome { get; }
    public bool IsEnd { get; }
    public bool IsDelete { get; }
    public bool IsCursorBackward { get; }
    public bool IsCursorForward { get; }
    public bool IsCursorClockRight { get; }
    public bool IsCursorClockLeft { get; }
    public bool IsSymbolMenuPhysicalKey { get; }
  }
}

// MARK: - KBEvent Enums.

public partial struct KBEvent {
  [Flags]
  public enum EventType : uint {
    KeyDown = 10,
    KeyUp = 11,
    FlagsChanged = 12,
  }

  [Flags]
  public enum EventModifierFlags : uint {
    CapsLock = 1 << 16,
    Shift = 1 << 17,
    Control = 1 << 18,
    Option = 1 << 19,
    Command = 1 << 20,
    NumericPad = 1 << 21,
    Help = 1 << 22,
    Function = 1 << 23,
    DeviceIndependentFlagsMask = 0xFFFF_0000,
  }

  public enum NonLiteralKeyType : ushort {
    None = 0,
    CarriageReturn = 36,  // Renamed from "kReturn" to avoid nomenclatural confusions.
    Tab = 48,
    Space = 49,
    SymbolMenuPhysicalKeyIntl = 50,  // vChewing Specific (Non-JIS)
    BackSpace = 51,                  // Renamed from "kDelete" to avoid nomenclatural confusions.
    Escape = 53,
    Command = 55,
    Shift = 56,
    CapsLock = 57,
    Option = 58,
    Control = 59,
    RightShift = 60,
    RightOption = 61,
    RightControl = 62,
    Function = 63,
    F17 = 64,
    VolumeUp = 72,
    VolumeDown = 73,
    Mute = 74,
    LineFeed = 76,  // Another keyCode to identify the Enter Key, typable by Fn+Enter.
    F18 = 79,
    F19 = 80,
    F20 = 90,
    Yen = 93,
    SymbolMenuPhysicalKeyJIS = 94,  // vChewing Specific (JIS)
    JISNumPadComma = 95,
    F5 = 96,
    F6 = 97,
    F7 = 98,
    F3 = 99,
    F8 = 100,
    F9 = 101,
    JISAlphanumericalKey = 102,
    F11 = 103,
    JISKanaSwappingKey = 104,
    F13 = 105,  // PrtSc
    F16 = 106,
    F14 = 107,
    F10 = 109,
    ContextMenu = 110,
    F12 = 111,
    F15 = 113,
    Help = 114,  // Insert
    Home = 115,
    PageUp = 116,
    WindowsDelete = 117,  // Renamed from "kForwardDelete" to avoid nomenclatural confusions.
    F4 = 118,
    End = 119,
    F2 = 120,
    PageDown = 121,
    F1 = 122,
    LeftArrow = 123,
    RightArrow = 124,
    DownArrow = 125,
    UpArrow = 126,
  }

  public enum BlackListedKeyCodes : ushort {
    F17 = 64,
    VolumeUp = 72,
    VolumeDown = 73,
    Mute = 74,
    F18 = 79,
    F19 = 80,
    F20 = 90,
    F5 = 96,
    F6 = 97,
    F7 = 98,
    F3 = 99,
    F8 = 100,
    F9 = 101,
    F11 = 103,
    F13 = 105,  // PrtSc
    F16 = 106,
    F14 = 107,
    F10 = 109,
    F12 = 111,
    F15 = 113,
    Help = 114,  // Insert
    F4 = 118,
    F2 = 120,
    F1 = 122,
  }
}
}
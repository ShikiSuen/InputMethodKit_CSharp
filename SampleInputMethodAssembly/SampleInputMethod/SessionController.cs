using System;
using AppKit;
using Foundation;
using InputMethodKit;

namespace SampleInputMethod {
[Register("SessionController")]
public partial class SessionController : IMKInputController {
  public SessionController(IMKServer server, NSObject @delegate, NSObject inputClient) {}

  public bool HandleEvent(NSEvent @event, NSObject sender) { return false; }

  public override void ActivateServer(NSObject sender) {}

  public override void DeactivateServer(NSObject sender) {}

  public override void SetValue(NSObject value, nint tag, NSObject sender) {}

  public override nuint RecognizedEvents(NSObject sender) {
    const int a = (int)NSEventType.KeyDown;
    const int b = (int)NSEventType.KeyUp;
    const int c = (int)NSEventType.FlagsChanged;
    const int d = a + b + c;
    return (nuint)d;
  }

  public override void UpdateComposition() {}

  public override void CancelComposition() {}

  public override NSMenu Menu => CreateInputMethodMenu();

  private NSMenu CreateInputMethodMenu() {
    NSMenu resultMenu = new();
    resultMenu.AddItem("Test Item", null, "");
    return resultMenu;
  }
}
}

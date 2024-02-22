using System;
using AppKit;
using Foundation;
using InputMethodKit;

namespace SampleInputMethod {
[Register("SessionController")]
public partial class SessionController : IMKInputController {
  private InputHandler handler;

  private IMKTextInput? cachedClient;

  public SessionController(IMKServer theServer, NSObject theDelegate, NSObject inputClient) {
    _ = theServer;
    _ = theDelegate;
    cachedClient = (IMKTextInput?)inputClient ?? Client();
    handler = new(controller: this);
  }

  public override IMKTextInput Client() => cachedClient ?? base.Client();

  public bool HandleEvent(NSEvent givenEvent, NSObject sender) {
    if (sender is not IMKTextInput) return false;  // Check sender's validity.
    if (givenEvent is null) return false;          // macOS feeds nulled NSEvents to this method in some situations.
    bool dropEvent = (givenEvent.Type != NSEventType.KeyDown && givenEvent.Type != NSEventType.FlagsChanged);
    if (dropEvent) return false;  // In this demo we only handle KeyDown.
    var kbEvent = new KBEvent(nsEvent: givenEvent);
    return handler.TriageInput(givenEvent: kbEvent);
  }

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
}

public partial class SessionController {
  public void CommitText(string textToCommit) { Client().InsertText((NSString)textToCommit, new(0, 0)); }

  private NSMenu CreateInputMethodMenu() {
    NSMenu resultMenu = new();
    resultMenu.AddItem("Test Item", null, "");
    return resultMenu;
  }
}
}

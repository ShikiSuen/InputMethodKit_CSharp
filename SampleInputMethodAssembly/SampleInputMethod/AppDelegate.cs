using AppKit;
using Foundation;

namespace SampleInputMethod {
[Register("SampleInputMethodAppDelegate")]
public class SampleInputMethodAppDelegate : NSApplicationDelegate {
  public SampleInputMethodAppDelegate() {}

  public override void DidFinishLaunching(NSNotification notification) {}

  public override void WillTerminate(NSNotification notification) {
    // Insert code here to tear down your application
  }
}
}

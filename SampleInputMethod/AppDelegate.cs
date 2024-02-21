using Foundation;

namespace SampleInputMethod {
[Register("AppDelegate")]
public class AppDelegate : SampleInputMethodAppDelegate {
  public AppDelegate() {}

  public override void DidFinishLaunching(NSNotification notification) =>
      base.DidFinishLaunching(notification: notification);

  public override void WillTerminate(NSNotification notification) => base.WillTerminate(notification: notification);
}
}

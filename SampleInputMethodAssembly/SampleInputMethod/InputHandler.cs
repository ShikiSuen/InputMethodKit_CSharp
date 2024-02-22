

namespace SampleInputMethod {
public class InputHandler {
  public SessionController controller { get; }

  public InputHandler(SessionController controller) { this.controller = controller; }

  public bool TriageInput(KBEvent givenEvent) {
    // Implement the key event processings here.
    // Return true if the processing intercepts the given key event, and false if not intercepting it.
    if (givenEvent.IsMainAreaNumKey) {
      var maybeKeyStr = KBEvent.NonNumPadNumericalKeys[givenEvent.KeyCode];
      string? translated = maybeKeyStr switch {
        "1" => "One", "2" => "Two",   "3" => "Three", "4" => "Four", "5" => "Five",
        "6" => "Six", "7" => "Seven", "8" => "Eight", "9" => "Nine", "0" => "Zero",
        _ => null,
      };
      if (translated is not string translatedKeyStr) {
        return false;
      }
      controller.CommitText(translatedKeyStr);
      return true;
    }
    return false;
  }
}
}

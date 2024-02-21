// (c) 2024 and onwards The vChewing Project (MIT License).
// ====================
// This code is released under the MIT license (SPDX-License-Identifier: MIT)

using System;
using AppKit;
using Foundation;
using ObjCRuntime;
using CoreGraphics;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace InputMethodKit {
[BaseType(typeof(NSObject))]
[Protocol, Model]
public interface NSObject_IMKServerProxy {
  // extern const NSString * IMKModeDictionary;
  [Export("IMKModeDictionary")]
  public NSString IMKModeDictionary { get; }

  // extern const NSString * IMKControllerClass;
  [Export("IMKControllerClass")]
  public NSString IMKControllerClass { get; }

  // extern const NSString * IMKDelegateClass;
  [Export("IMKDelegateClass")]
  public NSString IMKDelegateClass { get; }
}

// @interface IMKServer : NSObject <IMKServerProxy>
[BaseType(typeof(NSObject))]
[Protocol, Model]
public interface IMKServer : NSObject_IMKServerProxy {
  // -(id)initWithName:(NSString *)name bundleIdentifier:(NSString *)bundleIdentifier;
  [Export("initWithName:bundleIdentifier:")]
  public NativeHandle Constructor(string name, string bundleIdentifier);

  // -(id)initWithName:(NSString *)name controllerClass:(Class)controllerClassID delegateClass:(Class)delegateClassID;
  [Export("initWithName:controllerClass:delegateClass:")]
  public NativeHandle Constructor(string name, Class controllerClassID, Class delegateClassID);

  // -(NSBundle *)bundle;
  [Export("bundle")]
  public NSBundle Bundle { get; }

  // -(BOOL)paletteWillTerminate __attribute__((availability(macos, introduced=10.7)));
  [Introduced(PlatformName.MacOSX, 10, 7)]
  [Export("paletteWillTerminate")]
  public bool PaletteWillTerminate {
    get;
  }

  // -(BOOL)lastKeyEventWasDeadKey __attribute__((availability(macos, introduced=10.7)));
  [Introduced(PlatformName.MacOSX, 10, 7)]
  [Export("lastKeyEventWasDeadKey")]
  public bool LastKeyEventWasDeadKey {
    get;
  }
}

public partial interface Constants {
  // extern const NSString * kIMKCommandMenuItemName;
  [Export("kIMKCommandMenuItemName")]
  public NSString kIMKCommandMenuItemName { get; }

  // extern const NSString * kIMKCommandClientName;
  [Export("kIMKCommandClientName")]
  public NSString kIMKCommandClientName { get; }
}

// @interface IMKServerInput (NSObject)
[BaseType(typeof(NSObject))]
[Protocol, Model]
public interface NSObject_IMKServerInput {
  // -(BOOL)inputText:(NSString *)string key:(NSInteger)keyCode modifiers:(NSUInteger)flags client:(id)sender;
  [Export("inputText:key:modifiers:client:")]
  public bool InputText(string @string, nint keyCode, nuint flags, NSObject sender);

  // -(BOOL)inputText:(NSString *)string client:(id)sender;
  [Export("inputText:client:")]
  public bool InputText(string @string, NSObject sender);

  // -(BOOL)handleEvent:(NSEvent *)event client:(id)sender;
  [Export("handleEvent:client:")]
  public bool HandleEvent(NSEvent @event, NSObject sender);

  // -(BOOL)didCommandBySelector:(SEL)aSelector client:(id)sender;
  [Export("didCommandBySelector:client:")]
  public bool DidCommandBySelector(Selector aSelector, NSObject sender);

  // -(id)composedString:(id)sender;
  [Export("composedString:")]
  public NSObject ComposedString(NSObject sender);

  // -(NSAttributedString *)originalString:(id)sender;
  [Export("originalString:")]
  public NSAttributedString OriginalString(NSObject sender);

  // -(void)commitComposition:(id)sender;
  [Export("commitComposition:")]
  public void CommitComposition(NSObject sender);

  // -(NSArray *)candidates:(id)sender;
  [Export("candidates:")]
  public NSAttributedString[] Candidates(NSObject sender);
}

// @protocol IMKStateSetting
[Protocol]
public interface IMKStateSetting {
  // @required -(void)activateServer:(id)sender;
  [Abstract]
  [Export("activateServer:")]
  public void ActivateServer(NSObject sender);

  // @required -(void)deactivateServer:(id)sender;
  [Abstract]
  [Export("deactivateServer:")]
  public void DeactivateServer(NSObject sender);

  // @required -(id)valueForTag:(long)tag client:(id)sender;
  [Abstract]
  [Export("valueForTag:client:")]
  public NSObject ValueForTag(nint tag, NSObject sender);

  // @required -(void)setValue:(id)value forTag:(long)tag client:(id)sender;
  [Abstract]
  [Export("setValue:forTag:client:")]
  public void SetValue(NSObject value, nint tag, NSObject sender);

  // @required -(NSDictionary *)modes:(id)sender;
  [Abstract]
  [Export("modes:")]
  public NSDictionary Modes(NSObject sender);

  // @required -(NSUInteger)recognizedEvents:(id)sender;
  [Abstract]
  [Export("recognizedEvents:")]
  public nuint RecognizedEvents(NSObject sender);

  // @required -(void)showPreferences:(id)sender;
  [Abstract]
  [Export("showPreferences:")]
  public void ShowPreferences(NSObject sender);
}

// @protocol IMKMouseHandling
[Protocol]
public interface IMKMouseHandling {
  // @required -(BOOL)mouseDownOnCharacterIndex:(NSUInteger)index coordinate:(NSPoint)point
  // withModifier:(NSUInteger)flags continueTracking:(BOOL *)keepTracking client:(id)sender;

  [Abstract]
  [Export("mouseDownOnCharacterIndex:coordinate:withModifier:continueTracking:client:")]
  public bool MouseDownOnCharacterIndex(nuint index, CGPoint point, nuint flags, ref bool keepTracking,
                                        NSObject sender);

  // @required -(BOOL)mouseUpOnCharacterIndex:(NSUInteger)index coordinate:(NSPoint)point withModifier:(NSUInteger)flags
  // client:(id)sender;
  [Abstract]
  [Export("mouseUpOnCharacterIndex:coordinate:withModifier:client:")]
  public bool MouseUpOnCharacterIndex(nuint index, CGPoint point, nuint flags, NSObject sender);

  // @required -(BOOL)mouseMovedOnCharacterIndex:(NSUInteger)index coordinate:(NSPoint)point
  // withModifier:(NSUInteger)flags client:(id)sender;
  [Abstract]
  [Export("mouseMovedOnCharacterIndex:coordinate:withModifier:client:")]
  public bool MouseMovedOnCharacterIndex(nuint index, CGPoint point, nuint flags, NSObject sender);
}

// @interface IMKInputController : NSObject <IMKStateSetting, IMKMouseHandling>
[BaseType(typeof(NSObject))]
public interface IMKInputController : IMKStateSetting, IMKMouseHandling {
  // -(id)initWithServer:(IMKServer *)server delegate:(id)delegate client:(id)inputClient;
  [Export("initWithServer:delegate:client:")]
  public NativeHandle Constructor(IMKServer server, NSObject @delegate, NSObject inputClient);

  // -(void)updateComposition;
  [Export("updateComposition")]
  public void UpdateComposition();

  // -(void)cancelComposition;
  [Export("cancelComposition")]
  public void CancelComposition();

  // -(NSMutableDictionary *)compositionAttributesAtRange:(NSRange)range;
  [Export("compositionAttributesAtRange:")]
  public NSMutableDictionary CompositionAttributesAtRange(NSRange range);

  // -(NSRange)selectionRange;
  [Export("selectionRange")]
  public NSRange SelectionRange { get; }

  // -(NSRange)replacementRange;
  [Export("replacementRange")]
  public NSRange ReplacementRange {
    get;
  }

  // -(NSDictionary *)markForStyle:(NSInteger)style atRange:(NSRange)range;
  [Export("markForStyle:atRange:")]
  public NSDictionary MarkForStyle(nint style, NSRange range);

  // -(void)doCommandBySelector:(SEL)aSelector commandDictionary:(NSDictionary *)infoDictionary;
  [Export("doCommandBySelector:commandDictionary:")]
  public void DoCommandBySelector(Selector aSelector, NSDictionary infoDictionary);

  // -(void)hidePalettes;
  [Export("hidePalettes")]
  public void HidePalettes();

  // -(NSMenu *)menu;
  [Export("menu")]
  public NSMenu Menu { get; }

  // -(id)delegate;
  // -(void)setDelegate:(id)newDelegate;
  [Export("delegate")]
  public NSObject Delegate {
    get; set;
  }

  // -(IMKServer *)server;
  [Export("server")]
  public IMKServer Server();

  // -(id<IMKTextInput,NSObject>)client;
  [Export("client")]
  public IMKTextInput Client();

  // -(void)inputControllerWillClose __attribute__((availability(macos, introduced=10.7)));
  [Introduced(PlatformName.MacOSX, 10, 7)]
  [Export("inputControllerWillClose")]
  public void InputControllerWillClose();

  // -(void)annotationSelected:(NSAttributedString *)annotationString forCandidate:(NSAttributedString
  // *)candidateString;
  [Export("annotationSelected:forCandidate:")]
  public void AnnotationSelected(NSAttributedString annotationString, NSAttributedString candidateString);

  // -(void)candidateSelectionChanged:(NSAttributedString *)candidateString;
  [Export("candidateSelectionChanged:")]
  public void CandidateSelectionChanged(NSAttributedString candidateString);

  // -(void)candidateSelected:(NSAttributedString *)candidateString;
  [Export("candidateSelected:")]
  public void CandidateSelected(NSAttributedString candidateString);
}

[BaseType(typeof(NSObject))]
[Protocol, Model]
public interface IMKTextInput {
  // @required -(struct CGRect)firstRectForCharacterRange:(struct _NSRange)arg1 actualRange:(struct _NSRange *)arg2;
  [Abstract]
  [Export("firstRectForCharacterRange:actualRange:")]
  public CGRect FirstRectForCharacterRange(NSRange arg1, ref NSRange arg2);

  // @required -(NSString *)stringFromRange:(struct _NSRange)arg1 actualRange:(struct _NSRange *)arg2;
  [Abstract]
  [Export("stringFromRange:actualRange:")]
  public string StringFromRange(NSRange arg1, ref NSRange arg2);

  // @required -(NSString *)uniqueClientIdentifierString;
  [Abstract]
  [Export("uniqueClientIdentifierString")]
  public string UniqueClientIdentifierString { get; }

  // @required -(id)supportsProperty:(unsigned int)arg1;
  [Abstract]
  [Export("supportsProperty:")]
  public NSObject SupportsProperty(uint arg1);

  // @required -(int)windowLevel;
  [Abstract]
  [Export("windowLevel")]
  public int WindowLevel { get; }

  // @required -(NSString *)bundleIdentifier;
  [Abstract]
  [Export("bundleIdentifier")]
  public string BundleIdentifier { get; }

  // @required -(id)supportsUnicode;
  [Abstract]
  [Export("supportsUnicode")]
  public NSObject SupportsUnicode { get; }

  // @required -(void)selectInputMode:(NSString *)arg1;
  [Abstract]
  [Export("selectInputMode:")]
  public void SelectInputMode(string arg1);

  // @required -(void)overrideKeyboardWithKeyboardNamed:(NSString *)arg1;
  [Abstract]
  [Export("overrideKeyboardWithKeyboardNamed:")]
  public void OverrideKeyboardWithKeyboardNamed(string arg1);

  // @required -(NSArray *)validAttributesForMarkedText;
  [Abstract]
  [Export("validAttributesForMarkedText")]
  public NSObject[] ValidAttributesForMarkedText { get; }

  // @required -(NSDictionary *)attributesForCharacterIndex:(unsigned long long)arg1 lineHeightRectangle:(struct CGRect
  // *)arg2;
  [Abstract]
  [Export("attributesForCharacterIndex:lineHeightRectangle:")]
  public NSDictionary AttributesForCharacterIndex(ulong arg1, ref CGRect arg2);

  // @required -(long long)characterIndexForPoint:(struct CGPoint)arg1 tracking:(long long)arg2 inMarkedRange:(char
  // *)arg3;
  [Abstract]
  [Export("characterIndexForPoint:tracking:inMarkedRange:")]
  public long CharacterIndexForPoint(CGPoint arg1, long arg2, ref CGRect arg3);
  // @required -(long long)length;
  [Abstract]
  [Export("length")]
  public long Length { get; }

  // @required -(NSAttributedString *)attributedSubstringFromRange:(struct _NSRange)arg1;
  [Abstract]
  [Export("attributedSubstringFromRange:")]
  public NSAttributedString AttributedSubstringFromRange(NSRange arg1);

  // @required -(struct _NSRange)markedRange;
  [Abstract]
  [Export("markedRange")]
  public NSRange MarkedRange { get; }

  // @required -(struct _NSRange)selectedRange;
  [Abstract]
  [Export("selectedRange")]
  public NSRange SelectedRange { get; }

  // @required -(void)setMarkedText:(id)arg1 selectionRange:(struct _NSRange)arg2 replacementRange:(struct
  // _NSRange)arg3;
  [Abstract]
  [Export("setMarkedText:selectionRange:replacementRange:")]
  public void SetMarkedText(NSObject arg1, NSRange arg2, NSRange arg3);

  // @required -(void)insertText:(id)arg1 replacementRange:(struct _NSRange)arg2;
  [Abstract]
  [Export("insertText:replacementRange:")]
  public void InsertText(NSObject arg1, NSRange arg2);
}

}

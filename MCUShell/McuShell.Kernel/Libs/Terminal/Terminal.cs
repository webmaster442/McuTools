using System;
using System.Collections.Generic;
using System.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;

namespace AurelienRibon.Ui.Terminal {
	public class Terminal : TextBox {
		protected enum CommandHistoryDirection { BACKWARD, FORWARD }

		public bool IsPromptInsertedAtLaunch { get; set; }
		public bool IsSystemBeepEnabled { get; set; }
		public string Prompt { get; set; }

		public List<string> CommandLog { get; private set; }
        public int LastPomptIndex { get; private set; }
		public bool IsInputEnabled { get; private set; }

		private int indexInLog = 0;

		public Terminal() {
			IsUndoEnabled = false;
			AcceptsReturn = false;
			AcceptsTab = false;

			CommandLog = new List<string>();
			IsPromptInsertedAtLaunch = true;
			IsSystemBeepEnabled = true;
			LastPomptIndex = -1;
			Prompt = "> ";
			IsInputEnabled = false;

			Loaded += (s, e) => {
				if (IsPromptInsertedAtLaunch)
					InsertNewPrompt();
			};

			TextChanged += (s, e) => {
				ScrollToEnd();
			};
		}

		// --------------------------------------------------------------------
		// PUBLIC INTERFACE
		// --------------------------------------------------------------------

		public void InsertNewPrompt() {
			if (Text.Length > 0)
				Text += Text.EndsWith("\n") ? "" : "\n";
			Text += Prompt;
			CaretIndex = Text.Length;
			LastPomptIndex = Text.Length;
			IsInputEnabled = true;
		}

		public void InsertLineBeforePrompt(string text) {
			int oldPromptIndex = LastPomptIndex;
			String insertedText = text + (text.EndsWith("\n") ? "" : "\n");
			Text = Text.Insert(LastPomptIndex - Prompt.Length, insertedText);
			CaretIndex = Text.Length;
			LastPomptIndex = oldPromptIndex + insertedText.Length;
		}

		// --------------------------------------------------------------------
		// EVENT HANDLER
		// --------------------------------------------------------------------

		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			// If Ctrl+C is entered, raise an abortrequested event !
			if (e.Key == Key.C) {
				if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
					RaiseAbortRequested();
					e.Handled = true;
					return;
				}
			}

			// Store the length of Text before any input processing.
			int initialLength = Text.Length;

			// If input is not allowed, warn the user and discard its input.
			if (!IsInputEnabled) {
				if (IsSystemBeepEnabled)
					SystemSounds.Beep.Play();
				e.Handled = true;
				return;
			}

			// Test the caret position.
			//
			// 1. If located before the last prompt index
			//    ==> Warn, set the caret at the end of input text, add text, discard the input
			//        if user tries to erase text, else process it.
			//
			// 2. If located at the last prompt index and user tries to erase text
			//    ==> Warn, discard the input.
			//
			// 3. If located at the last prompt index and user tries to move backward
			//    ==> Warn, discard the input.
			//
			// 4. If located after (>=) the last prompt index and user presses the UP key
			//    ==> Launch command history backward, discard the input.
			//
			// 5. If located after (>=) the last prompt index and user presses the UP key
			//    ==> Launch command history forward, discard the input.
			//
			if (CaretIndex < LastPomptIndex) {
				if (IsSystemBeepEnabled)
					SystemSounds.Beep.Play();
				CaretIndex = Text.Length;
				e.Handled = false;
				if (e.Key == Key.Back || e.Key == Key.Delete)
					e.Handled = true;
			} else if (CaretIndex == LastPomptIndex && e.Key == Key.Back) {
				if (IsSystemBeepEnabled)
					SystemSounds.Beep.Play();
				e.Handled = true;
			} else if (CaretIndex == LastPomptIndex && e.Key == Key.Left) {
				if (IsSystemBeepEnabled)
					SystemSounds.Beep.Play();
				e.Handled = true;
			} else if (CaretIndex >= LastPomptIndex && e.Key == Key.Up) {
				HandleCommandHistoryRequest(CommandHistoryDirection.BACKWARD);
				e.Handled = true;
			} else if (CaretIndex >= LastPomptIndex && e.Key == Key.Down) {
				HandleCommandHistoryRequest(CommandHistoryDirection.FORWARD);
				e.Handled = true;
			}

			// If input has not yet been discarded, test the key for special inputs.
			// ENTER   => validates the input
			// TAB     => launches command completion with registered commands
			// CTRL+C  => raises an abort request event
			if (!e.Handled) {
				switch (e.Key) {
					case Key.Enter:
						HandleEnterKey();
						e.Handled = true;
						break;
				}
			}

			base.OnPreviewKeyDown(e);
		}

		// --------------------------------------------------------------------
		// VIRTUAL METHODS
		// --------------------------------------------------------------------

		protected virtual void HandleCommandHistoryRequest(CommandHistoryDirection direction) {
			switch (direction) {
				case CommandHistoryDirection.BACKWARD:
					if (indexInLog > 0)
						indexInLog--;
					if (CommandLog.Count > 0) {
						Text = GetTextWithPromptSuffix(CommandLog[indexInLog]);
						CaretIndex = Text.Length;
					}
					break;

				case CommandHistoryDirection.FORWARD:
					if (indexInLog < CommandLog.Count - 1)
						indexInLog++;
					if (CommandLog.Count > 0) {
						Text = GetTextWithPromptSuffix(CommandLog[indexInLog]);
						CaretIndex = Text.Length;
					}
					break;
			}
		}

		protected virtual void HandleEnterKey() {
			string line = Text.Substring(LastPomptIndex);
			Text += "\n";
			IsInputEnabled = false;
			LastPomptIndex = int.MaxValue;

			CommandLog.Add(line);
			indexInLog = CommandLog.Count;
			RaiseCommandEntered(line);
		}

		// --------------------------------------------------------------------
		// CLASS SPECIFIC UTILITIES
		// --------------------------------------------------------------------

		protected string GetTextWithPromptSuffix(string suffix) {
			string ret = Text.Substring(0, LastPomptIndex);
			return ret + suffix;
		}

		// --------------------------------------------------------------------
		// GENERAL UTILITIES
		// --------------------------------------------------------------------

		protected string GetShortestString(string[] strs) {
			string ret = strs[0];
			foreach (string str in strs)
				ret = str.Length < ret.Length ? str : ret;
			return ret;
		}

		protected string GetCommonPrefix(string[] strs) {
			string shortestStr = GetShortestString(strs);
			for (int i = 0; i < shortestStr.Length; i++)
				foreach (string str in strs)
					if (char.ToLower(str[i]) != char.ToLower(shortestStr[i]))
						return shortestStr.Substring(0, i);
			return shortestStr;
		}

		// --------------------------------------------------------------------
		// CUSTOM EVENTS
		// --------------------------------------------------------------------

		public event EventHandler<EventArgs> AbortRequested;
		public event EventHandler<CommandEventArgs> CommandEntered;

		public class CommandEventArgs : EventArgs {
			public string Command { get; private set; }
			public CommandEventArgs(string command) {
				Command = command;
			}
		}

		private void RaiseAbortRequested() {
			if (AbortRequested != null)
				AbortRequested(this, new EventArgs());
		}

		private void RaiseCommandEntered(string command) {
			if (CommandEntered != null)
				CommandEntered(this, new CommandEventArgs(command));
		}
	}
}

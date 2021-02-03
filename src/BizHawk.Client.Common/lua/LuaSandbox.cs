using System;
using System.Runtime.CompilerServices;
using BizHawk.Common;
using NLua;

// TODO - evaluate for re-entrancy problems
namespace BizHawk.Client.Common
{
	public class LuaSandbox
	{
		private static readonly ConditionalWeakTable<Lua, LuaSandbox> SandboxForThread = new ConditionalWeakTable<Lua, LuaSandbox>();

		public static Action<string> DefaultLogger { get; set; }

		public void SetSandboxCurrentDirectory(string dir)
		{
			_currentDirectory = dir;
		}

		private string _currentDirectory;
		private string _lastSetDirectory;

		private bool CoolSetCurrentDirectory(string path, string currDirSpeedHack = null)
		{
			string target = $"{_currentDirectory}\\";

			// first we'll bypass it with a general hack: don't do any setting if the value's already there (even at the OS level, setting the directory can be slow)
			// yeah I know, not the smoothest move to compare strings here, in case path normalization is happening at some point
			// but you got any better ideas?
			currDirSpeedHack ??= OSTailoredCode.IsUnixHost ? Environment.CurrentDirectory : CWDHacks.Get();
			if (currDirSpeedHack == path)
			{
				_lastSetDirectory = path;
				return true;
			}

			if (!OSTailoredCode.IsUnixHost) return CWDHacks.Set(target);
			if (!System.IO.Directory.Exists(_currentDirectory)) return false; //TODO is this necessary with Mono? Linux is fine with the CWD being nonexistent. also, is this necessary with .NET Core on Windows? --yoshi
			_lastSetDirectory = Environment.CurrentDirectory = _currentDirectory;
			return true;
		}

		private void Sandbox(Action callback, Action exceptionCallback)
		{
			try
			{
				if (_currentDirectory != null && _currentDirectory != _lastSetDirectory)
				{
					CoolSetCurrentDirectory(_currentDirectory);
				}

				EnvironmentSandbox.Sandbox(callback);
			}
			catch (NLua.Exceptions.LuaException ex)
			{
				Console.WriteLine(ex);
				DefaultLogger(ex.ToString());
				exceptionCallback?.Invoke();
			}
		}

		public static LuaSandbox CreateSandbox(Lua thread, string initialDirectory)
		{
			var sandbox = new LuaSandbox();
			SandboxForThread.Add(thread, sandbox);
			sandbox.SetSandboxCurrentDirectory(initialDirectory);
			return sandbox;
		}

		/// <exception cref="InvalidOperationException">could not get sandbox reference for thread (<see cref="CreateSandbox"/> has not been called)</exception>
		public static LuaSandbox GetSandbox(Lua thread)
		{
			// this is just placeholder.
			// we shouldn't be calling a sandbox with no thread--construct a sandbox properly
			if (thread == null)
			{
				return new LuaSandbox();
			}

			lock (SandboxForThread)
			{
				if (SandboxForThread.TryGetValue(thread, out var sandbox))
				{
					return sandbox;
				}
				
				// for now: throw exception (I want to manually creating them)
				// return CreateSandbox(thread);
				throw new InvalidOperationException("HOARY GORILLA HIJINX");
			}
		}

		public static void Sandbox(Lua thread, Action callback, Action exceptionCallback = null)
		{
			GetSandbox(thread).Sandbox(callback, exceptionCallback);
		}
	}
}

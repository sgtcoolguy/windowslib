using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace wstool
{
	public enum ActivateOptions
	{
		// No flags set
		None = 0x00000000,

		// The application is being activated for design mode, and thus will not be able to
		// to create an immersive window. Window creation must be done by design tools which
		// load the necessary components by communicating with a designer-specified service on
		// the site chain established on the activation manager.  The splash screen normally
		// shown when an application is activated will also not appear.  Most activations
		// will not use this flag.
		DesignMode = 0x00000001,

		// Do not show an error dialog if the app fails to activate
		NoErrorUI = 0x00000002,

		// Do not show the splash screen when activating the app
		NoSplashScreen = 0x00000004
	}

	[ComImport, Guid("2e941141-7f97-4756-ba1d-9decde894a3d"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	interface IApplicationActivationManager
	{
		// Activates the specified immersive application for the "Launch" contract, passing the provided arguments
		// string into the application.  Callers can obtain the process Id of the application instance fulfilling this contract.
		IntPtr ActivateApplication([In] String appUserModelId, [In] String arguments, [In] ActivateOptions options, [Out] out UInt32 processId);
		IntPtr ActivateForFile([In] String appUserModelId, [In] IntPtr /*IShellItemArray* */ itemArray, [In] String verb, [Out] out UInt32 processId);
		IntPtr ActivateForProtocol([In] String appUserModelId, [In] IntPtr /* IShellItemArray* */itemArray, [Out] out UInt32 processId);
	}

	[ComImport, Guid("45BA127D-10A8-46EA-8AB7-56EA9078943C")] // Application Activation Manager
	class ApplicationActivationManager : IApplicationActivationManager
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)/*, PreserveSig*/]
		public extern IntPtr ActivateApplication([In] String appUserModelId, [In] String arguments, [In] ActivateOptions options, [Out] out UInt32 processId);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		public extern IntPtr ActivateForFile([In] String appUserModelId, [In] IntPtr /*IShellItemArray* */ itemArray, [In] String verb, [Out] out UInt32 processId);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		public extern IntPtr ActivateForProtocol([In] String appUserModelId, [In] IntPtr /* IShellItemArray* */itemArray, [Out] out UInt32 processId);
	}

    public enum PACKAGE_EXECUTION_STATE
    {
        PES_UNKNOWN,
        PES_RUNNING,
        PES_SUSPENDING,
        PES_SUSPENDED,
        PES_TERMINATED
    }
    [ComImport, Guid("B1AEC16F-2383-4852-B0E9-8F0B1DC66B4D")]
    public class PackageDebugSettings
    {
    }

    [ComImport, Guid("F27C3930-8029-4AD1-94E3-3DBA417810C1"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPackageDebugSettings
    {
        int EnableDebugging([MarshalAs(UnmanagedType.LPWStr)] string packageFullName, [MarshalAs(UnmanagedType.LPWStr)] string debuggerCommandLine, IntPtr environment);
        int DisableDebugging([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);
        int Suspend([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);
        int Resume([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);
        int TerminateAllProcesses([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);
        int SetTargetSessionId(int sessionId);
        int EnumerageBackgroundTasks([MarshalAs(UnmanagedType.LPWStr)] string packageFullName,
                                                      out uint taskCount, out int intPtr, [Out] string[] array);
        int ActivateBackgroundTask(IntPtr something);
        int StartServicing([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);
        int StopServicing([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);
        int StartSessionRedirection([MarshalAs(UnmanagedType.LPWStr)] string packageFullName, uint sessionId);
        int StopSessionRedirection([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);
        int GetPackageExecutionState([MarshalAs(UnmanagedType.LPWStr)] string packageFullName,
                                            out PACKAGE_EXECUTION_STATE packageExecutionState);
        int RegisterForPackageStateChanges([MarshalAs(UnmanagedType.LPWStr)] string packageFullName,
                               IntPtr pPackageExecutionStateChangeNotification, out uint pdwCookie);
        int UnregisterForPackageStateChanges(uint dwCookie);
    }

    public class DebugTool
    {
        [DllImport("Ole32.dll")]
        public static extern int CoAllowSetForegroundWindow(IntPtr pUnk, IntPtr lpvReserved);

        public static void EnableDebug(String packageFullName)
        {
            // Set debug mode for App and activate installed application
            var debugSettings = (IPackageDebugSettings)(new PackageDebugSettings());
            debugSettings.EnableDebugging(packageFullName, null, (IntPtr)null);
        }
    }
}

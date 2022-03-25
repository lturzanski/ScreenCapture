using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Theme.Controls
{
    /// <summary>
    /// Adds icon content to a standard button.
    /// </summary>
    public class ModernButton
        : Button
    {
        /// <summary>
        /// Identifies the EllipseDiameter property.
        /// </summary>
        public static readonly DependencyProperty EllipseDiameterProperty = DependencyProperty.Register("EllipseDiameter", typeof(double), typeof(ModernButton), new PropertyMetadata(22D));
        /// <summary>
        /// Identifies the EllipseStrokeThickness property.
        /// </summary>
        public static readonly DependencyProperty EllipseStrokeThicknessProperty = DependencyProperty.Register("EllipseStrokeThickness", typeof(double), typeof(ModernButton), new PropertyMetadata(1D));
        /// <summary>
        /// Identifies the IconData property.
        /// </summary>
        public static readonly DependencyProperty IconDataProperty = DependencyProperty.Register("IconData", typeof(Geometry), typeof(ModernButton));
        /// <summary>
        /// Identifies the IconHeight property.
        /// </summary>
        public static readonly DependencyProperty IconHeightProperty = DependencyProperty.Register("IconHeight", typeof(double), typeof(ModernButton), new PropertyMetadata(12D));
        /// <summary>
        /// Identifies the IconWidth property.
        /// </summary>
        public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register("IconWidth", typeof(double), typeof(ModernButton), new PropertyMetadata(12D));

        /// <summary>
        /// Initializes a new instance of the <see cref="ModernButton"/> class.
        /// </summary>
        public ModernButton()
        {
            this.DefaultStyleKey = typeof(ModernButton);
        }

        /// <summary>
        /// Gets or sets the ellipse diameter.
        /// </summary>
        public double EllipseDiameter
        {
            get { return (double)GetValue(EllipseDiameterProperty); }
            set { SetValue(EllipseDiameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the ellipse stroke thickness.
        /// </summary>
        public double EllipseStrokeThickness
        {
            get { return (double)GetValue(EllipseStrokeThicknessProperty); }
            set { SetValue(EllipseStrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the icon path data.
        /// </summary>
        /// <value>
        /// The icon path data.
        /// </value>
        public Geometry IconData
        {
            get { return (Geometry)GetValue(IconDataProperty); }
            set { SetValue(IconDataProperty, value); }
        }

        /// <summary>
        /// Gets or sets the icon height.
        /// </summary>
        /// <value>
        /// The icon height.
        /// </value>
        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the icon width.
        /// </summary>
        /// <value>
        /// The icon width.
        /// </value>
        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int left, top, right, bottom;
    }

    internal enum MonitorDpiType
    {
        EffectiveDpi = 0,
        AngularDpi = 1,
        RawDpi = 2,
        Default = EffectiveDpi,
    }

    internal class NativeMethods
    {
        public const int S_OK = 0;
        public const int WM_DPICHANGED = 0x02E0;
        public const int MONITOR_DEFAULTTONEAREST = 0x00000002;

        [DllImport("Shcore.dll")]
        public static extern int GetProcessDpiAwareness(IntPtr hprocess, out ProcessDpiAwareness value);
        [DllImport("Shcore.dll")]
        public static extern int SetProcessDpiAwareness(ProcessDpiAwareness value);
        [DllImport("user32.dll")]
        public static extern bool IsProcessDPIAware();
        [DllImport("user32.dll")]
        public static extern int SetProcessDPIAware();
        [DllImport("shcore.dll")]
        public static extern int GetDpiForMonitor(IntPtr hMonitor, int dpiType, ref uint xDpi, ref uint yDpi);
        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, int flag);
    }

    // Taken from http://www.codeproject.com/Articles/707502/Version-Helper-API-for-NET
    // License: The Code Project Open License

    /// <summary>
    /// .NET wrapper for Version Helper functions.
    /// http://msdn.microsoft.com/library/windows/desktop/dn424972.aspx
    /// </summary>
    public static class OSVersionHelper
    {
        #region Supplementary data types

        /// <summary>
        /// Operating systems, the information which is stored within
        /// the class <seealso cref="OSVersionHelper"/>.
        /// </summary>
        public enum KnownOS
        {
            /// <summary>
            /// Windows XP.
            /// </summary>
            WindowsXP,

            /// <summary>
            /// Windows XP SP1.
            /// </summary>
            WindowsXPSP1,

            /// <summary>
            /// Windows XP SP2.
            /// </summary>
            WindowsXPSP2,

            /// <summary>
            /// Windows XP SP3.
            /// </summary>
            WindowsXPSP3,

            /// <summary>
            /// Windows Vista.
            /// </summary>
            WindowsVista,

            /// <summary>
            /// Windows Vista SP1.
            /// </summary>
            WindowsVistaSP1,

            /// <summary>
            /// Windows Vista SP2.
            /// </summary>
            WindowsVistaSP2,

            /// <summary>
            /// Windows 7.
            /// </summary>
            Windows7,

            /// <summary>
            /// Windows 7 SP1.
            /// </summary>
            Windows7SP1,

            /// <summary>
            /// Windows 8.
            /// </summary>
            Windows8,

            /// <summary>
            /// Windows 8.1.
            /// </summary>
            Windows8Point1
        }

        /// <summary>
        /// Information about operating system.
        /// </summary>
        private sealed class OsEntry
        {
            #region Properties

            /// <summary>
            /// The major version number of the operating system.
            /// </summary>
            public uint MajorVersion { get; private set; }

            /// <summary>
            /// The minor version number of the operating system.
            /// </summary>
            public uint MinorVersion { get; private set; }

            /// <summary>
            /// The major version number of the latest Service Pack installed
            /// on the system. For example, for Service Pack 3, the major
            /// version number is 3. If no Service Pack has been installed,
            /// the value is zero.
            /// </summary>
            public ushort ServicePackMajor { get; private set; }

            /// <summary>
            /// Flag indicating if the running OS matches, or is greater
            /// than, the OS specified with this entry. Should be initialized
            /// with <see cref="VerifyVersionInfo"/> method.
            /// </summary>
            public bool? MatchesOrGreater { get; set; }

            #endregion // Properties

            #region Constructor

            /// <summary>
            /// Creates a new entry of operating system.
            /// </summary>
            /// <param name="majorVersion">The major version number of the
            /// operating system.</param>
            /// <param name="minorVersion">The minor version number of the
            /// operating system.</param>
            /// <param name="servicePackMajor">The major version number of the
            /// latest Service Pack installed on the system. For example, for
            /// Service Pack 3, the major version number is 3. If no Service
            /// Pack has been installed, the value is zero.</param>
            public OsEntry(uint majorVersion, uint minorVersion,
                ushort servicePackMajor)
            {
                this.MajorVersion = majorVersion;
                this.MinorVersion = minorVersion;
                this.ServicePackMajor = servicePackMajor;
            }

            #endregion // Constructor
        }

        #endregion // Supplementary data types

        #region PInvoke data type declarations

        /// <summary>
        /// Wrapper for OSVERSIONINFOEX structure.
        /// http://msdn.microsoft.com/library/windows/desktop/ms724833.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct OsVersionInfoEx
        {
            /// <summary>
            /// The size of this data structure, in bytes.
            /// </summary>
            public uint OSVersionInfoSize;

            /// <summary>
            /// The major version number of the operating system.
            /// </summary>
            public uint MajorVersion;

            /// <summary>
            /// The minor version number of the operating system.
            /// </summary>
            public uint MinorVersion;

            /// <summary>
            /// The build number of the operating system.
            /// </summary>
            public uint BuildNumber;

            /// <summary>
            /// The operating system platform.
            /// </summary>
            public uint PlatformId;

            /// <summary>
            /// A null-terminated string, such as "Service Pack 3", that
            /// indicates the latest Service Pack installed on the system. If
            /// no Service Pack has been installed, the string is empty.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string CSDVersion;

            /// <summary>
            /// The major version number of the latest Service Pack installed
            /// on the system. For example, for Service Pack 3, the major
            /// version number is 3. If no Service Pack has been installed,
            /// the value is zero.
            /// </summary>
            public ushort ServicePackMajor;

            /// <summary>
            /// The minor version number of the latest Service Pack installed
            /// on the system. For example, for Service Pack 3, the minor
            /// version number is 0.
            /// </summary>
            public ushort ServicePackMinor;

            /// <summary>
            /// A bit mask that identifies the product suites available on the
            /// system, e.g., flags indicating if the operating system is
            /// Datacenter Server or Windows XP Embedded.
            /// </summary>
            public ushort SuiteMask;

            /// <summary>
            /// Any additional information about the system, e.g., flags
            /// indicating if the operating system is a domain controller,
            /// a server or a workstation.
            /// </summary>
            public byte ProductType;

            /// <summary>
            /// Reserved for future use.
            /// </summary>
            public byte Reserved;
        }

        #endregion // PInvoke data type declarations

        #region PInvoke function declarations

        /// <summary>
        /// <para>Wrapper for VerSetConditionMask function (
        /// http://msdn.microsoft.com/library/windows/desktop/ms725493.aspx).
        /// </para>
        /// <para>
        /// Sets the bits of a 64-bit value to indicate the comparison
        /// operator to use for a specified operating system version
        /// attribute. This method is used to build the dwlConditionMask
        /// parameter of the <see cref="VerifyVersionInfo"/> method.
        /// </para>
        /// </summary>
        /// <param name="dwlConditionMask">
        /// <para>A value to be passed as the dwlConditionMask parameter of
        /// the <see cref="VerifyVersionInfo"/> method. The function stores
        /// the comparison information in the bits of this variable.
        /// </para>
        /// <para>
        /// Before the first call to VerSetConditionMask, initialize this
        /// variable to zero. For subsequent calls, pass in the variable used
        /// in the previous call.
        /// </para>
        /// </param>
        /// <param name="dwTypeBitMask">A mask that indicates the member of
        /// the <see cref="OsVersionInfoEx"/> structure whose comparison
        /// operator is being set.</param>
        /// <param name="dwConditionMask">The operator to be used for the
        /// comparison.</param>
        /// <returns>Condition mask value.</returns>
        [DllImport("kernel32.dll")]
        private static extern ulong VerSetConditionMask(ulong dwlConditionMask,
           uint dwTypeBitMask, byte dwConditionMask);

        /// <summary>
        /// <para>
        /// Wrapper for VerifyVersionInfo function (
        /// http://msdn.microsoft.com/library/windows/desktop/ms725492.aspx).
        /// </para>
        /// <para>
        /// Compares a set of operating system version requirements to the
        /// corresponding values for the currently running version of the
        /// system.
        /// </para>
        /// </summary>
        /// <param name="lpVersionInfo">A pointer to an
        /// <see cref="OsVersionInfoEx"/> structure containing the operating
        /// system version requirements to compare.</param>
        /// <param name="dwTypeMask">A mask that indicates the members of the
        /// <see cref="OsVersionInfoEx"/> structure to be tested.</param>
        /// <param name="dwlConditionMask">The type of comparison to be used
        /// for each lpVersionInfo member being compared. Can be constructed
        /// with <see cref="VerSetConditionMask"/> method.</param>
        /// <returns>True if the current Windows OS satisfies the specified
        /// requirements; otherwise, false.</returns>
        [DllImport("kernel32.dll")]
        private static extern bool VerifyVersionInfo(
            [In] ref OsVersionInfoEx lpVersionInfo,
            uint dwTypeMask, ulong dwlConditionMask);

        #endregion // PInvoke declarations

        #region Local fields

        private static Dictionary<KnownOS, OsEntry> osEntries;

        private static bool? isServer = null;

        private static ulong? versionOrGreaterMask;
        private static uint? versionOrGreaterTypeMask;

        #endregion // Local fields

        #region Constructor

        /// <summary>
        /// Initializes dictionary of operating systems.
        /// </summary>
        static OSVersionHelper()
        {
            osEntries = new Dictionary<KnownOS, OsEntry>();
            osEntries.Add(KnownOS.WindowsXP, new OsEntry(5, 1, 0));
            osEntries.Add(KnownOS.WindowsXPSP1, new OsEntry(5, 1, 1));
            osEntries.Add(KnownOS.WindowsXPSP2, new OsEntry(5, 1, 2));
            osEntries.Add(KnownOS.WindowsXPSP3, new OsEntry(5, 1, 3));
            osEntries.Add(KnownOS.WindowsVista, new OsEntry(6, 0, 0));
            osEntries.Add(KnownOS.WindowsVistaSP1, new OsEntry(6, 0, 1));
            osEntries.Add(KnownOS.WindowsVistaSP2, new OsEntry(6, 0, 2));
            osEntries.Add(KnownOS.Windows7, new OsEntry(6, 1, 0));
            osEntries.Add(KnownOS.Windows7SP1, new OsEntry(6, 1, 1));
            osEntries.Add(KnownOS.Windows8, new OsEntry(6, 2, 0));
            osEntries.Add(KnownOS.Windows8Point1, new OsEntry(6, 3, 0));
        }

        #endregion // Constructor

        #region Public methods

        /// <summary>
        /// Indicates if the current OS version matches, or is greater than,
        /// the provided version information. This method is useful in
        /// confirming a version of Windows Server that doesn't share a
        /// version number with a client release.
        /// </summary>
        /// <param name="majorVersion">The major OS version number.</param>
        /// <param name="minorVersion">The minor OS version number.</param>
        /// <param name="servicePackMajor">The major Service Pack version
        /// number.</param>
        /// <returns>True if the the running OS matches, or is greater
        /// than, the specified version information; otherwise, false.
        /// </returns>
        internal static bool IsWindowsVersionOrGreater(
            uint majorVersion, uint minorVersion, ushort servicePackMajor)
        {
            OsVersionInfoEx osvi = new OsVersionInfoEx();
            osvi.OSVersionInfoSize = (uint)Marshal.SizeOf(osvi);
            osvi.MajorVersion = majorVersion;
            osvi.MinorVersion = minorVersion;
            osvi.ServicePackMajor = servicePackMajor;

            // These constants initialized with corresponding definitions in
            // winnt.h (part of Windows SDK)
            const uint VER_MINORVERSION = 0x0000001;
            const uint VER_MAJORVERSION = 0x0000002;
            const uint VER_SERVICEPACKMAJOR = 0x0000020;
            const byte VER_GREATER_EQUAL = 3;

            if (!versionOrGreaterMask.HasValue)
            {
                versionOrGreaterMask = VerSetConditionMask(
                    VerSetConditionMask(
                        VerSetConditionMask(
                            0, VER_MAJORVERSION, VER_GREATER_EQUAL),
                        VER_MINORVERSION, VER_GREATER_EQUAL),
                    VER_SERVICEPACKMAJOR, VER_GREATER_EQUAL);
            }

            if (!versionOrGreaterTypeMask.HasValue)
            {
                versionOrGreaterTypeMask = VER_MAJORVERSION |
                    VER_MINORVERSION | VER_SERVICEPACKMAJOR;
            }

            return VerifyVersionInfo(ref osvi, versionOrGreaterTypeMask.Value,
                versionOrGreaterMask.Value);
        }

        /// <summary>
        /// Indicates if the running OS version matches, or is greater than,
        /// the provided OS.
        /// </summary>
        /// <param name="os">OS to compare running OS to.</param>
        /// <returns>True if the the running OS matches, or is greater
        /// than, the specified OS; otherwise, false.</returns>
        public static bool IsWindowsVersionOrGreater(KnownOS os)
        {
            try
            {
                OsEntry osEntry = osEntries[os];
                if (!osEntry.MatchesOrGreater.HasValue)
                {
                    osEntry.MatchesOrGreater = IsWindowsVersionOrGreater(
                        osEntry.MajorVersion, osEntry.MinorVersion,
                        osEntry.ServicePackMajor);
                }

                return osEntry.MatchesOrGreater.Value;
            }
            catch (KeyNotFoundException e)
            {
                throw new ArgumentException("Unknown OS", e);
            }
        }

        #endregion // Public methods

        #region Public properties

        /// <summary>
        /// Indicates if the current OS version matches, or is greater than,
        /// the Windows XP version.
        /// </summary>
        public static bool IsWindowsXPOrGreater
        {
            get { return IsWindowsVersionOrGreater(KnownOS.WindowsXP); }
        }

        /// <summary>
        /// Indicates if the current OS version matches, or is greater than,
        /// the Windows XP with Service Pack 1 (SP1) version.
        /// </summary>
        public static bool IsWindowsXPSP1OrGreater
        {
            get { return IsWindowsVersionOrGreater(KnownOS.WindowsXPSP1); }
        }

        /// <summary>
        /// Indicates if the current OS version matches, or is greater than,
        /// the Windows XP with Service Pack 2 (SP2) version.
        /// </summary>
        public static bool IsWindowsXPSP2OrGreater
        {
            get { return IsWindowsVersionOrGreater(KnownOS.WindowsXPSP2); }
        }

        /// <summary>
        /// Indicates if the current OS version matches, or is greater than,
        /// the Windows XP with Service Pack 3 (SP3) version.
        /// </summary>
        public static bool IsWindowsXPSP3OrGreater
        {
            get { return IsWindowsVersionOrGreater(KnownOS.WindowsXPSP3); }
        }

        /// <summary>
        /// Indicates if the current OS version matches, or is greater than,
        /// the Windows Vista version.
        /// </summary>
        public static bool IsWindowsVistaOrGreater
        {
            get { return IsWindowsVersionOrGreater(KnownOS.WindowsVista); }
        }

        /// <summary>
        /// Indicates if the current OS version matches, or is greater than,
        /// the Windows Vista with Service Pack 1 (SP1) version.
        /// </summary>
        public static bool IsWindowsVistaSP1OrGreater
        {
            get { return IsWindowsVersionOrGreater(KnownOS.WindowsVistaSP1); }
        }

        /// <summary>
        /// Indicates if the current OS version matches, or is greater than,
        /// the Windows Vista with Service Pack 2 (SP2) version.
        /// </summary>
        public static bool IsWindowsVistaSP2OrGreater
        {
            get { return IsWindowsVersionOrGreater(KnownOS.WindowsVistaSP2); }
        }

        /// <summary>
        /// Indicates if the current OS version matches, or is greater than,
        /// the Windows 7 version.
        /// </summary>
        public static bool IsWindows7OrGreater
        {
            get { return IsWindowsVersionOrGreater(KnownOS.Windows7); }
        }

        /// <summary>
        /// Indicates if the current OS version matches, or is greater than,
        /// the Windows 7 with Service Pack 1 (SP1) version.
        /// </summary>
        public static bool IsWindows7SP1OrGreater
        {
            get { return IsWindowsVersionOrGreater(KnownOS.Windows7SP1); }
        }

        /// <summary>
        /// Indicates if the current OS version matches, or is greater than,
        /// the Windows 8 version.
        /// </summary>
        public static bool IsWindows8OrGreater
        {
            get { return IsWindowsVersionOrGreater(KnownOS.Windows8); }
        }

        /// <summary>
        /// Indicates if the current OS version matches, or is greater than,
        /// the Windows 8.1 version.
        /// </summary>
        public static bool IsWindows8Point1OrGreater
        {
            get { return IsWindowsVersionOrGreater(KnownOS.Windows8Point1); }
        }

        /// <summary>
        /// Indicates if the current OS is a Windows Server release.
        /// </summary>
        public static bool IsWindowsServer
        {
            get
            {
                if (!isServer.HasValue)
                {
                    // These constants initialized with corresponding
                    // definitions in winnt.h (part of Windows SDK)
                    const byte VER_NT_WORKSTATION = 0x0000001;
                    const uint VER_PRODUCT_TYPE = 0x0000080;
                    const byte VER_EQUAL = 1;

                    OsVersionInfoEx osvi = new OsVersionInfoEx();
                    osvi.OSVersionInfoSize = (uint)Marshal.SizeOf(osvi);
                    osvi.ProductType = VER_NT_WORKSTATION;
                    ulong dwlConditionMask = VerSetConditionMask(
                        0, VER_PRODUCT_TYPE, VER_EQUAL);

                    return !VerifyVersionInfo(
                        ref osvi, VER_PRODUCT_TYPE, dwlConditionMask);
                }

                return isServer.Value;
            }
        }

        #endregion // Public properties
    }

    /// <summary>
    /// Identifies dots per inch (dpi) awareness values.
    /// </summary>
    public enum ProcessDpiAwareness
    {
        /// <summary>
        /// Process is not DPI aware.
        /// </summary>
        DpiUnaware = 0,
        /// <summary>
        /// Process is system DPI aware (= WPF default).
        /// </summary>
        SystemDpiAware = 1,
        /// <summary>
        /// Process is per monitor DPI aware (Win81+ only).
        /// </summary>
        PerMonitorDpiAware = 2
    }

    /// <summary>
    /// Provides various common helper methods.
    /// </summary>
    public static class ModernUIHelper
    {
        private static bool? isInDesignMode;

        /// <summary>
        /// Determines whether the current code is executed in a design time environment such as Visual Studio or Blend.
        /// </summary>
        public static bool IsInDesignMode
        {
            get
            {
                if (!isInDesignMode.HasValue)
                {
                    isInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
                }
                return isInDesignMode.Value;
            }
        }

        /// <summary>
        /// Gets the DPI awareness of the current process.
        /// </summary>
        /// <returns>
        /// The DPI awareness of the current process
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception"></exception>
        public static ProcessDpiAwareness GetDpiAwareness()
        {
            if (OSVersionHelper.IsWindows8Point1OrGreater)
            {
                ProcessDpiAwareness value;
                var result = NativeMethods.GetProcessDpiAwareness(IntPtr.Zero, out value);
                if (result != NativeMethods.S_OK)
                {
                    throw new Win32Exception(result);
                }

                return value;
            }
            if (OSVersionHelper.IsWindowsVistaOrGreater)
            {
                // use older Win32 API to query system DPI awareness
                return NativeMethods.IsProcessDPIAware() ? ProcessDpiAwareness.SystemDpiAware : ProcessDpiAwareness.DpiUnaware;
            }

            // assume WPF default
            return ProcessDpiAwareness.SystemDpiAware;
        }

        /// <summary>
        /// Attempts to set the DPI awareness of this process to PerMonitorDpiAware
        /// </summary>
        /// <returns>A value indicating whether the DPI awareness has been set to PerMonitorDpiAware.</returns>
        /// <remarks>
        /// <para>
        /// For this operation to succeed the host OS must be Windows 8.1 or greater, and the initial
        /// DPI awareness must be set to DpiUnaware (apply [assembly:DisableDpiAwareness] to application assembly).
        /// </para>
        /// <para>
        /// When the host OS is Windows 8 or lower, an attempt is made to set the DPI awareness to SystemDpiAware (= WPF default). This
        /// effectively revokes the [assembly:DisableDpiAwareness] attribute if set.
        /// </para>
        /// </remarks>
        public static bool TrySetPerMonitorDpiAware()
        {
            var awareness = GetDpiAwareness();

            // initial awareness must be DpiUnaware
            if (awareness == ProcessDpiAwareness.DpiUnaware)
            {
                if (OSVersionHelper.IsWindows8Point1OrGreater)
                {
                    return NativeMethods.SetProcessDpiAwareness(ProcessDpiAwareness.PerMonitorDpiAware) == NativeMethods.S_OK;
                }

                // use older Win32 API to set the awareness to SystemDpiAware
                return NativeMethods.SetProcessDPIAware() == NativeMethods.S_OK;
            }

            // return true if per monitor was already enabled
            return awareness == ProcessDpiAwareness.PerMonitorDpiAware;
        }
    }

    /// <summary>
    /// Provides DPI information for a window.
    /// </summary>
    public class DpiInformation
    {
        internal DpiInformation(double wpfDpiX, double wpfDpiY)
        {
            this.WpfDpiX = wpfDpiX;
            this.WpfDpiY = wpfDpiY;
            this.ScaleX = 1;
            this.ScaleY = 1;
        }
        /// <summary>
        /// Gets the horizontal resolution of the WPF rendering DPI.
        /// </summary>
        public double WpfDpiX { get; private set; }
        /// <summary>
        /// Gets the vertical resolution of the WPF rendering DPI.
        /// </summary>
        public double WpfDpiY { get; private set; }
        /// <summary>
        /// Gets the horizontal resolution of the current monitor DPI.
        /// </summary>
        /// <remarks>Null when the process is not per monitor DPI aware.</remarks>
        public double? MonitorDpiX { get; private set; }
        /// <summary>
        /// Gets the vertical resolution of the current monitor DPI.
        /// </summary>
        /// <remarks>Null when the process is not per monitor DPI aware.</remarks>
        public double? MonitorDpiY { get; private set; }
        /// <summary>
        /// Gets the x-axis scale factor.
        /// </summary>
        public double ScaleX { get; private set; }
        /// <summary>
        /// Gets the y-axis scale factor.
        /// </summary>
        public double ScaleY { get; private set; }

        internal Vector UpdateMonitorDpi(double dpiX, double dpiY)
        {
            // calculate the vector of the current to new dpi
            var oldDpiX = this.MonitorDpiX ?? this.WpfDpiX;
            var oldDpiY = this.MonitorDpiY ?? this.WpfDpiY;

            this.MonitorDpiX = dpiX;
            this.MonitorDpiY = dpiY;

            this.ScaleX = dpiX / this.WpfDpiX;
            this.ScaleY = dpiY / this.WpfDpiY;

            return new Vector(dpiX / oldDpiX, dpiY / oldDpiY);
        }
    }

    /// <summary>
    /// A window instance that is capable of per-monitor DPI awareness when supported.
    /// </summary>
    public abstract class DpiAwareWindow
        : Window
    {
        /// <summary>
        /// Occurs when the system or monitor DPI for this window has changed.
        /// </summary>
        public event EventHandler DpiChanged;

        private HwndSource source;
        private DpiInformation dpiInfo;
        private bool isPerMonitorDpiAware;

        /// <summary>
        /// Initializes a new instance of the <see cref="DpiAwareWindow"/> class.
        /// </summary>
        public DpiAwareWindow()
        {
            this.SourceInitialized += OnSourceInitialized;

            // WM_DPICHANGED is not send when window is minimized, do listen to global display setting changes
            SystemEvents.DisplaySettingsChanged += OnSystemEventsDisplaySettingsChanged;

            // try to set per-monitor dpi awareness, before the window is displayed
            this.isPerMonitorDpiAware = ModernUIHelper.TrySetPerMonitorDpiAware();
        }

        /// <summary>
        /// Gets the DPI information for this window instance.
        /// </summary>
        /// <remarks>
        /// DPI information is available after a window handle has been created.
        /// </remarks>
        public DpiInformation DpiInformation
        {
            get { return this.dpiInfo; }
        }

        /// <summary>
        /// Raises the System.Windows.Window.Closed event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // detach global event handlers
            SystemEvents.DisplaySettingsChanged -= OnSystemEventsDisplaySettingsChanged;
        }

        private void OnSystemEventsDisplaySettingsChanged(object sender, EventArgs e)
        {
            if (this.source != null && this.WindowState == WindowState.Minimized)
            {
                RefreshMonitorDpi();
            }
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            this.source = (HwndSource)HwndSource.FromVisual(this);

            // calculate the DPI used by WPF; this is the same as the system DPI
            var matrix = source.CompositionTarget.TransformToDevice;

            this.dpiInfo = new DpiInformation(96D * matrix.M11, 96D * matrix.M22);

            if (this.isPerMonitorDpiAware)
            {
                this.source.AddHook(WndProc);

                RefreshMonitorDpi();
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_DPICHANGED)
            {
                // Marshal the value in the lParam into a Rect.
                var newDisplayRect = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));

                // Set the Window's position & size.
                var matrix = this.source.CompositionTarget.TransformFromDevice;
                var ul = matrix.Transform(new Vector(newDisplayRect.left, newDisplayRect.top));
                var hw = matrix.Transform(new Vector(newDisplayRect.right - newDisplayRect.left, newDisplayRect.bottom - newDisplayRect.top));
                this.Left = ul.X;
                this.Top = ul.Y;
                UpdateWindowSize(hw.X, hw.Y);

                // Remember the current DPI settings.
                var oldDpiX = this.dpiInfo.MonitorDpiX;
                var oldDpiY = this.dpiInfo.MonitorDpiY;

                // Get the new DPI settings from wParam
                var dpiX = (double)(wParam.ToInt32() >> 16);
                var dpiY = (double)(wParam.ToInt32() & 0x0000FFFF);

                if (oldDpiX != dpiX || oldDpiY != dpiY)
                {
                    this.dpiInfo.UpdateMonitorDpi(dpiX, dpiY);

                    // update layout scale
                    UpdateLayoutTransform();

                    // raise DpiChanged event
                    OnDpiChanged(EventArgs.Empty);
                }

                handled = true;
            }
            return IntPtr.Zero;
        }

        private void UpdateLayoutTransform()
        {
            if (this.isPerMonitorDpiAware)
            {
                var root = (FrameworkElement)this.GetVisualChild(0);
                if (root != null)
                {
                    if (this.dpiInfo.ScaleX != 1 || this.dpiInfo.ScaleY != 1)
                    {
                        root.LayoutTransform = new ScaleTransform(this.dpiInfo.ScaleX, this.dpiInfo.ScaleY);
                    }
                    else
                    {
                        root.LayoutTransform = null;
                    }
                }
            }
        }

        private void UpdateWindowSize(double width, double height)
        {
            // determine relative scalex and scaley
            var relScaleX = width / this.Width;
            var relScaleY = height / this.Height;

            if (relScaleX != 1 || relScaleY != 1)
            {
                // adjust window size constraints as well
                this.MinWidth *= relScaleX;
                this.MaxWidth *= relScaleX;
                this.MinHeight *= relScaleY;
                this.MaxHeight *= relScaleY;

                this.Width = width;
                this.Height = height;
            }
        }

        /// <summary>
        /// Refreshes the current monitor DPI settings and update the window size and layout scale accordingly.
        /// </summary>
        protected void RefreshMonitorDpi()
        {
            if (!this.isPerMonitorDpiAware)
            {
                return;
            }

            // get the current DPI of the monitor of the window
            var monitor = NativeMethods.MonitorFromWindow(this.source.Handle, NativeMethods.MONITOR_DEFAULTTONEAREST);

            uint xDpi = 96;
            uint yDpi = 96;
            if (NativeMethods.GetDpiForMonitor(monitor, (int)MonitorDpiType.EffectiveDpi, ref xDpi, ref yDpi) != NativeMethods.S_OK)
            {
                xDpi = 96;
                yDpi = 96;
            }
            // vector contains the change of the old to new DPI
            var dpiVector = this.dpiInfo.UpdateMonitorDpi(xDpi, yDpi);

            // update Width and Height based on the current DPI of the monitor
            UpdateWindowSize(this.Width * dpiVector.X, this.Height * dpiVector.Y);

            // update graphics and text based on the current DPI of the monitor
            UpdateLayoutTransform();
        }

        /// <summary>
        /// Raises the <see cref="E:DpiChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDpiChanged(EventArgs e)
        {
            var handler = this.DpiChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    /// <summary>
    /// The base implementation of a command.
    /// </summary>
    public abstract class CommandBase
        : ICommand
    {
        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { System.Windows.Input.CommandManager.RequerySuggested += value; }
            remove { System.Windows.Input.CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        public void OnCanExecuteChanged()
        {
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }
            OnExecute(parameter);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected abstract void OnExecute(object parameter);
    }

    /// <summary>
    /// The command that relays its functionality by invoking delegates.
    /// </summary>
    public class RelayCommand
        : CommandBase
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        /// <param name="canExecute">The can execute.</param>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            if (canExecute == null)
            {
                // no can execute provided, then always executable
                canExecute = (o) => true;
            }
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public override bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void OnExecute(object parameter)
        {
            execute(parameter);
        }
    }

    /// <summary>
    /// Represents a Modern UI styled dialog window.
    /// </summary>
    public class ModernDialog
        : DpiAwareWindow
    {
        /// <summary>
        /// Identifies the BackgroundContent dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundContentProperty = DependencyProperty.Register("BackgroundContent", typeof(object), typeof(ModernDialog));
        /// <summary>
        /// Identifies the Buttons dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register("Buttons", typeof(IEnumerable<Button>), typeof(ModernDialog));

        private ICommand closeCommand;

        private Button okButton;
        private Button cancelButton;
        private Button yesButton;
        private Button noButton;
        private Button closeButton;

        private MessageBoxResult messageBoxResult = MessageBoxResult.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModernDialog"/> class.
        /// </summary>
        public ModernDialog()
        {
            this.DefaultStyleKey = typeof(ModernDialog);
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            this.closeCommand = new RelayCommand(o => {
                var result = o as MessageBoxResult?;
                if (result.HasValue)
                {
                    this.messageBoxResult = result.Value;

                    // sets the Window.DialogResult as well
                    if (result.Value == MessageBoxResult.OK || result.Value == MessageBoxResult.Yes)
                    {
                        this.DialogResult = true;
                    }
                    else if (result.Value == MessageBoxResult.Cancel || result.Value == MessageBoxResult.No)
                    {
                        this.DialogResult = false;
                    }
                    else
                    {
                        this.DialogResult = null;
                    }
                }
                Close();
            });

            this.Buttons = new Button[] { this.CloseButton };

            // set the default owner to the app main window (if possible)
            if (Application.Current != null && Application.Current.MainWindow != this)
            {
                this.Owner = Application.Current.MainWindow;
            }
        }

        private Button CreateCloseDialogButton(string content, bool isDefault, bool isCancel, MessageBoxResult result)
        {
            return new Button
            {
                Content = content,
                Command = this.CloseCommand,
                CommandParameter = result,
                IsDefault = isDefault,
                IsCancel = isCancel,
                MinHeight = 21,
                MinWidth = 65,
                Margin = new Thickness(4, 0, 0, 0)
            };
        }

        /// <summary>
        /// Gets the close window command.
        /// </summary>
        public ICommand CloseCommand
        {
            get { return this.closeCommand; }
        }

        /// <summary>
        /// Gets the Ok button.
        /// </summary>
        public Button OkButton
        {
            get
            {
                if (this.okButton == null)
                {
                    this.okButton = CreateCloseDialogButton("OK", true, false, MessageBoxResult.OK);
                }
                return this.okButton;
            }
        }

        /// <summary>
        /// Gets the Cancel button.
        /// </summary>
        public Button CancelButton
        {
            get
            {
                if (this.cancelButton == null)
                {
                    this.cancelButton = CreateCloseDialogButton("Cancel", false, true, MessageBoxResult.Cancel);
                }
                return this.cancelButton;
            }
        }

        /// <summary>
        /// Gets the Yes button.
        /// </summary>
        public Button YesButton
        {
            get
            {
                if (this.yesButton == null)
                {
                    this.yesButton = CreateCloseDialogButton("Yes", true, false, MessageBoxResult.Yes);
                }
                return this.yesButton;
            }
        }

        /// <summary>
        /// Gets the No button.
        /// </summary>
        public Button NoButton
        {
            get
            {
                if (this.noButton == null)
                {
                    this.noButton = CreateCloseDialogButton("No", false, true, MessageBoxResult.No);
                }
                return this.noButton;
            }
        }

        /// <summary>
        /// Gets the Close button.
        /// </summary>
        public Button CloseButton
        {
            get
            {
                if (this.closeButton == null)
                {
                    this.closeButton = CreateCloseDialogButton("Close", true, false, MessageBoxResult.None);
                }
                return this.closeButton;
            }
        }

        /// <summary>
        /// Gets or sets the background content of this window instance.
        /// </summary>
        public object BackgroundContent
        {
            get { return GetValue(BackgroundContentProperty); }
            set { SetValue(BackgroundContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the dialog buttons.
        /// </summary>
        public IEnumerable<Button> Buttons
        {
            get { return (IEnumerable<Button>)GetValue(ButtonsProperty); }
            set { SetValue(ButtonsProperty, value); }
        }

        /// <summary>
        /// Gets the message box result.
        /// </summary>
        /// <value>
        /// The message box result.
        /// </value>
        public MessageBoxResult MessageBoxResult
        {
            get { return this.messageBoxResult; }
        }

        /// <summary>
        /// Displays a messagebox.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="title">The title.</param>
        /// <param name="button">The button.</param>
        /// <param name="owner">The window owning the messagebox. The messagebox will be located at the center of the owner.</param>
        /// <returns></returns>
        public static MessageBoxResult ShowMessage(string text, string title, MessageBoxButton button, Window owner = null)
        {
            var dlg = new ModernDialog
            {
                Title = title,
                Content = text,
                MinHeight = 0,
                MinWidth = 0,
                MaxHeight = 480,
                MaxWidth = 640,
            };
            if (owner != null)
            {
                dlg.Owner = owner;
            }

            dlg.Buttons = GetButtons(dlg, button);
            dlg.ShowDialog();
            return dlg.messageBoxResult;
        }

        private static IEnumerable<Button> GetButtons(ModernDialog owner, MessageBoxButton button)
        {
            if (button == MessageBoxButton.OK)
            {
                yield return owner.OkButton;
            }
            else if (button == MessageBoxButton.OKCancel)
            {
                yield return owner.OkButton;
                yield return owner.CancelButton;
            }
            else if (button == MessageBoxButton.YesNo)
            {
                yield return owner.YesButton;
                yield return owner.NoButton;
            }
            else if (button == MessageBoxButton.YesNoCancel)
            {
                yield return owner.YesButton;
                yield return owner.NoButton;
                yield return owner.CancelButton;
            }
        }
    }

    /// <summary>
    /// Represents a control that indicates that an operation is ongoing. 
    /// </summary>
    [TemplateVisualState(GroupName = GroupActiveStates, Name = StateInactive)]
    [TemplateVisualState(GroupName = GroupActiveStates, Name = StateActive)]
    public class ModernProgressRing
        : Control
    {
        private const string GroupActiveStates = "ActiveStates";
        private const string StateInactive = "Inactive";
        private const string StateActive = "Active";

        /// <summary>
        /// Identifies the IsActive property.
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(ModernProgressRing), new PropertyMetadata(false, OnIsActiveChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="ModernProgressRing"/> class.
        /// </summary>
        public ModernProgressRing()
        {
            this.DefaultStyleKey = typeof(ModernProgressRing);
        }

        private void GotoCurrentState(bool animate)
        {
            var state = this.IsActive ? StateActive : StateInactive;

            VisualStateManager.GoToState(this, state, animate);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            GotoCurrentState(false);
        }

        private static void OnIsActiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ModernProgressRing)o).GotoCurrentState(true);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the <see cref="ModernProgressRing"/> is showing progress.
        /// </summary>
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }
    }

    /// <summary>
    /// The platform does not currently support relative sized translation values. 
    /// This primitive control walks through visual state animation storyboards
    /// and looks for identifying values to use as percentages.
    /// </summary>
    public class RelativeAnimatingContentControl : ContentControl
    {
        /// <summary>
        /// A simple Epsilon-style value used for trying to determine if a double
        /// has an identifying value. 
        /// </summary>
        private const double SimpleDoubleComparisonEpsilon = 0.000009;

        /// <summary>
        /// The last known width of the control.
        /// </summary>
        private double _knownWidth;

        /// <summary>
        /// The last known height of the control.
        /// </summary>
        private double _knownHeight;

        /// <summary>
        /// A set of custom animation adapters used to update the animation
        /// storyboards when the size of the control changes.
        /// </summary>
        private List<AnimationValueAdapter> _specialAnimations;

        /// <summary>
        /// Initializes a new instance of the RelativeAnimatingContentControl
        /// type.
        /// </summary>
        public RelativeAnimatingContentControl()
        {
            SizeChanged += OnSizeChanged;
        }

        /// <summary>
        /// Handles the size changed event.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e != null && e.NewSize.Height > 0 && e.NewSize.Width > 0)
            {
                _knownWidth = e.NewSize.Width;
                _knownHeight = e.NewSize.Height;

                UpdateAnyAnimationValues();
            }
        }

        /// <summary>
        /// Walks through the known storyboards in the control's template that
        /// may contain identifying values, storing them for future
        /// use and updates.
        /// </summary>
        private void UpdateAnyAnimationValues()
        {
            if (_knownHeight > 0 && _knownWidth > 0)
            {
                // Initially, before any special animations have been found,
                // the visual state groups of the control must be explored. 
                // By definition they must be at the implementation root of the
                // control.
                if (_specialAnimations == null)
                {
                    _specialAnimations = new List<AnimationValueAdapter>();

                    foreach (VisualStateGroup group in VisualStateManager.GetVisualStateGroups(this))
                    {
                        if (group == null)
                        {
                            continue;
                        }
                        foreach (VisualState state in group.States)
                        {
                            if (state != null)
                            {
                                Storyboard sb = state.Storyboard;

                                if (sb != null)
                                {
                                    // Examine all children of the storyboards,
                                    // looking for either type of double
                                    // animation.
                                    foreach (Timeline timeline in sb.Children)
                                    {
                                        DoubleAnimation da = timeline as DoubleAnimation;
                                        DoubleAnimationUsingKeyFrames dakeys = timeline as DoubleAnimationUsingKeyFrames;
                                        if (da != null)
                                        {
                                            ProcessDoubleAnimation(da);
                                        }
                                        else if (dakeys != null)
                                        {
                                            ProcessDoubleAnimationWithKeys(dakeys);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Update special animation values relative to the current size.
                UpdateKnownAnimations();

                // HACK: force storyboard to use new values
                foreach (VisualStateGroup group in VisualStateManager.GetVisualStateGroups(this))
                {
                    if (group == null)
                    {
                        continue;
                    }
                    foreach (VisualState state in group.States)
                    {
                        if (state != null)
                        {
                            Storyboard sb = state.Storyboard;

                            if (sb != null)
                            {
                                // need to kick the storyboard, otherwise new values are not taken into account.
                                // it's sad, really don't want to start storyboards in vsm, but I see no other option
                                sb.Begin(this);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Walks through all special animations, updating based on the current
        /// size of the control.
        /// </summary>
        private void UpdateKnownAnimations()
        {
            foreach (AnimationValueAdapter adapter in _specialAnimations)
            {
                adapter.UpdateWithNewDimension(_knownWidth, _knownHeight);
            }
        }

        /// <summary>
        /// Processes a double animation with keyframes, looking for known 
        /// special values to store with an adapter.
        /// </summary>
        /// <param name="da">The double animation using key frames instance.</param>
        private void ProcessDoubleAnimationWithKeys(DoubleAnimationUsingKeyFrames da)
        {
            // Look through all keyframes in the instance.
            foreach (DoubleKeyFrame frame in da.KeyFrames)
            {
                var d = DoubleAnimationFrameAdapter.GetDimensionFromIdentifyingValue(frame.Value);
                if (d.HasValue)
                {
                    _specialAnimations.Add(new DoubleAnimationFrameAdapter(d.Value, frame));
                }
            }
        }

        /// <summary>
        /// Processes a double animation looking for special values.
        /// </summary>
        /// <param name="da">The double animation instance.</param>
        private void ProcessDoubleAnimation(DoubleAnimation da)
        {
            // Look for a special value in the To property.
            if (da.To.HasValue)
            {
                var d = DoubleAnimationToAdapter.GetDimensionFromIdentifyingValue(da.To.Value);
                if (d.HasValue)
                {
                    _specialAnimations.Add(new DoubleAnimationToAdapter(d.Value, da));
                }
            }

            // Look for a special value in the From property.
            if (da.From.HasValue)
            {
                var d = DoubleAnimationFromAdapter.GetDimensionFromIdentifyingValue(da.To.Value);
                if (d.HasValue)
                {
                    _specialAnimations.Add(new DoubleAnimationFromAdapter(d.Value, da));
                }
            }
        }

        #region Private animation updating system
        /// <summary>
        /// A selection of dimensions of interest for updating an animation.
        /// </summary>
        private enum DoubleAnimationDimension
        {
            /// <summary>
            /// The width (horizontal) dimension.
            /// </summary>
            Width,

            /// <summary>
            /// The height (vertical) dimension.
            /// </summary>
            Height,
        }

        /// <summary>
        /// A simple class designed to store information about a specific 
        /// animation instance and its properties. Able to update the values at
        /// runtime.
        /// </summary>
        private abstract class AnimationValueAdapter
        {
            /// <summary>
            /// Gets or sets the original double value.
            /// </summary>
            protected double OriginalValue { get; set; }

            /// <summary>
            /// Initializes a new instance of the AnimationValueAdapter type.
            /// </summary>
            /// <param name="dimension">The dimension of interest for updates.</param>
            public AnimationValueAdapter(DoubleAnimationDimension dimension)
            {
                Dimension = dimension;
            }

            /// <summary>
            /// Gets the dimension of interest for the control.
            /// </summary>
            public DoubleAnimationDimension Dimension { get; private set; }

            /// <summary>
            /// Updates the original instance based on new dimension information
            /// from the control. Takes both and allows the subclass to make the
            /// decision on which ratio, values, and dimension to use.
            /// </summary>
            /// <param name="width">The width of the control.</param>
            /// <param name="height">The height of the control.</param>
            public abstract void UpdateWithNewDimension(double width, double height);
        }

        private abstract class GeneralAnimationValueAdapter<T> : AnimationValueAdapter
        {
            /// <summary>
            /// Stores the animation instance.
            /// </summary>
            protected T Instance { get; set; }

            /// <summary>
            /// Gets the value of the underlying property of interest.
            /// </summary>
            /// <returns>Returns the value of the property.</returns>
            protected abstract double GetValue();

            /// <summary>
            /// Sets the value for the underlying property of interest.
            /// </summary>
            /// <param name="newValue">The new value for the property.</param>
            protected abstract void SetValue(double newValue);

            /// <summary>
            /// Gets the initial value (minus the identifying value portion) that the
            /// designer stored within the visual state animation property.
            /// </summary>
            protected double InitialValue { get; private set; }

            /// <summary>
            /// The ratio based on the original identifying value, used for computing
            /// the updated animation property of interest when the size of the
            /// control changes.
            /// </summary>
            private double _ratio;

            /// <summary>
            /// Initializes a new instance of the GeneralAnimationValueAdapter
            /// type.
            /// </summary>
            /// <param name="d">The dimension of interest.</param>
            /// <param name="instance">The animation type instance.</param>
            public GeneralAnimationValueAdapter(DoubleAnimationDimension d, T instance)
                : base(d)
            {
                Instance = instance;

                InitialValue = StripIdentifyingValueOff(GetValue());
                _ratio = InitialValue / 100;
            }

            /// <summary>
            /// Approximately removes the identifying value from a value.
            /// </summary>
            /// <param name="number">The initial number.</param>
            /// <returns>Returns a double with an adjustment for the identifying
            /// value portion of the number.</returns>
            public double StripIdentifyingValueOff(double number)
            {
                return Dimension == DoubleAnimationDimension.Width ? number - .1 : number - .2;
            }

            /// <summary>
            /// Retrieves the dimension, if any, from the number. If the number
            /// does not have an identifying value, null is returned.
            /// </summary>
            /// <param name="number">The double value.</param>
            /// <returns>Returns a double animation dimension if the number was
            /// contained an identifying value; otherwise, returns null.</returns>
            public static DoubleAnimationDimension? GetDimensionFromIdentifyingValue(double number)
            {
                double floor = Math.Floor(number);
                double remainder = number - floor;

                if (remainder >= .1 - SimpleDoubleComparisonEpsilon && remainder <= .1 + SimpleDoubleComparisonEpsilon)
                {
                    return DoubleAnimationDimension.Width;
                }
                if (remainder >= .2 - SimpleDoubleComparisonEpsilon && remainder <= .2 + SimpleDoubleComparisonEpsilon)
                {
                    return DoubleAnimationDimension.Height;
                }
                return null;
            }

            /// <summary>
            /// Updates the animation instance based on the dimensions of the
            /// control.
            /// </summary>
            /// <param name="width">The width of the control.</param>
            /// <param name="height">The height of the control.</param>
            public override void UpdateWithNewDimension(double width, double height)
            {
                double size = Dimension == DoubleAnimationDimension.Width ? width : height;
                UpdateValue(size);
            }

            /// <summary>
            /// Updates the value of the property.
            /// </summary>
            /// <param name="sizeToUse">The size of interest to use with a ratio
            /// computation.</param>
            private void UpdateValue(double sizeToUse)
            {
                SetValue(sizeToUse * _ratio);
            }
        }

        /// <summary>
        /// Adapter for DoubleAnimation's To property.
        /// </summary>
        private class DoubleAnimationToAdapter : GeneralAnimationValueAdapter<DoubleAnimation>
        {
            /// <summary>
            /// Gets the value of the underlying property of interest.
            /// </summary>
            /// <returns>Returns the value of the property.</returns>
            protected override double GetValue()
            {
                return (double)Instance.To;
            }

            /// <summary>
            /// Sets the value for the underlying property of interest.
            /// </summary>
            /// <param name="newValue">The new value for the property.</param>
            protected override void SetValue(double newValue)
            {
                Instance.To = newValue;
            }

            /// <summary>
            /// Initializes a new instance of the DoubleAnimationToAdapter type.
            /// </summary>
            /// <param name="dimension">The dimension of interest.</param>
            /// <param name="instance">The instance of the animation type.</param>
            public DoubleAnimationToAdapter(DoubleAnimationDimension dimension, DoubleAnimation instance)
                : base(dimension, instance)
            {
            }
        }

        /// <summary>
        /// Adapter for DoubleAnimation's From property.
        /// </summary>
        private class DoubleAnimationFromAdapter : GeneralAnimationValueAdapter<DoubleAnimation>
        {
            /// <summary>
            /// Gets the value of the underlying property of interest.
            /// </summary>
            /// <returns>Returns the value of the property.</returns>
            protected override double GetValue()
            {
                return (double)Instance.From;
            }

            /// <summary>
            /// Sets the value for the underlying property of interest.
            /// </summary>
            /// <param name="newValue">The new value for the property.</param>
            protected override void SetValue(double newValue)
            {
                Instance.From = newValue;
            }

            /// <summary>
            /// Initializes a new instance of the DoubleAnimationFromAdapter 
            /// type.
            /// </summary>
            /// <param name="dimension">The dimension of interest.</param>
            /// <param name="instance">The instance of the animation type.</param>
            public DoubleAnimationFromAdapter(DoubleAnimationDimension dimension, DoubleAnimation instance)
                : base(dimension, instance)
            {
            }
        }

        /// <summary>
        /// Adapter for double key frames.
        /// </summary>
        private class DoubleAnimationFrameAdapter : GeneralAnimationValueAdapter<DoubleKeyFrame>
        {
            /// <summary>
            /// Gets the value of the underlying property of interest.
            /// </summary>
            /// <returns>Returns the value of the property.</returns>
            protected override double GetValue()
            {
                return Instance.Value;
            }

            /// <summary>
            /// Sets the value for the underlying property of interest.
            /// </summary>
            /// <param name="newValue">The new value for the property.</param>
            protected override void SetValue(double newValue)
            {
                Instance.Value = newValue;
            }

            /// <summary>
            /// Initializes a new instance of the DoubleAnimationFrameAdapter
            /// type.
            /// </summary>
            /// <param name="dimension">The dimension of interest.</param>
            /// <param name="frame">The instance of the animation type.</param>
            public DoubleAnimationFrameAdapter(DoubleAnimationDimension dimension, DoubleKeyFrame frame)
                : base(dimension, frame)
            {
            }
        }
        #endregion
    }
}

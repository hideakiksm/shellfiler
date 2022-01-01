using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;

using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace ShellFiler.Util {
  /*
   * http://msdn.microsoft.com/en-us/library/bb774950%28VS.85%29.aspx
   * http://smdn.jp/programming/tips/createlnk/
   */
  public class Win32ShellLink : IDisposable {
    public string CurrentFile {
      get
      {
        string file;

        PersistFile.GetCurFile(out file);

        return file;
      }
    }

    public string TargetPath {
      get
      {
        CheckDisposed();

        var targetPath = CreatePathStringBuffer();

        if (shellLinkW != null) {
          var data = new WIN32_FIND_DATAW();

          shellLinkW.GetPath(targetPath, targetPath.Capacity, ref data, SLGP_FLAGS.UNCPRIORITY);
        }
        else {
          var data = new WIN32_FIND_DATAA();

          shellLinkA.GetPath(targetPath, targetPath.Capacity, ref data, SLGP_FLAGS.UNCPRIORITY);
        }
        
        return targetPath.ToString();
      }
      set
      {
        CheckDisposed();

        if (shellLinkW != null)
          shellLinkW.SetPath(value);
        else
          shellLinkA.SetPath(value);
      }
    }

    public string WorkingDirectory {
      get
      {
        CheckDisposed();

        var workingDirectory = CreatePathStringBuffer();

        if (shellLinkW != null)
          shellLinkW.GetWorkingDirectory(workingDirectory, workingDirectory.Capacity);
        else
          shellLinkA.GetWorkingDirectory(workingDirectory, workingDirectory.Capacity);

        return workingDirectory.ToString();
      }
      set
      {
        CheckDisposed();

        if (shellLinkW != null)
          shellLinkW.SetWorkingDirectory(value);
        else
          shellLinkA.SetWorkingDirectory(value);
      }
    }

    public string Arguments {
      get
      {
        CheckDisposed();

        var arguments = CreatePathStringBuffer();

        if (shellLinkW != null)
          shellLinkW.GetArguments(arguments, arguments.Capacity);
        else
          shellLinkA.GetArguments(arguments, arguments.Capacity);

        return arguments.ToString();
      }
      set
      {
        CheckDisposed();

        if (shellLinkW != null)
          shellLinkW.SetArguments(value);
        else
          shellLinkA.SetArguments(value);
      }
    }

    public string Description {
      get
      {
        CheckDisposed();

        var description = CreatePathStringBuffer();

        if (shellLinkW != null)
          shellLinkW.GetDescription(description, description.Capacity);
        else
          shellLinkA.GetDescription(description, description.Capacity);

        return description.ToString();
      }
      set
      {
        CheckDisposed();

        if (shellLinkW != null)
          shellLinkW.SetDescription(value);
        else
          shellLinkA.SetDescription(value);
      }
    }

    public IconLocation IconLocation {
      get
      {
        CheckDisposed();

        var iconFileBuffer = CreatePathStringBuffer();
        int iconIndex;

        if (shellLinkW != null)
          shellLinkW.GetIconLocation(iconFileBuffer, iconFileBuffer.Capacity, out iconIndex);
        else
          shellLinkA.GetIconLocation(iconFileBuffer, iconFileBuffer.Capacity, out iconIndex);

        return new IconLocation(iconFileBuffer.ToString(), iconIndex);
      }
      set
      {
        CheckDisposed();

        if (shellLinkW != null)
          shellLinkW.SetIconLocation(value.File, value.Index);
        else
          shellLinkA.SetIconLocation(value.File, value.Index);
      }
    }

    public SW ShowCommand {
      get
      {
        CheckDisposed();

        SW showCmd;

        if (shellLinkW != null)
          shellLinkW.GetShowCmd(out showCmd);
        else
          shellLinkA.GetShowCmd(out showCmd);

        return showCmd;
      }
      set
      {
        CheckDisposed();

        if (shellLinkW != null)
          shellLinkW.SetShowCmd(value);
        else
          shellLinkA.SetShowCmd(value);
      }
    }

    public Keys HotKey {
      get
      {
        CheckDisposed();

        ushort hotKey;

        if (shellLinkW != null)
          shellLinkW.GetHotkey(out hotKey);
        else
          shellLinkA.GetHotkey(out hotKey);

        return TranslateKeyCode(hotKey);
      }
      set
      {
        CheckDisposed();

        var newHotKey = TranslateKeyCode(value);

        if (shellLinkW != null)
          shellLinkW.SetHotkey(newHotKey);
        else
          shellLinkA.SetHotkey(newHotKey);
      }
    }

    private IPersistFile PersistFile {
      get
      {
        CheckDisposed();

        var ret = (shellLinkW != null)
          ? shellLinkW as IPersistFile
          : shellLinkA as IPersistFile;

        if (ret == null)
          throw new COMException("cannot create IPersistFile");
        else
          return ret;
      }
    }

    public Win32ShellLink()
      : this(null)
    {
    }

    public Win32ShellLink(string linkFile)
    {
      try {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
          shellLinkW = (IShellLinkW)new ShellLinkObject();
          shellLinkA = null;
        }
        else {
          shellLinkA = (IShellLinkA)new ShellLinkObject();
          shellLinkW = null;
        }
      }
      catch {
        throw new COMException("cannot create ShellLinkObject");
      }

      if (linkFile != null)
        Load(linkFile);
    }

    ~Win32ShellLink()
    {
      Dispose(false);
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (shellLinkW != null) {
        Marshal.ReleaseComObject(shellLinkW);
        shellLinkW = null;
      }

      if (shellLinkA != null) {
        Marshal.ReleaseComObject(shellLinkA);
        shellLinkA = null;
      }
    }

    public void Save()
    {
      var file = CurrentFile;

      if (file == null)
        throw new InvalidOperationException("file name must be specified");

      Save(file);
    }

    public void Save(string file)
    {
      CheckDisposed();

      if (file == null)
        throw new ArgumentNullException("file");

      PersistFile.Save(file, true);
    }

    public void Load(string file)
    {
      Load(file, IntPtr.Zero, SLR_FLAGS.ANY_MATCH | SLR_FLAGS.NO_UI, 1);
    }

    public void Load(string file, IntPtr hWnd, SLR_FLAGS flags)
    {
      Load(file, hWnd, flags, 1);
    }

    public void Load(string file, IntPtr hWnd, SLR_FLAGS flags, TimeSpan timeOut)
    {
      Load(file, hWnd, flags, (int)timeOut.TotalMilliseconds);
    }

    public void Load(string file, IntPtr hWnd, SLR_FLAGS flags, int timeoutMilliseconds)
    {
      CheckDisposed();

      if (!File.Exists(file))
        throw new FileNotFoundException("file not found", file);

      PersistFile.Load(file, 0x00000000);

      if ((int)(flags & SLR_FLAGS.NO_UI) != 0)
        flags |= (SLR_FLAGS)(timeoutMilliseconds << 16);

      if (shellLinkW != null)
        shellLinkW.Resolve(hWnd, flags);
      else
        shellLinkA.Resolve(hWnd, flags);
    }

    private static StringBuilder CreatePathStringBuffer()
    {
      return new StringBuilder(Consts.MAX_PATH, Consts.MAX_PATH);
    }

    private static ushort TranslateKeyCode(Keys key)
    {
      // IShellLink::SetHotkey Method
      //   wHotkey
      //     The new keyboard shortcut. The virtual key code is in the low-order byte, and the modifier flags are in the high-order byte.
      //     The modifier flags can be a combination of the values specified in the description of the IShellLink::GetHotkey method.
      var virtKey  = ((int)(key & Keys.KeyCode) & 0x00ff);
      var modifier = (((int)(key & Keys.Modifiers) >> 8) & 0xff00);

      return (ushort)(virtKey | modifier);
    }

    private static Keys TranslateKeyCode(ushort key)
    {
      // IShellLink::GetHotkey Method
      //   pwHotkey
      //     The address of the keyboard shortcut. The virtual key code is in the low-order byte, 
      //     and the modifier flags are in the high-order byte. The modifier flags can be a combination of the following values.
      var virtKey = (Keys)(key & 0x00ff);
      var modifier = (Keys)((key & 0xff00) << 8);

      return virtKey | modifier;
    }

    private void CheckDisposed()
    {
      if (shellLinkW == null && shellLinkA == null)
        throw new ObjectDisposedException(GetType().FullName);
    }

    private IShellLinkW shellLinkW = null;
    private IShellLinkA shellLinkA = null;
  }

  // ShellLink コクラス
  [ComImport, ClassInterface(ClassInterfaceType.None), Guid("00021401-0000-0000-C000-000000000046")]
  public class ShellLinkObject {}

  // IShellLinkWインターフェイス
  [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214F9-0000-0000-C000-000000000046")]
  public interface IShellLinkW { // cannot list any base interfaces here
    //HRESULT GetPath([out, size_is(cch)] LPWSTR pszFile, [in] int cch, [in, out, ptr] WIN32_FIND_DATAW *pfd, [in] DWORD fFlags);
    void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cch, ref WIN32_FIND_DATAW pfd, SLGP_FLAGS fFlags);

    //HRESULT GetIDList([out] LPITEMIDLIST * ppidl);
    void GetIDList(out IntPtr ppidl);

    //HRESULT SetIDList([in] LPCITEMIDLIST pidl);
    void SetIDList(IntPtr pidl);

    //HRESULT GetDescription([out, size_is(cch)] LPWSTR pszName, int cch);
    void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cch);

    //HRESULT SetDescription([in] LPCWSTR pszName);
    void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

    //HRESULT GetWorkingDirectory([out, size_is(cch)] LPWSTR pszDir, int cch);
    void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cch);

    //HRESULT SetWorkingDirectory([in] LPCWSTR pszDir);
    void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

    //HRESULT GetArguments([out, size_is(cch)] LPWSTR pszArgs, int cch);
    void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cch);

    //HRESULT SetArguments([in] LPCWSTR pszArgs);
    void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

    //HRESULT GetHotkey([out] WORD *pwHotkey);
    void GetHotkey(out ushort pwHotkey);

    //HRESULT SetHotkey([in] WORD wHotkey);
    void SetHotkey(ushort wHotkey);

    //HRESULT GetShowCmd([out] int *piShowCmd);
    void GetShowCmd(out SW piShowCmd);

    //HRESULT SetShowCmd([in] int iShowCmd);
    void SetShowCmd(SW iShowCmd);

    //HRESULT GetIconLocation([out, size_is(cch)] LPWSTR pszIconPath, [in] int cch, [out] int *piIcon);
    void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cch, out int piIcon);

    //HRESULT SetIconLocation([in] LPCWSTR pszIconPath, [in] int iIcon);
    void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

    //HRESULT SetRelativePath([in] LPCWSTR pszPathRel, [in] DWORD dwReserved);
    void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);

    //HRESULT Resolve([in] HWND hwnd, [in] DWORD fFlags);
    void Resolve(IntPtr hwnd, SLR_FLAGS fFlags);

    //HRESULT SetPath([in] LPCWSTR pszFile);
    void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
  }

  // IShellLinkAインターフェイス
  [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214EE-0000-0000-C000-000000000046")]
  public interface IShellLinkA { // cannot list any base interfaces here
    //HRESULT GetPath([out, size_is(cch)] LPSTR pszFile, [in] int cch, [in, out, ptr] WIN32_FIND_DATAW *pfd, [in] DWORD fFlags);
    void GetPath([Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszFile, int cch, ref WIN32_FIND_DATAA pfd, SLGP_FLAGS fFlags);

    //HRESULT GetIDList([out] LPITEMIDLIST * ppidl);
    void GetIDList(out IntPtr ppidl);

    //HRESULT SetIDList([in] LPCITEMIDLIST pidl);
    void SetIDList(IntPtr pidl);

    //HRESULT GetDescription([out, size_is(cch)] LPSTR pszName, int cch);
    void GetDescription([Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszName, int cch);

    //HRESULT SetDescription([in] LPCSTR pszName);
    void SetDescription([MarshalAs(UnmanagedType.LPStr)] string pszName);

    //HRESULT GetWorkingDirectory([out, size_is(cch)] LPSTR pszDir, int cch);
    void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszDir, int cch);

    //HRESULT SetWorkingDirectory([in] LPCSTR pszDir);
    void SetWorkingDirectory([MarshalAs(UnmanagedType.LPStr)] string pszDir);

    //HRESULT GetArguments([out, size_is(cch)] LPSTR pszArgs, int cch);
    void GetArguments([Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszArgs, int cch);

    //HRESULT SetArguments([in] LPCSTR pszArgs);
    void SetArguments([MarshalAs(UnmanagedType.LPStr)] string pszArgs);

    //HRESULT GetHotkey([out] WORD *pwHotkey);
    void GetHotkey(out ushort pwHotkey);

    //HRESULT SetHotkey([in] WORD wHotkey);
    void SetHotkey(ushort wHotkey);

    //HRESULT GetShowCmd([out] int *piShowCmd);
    void GetShowCmd(out SW piShowCmd);

    //HRESULT SetShowCmd([in] int iShowCmd);
    void SetShowCmd(SW iShowCmd);

    //HRESULT GetIconLocation([out, size_is(cch)] LPSTR pszIconPath, [in] int cch, [out] int *piIcon);
    void GetIconLocation([Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder pszIconPath, int cch, out int piIcon);

    //HRESULT SetIconLocation([in] LPCSTR pszIconPath, [in] int iIcon);
    void SetIconLocation([MarshalAs(UnmanagedType.LPStr)] string pszIconPath, int iIcon);

    //HRESULT SetRelativePath([in] LPCSTR pszPathRel, [in] DWORD dwReserved);
    void SetRelativePath([MarshalAs(UnmanagedType.LPStr)] string pszPathRel, uint dwReserved);

    //HRESULT Resolve([in] HWND hwnd, [in] DWORD fFlags);
    void Resolve(IntPtr hwnd, SLR_FLAGS fFlags);

    //HRESULT SetPath([in] LPCSTR pszFile);
    void SetPath([MarshalAs(UnmanagedType.LPStr)] string pszFile);
  }

  public partial class Consts {
    public const int MAX_PATH = 260;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
  public struct WIN32_FIND_DATAW {
    public uint dwFileAttributes;
    public ComTypes.FILETIME ftCreationTime;
    public ComTypes.FILETIME ftLastAccessTime;
    public ComTypes.FILETIME ftLastWriteTime;
    public uint nFileSizeHigh;
    public uint nFileSizeLow;
    public uint dwReserved0;
    public uint dwReserved1;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Consts.MAX_PATH)] public string cFileName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)] public string cAlternateFileName;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
  public struct WIN32_FIND_DATAA {
    public uint dwFileAttributes;
    public ComTypes.FILETIME ftCreationTime;
    public ComTypes.FILETIME ftLastAccessTime;
    public ComTypes.FILETIME ftLastWriteTime;
    public uint nFileSizeHigh;
    public uint nFileSizeLow;
    public uint dwReserved0;
    public uint dwReserved1;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Consts.MAX_PATH)] public string cFileName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)] public string cAlternateFileName;
  }

  // SW_XXX
  public enum SW : int {
    HIDE            = 0,
    NORMAL          = 1,
    SHOWNORMAL      = 1,
    SHOWMINIMIZED   = 2,
    MAXIMIZE        = 3,
    SHOWMAXIMIZED   = 3,
    SHOWNOACTIVATE  = 4,
    SHOW            = 5,
    MINIMIZE        = 6,
    SHOWMINNOACTIVE = 7,
    SHOWNA          = 8,
    RESTORE         = 9,
    SHOWDEFAULT     = 10,
    FORCEMINIMIZE   = 11,
  }

  [Flags]
  public enum SLGP_FLAGS : uint {
    SHORTPATH   = 1,
    UNCPRIORITY = 2,
    RAWPATH     = 4,
  }

  /// <summary>Flags determining how the links with missing targets are resolved.</summary>
  [Flags]
  public enum SLR_FLAGS : uint {
    /// <summary>
    /// Do not display a dialog box if the link cannot be resolved. 
    /// When SLR_NO_UI is set, a time-out value that specifies the 
    /// maximum amount of time to be spent resolving the link can 
    /// be specified in milliseconds. The function returns if the 
    /// link cannot be resolved within the time-out duration. 
    /// If the timeout is not set, the time-out duration will be 
    /// set to the default value of 3,000 milliseconds (3 seconds). 
    /// </summary>
    NO_UI       = 1,
    /// <summary>
    /// Allow any match during resolution.  Has no effect
    /// on ME/2000 or above, use the other flags instead.
    /// </summary>
    ANY_MATCH   = 2,
    /// <summary>
    /// If the link object has changed, update its path and list 
    /// of identifiers. If SLR_UPDATE is set, you do not need to 
    /// call IPersistFile::IsDirty to determine whether or not 
    /// the link object has changed. 
    /// </summary>
    UPDATE      = 4,
    /// <summary>Do not update the link information.</summary>
    NOUPDATE    = 8,
    /// <summary>Do not execute the search heuristics.</summary>
    NOSEARCH    = 16,
    /// <summary>Do not use distributed link tracking.</summary>
    NOTRACK     = 32,
    /// <summary>
    /// Disable distributed link tracking. By default, 
    /// distributed link tracking tracks removable media 
    /// across multiple devices based on the volume name. 
    /// It also uses the UNC path to track remote file 
    /// systems whose drive letter has changed. Setting 
    /// SLR_NOLINKINFO disables both types of tracking.
    /// </summary>
    NOLINKINFO  = 64,
    /// <summary>Call the Microsoft Windows Installer.</summary>
    INVOKE_MSI  = 128,

    /// <summary>
    /// Not documented in SDK.  Assume same as SLR_NO_UI but 
    /// intended for applications without a hWnd.
    /// </summary>
    UI_WITH_MSG_PUMP = 0x101,
  }

  public struct IconLocation {
    public static readonly IconLocation Empty = new IconLocation();

    public string File {
      get { return file; }
      set { file = value; }
    }

    public int Index {
      get { return index; }
      set { index = CheckIndex(value); }
    }

    public IconLocation(string file, int index)
      : this()
    {
      this.file = file;
      this.index = CheckIndex(index);
    }

    private int CheckIndex(int val)
    {
      if (val < 0)
        throw new ArgumentOutOfRangeException("val", "must be zero or positive number");

      return val;
    }

    private string file;
    private int index;
  }
}

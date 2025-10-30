using System;
using System.Runtime.InteropServices;

namespace MonoMod.Core.Interop
{
    internal static class Unix
    {
        // On Android, we need to use the fully qualified library name
        // On other Linux systems, "libc" works fine
        public const string LibC = "c";


        [DllImport(LibC, CallingConvention = CallingConvention.Cdecl, EntryPoint = "read")]
        public static extern unsafe nint Read(int fd, IntPtr buf, nint count);

        [DllImport(LibC, CallingConvention = CallingConvention.Cdecl, EntryPoint = "write")]
        public static extern unsafe nint Write(int fd, IntPtr buf, nint count);

        [DllImport(LibC, CallingConvention = CallingConvention.Cdecl, EntryPoint = "pipe2")]
        public static extern unsafe int Pipe2(int* pipefd, PipeFlags flags);

        [DllImport(LibC, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mmap")]
        public static extern unsafe nint Mmap(IntPtr addr, nuint length, Protection prot, MmapFlags flags, int fd, int offset);

        [DllImport(LibC, CallingConvention = CallingConvention.Cdecl, EntryPoint = "munmap")]
        public static extern unsafe int Munmap(IntPtr addr, nuint length);

        [DllImport(LibC, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mprotect")]
        public static extern unsafe int Mprotect(IntPtr addr, nuint len, Protection prot);

        [DllImport(LibC, CallingConvention = CallingConvention.Cdecl, EntryPoint = "sysconf")]
        public static extern unsafe long Sysconf(SysconfName name);

        [DllImport(LibC, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mincore")]
        public static extern unsafe int Mincore(IntPtr addr, nuint len, byte* vec);

        [DllImport(LibC, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mkstemp")]
        public static extern unsafe int MkSTemp(byte* template);

        [DllImport(LibC, CallingConvention = CallingConvention.Cdecl, EntryPoint = "__errno")]
        public static extern unsafe int* __errno();

        public static unsafe int Errno => *__errno();

        [Flags]
        public enum PipeFlags : int
        {
            CloseOnExec = 0x80000
        }

        [Flags]
        public enum Protection : int
        {
            None = 0x00,
            Read = 0x01,
            Write = 0x02,
            Execute = 0x04,
        }

        [Flags]
        public enum MmapFlags : int
        {
            Shared = 0x01,
            Private = 0x02,
            SharedValidate = 0x03,

            Fixed = 0x10,
            Anonymous = 0x20,

            GrowsDown = 0x00100,
            DenyWrite = 0x00800,
            [Obsolete("Use Protection.Execute instead", true)]
            Executable = 0x01000,
            Locked = 0x02000,
            NoReserve = 0x04000,
            Populate = 0x08000,
            NonBlock = 0x10000,
            Stack = 0x20000,
            HugeTLB = 0x40000,
            Sync = 0x80000,
            FixedNoReplace = 0x100000,
        }

        public enum SysconfName : long
        {
            // Standard POSIX sysconf values
            // These values are standardized across Linux, Android, and most Unix systems
            ArgMax = 0,
            ChildMax = 1,
            ClockTick = 2,
            NGroupsMax = 3,
            OpenMax = 4,
            StreamMax = 5,
            TZNameMax = 6,
            JobControl = 7,
            SavedIds = 8,
            RealtimeSignals = 9,
            PriorityScheduling = 10,
            Timers = 11,
            AsyncIO = 12,
            PrioritizedIO = 13,
            SynchronizedIO = 14,
            FSync = 15,
            MappedFiles = 16,
            MemLock = 17,
            MemLockRange = 18,
            MemoryProtection = 19,
            MessagePassing = 20,
            Semaphores = 21,
            SharedMemoryObjects = 22,
            AIOListIOMax = 23,
            AIOMax = 24,
            AIOPrioDeltaMax = 25,
            DelayTimerMax = 26,
            MQOpenMax = 27,
            MQPrioMax = 28,
            Version = 29,
            // _SC_PAGESIZE / _SC_PAGE_SIZE is typically 30 on most systems
            PageSize = 30,
            RTSigMax = 31,
            SemNSemsMax = 32,
            SemValueMax = 33,
            SigQueueMax = 34,
            TimerMax = 35,
        }
    }
}

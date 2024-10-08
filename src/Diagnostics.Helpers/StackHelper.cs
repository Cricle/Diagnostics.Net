﻿using Diagnostics.Helpers.Models;
using Microsoft.Diagnostics.Runtime;
using System;

namespace Diagnostics.Helpers
{
    public class LongStackCaptcher:IDisposable
    {
        public static LongStackCaptcher Create(DataTarget dataTarget, bool leaveDataTargetNoDispose = true)
        {
            return new LongStackCaptcher(dataTarget, new StackSnapshot(dataTarget.ClrVersions[0]), leaveDataTargetNoDispose);
        }

        public LongStackCaptcher(DataTarget dataTarget, StackSnapshot snapshot, bool leaveDataTargetNoDispose)
        {
            DataTarget = dataTarget;
            Snapshot = snapshot;
            Runtime = Snapshot.ClrInfo.CreateRuntime();
            LeaveDataTargetNoDispose = leaveDataTargetNoDispose;
        }

        public DataTarget DataTarget { get; }

        public StackSnapshot Snapshot { get; }

        public ClrRuntime Runtime { get; }

        public bool LeaveDataTargetNoDispose { get; }

        public RuntimeSnapshot GetSnapshot(ThreadMode threadMode= ThreadMode.Mini,int maxFrame=10)
        {
            Runtime.FlushCachedData();
            return RuntimeSnapshot.Create(Runtime, threadMode, maxFrame);
        }

        public void Dispose()
        {
            Runtime.Dispose();
            if (!LeaveDataTargetNoDispose)
            {
                DataTarget.Dispose();
            }
        }
    }
    public static class StackHelper
    {
        //https://github.com/microsoft/clrmd/blob/main/src/Samples/ClrStack/ClrStack.cs
        public static StackSnapshotCollection GetStackSnapshots()
        {
            var dt = DataTarget.CreateSnapshotAndAttach(PlatformHelper.CurrentProcessId);
            return GetStackSnapshots(dt);
        }
        public static StackSnapshotCollection GetStackSnapshots(int processId)
        {
            var dt = DataTarget.CreateSnapshotAndAttach(processId);
            return GetStackSnapshots(dt);
        }
        public static StackSnapshotCollection GetStackSnapshots(DataTarget dataTarget)
        {
            try
            {
                var isTarget64Bit = dataTarget.DataReader.PointerSize == 8;
                if (PlatformHelper.Is64Bit != isTarget64Bit)
                {
                    throw new Exception(string.Format("Architecture mismatch:  Process is {0} but target is {1}", PlatformHelper.Is64Bit ? "64 bit" : "32 bit", isTarget64Bit ? "64 bit" : "32 bit"));
                }
                var clrVersions = dataTarget.ClrVersions;
                var stacks = new StackSnapshot[clrVersions.Length];
                for (int i = 0; i < clrVersions.Length; i++)
                {
                    stacks[i] = new StackSnapshot(clrVersions[i]);
                }
                return new StackSnapshotCollection(dataTarget, stacks);
            }
            catch (Exception)
            {
                dataTarget.Dispose();
                throw;
            }
        }
    }
}
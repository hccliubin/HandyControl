﻿using System.Runtime.InteropServices;
using System.Windows.Interop;
using HandyControl.Data;
using HandyControl.Tools;

namespace HandyControl.Controls
{
    public class BlurWindow : Window
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            EnableBlur(this);
        }

        internal static void EnableBlur(Window window)
        {
            var accentPolicy = new ExternDllHelper.ACCENTPOLICY();
            var accentPolicySize = Marshal.SizeOf(accentPolicy);
            var versionInfo = CommonHelper.GetSystemVersionInfo();

            if (versionInfo >= SystemVersionInfo.Windows10_1809)
            {
                accentPolicy.AccentState = ExternDllHelper.ACCENTSTATE.ACCENT_ENABLE_ACRYLICBLURBEHIND;
            }
            else if (versionInfo >= SystemVersionInfo.Windows10)
            {
                accentPolicy.AccentState = ExternDllHelper.ACCENTSTATE.ACCENT_ENABLE_BLURBEHIND;
            }
            else
            {
                accentPolicy.AccentState = ExternDllHelper.ACCENTSTATE.ACCENT_ENABLE_TRANSPARENTGRADIENT;
            }

            accentPolicy.AccentFlags = 2;
            accentPolicy.GradientColor = ResourceHelper.GetResource<uint>("BlurGradientValue");

            var accentPtr = Marshal.AllocHGlobal(accentPolicySize);
            Marshal.StructureToPtr(accentPolicy, accentPtr, false);

            var data = new ExternDllHelper.WINCOMPATTRDATA
            {
                Attribute = ExternDllHelper.WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY,
                DataSize = accentPolicySize,
                Data = accentPtr
            };

            ExternDllHelper.SetWindowCompositionAttribute(new WindowInteropHelper(window).Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }
    }
}
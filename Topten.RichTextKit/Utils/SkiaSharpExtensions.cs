using System;
using System.Reflection;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace RichTextKit.Utils
{
    internal static class SkiaSharpExtensions
    {
        // P/Invoke declaration for the SkTypeface::makeClone method
        [DllImport("libSkiaSharp", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr sk_typeface_make_clone(IntPtr typefaceHandle, IntPtr fontArgumentsHandle);

        // Extension method for SKTypeface
        public static SKTypeface MakeClone(this SKTypeface typeface, SKFontArguments fontArguments)
        {
            if (typeface == null)
                throw new ArgumentNullException(nameof(typeface));
            if (fontArguments == null)
                throw new ArgumentNullException(nameof(fontArguments));

            IntPtr clonedTypefaceHandle = sk_typeface_make_clone(typeface.Handle, fontArguments.Handle);
            if (clonedTypefaceHandle == IntPtr.Zero)
                throw new InvalidOperationException("Failed to clone the SKTypeface.");

            // Use reflection to create an SKTypeface instance from the handle
            var constructor = typeof(SKTypeface).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] { typeof(IntPtr) },
                null);

            if (constructor == null)
                throw new InvalidOperationException("Could not find the SKTypeface constructor.");

            return (SKTypeface)constructor.Invoke(new object[] { clonedTypefaceHandle });
        }
    }

    /// <summary>
    /// Represents the font arguments for a typeface.
    /// </summary>
    public class SKFontArguments
    {
        /// <summary>
        /// Gets the handle for the font arguments.
        /// </summary>
        public IntPtr Handle { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SKFontArguments"/> class.
        /// </summary>
        /// <param name="handle"></param>
        public SKFontArguments(IntPtr handle)
        {
            Handle = handle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SKFontArguments"/> class.
        /// </summary>
        public SKFontArguments()
        {
            Handle = CreateHandle();
        }

        /// <summary>
        /// Gets or sets the variable weight of the font.
        /// </summary>
        public int Weight { get; set; } // Variable font weight

        /// <summary>
        /// Gets or sets the variable width of the font.
        /// </summary>
        public int Width { get; set; } // Variable font width

        /// <summary>
        /// Gets or sets the variable slant of the font.
        /// </summary>
        public int Slant { get; set; } // Variable font slant

        // P/Invoke declarations for SkFontArguments
        [DllImport("libSkiaSharp", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr sk_font_arguments_new();

        [DllImport("libSkiaSharp", CallingConvention = CallingConvention.Cdecl)]
        private static extern void sk_font_arguments_delete(IntPtr fontArgumentsHandle);

        [DllImport("libSkiaSharp", CallingConvention = CallingConvention.Cdecl)]
        private static extern void sk_font_arguments_set_weight(IntPtr fontArgumentsHandle, int weight);

        [DllImport("libSkiaSharp", CallingConvention = CallingConvention.Cdecl)]
        private static extern void sk_font_arguments_set_width(IntPtr fontArgumentsHandle, int width);

        [DllImport("libSkiaSharp", CallingConvention = CallingConvention.Cdecl)]
        private static extern void sk_font_arguments_set_slant(IntPtr fontArgumentsHandle, int slant);

        /// <summary>
        /// Method to create a handle for the font arguments
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private IntPtr CreateHandle()
        {
            IntPtr handle = sk_font_arguments_new();
            if (handle == IntPtr.Zero)
                throw new InvalidOperationException("Failed to create SkFontArguments.");

            sk_font_arguments_set_weight(handle, Weight);
            sk_font_arguments_set_width(handle, Width);
            sk_font_arguments_set_slant(handle, Slant);

            return handle;
        }

        /// <summary>
        /// Finalizer for the <see cref="SKFontArguments"/> class.
        /// </summary>
        ~SKFontArguments()
        {
            if (Handle != IntPtr.Zero)
            {
                sk_font_arguments_delete(Handle);
            }
        }
    }
}